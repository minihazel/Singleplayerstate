using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Singleplayerstate
{
    public partial class DevTools : Form
    {
        public string availableServers;
        public DevTools()
        {
            InitializeComponent();
        }

        private void DevTools_Load(object sender, EventArgs e)
        {
            availableServers = Properties.Settings.Default.availableServers;
            JObject servers = JObject.Parse(availableServers);

            foreach (var item in servers)
            {
                listAvailableInstallations.Items.Add(item.Key);
            }

        }

        private void listAutostart_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string actualText = listAutostart.GetItemText(listAutostart.SelectedItem);
            string otherText = listAutostart.GetItemText(listAutostart.Items[1]);

            if (listAutostart.SelectedIndex == 0)
            {
                if (actualText.ToLower().Contains("false"))
                {
                    string[] strings = { "autostart=true", otherText };
                    listAutostart.Items.Clear();
                    foreach (string s in strings)
                    {
                        listAutostart.Items.Add(s);
                    }
                }
                else
                {
                    string[] strings = { "autostart=false", otherText };
                    listAutostart.Items.Clear();
                    foreach (string s in strings)
                    {
                        listAutostart.Items.Add(s);
                    }
                }
            }
        }

        private void listAvailableInstallations_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listAvailableInstallations.SelectedIndex > -1)
            {
                string curText = listAvailableInstallations.GetItemText
                    (listAvailableInstallations.SelectedItem);
                if (curText != null)
                {
                    listAutostart.Items.RemoveAt(1);
                    listAutostart.Items.Add(curText);
                }
            }
        }

        private void listAvailableInstallations_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnSetAsAutostartOption_Click(object sender, EventArgs e)
        {
            if (listAvailableInstallations.SelectedIndex > -1)
            {
                string curText = listAvailableInstallations.GetItemText
                    (listAvailableInstallations.SelectedItem);
                if (curText != null)
                {
                    listAutostart.Items.RemoveAt(1);
                    listAutostart.Items.Add(curText);
                }
            }
        }

        private void listAvailableInstallations_KeyDown(object sender, KeyEventArgs e)
        {
            string curText = listAvailableInstallations.GetItemText
                    (listAvailableInstallations.SelectedItem);

            if (e.KeyCode == Keys.Delete)
            {
                string content = $"Do you want to delete {curText}?";
                MessageBoxButtons btns = MessageBoxButtons.YesNo;

                if (MessageBox.Show(content, this.Text, btns) == DialogResult.Yes)
                {

                }
            }
        }
    }
}
