using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Singleplayerstate
{
    public partial class msgBoard : Form
    {
        private const int HTBOTTOMRIGHT = 17;
        private const int WMNCHITTEST = 0x84;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2
            ;
        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 0x01;
        private const int HTCAPTION = 0x02;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr sendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool releaseCapture();

        private const int _WM_NCLBUTTONDOWN = 0xA1;

        private bool isResizing = false;
        private Point lastMousePosition;
        private mainForm mainForm;

        public msgBoard()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WMNCHITTEST && (int)m.Result == HTCLIENT)
            {
                m.Result = (IntPtr)HTCAPTION;
            }
        }

        private void msgBoard_Load(object sender, EventArgs e)
        {
            Bitmap temp = this.Icon.ToBitmap();
            appImage.Image = temp;
        }

        private void msgBoard_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(100, 100, 100);
            int borderWidth = 1;
            Rectangle borderRect = new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            using (Pen borderPen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(borderPen, borderRect);
            }
        }

        private void closeForm()
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            closeForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            closeForm();
        }

        private void msgBoard_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void messageContent_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void mainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnCopyMessage_Click(object sender, EventArgs e)
        {
            if (messageContent.InvokeRequired)
            {
                messageContent.BeginInvoke((MethodInvoker)delegate
                {
                    Clipboard.SetText(messageContent.Text);
                });
            }
            else
                Clipboard.SetText(messageContent.Text);

            statusCopy.Visible = true;
            Timer tmr = new Timer();
            tmr.Interval = 1000;
            tmr.Tick += (_sender, _e) =>
            {
                statusCopy.Visible = false;
                tmr.Stop();
                tmr.Dispose();
            };
            tmr.Start();
        }

        private void btnCopyMessage_MouseEnter(object sender, EventArgs e)
        {
            btnCopyMessage.BackgroundImage = Properties.Resources.layers_selected;
        }

        private void btnCopyMessage_MouseLeave(object sender, EventArgs e)
        {
            btnCopyMessage.BackgroundImage = Properties.Resources.layers;
        }

        private void btnLevelOne_Click(object sender, EventArgs e)
        {
            this.Size = new Size(700, 400);
            statusCopy.Select();
        }

        private void btnLevelTwo_Click(object sender, EventArgs e)
        {
            this.Size = new Size(900, 600);
            statusCopy.Select();
        }

        private void btnLevelThree_Click(object sender, EventArgs e)
        {
            this.Size = new Size(1100, 800);
            statusCopy.Select();
        }
    }
}
