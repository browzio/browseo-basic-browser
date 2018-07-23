using Gecko;
using Gecko.Interfaces;
using Gecko.Interop;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    /// <summary>
    /// Provides access to Gecko preferences.
    /// </summary>
    public sealed class nsPrefBranch : ComObject<nsIPrefBranch>
    {
        private static ComObject<nsIPrefService> _prefService;
        private static nsPrefBranch _userBranch;
        private static nsPrefBranch _defaultBranch;

        #region static members

        public static ComObject<nsIPrefService> PrefService
        {
            get { return _prefService ?? (_prefService = new ComObject<nsIPrefService>(Xpcom.GetService<nsIPrefService>(Contracts.PreferencesService))); }
        }

        /// <summary>
        /// Gets the preferences defined for the current user.
        /// </summary>
        public static nsPrefBranch User
        {
            get { return _userBranch ?? (_userBranch = new nsPrefBranch(PrefService.Instance.GetBranch(""))); }
        }

        /// <summary>
        /// Gets the set of preferences used as defaults when no user preference is set.
        /// </summary>
        public static nsPrefBranch Default
        {
            get { return _defaultBranch ?? (_defaultBranch = new nsPrefBranch(PrefService.Instance.GetDefaultBranch(""))); }
        }

        #endregion static members




        private nsPrefBranch(nsIPrefBranch prefBranch)
            : base(prefBranch)
        {

        }

        /// <summary>
        /// Reads all User preferences from the specified file.
        /// </summary>
        /// <param name="filename">Required. The name of the file from which preferences are read.  May not be null.</param>
        public static void Load(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            else if (!File.Exists(filename))
                throw new FileNotFoundException();
            using (var file = Xpcom.OpenFile(filename))
            {
                PrefService.Instance.ReadUserPrefs(file.Instance);
            }
        }

        /// <summary>
        /// Saves all User preferences to the specified file.
        /// </summary>
        /// <param name="filename">Required. The name of the file to which preferences are saved.  May not be null.</param>
        public static void Save(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");
            using (var file = Xpcom.OpenFile(filename))
            {
                PrefService.Instance.SavePrefFile(file.Instance);
            }
        }

        /// <summary>
        /// Resets all preferences to their default values.
        /// </summary>
        public void Reset()
        {
            if (this == _defaultBranch)
                PrefService.Instance.ResetPrefs();
            else
                PrefService.Instance.ResetUserPrefs();
        }

        const int PREF_INVALID = 0;
        const int PREF_STRING = 32;
        const int PREF_INT = 64;
        const int PREF_BOOL = 128;

        /// <summary>
        /// Gets or sets the preference with the given name.
        /// </summary>
        /// <param name="prefName">Required. The name of the preference to get or set.</param>
        /// <returns></returns>
        public object this[string prefName]
        {
            get
            {
                int type = Instance.GetPrefType(prefName);
                switch (type)
                {
                    case PREF_INVALID:
                        return null;
                    case PREF_STRING:
                        object comObject = null;
                        nsISupportsString aString = null;
                        Guid guid = typeof(nsISupportsString).GUID;
                        IntPtr pUnk = Instance.GetComplexValue(prefName, ref guid);
                        try
                        {
                            comObject = Marshal.GetObjectForIUnknown(pUnk);
                            aString = Xpcom.QueryInterface<nsISupportsString>(comObject);
                            return nsString.Get(aString.GetDataAttribute);
                        }
                        finally
                        {
                            Xpcom.FreeComObject(ref comObject);
                            Xpcom.FreeComObject(ref aString);
                        }
                    case PREF_INT:
                        return Instance.GetIntPref(prefName);
                    case PREF_BOOL:
                        return Instance.GetBoolPref(prefName);
                }
                throw new ArgumentException("prefName");
            }
            set
            {
                if (string.IsNullOrEmpty(prefName))
                    throw new ArgumentException("prefName");
                else if (value == null)
                    throw new ArgumentNullException("value");

                int existingType = Instance.GetPrefType(prefName);
                int assignedType = GetValueType(value);

                if (existingType != 0 && existingType != assignedType)
                {
                    throw new InvalidCastException(string.Format(
                        "A {0} value may not be assigned to '{1}' because it is already defined as {2}.",
                        value.GetType().Name,
                        prefName,
                        GetPreferenceType(prefName).Name));
                }
                switch (assignedType)
                {
                    case PREF_STRING:
                        Guid guid = typeof(nsISupportsString).GUID;
                        nsISupports aValueAsIUnknown = null;
                        var aValue = Xpcom.CreateInstance<nsISupportsString>(Contracts.SupportsString);
                        try
                        {
                            nsString.Set(aValue.SetDataAttribute, (string)value);
                            aValueAsIUnknown = Xpcom.QueryInterface<nsISupports>(aValue);
                            Instance.SetComplexValue(prefName, ref guid, aValueAsIUnknown);
                        }
                        finally
                        {
                            Xpcom.FreeComObject(ref aValueAsIUnknown);
                            Xpcom.FreeComObject(ref aValue);
                        }
                        break;
                    case PREF_INT:
                        Instance.SetIntPref(prefName, (int)value);
                        break;
                    case PREF_BOOL:
                        Instance.SetBoolPref(prefName, (bool)value);
                        break;
                }
            }
        }

        public void SetLocalizedString(string prefName, string value)
        {
            if (string.IsNullOrEmpty(prefName))
                throw new ArgumentException("prefName");
            else if (value == null)
                throw new ArgumentNullException("value");

            Guid guid = typeof(nsISupportsString).GUID;
            nsISupports aValueAsIUnknown = null;
            var aValue = Xpcom.CreateInstance<nsIPrefLocalizedString>(Contracts.PrefLocalizedString);
            try
            {
                aValue.SetDataAttribute(value);
                aValueAsIUnknown = Xpcom.QueryInterface<nsISupports>(aValue);
                Instance.SetComplexValue(prefName, ref guid, aValueAsIUnknown);
            }
            finally
            {
                Xpcom.FreeComObject(ref aValueAsIUnknown);
                Xpcom.FreeComObject(ref aValue);
            }
        }

        public string GetLocalizedString(string prefName)
        {
            if (string.IsNullOrEmpty(prefName))
                throw new ArgumentException("prefName");

            try
            {
                object comObject = null;
                nsIPrefLocalizedString aString = null;
                Guid guid = typeof(nsIPrefLocalizedString).GUID;
                IntPtr pUnk = Instance.GetComplexValue(prefName, ref guid);
                try
                {
                    comObject = Marshal.GetObjectForIUnknown(pUnk);
                    aString = Xpcom.QueryInterface<nsIPrefLocalizedString>(comObject);
                    return aString.GetDataAttribute();
                }
                finally
                {
                    Xpcom.FreeComObject(ref comObject);
                    Xpcom.FreeComObject(ref aString);
                }
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode != GeckoError.NS_ERROR_MALFORMED_URI)
                    throw;
                return this[prefName] as string;
            }
        }

        int GetValueType(object value)
        {
            if (value is int)
                return PREF_INT;
            else if (value is string)
                return PREF_STRING;
            else if (value is bool)
                return PREF_BOOL;

            throw new ArgumentException("Gecko preferences must be either a String, Int32, or Boolean value.", "prefName");
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the specified preference.
        /// </summary>
        /// <param name="name">Required. The name of the preference whose type is returned.</param>
        /// <returns></returns>
        public Type GetPreferenceType(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            switch (Instance.GetPrefType(name))
            {
                case PREF_STRING: return typeof(string);
                case PREF_INT: return typeof(int);
                case PREF_BOOL: return typeof(bool);
            }
            return null;
        }

        /// <summary>
        /// Sets whether the specified preference is locked. Locking a preference will cause the preference service to always return the default value regardless of whether there is a user set value or not.
        /// </summary>
        /// <param name="name">Required. The preference to lock or unlock.</param>
        /// <param name="locked">True if the preference should be locked; otherwise, false, and the preference is unlocked.</param>
        public void SetLocked(string name, bool locked)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (locked)
                Instance.LockPref(name);
            else
                Instance.UnlockPref(name);
        }

        /// <summary>
        /// Gets whether the specified preference is locked.
        /// </summary>
        /// <param name="name">Required. The preference whose lock status is returned.</param>
        /// <returns></returns>
        public bool GetLocked(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            return Instance.PrefIsLocked(name);
        }

        /// <summary>
        /// Clear user preferences
        /// </summary>
        /// <param name="name">Required. The preference to lock or unlock.</param>
        public void Clear(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            Instance.ClearUserPref(name);
        }
    }

    public class BFXPreferencesService 
    {
        public nsPrefBranch User
        {
            get { return nsPrefBranch.User; }
        }

        /// <summary>
        /// Gets the set of preferences used as defaults when no user preference is set.
        /// </summary>
        public nsPrefBranch Default
        {
            get { return nsPrefBranch.Default; }
        }


        public string UserPrefsFile { get { return Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefs.js");  } }
        public string UserPrefsFileTmp { get { return Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefstmp.js");  } }
        public async void SetPrefs(string user_prefs)
        {
            //if (File.Exists(UserPrefsFile))
            //{
            //    return;
            //    //var u = User;
            //    //var d = Default;
            //    using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
            //    {
            //        PreferencesService.Instance.ReadUserPrefs(FXServices.DirectoryServiceProvider.NewLocalFile(UserPrefsFile));
            //    }
            //    //nsPrefBranch.PrefService.Instance.ReadUserPrefs(FXServices.DirectoryServiceProvider.OpenFile(UserPrefsFile).Instance);
            //    return;
            //}
            //using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
            //{
            //    PreferencesService.Instance.SavePrefFile(FXServices.DirectoryServiceProvider.NewLocalFile(UserPrefsFile));
            //}

            ///speed
            //browser.cache.check_doc_frequency /* set it to 0, normally 3 */

            //network.http.pipelining                 /* default = false */
            //network.http.proxy.pipelining           /* default = false */
            //network.http.pipelining.maxrequests     /* default = 4 */dialup
            //network.http.max-connections-per-server /* default = 8 */
            //network.http.max-persistent-connections-per-server /* default = 2 */
            //network.http.max-persistent-connections-per-proxy /* default = 4 */
            //network.http.max-connections /* default = 24 */

            //content.notify.ontimer /* default = true */
            //content.interrupt.parsing /* default = true */
            //content.notify.interval /* default = 120000 (micro-seconds) */
            //content.notify.backoffcount /* default = -1, meaning never */
            //content.max.tokenizing.time /* default = 360000 (micro-seconds) */
            //content.switch.threshold /* default = 750000 (micro-seconds) */
            //content.maxtextrun /* default = 8191 */ 16385 
            //nglayout.initialpaint.delay /* set to 0, default = 250 (millisecs) */
            //browser.display.show_image_placeholders /* default = true , set to false */

            //browser.cache.memory.enable     /* default is true */
            //browser.cache.memory.capacity   /* -1 = size to fit, 123 = 123 Kb */

            //app.update.enabled             /* default is false */
            //app.update.autoUpdateEnabled   /* set to false. default = true */


            //security
            //extensions.update.enabled             /* default is false */
            //extensions.update.autoUpdateEnabled   /* set to false. default = true */

            //browser.chrome.site_icons /* set to false. default = true */
            //browser.chrome.favicons   /* set to false. default = true */

            //browser.related.autoload /* 0 = always, 1 = after first use, 2 = never */

            //capability.policy.default.SOAPCall.invoke      /* set to "NoAccess" */
            //capability.policy.default.SOAPCall.invokeAsync /* set to "NoAccess" */

            //network.cookie.cookieBehaviour         /* set to 2 (none), default = 0 (all) */

            //browser.tabs.loadInBackground            /* Set to false, default = true */
            //browser.tabs.loadBookmarksInBackground   /* false = default */


            //browser.cache.disk.parent_directory /* default = path to Cache parent */

            var pref = Default;

            var upref = User;
            //dom.mozInputMethod.enabled
            ///dom.forms.inputmode
            //pref["browser.places.smartBookmarksVersion"] = 7;

            pref["dom.mozInputMethod.enabled"] = false;
            pref["dom.sysmsg.enabled"] = false;
            pref["dom.forms.inputmode"] = true;
            pref["dom.max_script_run_time"] = 30;
            
            //nglayout.debug.disable_xul_cache

            //pref["nglayout.debug.disable_xul_cache"] = true;

            pref["app.update.enabled"] = false;
            pref["app.update.autoUpdateEnabled"] = false;
            pref["app.update.auto"] = false;
            pref["app.update.mode"] = 0;
            pref["app.update.service.enabled"] = false;

            pref["extensions.update.enabled"] = false;
            pref["extensions.update.autoUpdateEnabled"] = false;
            pref["extensions.update.autoUpdateDefault"] = false;
            pref["services.sync.prefs.sync.extensions.update.enabled"] = false;
            pref["extensions.e10sBlockedByAddons"] = true;
            pref["extensions.e10sBlocksEnabling"] = false;
            pref["intl.locale.matchOS"] = true;
            //pref["extensions.ui.locale.hidden"] = true;
            //pref["extensions.systemAddonSet"] = "{\"schema\":1,\"addons\":{}}";
            //The page’s settings blocked the loading of a resource at self (“style-src https://addons-discovery.cdn.mozilla.net 'sha256-DiZjxuHvKi7pvUQCxCVyk1kAFJEUWe+jf6HWMI5agj4='”).
            //extensions.webextensions.base-content-security-policy;script-src 'self' https://* moz-extension: blob: filesystem: 'unsafe-eval' 'unsafe-inline'; object-src 'self' https://* moz-extension: blob: filesystem:;
            //pref["extensions.webextensions.base-content-security-policy"] = "script-src 'self' https://* moz-extension: blob: filesystem: 'unsafe-eval' 'unsafe-inline'; object-src 'self' https://* moz-extension: blob: filesystem:;";
            //extensions.webextensions.default-content-security-policy;script-src 'self'; object-src 'self';
            //pref["extensions.webextensions.default-content-security-policy"] = "script-src 'self'; object-src 'self'";


            //pref["browser.chrome.site_icons"] = false;
            //pref["browser.chrome.favicons"] = false;

            pref["browser.cache.disk.parent_directory"] = "";

            //disable default browser check
            pref["browser.shell.checkDefaultBrowser"] = false;

            pref["browser.tabs.loadInBackground"] = false;
            pref["browser.tabs.loadBookmarksInBackground"] = false;
            pref["browser.tabs.closeWindowWithLastTab"] = false;

            pref["browser.rights.3.shown"] = true;

            pref["browser.startup.page"] = 1;//3=last session

            // disables the request to send performance data from displaying 
            pref["toolkit.telemetry.prompted"] = 2;
            pref["toolkit.telemetry.rejected"] = true;
            pref["toolkit.telemetry.enabled"] = false;
            pref["toolkit.crashreporter.enabled"] = false;

            pref["datareporting.healthreport.uploadEnabled"] = false;
            pref["datareporting.policy.currentPolicyVersion"] = 0;
            pref["datareporting.policy.dataSubmissionPolicyBypassNotification"] = false;
            pref["datareporting.policy.dataSubmissionPolicyAcceptedVersion"] = 0;
            //pref["datareporting.policy.dataSubmissionPolicyNotifiedTime"] = 0;
            pref["datareporting.policy.firstRunURL"] = "";
            pref["datareporting.policy.minimumPolicyVersion"] = 0;
            //pref["datareporting.sessions.current.firstPaint"] = 0;
            pref["datareporting.sessions.current.sessionRestored"] = 0;
            pref["datareporting.healthreport.service.enabled"] = false;
            pref["datareporting.policy.dataSubmissionEnabled"] = false;

            //browser.startup.blankWindow
            pref["browser.startup.blankWindow"] = true;

            ///sandbox 
            pref["security.sandbox.content.level"] = 0;
            pref["security.sandbox.gpu.level"] = 0;
            string tempdir = pref["security.sandbox.content.tempDirSuffix"] as string;
            if (tempdir == null || tempdir == "")
            {
                pref["security.sandbox.content.tempDirSuffix"] = "{" + Guid.NewGuid().ToString() + "}";
            }

            //testprefs
            pref["privacy.usercontext.about_newtab_segregation.enabled"] = false;
            pref["browser.pagethumbnails.capturing_disabled"] = true;
            pref["pageThumbs.enabled"] = false;
            //browser.tabs.showSingleWindowModePrefs true
            pref["browser.tabs.showSingleWindowModePrefs"] = true;
            //plugin.disable_full_page_plugin_for_types;application/pdf
            pref["plugin.disable_full_page_plugin_for_types"] = "application/pdf";
            //plugin.importedState;true
            pref["plugin.importedState"] = true;
            //bidi.browser.ui
            pref["bidi.browser.ui"] = false;
            //devtools.chrome.enabled
            //devtools.debugger.remote-enabled 
            pref["devtools.chrome.enabled"] = true;
            pref["devtools.debugger.remote-enabled"] = true;

            ////random
            pref["dom.keyboardevent.code.enabled"] = true;
            //browser.translation.detectLanguage
            pref["browser.translation.detectLanguage"] = false;
            //browser.slowStartup.notificationDisabled
            pref["browser.slowStartup.notificationDisabled"] = true;

            upref["network.proxy.type"] = 1;
            upref["network.proxy.share_proxy_settings"] = true;

            //upref["extensions.update.enable"] = false;


            if (!GloableProfData.PData.ProxyIP.IsNullOrEmpty())
            {
                upref["network.proxy.http"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.http_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                upref["network.proxy.ssl"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.ssl_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                upref["network.proxy.backup.ssl"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.backup.ssl_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                upref["network.proxy.ftp"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.ftp_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                upref["network.proxy.backup.ftp"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.backup.ftp_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                upref["network.proxy.socks"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.socks_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);
                upref["network.proxy.backup.socks"] = GloableProfData.PData.ProxyIP;
                upref["network.proxy.backup.socks_port"] = Convert.ToInt32(GloableProfData.PData.ProxyPort);

                if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty())
                {
                    DefaultPromptFactory.PromptGetter = () => new ProxyLoginPromptBypass();
                    //GeckoPreferences.Default["browser.xul.error_pages.enabled"] = false;

                    upref["network.proxy.login"] = GloableProfData.PData.ProxyUsername;
                    upref["network.proxy.password"] = GloableProfData.PData.ProxyPassword;
                }
            }

            //upref["extensions.hootlet.forceOpenScheduler"] = false;

            var prefnameSubstring = "user_pref(\"";
            var prefnameRemoveString = "\",";
            var prefvalueRemoveString = ");";
            var prefLines = user_prefs.SplitAndRemoveEmpty(Environment.NewLine);
            if (prefLines != null)
            {
                foreach (var line in prefLines)
                {
                    if (line.StartsWith("user_pref(\"browser.tabs.remote") ||
                        line.StartsWith("user_pref(\"extensions.xpiState") ||
                        !line.StartsWith("user_pref(\"extensions.")) continue;
                    var prefname = line.Substring(line.IndexOf(prefnameSubstring) + prefnameSubstring.Length);
                    prefname = prefname.Remove(prefname.IndexOf(prefnameRemoveString));
                    prefname = prefname.Trim();

                    var prefvalue = line.Substring(line.IndexOf(prefnameRemoveString) + prefnameRemoveString.Length);
                    prefvalue = prefvalue.Remove(prefvalue.IndexOf(prefvalueRemoveString));
                    prefvalue = prefvalue.Trim();
                    prefvalue = prefvalue.Replace("\\\\", "\\");
                    prefvalue = prefvalue.Replace("\"", "");

                    int parsed = 0;
                    if (int.TryParse(prefvalue, out parsed))
                    {
                        upref[prefname] = parsed;
                        continue;
                    }

                    if (prefvalue == "true" || prefvalue == "false")
                    {
                        upref[prefname] = bool.Parse(prefvalue);
                        continue;
                    }

                    upref[prefname] = prefvalue;
                }
            }

            pref["browser.tabs.remote.autostart"] = true;
            upref["browser.tabs.remote.autostart"] = false;
            upref["browser.tabs.remote.autostart.2"] = false;
            upref["dom.ipc.processCount"] = 0;

            //upref["extensions.imacros.defdatapath"] = "C:\\Users\\eli\\Documents\\iMacros\\Datasources";
            //upref["extensions.imacros.defdownpath"] = "C:\\Users\\eli\\Documents\\iMacros\\Downloads";
            //upref["extensions.imacros.defsavepath"] = "C:\\Users\\eli\\Documents\\iMacros\\Macros";
            //upref["extensions.imacros.just-installed"] = false;
            //pref("extensions.imacros.record-mode", "auto");
            //pref("extensions.imacros.expert-mode", false);
            //pref("extensions.imacros.id-priority", true);
            //pref("extensions.imacros.noloopwarning", true);
            //pref("extensions.imacros.clearparam", true);
            //pref("extensions.imacros.externaleditorpath", "");
            //pref("extensions.imacros.store-in-profile", false);
            //pref("extensions.imacros.maxMacroLength", 2500);
            //pref("extensions.imacros.white-list", "({'iopus.com':true, 'imacros.net':true})");
            //pref("extensions.imacros.openiMacrosShortcut", "VK_F8");
            //pref("extensions.imacros.show-tabsclose-dialog", true);
            //pref("extensions.imacros.show-pdflink-dialog", true);
            //pref("extensions.imacros.af-enabled", false);
            //pref("extensions.imacros.af-warn-runtime", true);
            //pref("extensions.imacros.af-runtime-value", 180);
            //pref("extensions.imacros.af-warn-commands", true);
            //pref("extensions.imacros.default-commands-list", "({'PROMPT':'/^\\\\s*PROMPT/i', 'PAUSE':'/^\\\\s*PAUSE/i', '/^\\\\s*SET !ENCRYPTION (?:YES|TMP|TMPKEY)/i':'/^\\\\s*SET !ENCRYPTION (?:YES|TMP|TMPKEY)/i'})");

            var panelUIfile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "paneluiconts.json");
            if (File.Exists(panelUIfile))
            {
                User["browser.uiCustomization.state"] = File.ReadAllText(panelUIfile);
            }

            if (File.Exists(ExtidsFile))
            {
                var filecontent = File.ReadAllText(ExtidsFile);

                if (!filecontent.IsNullOrEmpty())
                    User["extensions.webextensions.uuids"] = filecontent;
            }

            upref["extensions.imacros.browseoIAdefdatapath"] = FXServices.DirectoryServiceProvider.SelectedBIAProfileDatasourcePath;
            upref["extensions.imacros.maxMacroLength"] = 10000;

            string defdatapath = upref["extensions.imacros.defdatapath"] as string;
            if (defdatapath == null || defdatapath == "")
            {
                await MacroSettings.InitMacrosSettings();
                upref["extensions.imacros.defsavepath"] = MacroSettings.DefaulFoldertMacros;
                //user_pref("extensions.imacros.defdatapath", "C:\\Users\\eli\\Documents\\iMacros\\Datasources");
                //user_pref("extensions.imacros.defdownpath", "C:\\Users\\eli\\Documents\\iMacros\\Downloads");
                //user_pref("extensions.imacros.defsavepath", "C:\\Users\\eli\\Documents\\iMacros\\Macros");
                upref["extensions.imacros.defdatapath"] = MacroSettings.DefaulFoldertDataSources;
                upref["extensions.imacros.defdownpath"] = MacroSettings.DefaultFolderDownloads;
                upref["extensions.imacros.delay"] = 0;
                upref["extensions.imacros.encryptionType"] = 1;
                upref["extensions.imacros.externaleditor"] = false;
                upref["extensions.imacros.highlight"] = true;
                upref["extensions.imacros.maxwait"] = 60;
                upref["extensions.imacros.profiler-enabled"] = false;
                upref["extensions.imacros.scroll"] = true;
                upref["extensions.imacros.showjs"] = true;
                upref["extensions.imacros.use-toggle-hotkey"] = true;
                upref["extensions.imacros.version"] = "8.9.7";
            }

            //browser.places.smartBookmarksVersion
            //upref["browser.places.smartBookmarksVersion"] = "0";


            if (File.Exists(UserPrefsFile)) return;

            upref["browser.startup.homepage"] = "about:home";
            using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
            {
                PreferencesService.Instance.SavePrefFile(FXServices.DirectoryServiceProvider.NewLocalFile(UserPrefsFile));
            }

            //user_pref("sanity-test.device-id", "0x0416");
            //user_pref("sanity-test.driver-version", "20.19.15.4624");
            //user_pref("sanity-test.running", false);
            //user_pref("sanity-test.version", "20170302120751");
            //userPref["sanity-test.device-id"] = "0x0416";
            //userPref["sanity-test.driver-version"] = "20.19.15.4624";
            //userPref["sanity-test.running"] = false;
            //userPref["sanity-test.version"] = "20170302120751";

            //extensions.hootlet.forceOpenScheduler
        }

        public string ExtidsFile
        {
            get { return Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "extids.txt"); }
        }

        internal void Shutdown()
        {
            File.WriteAllText(ExtidsFile, FXServices.PreferencesService.User["extensions.webextensions.uuids"] as string);

            var panelUIplacement = User["browser.uiCustomization.state"] as string;
            if(panelUIplacement != null && panelUIplacement != "")
            {
                var panelUIfile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "paneluiconts.json");
                File.WriteAllText(panelUIfile, panelUIplacement);
            }
            nsPrefBranch.PrefService.Instance.SavePrefFile(FXServices.DirectoryServiceProvider.NewLocalFile(UserPrefsFile));

            //string prefFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefs.js");
            //string prefFileTmp = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefstmp.js");
            //if (File.Exists(prefFile))
            //{
            //    File.WriteAllText(prefFileTmp, File.ReadAllText(prefFile));
            //}
        }

     

        //public void SetPrefs()
        //{
        //    ///speed
        //    //browser.cache.check_doc_frequency /* set it to 0, normally 3 */

        //    //network.http.pipelining                 /* default = false */
        //    //network.http.proxy.pipelining           /* default = false */
        //    //network.http.pipelining.maxrequests     /* default = 4 */dialup
        //    //network.http.max-connections-per-server /* default = 8 */
        //    //network.http.max-persistent-connections-per-server /* default = 2 */
        //    //network.http.max-persistent-connections-per-proxy /* default = 4 */
        //    //network.http.max-connections /* default = 24 */

        //    //content.notify.ontimer /* default = true */
        //    //content.interrupt.parsing /* default = true */
        //    //content.notify.interval /* default = 120000 (micro-seconds) */
        //    //content.notify.backoffcount /* default = -1, meaning never */
        //    //content.max.tokenizing.time /* default = 360000 (micro-seconds) */
        //    //content.switch.threshold /* default = 750000 (micro-seconds) */
        //    //content.maxtextrun /* default = 8191 */ 16385 
        //    //nglayout.initialpaint.delay /* set to 0, default = 250 (millisecs) */
        //    //browser.display.show_image_placeholders /* default = true , set to false */

        //    //browser.cache.memory.enable     /* default is true */
        //    //browser.cache.memory.capacity   /* -1 = size to fit, 123 = 123 Kb */

        //    //app.update.enabled             /* default is false */
        //    //app.update.autoUpdateEnabled   /* set to false. default = true */


        //    //security
        //    //extensions.update.enabled             /* default is false */
        //    //extensions.update.autoUpdateEnabled   /* set to false. default = true */

        //    //browser.chrome.site_icons /* set to false. default = true */
        //    //browser.chrome.favicons   /* set to false. default = true */

        //    //browser.related.autoload /* 0 = always, 1 = after first use, 2 = never */

        //    //capability.policy.default.SOAPCall.invoke      /* set to "NoAccess" */
        //    //capability.policy.default.SOAPCall.invokeAsync /* set to "NoAccess" */

        //    //network.cookie.cookieBehaviour         /* set to 2 (none), default = 0 (all) */

        //    //browser.tabs.loadInBackground            /* Set to false, default = true */
        //    //browser.tabs.loadBookmarksInBackground   /* false = default */


        //    //browser.cache.disk.parent_directory /* default = path to Cache parent */

        //    //browser.tabs.showSingleWindowModePrefs true
















        //    //using (var PreferencesService = Xpcom.GetService2<Gecko.Interfaces.nsIPrefService>(Gecko.Contracts.PreferencesService))
        //    //{
        //    //    //var userPrefsFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefs.js");

        //    //    ////if (File.Exists(userPrefsFile))
        //    //    ////    PreferencesService.Instance.ReadUserPrefs(BrowseoFXServicesManager.DirectoryServiceProvider.NewLocalFile(userPrefsFile));
        //    //    ////else
        //    //    //    PreferencesService.Instance.SavePrefFile(FXServices.DirectoryServiceProvider.NewLocalFile(userPrefsFile));
        //    //}
        //    var userPrefsFile = Path.Combine(FXServices.DirectoryServiceProvider.SelectedBFXProfileCachPath, "prefs.js");
        //    var file = FXServices.DirectoryServiceProvider.NewLocalFile(userPrefsFile);
        //    try
        //    {
        //        if (!File.Exists(userPrefsFile))
        //        {
        //            nsPrefBranch.PrefService.Instance.SavePrefFile(file);
        //        }
        //        else
        //        {
        //           // nsPrefBranch.PrefService.Instance.ReadUserPrefs(file);
        //        }
        //    }
        //    finally
        //    {
        //        Xpcom.FreeComObject(ref file);
        //    }

        //    var pref = Default;
        //    pref["browser.startup.page"] = 1;//3=last session
        //    pref["browser.tabs.closeWindowWithLastTab"] = false;
        //    pref["toolkit.telemetry.enabled"] = false;
        //    //pref["toolkit.defaultChromeURI"] = "";
        //    //pref["toolkit.defaultChromeFeatures"] = "";
        //    //pref["toolkit.singletonWindowType"] = "";
        //    ////pref["toolkit.defaultChromeURI"] = "chrome://browser/content/browser.xul";
        //    ////pref["toolkit.defaultChromeFeatures"] = "chrome,dialog=no,all";
        //    //pref["toolkit.singletonWindowType"] = "navigator:browser";

        //    pref["browser.tabs.remote.autostart"] = false;
        //    pref["extensions.e10sBlockedByAddons"] = true;
        //    pref["extensions.e10sBlocksEnabling"] = false;

        //    //https://stackoverflow.com/questions/3729477/how-to-automatically-import-bookmarks-in-firefox
        //    //pref["browser.places.importBookmarksHTML"] = true;
        //    //pref["browser.bookmarks.restore_default_bookmarks"] = false;
        //    //pref["browser.shell.checkDefaultBrowser"] = false;
        //    //pref["browser.bookmarks.autoExportHTML"] = true;
        //    //pref["browser.bookmarks.file"] = "Bookmarks.html";

        //    //https://developer.mozilla.org/en-US/Firefox/Enterprise_deployment
        //    pref["app.update.enabled"] = false;
        //    pref["app.update.auto"] = false;
        //    pref["app.update.mode"] = 0;
        //    pref["app.update.service.enabled"] = false;
        //    pref["browser.rights.3.shown"] = true;
        //    // Don't show Whats New on first run after every update
        //    //pref["browser.startup.homepage_override.mstone"] = "ignore";
        //    //// pref["pdfjs.disabled"] = true;
        //    //pref["pdfjs.disabled"] = false;
        //    //// pref["shumway.disabled"] = true;
        //    //pref["shumway.disabled"] = false;
        //    //pref["plugins.notifyMissingFlash"] = false;
        //    //pref["plugins.hide_infobar_for_outdated_plugin"] = true;
        //    pref["datareporting.healthreport.service.enabled"] = false;
        //    pref["datareporting.policy.dataSubmissionEnabled"] = false;
        //    pref["toolkit.crashreporter.enabled"] = false;


        //    //http://firefoxcrash.blogspot.co.il/2016/03/firefox-setting-for-all-users.html
        //    //sets Google as the default search engine instead of Yahoo
        //    pref["browser.search.geoSpecificDefaults"] = false;
        //    pref["browser.search.defaultenginename"] = "Google";

        //    // set FireFox Default homepage 
        //    pref["browser.startup.homepage"] = "about:home";

        //    // disables the 'know your rights' button from displaying on first run
        //    pref["browser.usedOnWindows10"] = true;

        //    //disable default browser check
        //    pref["browser.shell.checkDefaultBrowser"] = false;

        //    // set all plugins to always activated
        //    // pref["plugin.default.state"]= 2;

        //    // hide choose what i share that pops up at the bottom of the browser after the first minute
        //    //pref.SetLocked("datareporting.policy.dataSubmissionPolicyBypassNotification", true);

        //    // disables the request to send performance data from displaying 
        //    pref["toolkit.telemetry.prompted"] = 2;
        //    pref["toolkit.telemetry.rejected"] = true;

        //    // do not block popups - can only be default or locked pref
        //    //pref["dom.disable_open_during_load"]= false;

        //    // prevent reader view from popping down
        //    pref["reader.parse-on-load.enabled"] = true;



        //    //distribution.testing.loadFromProfile extensions.enabledScopes
        //    //pref["distribution.testing.loadFromProfile"] = false;
        //    //pref["extensions.enabledScopes"] = 15;
        //    //pref["extensions.autoDisableScopes"] = 0;


        //    // The next 3 lines shows you how to clear all browsing data/history upon close
        //    //lockpref("privacy.sanitize.migrateFx3Prefs", true);
        //    //lockpref("privacy.sanitize.promptOnSanitize", false);
        //    //lockpref("privacy.sanitize.sanitizeOnShutdown", true);
        //    // http://www.pcc-services.com/kixtart/firefox-lockdown.html
        //    // This last line allows your browser to operate exclusively in Private Browsing Mode
        //    //pref("browser.privatebrowsing.autostart", true);


        //    pref["Ui.key.generalAccessKey"] = -1;
        //    pref["Ui.key.chromeAccess"] = 4;
        //    pref["Ui.key.contentAccess"] = 2;



        //    var userPref = User;
        //    userPref["datareporting.healthreport.uploadEnabled"] = false;
        //    userPref["datareporting.policy.currentPolicyVersion"] = 0;
        //    userPref["datareporting.policy.dataSubmissionEnabled"] = false;
        //    userPref["datareporting.policy.dataSubmissionPolicyBypassNotification"] = false;
        //    userPref["datareporting.policy.dataSubmissionPolicyAcceptedVersion"] = 0;
        //    //userPref["datareporting.policy.dataSubmissionPolicyNotifiedTime"] = 0;
        //    userPref["datareporting.policy.firstRunURL"] = "";
        //    userPref["datareporting.policy.minimumPolicyVersion"] = 0;
        //    userPref["datareporting.sessions.current.firstPaint"] = 0;
        //    userPref["datareporting.sessions.current.sessionRestored"] = 0;

        //    //random
        //    userPref["dom.keyboardevent.code.enabled"] = true;

        //    ///sandbox bullshit
        //    userPref["security.sandbox.content.level"] = 0;
        //    userPref["security.sandbox.gpu.level"] = 0;
        //    string tempdir = userPref["security.sandbox.content.tempDirSuffix"] as string;
        //    if (tempdir == null || tempdir == "")
        //    {
        //        userPref["security.sandbox.content.tempDirSuffix"] = Guid.NewGuid().ToString();
        //    }

        //    //testprefs
        //    userPref["privacy.usercontext.about_newtab_segregation.enabled"] = false;
        //    userPref["browser.pagethumbnails.capturing_disabled"] = true;
        //    userPref["pageThumbs.enabled"] = false;

        //}
    }
}
