using Organiser.Common.Classes;
using PData.FilesReader;
using ProjectsList.Helpers;
using Prospector.ViewModels;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Xilium.CefGlue.Client;

namespace BrowserAndFeatures
{
    /// <summary>
    /// Interaction logic for FeatureCallage.xaml
    /// </summary>
    public partial class FeatureCallage : UserControl
    {
        

        public FeatureCallage()
        {
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            //this.Closed += (sender, args) => { PostQuitMessage(0); };
            InitializeComponent();
            SetPersonData();
            (prospector.DataContext as FootPrintsOptionsVM).OnClickedSearch += FeatureCallage_OnClickedSearch;
        }

        void FeatureCallage_OnClickedSearch(string query)
        {
            browser.SearchFor(query);
            tbControl.SelectedIndex = 0;
        }

        //void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    e.Handled = true;
        //    MessageBox.Show("CLR Dispatcher unhandled exception: " + e.Exception.Message);
        //}

        //void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
            
        //    var exception = e.ExceptionObject as Exception;
        //    var message = exception == null ? "null" : exception.Message;
        //    MessageBox.Show("CLR unhandled exception: " + message);
        //}
        PersonData profile;
        public void SetPersonData(PersonData data = null)
        {
            string path = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp");
            string sitesFilePath = System.IO.Path.Combine(File.ReadAllText(path + "\\info.txt"), "ProjectData.ini");
            IniFile ini = new IniFile(sitesFilePath);
            profile = new PersonData();
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
                profile.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                profile.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                profile.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                profile.ProjectDIr = sitesFilePath.Replace("\\ProjectData.ini", "");
                try
                {
                    profile.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }
            //browser.SetPersonData(data);
            BrowserInit.Init(sitesFilePath, data == null ? profile : data);

        }

        internal void close()
        {
            browser.CloseAllTabs();
        }

        public void CloseAll()
        {
            //if (login != null && didLogin)
            //    login.Logout();
            RssReader.MainViewModel.isCloseing = true;
            close();
            BrowserInit.Shutdown();
        }

        //bool didLogin;
        //ManagerControl mc;
        //TaskList tl;
        //Login login;
        bool setevents = false;
        private void tbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (tbControl.SelectedIndex == 2)
            //{
            //    if (!didLogin)
            //    {
            //        Eli.Taskforce.Helpers.Gloables.ProjectName = profile.ProjectName;
            //        login = new Login();
            //        login.OnLoginSuccess += login_OnLoginSuccess;
            //        login.Show();
            //    }
            //    else
            //    {
            //        if (mc != null)
            //        {
            //            tmContent.Content = mc;
            //        }
            //        else
            //        {
            //            tmContent.Content = tl;
            //        }
            //    }
            //}
            if (tbControl.SelectedIndex == 2 && !setevents)
            {
                rssControl.SetProfileData(profile);
                rssControl.OnLaunchToBrowser += FeatureCallage_OnLaunchToBrowser;
                rssControl.OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
                setevents = true;

                //if (!(rssControl.DataContext is RssReader.MainViewModel))
                //{
                //    rssControl.DataContext = new RssReader.MainViewModel();
                //    (rssControl.DataContext as RssReader.MainViewModel).SetProfileData(profile);
                //    (rssControl.DataContext as RssReader.MainViewModel).OnLaunchToBrowser += FeatureCallage_OnLaunchToBrowser;
                //    (rssControl.DataContext as RssReader.MainViewModel).OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
                //}
                //else if ((rssControl.DataContext as RssReader.MainViewModel).mProfile == null)
                //{
                //    (rssControl.DataContext as RssReader.MainViewModel).SetProfileData(profile);
                //    (rssControl.DataContext as RssReader.MainViewModel).OnLaunchToBrowser += FeatureCallage_OnLaunchToBrowser;
                //    (rssControl.DataContext as RssReader.MainViewModel).OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
                //}
            }
        }

        void FeatureCallage_OnLaunchToBrowser(string link, string rssLink)
        {
            browser.LaunchNewWindowToLink(link, rssLink);
        }


       // void login_OnLoginSuccess(bool isManager, object control)
        //{
            //didLogin = true;

            //if(isManager)
            //{
            //    mc = (ManagerControl)control;
            //    tmContent.Content = mc;
            //}
            //else
            //{
            //    tl = (TaskList)control;
            //    tmContent.Content = tl;
            //}
        //}
    }
}
