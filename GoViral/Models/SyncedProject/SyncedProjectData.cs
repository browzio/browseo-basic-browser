using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Organiser.Common.Classes;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace GoViral.Models
{
    public class SyncedProjectData : ViewModelBase
    {
        [XmlIgnore]
        public ICommand OnCommandRaisedInView { get; set; }

        private bool isShared;  
        public bool IsShared
        {
            get { return isShared; }
            set { isShared = value; RaisePropertyChanged("IsShared"); }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; RaisePropertyChanged("Url"); }
        }

        private string pageName;
        public string PageName
        {
            get { return pageName; }
            set { pageName = value; RaisePropertyChanged("PageName"); }
        }

        private string fromProject;
        public string FromProject
        {
            get { return fromProject; }
            set { fromProject = value; RaisePropertyChanged("FromProject"); }
        }

        private object isOpenLock = new object();

        internal void RaiseOnCommandFromView(string param)
        {
            switch (param)
            {

                case "launchToSystemFirefox":
                    LaunchToSystemFF(false);
                    break;

                case "launchToSystemFirefox52":
                    LaunchToSystemFF(true);
                    break;

                case "launchToSystemChrome":
                    new Thread(launchToSystemBrowser).Start(false);
                    break;
                default:
                    break;
            }
        }

        private void LaunchToSystemFF(bool outerprocess)
        {
            Task.Run(() =>
            {
                string projpath = MyFilesDatabase.FindProjectDirByName(FromProject, "");
                var PersonData = MyFilesDatabase.GetUpPdaaFromPath(projpath);

                string ffpath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + FromProject);
                if (!Directory.Exists(ffpath)) Directory.CreateDirectory(ffpath);

                MyFilesDatabase.LaunchToSystemFF("-new-instance -no-remote -new-tab -url about:home -new-tab -url " + url + " -profile \"" + ffpath + "\"", ffpath, PersonData, outerprocess);
            });
        }

        private async void launchToSystemBrowser(object isFF)
        {
            try
            {
                UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_SEOEvent + " url " + Url);
                string projpath = MyFilesDatabase.FindProjectDirByName(FromProject, "");
                string url = Url;

                bool toff = Convert.ToBoolean(isFF);
                var PersonData = MyFilesDatabase.GetUpPdaaFromPath(projpath);

                if (toff)
                {
                    string ffpath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + FromProject);
                    if (!Directory.Exists(ffpath)) Directory.CreateDirectory(ffpath);

                    MyFilesDatabase.LaunchToSystemFF("-new-instance -no-remote -new-tab -url about:home -new-tab -url " + url + " -profile \"" + ffpath + "\"", ffpath, PersonData);
                }
                else
                {
                    string proxy = "";
                    if (!string.IsNullOrEmpty(PersonData.ProxyIP) && !string.IsNullOrWhiteSpace(PersonData.ProxyIP))
                    {
                        proxy = " --proxy-server=" + PersonData.ProxyIP + ":" + PersonData.ProxyPort;
                    }

                    string datadir = Path.Combine(MyFilesDatabase.GetBaseDir(), "SystemChromeDataDir\\ProfilesData\\" + FromProject);
                    if (!Directory.Exists(datadir)) Directory.CreateDirectory(datadir);

                    string restoreSession = "";
                    if (MessageBox.Show("Restore Previous Session? ", "Restore?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        restoreSession = " --restore-last-session";
                    }
                    string path = Path.Combine(MyFilesDatabase.GetBaseDir(), "Caches\\" + FromProject);
                    if (Directory.Exists(path))
                    {
                        if (MessageBox.Show("Would you like to sync your current browseo cache with this instance? ", "Sync?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                            try
                            {
                                await Task.Run(() =>
                                {
                                    string datatDirDefaultPath = datadir + "\\Default";
                                    if (!Directory.Exists(datatDirDefaultPath)) Directory.CreateDirectory(datatDirDefaultPath);
                                    SyncCacheFrom(path, datatDirDefaultPath, false);
                                });
                            }
                            catch
                            {

                                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                                if (MessageBox.Show("Cache Sync Failed, Dou want to continue?", "Continue?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                                    return;
                            }
                            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                        }
                    }
                    Process process = new Process();
                    process.StartInfo.FileName = "chrome";
                    process.StartInfo.Arguments = url + " --new-window --disable-media-stream --disable-webrtc-hw-encoding --disable-webrtc-hw-decoding --enforce-webrtc-ip-permission-check --no-default-browser-check --user-data-dir=\"" + datadir + "\"" + proxy + restoreSession;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();

                    System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher();
                    watcher.Path = datadir;
                    watcher.Filter = "*.*";//Watch all the files
                    watcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastAccess | System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size | System.IO.NotifyFilters.Attributes;
                    watcher.EnableRaisingEvents = true;
                    watcher.Deleted += (s, e) =>
                    {
                        if (e.Name == "lockfile")
                        {
                            lock (isOpenLock)
                            {
                                if (MessageBox.Show("Would you like to sync this sessions cache to browseo? ", "Sync Cache?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = Cursors.Wait; });
                                    try
                                    {
                                        string datatDirDefaultPath = datadir + "\\Default";
                                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                                        SyncCacheFrom(datatDirDefaultPath, path, true);
                                    }
                                    catch
                                    {
                                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                                        MessageBox.Show("Cache Sync Failed.");
                                    }
                                    Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                                }
                                try
                                {
                                    watcher.Dispose();
                                }
                                catch { }
                            }

                        }
                    };
                }
            }
            catch
            {

                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                MessageBox.Show("Failed to launch in browser.");
            }
        }

        internal void SyncCacheFrom(string path, string datatDirDefaultPath, bool isFromChrome)
        {

            string cookiesFile_From = path + "\\Cookies";
            string cookiesFile_To = datatDirDefaultPath + "\\Cookies";
            if (File.Exists(cookiesFile_From)) File.Copy(cookiesFile_From, cookiesFile_To, true);

            string cookiesJurnalFile_From = path + "\\Cookies-journal";
            string cookiesJurnalFile_To = datatDirDefaultPath + "\\Cookies-journal";
            if (File.Exists(cookiesJurnalFile_From)) File.Copy(cookiesJurnalFile_From, cookiesJurnalFile_To, true);


            if (!isFromChrome)
            {
                string datatDirDefaultCachePath = datatDirDefaultPath + "\\Cache";
                if (!Directory.Exists(datatDirDefaultCachePath)) Directory.CreateDirectory(datatDirDefaultCachePath);

                foreach (var file in new DirectoryInfo(path).GetFiles())
                {
                    if (file.Name == "Cookies" || file.Name == "Cookies-journal") continue;
                    file.CopyTo(datatDirDefaultCachePath + "\\" + file.Name, true);
                }
            }
            else
            {
                string datatDirDefaultCachePath = path + "\\Cache";
                if (!Directory.Exists(datatDirDefaultCachePath))
                {
                    foreach (var file in new DirectoryInfo(datatDirDefaultCachePath).GetFiles())
                    {
                        if (file.Name == "Cookies" || file.Name == "Cookies-journal") continue;
                        file.CopyTo(datatDirDefaultPath + "\\" + file.Name, true);
                    }
                }
            }

            string gpuCacheDir_From = path + "\\GPUCache";
            if (Directory.Exists(gpuCacheDir_From))
            {
                string gpuCacheDi_To = datatDirDefaultPath + "\\GPUCache";
                CopyAll(new DirectoryInfo(gpuCacheDir_From), new DirectoryInfo(gpuCacheDi_To));
            }

            string localStorageDir_From = path + "\\Local Storage";
            if (Directory.Exists(localStorageDir_From))
            {
                string localStorageDir_To = datatDirDefaultPath + "\\Local Storage";
                CopyAll(new DirectoryInfo(localStorageDir_From), new DirectoryInfo(localStorageDir_To));
            }

            string db_From = path + "\\databases";
            if (Directory.Exists(db_From))
            {
                string _To = datatDirDefaultPath + "\\databases";
                CopyAll(new DirectoryInfo(db_From), new DirectoryInfo(_To));
            }

            string idb_From = path + "\\IndexedDB";
            if (Directory.Exists(idb_From))
            {
                string _To = datatDirDefaultPath + "\\IndexedDB";
                CopyAll(new DirectoryInfo(idb_From), new DirectoryInfo(_To));
            }
        }


        public void CopyAll(DirectoryInfo source, DirectoryInfo target, bool replacetagetsWithDot = false)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                string name = diSourceSubDir.Name;
                if (replacetagetsWithDot)
                {
                    if (name.Contains("_"))
                    {
                        name = name.Replace("_", ".");
                        replacetagetsWithDot = false;
                    }
                }
                string nexrtDir = Path.Combine(target.FullName, name);
                Directory.CreateDirectory(nexrtDir);
                DirectoryInfo nextTargetSubDir = new DirectoryInfo(nexrtDir);
                CopyAll(diSourceSubDir, nextTargetSubDir, replacetagetsWithDot);
            }
        }

        //public SyncedProjectData()
        //{
        //    OnCommandRaisedInView = new RelayCommand(OnCommandRaisedInView_Activated);
        //}

        //private void OnCommandRaisedInView_Activated(object param)
        //{

        //}

        //
    }
}
