using Browmium.WPF.ViewModels;
using Browmium.WPF.WinForms;
using BrowseoFX_WPF.Core;
using GoViral.Instagram.InstViewModels;
using GoViral.ViewModels;
using IMacroMultyLayout.ViewModels;
using Organiser.Common.Classes;
using Organiser.Common.ViewModels;
using PData.FilesReader;
using Prospector.ViewModels;
using RssReader.Mvvm;
using RssReader.ViewModels;
using RssReader.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
//using zFirefoxXulBrowser.API;
//using zFirefoxXulBrowser.ViewModels;

namespace BrowserAndFeatures
{
    /// <summary>
    /// Interaction logic for FeatureCallage.xaml
    /// </summary>
    public partial class FeatureCallage : UserControl, IListenToFXManagerFC
    {
        //to host
        public event Action OnClickedReminders = delegate { };
        public event Action OnRequestedScreenLocation = delegate { };

        bool browserinit = false;

        //FoxXulViewModel ffXulVm;

        public FeatureCallage()
        {
            InitializeComponent();


            this.DataContext = this;
            //RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            if (App.browserinit)
            {
                BrowserLibraryInit.Instance.PlatformInitialize(null);
                //BrowserInit.Init(false);
                //FoxInit.Init();
                // Initializer.Init();
            }

            MyFilesDatabase.GetSites();

            (prospector.DataContext as FootPrintsOptionsVM).OnClickedSearch += FeatureCallage_OnClickedSearch;
            (prospector.DataContext as FootPrintsOptionsVM).OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;


            browser.OnCurateToPBN += Browser_OnCurateToPBN;
            browser.Loaded += Browser_Loaded;
            browser.OnRefreshedSessionSettings += Browser_OnRefreshedSessionSettings;
            browser.OnSentForSeo += Browser_OnSentForSeo;
            browser.OnClickedReminders += Browser_OnClickedReminders;

            BrowseoFXManager.Instance.ListenToFXManagerFC = this;

            //ffBrowser.OnCurateToPBN += Browser_OnCurateToPBN;
            //ffBrowser.Loaded += Browser_Loaded;
            //ffBrowser.OnRefreshedSessionSettings += Browser_OnRefreshedSessionSettings;
            //ffBrowser.OnSentForSeo += Browser_OnSentForSeo;
            //ffBrowser.OnClickedReminders += Browser_OnClickedReminders;
            //ffBrowser.OnRequestedWindowLocation += FfBrowser_OnRequestedWindowLocation;



            //Dispatcher.BeginInvoke(new Action(()=> 
            //{
            //    if (!BrowserInit.settings.MultiThreadedMessageLoop)
            //    {
            //        int times = 5;
            //        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
            //        //System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle,new Action(()=> 
            //        //{
            //        //    Xilium.CefGlue.CefRuntime.DoMessageLoopWork();
            //        //}) );
            //        System.Windows.Forms.Application.Idle += (sender, e) =>
            //        {
            //            //await semaphoreSlim.WaitAsync();
            //            //await Task.Delay(100);
            //            //if (times == 5) Xilium.CefGlue.CefRuntime.DoMessageLoopWork();
            //            //times--;
            //            //if (times == 0) times = 5;
            //            //semaphoreSlim.Release();

            //            Dispatcher.BeginInvoke(new Action(() =>
            //            {
            //                Xilium.CefGlue.CefRuntime.DoMessageLoopWork();
            //            }), null);
            //        };
            //    }

            //    System.Windows.Forms.Application.Run();
            //}), null);
        }

        //private void FfXulBrowser_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //TODO saved session?
        //    ffXulVm = new FoxXulViewModel();
        //    ffXulBrowser.DataContext = ffXulVm;
        //    //ffXulVm.InitBrowser(MyFilesDatabase.GetDefultHomePage());
        //}


