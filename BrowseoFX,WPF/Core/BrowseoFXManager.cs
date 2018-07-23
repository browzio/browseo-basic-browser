using BrowseoFX_WPF.Core.BrowserHandlers;
using BrowseoFX_WPF.Core.BrowserHandlers.GUI;
using BrowseoFX_WPF.Core.BrowserListeners;
using BrowseoFX_WPF.Core.DataAccess;
using Gecko;
using Gecko.Interfaces;
using Gecko.Interop;
using Gecko.Javascript;
using Gecko.Services;
using Gecko.Windows;
using Gecko.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static Gecko.Xpcom;
using Gecko.CustomMarshalers;
using Gecko.DOM;
using BrowseoFX_WPF.Core.Services;
using System.Windows;
using BrowseoFX_WPF.Core.Services.Browser;
using System.IO;
using BrowseoFX_WPF.Core.Services.BFXpcom;
using Gecko.Security;
using Gecko.GUI;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System.Threading;
using Organiser.Common.Windows;
using Organiser.Common;
using System.Collections.ObjectModel;
using Browser.Common.Models;
using Organiser.Common.Classes.SocialHelpers;
using BrowseoFX_WPF.Core.Services.Browser.Crawlers;
using DragDropListview;
using System.IO.Compression;
using BrowseoFX_WPF.Windows;

