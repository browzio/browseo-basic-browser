using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BrowseoFX_WPF.Core.DataAccess;
using BrowseoFX_WPF.Core.Native;
using Microsoft.Win32;
using BrowseoFX_WPF.Core.Services;
using Organiser.Common.Classes;

namespace BrowseoFX_WPF.Core.BrowserHandlers
{
    public class SettingsHandler
    {
        public bool FlashEnabled = true;
        public bool GetFlashEnabledPref { get { return FXServices.PreferencesService.User.Instance.GetIntPref(Flash) == 2 ? true : false; } }
        public const string Flash = "plugin.state.flash";

        public bool JavaEnabled = true;
        public bool GetJavaEnabledPref { get { return FXServices.PreferencesService.User.Instance.GetIntPref(Java) == 1 ? true : false; } }
        public const string Java = "plugin.state.java";

        public bool PluginsEnabled = true;
        public bool GetPluginsEnabledPref { get { return FXServices.PreferencesService.User.Instance.GetBoolPref(Plugins); } }
        public const string Plugins = "plugin.scan.plid.all";

        public bool WebGLEnabled = true;
        public bool GetWebGLEnabledPref { get { return FXServices.PreferencesService.User.Instance.GetBoolPref(WebGL_FE) && !FXServices.PreferencesService.User.Instance.GetBoolPref(WebGL_D); } }
        public const string WebGL_D = "webgl.disabled";
        public const string WebGL_FE = "webgl.force-enabled";
        
        public bool WebRTCEnabled = true;
        public bool GetWebRTCEnabledPref { get { return FXServices.PreferencesService.User.Instance.GetBoolPref(WebRtc_Connection) && FXServices.PreferencesService.User.Instance.GetBoolPref(WebRtc_Servers); } }
        public const string WebRtc_Connection = "media.peerconnection.enabled";
        public const string WebRtc_Servers = "media.peerconnection.use_document_iceservers";

        public bool DoNotTrackEnabled = true;
        public bool GetDoNotTrackEnabledPref
        {
            get
            {
                return FXServices.PreferencesService.User.Instance.GetBoolPref(DNT_HeaderEnabled) &&
                 FXServices.PreferencesService.User.Instance.GetBoolPref(DNT_HeaderServices) &&
                 FXServices.PreferencesService.User.Instance.GetBoolPref(DNT_Protection) &&
                 FXServices.PreferencesService.User.Instance.GetBoolPref(DNT_HeaderEnabled) &&
                 FXServices.PreferencesService.User.Instance.GetBoolPref(DNT_ProtectionServices) &&
                 FXServices.PreferencesService.User.Instance.GetIntPref(DNT_HeaderValue) == 1;
            }
        }
        public const string DNT_HeaderEnabled = "privacy.donottrackheader.enabled";
        public const string DNT_HeaderValue = "privacy.donottrackheader.value";
        public const string DNT_HeaderServices = "services.sync.prefs.sync.privacy.donottrackheader.enabled";
        public const string DNT_Protection = "privacy.trackingprotection.enabled";
        public const string DNT_ProtectionServices = "services.sync.prefs.sync.privacy.trackingprotection.enabled";

        public bool JavascriptEnabled = true;
        public bool GetJavascriptEnabledPref { get { return FXServices.PreferencesService.User.Instance.GetBoolPref(Javascript); } }
        public const string Javascript = "javascript.enabled";
        
