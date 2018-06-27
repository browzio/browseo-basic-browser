using Organiser.Common.Classes;
using PData.FilesReader;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zFirefoxXulBrowser.API;
using zFirefoxXulBrowser.ViewModels;

namespace zFirefoxXulBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetPersonData();

            var vm = new FoxXulViewModel();
            DataContext = vm;
            SetPersonData();
            //vm.InitBrowser("");
            this.Loaded -= MainWindow_Loaded;
        }

        public static void SetPersonData()
        {
            string path = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp");
            string sitesFilePath = System.IO.Path.Combine(System.IO.File.ReadAllText(path + "\\info.txt"), "ProjectData.ini");
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
            
            // FoxInit.Init(profile);
            Initializer.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }
    }
}
