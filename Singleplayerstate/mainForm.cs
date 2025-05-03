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
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Schema;
using WK.Libraries.BetterFolderBrowserNS;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Timer = System.Windows.Forms.Timer;

namespace Singleplayerstate
{
    public partial class mainForm : Form
    {
        string currentDirectory = "D:\\Games\\Escape From Tarkov\\SPT 3.11";
        string logsFolder = "";
        public string profiles_dict = null;
        public StringBuilder akiServerOutput;
        public string availableServers;
        public string availableAddons;
        string selectedServer = "";
        public string mainDir = null;
        string temporaryAID = null;

        public string fika_dir = "[FIKA]";
        public string fika_core_plugin = "Fika.Core.dll";
        public string fika_headless_plugin = "Fika.Headless.dll";
        public string fika_server_mod = "fika-server";

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

        private msgBoard messageWindow;

        public mainForm()
        {
            InitializeComponent();
            KeyDown += mainForm_KeyDown;
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            initiateLauncher();
        }

        private void initiateLauncher()
        {
            logsFolder = Path.Combine(currentDirectory, "logs");
            bool logsFolderExists = Directory.Exists(logsFolder);
            if (!logsFolderExists)
            {
                Directory.CreateDirectory(logsFolder);
            }

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

            profiles_dict = Path.Combine(currentDirectory, "profiles_dictionary.json");
            bool profilesDictExists = File.Exists(profiles_dict);
            if (!profilesDictExists)
            {
                generateProfileDictionary();
            }

            foreach (var kvp in folderPaths)
            {
                updateProfiles(kvp.Value);
            }

            bool fika_dir_exists = Directory.Exists(Path.Combine(currentDirectory, fika_dir));
            if (!fika_dir_exists)
            {
                try
                {
                    Directory.CreateDirectory(fika_dir);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message.ToString());
                }
            }

            listServers();

            txtSetDisplayName.Clear();
            btnAddInstall.PerformClick();
            chkAutoScroll.Checked = Properties.Settings.Default.autoScrollOption;
            chkLogOnExit.Checked = Properties.Settings.Default.logOnExit;
            chkMinimizeOnGameLaunch.Checked = Properties.Settings.Default.minimizeOnGameLaunch;

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

            if (Properties.Settings.Default.onServerDoubleClick)
                btnOnServerDoubleClick.Text = "Play SPT";
            else
                btnOnServerDoubleClick.Text = "Do nothing";

            if (Properties.Settings.Default.whenLauncherCloses)
                btnWhenLauncherExits.Text = "Do nothing";
            else
                btnWhenLauncherExits.Text = "Show pop-up";

            if (Properties.Settings.Default.devMode)
            {
                btnDevMode.Text = "Enabled";
                panelIPAddress.Visible = true;
            }
            else
            {
                btnDevMode.Text = "Disabled";
                panelIPAddress.Visible = false;
            }

            if (Properties.Settings.Default.closeOnExit)
                btnAppCloseNotification.Text = "Enabled";
            else
                btnAppCloseNotification.Text = "Disabled";

            if (Properties.Settings.Default.localhostIP != "" &&
                Properties.Settings.Default.localhostIP != "127.0.0.1")
            {
                string fetchedAddress = Properties.Settings.Default.localhostIP;
                IPAddress existingIP = IPAddress.Parse(fetchedAddress);
                string existingIP_string = existingIP.ToString();
                titleCurrentIP.Text = $"IP: {existingIP_string}";
            }
            else
            {
                titleCurrentIP.Text = $"IP: 127.0.0.1";
            }

            if (Properties.Settings.Default.isFikaEnabled)
            {
                btnFikaMode.Text = "Enabled";
                btnPlaySPTAKI.Text = "Play Fika";
                btnAdjustFikaSettings.Visible = true;
            }
            else
            {
                btnFikaMode.Text = "Disabled";
                btnPlaySPTAKI.Text = "Play SPT";
                btnAdjustFikaSettings.Visible = false;
            }

            checkAutoStart();
        }