        public const string UserAgent = "general.useragent.override";
        //public string UserAgent_Current = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
        public string UserAgent_Current
        {
            get
            {
                return BrowserSettimgs.UserAgent_CurrentFFBuild;
            }
            set
            {
                BrowserSettimgs.UserAgent_CurrentFFBuild = value;
            }
        }
        private List<string> userAgents;
        public List<string> UserAgents
        {
            get
            {
                if (userAgents == null)
                {
                    var tempuserAgents = new List<string>();
                    var ua_generalToken = "Mozilla/5.0 ";

                    var osMajor = Environment.OSVersion.Version.Major.ToString();
                    var osMinor = Environment.OSVersion.Version.Minor.ToString();
                    var ua_platform = $"Windows NT {osMajor}.{osMinor}; ";

                    var ua_process = BrowseoFX_WPF.Core.Native.NativeMethods.InternalCheckIsWow64() ? "WOW64; " : "";
                    var ua_revision = "rv:52.0";
                    var ua_geckoVersion = "Gecko/20100101 ";
                    var ua_firefoxVersion = "Firefox/52.0";

                    if (osMajor == "6" && osMinor == "2")
                        tempuserAgents.Add(ua_generalToken + "(Windows NT 10.0; " + ua_process + ua_revision + ") " + ua_geckoVersion + ua_firefoxVersion);

                    tempuserAgents.Add(ua_generalToken + "(" + ua_platform + ua_process + ua_revision + ") " + ua_geckoVersion + ua_firefoxVersion);
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; rv:52.0) Gecko/20100101 Firefox/52.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:50.0) Gecko/20100101 Firefox/50.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:55.0) Gecko/20100101 Firefox/55.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.3; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0");
                    tempuserAgents.Add("Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:52.0) Gecko/20100101 Firefox/52.0");
                    tempuserAgents.Add("Mozilla/5.0 (Android 4.4; Mobile; rv:41.0) Gecko/41.0 Firefox/41.0");
                    tempuserAgents.Add("Mozilla/5.0 (Android 4.4; Tablet; rv:41.0) Gecko/41.0 Firefox/41.");


                    userAgents = tempuserAgents.Distinct().ToList();

