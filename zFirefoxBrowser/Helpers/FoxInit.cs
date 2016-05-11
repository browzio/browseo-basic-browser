using Gecko;
using Gecko.Interop;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zFirefoxBrowser.Helpers
{
    public class FoxInit
    {
        public static void Init(PersonData data = null)
        {
            if (data != null)
            {
                GloableProfData.PData = data;
            }
            // GeckoWebBrowser.UseCustomPrompt();

            //setup cache path
            string profilepath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + GloableProfData.PData.ProjectName);
            if (!Directory.Exists(profilepath)) Directory.CreateDirectory(profilepath);
            Xpcom.ProfileDirectory = profilepath;

            var xulpath = AppDomain.CurrentDomain.BaseDirectory + "\\FFLibrary\\Firefox";
            xulpath = xulpath.Replace("\\\\", "\\");
            Xpcom.Initialize(xulpath);


            //settings
            GeckoPreferences.User["browser.xul.error_pages.enabled"] = true;
            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
            GeckoPreferences.User["full-screen-api.enabled"] = true;

            //GeckoPreferences.User["browser.safebrowsing.enabled"] = false;
            //GeckoPreferences.User["browser.safebrowsing.malware.enabled"] = false;
            GeckoPreferences.User["network.http.pipelining"] = true;
            GeckoPreferences.User["network.http.proxy.pipelining"] = true;
            GeckoPreferences.User["security.dialog_enable_delay"] = 0;
            GeckoPreferences.User["browser.tabs.animate"] = false;
            GeckoPreferences.User["extensions.blocklist.enabled"] = false;
            GeckoPreferences.User["plugins.click_to_play"] = true;

            //GeckoPreferences.User["media.gmp-provider.enabled"] = false;
            //GeckoPreferences.User["media.gmp-gmpopenh264.enabled"] = false;
            //GeckoPreferences.User["media.peerconnection.video.enabled"] = false;
            //GeckoPreferences.User["network.disable.ipc.security"] = true;
            //GeckoPreferences.User["extensions.blocklist.enabled"] = true;
            //GeckoPreferences.User["plugin.scan.4xPluginFolder"] = false;
            //GeckoPreferences.User["application.use_ns_plugin_finder"] = true;
            GeckoPreferences.Default["plugin.state.npctrl"] = 0;

            GeckoPreferences.User["general.useragent.override"] = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";

            foreach (Gecko.Plugins.PluginTag tag in Gecko.Plugins.PluginHost.GetPluginTags())
            {
                if (tag.Name.ToLower().Contains("silverlight"))
                {
                }
            }

            //ComPtr<nsIBlocklistService> pluginHost = Xpcom.GetService2<nsIBlocklistService>(Contracts.b);
            //pluginHost.Instance.

            if (GloableProfData.PData != null && !GloableProfData.PData.ProxyIP.IsNullOrEmpty())
            {
                try
                {
                    GeckoPreferences.User["network.proxy.http"] = GloableProfData.PData.ProxyIP;
                    GeckoPreferences.User["network.proxy.http_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                    //GeckoPreferences.User["client.proxy.http"] = GloableProfData.PData.ProxyIP;
                    //GeckoPreferences.User["client.proxy.http_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                    GeckoPreferences.User["network.proxy.ssl"] = GloableProfData.PData.ProxyIP;
                    GeckoPreferences.User["network.proxy.ssl_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                    GeckoPreferences.User["network.proxy.type"] = 1;
                    if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                    {
                        PromptFactory.PromptServiceCreator = () => new ProxyLoginPromptBypass();
                        GeckoPreferences.User["browser.xul.error_pages.enabled"] = false;

                        GeckoPreferences.User["network.proxy.login"] = GloableProfData.PData.ProxyUsername;
                        GeckoPreferences.User["network.proxy.password"] = GloableProfData.PData.ProxyPassword;
                    }

                    //PromptFactory.PromptServiceCreator = () => new ProxyLoginPromptBypass();
                    //GeckoPreferences.User["browser.xul.error_pages.enabled"] = false;
                    //GeckoPreferences.User["network.proxy.http"] = "104.168.97.203";
                    //GeckoPreferences.User["network.proxy.http_port"] = 8080;
                    //GeckoPreferences.User["network.proxy.ssl"] = "104.168.97.203";
                    //GeckoPreferences.User["network.proxy.ssl_port"] = 8080;
                    //GeckoPreferences.User["network.proxy.type"] = 1;
                    //GeckoPreferences.User["network.proxy.login"] = "organiser1";
                    //GeckoPreferences.User["network.proxy.password"] = "7fba457f61d66f633cfb7cca6f4b02ad";
                }
                catch (Exception ex)
                {
                    string msg = "Failed To Set Proxy Error: " + ex.Message;
                    msg.Show();
                }
            }
            LauncherDialog.Download += LauncherDialog_Download;
        }

        private static async void LauncherDialog_Download(object sender, LauncherDialogEvent launcherdialoge)
        {
            try
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog1.Filter = "All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = launcherdialoge.Filename;
                if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    launcherdialoge.Cancel();
                    return;
                }
                uint flags = (uint)nsIWebBrowserPersistConsts.PERSIST_FLAGS_NO_CONVERSION |
                         (uint)nsIWebBrowserPersistConsts.PERSIST_FLAGS_REPLACE_EXISTING_FILES;

                string url = launcherdialoge.Url;  //url to download
                string fullpath = saveFileDialog1.FileName; //destination file absolute path
                if (File.Exists(fullpath)) File.Delete(fullpath);

                nsIWebBrowserPersist persist = Xpcom.GetService<nsIWebBrowserPersist>("@mozilla.org/embedding/browser/nsWebBrowserPersist;1");
                nsIURI source = IOService.CreateNsIUri(url);
                nsIURI dest = IOService.CreateNsIUri(new Uri(fullpath).AbsoluteUri);
                persist.SetPersistFlagsAttribute(flags);
                persist.SaveURI(source, null, null, 0, null, null, (nsISupports)dest, null);
                try
                {
                    while (persist.GetCurrentStateAttribute() != (uint)nsIWebBrowserPersistConsts.PERSIST_STATE_FINISHED)
                    {
                        await Task.Run(() => { System.Threading.Thread.Sleep(1000); });
                    }

                    Process.Start(saveFileDialog1.FileName.Replace(System.IO.Path.GetFileName(saveFileDialog1.FileName),""));
                }
                catch { }
                //nsIURI source = IOService.CreateNsIUri(url);
                //nsIURI dest = IOService.CreateNsIUri(new Uri(fullpath).AbsoluteUri);

                //nsIWebBrowserPersist persist = Xpcom.GetService<nsIWebBrowserPersist>("@mozilla.org/embedding/browser/nsWebBrowserPersist;1");

                //nsIWindowWatcher win = Xpcom.GetService<nsIWindowWatcher>("@mozilla.org/embedcomp/window-watcher;1");
                //win.OpenWindow(null, new Uri("chrome://mozapps/content/downloads/downloads.xul").AbsoluteUri,
                //    "Downloads", "chrome,resizable=yes", null);

                //nsIDownloadManager DownloadMan = Xpcom.CreateInstance<nsIDownloadManager>("@mozilla.org/download-manager;1");
                //nsIDownload download = DownloadMan.AddDownload(0, source, dest, null, launcherdialoge.Mime, 0, null, (nsICancelable)persist, false);

                //persist.SetPersistFlagsAttribute(flags);
                //persist.SetProgressListenerAttribute((nsIWebProgressListener)download);
                //persist.SaveURI(source, null, null, 0, null, null, (nsISupports)dest, null);
            }
            catch { }
        }

        public static void Shutdown()
        {
            Xpcom.Shutdown();
        }

        private class ProxyLoginPromptBypass : PromptService, nsIAuthPrompt, nsIAuthPrompt2
        {
            static bool setProxy;

            public override bool PromptAuth(nsIChannel aChannel, uint level, nsIAuthInformation authInfo)
            {
                //if (!setProxy)
                //{
                if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                {
                    nsString.Set(authInfo.SetUsernameAttribute, GloableProfData.PData.ProxyUsername);
                    nsString.Set(authInfo.SetPasswordAttribute, GloableProfData.PData.ProxyPassword);
                    setProxy = true;
                    return true;
                }
                else
                {
                    return base.PromptAuth(aChannel, level, authInfo);
                }

                //}
                //else
                //{
                //    return base.PromptAuth(aChannel, level, authInfo);
                //}
            }

            public override nsICancelable AsyncPromptAuth(nsIChannel aChannel, nsIAuthPromptCallback aCallback, nsISupports aContext, uint level, nsIAuthInformation authInfo)
            {
                if (!setProxy)
                    throw new System.Runtime.InteropServices.COMException();
                else
                    return base.AsyncPromptAuth(aChannel, aCallback, aContext, level, authInfo);
            }
        }
    }
}
