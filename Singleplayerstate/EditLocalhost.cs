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

                if (txtIPString.Text == "")
                {
                    Close();
                }
                else if (txtIPString.Text.ToLower() == "reset")
                {
                    Properties.Settings.Default.localhostIP = "127.0.0.1";
                    Properties.Settings.Default.Save();

                    _labelToUpdate.Text = $"Edit local IP address" + Environment.NewLine +
                                          $"(current: 127.0.0.1:6969)";
                }
                else
                {
                    string value = txtIPString.Text;
                    try
                    {
                        IPAddress existingIP = IPAddress.Parse(value);
                        Properties.Settings.Default.localhostIP = existingIP.ToString();

                        _labelToUpdate.Text = $"Edit local IP address" + Environment.NewLine +
                                                  $"(current: {existingIP.ToString()}:{value})";
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Invalid IP address, please provide a valid address.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }
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