                    UserAgent_Current = UserAgents[0];
                }
                return userAgents;
            }
        }

        public string SystemTimeZone_Current = TimeZoneInfo.Local.DisplayName;
        private List<string> timeZones;
        public List<string> TimeZones
        {
            get
            {
                if (timeZones == null)
                {
                    timeZones = new List<string>();
                    foreach (var item in TimeZoneInfo.GetSystemTimeZones())
                    {
                        timeZones.Add(item.DisplayName);
                    }
                }
                return timeZones;
            }
        }

        public const string UserFontSettings = "about:preferences#content";

        public string SettingsFile
        {
            get { return Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "anonym.txt"); }
        }

        public string TimeZoneSettingsFile
        {
            get
            {
                return Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "dttzinfo.txt");
            }
        }


        public async void LoadAndSetPrefenceSettings()
        {
            await Task.Run(()=> 
            {
                if (File.Exists(SettingsFile))
                {
                    var settingsFileContent = File.ReadAllLines(SettingsFile);
                    FlashEnabled = Convert.ToBoolean(settingsFileContent[0].Trim());
                    JavaEnabled = Convert.ToBoolean(settingsFileContent[1].Trim());
                    PluginsEnabled = Convert.ToBoolean(settingsFileContent[2].Trim());
                    WebGLEnabled = Convert.ToBoolean(settingsFileContent[3].Trim());
                    WebRTCEnabled = Convert.ToBoolean(settingsFileContent[4].Trim());
                    JavascriptEnabled = Convert.ToBoolean(settingsFileContent[5].Trim());
                    DoNotTrackEnabled = Convert.ToBoolean(settingsFileContent[6].Trim());
                    UserAgent_Current = settingsFileContent[7].Trim();
                }

                if (File.Exists(TimeZoneSettingsFile))
                {
                    var tzID = File.ReadAllText(SettingsFile);
                    int id = -1;
                    int.TryParse(tzID,out id);
                    if (id == -1) return;
                    BrowserSettimgs.SITimeZone = id;
                    BrowserSettimgs.SetSysDateEnabled = true;
                }
            });

            //flash
            FXServices.PreferencesService.User[Flash] = FlashEnabled ? 2 : 0;
            FXServices.PreferencesService.User["pdfjs.disabled"] = !FlashEnabled;
            FXServices.PreferencesService.User["shumway.disabled"] = !FlashEnabled;

            //Java
            FXServices.PreferencesService.User[Java] = JavaEnabled ? 1 : 0;

            //Plugins
            FXServices.PreferencesService.User[Plugins] = PluginsEnabled;

            //WebGL
            FXServices.PreferencesService.User[WebGL_D] = !WebGLEnabled;
            FXServices.PreferencesService.User[WebGL_FE] = WebGLEnabled;

            //WebRTC
            FXServices.PreferencesService.User[WebRtc_Connection] = WebRTCEnabled;
            FXServices.PreferencesService.User[WebRtc_Servers] = WebRTCEnabled;

            //javascript
            FXServices.PreferencesService.User[Javascript] = JavascriptEnabled;

            //DNT
            FXServices.PreferencesService.User[DNT_HeaderEnabled] = !DoNotTrackEnabled;
            FXServices.PreferencesService.User[DNT_HeaderValue] = DoNotTrackEnabled ? 0 : 1;
            FXServices.PreferencesService.User[DNT_HeaderServices] = !DoNotTrackEnabled;
            FXServices.PreferencesService.User[DNT_Protection] = !DoNotTrackEnabled;
            FXServices.PreferencesService.User[DNT_ProtectionServices] = !DoNotTrackEnabled;

            //UserAgent
            FXServices.PreferencesService.User[UserAgent] = UserAgent_Current;
        }

        public async void SetPrefenceSettings(bool isChecked, PanelUIListenerState setting, string label)
        {
            switch (setting)
            {
                case PanelUIListenerState.PanelUImenuitem_Flash:
                    FlashEnabled = isChecked;
                    FXServices.PreferencesService.User[Flash] = FlashEnabled ? 2 : 0;
                    break;

                case PanelUIListenerState.PanelUImenuitem_Java:
                    JavaEnabled = isChecked;
                    FXServices.PreferencesService.User[Java] = JavaEnabled ? 1 : 0;
                    break;
                case PanelUIListenerState.PanelUImenuitem_Plugins:
                    PluginsEnabled = isChecked;
                    FXServices.PreferencesService.User[Plugins] = PluginsEnabled;
                    break;
                case PanelUIListenerState.PanelUImenuitem_WebGL:
                    WebGLEnabled = isChecked;
                    FXServices.PreferencesService.User[WebGL_D] = !WebGLEnabled;
                    FXServices.PreferencesService.User[WebGL_FE] = WebGLEnabled;
                    break;
                case PanelUIListenerState.PanelUImenuitem_WebRtc:
                    WebRTCEnabled = isChecked;
                    FXServices.PreferencesService.User[WebRtc_Connection] = WebRTCEnabled;
                    FXServices.PreferencesService.User[WebRtc_Servers] = WebRTCEnabled;
                    break;
                case PanelUIListenerState.PanelUImenuitem_DNT:
                    DoNotTrackEnabled = isChecked;
                    FXServices.PreferencesService.User[DNT_HeaderEnabled] = !DoNotTrackEnabled;
                    FXServices.PreferencesService.User[DNT_HeaderValue] = DoNotTrackEnabled ? 0 : 1;
                    FXServices.PreferencesService.User[DNT_HeaderServices] = !DoNotTrackEnabled;
                    FXServices.PreferencesService.User[DNT_Protection] = !DoNotTrackEnabled;
                    FXServices.PreferencesService.User[DNT_ProtectionServices] = !DoNotTrackEnabled;
                    break;
                case PanelUIListenerState.PanelUImenuitem_Javascript:
                    JavascriptEnabled = isChecked;
                    FXServices.PreferencesService.User[Javascript] = JavascriptEnabled;
                    break;
                case PanelUIListenerState.PanelUImenuitem_Useragent:
                    UserAgent_Current = label;
                    FXServices.PreferencesService.User[UserAgent] = UserAgent_Current;
                    break;

                case PanelUIListenerState.PanelUImenuitem_TimeZone:
                    SystemTimeZone_Current = label;
                    var tzID = BrowseoFXManager.Instance.TimezoneManager.SetTimeZoneByProcess(BrowseoFXManager.Instance.SettingsHandler.SystemTimeZone_Current);
                    if (tzID == -1) return;
                    BrowserSettimgs.SITimeZone = tzID;
                    BrowserSettimgs.SetSysDateEnabled = true;


                    File.WriteAllText(TimeZoneSettingsFile, tzID.ToString());
                    break;

                default:
                    break;
            }

            await Task.Run(() => { SaveSettings(); }); 
        }

        private void SaveSettings()
        {
            string settingsLines = "" +
                FlashEnabled + Environment.NewLine +
                JavaEnabled + Environment.NewLine +
                PluginsEnabled + Environment.NewLine +
                WebGLEnabled + Environment.NewLine +
                WebRTCEnabled + Environment.NewLine +
                DoNotTrackEnabled + Environment.NewLine +
                JavascriptEnabled + Environment.NewLine +
                UserAgent_Current;
            //FXServices.PreferencesService.User[Flash] + Environment.NewLine +
            //FXServices.PreferencesService.User[Java] + Environment.NewLine +
            //FXServices.PreferencesService.User[Plugins] + Environment.NewLine +
            //FXServices.PreferencesService.User[WebGL_D] + Environment.NewLine +
            //FXServices.PreferencesService.User[WebGL_FE] + Environment.NewLine +
            //FXServices.PreferencesService.User[WebRtc_Connection] + Environment.NewLine +
            //FXServices.PreferencesService.User[WebRtc_Servers] + Environment.NewLine +
            //FXServices.PreferencesService.User[DNT_HeaderEnabled] + Environment.NewLine +
            //FXServices.PreferencesService.User[DNT_HeaderValue] + Environment.NewLine +
            //FXServices.PreferencesService.User[DNT_HeaderServices] + Environment.NewLine +
            //FXServices.PreferencesService.User[DNT_Protection] + Environment.NewLine +
            //FXServices.PreferencesService.User[DNT_ProtectionServices] + Environment.NewLine +
            //FXServices.PreferencesService.User[Javascript] + Environment.NewLine +
            //FXServices.PreferencesService.User[UserAgent];
 
            File.WriteAllText(SettingsFile, settingsLines);
        }



        //public async Task SavePreferenceSettingsAsync()
        //{
        //    string savedPrefsDir = Path.Combine(LocalFiles.GetBaseDir(), "Browsers", BrowseoFXManager.Instance.Project.ProjectName, "Firefox", "AnonimitySettings");
        //    if (!Directory.Exists(savedPrefsDir)) await Task.Run(()=> { Directory.CreateDirectory(savedPrefsDir); });

        //    string settingsText =
        //        FlashEnabled + Environment.NewLine +
        //        JavaEnabled + Environment.NewLine +
        //        PluginsEnabled + Environment.NewLine +
        //        WebGLEnabled + Environment.NewLine +
        //        WebRTCEnabled + Environment.NewLine +
        //        JavascriptEnabled + Environment.NewLine +
        //        DoNotTrackEnabled + Environment.NewLine +
        //        UserAgent_Current + Environment.NewLine +
        //        SystemTimeZone_Current + Environment.NewLine;

        //    string savedPrefsFile = Path.Combine(savedPrefsDir,"config.txt");

        //    await Task.Run(() => { File.WriteAllText(savedPrefsFile, settingsText); });
        //}

        //public async Task LoadPreferenceSettingsAsync()
        //{
        //    string savedPrefsDir = Path.Combine(LocalFiles.GetBaseDir(), "Browsers", BrowseoFXManager.Instance.Project.ProjectName, "Firefox", "AnonimitySettings");
        //    if (!Directory.Exists(savedPrefsDir)) return;

        //    string savedPrefsFile = Path.Combine(savedPrefsDir, "config.txt");
        //    if (!File.Exists(savedPrefsFile)) return;

        //    string[] prefs = await Task.Run(() => { return File.ReadAllLines(savedPrefsFile); });
        //    for (int i = 0; i < prefs.Length; i++)
        //    {
        //        if (i == 0) FlashEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 1) JavaEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 2) PluginsEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 3) WebGLEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 4) WebRTCEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 5) JavascriptEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 6) DoNotTrackEnabled = Convert.ToBoolean(prefs[i]);
        //        else if (i == 7) UserAgent_Current = prefs[i];
        //        else if (i == 8) SystemTimeZone_Current = prefs[i];
        //    }
        //}
    }
}
