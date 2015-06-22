using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xilium.CefGlue.Client
{
    public partial class BrowserForSocialShare : Form
    {
        public BrowserForSocialShare()
        {
            InitializeComponent();
        }

        private void browserCntrl1_OnBrowserStatusChanged(string obj)
        {
            Text = "Loaded.";
        }
    }
}
