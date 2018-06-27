using Browseo.Browser.DataAccess.FileTypes;
using Browseo.Browser.Framework.IO;
using Browseo.Browser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.Browser.DataAccess
{       
    public class ProjectDataFilesReader
    {
        public string[] ProjectIniValues = 
            {
            "ProjectName",
            "ProfileName",
            "PhoneNumber",
            "FirstName",
            "LastName",
            "Username",
            "Email",
            "Password",
            "ProxyIP",
            "ProxyPort",
            "ProxyUsername",
            "ProxyPassword",
            "Sex",
            "BirthdayDay",
            "BirthdayMonth",
            "BirthdayYear",
            "Street",
            "City",
            "State",
            "Zip",
            "Country",
            "Notes",
            "WebAddress",
            "InVault",
            "InMoney",
            "BIADefault",
            "SIPBNType",
        };

        public static PersonData GetProjectOrProfilePersonData(string directory)
        {
            string sitesFilePath = directory;
            if (!directory.Contains(".ini"))
                sitesFilePath = Path.Combine(directory, "UserData.ini");
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
    }
}
