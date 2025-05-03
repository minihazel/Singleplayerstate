using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Singleplayerstate
{
    public partial class EditLocalhost : Form
    {
        private Label _labelToUpdate;

        public EditLocalhost(Label label)
        {
            InitializeComponent();
            _labelToUpdate = label;
        }

        private void EditLocalhost_Load(object sender, EventArgs e)
        {
            txtIPString.Clear();
            txtIPString.Select();
        }

        private void txtIPString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                switch (txtIPString.Text.ToLower())
                {
                    case "":
                        MessageBox.Show("No input detected, please provide a valid address. Alternatively, hit Escape to close this window.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case "reset":
                        Properties.Settings.Default.localhostIP = "127.0.0.1";
                        Properties.Settings.Default.Save();

                        _labelToUpdate.Text = $"Edit local IP address" + Environment.NewLine +
                                              $"(current: https://127.0.0.1:6969)";
                        break;

                    default:
                        string value = txtIPString.Text;
                        try
                        {
                            IPAddress existingIP = IPAddress.Parse(value);
                            Properties.Settings.Default.localhostIP = $"https:\\{existingIP.ToString()}";
                            Properties.Settings.Default.Save();

                            _labelToUpdate.Text = $"Edit local IP address" + Environment.NewLine +
                                                      $"(current: https://{existingIP.ToString()}:6969)";
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Invalid IP address, please provide a valid address.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }

                Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
