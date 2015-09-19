using PData.FilesReader;
using ProjectsList.Models;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Organiser.Common.Classes
{
    public class MyFilesDatabase
    {
        public const string SPLITTER = "{[:]}";

        public static string GetBaseDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        }

        public static List<ProjectData> GetProjects()
        {
            List<ProjectData> projects = new List<ProjectData>();
            string path = Path.Combine(GetBaseDir(), "Projects");
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    string sitesFilePath = Path.Combine(dir.FullName, "ProjectData.ini");
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
                    ProjectData proj = new ProjectData()
                    {
                        ProjectName = dir.Name,
                        ProjDir = dir.FullName,
                        PersonData = profile
                    };
                    projects.Add(proj);
                }
                return projects;
            }
            return projects;
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
                    if (ProjName.Contains("_tier_"))
                    {
                        ProjName = ProjName.Replace("_tier_", "");
                    }
                    //UserData
                    //checkif is profile
                    bool add = true;
                    foreach (FileInfo file in dirInfo.GetFiles())
                    {
                        if(file.Name.Contains("UserData"))
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        projects.Add(new KeyValuePair<string, string>(ProjName, dirInfo.FullName));
                        // Resursive call for each subdirectory.
                        WalkDirectoryTree(dirInfo, ref projects);
                    }
                }
            }
        }

        public static void CreatProject(PersonData pdata)
        {
            try
            {
                string path = Path.Combine(GetBaseDir(), "Projects\\" + pdata.ProjectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                IniFile fileWrighter = new IniFile(Path.Combine(path, "ProjectData.ini"));
                fileWrighter.IniWriteValue("Data", "ProjectName", pdata.ProjectName);
                fileWrighter.IniWriteValue("Data", "ProfileName", pdata.ProfileName);
                fileWrighter.IniWriteValue("Data", "FirstName", pdata.FirstName);
                fileWrighter.IniWriteValue("Data", "LastName", pdata.LastName);
                fileWrighter.IniWriteValue("Data", "Email", pdata.Email);
                fileWrighter.IniWriteValue("Data", "Password", pdata.Password);
                fileWrighter.IniWriteValue("Data", "Username", pdata.Username);
                fileWrighter.IniWriteValue("Data", "ProxyIP", pdata.ProxyIP);
                fileWrighter.IniWriteValue("Data", "ProxyPort", pdata.ProxyPort);
                fileWrighter.IniWriteValue("Data", "ProxyUsername", pdata.ProxyUsername);
                fileWrighter.IniWriteValue("Data", "ProxyPassword", pdata.ProxyPassword);
                fileWrighter.IniWriteValue("Data", "PhoneNumber", pdata.PhoneNumber);
                fileWrighter.IniWriteValue("Data", "Sex", "" + pdata.CmbSelectedIndexSex);
                fileWrighter.IniWriteValue("Data", "BirthdayDay", "" + pdata.CmbSelectedIndexDay);
                fileWrighter.IniWriteValue("Data", "BirthdayMonth", "" + pdata.CmbSelectedIndexMonth);
                fileWrighter.IniWriteValue("Data", "BirthdayYear", "" + pdata.BirthdayYear);
                fileWrighter.IniWriteValue("Data", "Street", "" + pdata.Street);
                fileWrighter.IniWriteValue("Data", "City", "" + pdata.City);
                fileWrighter.IniWriteValue("Data", "State", "" + pdata.State);
                fileWrighter.IniWriteValue("Data", "Zip", "" + pdata.Zip);
                fileWrighter.IniWriteValue("Data", "Country", "" + pdata.Country);
                fileWrighter.IniWriteValue("Data", "Notes", "" + pdata.Notes);
            }
            catch { MessageBox.Show("Project not saved."); }
        }

        public static void CreatSubProjectUser(PersonData pdata)
        {
            try
            {
                string path = Path.Combine(GetBaseDir(), "Projects\\" + pdata.ProjectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string userPath = Path.Combine(GetBaseDir(), "Projects\\" + pdata.ProjectName + "\\" + pdata.ProfileName);
                if (!Directory.Exists(userPath))
                    Directory.CreateDirectory(userPath);

                IniFile fileWrighter = new IniFile(Path.Combine(userPath, "UserData.ini"));
                fileWrighter.IniWriteValue("Data", "ProjectName", pdata.ProjectName);
                fileWrighter.IniWriteValue("Data", "ProfileName", pdata.ProfileName);
                fileWrighter.IniWriteValue("Data", "FirstName", pdata.FirstName);
                fileWrighter.IniWriteValue("Data", "LastName", pdata.LastName);
                fileWrighter.IniWriteValue("Data", "Email", pdata.Email);
                fileWrighter.IniWriteValue("Data", "Password", pdata.Password);
                fileWrighter.IniWriteValue("Data", "Username", pdata.Username);
                fileWrighter.IniWriteValue("Data", "ProxyIP", pdata.ProxyIP);
                fileWrighter.IniWriteValue("Data", "ProxyPort", pdata.ProxyPort);
                fileWrighter.IniWriteValue("Data", "ProxyUsername", pdata.ProxyUsername);
                fileWrighter.IniWriteValue("Data", "ProxyPassword", pdata.ProxyPassword);
                fileWrighter.IniWriteValue("Data", "PhoneNumber", pdata.PhoneNumber);
                fileWrighter.IniWriteValue("Data", "Sex", "" + pdata.CmbSelectedIndexSex);
                fileWrighter.IniWriteValue("Data", "BirthdayDay", "" + pdata.CmbSelectedIndexDay);
                fileWrighter.IniWriteValue("Data", "BirthdayMonth", "" + pdata.CmbSelectedIndexMonth);
                fileWrighter.IniWriteValue("Data", "BirthdayYear", "" + pdata.BirthdayYear);
                fileWrighter.IniWriteValue("Data", "Street", "" + pdata.Street);
                fileWrighter.IniWriteValue("Data", "City", "" + pdata.City);
                fileWrighter.IniWriteValue("Data", "State", "" + pdata.State);
                fileWrighter.IniWriteValue("Data", "Zip", "" + pdata.Zip);
                fileWrighter.IniWriteValue("Data", "Country", "" + pdata.Country);
                fileWrighter.IniWriteValue("Data", "Notes", "" + pdata.Notes);
            }
            catch { MessageBox.Show("Project not saved."); }
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

        public static List<string> GetProjectSites(ProjectData project)
        {
            List<string> currentList = currentList = new List<string>();
            string path = Path.Combine(project.ProjDir, "sites.txt");

            if (!File.Exists(path)) return currentList;

            foreach (string site in File.ReadAllLines(path))
            {
                string[] lineArr = site.Split(new string[] { "{:|:}" }, StringSplitOptions.None);
                currentList.Add(lineArr[1]);
            }
            return currentList;
        }

        public static void WrighProjectSitesData(ProjectData project)
        {
            if (project.Sites == null) return;

            //if (project.Sites.Count <= 1)
            //    foreach (string site in GetSubProjects(project))
            //        project.Sites.Add(site);

            FileStream fs = new FileStream(Path.Combine(project.ProjDir, "sites.txt"), FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach (string site in project.Sites)
                {
                    sw.WriteLine("Site {:|:}" + site);
                }
            }
            fs.Close();
        }

        public static void DeleteProject(ProjectData proj)
        {
            try
            {
                if (Directory.Exists(proj.ProjDir))
                    Directory.Delete(proj.ProjDir, true);
            }
            catch { }
        }

        public static void FlipCache(string oldProjectName, string newProjectName, bool flip)
        {
            string windowsPath = @"Microsoft\Windows\Temporary Internet Files";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 2, 9200, 0))
                windowsPath = @"Microsoft\Windows\INetCache";

            string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), windowsPath);

            if (Directory.Exists(cachePath))
            {
                if (flip)
                {
                    string destDir = Path.Combine(GetBaseDir(), "Caches\\" + oldProjectName);
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    DirectoryCopy(cachePath, destDir, true);
                }

                //DynamicBrowser.IECache.ClearCache();
                foreach (string file in Directory.GetFiles(cachePath))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }

                string newDir = Path.Combine(GetBaseDir(), "Caches\\" + newProjectName);
                if (!Directory.Exists(newDir))
                    return;
                else
                    DirectoryCopy(newDir, cachePath, true);
            }
            else
            {
                // DynamicBrowser.IECache.ClearCache();
            }
        }

        public static void WrightCache(string projname)
        {
            string windowsPath = @"Microsoft\Windows\Temporary Internet Files";
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 2, 9200, 0))
                windowsPath = @"Microsoft\Windows\INetCache";

            string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), windowsPath);

            if (Directory.Exists(cachePath))
            {
                string destDir = Path.Combine(GetBaseDir(), "Caches\\" + projname);
                if (!Directory.Exists(projname))
                    Directory.CreateDirectory(projname);

                DirectoryCopy(cachePath, destDir, true);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                try
                {
                    file.CopyTo(temppath, true);
                }
                catch { }
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void CreateBrowserPersonData(PersonData pdata)
        {
            string path = Path.Combine(GetBaseDir(), "Temp");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            IniFile fileWrighter = new IniFile(Path.Combine(path, "BrowserConfig.ini"));
            fileWrighter.IniWriteValue("Data", "ProjectName", pdata.ProjectName);
            fileWrighter.IniWriteValue("Data", "FirstName", pdata.FirstName);
            fileWrighter.IniWriteValue("Data", "LastName", pdata.LastName);
            fileWrighter.IniWriteValue("Data", "Email", pdata.Email);
            fileWrighter.IniWriteValue("Data", "Password", pdata.Password);
            fileWrighter.IniWriteValue("Data", "Username", pdata.Username);
            fileWrighter.IniWriteValue("Data", "ProxyIP", pdata.ProxyIP);
            fileWrighter.IniWriteValue("Data", "ProxyPort", pdata.ProxyPort);
            fileWrighter.IniWriteValue("Data", "ProxyUsername", pdata.ProxyUsername);
            fileWrighter.IniWriteValue("Data", "ProxyPassword", pdata.ProxyPassword);
            fileWrighter.IniWriteValue("Data", "PhoneNumber", pdata.PhoneNumber);
            fileWrighter.IniWriteValue("Data", "Sex", "" + pdata.CmbSelectedIndexSex);
            fileWrighter.IniWriteValue("Data", "BirthdayDay", "" + pdata.CmbSelectedIndexDay);
            fileWrighter.IniWriteValue("Data", "BirthdayMonth", "" + pdata.CmbSelectedIndexMonth);
            fileWrighter.IniWriteValue("Data", "BirthdayYear", "" + pdata.BirthdayYear);
            fileWrighter.IniWriteValue("Data", "Street", "" + pdata.Street);
            fileWrighter.IniWriteValue("Data", "City", "" + pdata.City);
            fileWrighter.IniWriteValue("Data", "State", "" + pdata.State);
            fileWrighter.IniWriteValue("Data", "Zip", "" + pdata.Zip);
            fileWrighter.IniWriteValue("Data", "Country", "" + pdata.Country);
            fileWrighter.IniWriteValue("Data", "Notes", "" + pdata.Notes);
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

        #region cookie data
        public static List<string> GetSites()
        {
            List<string> sites = new List<string>();

            string filePath = Path.Combine(GetBaseDir(), "VisitedSites\\SitesLog.txt");
            if (File.Exists(filePath))
            {
                try
                {
                    foreach (var item in File.ReadAllLines(filePath))
                    {
                        sites.Add(item);
                    }
                }
                catch { }
            }

            return sites;
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
                    foreach (var item in GetSites())
                    {
                        if (item == site)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        File.AppendAllText(filePath, site + Environment.NewLine);
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
    }
}
