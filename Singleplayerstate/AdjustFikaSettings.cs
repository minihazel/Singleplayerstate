using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace Singleplayerstate
{
    public partial class AdjustFikaSettings : Form
    {
        public AdjustFikaSettings()
        {
            InitializeComponent();
        }

        private void AdjustFikaSettings_Load(object sender, EventArgs e)
        {
            txtIPString.Clear();
            txtPortString.Select();

            string fikaHostIP = Properties.Settings.Default.fikaIP;
            int fikaHostPort = Properties.Settings.Default.fikaPort;

            if (Properties.Settings.Default.isFikaEnabled)
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.fikaIP))
                    txtIPString.Text = Properties.Settings.Default.fikaIP;
                else
                    txtIPString.Text = "127.0.0.1";

                if (!string.IsNullOrEmpty(fikaHostPort.ToString()))
                    txtPortString.Text = fikaHostPort.ToString();
                else
                    txtIPString.Text = "6969";

                txtIPString.Select();
            }
        }

        private void btnConfirmSettings_Click(object sender, EventArgs e)
        {
            string value = txtIPString.Text;
            int portValue = int.Parse(txtPortString.Text);

            try
            {
                txtIPString.Clear();
                txtPortString.Clear();

                IPAddress existingIP = IPAddress.Parse(value);
                Properties.Settings.Default.fikaIP = existingIP.ToString();
                Properties.Settings.Default.fikaPort = portValue;    
                Properties.Settings.Default.Save();

                string fullAddress = "https://" + Properties.Settings.Default.fikaIP + ":" + portValue.ToString();
                MessageBox.Show("Configuration change success! We will now connect to " + fullAddress + " on launch. Disable Fika mode if you want to disable this.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid IP address, please provide a valid address.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtIPString.Select();
            }
        }

        private void btnCancelAction_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtIPString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Shift)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                txtPortString.Select();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                btnConfirmSettings.PerformClick();
            }
        }

        private void txtPortString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                btnConfirmSettings.PerformClick();
            }
        }
    }
}
