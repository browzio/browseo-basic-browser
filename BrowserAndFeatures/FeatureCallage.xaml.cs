using GoViral.Instagram.InstViewModels;
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
using zFirefoxBrowser.Helpers;

namespace BrowserAndFeatures
{
    /// <summary>
    /// Interaction logic for FeatureCallage.xaml
    /// </summary>
    public partial class FeatureCallage : UserControl
    {
        //to host
        public event Action OnClickedReminders = delegate { };

        public FeatureCallage()
        {
            InitializeComponent();

            if (App.browserinit)
            {
                BrowserInit.Init();
                FoxInit.Init();
            }

           
            MyFilesDatabase.GetSites(); 

            (prospector.DataContext as FootPrintsOptionsVM).OnClickedSearch += FeatureCallage_OnClickedSearch;
            (prospector.DataContext as FootPrintsOptionsVM).OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;
            
            rssControl.OnLaunchToBrowser += RssControl_OnLaunchToBrowser; ;
            rssControl.OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
            rssControl.OnSelectedSendToSeo += RssControl_OnSelectedSendToSeo;
            rssControl.OnLaunchToMasher += rssControl_OnLaunchToMasher;
            rssControl.OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;

            browser.OnCurateToPBN += Browser_OnCurateToPBN;
            browser.Loaded += Browser_Loaded;
            browser.OnRefreshedSessionSettings += Browser_OnRefreshedSessionSettings;
            browser.OnSentForSeo += Browser_OnSentForSeo;
            browser.OnClickedReminders += Browser_OnClickedReminders;

            ffBrowser.OnCurateToPBN += Browser_OnCurateToPBN;
            ffBrowser.Loaded += Browser_Loaded;
            ffBrowser.OnRefreshedSessionSettings += Browser_OnRefreshedSessionSettings;
            ffBrowser.OnSentForSeo += Browser_OnSentForSeo;
            ffBrowser.OnClickedReminders += Browser_OnClickedReminders;
        }

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

            BrowserInit.Init(profile);
            FoxInit.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }

        #endregion

        public void CloseAll()
        {
            RssReader.MainViewModel.isCloseing = true;
            if (goViralVM != null)
            {
                goViralVM.DisposeBrowser();
                if (ucGoViral.ucSearch != null)
                {
                    if(ucGoViral.ucSearch.ViewModel != null)
                    {
                        ucGoViral.ucSearch.ViewModel.ShutDown();
                    }
                }
            }
            goViralVM = null;

            if(feedMasherVM != null)
                feedMasherVM.DisposeBrowser();
            feedMasherVM = null;

            if (instadmVM != null)
                instadmVM.DisposeBrowser();
            instadmVM = null;

            if (wisi.DataContext != null)
            {
                wisi.webBrowserEditor.webBrowser.Dispose();
            }

            browser.CloseAllTabs();
            BrowserInit.Shutdown();
            ffBrowser.CloseAllTabs();
            FoxInit.Shutdown();
            ProcessManager.Instance.DisposeAllProcess();

            GC.Collect(); 
        }

        #region other features tabs

        int previndex;
        Indexer.MainWindow imw; 
        Youtuber.MainWindow ytmw;
        GoViralVM goViralVM;
        InstaDominateVM instadmVM;
        LinksToRssVM feedMasherVM;
        bool wtfman = false;

        private void tbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if DEBUG
            int eio = 0;
#else

            if (!wtfman)
            {
                wtfman = true;
                new Thread(() =>
                {
                    string tmpdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp";
                    int waited = 0;
                    while (!Directory.Exists(tmpdir))
                    {
                        Thread.Sleep(500); waited++;
                        if (waited == 10)
                            System.Windows.Application.Current.Dispatcher.Invoke(() => { MyException ex = new MyException("Un Known"); ex.Source = "IMG"; throw ex; });
                    }
                    waited = 0;
                    string fpat = System.IO.Path.Combine(tmpdir, "tmpeo76354foo");
                    while (!File.Exists(fpat))
                    {
                        Thread.Sleep(500); waited++;
                        if (waited == 10)
                            System.Windows.Application.Current.Dispatcher.Invoke(() => { MyException ex = new MyException("Un Known"); ex.Source = "IMG"; throw ex; });
                    }
                    if (File.ReadAllText(fpat) != "browzio")
                        System.Windows.Application.Current.Dispatcher.Invoke(() => { MyException ex = new MyException("Un Known"); ex.Source = "IMG"; throw ex; });
                    else
                        File.Delete(fpat);
                }).Start();
            }
#endif


