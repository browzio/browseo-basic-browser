namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Xilium.CefGlue;

    public class DemoCefDialogHandler : CefDialogHandler
    {
        //protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFileName, string[] acceptTypes, CefFileDialogCallback callback)
        //{
        //    // callback.Continue(new string[] { @"C:\Users\eli\Desktop\12381645_956432671093361_801802024_n.mp4" });
        //    return false;
        //}

        protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, string[] acceptFilters, int selectedAcceptFilter, CefFileDialogCallback callback)
        {
            //return base.OnFileDialog(browser, mode, title, defaultFilePath, acceptFilters, selectedAcceptFilter, callback);
            return false;
        }
    }

    public class CefWebClientDownloadHandler : CefDownloadHandler
    {
        bool didnotshowcomplete;
        protected override void OnBeforeDownload(CefBrowser browser, CefDownloadItem downloadItem, string suggestedName, CefBeforeDownloadCallback callback)
        {

            callback.Continue("", true);

            //base.OnBeforeDownload(browser, downloadItem, suggestedName, callback);

        }

        protected override void OnDownloadUpdated(CefBrowser browser, CefDownloadItem downloadItem, CefDownloadItemCallback callback)
        {

            if (downloadItem.PercentComplete <= 0)
                didnotshowcomplete = false;

            if (downloadItem.PercentComplete >= 100 && !didnotshowcomplete && downloadItem.FullPath != null)
            {
                System.Windows.Forms.MessageBox.Show("Download complete");
                try
                {
                    Process.Start(downloadItem.FullPath.Remove(downloadItem.FullPath.LastIndexOf("\\")));
                }
                catch { }
                didnotshowcomplete = true;
            }

            base.OnDownloadUpdated(browser, downloadItem, callback);
        }
    }

    public class DemoCefContextMenuHandler : CefContextMenuHandler
    {
        CefWebBrowser m_core;

        public DemoCefContextMenuHandler(CefWebBrowser core)
        {
            m_core = core;
        }

        protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId, CefEventFlags eventFlags)
        {
            m_core.OnContextMenuItemSelected(commandId);
            return false;
        }

        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            // model.AddItem(333, "Show Dev Tools");
            model.AddItem(999, "Open In New Tab");
            model.AddItem(888, "Copy Link Address");
            model.AddItem(777, "Save Image As...");
            model.AddItem(333, "To Social Enagager");
            model.AddItem(222, "Curaste...");
            model.AddItem(666, "Curate It");
            try
            {
                if (frame.Url.ToLower().Contains("www.facebook.com/search") || frame.Url.ToLower().Contains("facebook.com/groups/?category=membership"))
                {
                    model.AddItem(555, "Dominate");
                    model.AddItem(444, "Dominate All");
                }
            }
            catch
            {
                //model.AddItem(555, "Dominate");
            }
        }
    }

    public class DemoCefFocusHandler : CefFocusHandler
    {
        protected override void OnGotFocus(CefBrowser browser)
        {
            // Console.ReadLine();
            base.OnGotFocus(browser);
        }

        protected override bool OnSetFocus(CefBrowser browser, CefFocusSource source)
        {
            //Console.ReadLine();
            browser.GetHost().SetFocus(true);
            return base.OnSetFocus(browser, source);
        }

        protected override void OnTakeFocus(CefBrowser browser, bool next)
        {
            //Console.ReadLine();
            base.OnTakeFocus(browser, next);
        }
    }

    public class DemoKeyboardHandler : CefKeyboardHandler
    {
        public Action<CefKeyEvent> OnPrePreviewKeyDown = delegate { };

        protected override bool OnPreKeyEvent(CefBrowser browser, CefKeyEvent keyEvent, IntPtr os_event, out bool isKeyboardShortcut)
        {
            OnPrePreviewKeyDown(keyEvent);
            return base.OnPreKeyEvent(browser, keyEvent, os_event, out isKeyboardShortcut);
        }
    }

    public class CefWebClient : CefClient
    {
        private readonly CefWebBrowser _core;
        private readonly CefWebLifeSpanHandler _lifeSpanHandler;
        private readonly CefWebDisplayHandler _displayHandler;
        private readonly CefWebLoadHandler _loadHandler;
        private readonly CefRequestHandler _requestHandler;
        // private readonly CefRenderProcessHandler _RenderProcessHandler;
        private readonly DemoCefContextMenuHandler _contextMenueHandler;

        private CefWebClientDownloadHandler downlaodHandler;
        private DemoCefDialogHandler dialogHandler;
        private DemoCefFocusHandler cefFocusHandler;

        private readonly DemoKeyboardHandler _keyboardHandler;
        public DemoKeyboardHandler KeyboardHandler { get { return _keyboardHandler; } }

        public CefWebClient(CefWebBrowser core, CefRequestHandler requestHandler = null, CefRenderProcessHandler RenderProcessHandler = null)
        {
            _core = core;
            _lifeSpanHandler = new CefWebLifeSpanHandler(_core);
            _displayHandler = new CefWebDisplayHandler(_core);
            _loadHandler = new CefWebLoadHandler(_core);
            _contextMenueHandler = new DemoCefContextMenuHandler(_core);
            _keyboardHandler = new DemoKeyboardHandler();
            cefFocusHandler = new DemoCefFocusHandler();

            if (requestHandler == null)
                _requestHandler = new CefWebRequestHandler(_core);
            else
                _requestHandler = requestHandler;
        }

        protected override CefDownloadHandler GetDownloadHandler()
        {
            if (downlaodHandler == null)
                downlaodHandler = new CefWebClientDownloadHandler();
            return downlaodHandler;
        }

        protected override CefDialogHandler GetDialogHandler()
        {
            if (dialogHandler == null)
                dialogHandler = new DemoCefDialogHandler();
            return dialogHandler;
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override CefRequestHandler GetRequestHandler()
        {
            return _requestHandler;
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            return _contextMenueHandler;
        }

        protected override CefFocusHandler GetFocusHandler()
        {
            //return cefFocusHandler;
            return base.GetFocusHandler();
        }

        #region unimplemented
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return base.GetRenderHandler();
        }

        protected override CefDragHandler GetDragHandler()
        {
            return base.GetDragHandler();
        }



        protected override CefGeolocationHandler GetGeolocationHandler()
        {
            return base.GetGeolocationHandler();
        }

        protected override CefJSDialogHandler GetJSDialogHandler()
        {
            return base.GetJSDialogHandler();
        }

        protected override CefKeyboardHandler GetKeyboardHandler()
        {
            return _keyboardHandler;
        }
        #endregion
    }
}
