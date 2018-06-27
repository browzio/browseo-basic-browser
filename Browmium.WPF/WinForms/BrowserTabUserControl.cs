using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xilium.CefGlue.WindowsForms;

namespace Browmium.WPF.WinForms
{
    public partial class BrowserTabUserControl : UserControl
    {
        private readonly string _mainTitle;

        public CefWebBrowser BrowserWindow { get; set; }

        public BrowserTabUserControl()
        {
            InitializeComponent();

            _mainTitle = Text;
        }

        public void Init(string startUrl)
        {
            BrowserWindow = new CefWebBrowser();
            BrowserWindow.StartUrl = startUrl;
            BrowserWindow.Dock = DockStyle.Fill;


            Controls.Add(BrowserWindow);


            // BrowserWindow.Browser.GetMainFrame().LoadUrl(startUrl);
        }
    }
}