            if (previndex != tbControl.SelectedIndex && tbControl.SelectedIndex<tbControl.Items.Count-2)
            {
                if (tbControl.SelectedIndex == 0)
                {
                    previndex = tbControl.SelectedIndex;

                    browser.SetBookmarksEvents(true);
                    ffBrowser.SetBookmarksEvents(false);
                }
                else if (tbControl.SelectedIndex == 1)
                {
                    previndex = tbControl.SelectedIndex;

                    browser.SetBookmarksEvents(false);
                    ffBrowser.SetBookmarksEvents(true);
                }
                else if (tbControl.SelectedIndex == 3 && rssControl.UserRssTabs.Count <=0)
                {
                    previndex = tbControl.SelectedIndex;
                    rssControl.InitTabs();
                }
                else if (tbControl.SelectedIndex == 4)
                {
                    previndex = tbControl.SelectedIndex;

                    if (wisi.DataContext == null)
                    {
                        setwisi();
                    }
                }
                else if (tbControl.SelectedIndex == 5)
                {
                    previndex = tbControl.SelectedIndex;
                    crreateFeedMAsherContext(); 
                }
                //else if (tbControl.SelectedIndex == 8)
                //{
                //    previndex = tbControl.SelectedIndex;
                //    if (ucInstagram.cntrlSorter.ViewModel == null)
                //    {
                //        ucInstagram.cntrlSorter.ViewModel = new SyncedProjectsVM(SyncedProjectsVM.TypeOfInsteo);
                //        ucInstagram.cntrlSorter.DataContext = ucInstagram.cntrlSorter.ViewModel;
                //    }

                //    if (instadmVM == null)
                //    {
                //        instadmVM = new InstaDominateVM();
                //        ucInstagram.cntrlDominator.DataContext = instadmVM;
                //        instadmVM.OnSendContentToSorter += (content) =>
                //        {
                //            ucInstagram.cntrlSorter.ViewModel.AddUrlToSavedProjectList("", "", content);
                //        };
                //    }
                //}
                else
                {
                    previndex = tbControl.SelectedIndex;
                }
            }
            else
            {
                if (tbControl.SelectedIndex == tbControl.Items.Count - 2)
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
                else if (tbControl.SelectedIndex == tbControl.Items.Count - 1)
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

        #region uc browser events
        private void Browser_OnRefreshedSessionSettings()
        {
            if (goViralVM != null)
            {
                goViralVM.RefreshBrowser();
            } 
        }

        private void Browser_OnCurateToPBN(string content, string link)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (wisi.DataContext == null)
                {

                    tbControl.SelectedIndex = 4;
                    setwisi();
                }

                wisi.injectHtml(content, link);
            });
        }

        private void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            //browser.CheckAndSetOpenTabs();

            browser.OnAddedToGoViral -= Browser_OnAddedToGoViral;
            //ffBrowser.OnAddedToGoViral -= Browser_OnAddedToGoViral;

            browser.OnAddedToGoViral += Browser_OnAddedToGoViral;
            //ffBrowser.OnAddedToGoViral += Browser_OnAddedToGoViral;

            if (goViralVM == null) goViralVM = ucGoViral.DataContext as GoViralVM;

            browser.Loaded -= Browser_Loaded;
            //ffBrowser.Loaded -= Browser_Loaded;
        }

        private void Browser_OnAddedToGoViral(string link,string type, List<string> multiLinks)
        {
            // createGoViralVM();
            //if (goViralVM == null) goViralVM = ucGoViral.DataContext as GoViralVM;
            goViralVM.AsyncAddLinkToList(link, type, multiLinks, showLinksWindow: true);
        }

        private void Browser_OnSentForSeo(string name, string url)
        {
            ucSharedSync.ViewModel.AddUrlToSavedProjectList(name, url, null);
        }
        
        private void Browser_OnClickedReminders()
        {
            OnClickedReminders();
        }

        public void SetRemindersCount()
        {
            Task.Factory.StartNew(()=> { browser.SetRemindersCount(); }); 
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

        private void ucSharedSync_Loaded(object sender, RoutedEventArgs e)
        {
            if (ucSharedSync.ViewModel == null)
            {
                ucSharedSync.ViewModel = new SyncedProjectsVM(SyncedProjectsVM.TypeOfSEO);
                ucSharedSync.DataContext = ucSharedSync.ViewModel;
            }
        }

        private void RssControl_OnSelectedSendToSeo(string title, string url)
        {
            ucSharedSync.ViewModel.AddUrlToSavedProjectList(title, url, null);
        }

        private void btnSpin_Click(object sender, RoutedEventArgs e)
        {
            if(wisi != null)
            {
                wisi.SpinAndCopyToClipboard();
            }
        }
    }
}
