using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Singleplayerstate
{
    public partial class msgBoard : Form
    {
        private const int HTBOTTOMRIGHT = 17;
        private const int WMNCHITTEST = 0x84;

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        private static extern bool ReleaseCapture();

        private bool isResizing = false;
        private Point lastMousePosition;
        private mainForm mainForm;

        public msgBoard()
        {
            InitializeComponent();
        }

        private void msgBoard_Load(object sender, EventArgs e)
        {

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
        }

        private void btnCopyMessage_MouseEnter(object sender, EventArgs e)
        {
            btnCopyMessage.BackgroundImage = Properties.Resources.layers_selected;
        }

        private void btnCopyMessage_MouseLeave(object sender, EventArgs e)
        {
            btnCopyMessage.BackgroundImage = Properties.Resources.layers;
        }
    }
}
