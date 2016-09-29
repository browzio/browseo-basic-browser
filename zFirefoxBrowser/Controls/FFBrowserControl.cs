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
using Organiser.Common.Classes;
using System.Runtime.InteropServices;
using System.IO;
using zFirefoxBrowser.Helpers;
using System.Diagnostics;
using Gecko.DOM;
using System.Windows.Input;
using Gecko.IO;

namespace zFirefoxBrowser.Controls
{
    //public class MyGeckoBrowser : GeckoWebBrowser, nsIContentFrameMessageManager, nsIInterfaceRequestor
    //{
    //    IntPtr nsIInterfaceRequestor.GetInterface(ref Guid uuid)
    //    {
    //        Console.WriteLine(uuid.ToString());
    //        object obj = this;

    //        // note: when a new window is created, gecko calls GetInterface on the webbrowser to get a DOMWindow in order
    //        // to set the starting url
    //       // if (this.WebBrowser != null)
    //      //  {
    //            if (uuid == typeof(nsIDOMWindow).GUID)
    //            {
    //               // obj = this.WebBrowser.GetContentDOMWindowAttribute();
    //            }
    //            else if (uuid == typeof(nsIDOMDocument).GUID)
    //            {
    //               // obj =
    //               //     new Gecko.WebIDL.Window(this.WebBrowser.GetContentDOMWindowAttribute(),
    //               //         (nsISupports)this.WebBrowser.GetContentDOMWindowAttribute()).Document;
    //            }else if(uuid.ToString() == "22117140-9c6e-11d3-aaf1-00805f8a4905")
    //        {

    //        }
    //      //  }

    //        IntPtr ppv, pUnk = Marshal.GetIUnknownForObject(obj);

    //        Marshal.QueryInterface(pUnk, ref uuid, out ppv);

    //        Marshal.Release(pUnk);

    //        return ppv;
    //    }

    //    public void AddMessageListener(nsAStringBase messageName, nsIMessageListener listener, bool listenWhenClosed)
    //    {
    //    }

    //    public void AddWeakMessageListener(nsAStringBase messageName, nsIMessageListener listener)
    //    {
    //    }

    //    public void Atob(nsAStringBase aAsciiString, nsAStringBase retval)
    //    {
    //    }

    //    public void Btoa(nsAStringBase aBase64Data, nsAStringBase retval)
    //    {
    //    }

    //    public void Dump(nsAStringBase aStr)
    //    {
    //    }

    //    public nsIDOMWindow GetContentAttribute()
    //    {
    //        return this.Window.DomWindow;
    //    }

    //    public bool MarkForCC()
    //    {
    //        return true;
    //    }

    //    public void PrivateNoteIntentionalCrash()
    //    {
    //    }

    //    public void RemoveMessageListener(nsAStringBase messageName, nsIMessageListener listener)
    //    {
    //    }

    //    public void RemoveWeakMessageListener(nsAStringBase messageName, nsIMessageListener listener)
    //    {
    //    }

    //    public void SendAsyncMessage(nsAStringBase messageName, ref JsVal obj, ref JsVal objects, nsIPrincipal principal, IntPtr jsContext, int argc)
    //    {
    //    }

    //    public JsVal SendRpcMessage(nsAStringBase messageName, ref JsVal obj, ref JsVal objects, nsIPrincipal principal, IntPtr jsContext, int argc)
    //    {
    //        return obj;
    //    }

    //    public JsVal SendSyncMessage(nsAStringBase messageName, ref JsVal obj, ref JsVal objects, nsIPrincipal principal, IntPtr jsContext, int argc)
    //    {
    //        return obj;
    //    }
    //}
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

        public bool InMacroPlaying { get; set; }

        public FFBrowserControl()
        {
            InitializeComponent();
        }

