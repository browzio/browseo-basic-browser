using Gecko;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    public class BFXWindowCreator : nsIWindowCreator2
    {
        public nsIWebBrowserChrome CreateChromeWindow(nsIWebBrowserChrome parent, uint chromeFlags)
        {
            return OnCreateChromeWindow(parent, (int)chromeFlags);
        }

        public nsIWebBrowserChrome CreateChromeWindow2(nsIWebBrowserChrome parent, uint chromeFlags, uint contextFlags, nsITabParent openingTab, mozIDOMWindowProxy aOpener, out bool cancel)
        {
            return OnCreateChromeWindow2(parent, (int)chromeFlags, (int)contextFlags, openingTab, aOpener, out cancel);
        }

        protected virtual nsIWebBrowserChrome OnCreateChromeWindow(nsIWebBrowserChrome parent, int chromeFlags)
        {
            bool cancel = false;
            return CreateChromeWindow2Internal(parent, chromeFlags, 0, null, null, out cancel);
        }

        protected virtual nsIWebBrowserChrome OnCreateChromeWindow2(nsIWebBrowserChrome parent, int chromeFlags, int contextFlags, nsITabParent openingTab, mozIDOMWindowProxy aOpener, out bool cancel)
        {
            return CreateChromeWindow2Internal(parent, chromeFlags, 0, openingTab, aOpener, out cancel);
        }

        private nsIWebBrowserChrome CreateChromeWindow2Internal(nsIWebBrowserChrome parent, int chromeFlags, int contextFlags, nsITabParent openingTab, mozIDOMWindowProxy opener, out bool cancel)
        {
            cancel = false;

            nsIXULWindow childXulWindow = null;
            if (parent != null)
            {
                nsIXULWindow parentXulWindow = Xpcom.QueryInterface<nsIXULWindow>(parent);
                try
                {
                    //if ((chromeFlags & nsIWebBrowserChromeConsts.CHROME_OPENAS_CHROME) == nsIWebBrowserChromeConsts.CHROME_OPENAS_CHROME
                    //    || (chromeFlags & nsIWebBrowserChromeConsts.CHROME_MODAL) == nsIWebBrowserChromeConsts.CHROME_MODAL)
                    //{
                        if (parentXulWindow != null)
                            childXulWindow = parentXulWindow.CreateNewWindow(chromeFlags, openingTab, opener);
                    //}
                    else
                    {
                        WebBrowserGlueBase wbg = Gecko.DOM.GeckoDOMExtensions.GetWebBrowserGlue(parent);
                        if (wbg != null)
                        {
                            nsIWebBrowserChrome chromeWindow = wbg.CreateWindow(out cancel);
                            if (cancel)
                                return null;
                            if (chromeWindow != null)
                                return chromeWindow;
                        }

                        var xw = new Gecko.GUI.GeckoXULWindow();
                        childXulWindow = xw.QueryInterface<nsIXULWindow>(); //addref
                        xw.Width = 600;
                        xw.Height = 400;
                        //cancel = true;
                        //return null;
                    }
                }
                finally
                {
                    Xpcom.FreeComObject(ref parentXulWindow);
                }
            }
            else
            {
                using (var appShell = Xpcom.GetService2<nsIAppShellService>(Contracts.AppShellService))
                {
                    if (appShell == null)
                        throw Marshal.GetExceptionForHR(GeckoError.NS_ERROR_FAILURE);

                    childXulWindow = appShell.Instance.CreateTopLevelWindow(null, null, (uint)chromeFlags, nsIAppShellServiceConsts.SIZE_TO_CONTENT, nsIAppShellServiceConsts.SIZE_TO_CONTENT, openingTab, opener);
                }
            }

            if (childXulWindow != null)
            {
                childXulWindow.SetContextFlagsAttribute((uint)contextFlags);
                nsIWebBrowserChrome chromeWindow = Xpcom.QueryInterface<nsIWebBrowserChrome>(childXulWindow);
                Xpcom.FreeComObject(ref childXulWindow);
                if (chromeWindow != null) return chromeWindow;
            }
            throw Marshal.GetExceptionForHR(GeckoError.NS_ERROR_FAILURE);
        }

        public void SetScreenId(uint aScreenId)
        {
            System.Diagnostics.Debug.Print("Not implemented: Gecko.WindowCreator.SetScreenId()");
        }
    }
}
