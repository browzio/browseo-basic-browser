using Gecko;
using Gecko.Interop;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static zFirefoxBrowser.ViewModels.FoxTabViewModel;

namespace zFirefoxBrowser.Helpers
{
    public class FoxInit
    {
        public static string DirForXul = "";
        //public static nsIMacroPlayer JSMacroPlayer;

        static nsILocalFile toNsFile(string file)
        {
            var nsfile = Xpcom.CreateInstance<nsILocalFile>("@mozilla.org/file/local;1");
            nsfile.InitWithPath(new nsAString(file));
            return nsfile;
        }

        public static void RegisterChromeDir(string dir)
        {
            var chromeDir = toNsFile(dir);
            var chromeFile = chromeDir.Clone();
            chromeFile.Append(new nsAString("chrome.manifest"));
            Xpcom.ComponentRegistrar.AutoRegister(chromeFile);
            Xpcom.ComponentManager.AddBootstrappedManifestLocation(chromeDir);
        }

        public static void Init(PersonData data = null)
        {
           // Debugger.Launch();
            try
            {
                if (data != null)
                {
                    GloableProfData.PData = data;
                }
                // GeckoWebBrowser.UseCustomPrompt();


                Xpcom.AfterInitalization += () =>
                {

                    //RegisterChromeDir(@"C:\Users\eli\Desktop\temp\ffcopy");
                    //nsIDirectoryService directoryService = Xpcom.GetService<nsIDirectoryService>("@mozilla.org/file/directory_service;1");
                    //if (directoryService != null) directoryService.RegisterProvider(new DirectoryServiceProvider());

                LauncherDialog.Download += LauncherDialog_Download;

                    SetProxyIfNeeded();

                    Xpcom.RegisterFactory(typeof(nsIFilePicker).GUID, "MacroFilePicker", "@mozilla.org/filepicker;1", new MacroFilePickerFactory(MacroFilePicker.Instance));
                    //const string ComponentManagerCID = "91775d60-d5dc-11d2-92fb-00e09805570f";
                    //nsIComponentRegistrar mgr = (nsIComponentRegistrar)Xpcom.GetObjectForIUnknown((IntPtr)Xpcom.GetService(new Guid(ComponentManagerCID)));
                    //Guid aClass = new Guid("a7139c0e-962c-44b6-bec3-aaaaaaaaaaab");
                    //Xpcom.RegisterFactory(aClass, "MyCSharpComClass", "@geckofx/mysharpclass;1", new ViewModels.FoxTabViewModel.MyCSharpComClassFactory());
                    //mgr.RegisterFactory(ref aClass, "Example C sharp com component", "@geckofx/mysharpclass;1", new ViewModels.FoxTabViewModel.MyCSharpComClassFactory());

                    //python C:\mozilla-build\xulrunner-41.0.2.en-US.win32.sdk\xulrunner-sdk\sdk\bin\header.py nsIMacroPlayer.idl -o nsIMacroPlayer.h -I C:\mozilla-build\xulrunner-41.0.2.en-US.win32.sdk\xulrunner-sdk\idl
                    //python C:\mozilla-build\xulrunner-41.0.2.en-US.win32.sdk\xulrunner-sdk\sdk\bin\typelib.py nsIMacroPlayer.idl -o nsIMacroPlayer.xpt -I C:\mozilla-build\xulrunner-41.0.2.en-US.win32.sdk\xulrunner-sdk\idl
                    var addonPathMAcros = AppDomain.CurrentDomain.BaseDirectory + "\\FFAddons\\MacrosJs";
                    RegisterChromeDir(addonPathMAcros);

                    //const string ComponentManagerCID = "91775d60-d5dc-11d2-92fb-00e09805570f";
                    //nsIComponentRegistrar mgr = (nsIComponentRegistrar)Xpcom.GetObjectForIUnknown((IntPtr)Xpcom.GetService(new Guid(ComponentManagerCID)));
                    //Guid aClass = new Guid("a7139c0e-962c-44b6-bec3-aaaaaaaaaaab");
                    //mgr.RegisterFactory(ref aClass, "Js To Browser", "@eli.browz.io/jsToBrowser;1", new MacrosComClassFactory());

                    // JSMacroPlayer = Xpcom.CreateInstance<nsIMacroPlayer>("@eli.browz.io/jsmacroaddon;1");
                    //JSMacroPlayer = Xpcom.CreateInstance<nsIMacroPlayer>("@eli.browz.io/jsmacroaddon;1");
                    //Xpcom.RegisterFactory(typeof(nsIMacroPlayer).GUID, "MacroPlayerClass", "@eli.browz.io/jsmacroaddon;1", new MacroPlayerClassFactory(MacroPlayerClass.Instance));
                };

                //setup cache path
                string profilepath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + GloableProfData.PData.ProjectName);
                if (!Directory.Exists(profilepath)) Directory.CreateDirectory(profilepath);
                else
                {
                    try
                    {
                        string startupFile = Path.Combine(profilepath, "startupCache", "startupCache.4.little");
                        FileInfo fi = new FileInfo(startupFile);
                        if (fi.Exists && fi.CreationTime <= new DateTime(2017, 9, 24,21,30,00))
                        {
                            fi.Delete();
                        }
                    }
                    catch { }
                }
                Xpcom.ProfileDirectory = profilepath;

                var xulpath = AppDomain.CurrentDomain.BaseDirectory + "\\FFLibrary\\Firefox";
                xulpath = xulpath.Replace("\\\\", "\\");
                Xpcom.Initialize(xulpath);
                DirForXul = xulpath;
                //Xpcom.Initialize(@"C:\Users\eli\Desktop\temp\ff");
                //DirForXul = @"C:\Users\eli\Desktop\temp\ff";

                //GeckoPreferences.User["devtools.chrome.enabled"] = true;
                //security.mixed_content.block_active_content
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
                user_pref("geo.wifi.logging.enabled", false);
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

                GeckoPreferences.Default["general.useragent.override"] = BrowserSettimgs.UserAgentFF;


                GeckoPreferences.Default["network.dns.disableIPv6"] = false;
                //GeckoPreferences.User["network.proxy.type"] = 1;
                //GeckoPreferences.User["network.proxy.share_proxy_settings"] = true;

                //GeckoPreferences.User["network.proxy.http"] = "104.238.156.110";
                //GeckoPreferences.User["network.proxy.http_port"] = 8800;

                //GeckoPreferences.User["network.proxy.ssl"] = "104.238.156.110";
                //GeckoPreferences.User["network.proxy.ssl_port"] = 8800;



                //GeckoPreferences.Default["accessibility.blockautorefresh"] = true;
                //GeckoPreferences.Default["browser.fullscreen.animate"] = 0;
                //GeckoPreferences.Default["nglayout.initialpaint.delay"] = 0;
                //GeckoPreferences.Default["content.notify.backoffcount"] = 5;
                //GeckoPreferences.User["breakpad.reportURL"] = "";
                //GeckoPreferences.User["browser.send_pings"] = false;
                //GeckoPreferences.User["browser.send_pings.require_same_host"] = true;
                //        [TestCase("gfx.font_rendering.graphite.enabled", true)]
                //[TestCase("dom.max_script_run_time", 0)]
                //[TestCase("browser.xul.error_pages.enabled", false)]
                //[TestCase("accessibility.force_disabled", 1)]
                //[TestCase("middlemouse.paste", false)]
                //[TestCase("middlemouse.paste", false)]
                //[TestCase("capability.principal.codebase.p0.granted", "UniversalXPConnect")]
                //[TestCase("capability.principal.codebase.p0.granted", "file://")]
                //[TestCase("capability.principal.codebase.p0.subjectName", "")]
                //[TestCase("security.fileuri.strict_origin_policy", false)]
                //[TestCase("layout.css.devPixelsPerPx", "1.0")]
                //[TestCase("breakpad.reportURL", "")]
                //[TestCase("breakpad.reportURL", "abcdefghijklmnopqrstuvwxyz!@#$%^&*()")]
                //[TestCase("breakpad.reportURL", "\u00fe\u00ff\uf323")]
                //GeckoPreferences.User["capability.principal.codebase.p0.granted"] = "UniversalXPConnect";
                //GeckoPreferences.User["capability.principal.codebase.p0.id"] = "file://";
                //GeckoPreferences.User["capability.principal.codebase.p0.subjectName"] = "";
                //GeckoPreferences.User["security.fileuri.strict_origin_policy"] = false;

                //GeckoPreferences.User["media.gmp-provider.enabled"] = false;
                //GeckoPreferences.User["media.gmp-gmpopenh264.enabled"] = false;
                //GeckoPreferences.User["media.peerconnection.video.enabled"] = false;
                //GeckoPreferences.User["network.disable.ipc.security"] = true;
                //GeckoPreferences.User["extensions.blocklist.enabled"] = true;
                //GeckoPreferences.User["plugin.scan.4xPluginFolder"] = false;
                //GeckoPreferences.User["application.use_ns_plugin_finder"] = true;

                //user_pref("network.http.pipelining", true);
                //user_pref("network.http.pipelining.abtest", false);
                //user_pref("network.http.pipelining.aggressive", true);
                //user_pref("network.http.pipelining.max-optimistic-requests", 3);
                //user_pref("network.http.pipelining.maxrequests", 12);
                //user_pref("network.http.pipelining.maxsize", 300000);
                //user_pref("network.http.pipelining.read-timeout", 60000);
                //user_pref("network.http.pipelining.reschedule-on-timeout", true);
                //user_pref("network.http.pipelining.reschedule-timeout", 15000);
                //user_pref("network.http.pipelining.ssl", true);
                //user_pref("network.http.proxy.pipelining", true);

                //user_pref("network.http.max-connections", 256);
                //user_pref("network.http.max-persistent-connections-per-proxy", 256);
                //user_pref("network.http.max-persistent-connections-per-server", 6);

                //user_pref("network.http.redirection-limit", 20);
                //user_pref("network.http.fast-fallback-to-IPv4", true);
                //user_pref("network.dns.disablePrefetch", true);
                //user_pref("network.prefetch-next", true);

                //user_pref("browser.safebrowsing.downloads.enabled", false);
                //user_pref("browser.safebrowsing.downloads.remote.enabled", false);
                //user_pref("browser.safebrowsing.enabled", false);
                //user_pref("browser.safebrowsing.maleware.enabled", false);

                // GeckoPreferences.Default["general.useragent.override"] = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0-0.20) Gecko/20100101 Firefox/45.0";


                NeedsToSetProxy = !GloableProfData.PData.ProxyIP.IsNullOrEmpty();

                //foreach (Gecko.Plugins.PluginTag tag in Gecko.Plugins.PluginHost.GetPluginTags())
                //{
                //    if (tag.Name.ToLower().Contains("silverlight"))
                //    {
                //    }
                //}

                //ComPtr<nsIBlocklistService> pluginHost = Xpcom.GetService2<nsIBlocklistService>(Contracts.b);
                //pluginHost.Instance.

            }
            catch { }
        }

