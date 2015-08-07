using Indexer;
using Organiser.Common.Classes;
using PData.FilesReader;
using ProjectsList.Helpers;
using Prospector.ViewModels;
using RssReader.Mvvm;
using RssReader.Windows;
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

        void FeatureCallage_OnLaunchToBrowser(string link, string rssLink)
        {
            browser.LaunchNewWindowToLink(link, rssLink);
        }

        bool setevents = false;
        int previndex;
        Indexer.MainWindow imw;
        LinksToRssWindow ltrw;
        Youtuber.MainWindow ytmw;
        private void tbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbControl.SelectedIndex == 2 && !setevents)
            {
                rssControl.SetProfileData(profile);
                rssControl.OnLaunchToBrowser += FeatureCallage_OnLaunchToBrowser;
                rssControl.OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
                setevents = true;
            }

            if (tbControl.SelectedIndex == 3)
            {
                tbControl.SelectedIndex = previndex;
            }
            else if (tbControl.SelectedIndex == 4) 
            {
                tbControl.SelectedIndex = previndex;
            }
            else if(tbControl.SelectedIndex == 5)
            {
                tbControl.SelectedIndex = previndex;
            }
            else
            {
                previndex = tbControl.SelectedIndex;
            }
        }

        private void youtuber_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ytmw == null)
            {
                ytmw = new Youtuber.MainWindow();
                ytmw.Topmost = true;
                ytmw.Closed += ltrw_Closed;
                ytmw.Show();
            }
        }

        private void indexer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (imw == null)
            {
                imw = new Indexer.MainWindow();
                imw.Title = "Indexer - One Link On A Line keep http://";
                imw.Topmost = true;
                imw.Closed += ltrw_Closed;
                imw.Show();
            }
        }

        private void feed_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ltrw == null)
            {
                ltrw = new LinksToRssWindow();
                ltrw.DataContext = new LinksToRssVM();
                ltrw.Topmost = true;
                ltrw.Closed += ltrw_Closed;
                ltrw.Show();
            }
        }

        void ltrw_Closed(object sender, EventArgs e)
        {
            ytmw = null;
            ltrw = null;
            imw = null;
        }
    }
}
