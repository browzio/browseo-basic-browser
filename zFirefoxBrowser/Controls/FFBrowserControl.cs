using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;

namespace zFirefoxBrowser.Controls
{
    public partial class FFBrowserControl : UserControl
    {
        public event Action<bool> OnBrowserLoadingChanged = delegate { };
        public event Action<string> OnBrowserTitleChanged = delegate { };
        public event Action<string> OnBrowserAddressChanged = delegate { };
        public event Action<string> OnBrowserStatusChanged = delegate { };
        public event Action<string> OnBrowserMessageChanged = delegate { };
        public event Action<string> OnCreateNewTab = delegate { };

        public event Action<string> OnBrowserContextMenuClicked = delegate { };

        public GeckoWebBrowser Browser { get; private set; }

        public FFBrowserControl()
        {
            InitializeComponent();
        }

        public void initBrowser(string url)
        {
            Browser = new GeckoWebBrowser();
            Browser.Dock = DockStyle.Fill;
            

            Browser.Navigate(url);

            
            //url in Navigating event may be the mapped version,
            //e.g. about:config in Navigating event is jar:file:///<xulrunner>/omni.ja!/chrome/toolkit/content/global/config.xul
            Browser.Navigating += (s, ee) =>
            {
                OnBrowserLoadingChanged(true);

                Console.WriteLine("Navigating: url: " + ee.Uri + ", top: " + ee.DomWindowTopLevel);
            };
            Browser.Navigated += (s, ee) =>
            {
                //OnBrowserLoadingChanged(false);
               // OnBrowserTitleChanged(Convert.ToString(ee.Uri));

                Console.WriteLine("Navigated: url: " + ee.Uri + ", top: " + ee.DomWindowTopLevel, ", errorPage: " + ee.IsErrorPage);
            };

            Browser.Retargeted += (s, ee) =>
            {
                var ch = ee.Request as Gecko.Net.Channel;
                Console.WriteLine("Retargeted: url: " + ee.Uri + ", contentType: " + ch.ContentType + ", top: " + ee.DomWindowTopLevel);
            };
            Browser.DocumentCompleted += (s, ee) =>
            {
                OnBrowserLoadingChanged(false);
                OnBrowserAddressChanged(ee.Uri.ToString());
                Console.WriteLine("DocumentCompleted: url: " + ee.Uri + ", top: " + ee.IsTopLevel);
            };


            Browser.DocumentTitleChanged += (s, e) =>
            {
                OnBrowserTitleChanged(Browser.DocumentTitle);
            };

            Browser.StatusTextChanged += (s,e)=>
            {
                OnBrowserStatusChanged(Browser.StatusText);
            };

            Browser.ConsoleMessage += (e, s) =>
            {
                OnBrowserMessageChanged(s.Message);
            };

            Browser.ShowContextMenu += (s, e) =>
            {
                e.ContextMenu.MenuItems.Add("-");

                //model.AddItem(333, "To Social Enagager");
                MenuItem tse = new MenuItem() { Name = "888", Text = "To Social Enagager" };
                tse.Click += Tse_Click;
                e.ContextMenu.MenuItems.Add(tse);

                e.ContextMenu.MenuItems.Add("-");

                //model.AddItem(222, "Curaste...");
                MenuItem cur = new MenuItem() { Name = "222", Text = "Curaste..." };
                cur.Click += Tse_Click;
                e.ContextMenu.MenuItems.Add(cur);

                //model.AddItem(666, "Curate It");
                MenuItem ci = new MenuItem() { Name = "666", Text = "Curate It" };
                ci.Click += Tse_Click;
                e.ContextMenu.MenuItems.Add(ci);

                if (Browser.Url!=null && 
                Browser.Url.ToString().ToLower().Contains("www.facebook.com/search") || Browser.Url.ToString().ToLower().Contains("facebook.com/groups/?category=membership"))
                {
                    e.ContextMenu.MenuItems.Add("-");

                    //model.AddItem(555, "Dominate");
                    MenuItem d = new MenuItem() { Name = "555", Text = "Dominate" };
                    d.Click += Tse_Click;
                    e.ContextMenu.MenuItems.Add(d);

                    //model.AddItem(444, "Dominate All");
                    MenuItem da = new MenuItem() { Name = "444", Text = "Dominate All" };
                    da.Click += Tse_Click;
                    e.ContextMenu.MenuItems.Add(da);
                }
            };

            // Popup window management.
            Browser.CreateWindow += (s, ee) =>
            {
                // A naive popup blocker, demonstrating popup cancelling.
                Console.WriteLine("A popup is trying to show: " + ee.Uri);
                if (ee.Uri.StartsWith("http://annoying-site.com") || ee.Uri.Contains("about:blank"))
                {
                    ee.Cancel = true;
                    // Console.WriteLine("A popup is blocked: " + ee.Uri);
                    return;
                }
                string target = ee.Uri.ToLower();
                if (!target.Contains("microsoft") &&
                    !target.Contains("facebook") &&
                    !target.Contains("twitter") &&
                    !target.Contains("gplus") &&
                    !target.Contains("session/") &&
                    !target.Contains("yahoo") &&
                    !target.Contains("login") && !target.Contains("connect") && !target.Contains("oauth") && !target.Contains("signup"))
                {
                    OnCreateNewTab(ee.Uri);
                    ee.Cancel = true;
                }
                // For <a target="_blank"> and window.open() without specs(3rd param),
                // e.Flags == GeckoWindowFlags.All, and we load it in a new tab;
                // otherwise, load it in a popup window, which is maximized by default.
                // This simulates firefox's behavior.
                //if (ee.Flags == GeckoWindowFlags.All)
                //    ee.WebBrowser = AddTab();
                //else
                //{
                //    var wa = System.Windows.Forms.Screen.GetWorkingArea(this);
                //    e.InitialWidth = wa.Width;
                //    e.InitialHeight = wa.Height;
                //}
            };


            this.SuspendLayout();
            this.Controls.Add(Browser);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void Tse_Click(object sender, EventArgs e)
        {
            OnBrowserContextMenuClicked((sender as MenuItem).Name);
        }

        internal void Navigate(string addressEditable)
        {
            string url = addressEditable;

            if (!url.Contains(".") && url.Length > 1 && !url.Contains("about:config"))
            {
                url = url.Replace(' ', '+');
                url = String.Format(@"http://google.com/search?v=1.0&q={0}", url);
            }

            Browser.Navigate(url);
        }

        internal void Back()
        {
            Browser.GoBack();
        }

        internal void Forward()
        {
            Browser.GoForward();
        }

        internal void Reload()
        {
            Browser.Reload();
        }
    }
}

// add a handler showing how to view the DOM
//			browser.DocumentCompleted += (s, e) => 	TestQueryingOfDom(browser);

// add a handler showing how to modify the DOM.
//			browser.DocumentCompleted += (s, e) => TestModifyingDom(browser);

// add a handle to detect when user modifies a contentEditable part of the document.
//browser.DomInput += (sender, args) => MessageBox.Show(String.Format("User modified element {0}", (args.Target.CastToGeckoElement() as GeckoHtmlElement).OuterHtml));

// Uncomment this to stop links from navigating.
// browser.DomClick += StopLinksNavigating;

// Demo use of ReadyStateChange.
// For some special page, e.g. about:config browser.Document is null.
//browser.ReadyStateChange += (s, e) => this.Text = browser.Document != null ? browser.Document.ReadyState : "";
// browser.EnableDefaultFullscreen();
