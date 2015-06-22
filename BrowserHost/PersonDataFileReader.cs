using BrowserHost.Models;
using PData.FilesReader;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserHost
{
    public class PersonDataFileReader
    {
        // private static string ProjectName = "";

        public static PersonData GetPersonData(PersonData personData)
        {
            PersonData data = personData;
            string sitesFilePath = Path.Combine(GetSratupPath() + "\\RAWSocialOrganizer", "Temp", "BrowserConfig.ini");
            if (!File.Exists(sitesFilePath))
            {
                for (int i = 0; i < 100; i++)
                {
                    if (File.Exists(sitesFilePath)) break;
                    Thread.Sleep(10);
                }
            }
            IniFile ini = new IniFile(sitesFilePath);
            try
            {
                //ProjectName = ini.IniReadValue("Data", "ProjectName");
                data.ProjectName = ini.IniReadValue("Data", "ProjectName");
                data.FirstName = ini.IniReadValue("Data", "FirstName");
                data.LastName = ini.IniReadValue("Data", "LastName");
                data.Email = ini.IniReadValue("Data", "Email");
                data.Password = ini.IniReadValue("Data", "Password");
                data.Username = ini.IniReadValue("Data", "Username");
                data.ProxyIP = ini.IniReadValue("Data", "ProxyIP");
                data.ProxyPort = ini.IniReadValue("Data", "ProxyPort");
                data.ProxyUsername = ini.IniReadValue("Data", "ProxyUsername");
                data.ProxyPassword = ini.IniReadValue("Data", "ProxyPassword");
                data.PhoneNumber = ini.IniReadValue("Data", "PhoneNumber");
                data.Street = ini.IniReadValue("Data", "Street");
                data.City = ini.IniReadValue("Data", "City");
                data.State = ini.IniReadValue("Data", "State");
                data.Zip = ini.IniReadValue("Data", "Zip");
                data.Country = ini.IniReadValue("Data", "Country");
                data.Notes = ini.IniReadValue("Data", "Notes");
                data.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                try
                {
                    data.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                    data.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                    data.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }
            return data;
        }

        public static string GetSratupPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public static void SaveSite(string site, string projName)
        {
            if (projName != "")
            {
                string dir = GetSratupPath() + "\\RAWSocialOrganizer\\Sites";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.AppendAllText(dir + "\\" + projName + ".txt", site + Environment.NewLine);
            }
        }

        public static ObservableCollection<SavedSite> GetSavedSites(ObservableCollection<SavedSite> SitesList, string projName)
        {
            SitesList.Clear();
            string dir = GetSratupPath() + "\\RAWSocialOrganizer\\Sites";
            if (Directory.Exists(dir))
            {
                if (!File.Exists(dir + "\\" + projName + ".txt")) return SitesList;
                foreach (string site in File.ReadAllLines(dir + "\\" + projName + ".txt"))
                {
                    SitesList.Add(new SavedSite() { Site = site });
                }
            }
            return SitesList;
        }


        public static void DeleteSite(ObservableCollection<SavedSite> SitesList, string projName)
        {
            string dir = GetSratupPath() + "\\RAWSocialOrganizer\\Sites";
            File.Delete(dir + "\\" + projName + ".txt");
            foreach (SavedSite site in SitesList)
            {
                File.AppendAllText(dir + "\\" + projName + ".txt", site.Site + Environment.NewLine);
            }
        }
    }
}
