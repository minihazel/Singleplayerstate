using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Singleplayerstate
{
    public partial class mainForm : Form
    {
        string currentDirectory = Environment.CurrentDirectory;
        public StringBuilder akiServerOutput;
        public string availableServers;
        public string availableAddons;
        string selectedServer = "";

        string temporaryAID;

        // BackgroundWorkers
        public BackgroundWorker CheckServerStatus;
        public BackgroundWorker TarkovProcessDetector;
        public BackgroundWorker TarkovEndDetector;
        public BackgroundWorker AkiServerDetector;

        // Booleans
        bool serverHasBeenSelected = false;
        bool isEditingInstall = false;
        bool hasStopped = false;
        bool hasNotifiedUser = false;
        bool serverIsRunning = false;
        bool firstServerNotify = false;
        bool ranViaHotkey = false;

        // Dictionaries for servers and addons
        public Dictionary<string, string> folderPaths = new Dictionary<string, string>();
        public Dictionary<string, string> addonPaths = new Dictionary<string, string>();

        // Default colors
        public Color hoverColor = Color.FromArgb(39, 44, 47);
        public Color holdColor = Color.FromArgb(39, 44, 47);

        public mainForm()
        {
            InitializeComponent();
            KeyDown += mainForm_KeyDown;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            folderPaths.Add("Placeholder", "Placeholder");
            addonPaths.Add("Placeholder", "Placeholder");

            availableServers = Properties.Settings.Default.availableServers;
            availableAddons = Properties.Settings.Default.availableAddons;

            if (string.IsNullOrWhiteSpace(availableServers) || availableServers == null)
            {
                Properties.Settings.Default.availableServers = "{}";
                Properties.Settings.Default.Save();
            }

            if (string.IsNullOrWhiteSpace(availableAddons) || availableAddons == null)
            {
                Properties.Settings.Default.availableAddons = "{}";
                Properties.Settings.Default.Save();
            }

            availableServers = Properties.Settings.Default.availableServers;
            folderPaths = JsonSerializer.Deserialize<Dictionary<string, string>>(availableServers);

            availableAddons = Properties.Settings.Default.availableAddons;
            addonPaths = JsonSerializer.Deserialize<Dictionary<string, string>>(availableAddons);

            listServers();

            txtSetDisplayName.Clear();
            btnAddInstall.PerformClick();
            chkAutoScroll.Checked = Properties.Settings.Default.autoScrollOption;

            if (Properties.Settings.Default.launchParameter == "donothing")
                btnWhenSPTAKILauncher.Text = "Do nothing";
            else if (Properties.Settings.Default.launchParameter == "minimize")
                btnWhenSPTAKILauncher.Text = "Minimize launcher";
            else if (Properties.Settings.Default.launchParameter == "tray")
                btnWhenSPTAKILauncher.Text = "Minimize to tray";
            else
                btnWhenSPTAKILauncher.Text = "View server tab";

            if (Properties.Settings.Default.exitParameter == "displaylauncher")
                btnWhenSPTAKIExits.Text = "Display launcher";
            else if (Properties.Settings.Default.exitParameter == "closelauncher")
                btnWhenSPTAKIExits.Text = "Close launcher";
            else
                btnWhenSPTAKIExits.Text = "Do nothing";

            if (Properties.Settings.Default.lastServer != null)
                fetchLastServer();
        }

        private void HandleDelete()
        {
            foreach (Label label in panelServers.Controls.OfType<Label>())
            {
                if (label.Padding == new Padding(10, 0, 0, 0) &&
                    selectedServer == label.Name)
                {
                    btnRemoveInstall.PerformClick();
                }
            }
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                HandleDelete();
            }

            if (e.KeyCode == Keys.F5)
            {
                ranViaHotkey = true;
                btnPlaySPTAKI.PerformClick();
            }
        }

        // ASYNC
        private async void enterInputMode(bool enter, string path)
        {
            if (enter)
            {
                await Task.Delay(350);

                btnBrowseForFolder.Size = new Size(579, 40);
                titleSetDisplayName.Visible = true;
                panelSetDisplayName.Visible = true;
                btnSetDisplayName.Visible = true;
                btnCancelProcess.Visible = true;
                btnBrowseForFolder.Text = path;

                txtSetDisplayName.Select();
            }
            else
            {
                btnBrowseForFolder.Size = new Size(300, 40);
                titleSetDisplayName.Visible = false;
                panelSetDisplayName.Visible = false;
                btnSetDisplayName.Visible = false;
                btnCancelProcess.Visible = false;
                btnBrowseForFolder.Text = "Browse...";
            }
        }

        // ASYNC
        private async void editGameInstall(string displayName, string oldInstall)
        {
            var browse = new BetterFolderBrowser();
            browse.Title = "Select folder that contains SPT-AKI";
            browse.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            browse.Multiselect = false;

            if (browse.ShowDialog(this) == DialogResult.OK)
            {
                string selectedFolder = browse.SelectedFolder;
                bool folderExists = Directory.Exists(selectedFolder);
                if (folderExists)
                {
                    if (folderPaths != null)
                    {
                        folderPaths[displayName] = selectedFolder;
                        string serializedPaths = JsonSerializer.Serialize(folderPaths);
                        Properties.Settings.Default.availableServers = serializedPaths;
                        Properties.Settings.Default.Save();

                        showMessage($"Updated folder:\n{oldInstall}\n\nto:\n{selectedFolder}");
                        listServers();

                        foreach (Control c in panelServers.Controls)
                        {
                            if (c is Label lbl)
                            {
                                if (lbl.Text.Contains(displayName))
                                {
                                    selectServer(lbl.Text, lbl, false);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void fetchLastServer()
        {
            if (Properties.Settings.Default.lastServer != null)
            {
                foreach (Control c in panelServers.Controls)
                {
                    if (c is Label lbl)
                    {
                        string cleanLbl = lbl.Text.Replace("✔️ ", "");

                        if (Properties.Settings.Default.lastServer == cleanLbl)
                        {
                            clickServer(lbl, true);
                        }
                    }
                }
            }
        }

        private string fetchCurrentServer()
        {
            if (selectedServer != null)
            {
                Control selectedLbl = panelServers.Controls.Find(selectedServer, false).FirstOrDefault();
                if (selectedLbl != null && selectedLbl is Label)
                {
                    string server = selectedLbl.Text.Replace("✔️ ", "");
                    return server;
                }
            }

            return null;
        }

        private void showMessage(string message)
        {
            MessageBox.Show(message, this.Text, MessageBoxButtons.OK);
        }

        public void saveServers(string displayName, string folderPath)
        {
            try
            {
                if (folderPaths != null)
                {
                    folderPaths.Add(displayName, folderPath);
                }
                else
                {
                    folderPaths = new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
            }

            string serializedPaths = JsonSerializer.Serialize(folderPaths);
            Properties.Settings.Default.availableServers = serializedPaths;
            Properties.Settings.Default.Save();
            enterInputMode(false, null);
            listServers();
        }

        public void saveServer(string displayName, string folderPath, string oldInstall)
        {
            try
            {
                if (folderPaths != null)
                {
                    folderPaths[displayName] = folderPath;
                }
                else
                {
                    folderPaths = new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
            }

            string serializedPaths = JsonSerializer.Serialize(folderPaths);
            Properties.Settings.Default.availableServers = serializedPaths;
            Properties.Settings.Default.Save();

            showMessage($"SPT-AKI installation {displayName} changed to folder {folderPath}!");
            enterInputMode(false, null);
            listServers();
        }

        public void saveAddon(string displayName, string addonPath)
        {
            addonPath = addonPath.Replace(Environment.NewLine, "").Replace("\r", "");

            try
            {
                if (addonPaths != null)
                    addonPaths[displayName] = addonPath;
                else
                    addonPaths = new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
            }

            string serializedPaths = JsonSerializer.Serialize(addonPaths);
            Properties.Settings.Default.availableAddons = serializedPaths;
            Properties.Settings.Default.Save();

            listAddons();
        }

        public void deselectAllServers()
        {
            foreach (Control c in panelServers.Controls)
            {
                if (c is Label)
                {
                    c.BackColor = panelServers.BackColor;
                }
            }
        }

        private void editGameProfile(string account, string gamePath)
        {
            string oldUser = account;
            string mainDir = gamePath;

            string userFolder = Path.Combine(mainDir, "user");
            string profilesFolder = Path.Combine(userFolder, "profiles");
            string fullProfilePath = Path.Combine(profilesFolder, $"{txtAccountAID.Text}.json");

            if (File.Exists(fullProfilePath))
            {
                string fileContent = File.ReadAllText(fullProfilePath);
                JObject parsedFile = JObject.Parse(fileContent);
                JObject info = (JObject)parsedFile["info"];
                string infoAID = (string)info["id"];

                JObject characters = (JObject)parsedFile["characters"];
                JObject pmc = (JObject)characters["pmc"];
                JObject Info = (JObject)pmc["Info"];

                if (infoAID == txtAccountAID.Text)
                {
                    Info["Nickname"] = txtUsername.Text;
                    string updatedJSON = JsonConvert.SerializeObject(parsedFile, Formatting.Indented);

                    try
                    {
                        File.WriteAllText(fullProfilePath, updatedJSON);
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                Environment.NewLine +
                                Environment.NewLine +
                                ex.ToString());
                    }

                    btnSelectAccount.Text = txtUsername.Text;
                    panelAccountProfiles.Visible = false;
                    panelAccountSeparator.Visible = false;
                    showMessage($"Old username {oldUser} changed to {txtUsername.Text}!");
                }
            }
        }

        // STATIC
        public static void arrInsert(ref string[] array, string item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
        }

        // STATIC
        public static void arrRemove(ref string[] array, string item)
        {
            int index = Array.IndexOf(array, item);

            if (index != -1)
            {
                for (int i = index; i < array.Length - 1; i++)
                {
                    array[i] = array[i + 1];
                }

                Array.Resize(ref array, array.Length - 1);
            }
        }

        private string convertProfile(string AID)
        {
            string cleanAID = Path.GetFileNameWithoutExtension(AID);

            string mainDir = txtGameInstallFolder.Text;
            string userFolder = Path.Combine(mainDir, "user");
            string profilesFolder = Path.Combine(userFolder, "profiles");
            bool profilesFolderExists = Directory.Exists(profilesFolder);
            if (profilesFolderExists)
            {
                string fullAID = Path.Combine(profilesFolder, $"{cleanAID}.json");
                bool fullAIDExists = File.Exists(fullAID);
                if (fullAIDExists)
                {
                    string fileContent = File.ReadAllText(fullAID);
                    JObject parsedFile = JObject.Parse(fileContent);
                    JObject characters = (JObject)parsedFile["characters"];
                    JObject info = (JObject)parsedFile["info"];

                    if (characters.Type != JTokenType.Null)
                    {
                        JObject pmc = (JObject)characters["pmc"];

                        if (pmc.Type != JTokenType.Null)
                        {
                            JObject Info = (JObject)pmc["Info"];
                            if (Info != null)
                            {
                                string Nickname = (string)Info["Nickname"];
                                string infoAID = (string)info["id"];

                                if (infoAID == cleanAID)
                                {
                                    return Nickname;
                                }
                            }
                        }
                    }
                }
            }

            return "Incomplete profile";
        }

        private string findAID(string displayName)
        {
            if (displayName.ToLower() == "incomplete profile" || displayName.ToLower() == "")
            {
                return "Incomplete profile";
            }
            else
            {
                string mainDir = txtGameInstallFolder.Text;
                string userFolder = Path.Combine(mainDir, "user");
                string profilesFolder = Path.Combine(userFolder, "profiles");
                bool profilesFolderExists = Directory.Exists(profilesFolder);
                if (profilesFolderExists)
                {
                    string[] profiles = Directory.GetFiles(profilesFolder, "*.json");

                    foreach (string profile in profiles)
                    {
                        string AID = Path.GetFileNameWithoutExtension(profile);

                        string fileContent = File.ReadAllText(profile);
                        JObject parsedFile = JObject.Parse(fileContent);
                        JObject info = (JObject)parsedFile["info"];
                        string infoAID = (string)info["id"];

                        JObject characters = (JObject)parsedFile["characters"];
                        JObject pmc = (JObject)characters["pmc"];

                        if (pmc != null)
                        {
                            JObject Info = (JObject)pmc["Info"];
                            if (Info != null)
                            {
                                string GameVersion = (string)Info["GameVersion"];
                                if (GameVersion.ToLower() == "standard")
                                    GameVersion = "Standard Edition";
                                else if (GameVersion.ToLower() == "left_behind")
                                    GameVersion = "Left Behind Edition";
                                else if (GameVersion.ToLower() == "prepare_for_darkness")
                                    GameVersion = "Prepare for Darkness Edition";
                                else if (GameVersion.ToLower() == "edge_of_darkness")
                                    GameVersion = "Edge of Darkness Edition";

                                string Nickname = (string)Info["Nickname"];

                                if (Nickname == displayName)
                                {
                                    infoGameEdition.Text = GameVersion;
                                    return AID;
                                }
                            }
                        }
                        else
                        {
                            return "Incomplete profile";
                        }
                    }
                }
            }

            return null;
        }

        // STATIC
        public static bool IsAkiServerRunning(string expectedFilePath)
        {
            Process[] processes = Process.GetProcessesByName("Aki.Server");

            foreach (Process process in processes)
            {
                try
                {
                    if (process.MainModule != null && process.MainModule.FileName.Equals(expectedFilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
                }
            }

            // Aki.Server process not found or path mismatch
            return false;
        }

        private void listServers()
        {
            panelServers.Controls.Clear();
            string[] servers = new string[0];

            foreach (var entry in folderPaths)
            {
                arrInsert(ref servers, entry.Key.ToString());
            }

            for (int i = 0; i < servers.Length; i++)
            {
                Label lbl = new Label();
                lbl.Name = $"listedServer{i}";
                lbl.AutoSize = false;
                lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Size = new Size(serverPlaceholder.Size.Width, serverPlaceholder.Size.Height);
                lbl.Location = new Point(serverPlaceholder.Location.X, serverPlaceholder.Location.Y + (i * serverPlaceholder.Size.Height));
                lbl.Font = new Font("Bender", 13, FontStyle.Bold);
                lbl.BackColor = panelServers.BackColor;
                lbl.ForeColor = Color.LightGray;
                lbl.Padding = new Padding(0, 0, 0, 0);
                lbl.Cursor = Cursors.Hand;
                lbl.MouseEnter += new EventHandler(lbl_MouseEnter);
                lbl.MouseLeave += new EventHandler(lbl_MouseLeave);
                lbl.MouseDown += new MouseEventHandler(lbl_MouseDown);
                lbl.MouseDoubleClick += new MouseEventHandler(lbl_MouseDoubleClick);
                lbl.MouseUp += new MouseEventHandler(lbl_MouseUp);
                lbl.Paint += new PaintEventHandler(lbl_Paint);

                lbl.Text = $"✔️ {servers[i]}";
                panelServers.Controls.Add(lbl);
            }

            int servercount = panelServers.Controls.OfType<Label>().Count();
            if (panelServers.Controls["serverPlaceholder"] != null) servercount -= 1;

            if (servercount == 1)
            {
                Control firstServer = panelServers.Controls["listedServer0"];
                if (firstServer != null)
                {
                    clickServer(firstServer, true);
                }
            }
        }

        private void listProfiles()
        {
            panelAccountProfiles.Controls.Clear();
            panelAccountProfiles.Visible = true;
            panelAccountSeparator.Visible = true;

            string mainDir = txtGameInstallFolder.Text;
            string userFolder = Path.Combine(mainDir, "user");
            string profilesFolder = Path.Combine(userFolder, "profiles");

            string[] profiles = Directory.GetFiles(profilesFolder, "*.json");

            for (int i = 0; i < profiles.Length; i++)
            {
                Label lbl = new Label();
                lbl.Name = $"accountProfile{i}";
                lbl.AutoSize = false;
                lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Size = new Size(profilesPlaceholder.Size.Width, profilesPlaceholder.Size.Height);
                lbl.Location = new Point(profilesPlaceholder.Location.X, profilesPlaceholder.Location.Y + (i * profilesPlaceholder.Size.Height));
                lbl.Font = new Font("Bender", 13, FontStyle.Bold);
                lbl.BackColor = panelAccountProfiles.BackColor;
                lbl.ForeColor = Color.LightGray;
                lbl.Padding = new Padding(10, 0, 0, 0);
                lbl.Cursor = Cursors.Hand;
                lbl.MouseEnter += new EventHandler(profile_MouseEnter);
                lbl.MouseLeave += new EventHandler(profile_MouseLeave);
                lbl.MouseDown += new MouseEventHandler(profile_MouseDown);
                lbl.MouseUp += new MouseEventHandler(profile_MouseUp);

                string convertedProfile = convertProfile(profiles[i]);
                if (convertedProfile != null || convertedProfile != "")
                {
                    lbl.Text = $"✔️ {convertProfile(profiles[i])}";
                }
                else
                {
                    lbl.Text = $"✔️ Incomplete profile";
                }

                panelAccountProfiles.Controls.Add(lbl);
            }
        }

        public void listAddons()
        {
            panelAddons.Controls.Clear();

            string[] addons = new string[0];
            arrInsert(ref addons, "Create new addon");

            foreach (var entry in addonPaths)
            {
                arrInsert(ref addons, entry.Key.ToString());
            }

            for (int i = 0; i < addons.Length; i++)
            {
                Label lbl = new Label();
                lbl.Name = $"addonItem{i}";
                lbl.AutoSize = false;
                lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Size = new Size(addonPlaceholder.Size.Width, addonPlaceholder.Size.Height);
                lbl.Location = new Point(addonPlaceholder.Location.X, addonPlaceholder.Location.Y + (i * addonPlaceholder.Size.Height));
                lbl.Font = new Font("Bender", 13, FontStyle.Bold);
                lbl.BackColor = panelAddons.BackColor;
                lbl.ForeColor = Color.LightGray;
                lbl.Padding = new Padding(10, 0, 0, 0);
                lbl.Cursor = Cursors.Hand;
                lbl.MouseEnter += new EventHandler(addon_MouseEnter);
                lbl.MouseLeave += new EventHandler(addon_MouseLeave);
                lbl.MouseDown += new MouseEventHandler(addon_MouseDown);
                lbl.MouseUp += new MouseEventHandler(addon_MouseUp);

                if (addons[i].ToLower() == "create new addon")
                    lbl.Text = addons[i].Replace("✔️ ", "");
                else
                    lbl.Text = $"✔️ {addons[i]}";

                panelAddons.Controls.Add(lbl);
            }
        }

        private void addon_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (label.Text.ToLower().Contains("create new addon"))
                {
                    AddonCreate frm = new AddonCreate(this, panelAddons, folderPaths, availableAddons);
                    frm.Show();

                    panelAddons.Visible = false;
                    panelAddonSeparator.Location = new Point(314, 0);
                    panelAddonSeparator.Visible = false;

                    toggleUI(false);
                }
                else
                {
                    string cleanItem = label.Text.Replace("✔️ ", "");
                    if ((Control.MouseButtons & MouseButtons.Right) != 0)
                    {
                        if (MessageBox.Show($"Would you like to remove addon {cleanItem}? This will not delete the path.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (addonPaths.ContainsKey(cleanItem))
                            {
                                addonPaths.Remove(cleanItem);
                                string serializedPaths = JsonSerializer.Serialize(addonPaths);
                                Properties.Settings.Default.availableAddons = serializedPaths;
                                Properties.Settings.Default.Save();

                                listAddons();
                            }
                        }
                    }
                    else
                    {
                        foreach (var entry in addonPaths)
                        {
                            if (entry.Key.ToString() == cleanItem)
                            {
                                string fullPath = entry.Value.ToString().Replace("\\", "/").Replace(Environment.NewLine, "").Replace("\r", "");

                                ProcessStartInfo newApp = new ProcessStartInfo();

                                try
                                {
                                    newApp.WorkingDirectory = Path.GetDirectoryName(fullPath);
                                }
                                catch (Exception ex)
                                {
                                    if (ex is ArgumentException && ex.Message.ToLower().Contains("illegal characters in path"))
                                    {
                                        fullPath = fullPath.Replace(Environment.NewLine, "").Replace("\r", "");
                                    }
                                }

                                newApp.FileName = Path.GetFileName(fullPath);
                                newApp.UseShellExecute = true;
                                newApp.Verb = "open";
                                Process.Start(newApp);

                                toggleAddonView(false);
                            }
                        }
                    }
                }
            }
        }

        private void addon_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.ForeColor = Color.Gray;
            }
        }

        private void addon_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.ForeColor = Color.LightGray;
                label.Invalidate();
            }
        }

        private void addon_MouseUp(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.BackColor = panelAddons.BackColor;
                label.Invalidate();
            }
        }

        private void profile_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                string cleanProfile = label.Text.Replace("✔️ ", "");

                string aidFound = findAID(cleanProfile.TrimEnd());

                if (aidFound.ToLower().Contains("incomplete profile"))
                {
                    btnSelectAccount.Text = "Incomplete profile";
                    txtUsername.Text = "Incomplete profile";
                    txtAccountAID.Text = aidFound;
                    btnSetUsername.Enabled = false;
                }
                else
                {
                    btnSelectAccount.Text = cleanProfile;
                    txtUsername.Text = cleanProfile;
                    txtAccountAID.Text = aidFound;
                    btnSetUsername.Enabled = true;
                }

                panelAccountProfiles.Visible = false;
                panelAccountSeparator.Visible = false;
            }
        }

        private void profile_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.ForeColor = Color.Gray;
            }
        }

        private void profile_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.ForeColor = Color.LightGray;
                label.Invalidate();
            }
        }

        private void profile_MouseUp(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.BackColor = panelServers.BackColor;
                label.Invalidate();
            }
        }

        private void displayInfo(string path)
        {
            txtLocalCache.Text = $"❌ user\\cache, server will load slower!";
            txtLoadOrderEditor.Text = $"❌ user\\mods\\Load Order Editor.exe";
            txtLOEPath.Text = $"❌ user\\mods\\Load Order Editor.exe";
            txtServerMods.Text = $"❌ user\\mods";
            txtClientMods.Text = $"❌ BepInEx\\plugins";
            infoServer.Text = $"Regular SPT-AKI (offline)";

            gameRequirementServer.Text = $"✔️ Aki.Server not found";
            gameRequirementServer.ForeColor = Color.Red;
            gameRequirementLauncher.Text = $"✔️ Aki.Launcher not found";
            gameRequirementLauncher.ForeColor = Color.Red;
            gameRequirementEFT.Text = $"✔️ Escape From Tarkov not found";
            gameRequirementEFT.ForeColor = Color.Red;
            extensionsRequirementLOE.Text = $"❌ Load Order Editor not found [Click here to download]";
            extensionsRequirementLOE.ForeColor = Color.Red;

            btnSelectAccount.Text = "None selected";
            txtUsername.Clear();
            btnPlaySPTAKI.Enabled = false;

            string mainDir = path;

            string userFolder = Path.Combine(mainDir, "user");
            string profilesFolder = Path.Combine(userFolder, "profiles");
            string[] profiles = Directory.GetFiles(profilesFolder, "*.json");
            string akiServerFile = Path.Combine(mainDir, "Aki.Server.exe");
            string akiLauncherFile = Path.Combine(mainDir, "Aki.Launcher.exe");
            string EFTFile = Path.Combine(mainDir, "EscapeFromTarkov.exe");

            string akiDataFolder = Path.Combine(mainDir, "Aki_Data");
            string akiServerFolder = Path.Combine(akiDataFolder, "Server");
            string akiConfigsFolder = Path.Combine(akiServerFolder, "configs");
            string akiCoreJSON = Path.Combine(akiConfigsFolder, "core.json");

            string BepInFolder = Path.Combine(path, "BepInEx");
            string pluginsFolder = Path.Combine(BepInFolder, "plugins");
            string SAINClient = Path.Combine(pluginsFolder, "SAIN");

            string cacheFolder = Path.Combine(userFolder, "cache");
            string modsFolder = Path.Combine(userFolder, "mods");
            string LOEPath = Path.Combine(modsFolder, "Load Order Editor.exe");
            string SAINServer = Path.Combine(modsFolder, "zSolarint-SAIN-ServerMod");

            // SPTAKI Tab
            if (Directory.Exists(cacheFolder))
                txtLocalCache.Text = $"✔️ user\\cache";

            if (File.Exists(LOEPath))
            {
                txtLOEPath.Text = LOEPath;

                txtLoadOrderEditor.Text = $"✔️ user\\mods\\Load Order Editor.exe";
                extensionsRequirementLOE.Text = $"✔️ Load Order Editor found";
                extensionsRequirementLOE.ForeColor = Color.SeaGreen;

                string order = Path.Combine(modsFolder, "order.json");
                bool orderExists = File.Exists(order);
                if (orderExists)
                {
                    string allMods = File.ReadAllText(order);
                    JObject orderMods = JObject.Parse(allMods);

                    JArray orderArray = (JArray)orderMods["order"];
                    if (orderArray != null)
                    {
                        int serverModsCount = orderArray.Count;
                        btnServerMods.Text = $"Server mods - {serverModsCount.ToString()}";
                    }
                }
                else
                {
                    int serverModsCount = 0;
                    string[] serverMods = Directory.GetDirectories(modsFolder);
                    foreach (string serverMod in serverMods)
                    {
                        string packageFile = Path.Combine(serverMod, "package.json");
                        bool packageFileExists = File.Exists(packageFile);
                        if (packageFileExists)
                            serverModsCount++;
                    }
                    btnServerMods.Text = $"Server mods - {serverModsCount.ToString()}";
                }
            }

            if (Directory.Exists(modsFolder))
                txtServerMods.Text = $"✔️ user\\mods";

            if (Directory.Exists(pluginsFolder))
                txtClientMods.Text = $"✔️ BepInEx\\plugins";

            if (Directory.Exists(SAINClient) && Directory.Exists(SAINServer))
                infoServer.Text = $"InSAIN mode (offline)";

            if (File.Exists(akiCoreJSON))
            {
                string akiCoreContent = File.ReadAllText(akiCoreJSON);
                JsonDocument doc = JsonDocument.Parse(akiCoreContent);

                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("compatibleTarkovVersion", out JsonElement compatibleTarkovVersion))
                {
                    string version = compatibleTarkovVersion.GetString();
                    infoGameVersion.Text = version;
                }
            }

            // Game Options Tab
            txtGameInstallFolder.Text = path;

            if (File.Exists(akiServerFile))
            {
                gameRequirementServer.Text = $"✔️ Aki.Server found";
                gameRequirementServer.ForeColor = Color.SeaGreen;
            }
            if (File.Exists(akiLauncherFile))
            {
                gameRequirementLauncher.Text = $"✔️ Aki.Launcher found";
                gameRequirementLauncher.ForeColor = Color.SeaGreen;
            }
            if (File.Exists(EFTFile))
            {
                gameRequirementEFT.Text = $"✔️ Escape From Tarkov found";
                gameRequirementEFT.ForeColor = Color.SeaGreen;
            }

            // Account Tab
            string firstProfile = convertProfile(profiles[0]);
            btnSelectAccount.Text = firstProfile;
            txtUsername.Text = firstProfile;
            txtAccountAID.Text = findAID(btnSelectAccount.Text);

            if (File.Exists(akiServerFile) && File.Exists(akiLauncherFile) && File.Exists(EFTFile))
                btnPlaySPTAKI.Enabled = true;
            else
                btnPlaySPTAKI.Enabled = false;

            // Checking local server status
            bool serverOn = IsAkiServerRunning(akiServerFile);
            if (serverOn)
            {
                showMessage("The server for this installation seems to be running already. Go ahead and hit Play!");
            }
        }

        private void clickServer(Control label, bool autoClick)
        {
            foreach (Control c in panelServers.Controls)
            {
                if (c is Label lbl && c.Name != label.Name)
                {
                    lbl.Invalidate();
                    lbl.Padding = new Padding(0, 0, 0, 0);
                }
            }

            label.Invalidate();
            label.BackColor = panelServers.BackColor;
            label.Padding = new Padding(10, 0, 0, 0);

            selectedServer = label.Name;
            selectServer(label.Text, label, autoClick);
            toggleAddonView(Properties.Settings.Default.addonPanelVisible);
        }

        private void selectServer(string displayName, Control c, bool autoClick)
        {
            deselectAllServers();
            string cleanOutput = c.Text.Replace("✔️ ", "");

            if (folderPaths != null)
            {
                foreach (var kvp in folderPaths)
                {
                    if (kvp.Key == cleanOutput)
                    {
                        string serverPath = kvp.Value;
                        bool doesServerExist = Directory.Exists(serverPath);

                        if (doesServerExist)
                        {
                            panelAccountProfiles.Controls.Clear();
                            panelAccountProfiles.Visible = false;
                            panelAccountSeparator.Visible = false;
                            btnSetUsername.Enabled = true;

                            displayInfo(serverPath);

                            if (!serverHasBeenSelected)
                            {
                                serverHasBeenSelected = true;
                            }

                            Properties.Settings.Default.lastServer = cleanOutput;
                            Properties.Settings.Default.Save();

                            if (autoClick)
                                btnSPTAKI.PerformClick();
                        }
                        else
                        {
                            showMessage("Unfortunately, it appears that this folder does not exist. Please restart the launcher and try again.");
                        }

                        break;
                    }
                }
            }
        }

        private void lbl_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (!serverHasBeenSelected)
                    clickServer(label, true);
                else
                    clickServer(label, false);
            }
        }

        private void lbl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                ranViaHotkey = true;
                btnPlaySPTAKI.PerformClick();
            }
        }

        private void lbl_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.ForeColor = Color.Gray;
            }
        }

        private void lbl_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.ForeColor = Color.LightGray;
                label.Invalidate();
            }
        }

        private void lbl_MouseUp(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.BackColor = panelServers.BackColor;
                label.Invalidate();
            }
        }

        private void lbl_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.Invalidate();
            }
        }

        private void lbl_Paint(object sender, PaintEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "" && label != null)
            {
                if (label.Name == selectedServer)
                {
                    float gradientWidth = label.Width / 1.25f; // Set the width to stop halfway
                    Rectangle gradientRect = new Rectangle(0, 0, (int)gradientWidth, label.Height);
                    Color startColor = Color.FromArgb(255, serverPlaceholder.BackColor); // 104 represents a specific opacity
                    Color endColor = Color.FromArgb(0, serverPlaceholder.BackColor); // 0 represents fully transparent

                    using (LinearGradientBrush brush = new LinearGradientBrush(gradientRect, startColor, endColor, LinearGradientMode.Horizontal))
                    {
                        e.Graphics.FillRectangle(brush, gradientRect);
                    }

                    using (Pen pen = new Pen(Color.FromArgb(255, 168, 191, 202), 3))
                    {
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, label.Height));
                    }
                }
            }
        }

        private void btnBrowseForFolder_Click(object sender, EventArgs e)
        {
            var browse = new BetterFolderBrowser();
            browse.Title = "Select folder that contains SPT-AKI";
            browse.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            browse.Multiselect = false;

            if (browse.ShowDialog(this) == DialogResult.OK)
            {
                string selectedFolder = browse.SelectedFolder;
                bool folderExists = Directory.Exists(selectedFolder);
                if (folderExists)
                {
                    enterInputMode(true, selectedFolder);
                }
            }
        }

        private void btnCancelProcess_Click(object sender, EventArgs e)
        {
            enterInputMode(false, null);
            lblServers.Select();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (serverIsRunning && !hasStopped)
                {
                    DialogResult result = MessageBox.Show("Aki's server is running, are you sure you want to exit?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        string findServer = fetchCurrentServer();
                        if (findServer != null)
                        {
                            Properties.Settings.Default.lastServer = findServer;
                        }

                        Properties.Settings.Default.addonPanelVisible = panelAddons.Visible;
                        Properties.Settings.Default.Save();

                        Application.Exit();
                    }
                }
            }
        }

        private void txtSetDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                btnSetDisplayName.PerformClick();
            }
        }

        private void btnSetDisplayName_Click(object sender, EventArgs e)
        {
            if (btnBrowseForFolder.Text != null && txtSetDisplayName.Text != null)
            {
                if (isEditingInstall)
                {
                    isEditingInstall = false;


                    string folderPath = btnBrowseForFolder.Text;
                    string displayName = txtSetDisplayName.Text;

                    if (txtSetDisplayName.Text == "")
                    {
                        string completed = Path.GetFileName(folderPath);
                        displayName = completed;
                    }

                    string oldInstall = txtGameInstallFolder.Text;

                    saveServer(displayName, folderPath, oldInstall);
                }
                else
                {
                    string folderPath = btnBrowseForFolder.Text;
                    string displayName = txtSetDisplayName.Text;

                    if (txtSetDisplayName.Text == "")
                    {
                        string completed = Path.GetFileName(folderPath);
                        displayName = completed;
                    }

                    saveServers(displayName, folderPath);
                }
            }

            txtSetDisplayName.Clear();
            lblServers.Select();
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            int serverCount = folderPaths.Count;

            if (serverCount == 0)
            {
                showMessage("There are no AKI folders available, please browse for one.");
            }
            else if (serverCount == 1)
            {
                if (MessageBox.Show($"Are you sure you want to clear {serverCount} folder? This will not delete them from your computer.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    folderPaths.Clear();
                    Properties.Settings.Default.availableServers = "{}";
                    Properties.Settings.Default.Save();

                    btnAddInstall.PerformClick();
                    serverHasBeenSelected = false;
                }
            }
            else if (serverCount > 1)
            {
                if (MessageBox.Show($"Are you sure you want to clear {serverCount} folders? This will not delete them from your computer.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    folderPaths.Clear();
                    Properties.Settings.Default.availableServers = "{}";
                    Properties.Settings.Default.Save();

                    btnAddInstall.PerformClick();
                    serverHasBeenSelected = false;
                }
            }

            lblServers.Select();
        }

        private void mainTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
        }

        private void txtGameInstallFolder_MouseEnter(object sender, EventArgs e)
        {
            txtGameInstallFolder.ForeColor = Color.DodgerBlue;
        }

        private void txtGameInstallFolder_MouseLeave(object sender, EventArgs e)
        {
            txtGameInstallFolder.ForeColor = Color.Gray;
        }

        private void txtGameInstallFolder_Click(object sender, EventArgs e)
        {
            string displayName = "";
            string oldInstall = txtGameInstallFolder.Text;

            foreach (Control c in panelServers.Controls)
            {
                if (c is Label && c.BackColor == serverPlaceholder.BackColor)
                {
                    displayName = c.Text.Replace("✔️ ", "");
                    break;
                }
            }

            editGameInstall(displayName, oldInstall);
        }

        private void btnRemoveInstall_Click(object sender, EventArgs e)
        {
            Control selectedControl = panelServers.Controls.Find(selectedServer, false).FirstOrDefault();
            string displayName = selectedControl.Text.Replace("✔️ ", "").TrimEnd();

            if (folderPaths != null)
            {
                if (folderPaths.ContainsKey(displayName))
                {
                    if (MessageBox.Show($"Remove installation {displayName}?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        folderPaths.Remove(displayName);
                        string serializedPaths = JsonSerializer.Serialize(folderPaths);
                        Properties.Settings.Default.availableServers = serializedPaths;
                        Properties.Settings.Default.Save();

                        if (folderPaths.Count < 1)
                        {
                            listServers();
                            btnAddInstall.PerformClick();
                            serverHasBeenSelected = false;
                        }
                        else
                        {
                            listServers();

                            Control lbl = panelServers.Controls.Find("listedServer0", false).FirstOrDefault();
                            if (lbl != null)
                            {
                                clickServer(lbl, true);
                            }
                        }
                    }
                }
            }
            lblServers.Select();
        }

        private void btnClearLocalCache_Click(object sender, EventArgs e)
        {
            if (txtLocalCache.Text.StartsWith("✔️"))
            {
                string mainDir = txtGameInstallFolder.Text;
                string userFolder = Path.Combine(mainDir, "user");
                string cacheFolder = Path.Combine(userFolder, "cache");

                if (Directory.Exists(cacheFolder))
                {
                    try
                    {
                        Directory.Delete(cacheFolder, true);
                        txtLocalCache.Text = "❌ user\\cache";
                        showMessage("Cache deleted!");
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine+
                                    ex.ToString());
                    }
                }
            }
            lblServers.Select();
        }

        private void btnLOE_Click(object sender, EventArgs e)
        {
            if (txtLoadOrderEditor.Text.StartsWith("✔️"))
            {
                string mainDir = txtGameInstallFolder.Text;
                string userFolder = Path.Combine(mainDir, "user");
                string modsFolder = Path.Combine(userFolder, "mods");
                string LOEFile = Path.Combine(modsFolder, "Load Order Editor.exe");

                if (Directory.Exists(modsFolder))
                {
                    if (File.Exists(LOEFile))
                    {
                        try
                        {
                            string fullPath = LOEFile;
                            ProcessStartInfo newApp = new ProcessStartInfo();
                            newApp.WorkingDirectory = Path.GetDirectoryName(fullPath);
                            newApp.FileName = Path.GetFileName(fullPath);
                            newApp.UseShellExecute = true;
                            newApp.Verb = "open";

                            Process.Start(newApp);
                        }
                        catch (Exception ex)
                        {
                            showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                        Environment.NewLine +
                                        Environment.NewLine +
                                        ex.ToString());
                        }
                    }
                }
            }
            lblServers.Select();
        }

        private void btnServerMods_Click(object sender, EventArgs e)
        {
            /*
            Label _Pnl = new Label();
            _Pnl.Name = "messageNotice";
            _Pnl.BackColor = Color.FromArgb(90, 0, 0, 0);
            _Pnl.ForeColor = Color.LightGray;
            _Pnl.AutoSize = false;
            _Pnl.TextAlign = ContentAlignment.MiddleCenter;
            _Pnl.Font = new Font("Bender", 13, FontStyle.Bold);
            _Pnl.Size = new Size(this.Size.Width, this.Size.Height / 2);
            _Pnl.Location = new Point(0, this.Size.Width / 8);
            _Pnl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            _Pnl.Text = "Hello world!";

            this.Controls.Add(_Pnl);
            _Pnl.BringToFront();
            */

            if (txtServerMods.Text.StartsWith("✔️"))
            {
                string mainDir = txtGameInstallFolder.Text;
                string userFolder = Path.Combine(mainDir, "user");
                string modsFolder = Path.Combine(userFolder, "mods");

                if (Directory.Exists(modsFolder))
                {
                    try
                    {
                        ProcessStartInfo newApp = new ProcessStartInfo();
                        newApp.WorkingDirectory = Path.GetDirectoryName(modsFolder);
                        newApp.FileName = modsFolder;
                        newApp.UseShellExecute = true;
                        newApp.Verb = "open";

                        Process.Start(newApp);
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
                    }
                }
            }
            lblServers.Select();
        }

        private void btnClientMods_Click(object sender, EventArgs e)
        {
            if (txtClientMods.Text.StartsWith("✔️"))
            {
                string mainDir = txtGameInstallFolder.Text;
                string BepInFolder = Path.Combine(mainDir, "BepInEx");
                string pluginsFolder = Path.Combine(BepInFolder, "plugins");

                if (Directory.Exists(pluginsFolder))
                {
                    try
                    {
                        ProcessStartInfo newApp = new ProcessStartInfo();
                        newApp.WorkingDirectory = Path.GetDirectoryName(pluginsFolder);
                        newApp.FileName = pluginsFolder;
                        newApp.UseShellExecute = true;
                        newApp.Verb = "open";

                        Process.Start(newApp);
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
                    }
                }
            }
            lblServers.Select();
        }

        private void btnWhenSPTAKILauncher_Click(object sender, EventArgs e)
        {
        }

        private void btnWhenSPTAKILauncher_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnWhenSPTAKILauncher.Text.ToLower())
            {
                case "do nothing":
                    Properties.Settings.Default.launchParameter = "minimize";
                    btnWhenSPTAKILauncher.Text = "Minimize launcher";
                    break;
                case "minimize launcher":
                    Properties.Settings.Default.launchParameter = "tray";
                    btnWhenSPTAKILauncher.Text = "Minimize to tray";
                    break;
                case "minimize to tray":
                    Properties.Settings.Default.launchParameter = "viewserver";
                    btnWhenSPTAKILauncher.Text = "View server tab";
                    break;
                case "view server tab":
                    Properties.Settings.Default.launchParameter = "donothing";
                    btnWhenSPTAKILauncher.Text = "Do nothing";
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void btnSelectAccount_Click(object sender, EventArgs e)
        {
            if (panelAccountProfiles.Visible)
            {
                panelAccountProfiles.Controls.Clear();
                panelAccountProfiles.Visible = false;
                panelAccountSeparator.Visible = false;
            }
            else
            {
                listProfiles();
            }

            lblServers.Select();
        }

        private void btnSetUsername_Click(object sender, EventArgs e)
        {
            editGameProfile(btnSelectAccount.Text, txtGameInstallFolder.Text);
            lblServers.Select();
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {

            lblServers.Select();
        }

        private void txtAccountAID_MouseDown(object sender, MouseEventArgs e)
        {
            lblServers.Select();
        }

        private void scrollToBottom(RichTextBox richTextBox)
        {
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }

        private void btnPlaySPTAKI_Click(object sender, EventArgs e)
        {
            if (!serverIsRunning)
                if (txtAccountAID.Text.ToLower() == "incomplete profile")
                {
                    showMessage("You\'re trying to launch SPT-AKI with an incomplete profile." + Environment.NewLine +
                        "" + Environment.NewLine +
                        "You can fix this by" + Environment.NewLine + Environment.NewLine +
                        "a) Running your incomplete profile in Aki.Launcher.exe" + Environment.NewLine +
                        "b) Selecting a working profile.");
                    btnAccount.PerformClick();
                }
                else
                {
                    beginLaunching();
                }
            else
            {
                string launcherProcess = "EscapeFromTarkov";
                Process[] launchers = Process.GetProcessesByName(launcherProcess);
                if (launchers != null && launchers.Length > 0)
                {
                    showMessage("Escape From Tarkov is already running. Please close it and try again.");
                }
                else
                {
                    string mainDir = txtGameInstallFolder.Text;
                    string serverFolder = Path.Combine(mainDir, "Aki.Server.exe");

                    bool serverOn = IsAkiServerRunning(serverFolder);
                    if (serverOn)
                    {
                        int akiPort;
                        string portPath = Path.Combine(mainDir, "Aki_Data\\Server\\database\\server.json");
                        bool portExists = File.Exists(portPath);
                        if (portExists)
                        {
                            string readPort = File.ReadAllText(portPath);
                            JObject portObject = JObject.Parse(readPort);
                            akiPort = (int)portObject["port"];
                        }
                        else
                            akiPort = 6969;

                        int port = akiPort;

                        launchTarkov(port);
                    }
                }
            }
            lblServers.Select();
        }

        public void toggleUI(bool enable)
        {
            if (enable)
            {
                if (btnClearLocalCache.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnClearLocalCache.Enabled = true; });
                else
                    btnClearLocalCache.Enabled = true;

                if (btnPlaySPTAKI.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnPlaySPTAKI.Enabled = true; });
                else
                    btnPlaySPTAKI.Enabled = true;

                if (panelServers.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelServers.Enabled = true; });
                else
                    panelServers.Enabled = true;

                if (panelGameOptions.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelGameOptions.Enabled = true; });
                else
                    panelGameOptions.Enabled = true;

                if (panelAccount.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelAccount.Enabled = true; });
                else
                    panelAccount.Enabled = true;

                if (panelAddInstall.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelAddInstall.Enabled = true; });
                else
                    panelAddInstall.Enabled = true;

                if (btnClearList.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnClearList.Enabled = true; });
                else
                    btnClearList.Enabled = true;
            }
            else
            {
                if (btnClearLocalCache.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnClearLocalCache.Enabled = false; });
                else
                    btnClearLocalCache.Enabled = false;

                if (btnPlaySPTAKI.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnPlaySPTAKI.Enabled = false; });
                else
                    btnPlaySPTAKI.Enabled = false;

                if (panelServers.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelServers.Enabled = false; });
                else
                    panelServers.Enabled = false;

                if (panelGameOptions.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelGameOptions.Enabled = false; });
                else
                    panelGameOptions.Enabled = false;

                if (panelAccount.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelAccount.Enabled = false; });
                else
                    panelAccount.Enabled = false;

                if (panelAddInstall.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelAddInstall.Enabled = false; });
                else
                    panelAddInstall.Enabled = false;

                if (btnClearList.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnClearList.Enabled = false; });
                else
                    btnClearList.Enabled = false;

                if (btnCloseAkiServer.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnCloseAkiServer.Text = "Force-close server"; });
                else
                    btnCloseAkiServer.Text = "Force-close server";
            }
        }
        
        public void toggleAddonView(bool enable)
        {
            if (enable)
            {
                panelAddons.Location = new Point(215, 0);
                panelAddons.Size = new Size(393, 419);
                panelAddons.Visible = true;

                panelAddonSeparator.Location = new Point(214, 0);
                panelAddonSeparator.Visible = true;

                listAddons();
            }
            else
            {
                panelAddons.Location = new Point(315, 0);
                panelAddons.Size = new Size(293, 419);
                panelAddons.Visible = false;

                panelAddonSeparator.Location = new Point(314, 0);
                panelAddonSeparator.Visible = false;
            }
        }

        private void beginLaunching()
        {
            killProcesses();

            if (TarkovProcessDetector != null)
            {
                TarkovProcessDetector.CancelAsync();
                TarkovProcessDetector.Dispose();
                TarkovProcessDetector = null;
            }
            if (TarkovEndDetector != null)
            {
                TarkovEndDetector.CancelAsync();
                TarkovEndDetector.Dispose();
                TarkovEndDetector = null;
            }
            if (AkiServerDetector != null)
            {
                AkiServerDetector.CancelAsync();
                AkiServerDetector.Dispose();
                AkiServerDetector = null;
            }

            TarkovProcessDetector = new BackgroundWorker();
            TarkovProcessDetector.DoWork += TarkovProcessDetector_DoWork;
            TarkovProcessDetector.RunWorkerCompleted += TarkovProcessDetector_RunWorkerCompleted;
            TarkovProcessDetector.WorkerSupportsCancellation = true;
            TarkovProcessDetector.RunWorkerAsync();

            Task.Delay(500);
            launchServer();

            if (Properties.Settings.Default.launchParameter == "minimize")
                this.WindowState = FormWindowState.Minimized;
            else if (Properties.Settings.Default.launchParameter == "viewserver")
                btnServer.PerformClick();
        }

        private void killAkiServer()
        {
            string akiServerProcess = "Aki.Server";
            try
            {
                Process[] procs = Process.GetProcessesByName(akiServerProcess);
                if (procs != null && procs.Length > 0)
                {
                    foreach (Process aki in procs)
                    {
                        if (!aki.HasExited)
                        {
                            if (!aki.CloseMainWindow())
                            {
                                try
                                {
                                    aki.Kill();
                                }
                                catch (Exception ex)
                                {
                                    if (ex is System.ComponentModel.Win32Exception win32Exception && win32Exception.Message == "Access is denied")
                                    {
                                        Console.WriteLine("Controlled exception access is denied occurred. If administrator account, ignore");
                                    }
                                }
                                aki.WaitForExit();
                            }
                            else
                            {
                                aki.WaitForExit();
                            }
                        }
                    }
                }

                if (AkiServerDetector != null)
                {
                    AkiServerDetector.CancelAsync();
                    AkiServerDetector.Dispose();
                    AkiServerDetector = null;
                }

                btnCloseAkiServer.Text = "Run server";
                txtServerIsRunning.Text = "❌ Server is closed";
                txtServerIsRunning.ForeColor = Color.Red;
                firstServerNotify = true;
            }
            catch (Exception err)
            {
                Debug.WriteLine($"TERMINATION FAILURE OF AKI SERVER (IGNORE): {err.ToString()}");
            }

            Task.Delay(200);
        }

        private void killProcesses()
        {
            string akiServerProcess = "Aki.Server";
            string eftProcess = "EscapeFromTarkov";

            try
            {
                Process[] procs = Process.GetProcessesByName(akiServerProcess);
                if (procs != null && procs.Length > 0)
                {
                    foreach (Process aki in procs)
                    {
                        if (!aki.HasExited)
                        {
                            if (!aki.CloseMainWindow())
                            {
                                try
                                {
                                    aki.Kill();
                                }
                                catch (Exception ex)
                                {
                                    if (ex is System.ComponentModel.Win32Exception win32Exception && win32Exception.Message == "Access is denied")
                                    {
                                        Console.WriteLine("Controlled exception access is denied occurred. If administrator account, ignore");
                                    }
                                }
                                aki.WaitForExit();
                            }
                            else
                            {
                                aki.WaitForExit();
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine($"TERMINATION FAILURE OF AKI SERVER (IGNORE): {err.ToString()}");
            }

            Task.Delay(200);

            try
            {
                Process[] procs = Process.GetProcessesByName(eftProcess);
                if (procs != null && procs.Length > 0)
                {
                    foreach (Process aki in procs)
                    {
                        if (!aki.HasExited)
                        {
                            if (!aki.CloseMainWindow())
                            {
                                try
                                {
                                    aki.Kill();
                                }
                                catch (Exception ex)
                                {
                                    if (ex is System.ComponentModel.Win32Exception win32Exception && win32Exception.Message == "Access is denied")
                                    {
                                        Console.WriteLine("Controlled exception access is denied occurred. If administrator account, ignore");
                                    }
                                }
                                aki.WaitForExit();
                            }
                            else
                            {
                                aki.WaitForExit();
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine($"TERMINATION FAILURE OF AKI LAUNCHER (IGNORE): {err.ToString()}");
            }

            if (panelServers.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate{panelServers.Enabled = true;});
            else
                panelServers.Enabled = true;

            if (panelSPTAKI.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { panelSPTAKI.Enabled = true; });
            else
                panelSPTAKI.Enabled = true;

            if (panelGameOptions.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { panelGameOptions.Enabled = true; });
            else
                panelGameOptions.Enabled = true;

            if (panelAccount.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { panelAccount.Enabled = true; });
            else
                panelAccount.Enabled = true;

            if (panelAddInstall.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { panelAddInstall.Enabled = true; });
            else
                panelAddInstall.Enabled = true;

            if (btnClearList.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { btnClearList.Enabled = true; });
            else
                btnClearList.Enabled = true;

            hasStopped = true;
            serverIsRunning = false;

            /*
            if (btnCloseAkiServer.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { btnCloseAkiServer.Text = "Run server"; });
            else
                btnCloseAkiServer.Enabled = true;

            if (txtServerIsRunning.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate {
                    txtServerIsRunning.Text = "❌ Server is closed";
                    txtServerIsRunning.ForeColor = Color.Red;
                });
            else
                txtServerIsRunning.Enabled = true;

            firstServerNotify = true;
            */

            if (akiOutput.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { akiOutput.Clear(); });
            else
                akiOutput.Clear();

            if (TarkovProcessDetector != null)
            {
                TarkovProcessDetector.CancelAsync();
                TarkovProcessDetector.Dispose();
                TarkovProcessDetector = null;
            }
            if (TarkovEndDetector != null)
            {
                TarkovEndDetector.CancelAsync();
                TarkovEndDetector.Dispose();
                TarkovEndDetector = null;
            }
            if (AkiServerDetector != null)
            {
                AkiServerDetector.CancelAsync();
                AkiServerDetector.Dispose();
                AkiServerDetector = null;
            }

            toggleUI(true);
        }

        private void runServerOnly()
        {
            Task.Delay(300);
            string serverFolder = txtGameInstallFolder.Text;

            string launcherProcess = "Aki.Server";
            Process[] launchers = Process.GetProcessesByName(launcherProcess);
            if (launchers != null && launchers.Length > 0)
            {
                foreach (Process aki in launchers)
                {
                    if (!aki.HasExited)
                    {
                        if (!aki.CloseMainWindow())
                        {
                            try
                            {
                                aki.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (ex is System.ComponentModel.Win32Exception win32Exception && win32Exception.Message == "Access is denied")
                                {
                                    Console.WriteLine("Controlled exception access is denied occurred. If administrator account, ignore");
                                }
                            }
                            aki.WaitForExit();
                        }
                        else
                        {
                            aki.WaitForExit();
                        }
                    }
                }
            }

            currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(serverFolder);
            Process akiServer = new Process();

            akiServer.StartInfo.WorkingDirectory = serverFolder;
            akiServer.StartInfo.FileName = "Aki.Server.exe";
            akiServer.StartInfo.CreateNoWindow = true;
            akiServer.StartInfo.UseShellExecute = false;
            akiServer.StartInfo.RedirectStandardOutput = true;
            akiServer.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            akiServer.OutputDataReceived += akiServer_OutputDataReceived;
            akiServer.Exited += akiServer_Exited;

            try
            {
                akiServer.Start();
                akiServer.BeginOutputReadLine();

                hasStopped = false;
                serverIsRunning = true;

                if (panelServers.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelServers.Enabled = false; });
                else
                    panelServers.Enabled = false;

                if (panelSPTAKI.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelSPTAKI.Enabled = false; });
                else
                    panelSPTAKI.Enabled = false;

                if (panelGameOptions.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelGameOptions.Enabled = false; });
                else
                    panelGameOptions.Enabled = false;

                if (panelAccount.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelAccount.Enabled = false; });
                else
                    panelAccount.Enabled = false;

                if (panelAddInstall.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { panelAddInstall.Enabled = false; });
                else
                    panelAddInstall.Enabled = false;

                if (btnClearList.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnClearList.Enabled = false; });
                else
                    btnClearList.Enabled = false;

                AkiServerDetector = new BackgroundWorker();
                AkiServerDetector.DoWork += AkiServerDetector_DoWork;
                AkiServerDetector.RunWorkerCompleted += AkiServerDetector_RunWorkerCompleted;
                AkiServerDetector.WorkerSupportsCancellation = true;
                AkiServerDetector.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void launchServer()
        {
            Task.Delay(300);
            string serverFolder = txtGameInstallFolder.Text;

            string launcherProcess = "Aki.Server";
            Process[] launchers = Process.GetProcessesByName(launcherProcess);
            if (launchers != null && launchers.Length > 0)
            {
                foreach (Process aki in launchers)
                {
                    if (!aki.HasExited)
                    {
                        if (!aki.CloseMainWindow())
                        {
                            try
                            {
                                aki.Kill();
                            }
                            catch (Exception ex)
                            {
                                if (ex is System.ComponentModel.Win32Exception win32Exception && win32Exception.Message == "Access is denied")
                                {
                                    Console.WriteLine("Controlled exception access is denied occurred. If administrator account, ignore");
                                }
                            }
                            aki.WaitForExit();
                        }
                        else
                        {
                            aki.WaitForExit();
                        }
                    }
                }
            }

            currentDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(serverFolder);
            Process akiServer = new Process();

            akiServer.StartInfo.WorkingDirectory = serverFolder;
            akiServer.StartInfo.FileName = "Aki.Server.exe";
            akiServer.StartInfo.CreateNoWindow = true;
            akiServer.StartInfo.UseShellExecute = false;
            akiServer.StartInfo.RedirectStandardOutput = true;
            akiServer.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            akiServer.OutputDataReceived += akiServer_OutputDataReceived;
            akiServer.Exited += akiServer_Exited;

            try
            {

                akiServer.Start();
                akiServer.BeginOutputReadLine();

                akiServerOutput = new StringBuilder();
                hasStopped = false;
                serverIsRunning = true;
                checkServerUptime();

                toggleUI(false);

                if (btnCloseAkiServer.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate { btnCloseAkiServer.Text = "Force-close server"; });
                else
                    btnCloseAkiServer.Text = "Force-close server";

                if (ranViaHotkey || Properties.Settings.Default.launchParameter == "tray")
                {
                    Hide();
                    trayIcon.Visible = true;
                    trayIcon.ShowBalloonTip(2000);
                }

                AkiServerDetector = new BackgroundWorker();
                AkiServerDetector.DoWork += AkiServerDetector_DoWork;
                AkiServerDetector.RunWorkerCompleted += AkiServerDetector_RunWorkerCompleted;
                AkiServerDetector.WorkerSupportsCancellation = true;
                AkiServerDetector.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void launchTarkov(int akiPort)
        {
            ProcessStartInfo _tarkov = new ProcessStartInfo();
            string aid = txtAccountAID.Text;
            string serverFolder = txtGameInstallFolder.Text;

            _tarkov.FileName = Path.Combine(serverFolder, "EscapeFromTarkov.exe");
            if (akiPort != 0)
                _tarkov.Arguments = $"-token={aid} -config={{\"BackendUrl\":\"http://127.0.0.1:{akiPort}\",\"Version\":\"live\"}}";
            else
                _tarkov.Arguments = $"-token={aid} -config={{\"BackendUrl\":\"http://127.0.0.1:6969\",\"Version\":\"live\"}}";

            Process tarkovGame = new Process();
            tarkovGame.StartInfo = _tarkov;
            tarkovGame.Start();
        }

        private void checkServerUptime()
        {
            if (CheckServerStatus != null)
            {
                CheckServerStatus.CancelAsync();
                CheckServerStatus.Dispose();
                CheckServerStatus = null;
            }

            CheckServerStatus = new BackgroundWorker();
            CheckServerStatus.WorkerSupportsCancellation = true;
            CheckServerStatus.WorkerReportsProgress = false;
            CheckServerStatus.DoWork += CheckServerWorker_DoWork;
            CheckServerStatus.RunWorkerCompleted += CheckServerWorker_RunWorkerCompleted;

            try
            {
                CheckServerStatus.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
            }
        }

        public void akiServer_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                printServerData(e.Data);
            }
        }

        private void akiServer_Exited(object sender, EventArgs e)
        {
        }

        private void printServerData(string data)
        {
            if (chkAutoScroll.Checked)
            {
                string res = data;
                if (!string.IsNullOrEmpty(res))
                {
                    res = Regex.Replace(res, @"\[[0-1];[0-9][a-z]|\[[0-9][0-9][a-z]|\[[0-9][a-z]|\[[0-9][A-Z]", String.Empty);

                    if (InvokeRequired)
                    {
                        BeginInvoke((MethodInvoker)delegate
                        {
                            akiOutput.AppendText($"{res}\n");
                            akiServerOutput.AppendLine(res);
                            scrollToBottom(akiOutput);
                        });
                    }
                    else
                    {
                        akiOutput.AppendText($"{res}\n");
                        akiServerOutput.AppendLine(res);
                        scrollToBottom(akiOutput);
                    }
                }
            }
        }

        public void TarkovProcessDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            string processName = "EscapeFromTarkov";
            while (true)
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    TarkovEndDetector = new BackgroundWorker();
                    TarkovEndDetector.DoWork += TarkovEndDetector_DoWork;
                    TarkovEndDetector.RunWorkerCompleted += TarkovEndDetector_RunWorkerCompleted;
                    TarkovEndDetector.WorkerSupportsCancellation = true;
                    TarkovEndDetector.RunWorkerAsync();

                    if (TarkovProcessDetector != null)
                    {
                        TarkovProcessDetector.CancelAsync();
                        TarkovProcessDetector.Dispose();
                        TarkovProcessDetector = null;
                    }

                    break;
                }

                System.Threading.Thread.Sleep(1000);
            }
        }

        public void TarkovProcessDetector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (TarkovProcessDetector != null && TarkovProcessDetector.IsBusy)
            {
                TarkovProcessDetector.CancelAsync();
                TarkovProcessDetector.Dispose();
                TarkovProcessDetector = null;
            }
        }

        public void TarkovEndDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            string processName = "EscapeFromTarkov";
            while (true)
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    killProcesses();
                    trayIcon.Visible = false;

                    if (Properties.Settings.Default.exitParameter == "displaylauncher" || ranViaHotkey)
                    {
                        ranViaHotkey = false;
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Show();
                                this.WindowState = FormWindowState.Normal;
                            });
                        }
                        else
                        {
                            Show();
                            this.WindowState = FormWindowState.Normal;
                        }
                    }
                    else if (Properties.Settings.Default.exitParameter == "closelauncher")
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                Application.Exit();
                            });
                        }
                        else
                        {
                            Application.Exit();
                        }
                    }

                    break;
                }
                Thread.Sleep(1500);
            }
        }

        public void TarkovEndDetector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (TarkovEndDetector != null)
            {
                TarkovEndDetector.CancelAsync();
                TarkovEndDetector.Dispose();
                TarkovEndDetector = null;
            }
        }

        public void AkiServerDetector_DoWork(object sender, DoWorkEventArgs e)
        {
            if (AkiServerDetector.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (AkiServerDetector != null)
            {
                string aki_server = "Aki.Server";
                bool isServerRunning = Process.GetProcesses().Any(p => p.ProcessName.Equals(aki_server, StringComparison.OrdinalIgnoreCase));

                if (!isServerRunning)
                {
                    if (!hasNotifiedUser)
                    {
                        showMessage("It appears that the server has closed. This message will only show once." + Environment.NewLine + "Escape From Tarkov will not be closed.");
                        hasNotifiedUser = true;
                    }
                }
                else 
                {
                    txtServerIsRunning.Text = "✔️ Server is running";
                    txtServerIsRunning.ForeColor = Color.SeaGreen;

                    /*
                    if (!firstServerNotify)
                    {
                        btnCloseAkiServer.Enabled = true;
                        btnCloseAkiServer.Text = "Force-close server";
                        txtServerIsRunning.Text = "✔️ Server is running";
                        txtServerIsRunning.ForeColor = Color.SeaGreen;
                        firstServerNotify = true;
                    }
                    */
                }
            }
        }

        public void AkiServerDetector_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (AkiServerDetector != null)
            {
                AkiServerDetector.CancelAsync();
                AkiServerDetector.Dispose();
                AkiServerDetector = null;
            }
        }

        private void CheckServerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string serverFolder = txtGameInstallFolder.Text;
            int akiPort;
            string portPath = Path.Combine(serverFolder, "Aki_Data\\Server\\database\\server.json");
            bool portExists = File.Exists(portPath);
            if (portExists)
            {
                string readPort = File.ReadAllText(portPath);
                JObject portObject = JObject.Parse(readPort);
                akiPort = (int)portObject["port"];
            }
            else
                akiPort = 6969;

            int port = akiPort; // the port to check
            int timeout = 600000; // the maximum time to wait for the port to open in milliseconds
            int delay;

            int elapsed = 0; // the time elapsed since starting to check the port

            while (!CheckPort(port))
            {
                if (elapsed >= timeout)
                {
                    e.Cancel = true;

                    if (CheckServerStatus != null)
                    {
                        CheckServerStatus.CancelAsync();
                        CheckServerStatus.Dispose();
                        CheckServerStatus = null;
                    }

                    showMessage("We could not detect a heartbeat from the Aki Server after 10 minutes.\n" +
                                "\n" +
                                "Max duration reached, falling back. Please diagnose your server and try again.");

                    killProcesses();

                    panelServers.Enabled = true;
                    btnClearList.Enabled = true;
                    return;
                }
                delay = 1000;
                Thread.Sleep(delay); // wait before checking again
                elapsed += delay;
            }
        }

        private void CheckServerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (CheckServerStatus != null)
                {
                    CheckServerStatus.CancelAsync();
                    CheckServerStatus.Dispose();
                    CheckServerStatus = null;
                }
            }
            else if (e.Error != null)
            {
                showMessage("An error occurred: " + e.Error.Message);
            }
            else
            {
                if (CheckServerStatus != null)
                {
                    CheckServerStatus.CancelAsync();
                    CheckServerStatus.Dispose();
                    CheckServerStatus = null;
                }
            }
        }

        private bool CheckPort(int port)
        {
            if (hasStopped)
            {
                return false;
            }
            else
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        client.Connect("127.0.0.1" /* GetLocalIPAddress() */, port);
                        Console.WriteLine("Port connection success. This is a debug message so ignore");
                        serverIsRunning = true;

                        // btnCloseAkiServer.Text = "Force-close server";
                        txtServerIsRunning.Text = "✔️ Server is running";
                        txtServerIsRunning.ForeColor = Color.SeaGreen;

                        launchTarkov(port);
                        return true;
                    }
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    if (ex is System.Net.Sockets.SocketException)
                    {
                        Console.WriteLine($"Server is not running... waiting!");
                        return false;
                    }
                }
            }
            return false;
        }

        private void btnCloseAkiServer_Click(object sender, EventArgs e)
        {
            if (btnCloseAkiServer.Text.ToLower() == "run server")
            {
                // runServerOnly();
                // btnCloseAkiServer.Enabled = false;
            }
            else if (btnCloseAkiServer.Text.ToLower() == "force-close server")
            {
                string aki_server = "Aki.Server";
                string eft_process = "EscapeFromTarkov";
                bool isServerRunning = Process.GetProcesses().Any(p => p.ProcessName.Equals(aki_server, StringComparison.OrdinalIgnoreCase));
                bool isEFTRunning = Process.GetProcesses().Any(p => p.ProcessName.Equals(eft_process, StringComparison.OrdinalIgnoreCase));

                if (!isServerRunning && !isEFTRunning)
                {
                    showMessage("SPT-AKI is not running!");
                }
                else
                {
                    // btnCloseAkiServer.Text = "Run server";
                    // txtServerIsRunning.Text = "✔️ Server is starting up...";
                    // txtServerIsRunning.ForeColor = Color.DodgerBlue;

                    txtServerIsRunning.Text = "❌ Server is closed";
                    txtServerIsRunning.ForeColor = Color.DodgerBlue;

                    killAkiServer();
                    showMessage("Force-closed Aki.Server. Make sure to quit Escape From Tarkov!");
                }
                lblServers.Select();
            }
        }

        private void extensionsRequirementLOE_Click(object sender, EventArgs e)
        {
            if (extensionsRequirementLOE.Text.StartsWith("❌") && extensionsRequirementLOE.Text.ToLower().Contains("click here to download"))
            {
                Process.Start("https://hub.sp-tarkov.com/files/file/1082-loe-load-order-editor");
            }
        }

        private void extensionsRequirementLOE_MouseEnter(object sender, EventArgs e)
        {
            if (extensionsRequirementLOE.Text.StartsWith("❌") && extensionsRequirementLOE.Text.ToLower().Contains("click here to download"))
            {
                extensionsRequirementLOE.Cursor = Cursors.Hand;
                extensionsRequirementLOE.ForeColor = Color.IndianRed;
            }
        }

        private void extensionsRequirementLOE_MouseLeave(object sender, EventArgs e)
        {
            if (extensionsRequirementLOE.Text.StartsWith("❌") && extensionsRequirementLOE.Text.ToLower().Contains("click here to download"))
            {
                extensionsRequirementLOE.Cursor = Cursors.Hand;
                extensionsRequirementLOE.ForeColor = Color.Red; 
            }
            else
            {
                extensionsRequirementLOE.Cursor = Cursors.Default;
                extensionsRequirementLOE.ForeColor = Color.SeaGreen;
            }
        }

        private void btnSPTAKI_Click(object sender, EventArgs e)
        {
            if (serverHasBeenSelected)
            {
                panelSPTAKI.BringToFront();
            }
            lblServers.Select();
        }

        private void btnGameOptions_Click(object sender, EventArgs e)
        {
            if (serverHasBeenSelected)
            {
                panelGameOptions.BringToFront();
            }
            lblServers.Select();
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            if (serverHasBeenSelected)
            {
                panelAccount.BringToFront();
            }
            lblServers.Select();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            if (serverHasBeenSelected)
            {
                panelServer.BringToFront();
            }
            lblServers.Select();
        }

        private void btnAddInstall_Click(object sender, EventArgs e)
        {
            panelAddInstall.BringToFront();
            lblServers.Select();
        }

        private void btnServerFolder_Click(object sender, EventArgs e)
        {
            if (txtServerFolder.Text.StartsWith("✔️"))
            {
                string mainDir = txtGameInstallFolder.Text;

                if (Directory.Exists(mainDir))
                {
                    try
                    {
                        ProcessStartInfo newApp = new ProcessStartInfo();
                        newApp.WorkingDirectory = Path.GetDirectoryName(mainDir);
                        newApp.FileName = Path.GetFileName(mainDir);
                        newApp.UseShellExecute = true;
                        newApp.Verb = "open";

                        Process.Start(newApp);
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString());
                    }
                }
            }
            lblServers.Select();
        }

        private void chkAutoScroll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoScroll.Checked)
                Properties.Settings.Default.autoScrollOption = true;
            else
                Properties.Settings.Default.autoScrollOption = false;

            Properties.Settings.Default.Save();
        }

        private void btnAddons_Click(object sender, EventArgs e)
        {
            if (panelAddons.Visible)
            {
                toggleAddonView(false);
            }
            else
            {
                toggleAddonView(true);
            }

            lblServers.Select();
        }

        private void txtAddons_Click(object sender, EventArgs e)
        {
            int c = addonPaths.Count;

            if (c == 1)
            {
                if (MessageBox.Show($"Are you sure you want to clear {c.ToString()} addon? This is a permanent action.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    addonPaths.Clear();
                    Properties.Settings.Default.availableAddons = "{}";
                    Properties.Settings.Default.Save();

                    panelAddons.Visible = false;
                    panelAddons.Controls.Clear();
                }
            }
            else if (c == 2)
            {
                if (MessageBox.Show($"Are you sure you want to clear both of your {c.ToString()} addons? This is a permanent action.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    addonPaths.Clear();
                    Properties.Settings.Default.availableAddons = "{}";
                    Properties.Settings.Default.Save();

                    panelAddons.Visible = false;
                    panelAddons.Controls.Clear();
                }
            }
            else if (c > 2)
            {
                if (MessageBox.Show($"Are you sure you want to clear all of your {c.ToString()} addons? This is a permanent action.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    addonPaths.Clear();
                    Properties.Settings.Default.availableAddons = "{}";
                    Properties.Settings.Default.Save();

                    panelAddons.Visible = false;
                    panelAddons.Controls.Clear();
                }
            }
        }

        private void txtAddons_MouseEnter(object sender, EventArgs e)
        {
            txtAddons.ForeColor = Color.DodgerBlue;
        }

        private void txtAddons_MouseLeave(object sender, EventArgs e)
        {
            txtAddons.ForeColor = Color.Gray;
        }

        private void btnWhenSPTAKIExits_Click(object sender, EventArgs e)
        {
        }

        private void btnWhenSPTAKIExits_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnWhenSPTAKIExits.Text.ToLower())
            {
                case "do nothing":
                    Properties.Settings.Default.exitParameter = "displaylauncher";
                    btnWhenSPTAKIExits.Text = "Display launcher";
                    break;
                case "display launcher":
                    Properties.Settings.Default.exitParameter = "closelauncher";
                    btnWhenSPTAKIExits.Text = "Close launcher";
                    break;
                case "close launcher":
                    Properties.Settings.Default.exitParameter = "donothing";
                    btnWhenSPTAKIExits.Text = "Do nothing";
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }

        private void btnWorkshop_Click(object sender, EventArgs e)
        {
            Process.Start("https://hub.sp-tarkov.com/files/");
        }

        private void txtSetDisplayName_TextChanged(object sender, EventArgs e)
        {
            btnSetDisplayName.Text = "Set as folder name";

            if (txtSetDisplayName.Text.Length > 0)
                btnSetDisplayName.Text = "Set as custom name";
        }
    }
}