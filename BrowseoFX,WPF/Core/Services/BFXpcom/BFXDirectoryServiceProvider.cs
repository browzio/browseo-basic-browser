using System;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Gecko.Interfaces;
using Gecko.CustomMarshalers;
using System.Collections.Generic;
using System.Text;
using Gecko;
using BrowseoFX_WPF.Core.DataAccess;
using BrowseoFX_WPF.Models;
using System.ComponentModel;
using Gecko.Interop;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    /// <summary>
    ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox
    /// [General]
    /// StartWithLastProfile=1
    /// 
    /// [Profile0]
    /// Name=default
    /// IsRelative=1
    /// Path=Profiles/3kw8mj5i.default
    /// Default=1
    /// </summary>
    //public class BrowserProfilesManagerService
    //{

    //}

   
    // Appends the distribution-specific search engine directories to the
    // array.  The directory structure is as follows:

    // appdir/
    // \- distribution/
    //    \- searchplugins/
    //       |- common/
    //       \- locale/
    //          |- <locale 1>/
    //          ...
    //          \- <locale N>/

    // common engines are loaded for all locales.  If there is no locale
    // directory for the current locale, there is a pref:
    // "distribution.searchplugins.defaultLocale"
    // which specifies a default locale to use.
    //internal class DirectoryProvider : nsIDirectoryServiceProvider, nsIDirectoryServiceProvider2
    //{
    //    const string NS_APP_DISTRIBUTION_SEARCH_DIR_LIST = "SrchPluginsDistDL";
    //    const string NS_DIRECTORY_SERVICE_CONTRACTID = "@mozilla.org/file/directory_service;1";

    //    const string XRE_APP_DISTRIBUTION_DIR = "XREAppDist";

    //    public IntPtr GetFile([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string prop, [MarshalAs(UnmanagedType.U1)] out bool persistent)
    //    {
    //        persistent = false;
    //        return IntPtr.Zero;
    //    }

    //    [return: MarshalAs(UnmanagedType.Interface)]
    //    public nsISimpleEnumerator GetFiles([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string prop)
    //    {
    //        if (prop == NS_APP_DISTRIBUTION_SEARCH_DIR_LIST) return null;
    //        else
    //        {
    //            using(var dirSvc = Xpcom.GetService2<nsIProperties>(NS_DIRECTORY_SERVICE_CONTRACTID))
    //            {
    //                if (dirSvc == null) return null;
    //                else
    //                {
    //                    List<nsIFile> distroFiles;

    //                    Guid nsiFileGuid = new Guid("2fa6884a-ae65-412a-9d4c-ce6e34544ba1");
    //                    dirSvc.Instance.Get(XRE_APP_DISTRIBUTION_DIR, ref nsiFileGuid);

    //                }
    //            }
    //        }
    //    }
    //}
    /// <summary>
    /// firefox.exe -no-remote -CreateProfile "JoelUser"
    /// A simple nsIDirectoryServiceProvider which provides the profile directory, etc.
    /// This is still an incomplete implementation and can cause issues -- if this happens, add the missing item.
    /// For lists of items in DirectoryService, see
    /// https://developer.mozilla.org/en-US/Add-ons/Code_snippets/File_I_O#Getting_files_in_special_directories
    /// </summary>
    public class BFXDirectoryServiceProvider : nsIDirectoryServiceProvider,
        nsIProperties
    {
        #region directory paths
        ////C:\Users\eli\AppData\Roaming\Mozilla\
        //public string MozillaBasePath
        //{
        //    get
        //    {
        //        return CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla"));
        //    }
        //}

        ////C:\Users\eli\AppData\Roaming\Mozilla\Firefox
        //public string MozillaFirefoxBasePath
        //{
        //    get
        //    {
        //        return CreateDirectory(Path.Combine(MozillaBasePath, "Firefox"));
        //    }
        //}

        ////C:\Users\eli\AppData\Roaming\Mozilla\Firefox\profiles.ini
        //public string MozillaProfilesFile
        //{
        //    get
        //    {
        //        return Path.Combine(MozillaFirefoxBasePath, "profiles.ini");
        //    }
        //}

        string _projectName;

        ////roaming/Browseo/BrowseoFX
        public string BrowseoFXBaseBrowserDir
        {
            get
            {
                return CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Browseo", "BrowseoFX"));
            }
        }

        public string BrowseoFXSysExtentionsxtSDir
        {
            get
            {
                return CreateDirectory(Path.Combine(BrowseoFXBaseBrowserDir, "SExtensions"));
            }
        }
        public string BrowseoFXSysExtentionsxtLDir
        {
            get
            {
                return CreateDirectory(Path.Combine(BrowseoFXBaseBrowserDir, "LExtensions"));
            }
        }

        public string BrowseoFXProfilesFile
        {
            get
            {
                return Path.Combine(BrowseoFXBaseBrowserDir, "profiles.ini");
            }
        }

        ////roaming/Browseo/BrowseoFX
        public string BrowseoFXBaseBrowserProfilesDir
        {
            get
            {
                return CreateDirectory(Path.Combine(BrowseoFXBaseBrowserDir, "Profiles"));
            }
        }
        ////roaming/Browseo/BrowseoFX/SelectedProfileName
        public string SelectedBFXProfileCachPath
        {
            get
            {
                return CreateDirectory(Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "CachesFF", _projectName));
            }
        }

        public string SelectedBIAProfileDatasourcePath
        {
            get
            {
                return CreateDirectory(Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "BrowseoIA_DataSource", _projectName));
            }
        }

        ////roaming/Browseo/BrowseoFX/SelectedProfileName
        //public string SelectedBFXProfileCachPath
        //{
        //    get
        //    {
        //        return CreateDirectory(Path.Combine(BrowseoFXBaseBrowserDir, "Profiles", SelectedProfile.ProfileName));
        //    }
        //}

        ////roaming/Browseo/BrowseoFX/SelectedProfileName
        public string SelectedBFXProfileRoamingPath
        {
            get
            {
                return CreateDirectory(Path.Combine(BrowseoFXBaseBrowserDir, "Roaming", SelectedProfile.ProfileName));
            }
        }

        public string SelectedBFXProfileLocalPath
        {
            get
            {
                return CreateDirectory(Path.Combine(BrowseoFXBaseBrowserDir, "Local", SelectedProfile.ProfileName));
            }
        }

        //SDKDirectory/bin
        public string GeckoPath
        {
            get
            {
                return Xpcom.SDKDirectory;
            }
        }
        #endregion

        public List<BrowserProfile> Profiles { get; set; }
        public BrowserProfile SelectedProfile { get; set; }

        public BFXDirectoryServiceProvider()
        {
            Profiles = new List<BrowserProfile>();
        }

        #region profile and dir init
        public void ReadProfilesIni()
        {
            if (!File.Exists(BrowseoFXProfilesFile)) return;

            Profiles.Clear();
            var profilesFileData = new IniFile(BrowseoFXProfilesFile);
            foreach (var section in profilesFileData.GetSectionNames())
            {
                if (section == "General")
                {
                    var StartWithLastProfile = profilesFileData.IniReadValue(section, "StartWithLastProfile");
                }
                else if (section.Contains("Profile"))
                {
                    Profiles.Add(new BrowserProfile()
                    {
                        SectionName = section,
                        ProfileName = profilesFileData.IniReadValue(section, "Name"),
                        IsRelative = profilesFileData.IniReadValue(section, "IsRelative"),
                        Path = profilesFileData.IniReadValue(section, "Path"),
                        Default = profilesFileData.IniReadValue(section, "Default"),
                    });
                }
            }
        }

        /// <summary>
        /// firefox.exe -no-remote -CreateProfile "projectName"
        /// </summary>
        public void SelectOrAddProfile(string projectName)
        {
            ReadProfilesIni();

            //var cleanName = projectName.Replace(" ", "-");

            SelectedProfile = Profiles.FirstOrDefault(p => p.ProfileName == projectName);

            if (SelectedProfile != null) return;
            else
            {
                var profilesFileData = new IniFile(BrowseoFXProfilesFile);
                profilesFileData.IniWriteValue("Profile" + Profiles.Count, "Name", projectName);
                profilesFileData.IniWriteValue("Profile" + Profiles.Count, "IsRelative", "0");
                profilesFileData.IniWriteValue("Profile" + Profiles.Count, "Path", SelectedBFXProfileCachPath);

                //firefox -CreateProfile "JoelUser c:\internet\joelusers-moz-profile"
                //var ffexePath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";//Path.Combine(GeckoPath, "firefox.exe"),
                //var createProfileProcess = new Process()
                //{
                //    StartInfo = new ProcessStartInfo()
                //    {
                //        Arguments = "-no-remote -CreateProfile \"" + cleanName + " " + Path.Combine(BrowseoFXBaseBrowserProfilesDir, cleanName) + "\"",
                //        CreateNoWindow = true,
                //        UseShellExecute = false,
                //        FileName = ffexePath,
                //    },
                //};
                //createProfileProcess.Start();
                //createProfileProcess.WaitForExit(10000);
            }
        }

        //class createProfileSync : ISynchronizeInvoke
        //{
        //    public bool InvokeRequired
        //    {
        //        get
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }

        //    public IAsyncResult BeginInvoke(Delegate method, object[] args)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public object EndInvoke(IAsyncResult result)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public object Invoke(Delegate method, object[] args)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        internal bool InitWithProjectName(string projectName)
        {
            _projectName = projectName;
            SelectOrAddProfile(projectName);
            ReadProfilesIni();
            SelectedProfile = Profiles.FirstOrDefault(p => p.ProfileName == projectName);
            if(SelectedProfile != null)
            {
                CreateCacheFiles();
                return true;
            }

            return false;
        }

        internal void CreateCacheFiles()
        {
            //times.json
            //{
            //  "created": 1513035917344
            //}
            var timesFile = Path.Combine(SelectedBFXProfileCachPath, "times.json");
            if (!File.Exists(timesFile))
            {
                var fileText = "{\"created\":"+DateTime.Now.ToUniversalTime().Millisecond+" }";
                File.WriteAllText(timesFile, fileText);
            }

            ////addons.json {"schema":5,"addons":[]}
            //var addonsFile = Path.Combine(SelectedBFXProfileCachPath, "addons.json");
            //if (!File.Exists(addonsFile))
            //{
            //    var fileText = "{\"schema\":5,\"addons\":[]}";
            //    File.WriteAllText(addonsFile, fileText);
            //}

            ////AlternateServices.txt
            //var alternateServicesFile = Path.Combine(SelectedBFXProfileCachPath, "AlternateServices.txt");
            //if (!File.Exists(alternateServicesFile)) File.Create(alternateServicesFile);

            ////compatibility.ini
            ////[Compatibility]
            ////LastVersion=52.0_20170302120751/20170302120751
            ////LastOSABI=WINNT_x86-msvc
            ////LastPlatformDir=C:\Program Files (x86)\Mozilla Firefox
            ////LastAppDir=C:\Program Files (x86)\Mozilla Firefox\browser
            ////InvalidateCaches=1
            //var compatibilityFile = Path.Combine(SelectedBFXProfileCachPath, "compatibility.ini");
            //if (!File.Exists(compatibilityFile))
            //{
            //    var fileText =
            //        "[Compatibility]" + Environment.NewLine +
            //        "LastVersion=52.0_20170302120751/20170302120751" + Environment.NewLine +
            //        "LastOSABI=WINNT_x86-msvc" + Environment.NewLine +
            //        "LastPlatformDir=C:\\Program Files (x86)\\Mozilla Firefox" + Environment.NewLine +
            //        "LastAppDir=C:\\Program Files (x86)\\Mozilla Firefox\\browser" + Environment.NewLine +
            //        "InvalidateCaches=1";
            //    File.WriteAllText(compatibilityFile, fileText);
            //}
            
            ////extensions.ini
            //var extensionsFile = Path.Combine(SelectedBFXProfileCachPath, "extensions.ini");
            //if (!File.Exists(extensionsFile))
            //{
            //    var fileText =
            //        "[ExtensionDirs]" + Environment.NewLine +
            //        "[ThemeDirs]" + Environment.NewLine +
            //        "Extension0="+Path.Combine(Xpcom.SDKDirectory, "browser", "extensions", "{972ce4c6-7e08-4474-a285-3208198ce6fd}.xpi") + Environment.NewLine +
            //        "[MultiprocessIncompatibleExtensions]";
            //    File.WriteAllText(extensionsFile, fileText);
            //}

            ////mimeTypes.rdf
            //var mimeTypesFile = Path.Combine(SelectedBFXProfileCachPath, "mimeTypes.rdf");
            //if (!File.Exists(mimeTypesFile)) File.Copy(Path.Combine(Xpcom.BFXComponentsDirectory, "fxfiles", "mimeTypes.rdf"),mimeTypesFile);

            ////SecurityPreloadState.txt
            //var SecurityPreloadStateFile = Path.Combine(SelectedBFXProfileCachPath, "SecurityPreloadState.txt");
            //if (!File.Exists(SecurityPreloadStateFile)) File.Create(SecurityPreloadStateFile);





            //directoryLinks.json
            //{"directory": [{"bgColor": "", "directoryId": 8174, "enhancedImageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/b18c52ee5445fb37ed466d695f7e018efd0ad58f.95655.png", "imageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/b18c52ee5445fb37ed466d695f7e018efd0ad58f.95655.png", "title": "Mozilla Community", "type": "affiliate", "url": "http://contribute.mozilla.org/"}, {"bgColor": "", "directoryId": 8158, "enhancedImageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/12abda9be001265d0721947e520149fb781562b8.10301.png", "imageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/12abda9be001265d0721947e520149fb781562b8.10301.png", "title": "Mozilla Developer Network", "type": "affiliate", "url": "https://developer.mozilla.org/en-US/?utm_source=mozilla&utm_medium=firefox-tile&utm_campaign=default"}, {"bgColor": "", "directoryId": 8144, "enhancedImageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/0152ecdf5b18e095d255486b18c3f6a04ba6300e.27260.png", "imageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/0152ecdf5b18e095d255486b18c3f6a04ba6300e.27260.png", "title": "Mozilla Webmaker", "type": "affiliate", "url": "https://webmaker.org/?utm_source=directory-tiles&utm_medium=firefox-browser"}, {"bgColor": "", "directoryId": 8137, "enhancedImageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/af34c0541c6cdff392ec4414cb5949fa7012271d.17141.png", "imageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/af34c0541c6cdff392ec4414cb5949fa7012271d.17141.png", "title": "Firefox Sync", "type": "affiliate", "url": "http://mozilla-europe.org/firefox/sync"}, {"bgColor": "", "directoryId": 1946, "enhancedImageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/cc63774b7a9aae02fe36bc5caf90c1e25e66a2bc.13791.png", "imageURI": "https://tiles-cloudfront.cdn.mozilla.net/images/e822cd4628c5162313f49f5d4556f8aafdf38750.11513.png", "title": "Mozilla Manifesto", "type": "affiliate", "url": "https://www.mozilla.org/about/manifesto/"}], "suggested": []}

            //frequencyCap.json
            //{}

            //pluginreg.dat
            //look at C:\Users\eli\Desktop\temp\fxcmdPTest\pluginreg.dat

            //sessionCheckpoints.json
            //{
            //    "profile-after-change": true,
            //    "final-ui-startup": true,
            //    "sessionstore-windows-restored": true,
            //    "quit-application-granted": true,
            //    "quit-application": true,
            //    "sessionstore-final-state-write-complete": true,
            //    "profile-change-net-teardown": true,
            //    "profile-change-teardown": true,
            //    "profile-before-change": true
            //}

            //sessionstore.js
            //{"version":["sessionrestore",1],"windows":[{"tabs":[{"entries":[{"url":"https://www.mozilla.org/en-US/firefox/52.0/firstrun/","title":"Welcome to Firefox","charset":"UTF-8","ID":2,"docshellID":2,"originalURI":"https://www.mozilla.org/en-US/firefox/52.0/firstrun/","triggeringPrincipal_base64":"SmIS26zLEdO3ZQBgsLbOywAAAAAAAAAAwAAAAAAAAEY=","docIdentifier":2,"children":[{"url":"https://accounts.firefox.com/signin?utm_campaign=fxa-embedded-form&utm_medium=referral&utm_source=firstrun&utm_content=fx-52.0&entrypoint=firstrun&service=sync&context=fx_firstrun_v2&style=chromeless&haltAfterSignIn=true&origin=https%3A%2F%2Fwww.mozilla.org","title":"Sign in to continue to Firefox Sync","charset":"UTF-8","ID":3,"docshellID":4,"referrer":"https://www.mozilla.org/en-US/firefox/52.0/firstrun/","referrerPolicy":0,"originalURI":"https://accounts.firefox.com/signin?utm_campaign=fxa-embedded-form&utm_medium=referral&utm_source=firstrun&utm_content=fx-52.0&entrypoint=firstrun&service=sync&context=fx_firstrun_v2&style=chromeless&haltAfterSignIn=true&origin=https%3A%2F%2Fwww.mozilla.org","triggeringPrincipal_b64":"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAAABCdntGuXUQAS/4CfOuSPZrLPEwK69Xkyth+CNIQ27P58B3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAQAABocAcwBjAHIAaQBwAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgACcAdQBuAHMAYQBmAGUALQBpAG4AbABpAG4AZQAnACAAJwB1AG4AcwBhAGYAZQAtAGUAdgBhAGwAJwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBzADMALgBhAG0AYQB6AG8AbgBhAHcAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBnAG8AbwBnAGwAZQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AeQBvAHUAdAB1AGIAZQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHMALgB5AHQAaQBtAGcALgBjAG8AbQA7ACAAaQBtAGcALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgAGQAYQB0AGEAOgAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvACoALgB0AGkAbABlAHMALgBtAGEAcABiAG8AeAAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAcABpAC4AbQBhAHAAYgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBjAHIAZQBhAHQAaQB2AGUAYwBvAG0AbQBvAG4AcwAuAG8AcgBnACAAaAB0AHQAcABzADoALwAvAGEAZAAuAGQAbwB1AGIAbABlAGMAbABpAGMAawAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvAHMAcAAuAGEAbgBhAGwAeQB0AGkAYwBzAC4AeQBhAGgAbwBvAC4AYwBvAG0AOwAgAGQAZQBmAGEAdQBsAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQA7ACAAZgByAGEAbQBlAC0AcwByAGMAIABoAHQAdABwAHMAOgAvAC8AKgAuAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAC0AYQBuAGEAbAB5AHQAaQBjAHMALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHkAbwB1AHQAdQBiAGUALQBuAG8AYwBvAG8AawBpAGUALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB0AHIAYQBjAGsAZQByAHQAZQBzAHQALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHMAdQByAHYAZQB5AGcAaQB6AG0AbwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAYwBjAG8AdQBuAHQAcwAuAGYAaQByAGUAZgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBhAGMAYwBvAHUAbgB0AHMALgBmAGkAcgBlAGYAbwB4AC4AYwBvAG0ALgBjAG4AIABoAHQAdABwAHMAOgAvAC8AdwB3AHcALgB5AG8AdQB0AHUAYgBlAC4AYwBvAG0AOwAgAHMAdAB5AGwAZQAtAHMAcgBjACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AbQBvAHoAaQBsAGwAYQAuAG8AcgBnACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbgBlAHQAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAGMAbwBtACAAJwB1AG4AcwBhAGYAZQAtAGkAbgBsAGkAbgBlACcAOwAgAGMAbwBuAG4AZQBjAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvACoALgB0AGkAbABlAHMALgBtAGEAcABiAG8AeAAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAcABpAC4AbQBhAHAAYgBvAHgALgBjAG8AbQA7ACAAYwBoAGkAbABkAC0AcwByAGMAIABoAHQAdABwAHMAOgAvAC8AKgAuAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAC0AYQBuAGEAbAB5AHQAaQBjAHMALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHkAbwB1AHQAdQBiAGUALQBuAG8AYwBvAG8AawBpAGUALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB0AHIAYQBjAGsAZQByAHQAZQBzAHQALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHMAdQByAHYAZQB5AGcAaQB6AG0AbwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAYwBjAG8AdQBuAHQAcwAuAGYAaQByAGUAZgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBhAGMAYwBvAHUAbgB0AHMALgBmAGkAcgBlAGYAbwB4AC4AYwBvAG0ALgBjAG4AIABoAHQAdABwAHMAOgAvAC8AdwB3AHcALgB5AG8AdQB0AHUAYgBlAC4AYwBvAG0A","principalToInherit_base64":"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAAABCdntGuXUQAS/4CfOuSPZrLPEwK69Xkyth+CNIQ27P58B3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAQAABocAcwBjAHIAaQBwAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgACcAdQBuAHMAYQBmAGUALQBpAG4AbABpAG4AZQAnACAAJwB1AG4AcwBhAGYAZQAtAGUAdgBhAGwAJwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBzADMALgBhAG0AYQB6AG8AbgBhAHcAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBnAG8AbwBnAGwAZQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AeQBvAHUAdAB1AGIAZQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHMALgB5AHQAaQBtAGcALgBjAG8AbQA7ACAAaQBtAGcALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgAGQAYQB0AGEAOgAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvACoALgB0AGkAbABlAHMALgBtAGEAcABiAG8AeAAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAcABpAC4AbQBhAHAAYgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBjAHIAZQBhAHQAaQB2AGUAYwBvAG0AbQBvAG4AcwAuAG8AcgBnACAAaAB0AHQAcABzADoALwAvAGEAZAAuAGQAbwB1AGIAbABlAGMAbABpAGMAawAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvAHMAcAAuAGEAbgBhAGwAeQB0AGkAYwBzAC4AeQBhAGgAbwBvAC4AYwBvAG0AOwAgAGQAZQBmAGEAdQBsAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQA7ACAAZgByAGEAbQBlAC0AcwByAGMAIABoAHQAdABwAHMAOgAvAC8AKgAuAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAC0AYQBuAGEAbAB5AHQAaQBjAHMALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHkAbwB1AHQAdQBiAGUALQBuAG8AYwBvAG8AawBpAGUALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB0AHIAYQBjAGsAZQByAHQAZQBzAHQALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHMAdQByAHYAZQB5AGcAaQB6AG0AbwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAYwBjAG8AdQBuAHQAcwAuAGYAaQByAGUAZgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBhAGMAYwBvAHUAbgB0AHMALgBmAGkAcgBlAGYAbwB4AC4AYwBvAG0ALgBjAG4AIABoAHQAdABwAHMAOgAvAC8AdwB3AHcALgB5AG8AdQB0AHUAYgBlAC4AYwBvAG0AOwAgAHMAdAB5AGwAZQAtAHMAcgBjACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AbQBvAHoAaQBsAGwAYQAuAG8AcgBnACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbgBlAHQAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAGMAbwBtACAAJwB1AG4AcwBhAGYAZQAtAGkAbgBsAGkAbgBlACcAOwAgAGMAbwBuAG4AZQBjAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvACoALgB0AGkAbABlAHMALgBtAGEAcABiAG8AeAAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAcABpAC4AbQBhAHAAYgBvAHgALgBjAG8AbQA7ACAAYwBoAGkAbABkAC0AcwByAGMAIABoAHQAdABwAHMAOgAvAC8AKgAuAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAC0AYQBuAGEAbAB5AHQAaQBjAHMALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHkAbwB1AHQAdQBiAGUALQBuAG8AYwBvAG8AawBpAGUALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB0AHIAYQBjAGsAZQByAHQAZQBzAHQALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHMAdQByAHYAZQB5AGcAaQB6AG0AbwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAYwBjAG8AdQBuAHQAcwAuAGYAaQByAGUAZgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBhAGMAYwBvAHUAbgB0AHMALgBmAGkAcgBlAGYAbwB4AC4AYwBvAG0ALgBjAG4AIABoAHQAdABwAHMAOgAvAC8AdwB3AHcALgB5AG8AdQB0AHUAYgBlAC4AYwBvAG0A","triggeringPrincipal_base64":"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAAABCdntGuXUQAS/4CfOuSPZrLPEwK69Xkyth+CNIQ27P58B3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAQAABocAcwBjAHIAaQBwAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgACcAdQBuAHMAYQBmAGUALQBpAG4AbABpAG4AZQAnACAAJwB1AG4AcwBhAGYAZQAtAGUAdgBhAGwAJwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBzADMALgBhAG0AYQB6AG8AbgBhAHcAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBnAG8AbwBnAGwAZQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AeQBvAHUAdAB1AGIAZQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHMALgB5AHQAaQBtAGcALgBjAG8AbQA7ACAAaQBtAGcALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgAGQAYQB0AGEAOgAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvACoALgB0AGkAbABlAHMALgBtAGEAcABiAG8AeAAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAcABpAC4AbQBhAHAAYgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBjAHIAZQBhAHQAaQB2AGUAYwBvAG0AbQBvAG4AcwAuAG8AcgBnACAAaAB0AHQAcABzADoALwAvAGEAZAAuAGQAbwB1AGIAbABlAGMAbABpAGMAawAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvAHMAcAAuAGEAbgBhAGwAeQB0AGkAYwBzAC4AeQBhAGgAbwBvAC4AYwBvAG0AOwAgAGQAZQBmAGEAdQBsAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQA7ACAAZgByAGEAbQBlAC0AcwByAGMAIABoAHQAdABwAHMAOgAvAC8AKgAuAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAC0AYQBuAGEAbAB5AHQAaQBjAHMALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHkAbwB1AHQAdQBiAGUALQBuAG8AYwBvAG8AawBpAGUALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB0AHIAYQBjAGsAZQByAHQAZQBzAHQALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHMAdQByAHYAZQB5AGcAaQB6AG0AbwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAYwBjAG8AdQBuAHQAcwAuAGYAaQByAGUAZgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBhAGMAYwBvAHUAbgB0AHMALgBmAGkAcgBlAGYAbwB4AC4AYwBvAG0ALgBjAG4AIABoAHQAdABwAHMAOgAvAC8AdwB3AHcALgB5AG8AdQB0AHUAYgBlAC4AYwBvAG0AOwAgAHMAdAB5AGwAZQAtAHMAcgBjACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AbQBvAHoAaQBsAGwAYQAuAG8AcgBnACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbgBlAHQAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAGMAbwBtACAAJwB1AG4AcwBhAGYAZQAtAGkAbgBsAGkAbgBlACcAOwAgAGMAbwBuAG4AZQBjAHQALQBzAHIAYwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAG0AbwB6AGkAbABsAGEALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbQBvAHoAaQBsAGwAYQAuAG4AZQB0ACAAaAB0AHQAcABzADoALwAvACoALgBtAG8AegBpAGwAbABhAC4AbwByAGcAIABoAHQAdABwAHMAOgAvAC8AKgAuAG0AbwB6AGkAbABsAGEALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwAqAC4AbwBwAHQAaQBtAGkAegBlAGwAeQAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUAdABhAGcAbQBhAG4AYQBnAGUAcgAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAHcAdwB3AC4AZwBvAG8AZwBsAGUALQBhAG4AYQBsAHkAdABpAGMAcwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvACoALgB0AGkAbABlAHMALgBtAGEAcABiAG8AeAAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAcABpAC4AbQBhAHAAYgBvAHgALgBjAG8AbQA7ACAAYwBoAGkAbABkAC0AcwByAGMAIABoAHQAdABwAHMAOgAvAC8AKgAuAG8AcAB0AGkAbQBpAHoAZQBsAHkALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAHQAYQBnAG0AYQBuAGEAZwBlAHIALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAGcAbwBvAGcAbABlAC0AYQBuAGEAbAB5AHQAaQBjAHMALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHkAbwB1AHQAdQBiAGUALQBuAG8AYwBvAG8AawBpAGUALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwB0AHIAYQBjAGsAZQByAHQAZQBzAHQALgBvAHIAZwAgAGgAdAB0AHAAcwA6AC8ALwB3AHcAdwAuAHMAdQByAHYAZQB5AGcAaQB6AG0AbwAuAGMAbwBtACAAaAB0AHQAcABzADoALwAvAGEAYwBjAG8AdQBuAHQAcwAuAGYAaQByAGUAZgBvAHgALgBjAG8AbQAgAGgAdAB0AHAAcwA6AC8ALwBhAGMAYwBvAHUAbgB0AHMALgBmAGkAcgBlAGYAbwB4AC4AYwBvAG0ALgBjAG4AIABoAHQAdABwAHMAOgAvAC8AdwB3AHcALgB5AG8AdQB0AHUAYgBlAC4AYwBvAG0A","docIdentifier":4,"structuredCloneState":"AgAAAAAA8f8AAAAACAD//wAAAAATAP//","structuredCloneVersion":8}],"persist":true}],"lastAccessed":1513038759393,"hidden":false,"attributes":{},"userContextId":0,"index":1,"storage":{"https://accounts.firefox.com":{"__fxa_storage.last_page_loaded":"{\"timestamp\":1513038760725,\"view\":\"signin\"}","__fxa_storage.canGoBack":"true","webChannelEvents":"[\"fxaccounts:loaded\"]"}},"image":"https://www.mozilla.org/media/img/firefox/favicon.e6bb0e59df3d.ico","iconLoadingPrincipal":"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYB3pRy0IA0EdOTmQAQS6D9QJIHOlRteE8wkTq4cYEyCMYAAAAC/////wAAAbsBAAAANGh0dHBzOi8vd3d3Lm1vemlsbGEub3JnL2VuLVVTL2ZpcmVmb3gvNTIuMC9maXJzdHJ1bi8AAAAAAAAABQAAAAgAAAAPAAAACP////8AAAAI/////wAAAAgAAAAPAAAAFwAAAB0AAAAXAAAAHQAAABcAAAAdAAAANAAAAAAAAAA0/////wAAAAD/////AAAAF/////8AAAAX/////wEAAAAAAAAAAAABAAAAAAAA"},{"entries":[{"url":"about:home","title":"Mozilla Firefox Start Page","charset":"","ID":1,"docshellID":3,"triggeringPrincipal_base64":"SmIS26zLEdO3ZQBgsLbOywAAAAAAAAAAwAAAAAAAAEY=","docIdentifier":1,"persist":true}],"lastAccessed":1513038772277,"hidden":false,"attributes":{},"userContextId":0,"index":1,"image":"chrome://branding/content/icon32.png","iconLoadingPrincipal":"ZT4OTT7kRfqycpfCC8AeuAAAAAAAAAAAwAAAAAAAAEYBLyd8AA6vTdu5NkEya6SKrpIHOlRteE8wkTq4cYEyCMYAAAAABWFib3V0AAAABGhvbWUAAODaHXAvexHTjNAAYLD8FKOSBzpUbXhPMJE6uHGBMgjGAAAAAA5tb3otc2FmZS1hYm91dAAAAARob21lAAAAAAAAAAAA"}],"selected":2,"_closedTabs":[],"width":1280,"height":1040,"screenX":4,"screenY":4,"sizemode":"normal","title":"Mozilla Firefox Start Page","_shouldRestore":true,"closedAt":1513038772281,"closedId":0}],"selectedWindow":0,"_closedWindows":[],"session":{"lastUpdate":1513038772405,"startTime":1513038751444,"recentCrashes":0},"global":{}}

            //SiteSecurityServiceState.txt
            //look at C:\Users\eli\Desktop\temp\fxcmdPTest\SiteSecurityServiceState.txt

            //xulstore.json
            //{"chrome://browser/content/browser.xul":{"navigator-toolbox":{"iconsize":"small"},"titlebar-placeholder-on-menubar-for-caption-buttons":{"width":"138"},"titlebar-placeholder-on-TabsToolbar-for-captions-buttons":{"width":"138"},"main-window":{"screenX":"4","screenY":"4","width":"1280","height":"1040","sizemode":"normal"},"sidebar-box":{"sidebarcommand":"","width":"","src":""},"sidebar-title":{"value":""}}}
            //{"chrome://browser/content/browser.xul":{"navigator-toolbox":{"iconsize":"small"},"sidebar-box":{"sidebarcommand":"","width":"","src":""},"sidebar-title":{"value":""}}}
        }
        #endregion

        /// <summary>
        /// CurWorkD
        ///C:\Program Files(x86)\Mozilla Firefox  
        ///Desk
        ///C:\Users\eli\Desktop 
        ///Home
        ///C:\Users\eli 
        ///Progs
        ///C:\Users\eli\AppData\Roaming\Microsoft\Windows\Start Menu\Programs 
        ///WinD
        ///C:\WINDOWS Scratchpad/4:48:1
        ///XCurProcD
        ///C:\Program Files(x86)\Mozilla Firefox\browser 
        ///GreD
        ///C:\Program Files(x86)\Mozilla Firefox\browser 
        ///GreBinD
        ///C:\Program Files(x86)\Mozilla Firefox\browser 
        ///AppData
        ///C:\Users\eli\AppData\Roaming 
        ///TmpD
        ///C:\Users\eli\AppData\Local\Temp
        ///ProfD
        ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default  
        ///ProfLD
        ///C:\Users\eli\AppData\Local\Mozilla\Firefox\Profiles\3kw8mj5i.default  
        ///ProfDS
        ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default  
        ///ProfLDS
        ///C:\Users\eli\AppData\Local\Mozilla\Firefox\Profiles\3kw8mj5i.default 
        ///UsrSrchPlugns
        ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\searchplugins 
        ///UChrm
        ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\chrome 
        ///UMimTyp
        ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\mimeTypes.rdf 
        ///LclSt
        ///C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\localstore.rdf 
        ///XREAddonAppDir
        ///C:\Program Files(x86)\Mozilla Firefox\browser 
        ///XREAppDist
        ///C:\Program Files(x86)\Mozilla Firefox\distribution 
        ///AChrom
        ///C:\Program Files(x86)\Mozilla Firefox\browser\chrome 
        ///PrfDef
        ///C:\Program Files(x86)\Mozilla Firefox\defaults\pref 
        ///XREUSysExt
        ///C:\Users\eli\AppData\Roaming\Mozilla\Extensions
        ///XREAppFeat
        ///C:\Program Files(x86)\Mozilla Firefox\browser\features
        ///XREExeF
        ///C:\Program Files(x86)\Mozilla Firefox\firefox.exe
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="persistent"></param>
        /// <returns></returns>
        public IntPtr GetFile(string prop, out bool persistent)
        {
            Console.WriteLine(prop);
            persistent = false;
            switch (prop)
            {
                //profile
                case "ProfD":
                case "PrefD":
                case "ProfLD":
                case "ProfDS":
                case "ProfLDS":
                case "cachePDir":
                case "permissionDBPDir":
                case "indexedDBPDir":
                    return NewLocalFileAsNsPtr(this.SelectedBFXProfileCachPath);

                case "UsrSrchPlugns":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "searchplugins"));

                case "UChrm":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "chrome"));
                    
                case "DfltDwnld":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "DfltDwnld"));

                case "UMimTyp": // required to handle mailto protocol, etc.
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "mimeTypes.rdf"));

                case "LclSt":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "localstore.rdf"));


                case "TmpD":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileLocalPath, "tmp"));

                case "UpdRootD":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileLocalPath, "updates"));

                case "UAppData":
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileRoamingPath, "AppData", "Mozilla", "Firefox"));
                case "AppData":
                    //return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
                    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileRoamingPath, "AppData"));


                    
                case "XRESysSExtPD":
                    return NewLocalFileAsNsPtr(BrowseoFXSysExtentionsxtSDir);
                case "XRESysLExtPD":
                    return NewLocalFileAsNsPtr(BrowseoFXSysExtentionsxtLDir);



                //fx
                case "XREUSysExt":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "extensions"));
                    
                case "XREAppDist":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "distribution"));
                    
                //case "GreBinD":
                //case "GreD":
                case "XCurProcD":
               case "XREAddonAppDir":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser"));

                case "AChrom":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "chrome"));

                case "PrfDef":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "defaults", "pref"));

                case "DictD":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "dictionaries"));

                case "XREAppFeat":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "features"));

                case "XREExeF":
                    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "firefox.exe"));



                //C:\WINDOWS
                case "WinD":
                    return NewLocalFileAsNsPtr(Path.GetDirectoryName(Environment.SystemDirectory));
                // User start menu programs directory!
                case "Progs":
                    return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs)));
                //C:\Users\eli
                case "Home":
                    return NewLocalFileAsNsPtr(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                //C:\Users\eli\Desktop
                case "Desk":
                    return NewLocalFileAsNsPtr(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }
            Debug.WriteLine("Gecko.Xpcom.DirectoryServiceProvider.GetFile: not implemented: {0}", prop);
            return IntPtr.Zero;
        }

        #region actual paths
        //        case "Uhist":
        //    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "history.dat"));
        //case "Upanels":
        //    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "panels.rdf"));
        //case "Bmarks":
        //    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "bookmarks.html"));
        //case "Dloads":
        //    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "downloads.rdf"));
        //case "SrchF":
        //    return NewLocalFileAsNsPtr(Path.Combine(this.SelectedBFXProfileCachPath, "search.rdf"));  
        ////Services.dirsvc.get('cachePDir', Ci.nsIFile).path;
        ///*
        //Exception: [Exception... "Component returned failure code: 0x80004005 (NS_ERROR_FAILURE) [nsIProperties.get]"  nsresult: "0x80004005 (NS_ERROR_FAILURE)"  location: "JS frame :: Scratchpad/1 :: <TOP_LEVEL> :: line 39"  data: no]
        //*/
        ////Services.dirsvc.get('XRESysSExtPD', Ci.nsIFile).path;
        ///*
        //Exception: [Exception... "Component returned failure code: 0x80004005 (NS_ERROR_FAILURE) [nsIProperties.get]"  nsresult: "0x80004005 (NS_ERROR_FAILURE)"  location: "JS frame :: Scratchpad/1 :: <TOP_LEVEL> :: line 91"  data: no]
        //*/
        ////Services.dirsvc.get('XRESysLExtPD', Ci.nsIFile).path;
        ///*
        //Exception: [Exception... "Component returned failure code: 0x80004005 (NS_ERROR_FAILURE) [nsIProperties.get]"  nsresult: "0x80004005 (NS_ERROR_FAILURE)"  location: "JS frame :: Scratchpad/1 :: <TOP_LEVEL> :: line 95"  data: no]
        //*/
        ////Services.dirsvc.get('permissionDBPDir', Ci.nsIFile).path;
        ///*
        //Exception: [Exception... "Component returned failure code: 0x80004005 (NS_ERROR_FAILURE) [nsIProperties.get]"  nsresult: "0x80004005 (NS_ERROR_FAILURE)"  location: "JS frame :: Scratchpad/1 :: <TOP_LEVEL> :: line 103"  data: no]
        //*/
        ////Services.dirsvc.get('DictD', Ci.nsIFile).path;
        ///*
        //Exception: [Exception... "Component returned failure code: 0x80004005 (NS_ERROR_FAILURE) [nsIProperties.get]"  nsresult: "0x80004005 (NS_ERROR_FAILURE)"  location: "JS frame :: Scratchpad/1 :: <TOP_LEVEL> :: line 131"  data: no]
        //*/GreBinD

        //public IntPtr GetFile(string prop, out bool persistent)
        //{
        //    //Console.WriteLine(prop);
        //    persistent = false;
        //    switch (prop)
        //    {
        //        //C:\Users\eli\AppData\Local\Mozilla\Firefox\Profiles\3kw8mj5i.default
        //        case "ProfLDS":
        //        case "ProfLD":
        //        case "cachePDir":
        //        case "permissionDBPDir":
        //        case "indexedDBPDir":
        //        //case "DictD":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathLocal));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default
        //        case "ProfDS":
        //        case "ProfD":
        //            var file = NewLocalFileAsNsPtr(Path.Combine(ProfilePathRoaming));
        //            return file;
        //        case "UMimTyp":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathRoaming, "mimeTypes.rdf"));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\searchplugins
        //        case "UsrSrchPlugns":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathRoaming));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\chrome
        //        case "UChrm":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathRoaming, "chrome"));

        //        //C:\Users\eli\AppData\Local\Temp
        //        case "TmpD":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathData, "Temp"));

        //        //C: \Users\eli\AppData\Local
        //        case "LocalAppData":
        //            return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));

        //        //C:\Users\eli\AppData\Roaming
        //        case "AppData":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathData, "AppData"));
        //        //return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox
        //        case "UAppData":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathData, "UAppData"));
        //            //return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox"));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Extensions
        //        case "XREUSysExt":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathData, "XREUSysExt"));
        //            //return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Extensions"));

        //        //C: \Users\eli\AppData\Local\Mozilla\updates\
        //        case "UpdRootD":
        //            return NewLocalFileAsNsPtr(Path.Combine(ProfilePathData, "UpdRootD"));
        //           // return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "updates"));




        //        //C:\Program Files (x86)\Mozilla Firefox\browser
        //        case "XREAddonAppDir":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser"));

        //        //C:\Program Files (x86)\Mozilla Firefox\distribution
        //        case "XREAppDist":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "distribution"));

        //        //C:\Program Files (x86)\Mozilla Firefox\browser\features
        //        case "XREAppFeat":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "features"));

        //        case "XREExeF":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath));

        //        //C:\Program Files (x86)\Mozilla Firefox
        //        case "GreBinD":
        //        case "GreD":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath));

        //        //C:\Program Files (x86)\Mozilla Firefox\browser
        //        case "XCurProcD":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser"));

        //        //C:\Program Files (x86)\Mozilla Firefox\defaults\pref
        //        case "PrfDef":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "defaults", "pref"));

        //        //C:\Program Files (x86)\Mozilla Firefox\browser\chrome
        //        case "AChrom":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "chrome"));




        //        //C:\Users\eli
        //        case "Home":
        //            return NewLocalFileAsNsPtr(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        //        //C:\Users\eli\Desktop
        //        case "Desk":
        //            return NewLocalFileAsNsPtr(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

        //        //C:\Users\eli\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
        //        case "Progs":
        //            return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs)));

        //        //C:\WINDOWS
        //        case "WinD":
        //            return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

        //        case "XRESysSExtPD":
        //        case "XRESysLExtPD":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "extensions"));

        //        default:
        //            break;
        //    }

        //    Debug.WriteLine("Gecko.Xpcom.DirectoryServiceProvider.GetFile: not implemented: {0}", prop);
        //    return IntPtr.Zero;
        //}
        #endregion

        //public IntPtr GetFile(string prop, out bool persistent)
        //{
        //    //Console.WriteLine(prop);
        //    persistent = false;
        //    switch (prop)
        //    {
        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default
        //        //C:\Users\eli\AppData\Local\Mozilla\Firefox\Profiles\3kw8mj5i.default
        //        case "ProfLDS":
        //        case "ProfLD":
        //        case "cachePDir":
        //        case "permissionDBPDir":
        //        case "indexedDBPDir":
        //        case "ProfDS":
        //        case "ProfD":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath));
        //        case "PrefD":
        //        case "PrefF":
        //        case "PrefDL":
        //        case "PrefDOverride":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath, "prefs.js"));

        //        case "UMimTyp":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath, "mimeTypes.rdf"));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\searchplugins
        //        case "UsrSrchPlugns":
        //        //    var uri = IOService.GetService().CreateNsIUri("chrome://browser/content/browser.xul");
        //        //    //case "DictD":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath, "searchplugins"));

        //        case "SrchPlugns":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath, "searchplugins"));

        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox\Profiles\3kw8mj5i.default\chrome
        //        case "UChrm":
        //                return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath, "chrome"));



        //        case "DfltDwnld":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileCachPath, "DfltDwnld"));

        //        //C:\Users\eli\AppData\Local\Temp
        //        case "TmpD":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileLocalPath, "AppData", "Temp"));

        //        //C: \Users\eli\AppData\Local
        //        case "LocalAppData":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileLocalPath, "AppData"));

        //        //C:\Users\eli\AppData\Roaming
        //        case "AppData":
        //        //C:\Users\eli\AppData\Roaming\Mozilla\Firefox
        //        case "UAppData":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileRoamingPath, "AppData"));
        //        //C:\Users\eli
        //        case "Home":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileRoamingPath, "Home"));
        //        //C:\Users\eli\Desktop
        //        case "Desk":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileRoamingPath, "Desktop"));
        //        //C: \Users\eli\AppData\Local\Mozilla\updates\
        //        case "UpdRootD":
        //            return NewLocalFileAsNsPtr(Path.Combine(SelectedBFXProfileRoamingPath, "updates"));


        //        case "XREUSysExt":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "extensions"));

        //        case "XREAppDist":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath));
        //        case "XREAppFeat":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "features"));
        //        case "XREAddonAppDir":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser"));
        //        //case "XREExeF":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "firefox.exe"));

        //        case "XRESysSExtPD":
        //            return NewLocalFileAsNsPtr(BrowseoFXSysExtentionsxtSDir);
        //        case "XRESysLExtPD":
        //            return NewLocalFileAsNsPtr(BrowseoFXSysExtentionsxtLDir);














        //        ////C:\Program Files (x86)\Mozilla Firefox\distribution
        //        //case "GreBinD":
        //        //case "GreD":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "distribution"));

        //        case "DictD":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "dictionaries"));

        //        ////C:\Program Files (x86)\Mozilla Firefox\browser\features
        //        //case "XREAppFeat":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "features"));

        //        //case "XREExeF":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath));
        //        //case "XREExeF":
        //        //    return NewLocalFileAsNsPtr(System.AppDomain.CurrentDomain.BaseDirectory);

        //        //case "GreBinD":
        //        //case "GreD":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath));
        //        //return NewLocalFileAsNsPtr(@"C:\Program Files (x86)\Mozilla Firefox");

        //        //C:\Program Files (x86)\Mozilla Firefox
        //        //case "GreBinD":
        //        //case "GreD":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath));

        //        ////C:\Program Files (x86)\Mozilla Firefox\browser
        //        //case "XREAddonAppDir":
        //        //    return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser"));
        //        //C:\Program Files (x86)\Mozilla Firefox\browser
        //        case "XCurProcD":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser"));

        //        //C:\Program Files (x86)\Mozilla Firefox\defaults\pref
        //        case "PrfDef":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "defaults", "pref"));

        //        //C:\Program Files (x86)\Mozilla Firefox\browser\chrome
        //        case "AChrom":
        //            return NewLocalFileAsNsPtr(Path.Combine(this.GeckoPath, "browser", "chrome"));




        //        //C:\Users\eli\AppData\Roaming\Microsoft\Windows\Start Menu\Programs
        //        case "Progs":
        //            return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs)));

        //        //C:\WINDOWS
        //        case "WinD":
        //            return NewLocalFileAsNsPtr(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows)));


        //        default:
        //            Debug.WriteLine("Gecko.Xpcom.DirectoryServiceProvider.GetFile: not implemented: {0}", prop);
        //            return IntPtr.Zero;
        //    }
        //}


        //PERMS_FILE      : 0o644,
        //PERMS_DIRECTORY : 0o755,

        /// <summary>
        /// Declaration in nsXPCOM.h
        /// XPCOM_API(nsresult) NS_NewLocalFile(const nsAString &amp;path, bool followLinks, nsIFile* *result);
        /// </summary>
        /// <param name="path"></param>
        /// <param name="followLinks"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [DllImport("xul.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NS_NewLocalFile([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Gecko.CustomMarshalers.AStringMarshaler))] nsAString path, [MarshalAs(UnmanagedType.U1)] bool followLinks, [MarshalAs(UnmanagedType.Interface)] out nsIFile result);
        [DllImport("xul.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int NS_NewLocalFile([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Gecko.CustomMarshalers.AStringMarshaler))] nsAString path, [MarshalAs(UnmanagedType.U1)] bool followLinks, out IntPtr result);

        public IntPtr NewLocalFileAsNsPtr(string filename)
        {
            IntPtr result;
            using (var fileName = new nsAString(filename))
            {
                int error = NS_NewLocalFile(fileName, true, out result);
                Marshal.ThrowExceptionForHR(error);
            }
            return result;
        }

        public nsIFile NewLocalFile(string filename)
        {
            nsIFile result;
            using (var fileName = new nsAString(filename))
            {
                int error = NS_NewLocalFile(fileName, true, out result);
                Marshal.ThrowExceptionForHR(error);
            }
            return result;
        }
        
        public string CreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public ComObject<nsIFile> OpenFile(string filename)
        {
            IntPtr nsFilePtr = NewLocalFileAsNsPtr(filename);
            try
            {
                return new ComObject<nsIFile>((nsIFile)Marshal.GetTypedObjectForIUnknown(nsFilePtr, typeof(nsIFile)));
            }
            finally
            {
                Marshal.Release(nsFilePtr);
            }
        }

        IntPtr nsIProperties.Get(string prop, ref Guid iid)
        {
            return IntPtr.Zero;
            // throw new NotImplementedException();
        }

        void nsIProperties.Set(string prop, nsISupports value)
        {
           // throw new NotImplementedException();
        }

        bool nsIProperties.Has(string prop)
        {
            return true;
           // throw new NotImplementedException();
        }

        void nsIProperties.Undefine(string prop)
        {
           // throw new NotImplementedException();
        }

        void nsIProperties.GetKeys(out uint count, out IntPtr keys)
        {
            count = 0;
            keys = IntPtr.Zero;
           // throw new NotImplementedException();
        }
    }
}