        private void performClosing()
        {
            int serverCount = panelServers.Controls.Count - 1;
            if (serverCount > -1)
            {
                string findServer = fetchCurrentServer();
                if (findServer != null)
                {
                    Properties.Settings.Default.lastServer = findServer;
                }

                Properties.Settings.Default.addonPanelVisible = panelAddons.Visible;
                Properties.Settings.Default.Save();

                saveAutostart(findServer);
                killProcesses();
            }
            else
            {
                Properties.Settings.Default.lastServer = null;
                saveAutostart(null);
                Properties.Settings.Default.addonPanelVisible = panelAddons.Visible;
                Properties.Settings.Default.Save();
                killProcesses();
            }

            Application.Exit();
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

        private void editGameInstall(string displayName, string oldInstall)
        {
            var browse = new BetterFolderBrowser();
            browse.Title = "Select folder that contains SPT";
            browse.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            browse.Multiselect = false;

            if (browse.ShowDialog(this) == DialogResult.OK)
            {
                string mainDir = txtGameInstallFolder.Text;
                string selectedFolder = browse.SelectedFolder;
                string fullPath = Path.GetFullPath(selectedFolder);
                bool folderExists = Directory.Exists(selectedFolder);
                if (folderExists)
                {
                    if (folderPaths != null)
                    {
                        if (fullPath == mainDir)
                        {
                            string content = "This path has already been selected. Press OK to continue";
                            showMessage(content, this.Text);
                        }
                        else
                        {
                            folderPaths[displayName] = selectedFolder;
                            string serializedPaths = JsonSerializer.Serialize(folderPaths);
                            Properties.Settings.Default.availableServers = serializedPaths;
                            Properties.Settings.Default.Save();

                            showMessage($"Updated folder:\n{oldInstall}\n\nto:\n{selectedFolder}", this.Text);
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
        }

        private string fetchName(string name)
        {
            string newName = name.Replace("✔️ ", "");
            return newName;
        }

        private void fetchLastServer()
        {
            if (Properties.Settings.Default.lastServer != null)
            {
                foreach (Control c in panelServers.Controls)
                {
                    if (c is Label lbl)
                    {
                        string cleanLbl = fetchName(lbl.Text);

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
                    string server = fetchName(selectedLbl.Text);
                    return server;
                }
            }

            return null;
        }

        private void generateProfileDictionary()
        {
            bool doesProfileDictExist = File.Exists(profiles_dict);
            if (!doesProfileDictExist)
            {
                JObject profiles = new JObject();
                foreach (var kvp in folderPaths)
                {
                    profiles[kvp.Value] = insertFirstProfile(kvp.Value);
                }

                JObject fullProfile = new JObject
                {
                    [profiles] = new JObject(),
                    ["currentFikaAID"] = "none"
                };

                string content = fullProfile.ToString();
                File.WriteAllText(profiles_dict, content);
            }
        }

        private void updateProfiles(string mainDir)
        {
            string profiles = Path.Combine(mainDir, "user", "profiles");
            bool profilesExists = Directory.Exists(profiles);
            if (profilesExists)
            {
                bool doesProfileDictExist = File.Exists(profiles_dict);
                if (!doesProfileDictExist)
                {
                    generateProfileDictionary();
                }

                string content = File.ReadAllText(profiles_dict);
                JObject objectContent = JObject.Parse(content);
                if (objectContent[mainDir] != null &&
                    string.IsNullOrEmpty((string)objectContent[mainDir]))
                {
                    return;
                }
                else
                    objectContent[mainDir] = insertFirstProfile(mainDir);
            }
        }

        private void fetchProfile(string mainDir)
        {
            bool doesProfileDictExist = File.Exists(profiles_dict);
            if (!doesProfileDictExist)
            {
                generateProfileDictionary();
            }

            string content = File.ReadAllText(profiles_dict);
            JObject objectContent = JObject.Parse(content);
            if (objectContent[mainDir] != null &&
                !string.IsNullOrEmpty((string)objectContent[mainDir]))
            {
                string aid = (string)objectContent[mainDir];
                btnSelectAccount.Text = convertProfile(aid);
                txtUsername.Text = convertProfile(aid);
                txtAccountAID.Text = aid;
            }
        }

        private void insertNewServerProfile(string mainDir)
        {
            bool doesProfileDictExist = File.Exists(profiles_dict);
            if (!doesProfileDictExist)
            {
                generateProfileDictionary();
            }

            string content = File.ReadAllText(profiles_dict);
            JObject objectContent = JObject.Parse(content);
            if (objectContent != null)
            {
                objectContent[mainDir] = insertFirstProfile(mainDir);
            }
            string updated = objectContent.ToString();
            File.WriteAllText(profiles_dict, updated);
        }

        private string insertFirstProfile(string mainDir)
        {
            string profilesFolder = Path.Combine(mainDir, "user", "profiles");
            bool profilesFolderExists = Directory.Exists(profilesFolder);
            if (profilesFolderExists)
            {
                string[] profiles = Directory.GetFiles(profilesFolder, "*.json");
                string firstProfile = Path.GetFileNameWithoutExtension(profiles[0]);
                return firstProfile;
            }
            return null;
        }

        private void selectFirstProfile(string mainDir)
        {
            string profilesFolder = Path.Combine(mainDir, "user", "profiles");
            bool profilesFolderExists = Directory.Exists(profilesFolder);
            if (profilesFolderExists)
            {
                string[] profiles = Directory.GetFiles(profilesFolder, "*.json");
                string firstProfile = Path.GetFileNameWithoutExtension(profiles[0]);

                btnSelectAccount.Text = convertProfile(firstProfile);
                txtUsername.Text = convertProfile(firstProfile);
                txtAccountAID.Text = firstProfile;
                btnSetUsername.Enabled = true;
                temporaryAID = firstProfile;

                setServerProfile(mainDir, firstProfile);
            }
        }

        private void selectProfile(string displayName)
        {
            string aidFound = findAID(displayName.TrimEnd());
            if (aidFound.ToLower().Contains("incomplete profile"))
            {
                if (MessageBox.Show("Incomplete profile detected. Selecting the first profile available." + Environment.NewLine +
                                    "" + Environment.NewLine +
                                    "Press OK to continue.", this.Text, MessageBoxButtons.OK) == DialogResult.OK)
                    selectFirstProfile(mainDir);
            }
            else
            {
                btnSelectAccount.Text = displayName;
                txtUsername.Text = displayName;
                txtAccountAID.Text = aidFound;
                btnSetUsername.Enabled = true;
                temporaryAID = aidFound;

                mainDir = txtGameInstallFolder.Text;
                setServerProfile(mainDir, aidFound);
            }

            panelAccountProfiles.Visible = false;
            panelAccountSeparator.Visible = false;
        }

        private void setServerProfile(string mainDir, string aid)
        {
            bool doesProfileDictExist = File.Exists(profiles_dict);
            if (!doesProfileDictExist)
            {
                generateProfileDictionary();
            }

            string content = File.ReadAllText(profiles_dict);
            JObject objectContent = JObject.Parse(content);
            if (objectContent != null && !string.IsNullOrEmpty((string)objectContent[mainDir]))
                objectContent[mainDir] = aid;

            string updated = objectContent.ToString();
            File.WriteAllText(profiles_dict, updated);
        }

        private void deleteProfile(string displayName)
        {
            string aidFound = findAID(displayName.TrimEnd());

            string path = txtGameInstallFolder.Text;
            bool pathExists = Directory.Exists(path);
            if (pathExists)
            {
                string userFolder = Path.Combine(path, "user");
                bool userFolderExists = Directory.Exists(userFolder);
                if (userFolderExists)
                {
                    string profilesFolder = Path.Combine(userFolder, "profiles");
                    bool profilesFolderExists = Directory.Exists(userFolder);
                    if (profilesFolderExists)
                    {
                        if (aidFound != null)
                        {
                            string fullProfile = $"{aidFound}.json";
                            string profilePath = Path.Combine(profilesFolder, fullProfile);

                            bool profilePathExists = File.Exists(profilePath);
                            if (profilePathExists)
                            {
                                try
                                {
                                    File.Delete(profilePath);
                                    listProfiles();

                                    Control firstProfile = panelAccountProfiles.Controls.Find("accountProfile0", false).FirstOrDefault();
                                    if (firstProfile != null)
                                    {
                                        string cleanProfile = fetchName(firstProfile.Text);
                                        selectProfile(cleanProfile);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                                        Environment.NewLine +
                                                        Environment.NewLine +
                                                        ex.ToString(), this.Text);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void showMessage(string content, string title)
        {
            if (messageWindow != null)
            {
                messageWindow.Close();
                messageWindow.Dispose();
            }

            messageWindow = new msgBoard();
            messageWindow.TopMost = true;
            messageWindow.messageContent.Text = content;
            messageWindow.messageTitle.Text = title;

            messageWindow.ShowDialog();
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
                                    ex.ToString(), this.Text);
            }

            string serializedPaths = JsonSerializer.Serialize(folderPaths);
            Properties.Settings.Default.availableServers = serializedPaths;
            Properties.Settings.Default.Save();

            string fullPath = Path.GetFullPath(folderPath);
            insertNewServerProfile(fullPath);

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
                                    ex.ToString(), this.Text);
            }

            string serializedPaths = JsonSerializer.Serialize(folderPaths);
            Properties.Settings.Default.availableServers = serializedPaths;
            Properties.Settings.Default.Save();

            showMessage($"SPT installation {displayName} changed to folder {folderPath}!", this.Text);
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
                                    ex.ToString(), this.Text);
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
                                ex.ToString(), this.Text);
                    }

                    btnSelectAccount.Text = txtUsername.Text;
                    panelAccountProfiles.Visible = false;
                    panelAccountSeparator.Visible = false;
                    showMessage($"Old username {oldUser} changed to {txtUsername.Text}!", this.Text);
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

            mainDir = txtGameInstallFolder.Text;
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
                mainDir = txtGameInstallFolder.Text;
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
                                else if (GameVersion.ToLower() == "unheard_edition")
                                    GameVersion = "Unheard Edition";

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

        private int returnClientModsList()
        {
            int num = 0;

            string gamePath = txtGameInstallFolder.Text;
            string BepInEx = Path.Combine(gamePath, "BepInEx");
            string LogOutput = Path.Combine(BepInEx, "LogOutput.log");

            bool akiServerIsRunning = IsAkiServerRunning();
            if (akiServerIsRunning)
                return -1;
            else
            {
                bool BepInExExists = Directory.Exists(BepInEx);
                bool LogOutputExists = File.Exists(LogOutput);

                if (BepInExExists && LogOutputExists)
                {
                    string loadString = File.ReadAllLines(LogOutput)[14];
                    if (loadString.Contains("plugins to load"))
                    {
                        Match numberSuccess = Regex.Match(loadString, @"\d+");
                        if (numberSuccess.Success)
                        {
                            num = int.Parse(numberSuccess.Value);
                            return num;
                        }
                    }
                }
            }

            return -1;
        }

        public static bool IsAkiServerRunning()
        {
            try
            {
                return Process.GetProcessesByName("SPT.Server").Any();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool isTarkovRunning()
        {
            try
            {
                return Process.GetProcessesByName("EscapeFromTarkov").Any();
            }
            catch (Exception)
            {
                return false;
            }
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
        }

        private async void checkAutoStart()
        {
            int servercount = panelServers.Controls.OfType<Label>().Count();
            if (panelServers.Controls["serverPlaceholder"] != null) servercount -= 1;

            string autostartFile = Path.Combine(currentDirectory, "autostart.txt");
            bool autostartExists = File.Exists(autostartFile);
            if (autostartExists)
            {
                string[] autocontent = File.ReadAllLines(autostartFile, Encoding.UTF8);
                string boolValue = null;
                string serverValue = null;

                if (autocontent.Length > 0 && autocontent[0] != null)
                    boolValue = autocontent[0].TrimEnd();

                if (autocontent.Length > 1 && autocontent[1] != null)
                    serverValue = autocontent[1].TrimEnd();

                if (serverValue != null && serverValue != "")
                {
                    if (folderPaths.ContainsKey(serverValue))
                    {
                        foreach (Control c in panelServers.Controls)
                        {
                            if (c is Label lbl)
                            {
                                if (fetchName(lbl.Text) == serverValue)
                                {
                                    if (boolValue.ToLower() == "autostart=true")
                                    {
                                        clickServer(lbl, true);
                                        await Task.Delay(500);
                                        ranViaHotkey = true;
                                        btnPlaySPTAKI.PerformClick();
                                    }
                                    else if (boolValue.ToLower() == "autostart=false")
                                    {
                                        clickServer(lbl, true);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (servercount == 1)
                {
                    Control firstServer = panelServers.Controls["listedServer0"];
                    if (firstServer != null)
                    {
                        clickServer(firstServer, true);
                        string content =
                            $"autostart=false" + Environment.NewLine;
                        
                        try
                        {
                            File.WriteAllText(autostartFile, content);
                        }
                        catch (Exception ex)
                        {
                            showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                        Environment.NewLine +
                                        Environment.NewLine +
                                        ex.ToString(), this.Text);
                        }
                    }
                }
            }
        }

        private void saveAutostart(string serverName)
        {
            string autostart = null;

            string autostartFile = Path.Combine(currentDirectory, "autostart.txt");
            bool autostartExists = File.Exists(autostartFile);
            if (autostartExists)
            {
                string[] lines = File.ReadAllLines(autostartFile);
                if (lines.Length > -1 && lines[0] != null)
                {
                    autostart = lines[0].TrimEnd();
                }

                try
                {
                    string[] updatedLines = { autostart, serverName };
                    File.WriteAllLines(autostartFile, updatedLines);
                }
                catch (Exception ex)
                {
                    showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                Environment.NewLine +
                                Environment.NewLine +
                                ex.ToString(), this.Text);
                }
            }
        }

        private void listProfiles()
        {
            panelAccountProfiles.Controls.Clear();
            panelAccountProfiles.Visible = true;
            panelAccountSeparator.Visible = true;

            mainDir = txtGameInstallFolder.Text;
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

        public void removeInstall(string displayName)
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
                    string cleanItem = fetchName(label.Text);
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
                string cleanProfile = fetchName(label.Text);
                selectProfile(cleanProfile);
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
            infoServer.Text = $"Regular SPT (offline)";

            gameRequirementServer.Text = $"✔️ SPT.Server not found";
            gameRequirementServer.ForeColor = Color.Red;
            gameRequirementLauncher.Text = $"✔️ SPT.Launcher not found";
            gameRequirementLauncher.ForeColor = Color.Red;
            gameRequirementEFT.Text = $"✔️ Escape From Tarkov not found";
            gameRequirementEFT.ForeColor = Color.Red;
            extensionsRequirementLOE.Text = $"❌ Load Order Editor not found [Click here to download]";
            extensionsRequirementLOE.ForeColor = Color.Red;

            btnSelectAccount.Text = "None selected";
            txtUsername.Clear();
            btnPlaySPTAKI.Enabled = true;
            btnClientMods.Text = "Client mods - N/A";

            string mainDir = path;

            string userFolder = Path.Combine(mainDir, "user");
            string akiServerFile = Path.Combine(mainDir, "SPT.Server.exe");
            string akiLauncherFile = Path.Combine(mainDir, "SPT.Launcher.exe");
            string EFTFile = Path.Combine(mainDir, "EscapeFromTarkov.exe");

            string akiDataFolder = Path.Combine(mainDir, "SPT_Data");
            string akiServerFolder = Path.Combine(akiDataFolder, "Server");
            string akiConfigsFolder = Path.Combine(akiServerFolder, "configs");
            string akiCoreJSON = Path.Combine(akiConfigsFolder, "core.json");

            string BepInFolder = Path.Combine(path, "BepInEx");
            string pluginsFolder = Path.Combine(BepInFolder, "plugins");
            string SAINClient = Path.Combine(pluginsFolder, "SAIN");
            string Fika_Core = Path.Combine(pluginsFolder, fika_core_plugin);
            string Fika_Headless = Path.Combine(pluginsFolder, fika_headless_plugin);

            string cacheFolder = Path.Combine(userFolder, "cache");
            string modsFolder = Path.Combine(userFolder, "mods");
            string LOEPath = Path.Combine(modsFolder, "Load Order Editor.exe");
            string SAINServer = Path.Combine(modsFolder, "zSolarint-SAIN-ServerMod");
            string Fika_Server_Mod = Path.Combine(modsFolder, fika_server_mod);
            Debug.WriteLine(Fika_Server_Mod);

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
                gameRequirementServer.Text = $"✔️ SPT.Server found";
                gameRequirementServer.ForeColor = Color.SeaGreen;
            }
            if (File.Exists(akiLauncherFile))
            {
                gameRequirementLauncher.Text = $"✔️ SPT.Launcher found";
                gameRequirementLauncher.ForeColor = Color.SeaGreen;
            }
            if (File.Exists(EFTFile))
            {
                gameRequirementEFT.Text = $"✔️ Escape From Tarkov found";
                gameRequirementEFT.ForeColor = Color.SeaGreen;
            }

            if (!File.Exists(akiServerFile) && !File.Exists(akiLauncherFile) || !File.Exists(EFTFile))
                btnPlaySPTAKI.Enabled = false;

            bool doesFikaCoreExist = File.Exists(Fika_Core);
            bool doesFikaServerModExist = Directory.Exists(Fika_Server_Mod);
            bool doesFikaHeadlessExist = File.Exists(Fika_Headless);

            bool isFikaCoreParked = File.Exists(Path.Combine(fika_dir, fika_core_plugin));
            bool isFikaServerModParked = File.Exists(Path.Combine(fika_dir, fika_server_mod));
            bool isFikaHeadlessParked = File.Exists(Path.Combine(fika_dir, fika_headless_plugin));

            string headlessStatus = doesFikaHeadlessExist ? "Active" : "Parked / Inactive";
            string coreStatus = doesFikaCoreExist ? "Active" : "Parked / Inactive";
            string serverModStatus = doesFikaServerModExist ? "Active" : "Parked / Inactive";

            fikaToolTip.SetToolTip(btnFikaMode,
                    $"Core plugin: {coreStatus}" + Environment.NewLine +
                    $"Server mod: {serverModStatus}" + Environment.NewLine +
                    $"Headless: {headlessStatus}");

            // Checking local server status
            bool serverOn = IsAkiServerRunning();
            if (serverOn)
            {
                btnPlaySPTAKI.Text = "Quit SPT";
            }

            // Account Tab
            bool userFolderExists = Directory.Exists(userFolder);
            if (userFolderExists)
            {
                string profilesFolder = Path.Combine(userFolder, "profiles");
                bool profilesFolderExists = Directory.Exists(profilesFolder);
                if (profilesFolderExists)
                {
                    fetchProfile(mainDir);
                }
                else
                {
                    showMessage("Could not detect a profiles folder. Install SPT and create a profile, then try again.", this.Text);
                }
            }
            else
            {
                showMessage("Could not detect a user folder. Install SPT and try again.", this.Text);
            }

            string serverFolder = txtGameInstallFolder.Text;
            int akiPort;
            string portPath = Path.Combine(serverFolder, "SPT_Data", "Server", "database", "server.json");
            bool portExists = File.Exists(portPath);
            if (portExists)
            {
                string readPort = File.ReadAllText(portPath);
                JObject portObject = JObject.Parse(readPort);
                akiPort = (int)portObject["port"];
                titleCurrentPort.Text = $"Port: {akiPort.ToString()}";
            }
            else
                titleCurrentPort.Text = "Port: 6969";

            string fikaFolder = Path.Combine(mainDir, "user", "fika");
            bool fikaFolderExists = Directory.Exists(fikaFolder);
            if (!fikaFolderExists)
            {
                return;
            }
            else
            {
                string[] fikaProfiles = Directory.GetFiles(fikaFolder, "*.json");
                if (fikaProfiles.Length == 0)
                {
                    return;
                }
                else
                {
                    string currentFile = Path.GetFileNameWithoutExtension(fikaProfiles[0]);
                    if (string.IsNullOrEmpty(currentFile))
                    {
                        Properties.Settings.Default.currentFikaProfile = currentFile;
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private void displayClientMods()
        {
            string path = txtGameInstallFolder.Text;
            string BepInFolder = Path.Combine(path, "BepInEx");
            string pluginsFolder = Path.Combine(BepInFolder, "plugins");
            string sptFolder = Path.Combine(pluginsFolder, "spt");

            int externalModCount = returnClientModsList();

            if (externalModCount > -1)
            {
                int sptDefaultCount = Directory.GetFiles(sptFolder, "*.dll").Count();
                string clientModsCount = $"Client mods - {externalModCount - sptDefaultCount}";
                btnClientMods.Text = clientModsCount;
                clientModTip.Active = false;
            }
            else
            {
                btnClientMods.Text = "Client mods - N/A";
                clientModTip.Active = true;
                clientModTip.SetToolTip(btnClientMods, "Please start SPT once to generate LogOutput.log to ensure correct readings.");
                txtClientMods.Text = $"✔️ BepInEx\\plugins (no LogOutput.log)";
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
            string cleanOutput = fetchName(c.Text);

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

                            if (!serverHasBeenSelected)
                            {
                                serverHasBeenSelected = true;
                            }

                            Properties.Settings.Default.lastServer = cleanOutput;
                            Properties.Settings.Default.Save();

                            displayInfo(serverPath);
                            displayClientMods();

                            if (autoClick)
                                btnSPTAKI.PerformClick();
                        }
                        else
                        {
                            string message_content = $"It appears that" + Environment.NewLine + Environment.NewLine +
                                                     $"Name: {cleanOutput}" + Environment.NewLine +
                                                     $"Path: {serverPath}" + Environment.NewLine + Environment.NewLine +
                                                     $"does not exist. Would you like to clear it from the list and refresh?";
                            MessageBoxButtons btns = MessageBoxButtons.YesNo;

                            if (MessageBox.Show(message_content, this.Text, btns) == DialogResult.Yes)
                            {
                                removeInstall(cleanOutput);
                            }
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
                if (Properties.Settings.Default.onServerDoubleClick)
                {
                    ranViaHotkey = true;
                    btnPlaySPTAKI.PerformClick();
                }
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
            browse.Title = "Select folder that contains SPT";
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
                if (!Properties.Settings.Default.isFikaEnabled)
                {
                    if (Properties.Settings.Default.closeOnExit)
                    {
                        string gamePath = txtGameInstallFolder.Text;
                        bool accepted = false;
                        bool akiServerIsRunning = IsAkiServerRunning();
                        if (akiServerIsRunning)
                        {
                            DialogResult result = MessageBox.Show("The AKI server is running, this will close the server and game. Are you sure you want to proceed?" +
                                Environment.NewLine +
                                Environment.NewLine +
                                "Click NO to cancel." + Environment.NewLine +
                                "Click YES to proceed." + Environment.NewLine +
                                "Click CANCEL to minimize.",
                                "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                            if (result == DialogResult.No)
                                e.Cancel = true;
                            else if (result == DialogResult.Yes)
                                accepted = true;
                            else
                            {
                                e.Cancel = true;
                                if (trayIcon != null)
                                {
                                    Hide();
                                    trayIcon.Visible = true;
                                    trayIcon.ShowBalloonTip(2000);
                                }
                            }

                            if (accepted)
                                performClosing();
                        }
                        else
                        {
                            if (!Properties.Settings.Default.whenLauncherCloses)
                            {
                                DialogResult result = MessageBox.Show("Are you sure you want to quit?" +
                                Environment.NewLine +
                                Environment.NewLine +
                                "Click NO to cancel." + Environment.NewLine +
                                "Click YES to quit." + Environment.NewLine +
                                "Click CANCEL to minimize.",
                                "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                                if (result == DialogResult.No)
                                    e.Cancel = true;
                                else if (result == DialogResult.Yes)
                                    accepted = true;
                                else
                                {
                                    e.Cancel = true;
                                    if (trayIcon != null)
                                    {
                                        Hide();
                                        trayIcon.Visible = true;
                                        trayIcon.ShowBalloonTip(2000);
                                    }
                                }

                                if (accepted)
                                    performClosing();
                            }
                            else
                                performClosing();
                        }
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

                    if (folderPaths.ContainsKey(displayName))
                    {
                        showMessage($"An installation with the name {displayName} already exists." + Environment.NewLine + Environment.NewLine +
                                    $"Please choose a different name!", this.Text);
                        txtSetDisplayName.Focus();
                    }
                    else
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

                    if (folderPaths.ContainsKey(displayName))
                    {
                        showMessage($"An installation with the name {displayName} already exists." + Environment.NewLine + Environment.NewLine +
                                    $"Please choose a different name!", this.Text);
                        txtSetDisplayName.Focus();
                    }
                    else
                        saveServers(displayName, folderPath);
                }
            }

            btnSPTAKI.PerformClick();
            txtSetDisplayName.Clear();
            lblServers.Select();
        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            int serverCount = folderPaths.Count;

            if (serverCount == 0)
            {
                showMessage("There are no AKI folders available, please browse for one.", this.Text);
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
                    displayName = fetchName(c.Text);
                    break;
                }
            }

            editGameInstall(displayName, oldInstall);
        }

        private void btnRemoveInstall_Click(object sender, EventArgs e)
        {
            Control selectedControl = panelServers.Controls.Find(selectedServer, false).FirstOrDefault();
            string displayName = fetchName(selectedControl.Text);

            if (folderPaths != null)
            {
                if (folderPaths.ContainsKey(displayName))
                {
                    if (MessageBox.Show($"Remove installation {displayName}?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        removeInstall(displayName);
                    }
                }
            }
            lblServers.Select();
        }

        private void btnClearLocalCache_Click(object sender, EventArgs e)
        {
            if (txtLocalCache.Text.StartsWith("✔️"))
            {
                mainDir = txtGameInstallFolder.Text;
                string userFolder = Path.Combine(mainDir, "user");
                string cacheFolder = Path.Combine(userFolder, "cache");

                if (Directory.Exists(cacheFolder))
                {
                    try
                    {
                        Directory.Delete(cacheFolder, true);
                        txtLocalCache.Text = "❌ user\\cache";
                        showMessage("Cache deleted!", this.Text);

                        string findServer = fetchCurrentServer();
                        if (findServer != null)
                        {
                            foreach (Control c in panelServers.Controls)
                            {
                                if (c is Label lbl)
                                {
                                    string cleanLbl = fetchName(findServer);
                                    clickServer(lbl, true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine+
                                    ex.ToString(), this.Text);
                    }
                }
            }
            lblServers.Select();
        }

        private void btnLOE_Click(object sender, EventArgs e)
        {
            if (txtLoadOrderEditor.Text.StartsWith("✔️"))
            {
                mainDir = txtGameInstallFolder.Text;
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
                                        ex.ToString(), this.Text);
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
                mainDir = txtGameInstallFolder.Text;
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
                                    ex.ToString(), this.Text);
                    }
                }
            }
            lblServers.Select();
        }

        private void btnClientMods_Click(object sender, EventArgs e)
        {
            if (txtClientMods.Text.StartsWith("✔️"))
            {
                mainDir = txtGameInstallFolder.Text;
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
                                    ex.ToString(), this.Text);
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
            if (txtUsername.Text != "" || txtUsername.Text.ToLower() != "incomplete profile")
            {
                string profileName = txtUsername.Text;
                string content = $"Delete {profileName}?" + Environment.NewLine + Environment.NewLine +
                                 $"This action is irreversible.";
                MessageBoxButtons btns = MessageBoxButtons.YesNo;

                if (MessageBox.Show(content, this.Text, btns) == DialogResult.Yes)
                {
                    deleteProfile(profileName);
                }
            }

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
            string gamePath = txtGameInstallFolder.Text;

            if (Properties.Settings.Default.isFikaEnabled)
            {
                if (isTarkovRunning())
                {
                    if (btnPlaySPTAKI.Text.ToLower() == "quit fika")
                    {
                        killTarkov();
                    } else
                    {
                        string message = "Escape From Tarkov is already running locally on this computer. Would you like to restart it?";
                        if (MessageBox.Show(message, this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            killTarkov();
                            btnPlaySPTAKI.PerformClick();
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                btnPlaySPTAKI.Text = "Waiting...";
                toggleUI(false);
                string hostIP = Properties.Settings.Default.fikaIP;
                int hostPort = Properties.Settings.Default.fikaPort;

                IPAddress hostAddress = IPAddress.Parse(hostIP);
                IPEndPoint hostEndPoint = new IPEndPoint(hostAddress, hostPort);

                Ping send_ping = new Ping();
                string data = "checker_data";
                int timeout = 5000;
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                PingOptions options = new PingOptions(64, true);
                PingReply reply = send_ping.Send(hostIP, timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                {
                    launchTarkov(hostPort);
                    btnPlaySPTAKI.Text = "Quit Fika";
                    toggleUI(true);
                    lblServers.Select();
                }
            }
            else
            {
                bool akiServerIsRunning = IsAkiServerRunning(gamePath);
                if (!akiServerIsRunning)
                {
                    if (txtAccountAID.Text.ToLower() == "incomplete profile")
                    {
                        showMessage("You\'re trying to launch SPT with an incomplete profile." + Environment.NewLine +
                            "" + Environment.NewLine +
                            "You can fix this by doing the following:" + Environment.NewLine + Environment.NewLine +
                            "a) Running your incomplete profile via the AKI launcher." + Environment.NewLine +
                            "b) Selecting a working profile.", this.Text);
                        btnAccount.PerformClick();
                    }
                    else
                    {
                        killProcesses();
                        beginLaunching();
                    }
                }
                else
                {
                    string content = $"It looks like the AKI server is running. We don't support manual launching, so we'll restart the game for you:" + Environment.NewLine + Environment.NewLine +
                                     $"Click YES to restart." + Environment.NewLine +
                                     $"Click NO to cancel.";
                    if (MessageBox.Show(content, this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        killProcesses();

                        if (txtAccountAID.Text.ToLower() == "incomplete profile")
                        {
                            showMessage("You\'re trying to launch SPT with an incomplete profile." + Environment.NewLine +
                                "" + Environment.NewLine +
                                "You can fix this by doing the following:" + Environment.NewLine + Environment.NewLine +
                                "a) Running your incomplete profile via the AKI launcher." + Environment.NewLine +
                                "b) Selecting a working profile.", this.Text);
                            btnAccount.PerformClick();
                        }
                        else
                        {
                            beginLaunching();
                        }
                    }
                }
                lblServers.Select();
            }
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

            btnPlaySPTAKI.Text = "Quit SPT";
            btnPlaySPTAKI.Enabled = false;

            Timer tmr = new Timer();
            tmr.Interval = 750;
            tmr.Tick += (sender, e) =>
            {
                btnPlaySPTAKI.Enabled = true;
                tmr.Stop();
                tmr.Dispose();
            };
            tmr.Start();
        }

        private void killAkiServer()
        {
            string akiServerProcess = "SPT.Server";
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

                if (btnCloseAkiServer.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate {
                        btnCloseAkiServer.Text = "Run server";
                        txtServerIsRunning.Text = "❌ Server is closed";
                        txtServerIsRunning.ForeColor = Color.Red;
                    });
                else
                {
                    btnCloseAkiServer.Text = "Run server";
                    txtServerIsRunning.Text = "❌ Server is closed";
                    txtServerIsRunning.ForeColor = Color.Red;
                }

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
            string akiServerProcess = "SPT.Server";
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

            killAkiServer();

            if (btnCloseAkiServer.InvokeRequired)
                BeginInvoke((MethodInvoker)delegate {btnCloseAkiServer.Enabled = false;});
            else
                btnCloseAkiServer.Enabled = false;

            hasStopped = true;
            serverIsRunning = false;

            if (chkLogOnExit.Checked)
            {
                try
                {
                    string fullServerOutput = "";

                    if (akiOutput.InvokeRequired)
                        BeginInvoke((MethodInvoker)delegate { fullServerOutput = akiOutput.Text; });
                    else
                        fullServerOutput = akiOutput.Text;

                    DateTime now = DateTime.Now;
                    string formattedTime = now.ToString("yyyy-MM-dd HH-mm-ss");
                    string filename = $"{formattedTime} server.log";

                    string path = Path.Combine(logsFolder, filename);
                    File.WriteAllText(path, fullServerOutput);
                }
                catch (Exception ex)
                {
                    showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                        Environment.NewLine +
                                        Environment.NewLine +
                                        ex.ToString(), this.Text);
                }
            }

            if (akiOutput.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate { akiOutput.Clear(); });
            }
            else
            {
                akiOutput.Clear();
            }

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

            Timer tmr = new Timer();
            tmr.Interval = 750;
            tmr.Tick += (sender, e) =>
            {
                if (btnPlaySPTAKI.InvokeRequired)
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        btnPlaySPTAKI.Text = "Play SPT";
                        btnPlaySPTAKI.Enabled = true;
                    });
                }
                else
                {
                    btnPlaySPTAKI.Text = "Play SPT";
                    btnPlaySPTAKI.Enabled = true;
                }

                tmr.Stop();
                tmr.Dispose();
            };
            tmr.Start();
        }

        private void killTarkov()
        {
            string tarkovProcess = "EscapeFromTarkov";
            Process[] procs = Process.GetProcessesByName(tarkovProcess);
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
                                    Debug.WriteLine("Controlled exception access is denied occurred. If administrator account, ignore");
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

        private void runServerOnly()
        {
            Task.Delay(300);
            string serverFolder = txtGameInstallFolder.Text;

            string launcherProcess = "SPT.Server";
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
            akiServer.StartInfo.FileName = "SPT.Server.exe";
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

                /*
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
                
                */

                if (txtServerIsRunning.InvokeRequired)
                    BeginInvoke((MethodInvoker)delegate {
                        txtServerIsRunning.Text = "✔️ Server is launching...";
                        txtServerIsRunning.ForeColor = Color.SeaGreen;
                    });
                else
                {
                    txtServerIsRunning.Text = "✔️ Server is launching...";
                    txtServerIsRunning.ForeColor = Color.SeaGreen;
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
                                    ex.ToString(), this.Text);
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void launchServer()
        {
            Task.Delay(300);
            string serverFolder = txtGameInstallFolder.Text;

            string launcherProcess = "SPT.Server";
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
            akiServer.StartInfo.FileName = "SPT.Server.exe";
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
                    BeginInvoke((MethodInvoker)delegate {
                        // btnCloseAkiServer.Enabled = true;
                        btnCloseAkiServer.Text = "Close server";
                        txtServerIsRunning.Text = "✔️ Server is launching...";
                        txtServerIsRunning.ForeColor = Color.SeaGreen;
                    });
                else
                {
                    // btnCloseAkiServer.Enabled = true;
                    btnCloseAkiServer.Text = "Close server";
                    txtServerIsRunning.Text = "✔️ Server is launching...";
                    txtServerIsRunning.ForeColor = Color.SeaGreen;
                }

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
                                    ex.ToString(), this.Text);
            }
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void launchTarkov(int currentPort)
        {
            if (Properties.Settings.Default.isFikaEnabled)
            {
                string hostIP = Properties.Settings.Default.fikaIP;
                int hostPort = currentPort;
                string fikaProfile = Properties.Settings.Default.currentFikaProfile;

                if (string.IsNullOrEmpty(fikaProfile)) return;

                ProcessStartInfo _tarkov = new ProcessStartInfo();
                string serverFolder = txtGameInstallFolder.Text;
                string localhostAddress = hostIP;
                IPAddress fetchedHostIP;

                string fikaFolder = Path.Combine(serverFolder, "user", "fika");
                _tarkov.FileName = Path.Combine(serverFolder, "EscapeFromTarkov.exe");

                if (IPAddress.TryParse(localhostAddress, out fetchedHostIP))
                {
                    if (hostPort != 0)
                        _tarkov.Arguments = $"-token={fikaProfile} -config={{'BackendUrl':'https://{fetchedHostIP}:{hostPort}','Version':'live','MatchingVersion':'live'}}";
                    else
                        _tarkov.Arguments = $"-token={fikaProfile} -config={{'BackendUrl':'https://{fetchedHostIP}:6969','Version':'live','MatchingVersion':'live'}}";
                }
                else
                {
                    showMessage("The IP address " + localhostAddress + " could not be parsed. Please adjust your Fika connection settings and try again." + Environment.NewLine + Environment.NewLine +
                                "Expected output: https://123.456.789.0123:1234", Text);
                    return;
                }

                if (chkMinimizeOnGameLaunch.Checked)
                {
                    if (WindowState != FormWindowState.Normal &&
                        WindowState != FormWindowState.Minimized &&
                        WindowState != FormWindowState.Maximized &&
                        !this.Visible &&
                        !trayIcon.Visible)
                    {
                        if (trayIcon != null)
                        {
                            Hide();
                            trayIcon.Visible = true;
                            trayIcon.ShowBalloonTip(2000);
                        }
                    }
                }

                Process tarkovGame = new Process();

                Environment.CurrentDirectory = serverFolder;
                tarkovGame.StartInfo.WorkingDirectory = serverFolder;
                tarkovGame.StartInfo = _tarkov;
                tarkovGame.Start();
                Environment.CurrentDirectory = currentDirectory;
            }
            else
            {
                ProcessStartInfo _tarkov = new ProcessStartInfo();
                string serverFolder = txtGameInstallFolder.Text;
                string localhostAddress = Properties.Settings.Default.localhostIP;
                IPAddress localhostIP;

                string content = File.ReadAllText(profiles_dict);
                JObject objectContent = JObject.Parse(content);
                string profile = (string)objectContent[serverFolder];

                if (profile != null && !string.IsNullOrEmpty(profile))
                {
                    string aid = profile;
                    _tarkov.FileName = Path.Combine(serverFolder, "EscapeFromTarkov.exe");

                    if (IPAddress.TryParse(localhostAddress, out localhostIP))
                    {
                        if (currentPort != 0)
                            _tarkov.Arguments = $"-token={aid} -config={{'BackendUrl':'https://{localhostIP}:{currentPort}','Version':'live','MatchingVersion':'live'}}";
                        else
                            _tarkov.Arguments = $"-token={aid} -config={{'BackendUrl':'https://{localhostIP}:6969','Version':'live','MatchingVersion':'live'}}";
                    }
                    else
                    {
                        if (currentPort != 0)
                            _tarkov.Arguments = $"-token={aid} -config={{'BackendUrl':'https://127.0.0.1:{currentPort}','Version':'live','MatchingVersion':'live'}}";
                        else
                            _tarkov.Arguments = $"-token={aid} -config={{'BackendUrl':'https://127.0.0.1:6969','Version':'live','MatchingVersion':'live'}}";
                    }

                    if (chkMinimizeOnGameLaunch.Checked)
                    {
                        if (WindowState != FormWindowState.Normal &&
                            WindowState != FormWindowState.Minimized &&
                            WindowState != FormWindowState.Maximized &&
                            !this.Visible &&
                            !trayIcon.Visible)
                        {
                            if (trayIcon != null)
                            {
                                Hide();
                                trayIcon.Visible = true;
                                trayIcon.ShowBalloonTip(2000);
                            }
                        }
                    }

                    Process tarkovGame = new Process();
                    tarkovGame.StartInfo = _tarkov;
                    tarkovGame.Start();
                }
            }
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
                                    ex.ToString(), this.Text);
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
                                btnSPTAKI.PerformClick();
                            });
                        }
                        else
                        {
                            Show();
                            this.WindowState = FormWindowState.Normal;
                            btnSPTAKI.PerformClick();
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
                string aki_server = "SPT.Server";
                bool isServerRunning = Process.GetProcesses().Any(p => p.ProcessName.Equals(aki_server, StringComparison.OrdinalIgnoreCase));

                if (!isServerRunning)
                {
                    if (!hasNotifiedUser)
                    {
                        showMessage("It appears that the server has closed. This message will only show once." + Environment.NewLine +
                            "Escape From Tarkov will not be closed.", this.Text);
                        hasNotifiedUser = true;
                    }
                }
                else 
                {
                    if (btnCloseAkiServer.InvokeRequired)
                        BeginInvoke((MethodInvoker)delegate {
                            btnCloseAkiServer.Text = "Force-close server";
                            txtServerIsRunning.Text = "✔️ Server is running";
                            txtServerIsRunning.ForeColor = Color.SeaGreen;
                        });
                    else
                    {
                        btnCloseAkiServer.Text = "Force-close server";
                        txtServerIsRunning.Text = "✔️ Server is running";
                        txtServerIsRunning.ForeColor = Color.SeaGreen;
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
            string portPath = Path.Combine(serverFolder, "SPT_Data", "Server", "database", "server.json");
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
                                "Max duration reached, falling back. Please diagnose your server and try again.", this.Text);

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
                showMessage("An error occurred: " + e.Error.Message, this.Text);
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

                        if (btnCloseAkiServer.InvokeRequired)
                            BeginInvoke((MethodInvoker)delegate {
                                btnCloseAkiServer.Text = "Force-close server";
                                txtServerIsRunning.Text = "✔️ Server is running";
                                txtServerIsRunning.ForeColor = Color.SeaGreen;
                            });
                        else
                        {
                            btnCloseAkiServer.Text = "Force-close server";
                            txtServerIsRunning.Text = "✔️ Server is running";
                            txtServerIsRunning.ForeColor = Color.SeaGreen;
                        }

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
                runServerOnly();
            }
            else if (btnCloseAkiServer.Text.ToLower() == "force-close server")
            {
                string aki_server = "SPT.Server";
                string eft_process = "EscapeFromTarkov";
                bool isServerRunning = Process.GetProcesses().Any(p => p.ProcessName.Equals(aki_server, StringComparison.OrdinalIgnoreCase));
                bool isEFTRunning = Process.GetProcesses().Any(p => p.ProcessName.Equals(eft_process, StringComparison.OrdinalIgnoreCase));

                if (!isServerRunning && !isEFTRunning)
                {
                    showMessage("SPT is not running!", this.Text);
                }
                else
                {
                    if (btnCloseAkiServer.InvokeRequired)
                        BeginInvoke((MethodInvoker)delegate {
                            btnCloseAkiServer.Text = "Run server";
                            txtServerIsRunning.Text = "❌ Server is closed";
                            txtServerIsRunning.ForeColor = Color.Red;
                        });
                    else
                    {
                        btnCloseAkiServer.Text = "Run server";
                        txtServerIsRunning.Text = "❌ Server is closed";
                        txtServerIsRunning.ForeColor = Color.Red;
                    }

                    killAkiServer();
                }
                lblServers.Select();
            }
            else if (btnCloseAkiServer.Text.ToLower() == "restart server")
            {
                killAkiServer();
                runServerOnly();
            }
            else if (btnCloseAkiServer.Text.ToLower().Contains("force-quit SPT"))
            {
                if (MessageBox.Show("Are you sure that you want to force-close SPT?" + Environment.NewLine + Environment.NewLine +
                    "Progress maybe lost.", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    performClosing();
                }
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
                mainDir = txtGameInstallFolder.Text;

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
                                    ex.ToString(), this.Text);
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
        }

        private void btnWorkshop_Click(object sender, EventArgs e)
        {
            searchForm frm = new searchForm();
            frm.ShowDialog();
        }

        private void txtSetDisplayName_TextChanged(object sender, EventArgs e)
        {
            btnSetDisplayName.Text = "Set as folder name";

            if (txtSetDisplayName.Text.Length > 0)
                btnSetDisplayName.Text = "Set as custom name";
        }

        private void btnOpenAutostart_Click(object sender, EventArgs e)
        {
            string autostartFile = Path.Combine(currentDirectory, "autostart.txt");
            bool autostartExists = File.Exists(autostartFile);
            if (autostartExists)
            {
                ProcessStartInfo newApp = new ProcessStartInfo();
                newApp.WorkingDirectory = Path.GetDirectoryName(autostartFile);
                newApp.FileName = Path.GetFileName(autostartFile);
                newApp.UseShellExecute = true;
                newApp.Verb = "open";

                Process.Start(newApp);
            }
            else
            {
                string content =
                            $"autostart=false" + Environment.NewLine;

                try
                {
                    File.WriteAllText(autostartFile, content);

                    ProcessStartInfo newApp = new ProcessStartInfo();
                    newApp.WorkingDirectory = Path.GetDirectoryName(autostartFile);
                    newApp.FileName = Path.GetFileName(autostartFile);
                    newApp.UseShellExecute = true;
                    newApp.Verb = "open";

                    Process.Start(newApp);
                }
                catch (Exception ex)
                {
                    showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                Environment.NewLine +
                                Environment.NewLine +
                                ex.ToString(), this.Text);
                }
            }
        }

        private void btnOnServerDoubleClick_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnOnServerDoubleClick.Text.ToLower())
            {
                case "do nothing":
                    Properties.Settings.Default.onServerDoubleClick = true;
                    btnOnServerDoubleClick.Text = "Play SPT";
                    break;
                case "play SPT":
                    Properties.Settings.Default.onServerDoubleClick = false;
                    btnOnServerDoubleClick.Text = "Do nothing";
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void btnCloseAkiServer_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.MouseButtons & MouseButtons.Right) != 0)
            {
                switch (btnCloseAkiServer.Text.ToLower())
                {
                    case "run server":
                        btnCloseAkiServer.Text = "Close server";
                        break;
                    case "close server":
                        btnCloseAkiServer.Text = "Restart server";
                        break;
                    case "restart server":
                        btnCloseAkiServer.Text = "Run server";
                        break;
                }
            }
            else
            {

            }
        }

        private void btnAutostartConfig_Click(object sender, EventArgs e)
        {
            // DevTools frm = new DevTools();
            // frm.ShowDialog();
            showMessage("This feature is currently a work-in-progress, we apologize for the inconvenience!", this.Text);
        }

        private void chkLogOnExit_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.logOnExit = chkLogOnExit.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnOpenLatestServerLog_Click(object sender, EventArgs e)
        {
            bool logsFolderExists = Directory.Exists(logsFolder);
            if (logsFolderExists)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(logsFolder);
                FileInfo[] files = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).ToArray();

                if (files.Length > 0)
                {
                    // Get the information about the last modified file
                    FileInfo lastModifiedFile = files[0];

                    string fullName = lastModifiedFile.FullName;

                    ProcessStartInfo logFile = new ProcessStartInfo();
                    logFile.WorkingDirectory = logsFolder;
                    logFile.FileName = Path.GetFileName(fullName);
                    logFile.UseShellExecute = true;
                    logFile.Verb = "open";
                    Process.Start(logFile);
                }
            }
        }

        private void btnMinimizeToTray_Click(object sender, EventArgs e)
        {
            if (trayIcon != null)
            {
                Hide();
                trayIcon.Visible = true;
                trayIcon.ShowBalloonTip(2000);
            }
        }

        private void btnRegenerateProfileDict_Click(object sender, EventArgs e)
        {
            bool doesProfileDictExist = File.Exists(profiles_dict);
            if (doesProfileDictExist)
            {
                File.Delete(profiles_dict);
                generateProfileDictionary();
            }
            else
            {
                showMessage("Couldn\'t detect the profile dictionary, regenerating...", this.Text);
                generateProfileDictionary();
            }
        }

        private void btnSaveOutputToFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to save the current server output to file?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //
            }
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            trayIcon.Visible = false;
        }

        private void chkMinimizeOnGameLaunch_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.minimizeOnGameLaunch = chkMinimizeOnGameLaunch.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnWhenLauncherExits_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnWhenLauncherExits.Text.ToLower())
            {
                case "do nothing":
                    Properties.Settings.Default.whenLauncherCloses = false;
                    btnWhenLauncherExits.Text = "Show pop-up";
                    break;
                case "show pop-up":
                    Properties.Settings.Default.whenLauncherCloses = true;
                    btnWhenLauncherExits.Text = "Do nothing";
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void btnModInstaller_Click(object sender, EventArgs e)
        {
            mainDir = txtGameInstallFolder.Text;
            string sptModInstaller = Path.Combine(mainDir, "SPT Mods Installer.exe");
            bool sptModInstallerExists = File.Exists(sptModInstaller);

            if (sptModInstallerExists)
            {
                try
                {
                    string fullPath = sptModInstaller;
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
                                ex.ToString(), this.Text);
                }
            }
            else
            {
                if (MessageBox.Show("SPT Mod Installer does not appear to be present. Would you like to download it?",
                    this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        string fullPath = "https://hub.sp-tarkov.com/files/file/1742-spt-mods-installer/";
                        Process.Start(fullPath);
                    }
                    catch (Exception ex)
                    {
                        showMessage("We appear to have run into a problem. If you\'re unsure what this is about, please contact the developer." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    ex.ToString(), this.Text);
                    }
                }
            }
            lblServers.Select();
        }

        private void btnWhenLauncherExits_Click(object sender, EventArgs e)
        {

        }

        private void btnDevMode_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnDevMode.Text.ToLower())
            {
                case "enabled":
                    Properties.Settings.Default.devMode = false;
                    btnDevMode.Text = "Disabled";
                    panelIPAddress.Visible = false;
                    break;
                case "disabled":
                    Properties.Settings.Default.devMode = true;
                    btnDevMode.Text = "Enabled";
                    panelIPAddress.Visible = true;
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void btnSetNewIP_MouseDown(object sender, MouseEventArgs e)
        {
            EditLocalhost editForm = new EditLocalhost(titleEditLocalIP);
            editForm.ShowDialog();
        }

        private void btnSetNewIP_Click(object sender, EventArgs e)
        {
        }

        private void btnSetNewPort_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void btnSetNewPort_Click(object sender, EventArgs e)
        {
        }

        private void btnFikaMode_Click(object sender, EventArgs e)
        {
        }

        private void btnFikaMode_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnFikaMode.Text.ToLower())
            {
                case "enabled":
                    Properties.Settings.Default.isFikaEnabled = false;
                    btnAdjustFikaSettings.Visible = false;
                    btnFikaMode.Text = "Disabled";
                    btnPlaySPTAKI.Text = "Play SPT";
                    break;

                case "disabled":
                    Properties.Settings.Default.isFikaEnabled = true;
                    btnAdjustFikaSettings.Visible = true;
                    btnFikaMode.Text = "Enabled";
                    btnPlaySPTAKI.Text = "Play Fika";
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void btnAdjustFikaSettings_Click(object sender, EventArgs e)
        {
            AdjustFikaSettings fikaForm = new AdjustFikaSettings();
            fikaForm.ShowDialog();
        }

        private void btnAppCloseNotification_Click(object sender, EventArgs e)
        {
        }

        private void btnFikaMode_MouseDown(object sender, MouseEventArgs e)
        {
            switch (btnFikaMode.Text.ToLower())
            {
                case "enabled":
                    Properties.Settings.Default.closeOnExit = true;
                    btnFikaMode.Text = "Disabled";
                    btnPlaySPTAKI.Text = "Play SPT";
                    break;
                case "disabled":
                    Properties.Settings.Default.closeOnExit = false;
                    btnFikaMode.Text = "Enabled";
                    btnPlaySPTAKI.Text = "Play Fika";
                    break;
            }

            Properties.Settings.Default.Save();
            lblServers.Select();
        }

        private void btnFikaMode_MouseDown(object sender, MouseEventArgs e)
        {
        }
    }
}