namespace BrowseoFX_WPF.Core
{
    public class nsTextInputProcessorCallback : nsITextInputProcessorCallback
    {
        [return: MarshalAs(UnmanagedType.U1)]
        public bool OnNotify([MarshalAs(UnmanagedType.Interface)] nsITextInputProcessor aTextInputProcessor, [MarshalAs(UnmanagedType.Interface)] nsITextInputProcessorNotification aNotification)
        {
            return true;
        }
    }
    public interface IListenToFXManager
    {
        void OnOpenIAMacros();
        void OnOpenFBConverseo();
        void OnOpenLSB();
        void OnOpenSEO();
        void OnOpenSocialStats(SocialStatsCrawlerService sscs);
    }
    public interface IListenToFXManagerFC
    {
        void OnCurateToPBN(string html, string uri);
        void OnSentForSeo(string sitename,string url);
    }
    public class BrowseoFXManager : 
        IWebviewListenerService,
        nsIDOMEventListener,
        nsIObserver
    {
        public PersonData Project { get { return GloableProfData.PData; } }
        public IListenToFXManager FxMAnagerListenerNotifyer { get; set; }
        public IListenToFXManagerFC ListenToFXManagerFC { get; set; }

        public TimezoneDataAccess TimezoneManager { get; set; }

        // public SessionStoreService SessionStoreService { get; private set; }
        //public FormsManagerService FormsManagerService { get; private set; }

        public PanelUIHandler PanelUIHandler { get; private set; }

        public TabbrowserHandler TabbrowserHandler { get; private set; }
        public SettingsHandler SettingsHandler { get; private set; }

        public Gecko.Windows.WebView GloableWebView { get; set; }
        public GeckoXULElement MainWindowElement
        {
            get
            {
                return GloableWebView.Widget.View.Document.GetElementById("main-window") as GeckoXULElement;
            }
        }

        public event Action OnReadyToBrowse; 

        public string BFXMacrosDirectory { get; set; }

        #region init and shutdown

        public static void SetPersonData()
        {
            string path = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp");
            string sitesFilePath = System.IO.Path.Combine(File.ReadAllText(path + "\\info.txt"), "ProjectData.ini");
            //string sitesFilePath = System.IO.Path.Combine(@"C:\Users\eli\AppData\Local\RAWSocialOrganizer\Projects\worpress", "ProjectData.ini");
            //string sitesFilePath = System.IO.Path.Combine(@"C:\Users\eli\AppData\Local\RAWSocialOrganizer\Projects\microsoft rename", "ProjectData.ini");
            IniFile ini = new IniFile(sitesFilePath);
            PersonData profile = new PersonData();
            try
            {
                profile.ProjectName = ini.IniReadValue("Data", "ProjectName");
                profile.ProfileName = ini.IniReadValue("Data", "ProfileName");
                profile.FirstName = ini.IniReadValue("Data", "FirstName");
                profile.LastName = ini.IniReadValue("Data", "LastName");
                profile.Email = ini.IniReadValue("Data", "Email");
                profile.Password = ini.IniReadValue("Data", "Password");
                profile.Username = ini.IniReadValue("Data", "Username");
                profile.ProxyIP = ini.IniReadValue("Data", "ProxyIP");
                profile.ProxyPort = ini.IniReadValue("Data", "ProxyPort");
                profile.ProxyUsername = ini.IniReadValue("Data", "ProxyUsername");
                profile.ProxyPassword = ini.IniReadValue("Data", "ProxyPassword");
                profile.PhoneNumber = ini.IniReadValue("Data", "PhoneNumber");
                profile.Street = ini.IniReadValue("Data", "Street");
                profile.City = ini.IniReadValue("Data", "City");
                profile.State = ini.IniReadValue("Data", "State");
                profile.Zip = ini.IniReadValue("Data", "Zip");
                profile.Country = ini.IniReadValue("Data", "Country");
                profile.WebAddress = ini.IniReadValue("Data", "WebAddress");
                profile.Notes = ini.IniReadValue("Data", "Notes");
                try
                {
                    profile.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                    profile.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                    profile.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                }
                catch { }
                profile.ProjectDir = sitesFilePath.Replace("\\ProjectData.ini", "");
                try
                {
                    profile.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }
            GloableProfData.PData = profile;
            //FoxInit.Init(profile);
            // Initializer.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }

        public async Task Init()
        {
            var binDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\firefox-sdk\\bin";
            //var binDirectory = @"C:\Users\eli\Desktop\move\xilium-xilium.cefglue-335450e6011d\BrowserAndFeatures\bin\x86\Debug" + "\\firefox-sdk\\bin";
            Xpcom.SDKDirectory = binDirectory;
            Xpcom.BFXComponentsDirectory = Path.Combine(binDirectory, "browser", "BFXComponents");

////#if DEBUG
//            SetPersonData();
////#endif


            


            if (GloableProfData.PData == null)
            {
                await Task.Delay(250);
                for (int i = 0; i < 7; i++)
                {
                    if (GloableProfData.PData != null) break;

                    await Task.Delay(250);
                }
            }


            #region init_xpcoms


            if (GloableProfData.PData == null || !FXServices.DirectoryServiceProvider.InitWithProjectName(GloableProfData.PData.ProjectName))
            {
                //TODO:NICE MESSAGEBOX
               // MessageBox.Show("No profile for the project was able to be created.");
            }
            else
            {
                //try
                //{
                //    string secFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "SiteSecurityServiceState.txt");
                //    if (File.Exists(secFile)) File.Delete(secFile);
                //}
                //catch { }

                //try
                //{
                //    string startupFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "startupCache", "startupCache.4.little");
                //    FileInfo fi = new FileInfo(startupFile);
                //    if (fi.Exists)
                //    {
                //        fi.Delete();
                //    }
                //}
                //catch { }

                //try
                //{

                //    string projpath = MyFilesDatabase.FindProjectDirByName(GloableProfData.PData.ProjectName, "");
                //    var PersonData = MyFilesDatabase.GetUpPdaaFromPath(projpath);
                //    string ffpath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + GloableProfData.PData.ProjectName);

                //    MyFilesDatabase.LaunchToSystemFF("", ffpath, PersonData, false);
                //}
                //catch
                //{

                //}

                var user_prefs = "";
                if (File.Exists(FXServices.PreferencesService.UserPrefsFile))
                {
                    user_prefs = File.ReadAllText(FXServices.PreferencesService.UserPrefsFile);
                }
                //string oldCurrentDir = Directory.GetCurrentDirectory();

                //MOZ_TOOLKIT_SEARCH
                Environment.SetEnvironmentVariable("MOZ_TOOLKIT_SEARCH", "1");
                Environment.SetEnvironmentVariable("MOZ_SANDBOX", "");
                Environment.SetEnvironmentVariable("MOZ_DISABLE_CONTENT_SANDBOX", "1");
                Environment.SetEnvironmentVariable("MOZ_DISABLE_GMP_SANDBOX", "1");
                Environment.SetEnvironmentVariable("MOZ_DISABLE_NPAPI_SANDBOX", "1");
                Environment.SetEnvironmentVariable("MOZ_DISABLE_GPU_SANDBOX", "1");

                Xpcom.InitializeXPCOM(Xpcom.SDKDirectory, FXServices.DirectoryServiceProvider);

                //Directory.SetCurrentDirectory(Xpcom.SDKDirectory);
                FXServices.RegisterDirectoryService();

                Xpcom.InitializeThreadManager();

                //if (File.Exists(FXServices.PreferencesService.UserPrefsFileTmp))
                //{
                //    File.WriteAllText(FXServices.PreferencesService.UserPrefsFile, File.ReadAllText(FXServices.PreferencesService.UserPrefsFileTmp));
                //    File.Delete(FXServices.PreferencesService.UserPrefsFileTmp);
                //}
                //using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
                //{
                //    PreferencesService.Instance.ReadUserPrefs(FXServices.DirectoryServiceProvider.NewLocalFile(FXServices.PreferencesService.UserPrefsFile));
                //}

                FXServices.PreferencesService.SetPrefs(user_prefs);
                SettingsHandler.LoadAndSetPrefenceSettings();
                //using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
                //{
                //    PreferencesService.Instance.ReadUserPrefs(FXServices.DirectoryServiceProvider.NewLocalFile(FXServices.PreferencesService.UserPrefsFile));
                //}
                Xpcom.DoEvents();

                Xpcom.RegisterInstance(typeof(nsIXULRuntime).GUID, "nsXULAppInfo", Gecko.Contracts.AppInfo, nsXULAppInfo.Instance);


                //XPC_XPCONNECT_CONTRACTID[]     = "@mozilla.org/js/xpc/XPConnect;1";
                using (var nsXPConnect = Xpcom.GetService2<Gecko.Interfaces.nsIXPConnect>("@mozilla.org/js/xpc/XPConnect;1"))
                {
                }

                using (var TransportService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.TransportService)) { }
                using (var DnsService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.DnsService)) { }
                using (var NetworkIOService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.NetworkIOService)) { }
                using (var ChromeRegistry = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.ChromeRegistry)) { }
                using (var sessionStore = Xpcom.GetService2<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1")) { }
                    //// using (var SuppressorService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.SuppressorService)) { }

                    ////"XulFx.xpi"
                    Xpcom.LoadExtension(BFXComponentsDirectory, "components.manifest");

                BFXMacrosDirectory = Path.Combine(binDirectory, "Imacros 8", "src");
                Xpcom.LoadExtension(BFXMacrosDirectory, "chrome.manifest");



                //Defaullt Fx Addons
                try
                {
                    var defaulltFxAddons = AppDomain.CurrentDomain.BaseDirectory + "\\Defaullt Fx Addons";
                    defaulltFxAddons = defaulltFxAddons.Replace("\\\\", "\\");
                    foreach (var item in new DirectoryInfo(defaulltFxAddons).GetFiles())
                    {
                        Xpcom.LoadExtension(item.FullName, "");
                    }
                }
                catch { }

                //using (var catMan = Xpcom.GetService2<nsICategoryManager>(Contracts.CategoryManager))
                //{

                //}

                //Xpcom.LoadExtension(BFXMacrosDirectory, "components.manifest");

                //Xpcom.LoadExtension(@"C:\Users\eli\Desktop\temp\omni1\omni\chrome", "chrome.manifest");

                //Xpcom.RegisterInstance(typeof(nsISessionStore).GUID, "SessionStore", "@mozilla.org/browser/sessionstore;1", FXServices.SessionStoreService);

                FXServices.ObserverService.AddObserver(new nsObserverAny(), "browserClassInitializer_12345", false);
                FXServices.ObserverService.AddObserver(new nsObserverAny(), "browserClassInitializer_12345", false);

                Xpcom.RegisterInstance(typeof(nsIWinTaskbar).GUID, "WinTaskbar", "@mozilla.org/windows-taskbar;1", nsWinTaskbar.Instance);
                Xpcom.RegisterInstance(typeof(nsIBrowserHandler).GUID, "nsBrowserContentHandler", "@mozilla.org/browser/clh;1", nsBrowserHandler.Instance);
                Xpcom.RegisterInstance(typeof(nsIBrowserGlue).GUID, "nsBrowserGlue", "@mozilla.org/browser/browserglue;1", nsBrowserGlue.Instance);
                Xpcom.RegisterInstance(typeof(nsIToolkitProfileService).GUID, "nsToolkitProfileService", "@mozilla.org/toolkit/profile-service;1", nsToolkitProfileService.Instance);
                ////nsIXulfxDOMWindowHelper
                ////



                using (var windowWatcher = Xpcom.GetService2<nsIWindowWatcher>(Contracts.WindowWatcher))
                {
                    windowWatcher.Instance.SetWindowCreator(FXServices.WindowCreator);
                }


                using (var startupNotifier = Xpcom.GetService2<Gecko.Interfaces.nsIObserver>(Gecko.Contracts.AppStartupNotifier))
                {
                    startupNotifier.Instance.Observe(null, "app-startup", null);
                }


                using (var appStartupSrv = Xpcom.GetService2<Gecko.Interfaces.nsIAppStartup>(Gecko.Contracts.AppStartup))
                {
                    appStartupSrv.Instance.CreateHiddenWindow();

                    using (var obsSvc = Xpcom.GetService2<Gecko.Interfaces.nsIObserverService>(Gecko.Contracts.ObserverService))
                    {
                        //sessionstore-init-started
                        //obsSvc.Instance.AddObserver(new nsObserverAny(), "xul-window-registered", false);
                        obsSvc.Instance.AddObserver(new nsObserverAny(), "sessionstore-init-started", false);

                        obsSvc.Instance.AddObserver(this, "browser-delayed-startup-finished", false);

                        obsSvc.Instance.NotifyObservers(null, "app-startup", null);
                        obsSvc.Instance.NotifyObservers(null, "profile-do-change", "startup");


                        Xpcom.DoEvents();
                        //addons
                        using (var em = Xpcom.GetService2<Gecko.Interfaces.nsIObserver>(Gecko.Contracts.AddonsIntegration))
                        {
                            em.Instance.Observe(null, "addons-startup", null);
                        }


                        obsSvc.Instance.NotifyObservers(null, "profile-after-change", "startup");
                        NS_CreateServicesFromCategory("profile-after-change", null, "profile-after-change");
                        //using (var browserSearchService = Xpcom.GetService2<Gecko.Interfaces.nsIBrowserSearchService>("@mozilla.org/browser/search-service;1"))
                        //{
                        //    browserSearchService.Instance.Init(null);
                        //}

                        obsSvc.Instance.NotifyObservers(null, "profile-initial-state", null);

                        obsSvc.Instance.NotifyObservers(null, "load-extension-defaults", null);

                        //using (var SessionStartup = Xpcom.GetService2<Gecko.Interfaces.nsISessionStartup>(Gecko.Contracts.SessionStartup))
                        //{
                        //    var re = SessionStartup.Instance.DoRestore();
                        //}

                        //cmdline app
                        //cmdline nsDefaultCLH
                        using (var cmdLine = Xpcom.GetService2<Gecko.Interfaces.nsICommandLineRunner>(Gecko.Contracts.CommandLine))
                        {
                            cmdLine.Instance.Run();
                        }


                        // clear out any environment variables which may have been set
                        // during the relaunch process now that we know we won't be relaunching.
                        Environment.SetEnvironmentVariable("XRE_PROFILE_PATH", "");
                        Environment.SetEnvironmentVariable("XRE_PROFILE_LOCAL_PATH", "");
                        Environment.SetEnvironmentVariable("XRE_PROFILE_NAME", "");
                        Environment.SetEnvironmentVariable("XRE_START_OFFLINE", "");
                        Environment.SetEnvironmentVariable("NO_EM_RESTART", "");
                        Environment.SetEnvironmentVariable("XUL_APP_FILE", "");
                        Environment.SetEnvironmentVariable("XRE_BINARY_PATH", "");



                        obsSvc.Instance.NotifyObservers(null, "final-ui-startup", null);
                    }

                    DefaultPromptFactory.Init();
                    //DefaultPromptFactory.Init();
                    //DownloadManager.Init();
                    //Xpcom.BeforeShutdown += (s, e) =>
                    //{
                    //    DownloadManager.Shutdown();
                    //};
                    //DownloadManager.DownloadRequest += DownloadManager_DownloadRequest;
                    //DownloadManager.DownloadCompleted += DownloadManager_DownloadCompleted;
                    CertOverrideService.GetService().ValidityOverride += Certificate_ValidityOverride;


                    appStartupSrv.Instance.DoneStartingUp();
                    //appStartupSrv.Instance.Run();
                }


                //Directory.SetCurrentDirectory(oldCurrentDir);
            }



            //try
            //{
            //    string projextDir = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "extensions");
            //    foreach (var item in new DirectoryInfo(projextDir).GetFiles())
            //    {
            //        using (ZipArchive archive = ZipFile.OpenRead(item.FullName))
            //        {
            //            foreach (ZipArchiveEntry entry in archive.Entries)
            //            {
            //                if (entry.FullName == "install.rdf")
            //                {
            //                    Xpcom.LoadExtension(item.FullName, "");
            //                }
            //            }
            //        }



            //        //var extractPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp", item.Name);
            //        //if (!Directory.Exists(extractPath)) Directory.CreateDirectory(extractPath);

            //        //System.IO.Compression.ZipFile.ExtractToDirectory(item.FullName, extractPath);
            //        // Console.WriteLine(item.FullName);
            //    }
            //}
            //catch { }

            Xpcom.DoEvents();
            #endregion

            GloableWebView = new WebView();
            GloableWebView.WebviewListenerService = this;
            //GloableWebView.Navigated += GloableWebView_Navigated;
            //FXServices.ObserverService.AddObserver(this, "http-on-examine-response", false);
        }
        void NS_CreateServicesFromCategory(string category, nsISupports origin, string observerTopic)
        {
            nsICategoryManager catMan = null;
            nsISimpleEnumerator enumerator = null;
            nsIUTF8StringEnumerator senumerator = null;
            try
            {
                catMan = Xpcom.GetService<nsICategoryManager>(Contracts.CategoryManager);
                if (catMan == null)
                    return;

                enumerator = catMan.EnumerateCategory(category);
                if (enumerator == null)
                    return;

                senumerator = Xpcom.QueryInterface<nsIUTF8StringEnumerator>(enumerator);
                if (senumerator == null)
                    return;

                while (senumerator.HasMore())
                {
                    nsISupports serviceInstance = null;
                    nsIObserver observer = null;
                    try
                    {
                        string entryString = nsString.Get(senumerator.GetNext);
                        string contractID = catMan.GetCategoryEntry(category, entryString);
                        serviceInstance = Xpcom.GetService<nsISupports>(contractID);
                        if (serviceInstance == null || observerTopic == null)
                            continue;

                        observer = Xpcom.QueryInterface<nsIObserver>(serviceInstance);
                        if (observer == null)
                            continue;

                        observer.Observe(origin, observerTopic, "");
                    }
                    catch (OutOfMemoryException) { }
                    catch (COMException) { }
                    finally
                    {
                        Xpcom.FreeComObject(ref serviceInstance);
                        Xpcom.FreeComObject(ref observer);
                    }
                }
            }
            finally
            {
                Xpcom.FreeComObject(ref catMan);
                Xpcom.FreeComObject(ref enumerator);
                Xpcom.FreeComObject(ref senumerator);
            }
        }

