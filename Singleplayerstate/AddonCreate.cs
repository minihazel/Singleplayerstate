using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;

namespace Singleplayerstate
{
    public partial class AddonCreate : Form
    {
        private mainForm frm;
        private Panel panelAddons;
        private Dictionary<string, string> folderPaths;
        private string availableAddons;

        public AddonCreate(mainForm frm, Panel panelAddons, Dictionary<string, string> folderPaths, string availableAddons)
        {
            InitializeComponent();
            this.frm = frm;
            this.panelAddons = panelAddons;
            this.folderPaths = folderPaths;
            this.availableAddons = availableAddons;
        }

        private void AddonCreate_Load(object sender, EventArgs e)
        {
            txtSetDisplayName.Clear();
            txtSetAddonPath.Clear();

            txtSetDisplayName.Select();
        }

        private void AddonCreate_FormClosing(object sender, FormClosingEventArgs e)
        {
            frm.toggleUI(true);
            panelAddons.Visible = false;
            panelAddons.Controls.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSetDisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                if (txtSetDisplayName.Text == "")
                {
                    MessageBox.Show("Display name cannot be empty.", this.Text, MessageBoxButtons.OK);
                    txtSetDisplayName.Select();
                }
                else
                {
                    txtSetAddonPath.Select();
                }
            }
        }

        private void txtSetAddonPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                if (txtSetAddonPath.Text == "")
                {
                    MessageBox.Show("Path cannot be empty.", this.Text, MessageBoxButtons.OK);
                    txtSetAddonPath.Select();
                }
                else
                {
                    txtSetAddonPath.Text = txtSetAddonPath.Text.Trim();
                    btnSetAddonPath.PerformClick();
                }
            }
        }

        private void btnSetAddonPath_Click(object sender, EventArgs e)
        {
            if (txtSetDisplayName.Text != "" && txtSetAddonPath.Text != "")
            {
                string displayName = txtSetDisplayName.Text;
                string displayPath = txtSetAddonPath.Text.Replace(Environment.NewLine, "").Replace("\r", "");

                if (MessageBox.Show($"Do you want to create new addon {displayName} with path '{displayPath}\'?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    frm.saveAddon(txtSetDisplayName.Text, txtSetAddonPath.Text);
                    frm.listAddons();
                    this.Close();
                }
                else
                {
                    txtSetAddonPath.Select();
                }
            }
        }

        private void chkPathType_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPathType.Checked)
                chkPathType.Text = "Folder";
            else
                chkPathType.Text = "File";
        }

        private void btnBrowsePath_Click(object sender, EventArgs e)
        {
            if (chkPathType.Checked)
            {
                var browse = new BetterFolderBrowser();
                browse.Title = "Select folder that your addon will point to";
                browse.RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                browse.Multiselect = false;

                if (browse.ShowDialog(this) == DialogResult.OK)
                {
                    string selectedFolder = browse.SelectedFolder.Replace("\\", "/");
                    bool folderExists = Directory.Exists(selectedFolder);
                    if (folderExists)
                    {
                        txtSetAddonPath.Text = selectedFolder;

                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                this.WindowState = FormWindowState.Normal;
                            });
                        }
                        else
                        {
                            this.WindowState = FormWindowState.Normal;
                        }

                        txtSetAddonPath.Select();
                        SendKeys.Send("{RIGHT}");
                    }
                }
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        this.WindowState = FormWindowState.Normal;
                    });
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                }

                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.Title = "Select file that your addon will point to";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.IsFolderPicker = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string fullPath = Path.GetFullPath(dialog.FileName).Replace("\\", "/");

                    if (File.Exists(fullPath))
                    {
                        txtSetAddonPath.Text = fullPath;

                        txtSetAddonPath.Select();
                        SendKeys.Send("{RIGHT}");
                    }
                }
            }
        }
    }
}
