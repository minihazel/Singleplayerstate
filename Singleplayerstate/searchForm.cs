using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Singleplayerstate
{
    public partial class searchForm : Form
    {
        public searchForm()
        {
            InitializeComponent();
        }

        private void searchForm_Load(object sender, EventArgs e)
        {
            txtSearchString.Clear();
            txtSearchString.Select();
        }

        private void txtSearchString_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                if (txtSearchString.Text == "")
                {
                    Process.Start("https://hub.sp-tarkov.com/");
                    Close();
                }
                else
                {
                    string keyword = txtSearchString.Text.ToLower();
                    string googleURI = null;

                    if (keyword.Contains("realism"))
                    {
                        googleURI = $"https://github.com/space-commits/SPT-Realism-Mod-Client/releases";
                    }
                    else if (keyword.Contains("hub"))
                    {
                        googleURI = $"https://hub.sp-tarkov.com/";
                    }
                    else if (keyword.Contains("spt") ||
                        keyword.Contains("spt-aki") ||
                        keyword.Contains("spt aki"))
                    {
                        googleURI = $"https://dev.sp-tarkov.com/SPT-AKI/Stable-releases/releases/";
                    }
                    else if (keyword.Contains("installer"))
                    {
                        googleURI = $"https://ligma.waffle-lord.net/SPTInstaller.exe";
                    }
                    else if (keyword.Contains("sain"))
                    {
                        googleURI = $"https://github.com/Solarint/SAIN/releases/";
                    }
                    else if (keyword.Contains("swag"))
                    {
                        googleURI = $"https://github.com/p-kossa/nookys-swag-presets-spt/releases/";
                    }
                    else if (keyword.Contains("donuts"))
                    {
                        googleURI = $"https://github.com/p-kossa/nookys-swag-presets-spt/releases/";
                    }
                    else if (keyword.Contains("ai limit"))
                    {
                        googleURI = $"https://github.com/dvize/SPT-AILimit/releases/";
                    }
                    else if (keyword.Contains("ram"))
                    {
                        googleURI = $"https://drive.google.com/file/d/1xT0bdTeJJc5AdFdrnlQCr9YULTbs2afO/view?usp=drive_link";
                    }
                    else if (keyword.Contains("clutter"))
                    {
                        googleURI = $"https://github.com/CJ-SPT/DeClutterer/releases/";
                    }
                    else if (keyword.Contains("demi"))
                    {
                        googleURI = $"https://github.com/minihazel/Deminvincibility/releases/";
                    }
                    else if (keyword.Contains("deminvincibility"))
                    {
                        googleURI = $"https://github.com/minihazel/Deminvincibility/releases/";
                    }
                    else if (keyword.Contains("minimalistlauncher") ||
                        keyword.Contains("minimal launcher") ||
                        keyword.Contains("minimallauncher"))
                    {
                        googleURI = $"https://github.com/minihazel/Singleplayerstate/releases";
                    }
                    else
                    {
                        googleURI = $"https://www.google.com/search?q=spt-aki+{keyword}";
                    }

                    Process.Start(googleURI);
                    Close();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
