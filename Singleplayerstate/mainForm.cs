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
using System.Text;
using System.Text.Json;
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
        string availableServers;
        string selectedServer = "";
        string temporaryAID;

        // BackgroundWorkers
        public BackgroundWorker CheckServerStatus;
        public BackgroundWorker TarkovProcessDetector;
        public BackgroundWorker TarkovEndDetector;
        public BackgroundWorker AkiServerDetector;

        bool serverHasBeenSelected = false;
        bool isEditingInstall = false;
        bool hasStopped = false;
        bool hasNotifiedUser = false;
        bool serverIsRunning = false;

        Dictionary<string, string> folderPaths = new Dictionary<string, string>();

        public Color hoverColor = Color.FromArgb(39, 44, 47);
        public Color holdColor = Color.FromArgb(39, 44, 47);

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            folderPaths.Add("Placeholder", "Placeholder");
            availableServers = Properties.Settings.Default.availableServers;

            if (string.IsNullOrWhiteSpace(availableServers) || availableServers == null)
            {
                Properties.Settings.Default.availableServers = "{}";
                Properties.Settings.Default.Save();
            }

            availableServers = Properties.Settings.Default.availableServers;
            folderPaths = JsonSerializer.Deserialize<Dictionary<string, string>>(availableServers);

            listServers();
            txtSetDisplayName.Clear();

            TabPage tabAddInstall = mainTab.TabPages["tabAddInstall"];
            if (tabAddInstall != null)
                mainTab.SelectedTab = tabAddInstall;
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
                                    selectServer(lbl.Text, lbl);
                                }
                            }
                        }
                    }
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

        public static void arrInsert(ref string[] array, string item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = item;
        }

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
                    string fullProfile = File.ReadAllText(fullAID);
                    string fileContent = File.ReadAllText(fullAID);
                    JObject parsedFile = JObject.Parse(fileContent);
                    JObject info = (JObject)parsedFile["info"];
                    string infoAID = (string)info["id"];

                    JObject characters = (JObject)parsedFile["characters"];
                    JObject pmc = (JObject)characters["pmc"];
                    JObject Info = (JObject)pmc["Info"];

                    string Nickname = (string)Info["Nickname"];

                    if (infoAID == cleanAID)
                    {
                        return Nickname;
                    }
                }
            }
            return null;
        }

        private string findAID(string displayName)
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
                    JObject Info = (JObject)pmc["Info"];

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
            return null;
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
                lbl.MouseUp += new MouseEventHandler(lbl_MouseUp);
                lbl.Paint += new PaintEventHandler(lbl_Paint);

                lbl.Text = $"✔️ {servers[i]}";
                panelServers.Controls.Add(lbl);
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

                lbl.Text = $"✔️ {convertProfile(profiles[i])}";
                panelAccountProfiles.Controls.Add(lbl);
            }
        }

        private void profile_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                string cleanProfile = label.Text.Replace("✔️ ", "");

                btnSelectAccount.Text = cleanProfile;
                txtUsername.Text = cleanProfile;
                txtAccountAID.Text = findAID(cleanProfile.TrimEnd());

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
            txtLocalCache.Text = $"❌ user\\cache";
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

            gameRequirementServer.Text = $"✔️ Aki.Server found";
            gameRequirementServer.ForeColor = Color.SeaGreen;
            gameRequirementLauncher.Text = $"✔️ Aki.Launcher found";
            gameRequirementLauncher.ForeColor = Color.SeaGreen;
            gameRequirementEFT.Text = $"✔️ Escape From Tarkov found";
            gameRequirementEFT.ForeColor = Color.SeaGreen;

            // Account Tab
            string firstProfile = convertProfile(profiles[0]);
            btnSelectAccount.Text = firstProfile;
            txtUsername.Text = firstProfile;
            txtAccountAID.Text = findAID(btnSelectAccount.Text);
        }

        private void selectServer(string displayName, Control c)
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
                            displayInfo(serverPath);

                            if (!serverHasBeenSelected)
                            {
                                serverHasBeenSelected = true;
                                TabPage tabSPTAKI = mainTab.TabPages["tabSPTAKI"];
                                if (tabSPTAKI != null)
                                    mainTab.SelectedTab = tabSPTAKI;
                            }
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
                foreach (Control c in panelServers.Controls)
                {
                    if (c is Label lbl && c.Name != label.Name)
                    {
                        lbl.Invalidate();
                        lbl.Padding = new Padding(0, 0, 0, 0);
                    }
                }

                label.BackColor = panelServers.BackColor;
                label.Padding = new Padding(10, 0, 0, 0);
                label.Invalidate();

                selectedServer = label.Name;
                selectServer(label.Text, label);
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
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
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
                    string oldInstall = txtGameInstallFolder.Text;

                    saveServer(displayName, folderPath, oldInstall);
                }
                else
                {
                    string folderPath = btnBrowseForFolder.Text;
                    string displayName = txtSetDisplayName.Text;

                    saveServers(displayName, folderPath);
                }
            }
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            int serverCount = folderPaths.Count;

            if (MessageBox.Show($"Are you sure you want to clear all your {serverCount} folders? This will not delete them from your computer.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                folderPaths.Clear();
                Properties.Settings.Default.availableServers = "{}";
                Properties.Settings.Default.Save();

                listServers();
                mainTab.Visible = false;
            }
        }

        private void mainTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!serverHasBeenSelected)
            {
                if (e.TabPage != mainTab.TabPages["tabAddInstall"])
                {
                    e.Cancel = true;
                }
            }
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
                    if (MessageBox.Show($"Remove installation {displayName}? This will restart Minimalist Launcher.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        folderPaths.Remove(displayName);
                        string serializedPaths = JsonSerializer.Serialize(folderPaths);
                        Properties.Settings.Default.availableServers = serializedPaths;
                        Properties.Settings.Default.Save();

                        Application.Restart();
                    }
                }
            }
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
        }

        private void btnServerMods_Click(object sender, EventArgs e)
        {
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
        }

        private void btnWhenSPTAKILauncher_Click(object sender, EventArgs e)
        {
            switch (btnWhenSPTAKILauncher.Text.ToLower())
            {
                case "do nothing":
                    Properties.Settings.Default.launchParameter = 0;
                    btnWhenSPTAKILauncher.Text = "Minimize launcher";
                    break;
                case "minimize launcher":
                    Properties.Settings.Default.launchParameter = 1;
                    btnWhenSPTAKILauncher.Text = "Minimize launcher + open after close";
                    break;
                case "minimize launcher + open after close":
                    Properties.Settings.Default.launchParameter = 2;
                    btnWhenSPTAKILauncher.Text = "Do nothing";
                    break;
            }

            Properties.Settings.Default.Save();
        }

        private void btnSelectAccount_Click(object sender, EventArgs e)
        {
            listProfiles();
        }

        private void btnSetUsername_Click(object sender, EventArgs e)
        {
            editGameProfile(btnSelectAccount.Text, txtGameInstallFolder.Text);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {

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
                beginLaunching();
            else
                showMessage("The Aki Server is already running. Please refer to the Server tab to close it manually.");
        }

        private void beginLaunching()
        {
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

            killProcesses();
            launchServer();
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

            if (akiOutput.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate { akiOutput.Clear(); });
            else
                akiOutput.Clear();
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

                hasStopped = false;
                serverIsRunning = true;
                checkServerUptime();

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

            Console.WriteLine(aid);

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
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    akiOutput.AppendText(data + Environment.NewLine);
                    scrollToBottom(akiOutput);
                });
            }
            else
            {
                akiOutput.AppendText(data + Environment.NewLine);
                scrollToBottom(akiOutput);
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
                    Console.WriteLine("Tarkov detected");

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
                    Console.WriteLine("Quit Tarkov");
                    if (Properties.Settings.Default.launchParameter == 2)
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                this.Show();
                            });
                        }
                        else
                        {
                            this.Show();
                        }
                    }

                    killProcesses();
                    break;
                }
                Thread.Sleep(2500);
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

                    mainTab.Enabled = true;
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
                        serverIsRunning = true;
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
                killAkiServer();
                showMessage("Force killed Aki.Server. Make sure to quit Escape From Tarkov!");
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
    }
}