        public async void initBrowser(string url, Action SetLoadingFalse)
        {
            //Debugger.Launch();
            bool setProxPass = await FoxInit.AwaitforProxySet();
            if (!setProxPass)
            {
                MessageBox.Show("Proxy took longer then 30 seconds to respond with a 'successfull reply'. Please make sure the proxy is acting correctly before continuing.");
                if (SetLoadingFalse != null) SetLoadingFalse();
            }

            Browser = new GeckoWebBrowser();
            Browser.Dock = DockStyle.Fill;


            Browser.Navigate(url);


            //url in Navigating event may be the mapped version,
            //e.g. about:config in Navigating event is jar:file:///<xulrunner>/omni.ja!/chrome/toolkit/content/global/config.xul
            Browser.Navigating += (s, ee) =>
            {
                //if (!FoxInit.DidsetProxy && url != "about:blank" && GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                //{
                //    FoxInit.SetProxyIfNeeded();
                //    Browser.Reload();
                //}
                if (ee.Uri != null && (ee.Uri.AbsoluteUri.ToLower().EndsWith("/feed") || ee.Uri.AbsoluteUri.ToLower().EndsWith("/feed/")))
                {
                    ee.Cancel = true;
                    Browser.Navigate("view-source:" + ee.Uri.AbsoluteUri);
                    return;
                }
                OnBrowserLoadingChanged(true);

                // Console.WriteLine("Navigating: url: " + ee.Uri + ", top: " + ee.DomWindowTopLevel);
            };
            //Browser.Navigated += (s, ee) =>
            //{
            //    //if (!FoxInit.DidsetProxy && url != "about:blank")
            //    //{
            //    //    FoxInit.SetProxyIfNeeded();
            //    //    Browser.Reload();
            //    //}
            //    //OnBrowserLoadingChanged(false);
            //    // OnBrowserTitleChanged(Convert.ToString(ee.Uri));

            //    // Console.WriteLine("Navigated: url: " + ee.Uri + ", top: " + ee.DomWindowTopLevel, ", errorPage: " + ee.IsErrorPage);
            //};

            //Browser.Retargeted += (s, ee) =>
            //{
            //    var ch = ee.Request as Gecko.Net.Channel;

            //    // ch.Cancel(1);
            //   // ch.Resume();
            //    return;
            //    OnCreateNewTab(ee.Uri.AbsoluteUri);
            //    // Console.WriteLine("Retargeted: url: " + ee.Uri + ", contentType: " + ch.ContentType + ", top: " + ee.DomWindowTopLevel);
            //};
            Browser.DocumentCompleted += (s, ee) =>
            {
                OnBrowserLoadingChanged(false);
                OnBrowserAddressChanged(ee.Uri.ToString());
                // Console.WriteLine("DocumentCompleted: url: " + ee.Uri + ", top: " + ee.IsTopLevel);
            };


            Browser.DocumentTitleChanged += (s, e) =>
            {
                if (Browser.DocumentTitle == "Page Load Error")
                {
                    if (SetLoadingFalse != null) SetLoadingFalse();
                }
                OnBrowserTitleChanged(Browser.DocumentTitle);
            };

            Browser.StatusTextChanged += (s, e) =>
            {
                OnBrowserStatusChanged(Browser.StatusText);
            };

            //Browser.ConsoleMessage += (e, s) =>
            //{
            //    //if (s.Message.Contains("NS_ERROR_FACTORY_NOT_REGISTERED"))
            //    //{

            //    //}
            //    //var bhead = Browser.Document.Head;
            //    //bhead.OuterHtml =  bhead.OuterHtml.Replace("<head>", "<head><meta charset=\"utf-8\"></meta>");
            //    //Console.WriteLine(s.Message);
            //    //if (Browser.Url.AbsolutePath.Contains("whoer"))
            //    //{

            //    //}
            //    // OnBrowserMessageChanged(s.Message);
            //};

            Browser.ShowContextMenu += (s, e) =>
            {

                if (!e.AssociatedLink.IsNullOrEmpty())
                {
                    e.ContextMenu.MenuItems.Add("-");

                    MenuItem nt = new MenuItem() { Name = "1", Text = "Open In New Tab" };
                    nt.Click += (ss, ee) => { OnCreateNewTab(e.AssociatedLink); };
                    e.ContextMenu.MenuItems.Add(nt);
                }

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

                if (Browser.Url != null &&
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
                //Console.WriteLine("A popup is trying to show: " + ee.Uri);
                if (InMacroPlaying) return;

                if (InMacroPlaying || ee.Uri.Contains("about:blank"))
                {
                    ee.Cancel = true;
                    // Console.WriteLine("A popup is blocked: " + ee.Uri);
                    return;
                }
                string target = ee.Uri.ToLower();
                if (Browser.Url.AbsoluteUri.Contains("https://mail.google.com") || (!target.Contains("microsoft") &&
                    !target.Contains("facebook") &&
                    !target.Contains("zapier.com") &&
                    !target.Contains("twitter") &&
                    !target.Contains("gplus") &&
                    !target.Contains("session/") &&
                    !target.Contains("yahoo") &&
                    !target.Contains("login") && !target.Contains("connect") && !target.Contains("oauth") && !target.Contains("signup")))
                {
                    OnCreateNewTab(ee.Uri);
                    ee.Cancel = true;
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


            this.SuspendLayout();
            this.Controls.Add(Browser);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// PRIVATE BROWS
        ///          var field = typeof(GWB).GetField("WebBrowser", BindingFlags.Instance | BindingFlags.NonPublic);
        ///          nsIWebBrowser nsIWebBrowser = (nsIWebBrowser)field.GetValue(Browser); //this might be null if called right before initialization of browser
        ///          Xpcom.QueryInterface<nsILoadContext>(nsIWebBrowser).SetPrivateBrowsing(true);
        /// </summary>

        private void Tse_Click(object sender, EventArgs e)
        {
            OnBrowserContextMenuClicked((sender as MenuItem).Name);
        }

        internal void Navigate(string addressEditable)
        {
            if (CheckBrowserNull()) return;

            string url = addressEditable;

            if (!url.Contains(".") && url.Length > 1 && !url.Contains("about:config"))
            {
                url = url.Replace(' ', '+');
                url = String.Format(@"http://google.com/search?v=1.0&q={0}", url);
            }

            //MimeInputStream stream = MimeInputStream.Create();
            //stream.AddHeader("Host", "www.facebook.com");
            //stream.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0");
            //stream.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            //stream.AddHeader("Accept-Language", "en-US,en;q=0.5");
            //stream.AddHeader("Accept-Encoding", "gzip, deflate, br");
            //stream.AddHeader("Connection", "keep-alive");
            ////stream.AddHeader("Accept-Charset", "ISO-8859-1");

            //Browser.Navigate(url, GeckoLoadFlags.None,null, null, stream);
            Browser.Navigate(url);
        }

        private bool CheckBrowserNull()
        {
            if(Browser == null)
            {
                MessageBox.Show("The browser and proxy is still initializing.");
                return true;
            }

            return false;
        }

        internal void Back()
        {
            //GeckoPreferences.User["network.proxy.type"] = 0;
            // FoxInit.SetProxyIfNeeded();
            //            using (var context = new AutoJSContext(Browser.Window.DomWindow))
            //            {
            //                string result = String.Empty;
            //                var success = context.EvaluateScript(@"
            //try
            //{ 
            //alert(window.navigator);
            //for (property in navigator) { alert(property + ' ' + navigator[property]); }
            //}
            //catch(e)
            //{
            //    alert(e);
            //}", out result);
            //            }

            //using (var context = new AutoJSContext(Browser.Window.DomWindow))
            //{
            //    string result = String.Empty;
            //    var success = context.EvaluateScript(@"Components.utils.import(""resource://gre/modules/XPCOMUtils.jsm""); function yo(){var baseWindow = window.QueryInterface(Components.interfaces.nsIInterfaceRequestor)
            //            .getInterface(Components.interfaces.nsIWebNavigation)
            //            .QueryInterface(Components.interfaces.nsIDocShellTreeItem)
            //            .treeOwner
            //            .QueryInterface(Components.interfaces.nsIInterfaceRequestor)
            //            .getInterface(Components.interfaces.nsIBaseWindow); return baseWindow} yo();", out result);
            //}\
            //GeckoPreferences.User["devtools.debugger.remote-enabled"] = true;

            ////see <geckofx_src>/chrome dir
            //FoxInit.RegisterChromeDir(@"C:\Users\eli\Downloads\GeckoFX Development Tools Firefox 46\GeckoFX Development Tools Firefox 46");

            //var browser = new GeckoWebBrowser();
            //browser.NavigationError += (s, e) =>
            //{
            //    Console.Error.WriteLine("StartDebugServer error: 0x" + e.ErrorCode.ToString("X"));
            //    browser.Dispose();
            //};
            //browser.DocumentCompleted += (s, e) =>
            //{
            //    Console.WriteLine("StartDebugServer completed");
            //    browser.Dispose();
            //};
            ////see <geckofx_src>/chrome/debugger-server.html
            //browser.Navigate("chrome://geckofx/content/debugger-server.html");

            if (CheckBrowserNull()) return;
            Browser.GoBack();
        }

        internal void Forward()
        {
            if (CheckBrowserNull()) return;
            Browser.GoForward();
        }

        internal void Reload()
        {
            if (CheckBrowserNull()) return;
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