        private void DownloadManager_DownloadCompleted(object sender, DownloadEventArgs e)
        {
        }

        private void DownloadManager_DownloadRequest(object sender, DownloadRequestEvent e)
        {
        }

        private void Certificate_ValidityOverride(object sender, CertOverrideEventArgs e)
        {

        }

        //internal async Task Init()
        //{
        //    var binDirectory = @"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\PutXulRunnerFolderHere\firefox-sdk\bin";
        //    Xpcom.SDKDirectory = binDirectory;
        //    Xpcom.BFXComponentsDirectory = Path.Combine(binDirectory, "browser", "BFXComponents");

        //    #region todo read profiles from...
        //    Project = new PersonData()
        //    {
        //        BIADefault = false,
        //        BirthdayYear = 1991,
        //        Children = "",
        //        City = "jerusalem",
        //        CmbSelectedIndexDay = 29,
        //        CmbSelectedIndexMonth = 4,
        //        CmbSelectedIndexSex = 0,
        //        Country = "israel",
        //        Email = "elimdadia@gmail.com",
        //        FirstName = "eli",
        //        InMonney = true,
        //        LastName = "dadia",
        //        Notes = "rockin",
        //        PhoneNumber = "0548795470",
        //        Password = "mwahaha",
        //        ProfileName = "demo profile",
        //        ProjectName = "BFXDemo6",
        //    };

        //    Project.Profiles.Add(Project);
        //    for (int i = 0; i < 5; i++)
        //    {
        //        Project.Profiles.Add(new PersonData()
        //        {
        //            BIADefault = false,
        //            BirthdayYear = 1991 + i,
        //            Children = "",
        //            City = "jerusalem",
        //            CmbSelectedIndexDay = 29,
        //            CmbSelectedIndexMonth = 4,
        //            CmbSelectedIndexSex = 0,
        //            Country = "israel" + 1,
        //            Email = i + "elimdadia@gmail.com",
        //            FirstName = "eli" + i,
        //            InMonney = true,
        //            LastName = "dadia" + i,
        //            Notes = "rockin" + i,
        //            PhoneNumber = "0548795470" + i,
        //            Password = "mwahaha" + i,
        //            ProfileName = "demo profile name" + i,
        //            ProjectName = "BFXDemo6",
        //            Username = "Ninja" + i
        //        });
        //    }
        //    //TODO: make relient on reading already saved prefs await SettingsHandler.LoadPreferenceSettingsAsync();
        //    #endregion


        //    #region init_xpcoms


        //    if (!FXServices.DirectoryServiceProvider.InitWithProjectName(Project.ProjectName))
        //    {
        //        //TODO:NICE MESSAGEBOX
        //        MessageBox.Show("No profile for the project was able to be created.");
        //    }
        //    else
        //    {
        //        //MOZ_TOOLKIT_SEARCH
        //        Environment.SetEnvironmentVariable("MOZ_TOOLKIT_SEARCH", "1");
        //        Environment.SetEnvironmentVariable("MOZ_SANDBOX", "");
        //        Environment.SetEnvironmentVariable("MOZ_DISABLE_CONTENT_SANDBOX", "1");
        //        Environment.SetEnvironmentVariable("MOZ_DISABLE_GMP_SANDBOX", "1");
        //        Environment.SetEnvironmentVariable("MOZ_DISABLE_NPAPI_SANDBOX", "1");
        //        Environment.SetEnvironmentVariable("MOZ_DISABLE_GPU_SANDBOX", "1");

        //        FXServices.DirectoryServiceProvider.CreateCacheFiles();
        //        Xpcom.InitializeXPCOM(Xpcom.SDKDirectory, FXServices.DirectoryServiceProvider);

        //        using (var directoryService = Xpcom.GetService2<nsIDirectoryService>(Gecko.Contracts.DirectoryService))
        //        {
        //            if (directoryService != null)
        //                directoryService.Instance.RegisterProvider(FXServices.DirectoryServiceProvider);
        //        }
        //        Xpcom.InitializeThreadManager();
        //        Xpcom.DoEvents();
        //        Xpcom.RegisterInstance(typeof(nsIXULRuntime).GUID, "nsXULAppInfo", Gecko.Contracts.AppInfo, nsXULAppInfo.Instance);


