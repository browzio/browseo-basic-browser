using Microsoft.Win32;
using PData.FilesReader;
using ProjectsList.Models;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Linq;

namespace Organiser.Common.Classes
{
    public class BrowserSettimgs
    {
        public static bool JavaEnabled = true;
        public static bool JavascriptEnabled = true;
        public static bool FlashEnabled = true;
        public static bool DoNotTrackEnabled = true; 
        public static bool SetSysDateEnabled = false;

        private static List<string> timeZoneList;
        public static List<string> AvailableTimeZones
        {
            get
            {
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

                if (timeZoneList == null)
                {
                    BrowserSettimgs.AvailableTimeZones = new List<string>();       
                    for (int i = 0; i < timeZones.Count; i++)
                    {
                        TimeZoneInfo tz = timeZones[i];

                        if (tz.DisplayName == TimeZoneInfo.Local.DisplayName) SITimeZone = i;

                        BrowserSettimgs.AvailableTimeZones.Add(tz.DisplayName);
                    }
                }
                else
                {
                    for (int i = 0; i < timeZones.Count; i++)
                    {
                        TimeZoneInfo tz = timeZones[i];    
                        if (tz.DisplayName == TimeZoneInfo.Local.DisplayName)
                        {
                            SITimeZone = i;
                            break;
                        }
                    }

                }

                return timeZoneList;
            }
            set { timeZoneList = value; }
        }
        public static int SITimeZone { get; set; }
    }

    public class MyFilesDatabase
    {
        public const string SPLITTER = "{[:]}";

        public static bool CanSeeProxys = false;

        static System.Threading.Thread ramCheckerThread;
        static ulong availmem = 0;
        static int timesToCheck = 0;

        public static string GetBaseDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        }

