using Microsoft.Win32;
using PData.FilesReader;
using ProjectsList.Models;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.IO;
using Delimon.Win32.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Organiser.Common.Classes
{
    public class BrowserSettimgs
    {
      //8514oem
      //AR BERKLEY
      //AR BLANCA
      //AR BONNIE
      //AR CARTER
      //AR CENA
      //AR CHRISTY
      //AR DARLING
      //AR DECODE
      //AR DELANEY
      //AR DESTINE
      //AR ESSENCE
      //AR HERMANN
      //AR JULIAN
      //Arial
      //Arial Black
      //Arimo
      //Calibri
      //Calibri Light
      //Cambria
      //Cambria Math
      //Candara
      //Comic Sans MS
      //Consolas
      //Constantia
      //Corbel
      //Courier
      //Courier New
      //DejaVu Sans
      //DejaVu Sans Condensed
      //DejaVu Sans Light
      //DejaVu Sans Mono
      //DejaVu Serif
      //DejaVu Serif Condensed
      //DengXian
      //Ebrima
      //Fixedsys
      //Franklin Gothic Medium
      //Gabriola
      //Gadugi
      //Gentium Basic
      //Gentium Book Basic
      //Georgia
      //Impact
      //Javanese Text
      //Leelawadee UI
      //Leelawadee UI Semilight
      //Lucida Console
      //Lucida Sans Unicode
      //MS Gothic
      //MS PGothic
      //MS Sans Serif
      //MS Serif
      //MS UI Gothic
      //MV Boli
      //Malgun Gothic
      //Malgun Gothic Semilight
      //Marlett
      //Microsoft Himalaya
      //Microsoft JhengHei
      //Microsoft JhengHei Light
      //Microsoft JhengHei UI
      //Microsoft JhengHei UI Light
      //Microsoft MHei
      //Microsoft NeoGothic
      //Microsoft New Tai Lue
      //Microsoft PhagsPa
      //Microsoft Sans Serif
      //Microsoft Tai Le
      //Microsoft YaHei
      //Microsoft YaHei Light
      //Microsoft YaHei UI
      //Microsoft YaHei UI Light
      //Microsoft Yi Baiti
      //MingLiU-ExtB
      //MingLiU_HKSCS-ExtB
      //Modern
      //Mongolian Baiti
      //Myanmar Text
      //NSimSun
      //Nirmala UI
      //Nirmala UI Semilight
      //OpenSymbol
      //PMingLiU-ExtB
      //Palatino Linotype
      //Roman
      //Script
      //Segoe MDL2 Assets
      //Segoe Print
      //Segoe Script
      //Segoe UI
      //Segoe UI Black
      //Segoe UI Emoji
      //Segoe UI Historic
      //Segoe UI Light
      //Segoe UI Semibold
      //Segoe UI Semilight
      //Segoe UI Symbol
      //Segoe WP
      //Segoe WP Black
      //Segoe WP Light
      //Segoe WP SemiLight
      //Segoe WP Semibold
      //SimSun
      //SimSun-ExtB
      //Sitka Banner
      //Sitka Display
      //Sitka Heading
      //Sitka Small
      //Sitka Subheading
      //Sitka Text
      //Small Fonts
      //Sylfaen
      //Symbol
      //System
      //Tahoma
      //TeamViewer11
      //Terminal
      //Times New Roman
      //Trebuchet MS
      //Verdana
      //Webdings
      //Wingdings
      //Yu Gothic
      //Yu Gothic Light
      //Yu Gothic Medium
      //Yu Gothic UI
      //Yu Gothic UI Light
      //Yu Gothic UI Semibold
      //Yu Gothic UI Semilight
      
      //UTF-8
      //Windows-1252
      //UTF-16LE
      //Windows-1256
      //ISO-8859-6
      //ISO-8859-4
      //ISO-8859-13
      //Windows-1257
      //ISO-8859-14
      //ISO-8859-2
      //Windows-1250
      //GBK
      //gb18030
      //Big5
      //ISO-8859-5
      //Windows-1251
      //KOI8-R
      //KOI8-U
      //IBM866
      //ISO-8859-7
      //Windows-1253
      //Windows-1255
      //ISO-8859-8-I
      //ISO-8859-8
      //Shift_JIS
      //EUC-JP
      //ISO-2022-JP
      //ISO-8859-10
      //ISO-8859-3
      //ISO-8859-15
      //Macintosh
     static BrowserSettimgs()
        {
            ResetDefaoultFonts();
        }
        public static void ResetDefaoultFonts()
        {
            BrowserSettimgs.SIFontStandard = BrowserSettimgs.AvailableFonts.IndexOf("Times New Roman");
            BrowserSettimgs.SIFontSerif = BrowserSettimgs.AvailableFonts.IndexOf("Times New Roman");
            BrowserSettimgs.SIFontSansSerif = BrowserSettimgs.AvailableFonts.IndexOf("Arial");
            BrowserSettimgs.SIFontFixedWidth = BrowserSettimgs.AvailableFonts.IndexOf("Consolas");
            BrowserSettimgs.DefaultFontSize = 16;
            BrowserSettimgs.MnimumFontSize = 0;
            BrowserSettimgs.SIFontEncodings = BrowserSettimgs.AvailableEncodeings.IndexOf("Windows-1252");
            AcceptLanguage = "en-US, en";
        }
        public static bool JavaEnabled = true;
        public static bool JavascriptEnabled = true;
        public static bool FlashEnabled = true;
        public static bool DoNotTrackEnabled = true; 
        public static bool SetSysDateEnabled = false;
        public static bool WebRTCEnabled = true;
        public static bool WebGLEnabled = true;

        public static string UserAgentFF = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
        public static string UserAgentChrome = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.170 Safari/537.36";
        public static string AcceptLanguage = "en-US, en";

        //public string UserAgent_Current = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
        public static string UserAgent_CurrentFFBuild = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";

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

        private static List<string> availableFonts;
        public static List<string> AvailableFonts
        {
            get
            {
                if(availableFonts == null)
                {
                    availableFonts = new List<string>();
                    availableFonts.Add("8514oem");
                    availableFonts.Add("AR BERKLEY");
                    availableFonts.Add("AR BLANCA");
                    availableFonts.Add("AR BONNIE");
                    availableFonts.Add("AR CARTER");
                    availableFonts.Add("AR CENA");
                    availableFonts.Add("AR CHRISTY");
                    availableFonts.Add("AR DARLING");
                    availableFonts.Add("AR DECODE");
                    availableFonts.Add("AR DELANEY");
                    availableFonts.Add("AR DESTINE");
                    availableFonts.Add("AR ESSENCE");
                    availableFonts.Add("AR HERMANN");
                    availableFonts.Add("AR JULIAN");
                    availableFonts.Add("Arial");
                    availableFonts.Add("Arial Black");
                    availableFonts.Add("Arimo");
                    availableFonts.Add("Calibri");
                    availableFonts.Add("Calibri Light");
                    availableFonts.Add("Cambria");
                    availableFonts.Add("Cambria Math");
                    availableFonts.Add("Candara");
                    availableFonts.Add("Comic Sans MS");
                    availableFonts.Add("Consolas");
                    availableFonts.Add("Constantia");
                    availableFonts.Add("Corbel");
                    availableFonts.Add("Courier");
                    availableFonts.Add("Courier New");
                    availableFonts.Add("DejaVu Sans");
                    availableFonts.Add("DejaVu Sans Condensed");
                    availableFonts.Add("DejaVu Sans Light");
                    availableFonts.Add("DejaVu Sans Mono");
                    availableFonts.Add("DejaVu Serif");
                    availableFonts.Add("DejaVu Serif Condensed");
                    availableFonts.Add("DengXian");
                    availableFonts.Add("Ebrima");
                    availableFonts.Add("Fixedsys");
                    availableFonts.Add("Franklin Gothic Medium");
                    availableFonts.Add("Gabriola");
                    availableFonts.Add("Gadugi");
                    availableFonts.Add("Gentium Basic");
                    availableFonts.Add("Gentium Book Basic");
                    availableFonts.Add("Georgia");
                    availableFonts.Add("Impact");
                    availableFonts.Add("Javanese Text");
                    availableFonts.Add("Leelawadee UI");
                    availableFonts.Add("Leelawadee UI Semilight");
                    availableFonts.Add("Lucida Console");
                    availableFonts.Add("Lucida Sans Unicode");
                    availableFonts.Add("MS Gothic");
                    availableFonts.Add("MS PGothic");
                    availableFonts.Add("MS Sans Serif");
                    availableFonts.Add("MS Serif");
                    availableFonts.Add("MS UI Gothic");
                    availableFonts.Add("MV Boli");
                    availableFonts.Add("Malgun Gothic");
                    availableFonts.Add("Malgun Gothic Semilight");
                    availableFonts.Add("Marlett");
                    availableFonts.Add("Microsoft Himalaya");
                    availableFonts.Add("Microsoft JhengHei");
                    availableFonts.Add("Microsoft JhengHei Light");
                    availableFonts.Add("Microsoft JhengHei UI");
                    availableFonts.Add("Microsoft JhengHei UI Light");
                    availableFonts.Add("Microsoft MHei");
                    availableFonts.Add("Microsoft NeoGothic");
                    availableFonts.Add("Microsoft New Tai Lue");
                    availableFonts.Add("Microsoft PhagsPa");
                    availableFonts.Add("Microsoft Sans Serif");
                    availableFonts.Add("Microsoft Tai Le");
                    availableFonts.Add("Microsoft YaHei");
                    availableFonts.Add("Microsoft YaHei Light");
                    availableFonts.Add("Microsoft YaHei UI");
                    availableFonts.Add("Microsoft YaHei UI Light");
                    availableFonts.Add("Microsoft Yi Baiti");
                    availableFonts.Add("MingLiU-ExtB");
                    availableFonts.Add("MingLiU_HKSCS-ExtB");
                    availableFonts.Add("Modern");
                    availableFonts.Add("Mongolian Baiti");
                    availableFonts.Add("Myanmar Text");
                    availableFonts.Add("NSimSun");
                    availableFonts.Add("Nirmala UI");
                    availableFonts.Add("Nirmala UI Semilight");
                    availableFonts.Add("OpenSymbol");
                    availableFonts.Add("PMingLiU-ExtB");
                    availableFonts.Add("Palatino Linotype");
                    availableFonts.Add("Roman");
                    availableFonts.Add("Script");
                    availableFonts.Add("Segoe MDL2 Assets");
                    availableFonts.Add("Segoe Print");
                    availableFonts.Add("Segoe Script");
                    availableFonts.Add("Segoe UI");
                    availableFonts.Add("Segoe UI Black");
                    availableFonts.Add("Segoe UI Emoji");
                    availableFonts.Add("Segoe UI Historic");
                    availableFonts.Add("Segoe UI Light");
                    availableFonts.Add("Segoe UI Semibold");
                    availableFonts.Add("Segoe UI Semilight");
                    availableFonts.Add("Segoe UI Symbol");
                    availableFonts.Add("Segoe WP");
                    availableFonts.Add("Segoe WP Black");
                    availableFonts.Add("Segoe WP Light");
                    availableFonts.Add("Segoe WP SemiLight");
                    availableFonts.Add("Segoe WP Semibold");
                    availableFonts.Add("SimSun");
                    availableFonts.Add("SimSun-ExtB");
                    availableFonts.Add("Sitka Banner");
                    availableFonts.Add("Sitka Display");
                    availableFonts.Add("Sitka Heading");
                    availableFonts.Add("Sitka Small");
                    availableFonts.Add("Sitka Subheading");
                    availableFonts.Add("Sitka Text");
                    availableFonts.Add("Small Fonts");
                    availableFonts.Add("Sylfaen");
                    availableFonts.Add("Symbol");
                    availableFonts.Add("System");
                    availableFonts.Add("Tahoma");
                    availableFonts.Add("TeamViewer11");
                    availableFonts.Add("Terminal");
                    availableFonts.Add("Times New Roman");
                    availableFonts.Add("Trebuchet MS");
                    availableFonts.Add("Verdana");
                    availableFonts.Add("Webdings");
                    availableFonts.Add("Wingdings");
                    availableFonts.Add("Yu Gothic");
                    availableFonts.Add("Yu Gothic Light");
                    availableFonts.Add("Yu Gothic Medium");
                    availableFonts.Add("Yu Gothic UI");
                    availableFonts.Add("Yu Gothic UI Light");
                    availableFonts.Add("Yu Gothic UI Semibold");
                    availableFonts.Add("Yu Gothic UI Semilight");
                }

                return availableFonts;
            }
        }

        public static int SIFontStandard { get; set; }
        public static int SIFontSerif { get; set; }
        public static int SIFontSansSerif { get; set; }
        public static int SIFontFixedWidth { get; set; }
        public static bool HideFonts { get; set; }

        public static int DefaultFontSize { get; set; }
        public static int MnimumFontSize { get; set; }

        private static List<string> availableEncodings;

        public static List<string> AvailableEncodeings
        {
            get
            {
                if (availableEncodings == null)
                {
                    availableEncodings = new List<string>();
                    availableEncodings.Add("UTF-8");
                    availableEncodings.Add("Windows-1252");
                    availableEncodings.Add("UTF-16LE");
                    availableEncodings.Add("Windows-1256");
                    availableEncodings.Add("ISO-8859-6");
                    availableEncodings.Add("ISO-8859-4");
                    availableEncodings.Add("ISO-8859-13");
                    availableEncodings.Add("Windows-1257");
                    availableEncodings.Add("ISO-8859-14");
                    availableEncodings.Add("ISO-8859-2");
                    availableEncodings.Add("Windows-1250");
                    availableEncodings.Add("GBK");
                    availableEncodings.Add("gb18030");
                    availableEncodings.Add("Big5");
                    availableEncodings.Add("ISO-8859-5");
                    availableEncodings.Add("Windows-1251");
                    availableEncodings.Add("KOI8-R");
                    availableEncodings.Add("KOI8-U");
                    availableEncodings.Add("IBM866");
                    availableEncodings.Add("ISO-8859-7");
                    availableEncodings.Add("Windows-1253");
                    availableEncodings.Add("Windows-1255");
                    availableEncodings.Add("ISO-8859-8-I");
                    availableEncodings.Add("ISO-8859-8");
                    availableEncodings.Add("Shift_JIS");
                    availableEncodings.Add("EUC-JP");
                    availableEncodings.Add("ISO-2022-JP");
                    availableEncodings.Add("ISO-8859-10");
                    availableEncodings.Add("ISO-8859-3");
                    availableEncodings.Add("ISO-8859-15");
                    availableEncodings.Add("Macintosh");
                }

                return availableEncodings;
            }
        }

        public static int SIFontEncodings { get; set; }
    }

    public class MyFilesDatabase
    {
        public class Path
        {
            public static string Combine(params string[] path)
            {
                string toreturn = "";
                foreach (var item in path)
                {
                    toreturn += item + "\\";
                }
                toreturn = toreturn.Replace("\\\\", "\\");
                if (toreturn.EndsWith("\\")) toreturn = toreturn.Remove(toreturn.LastIndexOf("\\"));

                return toreturn;
            }
        }
        public class File : Delimon.Win32.IO.File
        {
            new public static void WriteAllText(string path, string contents)
            {
                if (File.Exists(path)) File.Delete(path);

                Delimon.Win32.IO.File.WriteAllText(path, contents);
            }

            new public static void WriteAllText(string path, string contents, Encoding enc)
            {
                if (File.Exists(path)) File.Delete(path);

                Delimon.Win32.IO.File.WriteAllText(path, contents, enc);
            }

            new public static void WriteAllLines(string path, string[] contents)
            {
                WriteAllLines(path, contents.ToList());
            }

            public static void WriteAllLines(string path, IEnumerable<string> contents)
            {
                string content = "";
                foreach (var line in contents)
                {
                    content += line + Environment.NewLine;
                }
                WriteAllText(path, content);
            }

            new public static bool Exists(string path)
            {
                return Delimon.Win32.IO.File.Exists(path);
            }
        }
        public class Directory : Delimon.Win32.IO.Directory
        {
            new public static void CreateDirectory(string path)
            {
                string thenewDirectorys = path;
                thenewDirectorys = thenewDirectorys.Replace(GetBaseDir(), "");
                var dirs = thenewDirectorys.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                string appendedDirs = Path.Combine(GetBaseDir());
                foreach (var dir in dirs)
                {
                    appendedDirs = appendedDirs + "\\" + dir;
                    if (!Directory.Exists(appendedDirs)) Delimon.Win32.IO.Directory.CreateDirectory(appendedDirs);
                }
            }
        }
        public const string SPLITTER = "{[:]}";

        public static bool CanSeeProxys = false;

        static System.Threading.Thread ramCheckerThread;
        static ulong availmem = 0;
        static int timesToCheck = 0;

        public static string GetBaseDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        }

        public static string GetBaseProjectsDir()
        {
            return GetBaseDir() + "\\Projects";
        }

        #region projects
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

        public static List<KeyValuePair<string, string>> GetAllProjectsAndDirs(bool all)
        {
            List<KeyValuePair<string, string>> projects = new List<KeyValuePair<string, string>>();
            string path = Path.Combine(GetBaseDir(), "Projects");

            if (Directory.Exists(path))
            {
                WalkDirectoryTree(new DirectoryInfo(path),all, ref projects);
                return projects;
            }
            
            return projects;
        }

        public static void WalkDirectoryTree(DirectoryInfo root,bool all, ref List<KeyValuePair<string, string>> projects)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            try
            {
                files = root.GetFiles();
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
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    string ProjName = dirInfo.Name;

                    if (!dirInfo.GetFiles().Any(f => f.Name.Contains("UserData")) && 
                        (!ProjName.Contains("_folder") || (all && ProjName.Contains("_folder"))))
                    {
                        if (dirInfo.Parent != null && dirInfo.Parent.Name != "Projects" && !ProjName.Contains("_tier_") && !ProjName.Contains("_folder_")) continue;
                        if (ProjName.StartsWith("_tier_")) ProjName = ProjName.Substring(6);
                        if (ProjName.StartsWith("_folder_")) ProjName = ProjName.Substring(8);

                        projects.Add(new KeyValuePair<string, string>(ProjName, dirInfo.FullName));
                    }

                    WalkDirectoryTree(dirInfo,all, ref projects);
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

        public static string GetFaviconIfExists(string authority)
        {
            string dir = Path.Combine(GetBaseDir(), "FaviconsShared");
            if (!Directory.Exists(dir)) return "";

            string file = Path.Combine(dir, authority + ".ico");
            if (!File.Exists(file)) return "";

            return file;
        }

        public static string SaveImageFromBytes(byte[] bytes, string authority)
        {
            string dir = Path.Combine(GetBaseDir(), "FaviconsShared");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string file = Path.Combine(dir, authority + ".ico");
            if (File.Exists(file)) return file;

            try
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(bytes)))
                {
                    image.Save(file);
                }
            }
            catch { return ""; }

            return file;
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

        public static List<PersonData> GetAllProfiles()
        {
            List<PersonData> allProfiles = new List<PersonData>();
            WalkDirectoryTreeGetAllProfiles(new DirectoryInfo(Path.Combine(GetBaseDir(), "Projects")), allProfiles);
            return allProfiles;
        }
        public static void WalkDirectoryTreeGetAllProfiles(DirectoryInfo root, List<PersonData> allProfiles)
        {
            DirectoryInfo[] subDirs = null;

            string filepath = Path.Combine(root.FullName, "UserData.ini");
            if (!File.Exists(filepath)) filepath = filepath.Replace("UserData.ini", "ProjectData.ini");
            if (File.Exists(filepath))
            {
                PersonData pdata = GetSubProjectPersonData(filepath);
                pdata.ProjectDir = root.FullName;
                allProfiles.Add(pdata);
            }

            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories();
            if (subDirs != null)
            {
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTreeGetAllProfiles(dirInfo, allProfiles);
                }
            }
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
                try
                {
                    profile.InPBNVault = Convert.ToBoolean(ini.IniReadValue("Data", "InVault"));
                    profile.SIPBNType = Convert.ToInt32(ini.IniReadValue("Data", "SIPBNType"));
                }
                catch
                { }
                try
                {
                    profile.InMonney = Convert.ToBoolean(ini.IniReadValue("Data", "InMoney"));
                }
                catch
                { }
                try
                {
                    profile.BIADefault = Convert.ToBoolean(ini.IniReadValue("Data", "BIADefault"));
                }
                catch { }
            }
            catch { }
            return profile;
        }

        public static void ReWrightProjData(PersonData pdata, string dir)
        {
            try
            {
                string sitesFilePath = dir;
                if (!dir.Contains(".ini"))
                    sitesFilePath = Path.Combine(dir, "UserData.ini");
                if (!File.Exists(sitesFilePath))
                    sitesFilePath = sitesFilePath.Replace("UserData.ini", "ProjectData.ini");

                IniFile fileWrighter = new IniFile(sitesFilePath);
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
                fileWrighter.IniWriteValue("Data", "WebAddress", pdata.WebAddress);
                fileWrighter.IniWriteValue("Data", "Notes", "" + pdata.Notes);
                fileWrighter.IniWriteValue("Data", "InVault", "" + pdata.InPBNVault);
                fileWrighter.IniWriteValue("Data", "SIPBNType", "" + pdata.SIPBNType);
                fileWrighter.IniWriteValue("Data", "InMoney", "" + pdata.InMonney);
            }
            catch
            {
            }
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
        #endregion
        #region sessions
        public static void DeleteSession(string projectName, bool isff = false)
        {
            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if (isff) directory = Path.Combine(GetBaseDir(), "FFSavedSessions", projectName);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }

        public static void SaveSession(string projectName, List<string> links, bool isff = false)
        {
            links.Add(
                BrowserSettimgs.FlashEnabled +
                "," + BrowserSettimgs.JavaEnabled +
                "," + BrowserSettimgs.JavascriptEnabled +
                "," + BrowserSettimgs.SetSysDateEnabled +
                "," + BrowserSettimgs.SITimeZone+
                "," + BrowserSettimgs.DoNotTrackEnabled+
                "," + BrowserSettimgs.WebRTCEnabled +
                "," + BrowserSettimgs.UserAgentChrome.Replace(",", MyFilesDatabase.SPLITTER) +
                "," + BrowserSettimgs.UserAgentFF.Replace(",", MyFilesDatabase.SPLITTER) +
                "," + BrowserSettimgs.SIFontStandard +
                "," + BrowserSettimgs.SIFontSerif +
                "," + BrowserSettimgs.SIFontSansSerif +
                "," + BrowserSettimgs.SIFontFixedWidth +
                "," + BrowserSettimgs.DefaultFontSize +
                "," + BrowserSettimgs.MnimumFontSize +
                "," + BrowserSettimgs.SIFontEncodings +
                "," + BrowserSettimgs.WebGLEnabled +
                "," + BrowserSettimgs.HideFonts+
                "," + BrowserSettimgs.AcceptLanguage.Replace(",", MyFilesDatabase.SPLITTER));

            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if(isff) directory = Path.Combine(GetBaseDir(), "FFSavedSessions", projectName);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, "sites.txt");    
            File.WriteAllLines(filePath, links.ToArray());
        }

        public static string GeChromeAgentRealQuick(string projectName)
        {
            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if (!Directory.Exists(directory)) return BrowserSettimgs.UserAgentChrome;

            string filePath = Path.Combine(directory, "sites.txt");
            if (!File.Exists(filePath)) return BrowserSettimgs.UserAgentChrome;

            try
            {
                List<string> fileLines = File.ReadAllLines(filePath).ToList();
                if (fileLines.Count > 0)
                {
                    fileLines.RemoveAll(line => string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line));
                    string lastLine = fileLines[fileLines.Count - 1];
                    if (lastLine.Contains(","))
                    {
                        string[] browserSettings = lastLine.Split(',');
                        if (browserSettings.Length > 7)
                        {
                            BrowserSettimgs.UserAgentChrome = browserSettings[7].Replace(MyFilesDatabase.SPLITTER, ",");
                        }
                    }
                }
            }
            catch
            {
            }

            return BrowserSettimgs.UserAgentChrome;
        }

        public static List<string> GetSavedSesstion(string projectName, bool isff = false)
        {
            string directory = Path.Combine(GetBaseDir(), "SavedSessions", projectName);
            if (isff) directory = Path.Combine(GetBaseDir(), "FFSavedSessions", projectName);

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
                        if (browserSettings.Length > 5)
                        {
                            BrowserSettimgs.DoNotTrackEnabled = Convert.ToBoolean(browserSettings[5]);
                        }
                        if (browserSettings.Length > 6)
                        {
                            BrowserSettimgs.WebRTCEnabled = Convert.ToBoolean(browserSettings[6]);
                        }
                        if (!isff && browserSettings.Length > 7)
                        {
                            BrowserSettimgs.UserAgentChrome = browserSettings[7].Replace(MyFilesDatabase.SPLITTER, ",");
                        }
                        if (isff && browserSettings.Length > 8)
                        {
                            BrowserSettimgs.UserAgentFF = browserSettings[8].Replace(MyFilesDatabase.SPLITTER, ",");
                        }
                        if (browserSettings.Length > 9)
                        {
                            BrowserSettimgs.SIFontStandard = Convert.ToInt32(browserSettings[9]);
                            BrowserSettimgs.SIFontSerif = Convert.ToInt32(browserSettings[10]);
                            BrowserSettimgs.SIFontSansSerif = Convert.ToInt32(browserSettings[11]);
                            BrowserSettimgs.SIFontFixedWidth = Convert.ToInt32(browserSettings[12]);
                            BrowserSettimgs.DefaultFontSize = Convert.ToInt32(browserSettings[13]);
                            BrowserSettimgs.MnimumFontSize = Convert.ToInt32(browserSettings[14]);
                            BrowserSettimgs.SIFontEncodings = Convert.ToInt32(browserSettings[15]);
                            BrowserSettimgs.WebGLEnabled = Convert.ToBoolean(browserSettings[16]);
                        }
                        if (browserSettings.Length > 17)
                        {
                            BrowserSettimgs.HideFonts = Convert.ToBoolean(browserSettings[17]);
                        }
                        if(browserSettings.Length > 18)
                        {
                            BrowserSettimgs.AcceptLanguage = browserSettings[18].Replace(MyFilesDatabase.SPLITTER, ",");
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
            try
            {
                if (fileLines.Count > 0)
                {
                    string lastLine = fileLines[fileLines.Count - 1];
                    if (lastLine.Contains(",") && !lastLine.Contains(".")) fileLines.RemoveAt(fileLines.Count - 1);
                }
            }
            catch { }
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
        static string rssFilename = "rssLinks.txt";

        public static void SaveRssFeedsSiteLinks(string[] links, PersonData profile, string tabTitle)
        {
            string directoryPath = Path.Combine(GetBaseDir(), "SavedRssLinks", profile.ProjectName, tabTitle);
            string filePath = Path.Combine(directoryPath, rssFilename);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            File.WriteAllLines(filePath, links);

            //File.WriteAllText(filePath, loadFeedsGrouped + Environment.NewLine);

            ////string[] splitLinks = links.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            //foreach (string link in links)
            //{
            //    File.AppendAllText(filePath, link + Environment.NewLine);    
            //}
        }



        public static List<string> GetRssFeedLinks(PersonData profile, string tabTitle)
        {
            return GetRssFeedLinks(profile.ProjectName, tabTitle);
        }

        public static List<string> GetRssFeedLinks(string projectname, string tabTitle)
        {
            List<string> returnedList = new List<string>();

            string directoryPath = Path.Combine(GetBaseDir(), "SavedRssLinks", projectname, tabTitle);
            string filePath = Path.Combine(directoryPath, rssFilename);

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


        public static int GetRemindersCount(string projectName)
        {
            string remindersDir = Path.Combine(GetBaseDir(), "TaskReminders", projectName);
            if (Directory.Exists(remindersDir))
            {
               return Directory.GetFiles(remindersDir).Length;
            }

            return 0;
        }

        public static List<string> GetRemindersText(string projectName)
        {
            string remindersDir = Path.Combine(GetBaseDir(), "TaskReminders", projectName);
            if (Directory.Exists(remindersDir))
            {
                List<string> jsonRemindersList = new List<string>();
                foreach (var f in new DirectoryInfo(remindersDir).GetFiles())
                {
                    jsonRemindersList.Add(File.ReadAllText(f.FullName));
                }

                return jsonRemindersList;
            }

            return null;
        }

        public static string DownloadImage(string url,string filepath="")
        {
            string saveFileFilename = filepath;
            bool openFile = saveFileFilename == "";
            if (openFile)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "Png files (*.png)|*.png|JPeg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                    sfd.FilterIndex = 0;
                    sfd.RestoreDirectory = true;
                    if (sfd.ShowDialog() != true) return;
                    saveFileFilename = sfd.FileName;
                });
            }
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Proxy = GetRequestsProxy();

                    byte[] data = webClient.DownloadData(url);

                    using (System.IO.MemoryStream mem = new System.IO.MemoryStream(data))
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
                    if(openFile) System.Diagnostics.Process.Start(fileInfo.DirectoryName);
                }
                catch { }

                return "";
            }
            catch (Exception ex)
            {
               if(openFile) MessageBox.Show("Failed to save image. " + ex.Message);
                return "Failed to save image. " + ex.Message;
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
            try
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
                            try
                            {
                                double total = 0;
                                bool showedMSgBox = false;
                                foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("BrowserModules"))
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
                            }
                            catch { }
                        });
                        ramCheckerThread.Start();
                    }
                    timesToCheck = 0;
                }
            }
            catch { }
        }

        public static string GetDefultHomePage()
        {
            string dirpathToHomePage = Path.Combine(GetBaseDir(), "DefaultHomePage");
            if (Directory.Exists(dirpathToHomePage))
            {
                string filePathForHomePage = Path.Combine(GetBaseDir(), "DefaultHomePage", "homePage.txt");
                if (File.Exists(filePathForHomePage))
                {
                    var lines = File.ReadAllLines(filePathForHomePage);
                    if (lines == null) return "";
                    var line = new Random().Next(0, lines.Length - 1);
                    return lines.Length > 0 && line <= lines.Length - 1 ? lines[line] : "";
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
        public static void SetClipboardText(string text, bool showException = true)
        {
            try
            {
                Clipboard.Clear();

                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                if (!showException) return;

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

        public static void SetUpPdaaFromPath(string projectPath)
        {
            string filepath = Path.Combine(projectPath, "ProjectData.ini");
            IniFile ini = new IniFile(filepath);
            GloableProfData.PData = GetUpPdaaFromPath(projectPath);
            try
            { 
                MyFilesDatabase.GetSavedSesstion(GloableProfData.PData.ProjectName);
            }
            catch { }
        }

        public static PersonData GetUpPdaaFromPath(string projectPath)
        {
            string filepath = Path.Combine(projectPath, "ProjectData.ini");
            IniFile ini = new IniFile(filepath);
            PersonData PData = new PersonData();
            try
            {
                PData.ProjectName = ini.IniReadValue("Data", "ProjectName");
                PData.ProfileName = ini.IniReadValue("Data", "ProfileName");
                PData.FirstName = ini.IniReadValue("Data", "FirstName");
                PData.LastName = ini.IniReadValue("Data", "LastName");
                PData.Email = ini.IniReadValue("Data", "Email");
                PData.Password = ini.IniReadValue("Data", "Password");
                PData.Username = ini.IniReadValue("Data", "Username");
                PData.ProxyIP = ini.IniReadValue("Data", "ProxyIP");
                PData.ProxyPort = ini.IniReadValue("Data", "ProxyPort");
                PData.ProxyUsername = ini.IniReadValue("Data", "ProxyUsername");
                PData.ProxyPassword = ini.IniReadValue("Data", "ProxyPassword");
                PData.PhoneNumber = ini.IniReadValue("Data", "PhoneNumber");
                PData.Street = ini.IniReadValue("Data", "Street");
                PData.City = ini.IniReadValue("Data", "City");
                PData.State = ini.IniReadValue("Data", "State");
                PData.Zip = ini.IniReadValue("Data", "Zip");
                PData.Country = ini.IniReadValue("Data", "Country");
                PData.WebAddress = ini.IniReadValue("Data", "WebAddress");
                PData.Notes = ini.IniReadValue("Data", "Notes");
                try
                {
                   PData.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                   PData.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                   PData.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                }
                catch { }
                PData.ProjectDir = projectPath;
                try
                {
                    PData.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }
            return PData;
        }

        #region imacros

        public static void SetUpImacroProfileInfo()
        {
            var datasourcePath = Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "BrowseoIA_DataSource", GloableProfData.PData.ProjectName);

            if (!Directory.Exists(datasourcePath)) Directory.CreateDirectory(datasourcePath);
            var pdataSource = GloableProfData.PData;
            if (!pdataSource.BIADefault && pdataSource.Profiles != null)
            {
                foreach (var p in pdataSource.Profiles)
                {
                    if(p.BIADefault)
                    {
                        pdataSource = p;
                        break;
                    }
                }
            }
            var datasourceFile = Path.Combine(datasourcePath, "ProjectProfileInfo.txt");
            var datatsourceText = pdataSource.ProjectName + "," +
                                                    pdataSource.ProfileName + "," +
                                                    pdataSource.FirstName + "," +
                                                    pdataSource.LastName + "," +
                                                    pdataSource.PhoneNumber + "," +
                                                    pdataSource.Username + "," +
                                                    pdataSource.Email + "," +
                                                    pdataSource.Password + "," +
                                                    (pdataSource.CmbSelectedIndexSex + 1).ToString() + "," +//1 male 2 female
                                                    (pdataSource.CmbSelectedIndexDay + 1).ToString() + "," + //actual day
                                                    (pdataSource.CmbSelectedIndexMonth + 1).ToString() + "," +//actual month
                                                    pdataSource.BirthdayYear.ToString() + "," + 
                                                    pdataSource.Street + "," +
                                                    pdataSource.City + "," +
                                                    pdataSource.State + "," +
                                                    pdataSource.Zip + "," +
                                                    pdataSource.Country + "," +
                                                    pdataSource.WebAddress + "," +
                                                    pdataSource.Notes;

            File.WriteAllText(datasourceFile, datatsourceText);
        }

        public static string GetBaseMacroDir()
        {
            string ddir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer\\MacroDefaultDir";
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);

            return ddir;
        }
        public static string GetBaseMacroDownloadDir()
        {
            string ddir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer\\MacroDefaultDir\\Downloads";
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);

            return ddir;
        }
        public static string GetBaseMacroDatasourcesDir()
        {
            string ddir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer\\MacroDefaultDir\\Datasources";
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);

            return ddir;
        }
        public static string GetBaseMacroSettingsDir()
        {
            string ddir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer\\MacroDefaultDir\\Settings";
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);

            return ddir;
        }
        public static string GetBaseMacroScriptsDir()
        {
            string ddir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer\\MacroDefaultDir\\Scripts";
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);

            return ddir;
        }

        public static void SaveImportedProjectsForMultyMacro(string projectName, string data)
        {
            string ddir = Path.Combine(GetBaseDir(), "MacrosMultyTabs", projectName);
            if (!Directory.Exists(ddir)) Directory.CreateDirectory(ddir);

            string fPath = Path.Combine(ddir, "savedProjects");
            File.WriteAllText(fPath, data);
        }

        public static string GEtImportedProjectsForMultyMacroData(string projectName)
        {
            string ddir = Path.Combine(GetBaseDir(), "MacrosMultyTabs", projectName);
            if (!Directory.Exists(ddir)) return "";

            string fPath = Path.Combine(ddir, "savedProjects");
            if (!File.Exists(fPath)) return "";

            return File.ReadAllText(fPath);
        }

        //[DllImport("user32.dll")]
        //static extern int SetWindowText(IntPtr hWnd, string text);

        public static async void LaunchToSystemFF(string args,string cachepath, PersonData PersonData, bool outerprocess = true,string projname = "")
        {
            string ffpath = cachepath;


            string prefs = ffpath + "\\prefs.js";
            string filetext = File.Exists(prefs) ? File.ReadAllText(prefs) : "";
            //string filetext = "";
            if (File.Exists(prefs)) File.Delete(prefs);

            List<string> fileTextLines = filetext.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = fileTextLines.Count - 1; i >= 0; i--)
            {
                var line = fileTextLines[i];
                if (line.Contains("user_pref(\"plugin.state.npctrl\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"plugin.state.flash\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"plugin.state.java\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"media.peerconnection.enabled\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"privacy.donottrackheader.enabled\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"services.sync.prefs.sync.privacy.donottrackheader.enabled\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.type\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.http\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.http_port\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ssl\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ssl_port\", "))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.ssl\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.ssl_port\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ftp\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.ftp_port\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.socks\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.socks_port\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.socks\","))
                {
                    fileTextLines.RemoveAt(i);
                }
                else if (line.Contains("user_pref(\"network.proxy.backup.socks_port\","))
                {
                    fileTextLines.RemoveAt(i);
                }
            }



            if (outerprocess)
            {
                //filetext +=
                //     "user_pref(\"plugin.state.npctrl\", 0); \n" +
                //      "user_pref(\"plugin.state.flash\", 0); \n" +
                //       "user_pref(\"plugin.state.java\", 0); \n" +
                //        "user_pref(\"media.peerconnection.enabled\", false); \n" +
                //         "user_pref(\"privacy.donottrackheader.enabled\", true); \n" +
                //         "user_pref(\"browser.tabs.remote.autostart.2\", false); \n" +
                //         "user_pref(\"browser.tabs.remote.autostart\", false); \n" +
                //        "user_pref(\"services.sync.prefs.sync.privacy.donottrackheader.enabled\", true);";

                fileTextLines.Add("user_pref(\"plugin.state.npctrl\", 0);");
                fileTextLines.Add("user_pref(\"plugin.state.flash\", 0);");
                fileTextLines.Add("user_pref(\"plugin.state.java\", 0);");
                fileTextLines.Add("user_pref(\"media.peerconnection.enabled\", false);");
                fileTextLines.Add("user_pref(\"privacy.donottrackheader.enabled\", true);");
                fileTextLines.Add("user_pref(\"browser.tabs.remote.autostart.2\", false);");
                fileTextLines.Add("user_pref(\"browser.tabs.remote.autostart\", false);");
                fileTextLines.Add("user_pref(\"services.sync.prefs.sync.privacy.donottrackheader.enabled\", true);");
            }

            if (!string.IsNullOrEmpty(PersonData.ProxyIP) && !string.IsNullOrWhiteSpace(PersonData.ProxyIP))
            {
                fileTextLines.Add("user_pref(\"network.proxy.type\", 1); ");
                fileTextLines.Add("user_pref(\"network.proxy.share_proxy_settings\", true);");
                fileTextLines.Add("user_pref(\"network.proxy.http\", \"" + PersonData.ProxyIP + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.http_port\", " + PersonData.ProxyPort + ");");
                fileTextLines.Add("user_pref(\"network.proxy.ssl\", \"" + PersonData.ProxyIP + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.ssl_port\", " + PersonData.ProxyPort + ");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.ssl\", \"" + PersonData.ProxyIP + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.ssl_port\", " + PersonData.ProxyPort + ");");
                fileTextLines.Add("user_pref(\"network.proxy.ftp\", \"" + PersonData.ProxyIP + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.ftp_port\", " + PersonData.ProxyPort + ");");
                fileTextLines.Add("user_pref(\"network.proxy.socks\", \"" + PersonData.ProxyIP + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.socks_port\", " + PersonData.ProxyPort + ");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.socks\", \"" + PersonData.ProxyIP + "\");");
                fileTextLines.Add("user_pref(\"network.proxy.backup.socks_port\", " + PersonData.ProxyPort + ");");

                //filetext =
                //     filetext + " \n" +
                //     "user_pref(\"network.proxy.type\", 1); \n" +
                //     "user_pref(\"network.proxy.share_proxy_settings\", true); \n" +
                //     "user_pref(\"network.proxy.http\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.http_port\", " + PersonData.ProxyPort + "); \n" +
                //     "user_pref(\"network.proxy.ssl\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.ssl_port\", " + PersonData.ProxyPort + "); \n" +
                //     "user_pref(\"network.proxy.backup.ssl\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.backup.ssl_port\", " + PersonData.ProxyPort + "); \n" +
                //     "user_pref(\"network.proxy.ftp\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.ftp_port\", " + PersonData.ProxyPort + "); \n" +
                //     "user_pref(\"network.proxy.backup.ftp\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.backup.ftp_port\", " + PersonData.ProxyPort + "); \n" +
                //     "user_pref(\"network.proxy.socks\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.socks_port\", " + PersonData.ProxyPort + "); \n" +
                //     "user_pref(\"network.proxy.backup.socks\", \"" + PersonData.ProxyIP + "\"); \n" +
                //     "user_pref(\"network.proxy.backup.socks_port\", " + PersonData.ProxyPort + ");";
            }
            File.WriteAllLines(prefs, fileTextLines.ToArray());
            //if (outerprocess)
            //{
                var exePath =outerprocess ? AppDomain.CurrentDomain.BaseDirectory + "\\firefox-sdk\\bin\\firefox.exe" : "firefox.exe";
                exePath = exePath.Replace("\\\\", "\\");
                Process process = new Process();
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = true;
                process.Start();
                //if (projname != "")
                //{
                //    await Task.Delay(500); // <-- ugly hack
                //    SetWindowText(process.MainWindowHandle, projname);
                //}
           // }
        }
        #endregion
    }
}