        //        //using (var profileService = Xpcom.CreateInstance2<Gecko.Interfaces.nsIToolkitProfileService>("@mozilla.org/toolkit/profile-service;1"))
        //        //{
        //        //    var userFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath);
        //        //    var lfile = FXServices.DirectoryServiceProvider.NewLocalFile(userFile);
        //        //    try
        //        //    {
        //        //        using (var nam = new nsAUTF8String(Project.ProjectName))
        //        //        {
        //        //            profileService.Instance.CreateProfile(lfile, nam);
        //        //        }
        //        //    }
        //        //    finally
        //        //    {
        //        //        Xpcom.FreeComObject(ref lfile);
        //        //    }
        //        //}

        //        //
        //        //amIAddonManagerStartup
        //        //var startup = new nsAppStartup();
        //        //Xpcom.RegisterInstance(typeof(nsIAppStartup).GUID, "nsAppStartup", Gecko.Contracts.AppStartup, startup);
        //        //using (var appStartupSrv = Xpcom.GetService2<Gecko.Interfaces.nsIAppStartup>(Gecko.Contracts.AppStartup))
        //        //{
        //        //    appStartupSrv.Instance.GetStartupInfo();//.CreateInstanceWithProfile(new nsToolkitProfile());
        //        //}

        //        //using (var nsToolkitChromeRegistry = Xpcom.GetService2<Gecko.Interfaces.nsIToolkitChromeRegistry>("@mozilla.org/chrome/chrome-registry;1"))
        //        //{
        //        //    var uri = IOService.GetService().CreateNsIUri("resource://gre/modules/BrowserUtils.jsm");
        //        //   // nsToolkitChromeRegistry.Instance.AllowScriptsForPackage
        //        //    nsToolkitChromeRegistry.Instance.WrappersEnabled(uri);
        //        //}


        //        FXServices.PreferencesService.SetPrefs();

        //        FXServices.ObserverService.AddObserver(new nsObserverAny(), "browserClassInitializer_12345", false);


        //        using (var TransportService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.TransportService)) { }
        //        using (var DnsService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.DnsService)) { }
        //        using (var NetworkIOService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.NetworkIOService)) { }
        //        using (var ChromeRegistry = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.ChromeRegistry)) { }
        //        // using (var SuppressorService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(Gecko.Contracts.SuppressorService)) { }

        //        using (var windowWatcher = Xpcom.GetService2<nsIWindowWatcher>(Contracts.WindowWatcher))
        //        {
        //            using (var creator = Xpcom.GetService2<Gecko.Interfaces.nsIWindowCreator>(Gecko.Contracts.AppStartup))
        //            {
        //                windowWatcher.Instance.SetWindowCreator(creator.Instance);
        //            }
        //            //windowWatcher.Instance.SetWindowCreator(WindowCreator.Instance);
        //        }


        //        Xpcom.LoadExtension(BFXComponentsDirectory, "components.manifest");
        //        //var xulfxPath = System.IO.Path.Combine(@"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\bin\Debug", "XulFx.xpi");
        //        ////Xpcom.XulfxPath = xulfxPath;
        //        //Xpcom.LoadExtension(@"C:\Users\eli\Documents\Visual Studio 2015\Projects\Firefox Builds\Builds\xulfx\Rebuilt\vmas-xulfx-a340969180cd\PutXulRunnerFolderHere\firefox-sdk\bin\browser\BFXComponents\interfaces.xpt", "");

        //        //Xpcom.RegisterInstance(typeof(nsISessionStore).GUID, "SessionStore", "@mozilla.org/browser/sessionstore;1", BrowseoFXServicesManager.SessionStoreService);

        //        Xpcom.RegisterInstance(typeof(nsIWinTaskbar).GUID, "WinTaskbar", "@mozilla.org/windows-taskbar;1", nsWinTaskbar.Instance);
        //        Xpcom.RegisterInstance(typeof(nsIBrowserHandler).GUID, "nsBrowserContentHandler", "@mozilla.org/browser/clh;1", nsBrowserHandler.Instance);
        //        Xpcom.RegisterInstance(typeof(nsIBrowserGlue).GUID, "nsBrowserGlue", "@mozilla.org/browser/browserglue;1", nsBrowserGlue.Instance);
        //        Xpcom.RegisterInstance(typeof(nsIToolkitProfileService).GUID, "nsToolkitProfileService", "@mozilla.org/toolkit/profile-service;1", nsToolkitProfileService.Instance);
        //        //nsIXulfxDOMWindowHelper
        //        //

        //        using (var startupNotifier = Xpcom.GetService2<Gecko.Interfaces.nsIObserver>(Gecko.Contracts.AppStartupNotifier))
        //        {
        //            startupNotifier.Instance.Observe(null, "app-startup", null);
        //        }

        //        using (var appStartupSrv = Xpcom.GetService2<Gecko.Interfaces.nsIAppStartup>(Gecko.Contracts.AppStartup))
        //        {
        //            appStartupSrv.Instance.CreateHiddenWindow();

        //            using (var obsSvc = Xpcom.GetService2<Gecko.Interfaces.nsIObserverService>(Gecko.Contracts.ObserverService))
        //            {
        //                //obsSvc.Instance.AddObserver(new nsObserverAny(), "browser-delayed-startup-finished", false);

        //                obsSvc.Instance.NotifyObservers(null, "app-startup", null);
        //                obsSvc.Instance.NotifyObservers(null, "profile-do-change", "startup");


        //                Xpcom.DoEvents();
        //                //addons
        //                using (var em = Xpcom.GetService2<Gecko.Interfaces.nsIObserver>(Gecko.Contracts.AddonsIntegration))
        //                {
        //                    em.Instance.Observe(null, "addons-startup", null);
        //                }

        //                using (var SessionStartup = Xpcom.GetService2<Gecko.Interfaces.nsISessionStartup>(Gecko.Contracts.SessionStartup)) { }
        //                //using (var browserSearchService = Xpcom.GetService2<Gecko.Interfaces.nsIBrowserSearchService>("@mozilla.org/browser/search-service;1"))
        //                //{
        //                //    browserSearchService.Instance.Init(null);
        //                //}

        //                obsSvc.Instance.NotifyObservers(null, "load-extension-defaults", null);
        //                obsSvc.Instance.NotifyObservers(null, "profile-after-change", "startup");
        //                obsSvc.Instance.NotifyObservers(null, "profile-initial-state", null);


        //                //cmdline app
        //                //cmdline nsDefaultCLH
        //                using (var cmdLine = Xpcom.GetService2<Gecko.Interfaces.nsICommandLineRunner>(Gecko.Contracts.CommandLine))
        //                {
        //                    cmdLine.Instance.Run();
        //                }


        //                // clear out any environment variables which may have been set
        //                // during the relaunch process now that we know we won't be relaunching.
        //                //Environment.SetEnvironmentVariable("XRE_PROFILE_PATH", "");
        //                //Environment.SetEnvironmentVariable("XRE_PROFILE_LOCAL_PATH", "");
        //                //Environment.SetEnvironmentVariable("XRE_PROFILE_NAME", "");
        //                //Environment.SetEnvironmentVariable("XRE_START_OFFLINE", "");
        //                //Environment.SetEnvironmentVariable("NO_EM_RESTART", "");
        //                //Environment.SetEnvironmentVariable("XUL_APP_FILE", "");
        //                //Environment.SetEnvironmentVariable("XRE_BINARY_PATH", "");



        //                obsSvc.Instance.NotifyObservers(null, "final-ui-startup", null);
        //            }


        //            appStartupSrv.Instance.DoneStartingUp();
        //            appStartupSrv.Instance.Run();
        //        }
        //    }

        //    Xpcom.DoEvents();
        //    #endregion


        //    GloableWebView = new WebView();
        //    GloableWebView.WebviewListenerService = this;
        //    GloableWebView.Navigated += GloableWebView_Navigated;
        //}

