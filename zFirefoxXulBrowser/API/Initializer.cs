using Gecko;
using Gecko.Interfaces;
using Gecko.Javascript;
using Gecko.Services;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zFirefoxXulBrowser.API
{
    public class Initializer
    {
        public static void Init(PersonData data = null)
        {
            if (data != null)
            {
                GloableProfData.PData = data;
            }

            //InitDirectoryPaths();
            //GeckoPreferences.Default["media.peerconnection.enabled"] = false;
            //GeckoPreferences.Default["media.peerconnection.use_document_iceservers"] = false;
            ////Mozilla/5.0 (Windows NT 6.2; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0
            //GeckoPreferences.Default["general.useragent.override"] = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
            ////SetSettings();
            ////InitPreferences();
            //InitProxy();
        }

        private static void InitDirectoryPaths()
        {
            //var xulfxPath = System.IO.Path.Combine(@"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\bin\Debug", "XulFx.xpi");
            //Xpcom.XulfxPath = xulfxPath;
            var XulComponents = AppDomain.CurrentDomain.BaseDirectory + "\\MozillaFx\\XulComponents";
            XulComponents = XulComponents.Replace("\\\\", "\\");
            Xpcom.ComponentsPath = XulComponents;

            var xulfxPath = AppDomain.CurrentDomain.BaseDirectory + "\\MozillaFx\\XulFx.xpi";
            xulfxPath = xulfxPath.Replace("\\\\", "\\");
            Xpcom.XulfxPath = xulfxPath;

            //var profileDirectory = @"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\bin\Debug\profile";
            //Xpcom.ProfilePath = profileDirectory;
            var profileDirectory = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + GloableProfData.PData.ProjectName);
            profileDirectory = profileDirectory.Replace("\\\\", "\\");
            //Xpcom.ProfilePath = profileDirectory;
            try
            {
                string startupFile = Path.Combine(profileDirectory, "startupCache", "startupCache.4.little");
                FileInfo fi = new FileInfo(startupFile);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
            catch { }
                        //if (!Directory.Exists(profileDirectory)) Directory.CreateDirectory(profileDirectory);
            //try
            //{
            //    var filepath = Path.Combine(profileDirectory, "userChrome.css");
            //    if (!File.Exists(filepath))
            //    {
            //        /* set default namespace to XUL */
            //        //var chrome = "@namespace url(\"http://www.mozilla.org/keymaster/gatekeeper/there.is.only.xul\");" +
            //        //    "toolbar, " +
            //        //    "toolbarpalette {" +
            //        //        "background - color: rgb(235, 235, 235) !important;" +
            //        //    "}" +
            //        //    "toolbar#nav-bar {" +
            //        //        "background-image: none !important;" +
            //        //    "}";
            //        var chrome = "@namespace url(\"http://www.mozilla.org/keymaster/gatekeeper/there.is.only.xul\");" +
            //        "toolbar {" +
            //            "background - color: rgb(235, 235, 235) !important;" +
            //        "}";
            //        File.WriteAllText(filepath, chrome);
            //    }

            //var binDirectory = @"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\PutXulRunnerFolderHere\firefox-sdk\bin";
            //Xpcom.Initialize(binDirectory);
            var binDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\MozillaFx";
            binDirectory = binDirectory.Replace("\\\\", "\\");
            Xpcom.Initialize(binDirectory);

            //var xulfxPath = Path.Combine(@"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\bin\Debug", "XulFx.xpi");
            //Xpcom.XulfxPath = xulfxPath;

            //var profileDirectory = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesMozillaFx\\" + GloableProfData.PData.ProjectName);
            //if (!Directory.Exists(profileDirectory)) Directory.CreateDirectory(profileDirectory);
            //try
            //{
            //    var filepath = Path.Combine(profileDirectory, "userChrome.css");
            //    if (!File.Exists(filepath))
            //    {
            //        /* set default namespace to XUL */
            //        //var chrome = "@namespace url(\"http://www.mozilla.org/keymaster/gatekeeper/there.is.only.xul\");" +
            //        //    "toolbar, " +
            //        //    "toolbarpalette {" +
            //        //        "background - color: rgb(235, 235, 235) !important;" +
            //        //    "}" +
            //        //    "toolbar#nav-bar {" +
            //        //        "background-image: none !important;" +
            //        //    "}";
            //        var chrome = "@namespace url(\"http://www.mozilla.org/keymaster/gatekeeper/there.is.only.xul\");" +
            //        "toolbar {" +
            //            "background - color: rgb(235, 235, 235) !important;" +
            //        "}";
            //        File.WriteAllText(filepath, chrome);
            //    }

            ////    string startupFile = Path.Combine(profileDirectory, "startupCache", "startupCache.4.little");
            ////    FileInfo fi = new FileInfo(startupFile);
            ////    if (fi.Exists)
            ////    {
            ////        fi.Delete();
            ////    }
            ////}
            ////catch { }
            //Xpcom.ProfilePath = profileDirectory;


            //var binDirectory = @"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\PutXulRunnerFolderHere\firefox-sdk\bin";
            //Xpcom.Initialize(binDirectory);


            //  nsConsoleListener.Init();
        }

        private static void InitPreferences()
        {
            //settings
            GeckoPreferences.Default["browser.xul.error_pages.enabled"] = true;
            GeckoPreferences.Default["gfx.font_rendering.graphite.enabled"] = true;
            GeckoPreferences.Default["full-screen-api.enabled"] = true;

            //performance
            GeckoPreferences.User["gfx.direct2d.disabled"] = true;
            GeckoPreferences.User["layers.acceleration.disabled"] = true;
            GeckoPreferences.User["javascript.options.jit.chrome"] = true;
            GeckoPreferences.User["webgl.force-enabled"] = true;
            //GeckoPreferences.User["layers.acceleration.force-enabled"] = true;
            GeckoPreferences.User["layers.offmainthreadcomposition.enabled"] = true;
            GeckoPreferences.User["browser.display.show_image_placeholders"] = false;
            GeckoPreferences.User["content.notify.interval"] = 500000;
            GeckoPreferences.User["content.switch.threshold"] = 250000;
            //GeckoPreferences.User["javascript.options.mem.high_water_mark"] = 512;
            GeckoPreferences.User["image.mem.max_decoded_image_kb"] = 512000;

            // GeckoPreferences.User["browser.safebrowsing.enabled"] = false;
            // GeckoPreferences.User["browser.safebrowsing.malware.enabled"] = false;
            GeckoPreferences.Default["network.http.pipelining"] = true;
            GeckoPreferences.Default["network.http.proxy.pipelining"] = true;
            GeckoPreferences.Default["network.http.pipelining.aggressive"] = true;
            GeckoPreferences.Default["network.http.pipelining.ssl"] = true;
            GeckoPreferences.Default["network.http.speculative-parallel-limit"] = 0;
            GeckoPreferences.Default["network.http.pipelining.maxrequests"] = 8;
            GeckoPreferences.Default["network.proxy.socks_remote_dns"] = false;
            GeckoPreferences.Default["network.prefetch-next"] = false;
            GeckoPreferences.Default["allow_scripts_to_close_windows"] = false;

            GeckoPreferences.Default["security.dialog_enable_delay"] = 0;
            GeckoPreferences.Default["browser.tabs.animate"] = false;
            GeckoPreferences.Default["extensions.blocklist.enabled"] = false;
            GeckoPreferences.Default["plugins.click_to_play"] = true;

            GeckoPreferences.Default["browser.cache.use_new_backend"] = 1;
            GeckoPreferences.Default["browser.download.animateNotifications"] = false;
            GeckoPreferences.Default["browser.preferences.animateFadeIn"] = false;

            GeckoPreferences.Default["geo.enabled"] = false;
            GeckoPreferences.User["geo.wifi.uri"] = "https://127.0.0.1";
            GeckoPreferences.User["geo.wifi.logging.enabled"] = false;
            GeckoPreferences.User["browser.search.geoSpecificDefaults"] = false;
            GeckoPreferences.User["browser.search.geoSpecificDefaults.url"] = "";
            GeckoPreferences.User["browser.search.geoip.url"] = "";

            GeckoPreferences.User["toolkit.telemetry.enabled"] = false;
            GeckoPreferences.User["toolkit.telemetry.server"] = "";

            GeckoPreferences.User["image.http.accept"] = "*/*";
            GeckoPreferences.User["services.sync.prefs.sync.intl.accept_languages"] = true;

            GeckoPreferences.User["capability.principal.codebase.p0.granted"] = "UniversalXPConnect";
            //GeckoPreferences.User["capability.principal.codebase.p0.id"] = "file://";
            GeckoPreferences.User["capability.principal.codebase.p0.subjectName"] = "";
            GeckoPreferences.User["security.fileuri.strict_origin_policy"] = false;

            GeckoPreferences.Default["plugin.state.npctrl"] = 0;

            //GeckoPreferences.Default["general.useragent.override"] = BrowserSettimgs.UserAgentFF;


            GeckoPreferences.Default["network.dns.disableIPv6"] = false;

            GeckoPreferences.Default["dom.ipc.plugins.flash.subprocess.crashreporter"] = false;
            GeckoPreferences.Default["dom.ipc.plugins.reportCrashURL"] = false;
            GeckoPreferences.User["dom.ipc.plugins.enabled.npctrl.dll"] = false;
            GeckoPreferences.User["dom.ipc.plugins.enabled.npqtplugin.dll"] = false;
            GeckoPreferences.User["dom.ipc.plugins.enabled.npswf32.dll"] = false;
            GeckoPreferences.User["dom.ipc.plugins.enabled.nptest.dll"] = false;
        }


        public static void InitProxy()
        {
            if (!GloableProfData.PData.ProxyIP.IsNullOrEmpty())
            {
                GeckoPreferences.User["network.proxy.type"] = 1;
                GeckoPreferences.User["network.proxy.share_proxy_settings"] = true;

                GeckoPreferences.User["network.proxy.http"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.http_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                GeckoPreferences.User["network.proxy.ssl"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.ssl_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                GeckoPreferences.User["network.proxy.backup.ssl"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.backup.ssl_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                GeckoPreferences.User["network.proxy.ftp"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.ftp_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                GeckoPreferences.User["network.proxy.backup.ftp"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.backup.ftp_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                GeckoPreferences.User["network.proxy.socks"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.socks_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                GeckoPreferences.User["network.proxy.backup.socks"] = GloableProfData.PData.ProxyIP;
                GeckoPreferences.User["network.proxy.backup.socks_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                {
                    //TODO check
                    DefaultPromptFactory.PromptGetter = () => new ProxyLoginPromptBypass();

                    GeckoPreferences.User["network.proxy.login"] = GloableProfData.PData.ProxyUsername;
                    GeckoPreferences.User["network.proxy.password"] = GloableProfData.PData.ProxyPassword;
                }
            }
        }

        public static void SetSettings()
        {
            //if (!BrowserSettimgs.DoNotTrackEnabled)
            //{
            //    GeckoPreferences.Default["privacy.donottrackheader.enabled"] = true;
            //    GeckoPreferences.Default["privacy.trackingprotection.enabled"] = true;
            //    GeckoPreferences.Default["privacy.donottrackheader.value"] = 1;
            //    GeckoPreferences.Default["services.sync.prefs.sync.privacy.donottrackheader.enabled"] = true;
            //    GeckoPreferences.Default["services.sync.prefs.sync.privacy.trackingprotection.enabled"] = true;
            //}
            //else
            //{
            //    GeckoPreferences.Default["privacy.donottrackheader.enabled"] = false;
            //    GeckoPreferences.Default["privacy.trackingprotection.enabled"] = false;
            //    GeckoPreferences.Default["privacy.donottrackheader.value"] = 0;
            //    GeckoPreferences.Default["services.sync.prefs.sync.privacy.donottrackheader.enabled"] = false;
            //    GeckoPreferences.Default["services.sync.prefs.sync.privacy.trackingprotection.enabled"] = false;
            //}

            //if (BrowserSettimgs.FlashEnabled)
            //{
            //    //plugin.state.flash
            //    GeckoPreferences.Default["plugin.state.flash"] = 2;
            //    GeckoPreferences.Default["plugin.scan.plid.all"] = true;
            //}
            //else
            //{
            //    GeckoPreferences.Default["plugin.state.flash"] = 0;
            //    GeckoPreferences.Default["plugin.scan.plid.all"] = false;
            //}

            //if (BrowserSettimgs.JavaEnabled)
            //{
            //    //plugin.state.java;1
            //    GeckoPreferences.Default["plugin.state.java"] = 1;
            //}
            //else
            //{
            //    GeckoPreferences.Default["plugin.state.java"] = 0;
            //}

            //if (BrowserSettimgs.JavascriptEnabled)
            //{
            //    //javascript.enabled;true
            //    GeckoPreferences.Default["javascript.enabled"] = true;
            //}
            //else
            //{
            //    GeckoPreferences.Default["javascript.enabled"] = false;
            //}

            ////if (BrowserSettimgs.WebRTCEnabled)
            ////{
            ////    GeckoPreferences.Default["media.peerconnection.enabled"] = true;
            ////    GeckoPreferences.Default["media.peerconnection.use_document_iceservers"] = true;
            ////}
            ////else
            ////{
            //    GeckoPreferences.Default["media.peerconnection.enabled"] = false;
            //    GeckoPreferences.Default["media.peerconnection.use_document_iceservers"] = false;
            ////}

            //if (BrowserSettimgs.WebGLEnabled)
            //{
            //    GeckoPreferences.Default["webgl.disabled"] = false;
            //    GeckoPreferences.User["webgl.force-enabled"] = true;
            //}
            //else
            //{
            //    GeckoPreferences.Default["webgl.disabled"] = true;
            //    GeckoPreferences.User["webgl.force-enabled"] = false;
            //}



            //if (BrowserSettimgs.SIFontStandard != BrowserSettimgs.AvailableFonts.IndexOf("Times New Roman"))
            //{
            //    GeckoPreferences.Default["font.default.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            //}
            //else
            //{
            //    GeckoPreferences.Default["font.default.x-western"] = "serif";
            //}
            //if (BrowserSettimgs.SIFontSerif != BrowserSettimgs.AvailableFonts.IndexOf("Times New Roman"))
            //{
            //    GeckoPreferences.Default["font.name.serif.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            //}
            //else
            //{
            //    GeckoPreferences.Default["font.name.serif.x-western"] = "Times New Roman";
            //}
            //if (BrowserSettimgs.SIFontSansSerif != BrowserSettimgs.AvailableFonts.IndexOf("Arial"))
            //{
            //    GeckoPreferences.Default["font.name.sans-serif.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            //}
            //else
            //{
            //    GeckoPreferences.Default["font.name.sans-serif.x-western"] = "Arial";
            //}
            //if (BrowserSettimgs.SIFontFixedWidth != BrowserSettimgs.AvailableFonts.IndexOf("Consolas"))
            //{
            //    GeckoPreferences.Default["font.name.monospace.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            //    GeckoPreferences.Default["font.name.cursive.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            //}
            //else
            //{
            //    GeckoPreferences.Default["font.name.monospace.x-western"] = "Courier New";
            //    GeckoPreferences.Default["font.name.cursive.x-western"] = "Comic Sans MS";
            //}
            //if (BrowserSettimgs.DefaultFontSize != 16)
            //{
            //    GeckoPreferences.Default["font.size.variable.x-western"] = BrowserSettimgs.DefaultFontSize;
            //    GeckoPreferences.Default["font.size.fixed.x-western"] = BrowserSettimgs.DefaultFontSize;
            //}
            //else
            //{
            //    GeckoPreferences.Default["font.size.variable.x-western"] = 16;
            //    GeckoPreferences.Default["font.size.fixed.x-western"] = 13;
            //}

            //if (BrowserSettimgs.HideFonts)
            //{
            //    GeckoPreferences.User["browser.display.use_document_fonts"] = 0;
            //}
            //else
            //{
            //    GeckoPreferences.Default["browser.display.use_document_fonts"] = 1;
            //}
            //GeckoPreferences.Default["font.minimum-size.x-western"] = BrowserSettimgs.MnimumFontSize;


            //GeckoPreferences.Default["general.useragent.override"] = BrowserSettimgs.UserAgentFF;

            //GeckoPreferences.User["intl.accept_languages"] = BrowserSettimgs.AcceptLanguage;
            //// GeckoPreferences.Default["extensions.qls.backup_acceptlanguages"] = BrowserSettimgs.AcceptLanguage;
            //GeckoPreferences.User["general.useragent.locale"] = BrowserSettimgs.AcceptLanguage.Contains(",") ? BrowserSettimgs.AcceptLanguage.Remove(BrowserSettimgs.AcceptLanguage.IndexOf(",")).Trim() : BrowserSettimgs.AcceptLanguage;
        }

        public static void SetPersonData(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            if (GloableProfData.PData == null)
            {
                GloableProfData.PData = new PersonData()
                {
                    BirthdayYear = birthdayYear,
                    Children = children,
                    City = city,
                    CmbSelectedIndexDay = cmbSelectedIndexDay,
                    CmbSelectedIndexMonth = cmbSelectedIndexMonth,
                    CmbSelectedIndexSex = cmbSelectedIndexSex,
                    Country = country,
                    Dir = dir,
                    Email = email,
                    FilePath = filePath,
                    FirstName = firstName,
                    InMonney = inMonney,
                    InPBNVault = inPBNVault,
                    LastName = lastName,
                    Notes = notes,
                    Password = password,
                    PhoneNumber = phoneNumber,
                    ProfileName = profileName,
                    ProjectDir = projectDir,
                    ProjectName = projectName,
                    ProxyIP = proxyIP,
                    ProxyPassword = proxyPassword,
                    ProxyPort = proxyPort,
                    ProxyUsername = proxyUsername,
                    SIPBNType = sIPBNType,
                    State = state,
                    Street = street,
                    Username = username,
                    WebAddress = webAddress,
                    Zip = zip,
                };
            }
            else
            {
                GloableProfData.PData.BirthdayYear = birthdayYear;
                GloableProfData.PData.Children = children;
                GloableProfData.PData.City = city;
                GloableProfData.PData.CmbSelectedIndexDay = cmbSelectedIndexDay;
                GloableProfData.PData.CmbSelectedIndexMonth = cmbSelectedIndexMonth;
                GloableProfData.PData.CmbSelectedIndexSex = cmbSelectedIndexSex;
                GloableProfData.PData.Country = country;
                GloableProfData.PData.Dir = dir;
                GloableProfData.PData.Email = email;
                GloableProfData.PData.FilePath = filePath;
                GloableProfData.PData.FirstName = firstName;
                GloableProfData.PData.InMonney = inMonney;
                GloableProfData.PData.InPBNVault = inPBNVault;
                GloableProfData.PData.LastName = lastName;
                GloableProfData.PData.Notes = notes;
                GloableProfData.PData.Password = password;
                GloableProfData.PData.PhoneNumber = phoneNumber;
                GloableProfData.PData.ProfileName = profileName;
                GloableProfData.PData.ProjectDir = projectDir;
                GloableProfData.PData.ProjectName = projectName;
                GloableProfData.PData.ProxyIP = proxyIP;
                GloableProfData.PData.ProxyPassword = proxyPassword;
                GloableProfData.PData.ProxyPort = proxyPort;
                GloableProfData.PData.ProxyUsername = proxyUsername;
                GloableProfData.PData.SIPBNType = sIPBNType;
                GloableProfData.PData.State = state;
                GloableProfData.PData.Street = street;
                GloableProfData.PData.Username = username;
                GloableProfData.PData.WebAddress = webAddress;
                GloableProfData.PData.Zip = zip;
            }
        }

        public static void Shutdown()
        {
            //Xpcom.Shutdown();
        }

        //private class ProxyLoginPromptBypass : DefaultPromptService, nsIAuthPrompt, nsIAuthPrompt2
        //{
        //    static bool setProxy;

        //    public override bool PromptAuth(nsIChannel aChannel, uint level, nsIAuthInformation authInfo)
        //    {
        //        if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
        //        {
        //            nsString.Set(authInfo.SetUsernameAttribute, GloableProfData.PData.ProxyUsername);
        //            nsString.Set(authInfo.SetPasswordAttribute, GloableProfData.PData.ProxyPassword);
        //            //GeckoPreferences.Default.Reset();
        //            return true;
        //        }
        //        else
        //        {
        //            return base.PromptAuth(aChannel, level, authInfo);
        //        }
        //    }

        //    public override nsICancelable AsyncPromptAuth(nsIChannel aChannel, nsIAuthPromptCallback aCallback, nsISupports aContext, uint level, nsIAuthInformation authInfo)
        //    {
        //        throw new System.Runtime.InteropServices.COMException();
        //    }
        //}
    }

    //public class nsConsoleListener : Gecko.Interfaces.nsIConsoleListener, Gecko.Interfaces.nsIObserver
    //{
    //    public static void Init()
    //    {

    //        var cobs = new nsConsoleListener();
    //        var cc = Xpcom.GetService<Gecko.Interfaces.nsIConsoleService>(Gecko.Contracts.ConsoleService);
    //        cc.RegisterListener(cobs);
    //        var svc = Xpcom.GetService<Gecko.Interfaces.nsIObserverService>(Gecko.Contracts.ObserverService);
    //        svc.AddObserver(cobs, "console-api-log-event", false);
    //    }

    //    public void Observe(Gecko.Interfaces.nsIConsoleMessage aMessage)
    //    {
    //        string message = aMessage.GetMessageAttribute();
    //        if (message.StartsWith("[JavaScript Error:"))
    //        {
    //            Console.WriteLine("[{0}] jserror: {1}", DateTime.UtcNow.ToString("HH:mm:ss"), message);
    //        }
    //    }

    //    void Gecko.Interfaces.nsIObserver.Observe(Gecko.Interfaces.nsISupports aSubject, string aTopic, string aData)
    //    {
    //        try
    //        {
    //            var js = GeckoJavascriptBridge.GetService();
    //            string s = js.EvaluateToString(aSubject, GeckoPrincipal.SystemPrincipal, "this.wrappedJSObject.arguments + ' [level: ' + this.wrappedJSObject.level + ', file: \"' + this.wrappedJSObject.filename + '\", line: ' + this.wrappedJSObject.lineNumber + ']'");
    //            Console.WriteLine("[{0}] console ({1}): {2}", DateTime.UtcNow.ToString("HH:mm:ss"), aData, s);
    //        }
    //        catch (Gecko.GeckoJavaScriptException e)
    //        {
    //            Console.WriteLine("[{0}] {1}", DateTime.UtcNow.ToString("HH:mm:ss"), e.ToString());
    //        }
    //    }
    //}
}