        #region setup data
        public void SetPermissions(bool allowProspector, bool allowRSS, bool allowPBN, bool allowFeedMash, bool allowIndexer, bool allowYoutube, bool canSeeProxys, bool hasKK)
        { 
            if (!allowProspector) tabProspector.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowRSS) tabrssControl.Visibility = System.Windows.Visibility.Collapsed;
            if (!allowPBN) tabrsswisi.Visibility = System.Windows.Visibility.Collapsed;
            //if (!allowFeedMash) feed.Visibility = System.Windows.Visibility.Collapsed;
            //if (!allowIndexer) indexer.Visibility = System.Windows.Visibility.Collapsed;
            //if (!allowYoutube) youtuber.Visibility = System.Windows.Visibility.Collapsed;
            BrowseoFXManager.Instance.PanelUIHandler.IsEnabledForKK = hasKK;
            if (!hasKK)
            {
                prospector.tabKingKontent.Visibility = System.Windows.Visibility.Collapsed;
                //prospector.tabDW.Visibility = System.Windows.Visibility.Collapsed;
            }
            prospector.tabDW.Visibility = System.Windows.Visibility.Collapsed;
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

            BrowserLibraryInit.Instance.PlatformInitialize(profile);
            //browser.OpenTabsFromPastSessions();
        }

        public void GotScrennCords(string message)
        {
            //ffXulBrowser.GotScreenCords(message);
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
            GloableProfData.PData = profile;
            BrowserLibraryInit.Instance.PlatformInitialize(profile);
            MyFilesDatabase.SetUpImacroProfileInfo();
            //FoxInit.Init(profile);
            // Initializer.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }

        #endregion

        public void CloseAll()
        {
            RssReader.MainViewModel.isCloseing = true;

            if(feedMasherVM != null)
                feedMasherVM.DisposeBrowser();
            feedMasherVM = null;

            if (instadmVM != null)
                instadmVM.DisposeBrowser();
            instadmVM = null;

            // browserChrome.CloseAllTabs();
            browser.CloseAllTabs();
            BrowserLibraryInit.Instance.ShutDown();

            ffXulBrowser.CloseAllTabs();
            //FoxInit.Shutdown();
            ProcessManager.Instance.DisposeAllProcess();

            //GC.Collect(); 
        }

        #region other features tabs

        int previndex;
        //Indexer.MainWindow imw; 
        //Youtuber.MainWindow ytmw;
        //GoViralVM goViralVM;
        InstaDominateVM instadmVM;
        LinksToRssVM feedMasherVM;
        int curIndex = -1;

        private void tbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (curIndex == tbControl.SelectedIndex) return;

                curIndex = tbControl.SelectedIndex;


                //if (previndex != tbControl.SelectedIndex && tbControl.SelectedIndex<tbControl.Items.Count-2)
                //{
                if (tbControl.SelectedIndex == 0)
                {
                    browser.SetBookmarksEvents(false);
                    //ffXulBrowser.SetBookmarksEvents(true);
                }
                else if (tbControl.SelectedIndex == 1)
                {
                    browser.SetBookmarksEvents(true);
                    //ffXulBrowser.SetBookmarksEvents(false);
                }
                else if (tbControl.SelectedIndex == 3)
                {
                    return;
                }
                else if (tbControl.SelectedIndex == 4)
                {
                    if (wisi.DataContext == null || wisi.DataContext == this)
                    {
                        setwisi();
                    }
                }
                //else if (tbControl.SelectedIndex == 5)
                //{
                //    //crreateFeedMAsherContext();
                //    setLauncherContext();
                //}
                else if (tbControl.SelectedIndex == 5)
                {
                    if (macroRunner.DataContext == null || macroRunner.DataContext.GetType() != typeof(MultyMacroVm))
                    {
                        MultyMacroVm vm = new MultyMacroVm();
                        macroRunner.DataContext = vm;
                    }
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
                //else
                //    {
                //        previndex = tbControl.SelectedIndex;
                //    }
                //}
                //else
                //{
                //    if (tbControl.SelectedIndex == tbControl.Items.Count - 2)
                //    {
                //        tbControl.SelectedIndex = previndex;
                //        if (imw == null)
                //        {
                //            imw = new Indexer.MainWindow();
                //            imw.Title = "Indexer - One Link On A Line keep http://";
                //            imw.Topmost = true;
                //            imw.Closed += ltrw_Closed;
                //            imw.Show();
                //        }
                //    }
                //    else if (tbControl.SelectedIndex == tbControl.Items.Count - 1)
                //    {
                //        tbControl.SelectedIndex = previndex;
                //        if (ytmw == null)
                //        {
                //            ytmw = new Youtuber.MainWindow();
                //            ytmw.Topmost = true;
                //            ytmw.Closed += ltrw_Closed;
                //            ytmw.Show();
                //        }
                //    }
                //}
            }
            catch { }
        }