        public void Shutdown()
        {

            //var userPrefsFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefs.js");
            //var file = FXServices.DirectoryServiceProvider.NewLocalFile(userPrefsFile);
            //try
            //{
            // nsPrefBranch.PrefService.Instance.SavePrefFile(null);
            //}
            //finally
            //{
            //    Xpcom.FreeComObject(ref file);
            //}
            //using (var observerService = Xpcom.GetService2<Gecko.Interfaces.nsIObserverService>("@mozilla.org/observer-service;1"))
            //{
            //    observerService.Instance.NotifyObservers(null, "xpcom-will-shutdown", null);
            //    //observerService.Instance.NotifyObservers(Xpcom.ServiceManager.Instance as Gecko.Interfaces.nsISupports, "xpcom-shutdown", null);
            //}
            //FXServices.ObserverService.NotifyObservers(null, "browser-lastwindow-close-granted", null);
            //FXServices.ObserverService.NotifyObservers(null, "quit-application-granted", "syncShutdown");
            //FXServices.ObserverService.NotifyObservers(null, "quit-application", "");

            //using (var sessionStore = Xpcom.GetService2<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1"))
            //{
            //    sessionStore.Instance.Observe(null, "quit-application-granted", "syncShutdown");
            //    sessionStore.Instance.Observe(null, "quit-application", "");
            //    //sessionStore.Instance.RestoreLastSession();
            //}

            //string projextDir = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "extensions");
            //foreach (var item in new DirectoryInfo(projextDir).GetFiles())
            //{
            //    var extractPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp", item.Name);
            //    if (!Directory.Exists(extractPath)) Directory.CreateDirectory(extractPath);

            //    System.IO.Compression.ZipFile.ExtractToDirectory(item.FullName, extractPath);
            //    // Console.WriteLine(item.FullName);
            //    // Xpcom.LoadExtension(item.FullName, "");
            //}
            GloableWebView.Dispose();
            using (var appStartupSrv = Xpcom.GetService2<Gecko.Interfaces.nsIAppStartup>(Gecko.Contracts.AppStartup))
            {
                appStartupSrv.Instance.DestroyHiddenWindow();
            }
                FXServices.PreferencesService.Shutdown();
            Xpcom.Shutdown(false);
        }

        #endregion

        #region IWebviewListenerService
        public void BuiltWindowCore(GeckoXULWindow widget)
        {
        }

        //TODO:ew
        bool hitit = false;
        public void GloableBrowserDocumentLoaded()
        {
            if (hitit) return;
            hitit = true;
            

            try
            {
                string projextDir = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "extensions");
                foreach (var item in new DirectoryInfo(projextDir).GetFiles())
                {
                    Xpcom.LoadExtension(item.FullName, "");
                    //using (ZipArchive archive = ZipFile.OpenRead(item.FullName))
                    //{
                    //    foreach (ZipArchiveEntry entry in archive.Entries)
                    //    {
                    //        if (entry.FullName == "install.rdf")
                    //        {
                    //            Xpcom.LoadExtension(item.FullName, "");
                    //        }
                    //    }
                    //}



                    //var extractPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp", item.Name);
                    //if (!Directory.Exists(extractPath)) Directory.CreateDirectory(extractPath);

                    //System.IO.Compression.ZipFile.ExtractToDirectory(item.FullName, extractPath);
                    // Console.WriteLine(item.FullName);
                }
            }
            catch { }

            //using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
            //{
            //    PreferencesService.Instance.ReadUserPrefs(FXServices.DirectoryServiceProvider.NewLocalFile(FXServices.PreferencesService.UserPrefsFile));
            //}



            //GeckoTextInputProcessor inputProcessor = new GeckoTextInputProcessor();
            //inputProcessor.BeginInputTransaction(GloableWebView.Window, new nsTextInputProcessorCallback());
            //inputProcessor.BeginInputTransaction(GloableWebView.Widget.View, new nsTextInputProcessorCallback());

            //var sessionStore = Xpcom.GetService<Gecko.Interfaces.nsIObserver>("@mozilla.org/browser/sessionstore;1");
            //FXServices.ObserverService.NotifyObservers(null, "browser:purge-session-history", "");

            //Panel Ui Edits
           PanelUIHandler.Init();

            //Session store events
            //using (GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(MainWindowElement.Instance).Wrap(GeckoDOMEventTarget.Create))
            //{
            //    foreach (var evnt in BrowseoFXServicesManager.SessionStoreService.TabEvents)
            //    {
            //        eventTarget.AddEventListener(evnt, BrowseoFXServicesManager.SessionStoreService, true, true, 2);
            //    }
            //}
            //using (GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(MainWindowElement.Instance).Wrap(GeckoDOMEventTarget.Create))
            //{
            //    foreach (var evnt in BrowseoFXServicesManager.SessionStoreService.TabEvents)
            //    {
            //        eventTarget.AddEventListener(evnt, BrowseoFXServicesManager.SessionStoreService, true, true, 2);
            //    }
            //}


            //using (var thisWindow = Xpcom.QueryInterface2<mozIDOMWindowProxy>(BrowseoFXManager.Instance.GloableWebView.Widget.View.Instance))
            //{
            //    using (var domchromeWindow = Xpcom.QueryInterface2<nsIDOMChromeWindow>(thisWindow.Instance))
            //    {
            //        using (var group = new nsAString("browsers"))
            //        {
            //            var mm = domchromeWindow.Instance.GetGroupMessageManager(group);

            //            foreach (var message in BrowseoFXServicesManager.SessionStoreService.Messages)
            //            {
            //                group.SetData(message);
            //                mm.AddMessageListener(group, BrowseoFXServicesManager.SessionStoreService, true);
            //            }

            //            group.SetData("chrome://browser/content/content-sessionStore.js");

            //            var mmff = mm as nsIFrameScriptLoader;
            //            mmff.LoadFrameScript(group, true, true);

            //        }
            //    }
            //}


            TabbrowserHandler.Init();
           // var chromeRegistry = Xpcom.GetService<Gecko.Interfaces.nsIXULChromeRegistry>("@mozilla.org/chrome/chrome-registry;1");
            //chromeRegistry.ReloadChrome();
            //chromeRegistry.RefreshSkins();
            //FormsManagerService.Instance.SaveProjectsProfilesToFormHistory();