        public static async Task<bool> AwaitforProxySet()
        {
            if (FoxInit.NeedsToSetUserPass || FoxInit.NeedsToSetProxy)
            {
                bool setProxyPass = await Task<bool>.Run(async () =>
                {
                    int ranInWhile = 0;
                    while (FoxInit.NeedsToSetUserPass || FoxInit.NeedsToSetProxy)
                    {
                        //try
                        //{
                        //    System.Threading.Thread.Sleep(250);
                        //}
                        //catch { }
                        await Task.Delay(250);
                        if (ranInWhile++ >= 120)
                        {
                            FoxInit.NeedsToSetUserPass = false;
                            FoxInit.NeedsToSetProxy = false;
                            return false;
                        }
                    }
                    return true;
                });
            }

            return true;
        }

        public static void SetSettings()
        {
            if (!BrowserSettimgs.DoNotTrackEnabled)
            {
                GeckoPreferences.Default["privacy.donottrackheader.enabled"] = true;
                GeckoPreferences.Default["privacy.trackingprotection.enabled"] = true;
                GeckoPreferences.Default["privacy.donottrackheader.value"] = 1;
                GeckoPreferences.Default["services.sync.prefs.sync.privacy.donottrackheader.enabled"] = true;
                GeckoPreferences.Default["services.sync.prefs.sync.privacy.trackingprotection.enabled"] = true;
            }
            else
            {
                GeckoPreferences.Default["privacy.donottrackheader.enabled"] = false;
                GeckoPreferences.Default["privacy.trackingprotection.enabled"] = false;
                GeckoPreferences.Default["privacy.donottrackheader.value"] = 0;
                GeckoPreferences.Default["services.sync.prefs.sync.privacy.donottrackheader.enabled"] = false;
                GeckoPreferences.Default["services.sync.prefs.sync.privacy.trackingprotection.enabled"] = false;
            }

            if (BrowserSettimgs.FlashEnabled)
            {
                //plugin.state.flash
                GeckoPreferences.Default["plugin.state.flash"] = 2;
                GeckoPreferences.Default["plugin.scan.plid.all"] = true;
            }
            else
            {
                GeckoPreferences.Default["plugin.state.flash"] = 0;
                GeckoPreferences.Default["plugin.scan.plid.all"] = false;
            }

            if (BrowserSettimgs.JavaEnabled)
            {
                //plugin.state.java;1
                GeckoPreferences.Default["plugin.state.java"] = 1;
            }
            else
            {
                GeckoPreferences.Default["plugin.state.java"] = 0;
            }

            if (BrowserSettimgs.JavascriptEnabled)
            {
                //javascript.enabled;true
                GeckoPreferences.Default["javascript.enabled"] = true;
            }
            else
            {
                GeckoPreferences.Default["javascript.enabled"] = false;
            }

            if (BrowserSettimgs.WebRTCEnabled)
            {
                GeckoPreferences.Default["media.peerconnection.enabled"] = true;
                GeckoPreferences.Default["media.peerconnection.use_document_iceservers"] = true;
            }
            else
            {
                GeckoPreferences.Default["media.peerconnection.enabled"] = false;
                GeckoPreferences.Default["media.peerconnection.use_document_iceservers"] = false;
            }

            if (BrowserSettimgs.WebGLEnabled)
            {
                GeckoPreferences.Default["webgl.disabled"] = false;
                GeckoPreferences.User["webgl.force-enabled"] = true;
            }
            else
            {
                GeckoPreferences.Default["webgl.disabled"] = true;
                GeckoPreferences.User["webgl.force-enabled"] = false;
            }



            if (BrowserSettimgs.SIFontStandard != BrowserSettimgs.AvailableFonts.IndexOf("Times New Roman"))
            {
                GeckoPreferences.Default["font.default.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            }
            else
            {
                GeckoPreferences.Default["font.default.x-western"] = "serif";
            }
            if (BrowserSettimgs.SIFontSerif != BrowserSettimgs.AvailableFonts.IndexOf("Times New Roman"))
            {
                GeckoPreferences.Default["font.name.serif.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            }
            else
            {
                GeckoPreferences.Default["font.name.serif.x-western"] = "Times New Roman";
            }
            if (BrowserSettimgs.SIFontSansSerif != BrowserSettimgs.AvailableFonts.IndexOf("Arial"))
            {
                GeckoPreferences.Default["font.name.sans-serif.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            }
            else
            {
                GeckoPreferences.Default["font.name.sans-serif.x-western"] = "Arial";
            }
            if (BrowserSettimgs.SIFontFixedWidth != BrowserSettimgs.AvailableFonts.IndexOf("Consolas"))
            {
                GeckoPreferences.Default["font.name.monospace.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
                GeckoPreferences.Default["font.name.cursive.x-western"] = BrowserSettimgs.AvailableFonts[BrowserSettimgs.SIFontStandard];
            }
            else
            {
                GeckoPreferences.Default["font.name.monospace.x-western"] = "Courier New";
                GeckoPreferences.Default["font.name.cursive.x-western"] = "Comic Sans MS";
            }
            if (BrowserSettimgs.DefaultFontSize != 16)
            {
                GeckoPreferences.Default["font.size.variable.x-western"] = BrowserSettimgs.DefaultFontSize;
                GeckoPreferences.Default["font.size.fixed.x-western"] = BrowserSettimgs.DefaultFontSize;
            }
            else
            {
                GeckoPreferences.Default["font.size.variable.x-western"] = 16;
                GeckoPreferences.Default["font.size.fixed.x-western"] = 13;
            }

            if (BrowserSettimgs.HideFonts)
            {
                GeckoPreferences.User["browser.display.use_document_fonts"] = 0;
            }
            else
            {
                GeckoPreferences.Default["browser.display.use_document_fonts"] = 1;
            }
            GeckoPreferences.Default["font.minimum-size.x-western"] = BrowserSettimgs.MnimumFontSize;
            

            GeckoPreferences.Default["general.useragent.override"] = BrowserSettimgs.UserAgentFF;
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

        public static bool DidsetProxy { get;  set; }
        public static bool NeedsToSetUserPass { get; set; }
        public static bool NeedsToSetProxy { get;  set; }

        public static void SetProxyIfNeeded()
        {
            try
            {
                if (DidsetProxy) return;
                DidsetProxy = true;
                GeckoWebBrowser browser = new GeckoWebBrowser();
                browser.Navigate("https://whoer.net/");
                browser.Navigated += (s, ee) =>
                {
                    if (GloableProfData.PData != null && !GloableProfData.PData.ProxyIP.IsNullOrEmpty())
                    {
                        try
                        {
                            GeckoPreferences.User["network.proxy.type"] = 1;
                            GeckoPreferences.User["network.proxy.share_proxy_settings"] = true;

                            GeckoPreferences.User["network.proxy.http"] = GloableProfData.PData.ProxyIP;
                            GeckoPreferences.User["network.proxy.http_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                            //GeckoPreferences.User["client.proxy.http"] = GloableProfData.PData.ProxyIP;
                            //GeckoPreferences.User["client.proxy.http_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
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
                            if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty()) NeedsToSetUserPass = true;

                            NeedsToSetProxy = false;
                            if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                            {
                                PromptFactory.PromptServiceCreator = () => new ProxyLoginPromptBypass();
                                //GeckoPreferences.Default["browser.xul.error_pages.enabled"] = false;

                                GeckoPreferences.User["network.proxy.login"] = GloableProfData.PData.ProxyUsername;
                                GeckoPreferences.User["network.proxy.password"] = GloableProfData.PData.ProxyPassword;
                            }
                            else
                            {
                                NeedsToSetUserPass = false;
                            }


                            //browser.Dispose();

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
                };
            }
            catch { }
        }

        private static void user_pref(string pref, object val)
        {
            GeckoPreferences.User[pref] = val;
        }

        private static async void LauncherDialog_Download(object sender, LauncherDialogEvent launcherdialoge)
        {
            string url = launcherdialoge.Url;  //url to download
            string fullpath = "";//destination file absolute path
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

                url = launcherdialoge.Url;  //url to download
                fullpath = saveFileDialog1.FileName; //destination file absolute path
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
            }
            catch(Exception ex)
            {
                try
                {
                    nsILocalFile objTarget = Xpcom.CreateInstance<nsILocalFile>("@mozilla.org/file/local;1");

                    using (nsAString tmp = new nsAString(@Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\temp.tmp"))
                    {
                        objTarget.InitWithPath(tmp);
                    }

                    nsIURI source = IOService.CreateNsIUri(url);
                    nsIURI dest = IOService.CreateNsIUri(new Uri(fullpath).AbsoluteUri);
                    nsAStringBase t = (nsAStringBase)new nsAString(System.IO.Path.GetFileName(fullpath));

                    nsIWebBrowserPersist persist = Xpcom.CreateInstance<nsIWebBrowserPersist>("@mozilla.org/embedding/browser/nsWebBrowserPersist;1");

                    nsITransfer nst = Xpcom.CreateInstance<nsITransfer>("@mozilla.org/transfer;1");
                    nst.Init(source, dest, t, launcherdialoge.Mime, 0, null, persist, false);

                    if (nst != null)
                    {
                        persist.SetPersistFlagsAttribute(2 | 32 | 16384);
                        persist.SetProgressListenerAttribute((nsIWebProgressListener)nst);
                        persist.SaveURI(source, null, null, (uint)Gecko.nsIHttpChannelConsts.REFERRER_POLICY_NO_REFERRER, null, null, (nsISupports)dest, null);

                        try
                        {
                            while (persist.GetCurrentStateAttribute() != (uint)nsIWebBrowserPersistConsts.PERSIST_STATE_FINISHED)
                            {
                                await Task.Run(() => { System.Threading.Thread.Sleep(1000); });
                            }

                            Process.Start(fullpath);
                        }
                        catch { }
                    }
                }
                catch (Exception eex)
                {
                }
            }

            //Xpcom.InitChromeContext();
            //RegisterChromeDir(@"C:\Users\eli\Desktop\temp\browser");
        }

        /// <summary>
        /// to change error context
        /// </summary>
        public class ErrorModeContext : IDisposable
        {
            [DllImport("kernel32.dll")]
            static extern FilterDelegate SetUnhandledExceptionFilter(FilterDelegate lpTopLevelExceptionFilter);
            public delegate bool FilterDelegate(Exception ex);

            private readonly int _oldMode;

            public ErrorModeContext(ErrorModes mode)
            {
                FilterDelegate fd = delegate (Exception ex)
                {
                    return true;
                };
                _oldMode = SetErrorMode((int)mode);
            }

            ~ErrorModeContext()
            {
                Dispose(false);
            }

            private void Dispose(bool disposing)
            {
                SetErrorMode(_oldMode);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            [DllImport("kernel32.dll")]
            private static extern int SetErrorMode(int newMode);
        }

        [Flags]
        public enum ErrorModes
        {
            Default = 0x0,
            FailCriticalErrors = 0x0001,
            NoGpFaultErrorBox = 0x2, // &lt;- this is the one we need
            NoAlignmentFaultExcept = 0x0004,
            NoOpenFileErrorBox = 0x8000,
            SEM_NOGPFAULTERRORBOX = 0x0002,
        }

        public static void Shutdown()
        {
            //using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            //{
                try
                {
                    Xpcom.Shutdown();


                //if (JSMacroPlayer != null)
                //{
                //    using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
                //    {
                //        IntPtr pUnk = Marshal.GetIUnknownForObject(JSMacroPlayer);
                //        Marshal.Release(pUnk);
                //    }
                //}
            }
                catch { }
           // }
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
                    NeedsToSetUserPass = false;
                    //GeckoPreferences.Default.Reset();
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
               // if (!setProxy)
                    throw new System.Runtime.InteropServices.COMException();
                //else
                //    return base.AsyncPromptAuth(aChannel, aCallback, aContext, level, authInfo);
            }

            //public override nsICancelable AsyncPromptAuth(nsIChannel aChannel, nsIAuthPromptCallback aCallback, nsISupports aContext, uint level, nsIAuthInformation authInfo)
            //{
            //    string userName = nsString.Get(authInfo.GetUsernameAttribute);
            //    string password = nsString.Get(authInfo.GetPasswordAttribute);

            //    string realm = nsString.Get(authInfo.GetRealmAttribute);

            //    nsString.Set(authInfo.SetUsernameAttribute, GloableProfData.PData.ProxyUsername);
            //    nsString.Set(authInfo.SetPasswordAttribute, GloableProfData.PData.ProxyPassword);
            //    //aCallback.OnAuthAvailable(aContext, authInfo);

            //    Cancelable cancel = new Cancelable();
            //    return cancel;
            //}

            //            public nsICancelable AsyncPromptAuth(nsIChannel aChannel, nsIAuthPromptCallback aCallback, nsISupports aContext, uint level, nsIAuthInformation authInfo)
            //            {
            //                if (!setProxy && !GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
            //                {
            //                    string userName = nsString.Get(authInfo.GetUsernameAttribute);
            //                    string password = nsString.Get(authInfo.GetPasswordAttribute);

            //                    string realm = nsString.Get(authInfo.GetRealmAttribute);

            //                    Timer t = new Timer();
            //                    t.Interval = 1000;
            //                    t.Start();
            //                    t.Elapsed += (s, e) =>
            //                    {
            //                        nsString.Set(authInfo.SetUsernameAttribute, "USERNAME");
            //                        nsString.Set(authInfo.SetPasswordAttribute, "PASSWORD");
            //                        aCallback.OnAuthAvailable(aContext, authInfo);
            //                        t.Stop();
            //                    };

            //                    Cancelable cancel = new Cancelable();
            //                    return cancel;
            //                }
            //                else
            //                {
            //                    return base.AsyncPromptAuth(aChannel, aCallback, aContext, level, authInfo);
            //                }
            //            }

            //private class Cancelable : nsICancelable
            //{
            //    public int Reason { set; get; }
            //    public void Cancel(int aReason)
            //    {
            //        Reason = aReason;
            //    }
            //}
        }

        private class DirectoryServiceProvider : nsIDirectoryServiceProvider
        {
            public nsIFile GetFile(string prop, ref bool persistent)
            {
                switch (prop)
                {
                    //case "AppData":
                    //    return (nsIFile)Xpcom.NewNativeLocalFile(MyFilesDatabase.GetBaseDir() + "AppData");
                    default:
                        Debug.WriteLine("Gecko.Xpcom.DirectoryServiceProvider.GetFile: not implemented: " + prop);
                        return null;
                }
            }
        }
    }
}
