using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zFirefoxBrowser.Controls
{
    /// <summary>
    /// Interaction logic for FFBrowserUserControl.xaml
    /// </summary>
    public partial class FFBrowserUserControl : UserControl
    {
        public FFBrowserUserControl()
        {
            InitializeComponent();

            //this.Loaded += FFBrowserUserControl_Loaded;
        }

        private void FFBrowserUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (gdBrowser.Children.Count > 0) return;
            //Gecko.Xpcom.ProfileDirectory = @"C:\Users\eli\AppData\Local\CacheTesting1";
            string xulrunnerPath = XULRunnerLocator.GetXULRunnerLocation();
            //Xpcom.Initialize(AppDomain.CurrentDomain.BaseDirectory + "FFLibrary");
            Xpcom.Initialize(@"C:\Users\eli\Desktop\move\plugins\WpfHost\bin\Debug\FFLibrary");
            // Uncomment the follow line to enable error page
            GeckoPreferences.User["browser.xul.error_pages.enabled"] = true;

            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;

            GeckoPreferences.User["full-screen-api.enabled"] = true;

            var browser = new GeckoWebBrowser();
           // browser.Dock = System.Windows.Forms.DockStyle.Fill;
            WindowsFormsHost wfh = new WindowsFormsHost();
            wfh.Child = browser;
            gdBrowser.Children.Add(wfh);
            //ffBrowserControl.Content = wfh;

            //url in Navigating event may be the mapped version,
            //e.g. about:config in Navigating event is jar:file:///<xulrunner>/omni.ja!/chrome/toolkit/content/global/config.xul
            browser.Navigating += (s, ee) =>
            {
                Console.WriteLine("Navigating: url: " + ee.Uri + ", top: " + ee.DomWindowTopLevel);
            };
            browser.Navigated += (s, ee) =>
            {
                Console.WriteLine("Navigated: url: " + ee.Uri + ", top: " + ee.DomWindowTopLevel, ", errorPage: " + ee.IsErrorPage);
            };

            browser.Retargeted += (s, ee) =>
            {
                var ch = ee.Request as Gecko.Net.Channel;
                Console.WriteLine("Retargeted: url: " + ee.Uri + ", contentType: " + ch.ContentType + ", top: " + ee.DomWindowTopLevel);
            };
            browser.DocumentCompleted += (s, ee) =>
            {
                Console.WriteLine("DocumentCompleted: url: " + ee.Uri + ", top: " +ee.IsTopLevel);
            };
            
            browser.Navigate("http://www.google.com");
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

            //browser.DocumentTitleChanged += (s, e) => tabPage.Text = browser.DocumentTitle;

            // browser.EnableDefaultFullscreen();

            // Popup window management.
            browser.CreateWindow += (s, ee) =>
            {
                // A naive popup blocker, demonstrating popup cancelling.
                Console.WriteLine("A popup is trying to show: " + ee.Uri);
                if (ee.Uri.StartsWith("http://annoying-site.com"))
                {
                    ee.Cancel = true;
                    Console.WriteLine("A popup is blocked: " + ee.Uri);
                    return;
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
        }
    }
}