            //Most visited check

        }

        public void GloableViewLoadDone()
        {
            //try
            //{
            //    //var mainWindow = (Gecko.DOM.GeckoXULElement)GloableWebView.Widget.View.Document.GetElementById("main-window");

            //    GloableWebView.Navigate(FXServices.PreferencesService.User.GetLocalizedString("browser.startup.homepage"));
            //    // GloableWebView.Navigate("facebook.com");

            //    //var tabbrowser = (Gecko.DOM.GeckoXULElement)GloableWebView.Widget.View.Document.GetElementById("content");
            //    //var tab = (Gecko.DOM.GeckoXULElement)tabbrowser.GetProperty<nsIDOMNode>("selectedTab").Wrap(GeckoNode.Create);

            //    ////GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(tabbrowser.Instance).Wrap(GeckoDOMEventTarget.Create);
            //    ////eventTarget.AddEventListener("TabOpen", this, true, true, 2);

            //    //GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(GloableWebView.Widget.View.Document.Instance).Wrap(GeckoDOMEventTarget.Create);
            //    //eventTarget.AddEventListener("TabOpen", this, true, true, 2);

            //    //using (var cmdLine = Xpcom.GetService2<Gecko.Interfaces.nsICommandLineRunner>(Gecko.Contracts.CommandLine))
            //    //{
            //    //    cmdLine.Instance.SetWindowContext(GloableWebView.Window.Instance);
            //    //    cmdLine.Instance.Run();
            //    //}
            //}
            //catch (Exception e)
            //{

            //}

            try
            {
                if (OnReadyToBrowse == null)
                {
                    //GetDefultHomePage
                    //"browser.startup.homepage"
                    var homepage = MyFilesDatabase.GetDefultHomePage();
                    if (homepage == "") homepage = FXServices.PreferencesService.User.GetLocalizedString("browser.startup.homepage");
                    GloableWebView.Navigate(homepage);

                    MaybeMigrateBookmarksForFF52();
                }
                else
                {
                    OnReadyToBrowse();
                }
                

                ////nsINavBookmarksService prefService = Xpcom.GetService<nsINavBookmarksService>(typeof(nsINavBookmarksService).GUID);
                //int i = 0;

                //using (var TransportService = Xpcom.GetService2<Gecko.Interfaces.nsISupports>(typeof(nsINavBookmarksService).GUID))
                //{
                //}
                //System.Timers.Timer t = new System.Timers.Timer(60000);
                //t.Elapsed += T_Elapsed;
                //t.Start();
            }
            catch (Exception e)
            {

            }
        }
        //private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    var homepage = MyFilesDatabase.GetDefultHomePage();
        //    if (homepage == "") homepage = FXServices.PreferencesService.User.GetLocalizedString("browser.startup.homepage");
        //    GloableWebView.Navigate(homepage);
        //}
        #endregion

        #region migration 
        bool called = false;

        private void MaybeMigrateBookmarksForFF52()
        {
            if (DragDropMainViewModel.Instance.FinishedLoadingInstance)
            {
                if (DragDropMainViewModel.Instance.FoldersAndSitesList.Count > 0)
                {
                    string migrationDataDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "Migration", GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(migrationDataDir)) Directory.CreateDirectory(migrationDataDir);

                    string migrationFile = Path.Combine(migrationDataDir, "Bookmarks52.html");
                    if (File.Exists(migrationFile)) return;

                    try
                    {
                        StringBuilder htmlContent = new StringBuilder("");//temporary buffer to save FF bookmark file in it
                        StringBuilder spacer = new StringBuilder("    ");//number of space in beginning of each line in FF bookmark file
                        try
                        {
                            htmlContent.Append(@"<!DOCTYPE NETSCAPE-Bookmark-file-1>"
                                             + "\r\n<!-- This is an automatically generated file."
                                             + "\r\n     It will be read and overwritten."
                                             + "\r\n     DO NOT EDIT! -->");
                            htmlContent.Append("\r\n<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");
                            htmlContent.Append("\r\n<TITLE>Bookmarks</TITLE>\r\n<H1>Bookmarks Menu</H1>");
                            htmlContent.Append("\r\n<DL><p>");

                            //Converts XML file to FF bookmark file  
                            foreach (var bmark in DragDropMainViewModel.Instance.FoldersAndSitesList)
                            {
                                if (bmark.IsFolder)
                                {
                                    htmlContent.Append(
                                    "\r\n" + "<DT><H3>"
                                    + bmark.Name
                                    + "</H3>"
                                    + "\r\n" + spacer.ToString() + "<DL><p>");
                                    foreach (var bmarkin in bmark.Sites)
                                    {
                                        htmlContent.Append(
                                        "\r\n" + spacer.ToString()
                                        + "<DT><A HREF=\"" + bmarkin.Link + "\">"
                                        + bmarkin.Name
                                        + "</A>");
                                    }
                                     htmlContent.Append(
                                    "\r\n" + spacer.ToString() + "</DL><p>");
                                }
                                else
                                {
                                    htmlContent.Append(
                                        "\r\n" + spacer.ToString()
                                        + "<DT><A HREF=\"" + bmark.Link + "\">"
                                        + bmark.Name
                                        + "</A>");
                                }
                            }

                            if (spacer.Length > 0) spacer.Length -= 4;
                            htmlContent.Append("\r\n" + spacer.ToString() + "</DL>");
                        }
                        catch
                        {
                            return;
                        }

                        File.WriteAllText(migrationFile, htmlContent.ToString());
                        System.Diagnostics.Process.Start(migrationDataDir);

                        GloableWebView.Window.OpenDialog("chrome://browser/content/places/places.xul", "Library", "chrome,toolbar=yes,dialog=no,resizable", null);
                    }
                    catch (Exception ex)
                    {
                    }

                }
            }
            else { called = true; }
        }
        #endregion

        #region Webview
        internal void RefreshBrowserTabs()
        {
            var reloadMenuItem = GloableWebView.Widget.View.Document.GetElementById("context_reloadAllTabs");
            var mouseEvent = GloableWebView.Widget.View.Document.CreateEvent<Gecko.DOM.GeckoMouseEvent>("MouseEvent");
            try
            {
                mouseEvent.InitEvent("command", true, true);
                reloadMenuItem.DispatchEvent(mouseEvent);
            }
            catch (Exception e)
            {
                //if (e.ErrorCode != GeckoError.NS_ERROR_FAILURE)
                //    throw;
            }
            finally
            {
                Xpcom.FreeComObject(ref reloadMenuItem);
                Xpcom.FreeComObject(ref mouseEvent);
            }
        }


        internal void OpenTimeSwitchPage()
        {
            try
            {
                // var webnav = GloableWebView.Window.QueryInterface<Gecko.Interfaces.nsIWebNavigation>().Wrap(GeckoWebNavigation.Create);
                //  if(webnav == null)
                //  {
                //    var wm = Xpcom.GetService<Gecko.Interfaces.nsIWindowMediator>(Contracts.WindowMediator);
                //    var win = wm.GetMostRecentWindow();
                //var webnav = Xpcom.QueryInterface<Gecko.Interfaces.nsIWebNavigation>(win).Wrap(GeckoWebNavigation.Create);
               // GloableWebView.Widget.View.Navigator.Instance.LoadURI(SettingsHandler.DefaultFontSettings, GeckoLoadFlags.None, null, null, null);
                // }
                //webnav.LoadURI(SettingsHandler.DefaultFontSettings, GeckoLoadFlags.None, null, null, null);

                // GloableWebView.Navigate

                // var wm = Xpcom.GetService<Gecko.Interfaces.nsIWindowMediator>(Contracts.WindowMediator);
                // var win = wm.GetMostRecentWindow("navigator:browser");
                // Gecko.DOM.GeckoWindow gwin = Gecko.DOM.GeckoWindow.Create(win);
                //var nav = gwin.QueryInterface<Gecko.Interfaces.nsIWebNavigation>().Wrap(GeckoWebNavigation.Create);
                // nav.LoadURI(SettingsHandler.DefaultFontSettings, GeckoLoadFlags.None, null, null, null);

                //var uri = IOService.GetService().Instance.NewURI(new nsAUTF8String(SettingsHandler.DefaultFontSettings),null,null);

                //// Open URL in user's default browser.
                //var extps = Xpcom.GetService2<Gecko.Interfaces.nsIExternalProtocolService>("@mozilla.org/uriloader/external-protocol-service;1"); 
                //extps.Instance.LoadURI(uri, null);

                // GeckoJavascriptBridge.GetService().Evaluate("browser.tabs.create({ url:\"https://example.org\"});");
                // var val = GloableWebView.Window.Evaluate("browser.tabs.create({ url:\"https://example.org\"});");
            }
            catch
            {

            }
           //var window = GloableWebView.Widget.View.Document.DefaultView.Open(SettingsHandler.DefaultFontSettings, null, null);//.Location.Assign("view-source:" + window.Document.Uri);

            //var evt = GloableWebView.Widget.View.Document.CreateEvent<Gecko.DOM.GeckoUIEvent>("UIEvents");
            //evt.InitUIEvent("TabOpen", true, false, GloableWebView.Window, 0);
            //newTab.browser.dispatchEvent(evt);

            // GloableWebView.Navigate(SettingsHandler.DefaultFontSettings);
        }

        private void GloableWebView_Navigated(object sender, GeckoNavigatedEventArgs e)
        {
            
            //nsObserverService.ObserverService.NotifyObservers(null, "browseoFX-getwindowsAndPageCount", "");
            //using (var windowMediator = Xpcom.GetService2<Gecko.Interfaces.nsIWindowMediator>(Contracts.WindowMediator))
            //{
            //    var win = windowMediator.Instance.GetMostRecentWindow("navigator:browser");
            //    //observerService.Instance.NotifyObservers(Xpcom.ServiceManager.Instance as Gecko.Interfaces.nsISupports, "xpcom-shutdown", null);
            //}

            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            //using (var windowMediator = Xpcom.GetService2<Gecko.Interfaces.nsIWindowMediator>("@mozilla.org/appshell/window-mediator;1"))
            //{
            //    var windo = windowMediator.Instance.GetEnumerator(null);
            //    while (windo.HasMoreElements())
            //    {
            //        var el = windo.GetNext();

            //    }
            //}

            //using (var sessionStore = Xpcom.GetService2<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1"))
            //{
            //    using (var result = new nsAString())
            //    {
            //        sessionStore.Instance.GetBrowserState(result);
            //        var resString = result.ToString();
            //        Console.WriteLine(resString);
            //    }

            //    using (var result = new nsAString())
            //    {
            //        sessionStore.Instance.GetWindowState(GloableWebView.Widget.View.Instance, result);
            //        var resString = result.ToString();
            //        Console.WriteLine(resString);
            //    }
            //}
            //Console.WriteLine("====================================================");
            //Console.WriteLine("====================================================");
            //Console.WriteLine("====================================================");
        }

        #endregion

        #region nsIDOMEventListener

        public void HandleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent @event)
        {
            var args = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(GeckoDOMEventArgs.Create);
            switch (args.Type)
            {
                case "TabOpen":
                    var tabbrowser = (Gecko.DOM.GeckoXULElement)GloableWebView.Widget.View.Document.GetElementById("content");

                    var tst = Xpcom.QueryInterface<Gecko.Interfaces.nsIWinTaskbar>("@mozilla.org/windows-taskbar;1");
                    //nsIDOMCustomEvent evt = Xpcom.QueryInterface<Gecko.Interfaces.nsIDOMCustomEvent>(args.Instance);
                    //nsIDOMEventTarget target = Xpcom.QueryInterface<nsIDOMEventTarget>(tabbrowser.Instance);
                    //var evt = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(Gecko.DOM.GeckoEvent.Create);

                    //var evt = GloableWebView.Widget.View.Document.CreateEvent<Gecko.DOM.GeckoEvent>("CustomEvent");
                    //evt.InitEvent("TabOpen", true, true);
                    //tabbrowser.DispatchEvent(evt);


                    //    using (var sessionStore = Xpcom.GetService2<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1"))
                    //    {
                    //        using (var result = new nsAString())
                    //        {
                    //            sessionStore.Instance.GetBrowserState(result);
                    ////            result.SetData("{\"version\":[\"sessionrestore\",1],\"windows\":[{\"tabs\":[\"entries\": [{"
                    ////                + 
                    ////"\"url\": \"https://dxr.mozilla.org/mozilla-central/search?q=SetBrowserState&redirect=true\","
                    ////+"\"title\": \"SetBrowserState - DXR Search\"," 
                    ////+"\"charset\": \"UTF-8\"," 
                    ////+"\"ID\": 100,"
                    ////+"\"docshellID\": 62,"
                    ////+"\"triggeringPrincipal_b64\": \"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAAU2h0dHBzOi8vZHhyLm1vemlsbGEub3JnL21vemlsbGEtY2VudHJhbC9zb3VyY2UvYnJvd3Nlci9iYXNlL2NvbnRlbnQvYnJvd3Nlci5qcyM4NTY0AAAAAAAAAAUAAAAIAAAADwAAAAj/////AAAACP////8AAAAIAAAADwAAABcAAAA8AAAAFwAAADcAAAAXAAAALQAAAEQAAAAHAAAATAAAAAIAAAAA/////wAAABf/////AAAATwAAAAQBAAAAAAAAAAAAAQAAAAAAAA==\","
                    ////+"\"principalToInherit_base64\": \"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAAU2h0dHBzOi8vZHhyLm1vemlsbGEub3JnL21vemlsbGEtY2VudHJhbC9zb3VyY2UvYnJvd3Nlci9iYXNlL2NvbnRlbnQvYnJvd3Nlci5qcyM4NTY0AAAAAAAAAAUAAAAIAAAADwAAAAj/////AAAACP////8AAAAIAAAADwAAABcAAAA8AAAAFwAAADcAAAAXAAAALQAAAEQAAAAHAAAATAAAAAIAAAAA/////wAAABf/////AAAATwAAAAQBAAAAAAAAAAAAAQAAAAAAAA==\","
                    ////+"\"triggeringPrincipal_base64\": \"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAAU2h0dHBzOi8vZHhyLm1vemlsbGEub3JnL21vemlsbGEtY2VudHJhbC9zb3VyY2UvYnJvd3Nlci9iYXNlL2NvbnRlbnQvYnJvd3Nlci5qcyM4NTY0AAAAAAAAAAUAAAAIAAAADwAAAAj/////AAAACP////8AAAAIAAAADwAAABcAAAA8AAAAFwAAADcAAAAXAAAALQAAAEQAAAAHAAAATAAAAAIAAAAA/////wAAABf/////AAAATwAAAAQBAAAAAAAAAAAAAQAAAAAAAA==\","
                    ////+"\"docIdentifier\": 24,"
                    ////+"\"structuredCloneState\": \"AgAAAAAA8f8AAAAACAD//wAAAAATAP//\","
                    ////+"\"structuredCloneVersion\": 8,"
                    ////+"\"persist\": true"+
                    ////                "}],\"selected\":1,\"_closedTabs\":[],\"busy\":false}],\"selectedWindow\":1,\"_closedWindows\":[],\"session\":{\"lastUpdate\":1510623725633,\"startTime\":1510624823496,\"recentCrashes\":0},\"global\":{}}"); 
                    //           // sessionStore.Instance.SetBrowserState(result);
                    //            var resString = result.ToString();
                    //        }

                    //        using (var result = new nsAString())
                    //        {
                    //            sessionStore.Instance.GetWindowState(GloableWebView.Widget.View.Instance, result);
                    //            var resString = result.ToString();
                    //        }

                    //        //sessionStore.Instance.SetTabState
                    //        //GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(sessionStore.Instance).Wrap(GeckoDOMEventTarget.Create);
                    //    }
                    break;

                default:
                    break;
            }

            //GCHandle handle1 = GCHandle.Alloc(GloableWebView.Widget.View.Instance);
            //IntPtr parameter = (IntPtr)handle1;
        }


        #endregion

        #region nsIObserver

        public void Observe([MarshalAs(UnmanagedType.Interface)] nsISupports aSubject, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string aTopic, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(WStringMarshaler))] string aData)
        {
            Console.WriteLine(aTopic);
            switch (aTopic)
            {
                case "browser-delayed-startup-finished":
                   // MaybeMigrateBookmarksForFF52();

                    //var tabbrowser = (Gecko.DOM.GeckoXULElement)GloableWebView.Widget.View.Document.GetElementById("content");
                    //var tab = (Gecko.DOM.GeckoXULElement)tabbrowser.GetProperty<nsIDOMNode>("selectedTab").Wrap(GeckoNode.Create);
                    ////tab.GetAttribute("usercontextid");
                    ////var wind = Xpcom.QueryInterface<mozIDOMWindowProxy>(GloableWebView.Widget.Instance);
                    ////nsObserverService.ObserverService.NotifyObservers(GloableWebView.Widget.View.Instance as nsISupports, "browser-window-before-show", null);

                    ////try
                    ////{
                    ////    var reloadMenuItem = GloableWebView.Widget.View.Document.GetElementById("context_reloadAllTabs");
                    ////    var mouseEvent = GloableWebView.Widget.View.Document.CreateEvent<Gecko.DOM.GeckoMouseEvent>("MouseEvent");
                    ////    try
                    ////    {
                    ////        mouseEvent.InitEvent("command", true, true);
                    ////        reloadMenuItem.DispatchEvent(mouseEvent);
                    ////    }
                    ////    catch (Exception e)
                    ////    {
                    ////        //if (e.ErrorCode != GeckoError.NS_ERROR_FAILURE)
                    ////        //    throw;
                    ////    }
                    ////    finally
                    ////    {
                    ////        Xpcom.FreeComObject(ref reloadMenuItem);
                    ////        Xpcom.FreeComObject(ref mouseEvent);
                    ////    }

                    ////    using (var sessionStore = Xpcom.GetService2<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1"))
                    ////    {
                    ////        var eventTarget = Xpcom.QueryInterface<nsIDOMEventListener>(sessionStore.Instance);

                    ////        var evnt = GloableWebView.Widget.View.Document.CreateEvent<Gecko.DOM.GeckoUIEvent>("MouseEvent");
                    ////        evnt.InitEvent("TabOpen",true,true);
                    ////        tab.DispatchEvent(evnt);
                    ////        //eventTarget.HandleEvent();
                    ////        //var ssOvserver = sessionStore.QueryInterface<nsIObserver>();
                    ////        //ssOvserver.Observe(aSubject, aTopic, aData);
                    ////    }
                    ////}
                    ////catch
                    ////{

                    ////}
                    ////GloableWebView.Navigate("about:home");
                    break;


                case "http-on-examine-response":
                    //var httpChannel = Xpcom.QueryInterface<nsIHttpChannel>(aSubject);
                    //if (httpChannel.GetResponseStatusAttribute() != 200)
                    //{
                    //    return;
                    //}

                    //// thre is no clean way to check the presence of csp header. an exception
                    //// will be thrown if it is not there.
                    //// https://developer.mozilla.org/en-US/docs/XPCOM_Interface_Reference/nsIHttpChannel
                    //try
                    //{
                    //    using (var result = new nsACString())
                    //    {
                    //        httpChannel.GetResponseHeader(new nsACString("Content-Security-Policy"), result);
                    //        //httpChannel.SetResponseHeader(new nsACString("Content-Security-Policy"), result, false);
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    //try
                    //    //{
                    //    //    // Fallback mechanism support             
                    //    //    cspRules = httpChannel.getResponseHeader("X-Content-Security-Policy");
                    //    //    mycsp = _getCspAppendingMyHostDirective(cspRules);
                    //    //    httpChannel.setResponseHeader('X-Content-Security-Policy', mycsp, false);
                    //    //}
                    //    //catch (e)
                    //    //{
                    //    //    // no csp headers defined
                    //    //    return;
                    //    //}
                    //}
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region navbar buttons
        internal void OpenIAMacros()
        {
            MaybeMigrateBookmarksForFF52();
            if (FxMAnagerListenerNotifyer != null) FxMAnagerListenerNotifyer.OnOpenIAMacros();
        }

        internal void OpenFbConverseo()
        {
             if (FxMAnagerListenerNotifyer != null) FxMAnagerListenerNotifyer.OnOpenFBConverseo();
            //BrowserWindow w = new BrowserWindow();
        }

        internal void OpenLSB()
        {
            if (FxMAnagerListenerNotifyer != null) FxMAnagerListenerNotifyer.OnOpenLSB();
        }

        internal void OpenSEO()
        {
            if (FxMAnagerListenerNotifyer != null) FxMAnagerListenerNotifyer.OnOpenSEO();
        }

        #region social stats

        internal void OpenSocialStats()
        {
            if (FxMAnagerListenerNotifyer != null)
            {
                SocialStatsCrawlerService sscs = new SocialStatsCrawlerService();
                FxMAnagerListenerNotifyer.OnOpenSocialStats(sscs);
            }
        }

        #endregion
         
        #region cpontop
        private Thread CPWthread;
        private static int lastProfileIndex = 0; //last index for profile picker        

        public async void OpenCP()
        {
            PersonData profile = await getSelectedProfile(false);
            if (profile == null) return;

            if (CPWthread == null || !CPWthread.IsAlive)
            {
                CPWthread = new Thread(() =>
                {
                    CreateProjectWindow projWindow = new CreateProjectWindow();
                    projWindow.DataContext = profile;
                    if (!MyFilesDatabase.CanSeeProxys)
                    {
                        projWindow.tbProxys.Visibility = System.Windows.Visibility.Collapsed;
                        projWindow.dpProxys.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    projWindow.projName.Text = profile.ProfileName;
                    projWindow.Topmost = true;
                    projWindow.WindowStyle = System.Windows.WindowStyle.None;
                    projWindow.AllowsTransparency = true;
                    projWindow.Opacity = 0.9;
                    projWindow.grdinfo.Opacity = 0.9;
                    projWindow.tbbutton.Text = "Close";
                    projWindow.IsReadOnly = true;
                    projWindow.cbSex.IsEnabled = projWindow.spBirth.IsEnabled = projWindow.spPBN.IsEnabled =
                    projWindow.spMoney.IsEnabled = projWindow.cmbMoney.IsEnabled = projWindow.cmbPbn.IsEnabled = false;
                    projWindow.Closed += ProjWindow_Closed;
                    projWindow.ShowDialog();
                });

                CPWthread.SetApartmentState(ApartmentState.STA);
                CPWthread.Start();
            }
        }

        public async Task<PersonData> getSelectedProfile(bool isfromMacro, string imacroName = "")
        {
            return await Task<PersonData>.Factory.StartNew(() =>
            {
                PersonData profile = ObjectCopier.DeepClone<PersonData>(GloableProfData.PData);

                if (MyFilesDatabase.HasMultipleProfiles(GloableProfData.PData.ProjectDir))
                {
                    SelectProfileWindow selectProfile = new SelectProfileWindow(GloableProfData.PData.ProjectName, GloableProfData.PData.ProjectDir, lastProfileIndex, "");
                    selectProfile.FromMacro = isfromMacro;
                    if (isfromMacro) selectProfile.Title = "For " + imacroName;
                    if (isfromMacro)
                    {
                        selectProfile.Topmost = true;
                        selectProfile.Topmost = false;
                    }
                    selectProfile.ShowDialog();
                    if (!selectProfile.OkClicked)
                    {
                        return null;
                    }
                    lastProfileIndex = selectProfile.cmProfiles.SelectedIndex;
                    profile = MyFilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
                }

                return profile;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ProjWindow_Closed(object sender, EventArgs e)
        {
            CPWthread = null;
        }
        #endregion

        #endregion

        #region context menu extras
        internal void RaiseOnCurateToPBN(string html, string uri)
        {
            ListenToFXManagerFC.OnCurateToPBN(html, uri);
        }

        internal void RaiseOnSentForSeo(string sitename, string url)
        {
            ListenToFXManagerFC.OnSentForSeo(sitename, url);
        }
        #endregion

        #region Singletone Instance
        private static BrowseoFXManager instance;
        public static BrowseoFXManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new BrowseoFXManager();
                return instance;
            }
        }
        private BrowseoFXManager()
        {
            SettingsHandler = new SettingsHandler();
            PanelUIHandler = new PanelUIHandler();
            TabbrowserHandler = new TabbrowserHandler();
            TimezoneManager = new TimezoneDataAccess();
        }
        #endregion
    }



    public class nsAppStartup : ComObject<nsIAppStartup>
    {
        private static nsIAppStartup GetAppStartUpSercive()
        {
            nsIAppStartup startup = Xpcom.GetService<Gecko.Interfaces.nsIAppStartup>(Gecko.Contracts.AppStartup);


            if (startup != null)
                return startup;

            throw Marshal.GetExceptionForHR(GeckoError.NS_ERROR_FAILURE);
        }

        public nsAppStartup() : base(GetAppStartUpSercive())
        {
        }

    }


    public class ProxyLoginPromptBypass : DefaultPromptService, nsIAuthPrompt, nsIAuthPrompt2
    {
        static bool setProxy;

        public override bool PromptAuth(nsIChannel aChannel, uint level, nsIAuthInformation authInfo)
        {
            if (!setProxy)
            {
                if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                {
                    nsString.Set(authInfo.SetUsernameAttribute, GloableProfData.PData.ProxyUsername);
                    nsString.Set(authInfo.SetPasswordAttribute, GloableProfData.PData.ProxyPassword);
                    setProxy = true;
                    //GeckoPreferences.Default.Reset();
                    return true;
                }
                else
                {
                    return base.PromptAuth(aChannel, level, authInfo);
                }
            }
            else
            {
                return base.PromptAuth(aChannel, level, authInfo);
            }
        }

        public override nsICancelable AsyncPromptAuth(nsIChannel aChannel, nsIAuthPromptCallback aCallback, nsISupports aContext, uint level, nsIAuthInformation authInfo)
        {
            if (!setProxy)
            throw new System.Runtime.InteropServices.COMException();
            else
               return base.AsyncPromptAuth(aChannel, aCallback, aContext, level, authInfo);
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
    }

        //private class Cancelable : nsICancelable
        //{
        //    public int Reason { set; get; }
        //    public void Cancel(int aReason)
        //    {
        //        Reason = aReason;
        //    }
        //}
    
}