        public static List<KeyValuePair<string, string>> GetSubProjectsFolders(string path, string projname)
        {
            List<KeyValuePair<string, string>> projects = new List<KeyValuePair<string, string>>();
           // string path = Path.Combine(GetBaseDir(), "Projects\\" + projectName);
            if (Directory.Exists(path))
            {
                projects.Add(new KeyValuePair<string, string>(projname, Path.Combine(path, "ProjectData.ini")));
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    if (!dir.Name.Contains("_tier_"))
                    projects.Add(new KeyValuePair<string, string>(dir.Name, dir.FullName));
                }
                return projects;
            }
            return projects;
        }

        public static List<KeyValuePair<string, string>> GetAllProjectsAndDirs()
        {
            List<KeyValuePair<string, string>> projects = new List<KeyValuePair<string, string>>();
            string path = Path.Combine(GetBaseDir(), "Projects");

            if (Directory.Exists(path))
            {
                WalkDirectoryTree(new DirectoryInfo(path), ref projects);
                return projects;
            }
            
            return projects;
        }

        public static void WalkDirectoryTree(System.IO.DirectoryInfo root, ref List<KeyValuePair<string, string>> projects)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories();

            if (subDirs != null)
            {
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    string ProjName = dirInfo.Name;
                    if (!dirInfo.GetFiles().Any(f => f.Name.Contains("UserData")) && !ProjName.Contains("_folder"))
                    {
                        if (ProjName.Contains("_tier_"))
                        {
                            ProjName = ProjName.Replace("_tier_", "");
                        }
                        projects.Add(new KeyValuePair<string, string>(ProjName, dirInfo.FullName));
                    }

                    WalkDirectoryTree(dirInfo, ref projects);
                }
            }
        }

        public static string FindProjectDirByName(string projectName, string profileName)
        {
            string path = Path.Combine(GetBaseDir(), "Projects");
            string dir = RecursiveProjectFindByName(new DirectoryInfo(path), projectName, profileName);
            
            return dir;
        }

        private static string RecursiveProjectFindByName(DirectoryInfo directoryInfo, string projectName, string profileName)
        {
            if (directoryInfo != null)
            {
                FileInfo[] files = directoryInfo.GetFiles();
                if (files != null)
                {
                    foreach (FileInfo fi in files)
                    {
                        if (profileName == "")
                        {
                            if (fi.Name == "ProjectData.ini")
                            {
                                IniFile ini = new IniFile(fi.FullName);
                                string ProjectName = ini.IniReadValue("Data", "ProjectName");
                                if(projectName == ProjectName) return fi.Directory.FullName;
                            }
                        }
                        else
                        {
                            if (fi.Name == "ProjectData.ini" || fi.Name == "UserData.ini")
                            {
                                IniFile ini = new IniFile(fi.FullName);
                                string ProjectName = ini.IniReadValue("Data", "ProjectName");
                                string ProfileName = ini.IniReadValue("Data", "ProfileName");

                                if (ProjectName.Trim().ToLower() == projectName.Trim().ToLower())
                                {
                                    if (ProfileName.Trim().ToLower() == profileName.Trim().ToLower())
                                    {
                                        return fi.Directory.FullName;
                                    }
                                    else
                                    {
                                        if (fi.Name == "ProjectData.ini")
                                        {
                                            DirectoryInfo[] Pdirs = directoryInfo.GetDirectories();
                                            if (Pdirs != null)
                                            {
                                                foreach (var dir in Pdirs)
                                                {
                                                    FileInfo[] Pfiles = dir.GetFiles();
                                                    if (Pfiles != null)
                                                    {
                                                        foreach (FileInfo pfi in files)
                                                        {
                                                            if (pfi.Name == "UserData.ini")
                                                            {
                                                                string Name = ini.IniReadValue("Data", "ProjectName");
                                                                string ProName = ini.IniReadValue("Data", "ProfileName");
                                                                if (Name.Trim().ToLower() == Name.Trim().ToLower())
                                                                {
                                                                    if (ProName.Trim().ToLower() == ProName.Trim().ToLower())
                                                                    {
                                                                        return pfi.Directory.FullName;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    return fi.Directory.FullName;
                                }
                            }
                        }
                    }
                }


                DirectoryInfo[] dirs = directoryInfo.GetDirectories();
                if (dirs != null)
                {
                    foreach (DirectoryInfo item in dirs)
                    {
                        string dir = RecursiveProjectFindByName(item, projectName, profileName);

                        if (dir != "")
                            return dir;
                    }
                }
            }

            return "";
        }

        public static bool HasMultipleProfiles(string path)
        {
            //string path = Path.Combine(GetBaseDir(), "Projects\\" + projectName);
            if (!Directory.Exists(path))
                return false;

            if (Directory.GetDirectories(path).Length >= 1)
                return true;

            return false;
        }

        public static PersonData GetSubProjectPersonData(string selectedProjectPath)
        {
            string sitesFilePath = selectedProjectPath;
            if (!selectedProjectPath.Contains(".ini"))
                sitesFilePath = Path.Combine(selectedProjectPath, "UserData.ini");
            if (!File.Exists(sitesFilePath))
                sitesFilePath = sitesFilePath.Replace("UserData.ini", "ProjectData.ini");

            PersonData profile = new PersonData();

            if (!File.Exists(sitesFilePath))
                return profile;

            IniFile ini = new IniFile(sitesFilePath);
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
                profile.Notes = ini.IniReadValue("Data", "Notes");
                profile.WebAddress = ini.IniReadValue("Data", "WebAddress");
                profile.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                profile.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                profile.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                try
                {
                    profile.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }
            return profile;
        }

        public static PersonData SetProfileFromini(string path)
        {
            PersonData profile = new PersonData();
            IniFile ini = new IniFile(path);
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
                profile.WebAddress = ini.IniReadValue("Data", "WebAddress");
                profile.Zip = ini.IniReadValue("Data", "Zip");
                profile.Country = ini.IniReadValue("Data", "Country");
                profile.Notes = ini.IniReadValue("Data", "Notes");
                profile.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                profile.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                profile.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                try
                {
                    profile.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }

            return profile;
        }

        #region sessions
        public static void DeleteSession(string projectName)
        {
            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public static void SaveSession(string projectName, List<string> links)
        {
            links.Add(
                BrowserSettimgs.FlashEnabled +
                "," + BrowserSettimgs.JavaEnabled +
                "," + BrowserSettimgs.JavascriptEnabled +
                "," + BrowserSettimgs.SetSysDateEnabled +
                "," + BrowserSettimgs.SITimeZone+
                "," + BrowserSettimgs.DoNotTrackEnabled);

            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, "sites.txt");    
            File.WriteAllLines(filePath, links.ToArray());
        }

        public static List<string> GetSavedSesstion(string projectName)
        {
            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if (!Directory.Exists(directory)) return new List<string>();

            string filePath = Path.Combine(directory, "sites.txt");
            if (!File.Exists(filePath)) return new List<string>();

            List<string> fileLines = File.ReadAllLines(filePath).ToList();
            try
            {
                if (fileLines.Count > 0)
                {
                   fileLines.RemoveAll(line => string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line));
                   string lastLine = fileLines[fileLines.Count - 1];

                    if (lastLine.Contains(","))
                    {
                        string[] browserSettings = lastLine.Split(',');
                        BrowserSettimgs.FlashEnabled = Convert.ToBoolean(browserSettings[0]);
                        BrowserSettimgs.JavaEnabled = Convert.ToBoolean(browserSettings[1]);
                        BrowserSettimgs.JavascriptEnabled = Convert.ToBoolean(browserSettings[2]);
                        BrowserSettimgs.SetSysDateEnabled = Convert.ToBoolean(browserSettings[3]);
                        BrowserSettimgs.SITimeZone = Convert.ToInt32(browserSettings[4]);
                        if(browserSettings.Length > 5)
                        {
                            BrowserSettimgs.DoNotTrackEnabled = Convert.ToBoolean(browserSettings[5]);
                        }
                        if (BrowserSettimgs.SetSysDateEnabled)
                        {
                            TimeHelper.StartSetTimeAndZoneProcess(new DateAndTimeZone() { TimeZone = TimeZoneInfo.GetSystemTimeZones()[BrowserSettimgs.SITimeZone] });
                        }
                        fileLines.RemoveAt(fileLines.Count - 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return fileLines;
        }
        #endregion

        #region cookie data
        public static List<string> CookieSites = new List<string>();//sites saved for quick reference 

        public static void GetSites()
        {
            new System.Threading.Thread(() =>
            {
                CookieSites.Clear();

                string filePath = Path.Combine(GetBaseDir(), "VisitedSites\\SitesLog.txt");
                if (File.Exists(filePath))
                {
                    try
                    {
                        foreach (var item in File.ReadAllLines(filePath))
                        {
                            CookieSites.Add(item);
                        }
                    }
                    catch { }
                }
            }).Start();
        }

        public static void AppendToSavedSites(string site)
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    string dir = Path.Combine(GetBaseDir(), "VisitedSites");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    string filePath = Path.Combine(GetBaseDir(), "VisitedSites\\SitesLog.txt");
                    bool found = false;
                    foreach (var item in CookieSites)
                    {
                        if (item == site)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        File.AppendAllText(filePath, site + Environment.NewLine);
                        CookieSites.Add(site);
                    }
                }
                catch { }
            }).Start();
        }
        #endregion

        #region bookmarks

        public static void SaveSiteBookmark(string url, string name, string projName, string saveTimeStamp)
        {
            string dirForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks");
            if (!Directory.Exists(dirForBookmarks)) Directory.CreateDirectory(dirForBookmarks);

            string filePath = Path.Combine(GetBaseDir(), "Bookmarks\\" + projName + ".txt");

            if (File.Exists(filePath))
            {
                foreach (string site in File.ReadAllLines(filePath))
                {
                    if (site.Contains(url + SPLITTER + name))
                        return;
                }
            }

           string lineToSave = url + SPLITTER + name + SPLITTER + saveTimeStamp;

            File.AppendAllText(filePath, lineToSave + Environment.NewLine);
        }

        public static IEnumerable<KeyValuePair<string, string>> GetBookmarkedFolders(string ProjectName)
        {
            string dirForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks", ProjectName);
            List<KeyValuePair<string, string>> dirArray = new List<KeyValuePair<string, string>>();

            if (Directory.Exists(dirForBookmarks))
            {
                foreach (string dir in Directory.GetDirectories(dirForBookmarks))
                {
                    dirArray.Add(new KeyValuePair<string, string>(dir, Directory.GetCreationTime(dir).ToString()));
                }
            }

            return dirArray;
        }

        public static IEnumerable<string> GetBookmarkedSitesByPath(string folderPath, string projname)
        {
            string pathToBoockmarksLog = Path.Combine(folderPath, projname + ".txt");

            string[] bookmarksLines = new string[0];

            if (File.Exists(pathToBoockmarksLog))
                bookmarksLines = File.ReadAllLines(pathToBoockmarksLog);

            return bookmarksLines;
        }

        public static IEnumerable<string> GetBookmarkedSitesByProjName(string projname)
        {
            string pathToBoockmarksLog = Path.Combine(GetBaseDir(), "Bookmarks", projname + ".txt");

            string[] bookmarksLines = new string[0];

            if (File.Exists(pathToBoockmarksLog))
                bookmarksLines = File.ReadAllLines(pathToBoockmarksLog);

            return bookmarksLines;
        }

        public static void AppendBookmarkByFolderAnProjName(string projectName, string folderName, string url, string name, string dateTimeStamp)
        {
            string dirForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks", projectName, folderName);
            if (!Directory.Exists(dirForBookmarks)) Directory.CreateDirectory(dirForBookmarks);

            string filePath = Path.Combine(dirForBookmarks, projectName + ".txt");
            if (File.Exists(filePath))
            {
                foreach (string site in File.ReadAllLines(filePath))
                {
                    if (site.Contains(url + SPLITTER + name))
                        return;
                }
            }

            File.AppendAllText(filePath, (url + SPLITTER + name + SPLITTER + dateTimeStamp) + Environment.NewLine);
        }

        public static void AppendBookmarkByFolderAnProjNameNoSites(string projectName, string folderName)
        {
            string dirForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks", projectName, folderName);
            if (!Directory.Exists(dirForBookmarks)) Directory.CreateDirectory(dirForBookmarks);
        }

        public static void DeleteBookmarks(string projectName)
        {
            string fileForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks", projectName + ".txt");
            if (File.Exists(fileForBookmarks)) File.Delete(fileForBookmarks);

            string dirForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks", projectName);
            if (Directory.Exists(dirForBookmarks)) Directory.Delete(dirForBookmarks, true);
        }

        public static void MigrateOldBookmarks(string projectName)
        {
            string dirForBookmarks = Path.Combine(GetBaseDir(), "Bookmarks");
            if (!Directory.Exists(dirForBookmarks))
            {
                Directory.CreateDirectory(dirForBookmarks);

                string dirForBookmarksOld = Path.Combine(GetBaseDir(), "Sites");
                if (!Directory.Exists(dirForBookmarksOld)) return;

                foreach (string file in Directory.GetFiles(dirForBookmarksOld))
                {
                    foreach (string site in File.ReadAllLines(file))
                    {
                        string filePath = file.Replace("\\Sites\\", "\\Bookmarks\\");
                        File.AppendAllText(filePath, (site + SPLITTER + site) + Environment.NewLine);
                    }
                }
            }
        }

        public static void SaveBookmarkedSession(string projectName, string name, string[] sites, string[] names, string[] dateTimeStamp)
        {
            string directory = Path.Combine(GetBaseDir(), "BookmarkSessions", projectName, name);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string[] fileLines = new string[sites.Length];
            for (int i = 0; i < sites.Length; i++)
            {
                fileLines[i] = sites[i] + SPLITTER + names[i] + SPLITTER + dateTimeStamp;
            }
            string file = Path.Combine(directory, "sites.txt");
            File.WriteAllLines(file, fileLines);
        }

        #endregion

        #region rss

        public static void SaveRssFeedsSiteLinks(string links, PersonData profile, string tabTitle)
        {
            string directoryPath = Path.Combine(GetBaseDir(), "SavedRssLinks", profile.ProjectName, tabTitle);
            string filePath = Path.Combine(directoryPath, "rssLinks.txt");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            string[] splitLinks = links.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string link in splitLinks)
            {
                File.AppendAllText(filePath, link + Environment.NewLine);    
            }
        }

        public static List<string> GetRssFeedLinks(PersonData profile, string tabTitle)
        {
            return GetRssFeedLinks(profile.ProjectName, tabTitle);
        }

        public static List<string> GetRssFeedLinks(string projectname, string tabTitle)
        {
            List<string> returnedList = new List<string>();

            string directoryPath = Path.Combine(GetBaseDir(), "SavedRssLinks", projectname, tabTitle);
            string filePath = Path.Combine(directoryPath, "rssLinks.txt");

            if (!Directory.Exists(directoryPath)) return returnedList;

            if (!File.Exists(filePath)) return returnedList;

            foreach (string link in File.ReadAllLines(filePath))
            {
                returnedList.Add(link);
            }

            return returnedList;
        }

        public static List<string> GetRssFeedLinksTabsTitle(PersonData profile)
        {
            return GetRssFeedLinksTabsTitlesByName(profile.ProjectName);
        }

        public static List<string> GetRssFeedLinksTabsTitlesByName(string projectName)
        {
            List<string> returnedList = new List<string>();

            string directoryPath = Path.Combine(GetBaseDir(), "SavedRssLinks", projectName);

            if (!Directory.Exists(directoryPath)) return returnedList;

            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                returnedList.Add(dir.Name);
            }

            return returnedList;
        }

        public static void RemoveDeleteRssTab(PersonData profile, string tabTitle)
        {
            string directoryPath = Path.Combine(GetBaseDir(), "SavedRssLinks", profile.ProjectName, tabTitle);
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }

        #endregion

        public static void DownloadImage(string url)
        {
            string saveFileFilename = "";
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Png files (*.png)|*.png|JPeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                sfd.FilterIndex = 0;
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() != true) return;
                saveFileFilename = sfd.FileName;
            });
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Proxy = GetRequestsProxy();

                    byte[] data = webClient.DownloadData(url);

                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        using (var yourImage = System.Drawing.Image.FromStream(mem))
                        {
                            yourImage.Save(saveFileFilename);
                        }
                    }
                }

                try
                {
                    FileInfo fileInfo = new FileInfo(saveFileFilename);
                    System.Diagnostics.Process.Start(fileInfo.DirectoryName);
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save image. " + ex.Message);
            }
        }

        public static IWebProxy GetRequestsProxy()
        {
            WebProxy proxy = null;
            try
            {
                if (!string.IsNullOrEmpty(GloableProfData.PData.ProxyIP) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyIP) &&
                    !string.IsNullOrEmpty(GloableProfData.PData.ProxyPort) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyPort))
                {
                    proxy = new WebProxy(GloableProfData.PData.ProxyIP, Convert.ToInt32(GloableProfData.PData.ProxyPort));

                    if (!string.IsNullOrEmpty(GloableProfData.PData.ProxyUsername) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyUsername) &&
                        !string.IsNullOrEmpty(GloableProfData.PData.ProxyPassword) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyPassword))
                    {
                        proxy.Credentials = new NetworkCredential(GloableProfData.PData.ProxyUsername, GloableProfData.PData.ProxyPassword);
                    }
                }
            }
            catch { }
            return proxy;
        }

        public static void CheckRamUsage()
        {
            if (availmem == 0)
            {
                Microsoft.VisualBasic.Devices.ComputerInfo ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
                availmem = ci.AvailablePhysicalMemory;
                availmem = availmem / (1024 * 1024);
            }

            if (timesToCheck++ >= 5)
            {
                if (ramCheckerThread == null || !ramCheckerThread.IsAlive)
                {
                    ramCheckerThread = new System.Threading.Thread(() =>
                    {
                        double total = 0;
                        bool showedMSgBox = false;
                        foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("BrowserAndFeatures"))
                        {
                            var counter = new System.Diagnostics.PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
                            total += counter.RawValue / (1024 * 1024);
                            if ((availmem - total) < 350)
                            {
                                if (!showedMSgBox)
                                    Application.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        MessageBox.Show(
                                            "You have only " + (availmem - total) + "mb of ram space left please close down other applications" +
                                            " to free up ram before continuing. Or refrain from openning more tabs, keep in mind" +
                                            " you will risk your computer and Browseo's performance.");
                                    });
                                showedMSgBox = true;
                            }
                        }
                    });
                    ramCheckerThread.Start();
                }
                timesToCheck = 0;
            }
        }

        public static string GetDefultHomePage()
        {
            string dirpathToHomePage = Path.Combine(GetBaseDir(), "DefaultHomePage");
            if (Directory.Exists(dirpathToHomePage))
            {
                string filePathForHomePage = Path.Combine(GetBaseDir(), "DefaultHomePage", "homePage.txt");
                if (File.Exists(filePathForHomePage))
                {
                    return File.ReadAllText(filePathForHomePage);
                }
            }
            return "";
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static void SetMozIds()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    MozscapeAPI.mozId = "";
                    MozscapeAPI.mozSecret = "";
                    string mozDir = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Prospector", "ApiKeys");
                    if (System.IO.Directory.Exists(mozDir))
                    {

                        string filePath = System.IO.Path.Combine(mozDir, "moz.txt");
                        if (System.IO.File.Exists(filePath))
                        {
                            string[] fileText = File.ReadAllText(filePath).Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                            MozscapeAPI.mozId = fileText[0];
                            MozscapeAPI.mozSecret = fileText[1];
                        }
                    }
                }
                catch { }
            }).Start();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetOpenClipboardWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetWindowText(int hwnd, StringBuilder text, int count);

        public static string getOpenClipboardWindowText()
        {
            IntPtr hwnd = GetOpenClipboardWindow();
            StringBuilder sb = new StringBuilder(501);
            GetWindowText(hwnd.ToInt32(), sb, 500);
            return sb.ToString();
            // example:
            // skype_plugin_core_proxy_window: 02490E80
        }
        public static void SetClipboardText(string text)
        {
            try
            {
                Clipboard.Clear();

                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                try
                {
                    string msg = ex.Message;
                    msg += Environment.NewLine;
                    msg += Environment.NewLine;
                    msg += "The problem:";
                    msg += Environment.NewLine;
                    msg += getOpenClipboardWindowText();
                    MessageBox.Show(msg);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }
    }
}