        private void tbControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is TabItem && (e.Source as TabItem).Header.ToString() == "RSS")
            {
                e.Handled = true;

                if (rssMainControl.DataContext is RSSMainWorkspaceViewModel)
                {
                    //await RSSMainWorkspaceViewModel.Instance.MakeInvis();

                    tbControl.SelectedIndex = 3;

                    //RSSMainWorkspaceViewModel.Instance.MakeVisible();
                }
                else
                {
                    tbControl.SelectedIndex = 3;

                    RSSMainWorkspaceViewModel.Instance.OnRSSMainWorkspaceViewModelMessage += OnRSSMainWorkspaceViewModelMessage;

                    rssMainControl.DataContext = RSSMainWorkspaceViewModel.Instance;
                    RSSMainWorkspaceViewModel.Instance.LoadFeedTabs();
                }
            }
        }

        private async void OnRSSMainWorkspaceViewModelMessage(RSSMainWorkspaceViewModelMessage message)
        {
            //rssControl.OnLaunchToBrowser += RssControl_OnLaunchToBrowser; ;
            //rssControl.OnLaunchToTabBrowser += FeatureCallage_OnClickedSearch;
            //rssControl.OnSelectedSendToSeo += RssControl_OnSelectedSendToSeo;
            //rssControl.OnLaunchToMasher += rssControl_OnLaunchToMasher;
            //rssControl.OnSelectedSendToPbn += RssControl_OnSelectedSendToPbn;

            switch (message.MessageType)
            {
                case "OnSelectedSendToSeo":
                    RssControl_OnSelectedSendToSeo(
                        message.Parameters[0] as string,
                        message.Parameters[1] as string);
                    break;

                case "OnSelectedSendToPbn":
                    RssControl_OnSelectedSendToPbn(
                        message.Parameters[0] as string, 
                        message.Parameters[1] as string,
                        message.Parameters[2] as string, 
                        message.Parameters[3] as string, 
                        message.Parameters[4] as string);
                    break;

                case "OnSelectedLaunchLinkMasher":
                    rssControl_OnLaunchToMasher(message.Parameters[0] as string);
                    break;

                case "OnSelectedLaunchLink":
                    FeatureCallage_OnClickedSearch(
                        message.Parameters[0] as string, 
                        Convert.ToBoolean(message.Parameters[1]));
                    break;

                case "OnClickedOpenSocialShareLink":
                    RssControl_OnLaunchToBrowser(
                        message.Parameters[0] as string,
                        message.Parameters[1] as string,
                        Convert.ToBoolean(message.Parameters[2]));
                    break;

                default:
                    break;
            }
        }

        //private void crreateFeedMAsherContext()
        //{
        //    //if (feedMasherVM == null)
        //    //{
        //    //    feedMasherVM = new LinksToRssVM(); 
        //    //    ucFeedMasher.DataContext = feedMasherVM;
        //    //}
        //}
        #region fxmanagertoFC
        public void OnCurateToPBN(string html, string uri)
        {
            Browser_OnCurateToPBN(html, uri);
        }

        public void OnSentForSeo(string sitename, string url)
        {
            Browser_OnSentForSeo(sitename, url);
        }

        #endregion
        #region uc browser events

        private void Browser_OnRefreshedSessionSettings()
        {
            //if (goViralVM != null)
            //{
            //    goViralVM.RefreshBrowser();
            //} 
        }

        private void Browser_OnCurateToPBN(string content, string link)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                if (wisi.DataContext == null || wisi.DataContext == this)
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
            ffXulBrowser.OnAddedToGoViral -= Browser_OnAddedToGoViral;
            ffXulBrowser.OnSpinClicked -= Browser_OnSpinClicked;

            ffXulBrowser.OnSpinClicked += Browser_OnSpinClicked;
            browser.OnAddedToGoViral += Browser_OnAddedToGoViral;
            ffXulBrowser.OnAddedToGoViral += Browser_OnAddedToGoViral;

            browser.Loaded -= Browser_Loaded;
            ffXulBrowser.Loaded -= Browser_Loaded;
        }

        private void GoViralVM_OnCreateNewTab(string url)
        {
            if (url == null) return;
            browser.CreateNewTab(url);
        }

        private void Browser_OnAddedToGoViral(string link,string type, List<string> multiLinks)
        {
            // createGoViralVM();
            //if (goViralVM == null) goViralVM = ucGoViral.DataContext as GoViralVM;
            ffXulBrowser.AsyncAddLinkToList(link, type, multiLinks, showLinksWindow: true);
        }

        private void Browser_OnSentForSeo(string name, string url)
        {
            ffXulBrowser.ucSharedSync.ViewModel.AddUrlToSavedProjectList(name, url, null);
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

        void FeatureCallage_OnClickedSearch(string query,bool isFF)
        {
            if (isFF)
            {
                ffXulBrowser.SearchFor(query);
                tbControl.SelectedIndex = 0;
            }
            else
            {
                browser.SearchFor(query);
                tbControl.SelectedIndex = 1;
            }
        }

        private void RssControl_OnLaunchToBrowser(string link, string rssLink,bool ff)
        {
            if (ff) ffXulBrowser.LaunchNewWindow(link,rssLink);
            else browser.LaunchNewWindowToLink(link, rssLink);
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
            if (wisi.DataContext == null || wisi.DataContext == this)
            {  
                tbControl.SelectedIndex = 4;
                setwisi();
            }

            wisi.AddSetRssFeed(link, title, imglink, date, description);
        }

        void rssControl_OnLaunchToMasher(string link)
        {
            //crreateFeedMAsherContext();
            feedMasherVM.AddMasherLink(link);
        }

        void ltrw_Closed(object sender, EventArgs e)
        {
            //ytmw = null; 
            //imw = null;
        }
        #endregion
        
        private void FfBrowser_OnRequestedWindowLocation()
        {
            OnRequestedScreenLocation();
        }

        //private void ucSharedSync_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (ucSharedSync.ViewModel == null)
        //    {
        //        ucSharedSync.ViewModel = new SyncedProjectsVM(SyncedProjectsVM.TypeOfSEO);
        //        ucSharedSync.DataContext = ucSharedSync.ViewModel;
        //    }
        //}

        //private void ucSystemBrowSERLauncher_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (ucSystemBrowSERLauncher.ViewModel == null)
        //    {
        //        ucSystemBrowSERLauncher.ViewModel = new SyncedProjectsVM(SyncedProjectsVM.TypeOfSystemBrowSERLauncher);
        //        ucSystemBrowSERLauncher.DataContext = ucSystemBrowSERLauncher.ViewModel;
        //    }
        //}

        private void RssControl_OnSelectedSendToSeo(string title, string url)
        {
            ffXulBrowser.ucSharedSync.ViewModel.AddUrlToSavedProjectList(title, url, null);
        }

        private void Browser_OnSpinClicked()
        {
            if (wisi != null)
            {
                wisi.SpinAndCopyToClipboard();
            }
        }

        private void browserChrome_Loaded(object sender, RoutedEventArgs e)
        {
            //browser.OnCurateToPBN += Browser_OnCurateToPBN;
            //browser.Loaded += Browser_Loaded;
            //browser.OnRefreshedSessionSettings += Browser_OnRefreshedSessionSettings;
            //browser.OnSentForSeo += Browser_OnSentForSeo;
            //browser.OnClickedReminders += Browser_OnClickedReminders;

            //browserChrome.DataContext = new BrowserTabViewModel();
        }

        private void tbPreviewLink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb == null) return;

            Process.Start(tb.Text);
        }
    }
}
