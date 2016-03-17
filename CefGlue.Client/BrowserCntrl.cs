using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xilium.CefGlue.WindowsForms;
using SocialOrganizer.Models;
using Organiser.Common.Classes;
using Organiser.Common;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DragDropListview;
using System.Net;
using System.IO;

namespace Xilium.CefGlue.Client
{
    public partial class BrowserCntrl : UserControl
    {

        public event Action<string> OnBrowserTitleChanged = delegate { };
        public event Action<string> OnBrowserAddressChanged = delegate { };
        public event Action<string> OnBrowserStatusChanged = delegate { };
        public event Action<string> OnBrowserMessageChanged = delegate { };
        public event Action<string> OnCreateNewTab = delegate { };
        public event Action<bool> OnBrowserLoadingChanged = delegate { };
        public event Action<int> OnBrowserContextMenuClicked = delegate { };     

        public CefWebBrowser CBrowser { get; set; }

        public string CurrAddress { get; set; }

        private bool isWindowPopUp;  

        public BrowserCntrl()
        {
            InitializeComponent();
            this.PreviewKeyDown += BrowserCntrl_PreviewKeyDown;
        }

        private void BrowserCntrl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        public void init(string startUrl)
        {
            CefState StateJavascript = BrowserSettimgs.JavascriptEnabled ? CefState.Enabled : CefState.Disabled;
            CefState StateJava = BrowserSettimgs.JavaEnabled ? CefState.Enabled : CefState.Disabled;
            CefState StateFlash = BrowserSettimgs.FlashEnabled ? CefState.Enabled : CefState.Disabled;
           CBrowser = new CefWebBrowser();
            CBrowser.HandleWasCreated += browser_OnHandleCreated;
            if(startUrl != "")
                CBrowser.StartUrl = startUrl;
            CBrowser.Width = this.Width;
            CBrowser.Height = this.Height;
            CBrowser.Dock = DockStyle.Fill;
            CBrowser.BrowserSettings = new CefBrowserSettings()
            {
                JavaScriptAccessClipboard = StateJavascript,
                JavaScriptDomPaste = StateJavascript,
                JavaScript = StateJavascript,
                Java = StateJava,
                Plugins = StateFlash,
            };
            CBrowser.BringToFront();

            
            CBrowser.TitleChanged += CBrowser_TitleChanged;
            CBrowser.AddressChanged += CBrowser_AddressChanged;
            CBrowser.StatusMessage += CBrowser_StatusMessage;
            CBrowser.BeforePopup += CBrowser_BeforePopup;
            CBrowser.LoadingStateChange += CBrowser_LoadingStateChange;
            CBrowser.ConsoleMessage += CBrowser_ConsoleMessage;
            CBrowser.OnContextMenuItemClicked += CBrowser_OnContextMenuItemClicked;    

            this.SuspendLayout();
            this.Controls.Add(CBrowser);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #region events

        void CBrowser_OnContextMenuItemClicked(int contextMenueItemID)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(CBrowser_OnContextMenuItemClicked), contextMenueItemID);
                return;
            }

            OnBrowserContextMenuClicked(contextMenueItemID);
        }

        void CBrowser_BeforePopup(object sender, BeforePopupEventArgs e)
        {
            e.Handled = true; 

            isWindowPopUp = true;
            if (e.TargetUrl != null)
                OnCreateNewTab(e.TargetUrl);
            isWindowPopUp = false;
        }

        void CBrowser_StatusMessage(object sender, StatusMessageEventArgs e)
        {
            if (isWindowPopUp)
                return; 
            OnBrowserStatusChanged(e.Value);
        }

        void CBrowser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            if (isWindowPopUp)
                return;

            CurrAddress = CBrowser.Address;  
            OnBrowserAddressChanged(CBrowser.Address);
        }

        void CBrowser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            if (isWindowPopUp)
                return;
            var title = CBrowser.Title;
            if (title != null)
            {
                if (title.Length > 18)
                {
                    title = title.Substring(0, 18) + "...";
                }
                OnBrowserTitleChanged(title);
            }
        }

        void CBrowser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (isWindowPopUp)
                return;

            OnBrowserMessageChanged(e.Message);
        }

        void CBrowser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
        {
            if (isWindowPopUp)
                return;
           // CBrowser.Browser.GetMainFrame().ExecuteJavaScript("alert(window.MediaStreamTrack);", CBrowser.Browser.GetMainFrame().Url, 0);
          //  CBrowser.Browser.GetMainFrame().ExecuteJavaScript("alert(window.MediaStreamTrack);", CBrowser.Browser.GetMainFrame().Url, 0);
            OnBrowserLoadingChanged(e.IsLoading);
            //if(!e.IsLoading)//
            //    CBrowser.Browser.GetMainFrame().ExecuteJavaScript("for (property in navigator) { alert(property + ' ' + navigator[property]); }", CBrowser.Browser.GetMainFrame().Url, 0);
        }

        void browser_OnHandleCreated()
        {
            CefWebClient client = CBrowser.CreateWebClient(new RequestHandleing(CBrowser), null);
            client.KeyboardHandler.OnPrePreviewKeyDown += OnBrowserPreviewKeyDown;
        }

        private void OnBrowserPreviewKeyDown(CefKeyEvent keyEvent)
        {
            if(keyEvent.Modifiers == CefEventFlags.ControlDown && keyEvent.Character == 'T')
            {
                OnCreateNewTab("");
            }
        }

        #endregion

        #region navigation

        public void Navigate(string url)
        {
            try
            {
                if (!url.Contains(".") && url.Length > 1)
                {
                    string linkb = url;
                    linkb = url.Replace(' ', '+');
                    linkb = String.Format(@"http://google.com/search?v=1.0&q={0}", linkb);
                    CBrowser.Browser.GetMainFrame().LoadUrl(linkb);
                }
                else
                {
                    //CefRequest request = CefRequest.Create();
                    //System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
                    //headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                    //headers.Add("Accept-Encoding", "gzip, deflate, sdch");
                    //headers.Add("Connection", "keep-alive");
                    //headers.Add("Host", "whoer.net");
                    //headers.Add("Upgrade-Insecure-Requests", "1");
                    //headers.Add("DNT", "1");       
                    //request.SetHeaderMap(headers);
                    //request.Url = url;
                    //CBrowser.Browser.GetMainFrame().LoadRequest(request);
                    CBrowser.Browser.GetMainFrame().LoadUrl(url);
                    MyFilesDatabase.AppendToSavedSites(url);
                }
            }catch { }
        }

        public void Reload()
        {
            try
            {
                CBrowser.Browser.Reload();
            }
            catch { }
        }

        public void Forward()
        {
            try
            {
                if (CBrowser.Browser.CanGoForward)
                    CBrowser.Browser.GoForward();
            }
            catch { }
        }

        public void Back()
        {
            try
            {
                if (CBrowser.Browser.CanGoBack)
                    CBrowser.Browser.GoBack();
            }
            catch { }
        }

        #endregion

        public void DisposeBrowserComponents()
        {
            Dispose(true);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
