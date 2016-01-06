using GoViral.ViewModels;
using Indexer;
using Organiser.Common.Classes;
using PData.FilesReader;
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
            InitializeComponent();

            if(App.browserinit)
                BrowserInit.Init();

            //MyFilesDatabase.GetSites(); 

            (prospector.DataContext as FootPrintsOptionsVM).OnClickedSearch += FeatureCallage_OnClickedSearch;
            (prospector.DataContext as FootPrintsOptionsVM).OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;
            
            rssControl.OnLaunchToBrowser += RssControl_OnLaunchToBrowser; ;
            rssControl.OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
            rssControl.OnLaunchToMasher += rssControl_OnLaunchToMasher;
            rssControl.OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;

            browser.OnCurateToPBN += Browser_OnCurateToPBN;
            browser.Loaded += Browser_Loaded;
        }

        //public FeatureCallage(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        //{
        //    InitializeComponent();

        //    SetPersonData();
        //    //SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);

        //    (prospector.DataContext as FootPrintsOptionsVM).OnClickedSearch += FeatureCallage_OnClickedSearch;
        //    (prospector.DataContext as FootPrintsOptionsVM).OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;
        //    browser.OnCurateToPBN += Browser_OnCurateToPBN;
        //}


        #region setup data
        public void SetPermissions(bool allowProspector, bool allowRSS, bool allowPBN, bool allowFeedMash, bool allowIndexer, bool allowYoutube, bool canSeeProxys, bool hasKK)
        { 
            if (!allowProspector) tabProspector.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowRSS) tabrssControl.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowPBN) tabrsswisi.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowFeedMash) feed.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowIndexer) indexer.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowYoutube) youtuber.Visibility = System.Windows.Visibility.Collapsed;
            if (!hasKK) prospector.tabKingKontent.Visibility = System.Windows.Visibility.Collapsed;
            if (canSeeProxys) MyFilesDatabase.CanSeeProxys = true;
        }

        public void SetPersonData(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            PersonData profile = new PersonData()
            {
                BirthdayYear = birthdayYear,
                Children = children,
                City = city,
                CmbSelectedIndexDay = cmbSelectedIndexDay,
                CmbSelectedIndexMonth = cmbSelectedIndexMonth,
                CmbSelectedIndexSex = cmbSelectedIndexSex,
                Country = country,
                Dir = dir,
                Email = email,
                FilePath = filePath,
                FirstName = firstName,
                InMonney = inMonney,
                InPBNVault = inPBNVault,
                LastName = lastName,
                Notes = notes,
                Password = password,
                PhoneNumber = phoneNumber,
                ProfileName = profileName,
                ProjectDir = projectDir,
                ProjectName = projectName,
                ProxyIP = proxyIP,
                ProxyPassword = proxyPassword,
                ProxyPort = proxyPort,
                ProxyUsername = proxyUsername,
                SIPBNType = sIPBNType,
                State = state,
                Street = street,
                Username = username,
                WebAddress = webAddress,
                Zip = zip,
            };

            BrowserInit.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }

        public static void SetPersonData()
        {
            string path = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp");
            string sitesFilePath = System.IO.Path.Combine(File.ReadAllText(path + "\\info.txt"), "ProjectData.ini");
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

            BrowserInit.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }

        #endregion

        public void CloseAll()
        {
            RssReader.MainViewModel.isCloseing = true;
            if (goViralVM != null)
                goViralVM.DisposeBrowser();

            if(feedMasherVM != null)
                feedMasherVM.DisposeBrowser();

            //GC.Collect();


            browser.CloseAllTabs();
            BrowserInit.Shutdown(); 
        }

        #region other features tabs

        int previndex;
        Indexer.MainWindow imw; 
        Youtuber.MainWindow ytmw;
        GoViralVM goViralVM; 
        LinksToRssVM feedMasherVM;

        private void tbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (previndex != tbControl.SelectedIndex && tbControl.SelectedIndex<6)
            {
                if (tbControl.SelectedIndex == 2 && rssControl.UserRssTabs.Count <=0)
                {
                    previndex = tbControl.SelectedIndex;
                    rssControl.InitTabs();
                }
                else if (tbControl.SelectedIndex == 3)
                {
                    previndex = tbControl.SelectedIndex;

                    if (wisi.DataContext == null)
                    {
                        setwisi();
                    }
                }
                else if (tbControl.SelectedIndex == 4)
                {
                    previndex = tbControl.SelectedIndex;
                    crreateFeedMAsherContext(); 
                }
                else if (tbControl.SelectedIndex == 5)
                {
                    previndex = tbControl.SelectedIndex;
                    { 
                        createGoViralVM();
                    } 
                }
                else
                {
                    previndex = tbControl.SelectedIndex;
                }
            }
            else
            {
                if (tbControl.SelectedIndex == 6)
                {
                    tbControl.SelectedIndex = previndex;
                    if (imw == null)
                    {
                        imw = new Indexer.MainWindow();
                        imw.Title = "Indexer - One Link On A Line keep http://";
                        imw.Topmost = true;
                        imw.Closed += ltrw_Closed;
                        imw.Show();
                    }
                }
                else if (tbControl.SelectedIndex == 6)
                {
                    tbControl.SelectedIndex = previndex;
                    if (ytmw == null)
                    {
                        ytmw = new Youtuber.MainWindow();
                        ytmw.Topmost = true;
                        ytmw.Closed += ltrw_Closed;
                        ytmw.Show();
                    }
                }
            }
        }

        private void crreateFeedMAsherContext()
        {
            if (feedMasherVM == null)
            {
                feedMasherVM = new LinksToRssVM(); 
                ucFeedMasher.DataContext = feedMasherVM;
            }
        }

        private void createGoViralVM()
        {
            if (goViralVM == null)
            {
                goViralVM = new GoViralVM();
                ucGoViral.DataContext = goViralVM;   
            }
        }

        #region uc browser events
        private void Browser_OnCurateToPBN(string content, string link)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (wisi.DataContext == null)
                {

                    tbControl.SelectedIndex = 3;
                    setwisi();
                }

                wisi.injectHtml(content, link);
            });
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            browser.CheckAndSetOpenTabs();
            browser.OnAddedToGoViral += Browser_OnAddedToGoViral;

            browser.Loaded -= Browser_Loaded;
        }

        private void Browser_OnAddedToGoViral(string link)
        {
            createGoViralVM();
            goViralVM.AsyncAddLinkToList(link);
        }
        #endregion

        void FeatureCallage_OnClickedSearch(string query)
        {
            browser.SearchFor(query);
            tbControl.SelectedIndex = 0;
        }

        private void RssControl_OnLaunchToBrowser(string link, string rssLink)
        {
            browser.LaunchNewWindowToLink(link, rssLink);
        }

        private void setwisi()
        {
            WPF_WYSIWYG_HTML_Editor.XmlRpcVM wvm = new WPF_WYSIWYG_HTML_Editor.XmlRpcVM();
            wvm.SetProfileDate(GloableProfData.PData);
            wisi.DataContext = wvm;
            wisi.SetProfileData(GloableProfData.PData);
            wisi.NewItUp();
        }

        private void RssControl_OnSelectedSendToPbn(string link, string title, string imglink, string date, string description)
        {
            //tbControl.SelectedIndex = 3;
            if (wisi.DataContext == null)
            {  
                tbControl.SelectedIndex = 3;
                setwisi();
            }

            wisi.AddSetRssFeed(link, title, imglink, date, description);
        }

        void rssControl_OnLaunchToMasher(string link)
        {
            crreateFeedMAsherContext();
            feedMasherVM.AddMasherLink(link);
        }

        void ltrw_Closed(object sender, EventArgs e)
        {
            ytmw = null; 
            imw = null;
        }
        #endregion
    }
}
