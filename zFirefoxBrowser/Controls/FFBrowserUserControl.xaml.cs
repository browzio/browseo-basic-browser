using DragDropListview;
using Gecko;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zFirefoxBrowser.Helpers;
using zFirefoxBrowser.ViewModels;
using System.Linq;

namespace zFirefoxBrowser.Controls
{
    /// <summary>
    /// Interaction logic for FFBrowserUserControl.xaml
    /// </summary>
    public partial class FFBrowserUserControl : UserControl
    {
        public ObservableCollection<FoxTabViewModel> BrowserTabs { get; set; }
        public FoxTabViewModel SelectedTab { get; set; }

        public event Action<string, string> OnCurateToPBN = delegate { };
        public event Action<string, string, List<string>> OnAddedToGoViral = delegate { };//link,type,multi
        public event Action OnRefreshedSessionSettings = delegate { };
        public event Action OnClickedReminders = delegate { };
        public event Action<string, string> OnSentForSeo = delegate { };//name,url
        public event Action OnRequestedWindowLocation = delegate { };

        public FFBrowserUserControl()
        {
            InitializeComponent();

            DataContext = this;

            BrowserTabs = new ObservableCollection<FoxTabViewModel>();
        }

        private void browserHost_OnCloseTab(ExecutedRoutedEventArgs e)
        {
            try
            {
                if (BrowserTabs.Count > 0)
                {
                    //Obtain the original source element for this event
                    var originalSource = (FrameworkElement)e.OriginalSource;

                    FoxTabViewModel browserViewModel = (FoxTabViewModel)originalSource.DataContext;
                    

                    BrowserTabs.Remove(browserViewModel);
                    browserViewModel.Dispose();

                    if (BrowserTabs.Count > 0)
                        BrowserTabs[0].TabMargin = new Thickness(-3, 0, 0, 0);

                    removeBTVMEvents(browserViewModel);
                    browserViewModel = null;
                    GC.Collect();
                }
            }
            catch(Exception ex) { }
        }
        public void CloseAllTabs()
        {
            foreach (var b in BrowserTabs)
            {
                b.Dispose();
            }
            BrowserTabs.Clear();
        }

        private void browserHost_OnOpenNewTab()
        {
            CreateNewTab("");
        }
        private void CreateNewTab(string url,bool switchTo = true)
        {
            MyFilesDatabase.CheckRamUsage();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                FoxTabViewModel btvm = new FoxTabViewModel(url == "" ? MyFilesDatabase.GetDefultHomePage() : url);
                setBTVMEvents(btvm);
                btvm.Title = url;
                Task.Factory.StartNew(() => { btvm.ReminderCount = MyFilesDatabase.GetRemindersCount(GloableProfData.PData.ProjectName); });
                if (BrowserTabs.Count > 0)
                    btvm.TabMargin = new Thickness(-20, 0, 0, 0);
                else
                    btvm.TabMargin = new Thickness(-3, 0, 0, 0);
                BrowserTabs.Add(btvm);

                if(switchTo) browserHost.TabControl.SelectedIndex = browserHost.TabControl.Items.Count - 1;
            });

            //Application.Current.Dispatcher.Invoke((Action)delegate
            //{
            //    Thread thread = new Thread(() =>
            //    {
            //        FoxTabViewModel btvm = new FoxTabViewModel(url == "" ? MyFilesDatabase.GetDefultHomePage() : url);
            //        setBTVMEvents(btvm);
            //        btvm.Title = url;
            //        Task.Factory.StartNew(() => { btvm.ReminderCount = MyFilesDatabase.GetRemindersCount(GloableProfData.PData.ProjectName); });
            //        if (BrowserTabs.Count > 0)
            //            btvm.TabMargin = new Thickness(-20, 0, 0, 0);
            //        else
            //            btvm.TabMargin = new Thickness(-3, 0, 0, 0);
            //        BrowserTabs.Add(btvm);

            //        browserHost.TabControl.SelectedIndex = browserHost.TabControl.Items.Count - 1;
            //        System.Windows.Threading.Dispatcher.Run();
            //    });


            //    thread.SetApartmentState(ApartmentState.STA);
            //    thread.Start();
            //});

        }

        private void setBTVMEvents(FoxTabViewModel btvm)
        {
            btvm.OnCreateNewTab += btvm_OnCreateNewTab;
            btvm.OnCurateToPBN += Btvm_OnCurateToPBN;
            btvm.OnAddedToGoViral += Btvm_OnAddedToGoViral;
            btvm.OnClickedSaveSession += Btvm_OnClickedSaveSession;
            btvm.OnSetUserAgent += Btvm_OnSetUserAgent;
            btvm.OnClickedDeleteSession += Btvm_OnClickedDeleteSession;
            btvm.OnClickedSaveSessionToBookmarks += Btvm_OnClickedSaveSessionToBookmarks;
            btvm.OnClickedReminders += Btvm_OnClickedReminders;
            //btvm.OnRefreshTabSettingsTab += Btvm_OnRefreshTabSettings;
            btvm.OnRefreshSessionSettings += Btvm_OnRefreshSessionSettings;
            btvm.OnSentForSeo += Btvm_OnSentForSeo;
            btvm.OnRequestedWindowLocation += Btvm_OnRequestedWindowLocation;
            btvm.AnyPlaingJS += Btvm_AnyPlaingJS;
            btvm.OnOpenNewTabToUrl += Btvm_OnOpenNewTabToUrl;
            //btvm.OnOpenNewTab += Btvm_OnOpenNewTab;
            //btvm.OnChangeTabContext += Btvm_OnChangeTabContext;
            btvm.OnCloseTab += Btvm_OnCloseTab;
        }

        private void removeBTVMEvents(FoxTabViewModel btvm)
        {
            btvm.OnCreateNewTab -= btvm_OnCreateNewTab;
            btvm.OnCurateToPBN -= Btvm_OnCurateToPBN;
            btvm.OnAddedToGoViral -= Btvm_OnAddedToGoViral;
            btvm.OnClickedSaveSession -= Btvm_OnClickedSaveSession;
            btvm.OnSetUserAgent -= Btvm_OnSetUserAgent;
            btvm.OnClickedDeleteSession -= Btvm_OnClickedDeleteSession;
            btvm.OnClickedSaveSessionToBookmarks -= Btvm_OnClickedSaveSessionToBookmarks;
            btvm.OnClickedReminders -= Btvm_OnClickedReminders;
            //btvm.OnRefreshTabSettingsTab += Btvm_OnRefreshTabSettings;
            btvm.OnRefreshSessionSettings -= Btvm_OnRefreshSessionSettings;
            btvm.OnSentForSeo -= Btvm_OnSentForSeo;
            btvm.OnRequestedWindowLocation -= Btvm_OnRequestedWindowLocation;
            btvm.AnyPlaingJS -= Btvm_AnyPlaingJS;
            btvm.OnOpenNewTabToUrl -= Btvm_OnOpenNewTabToUrl;
            //btvm.OnOpenNewTab -= Btvm_OnOpenNewTab;
            //btvm.OnChangeTabContext -= Btvm_OnChangeTabContext;
            btvm.OnCloseTab -= Btvm_OnCloseTab;
        }

        private bool Btvm_AnyPlaingJS()
        {
            return BrowserTabs.Any(b=>b.runningInJsMode);
        }

        private void Btvm_OnRequestedWindowLocation()
        {
            OnRequestedWindowLocation();
        }


        #region btvm events
        private void Btvm_OnCloseTab(bool allOthers, FoxTabViewModel tab)
        {
            if (allOthers)
            {
                for (int i = BrowserTabs.Count - 1; i >= 0; i--)
                {
                    var thatTab = BrowserTabs[i];
                    if (thatTab == tab) continue;

                    removeBTVMEvents(thatTab);
                    thatTab.Dispose();
                    BrowserTabs.Remove(thatTab);
                }
            }
            else
            {
                removeBTVMEvents(tab);
                tab.Dispose();
                BrowserTabs.Remove(tab);

                if (BrowserTabs.Count > 0)
                {
                    BrowserTabs[0].TabMargin = new Thickness(-3, 0, 0, 0);
                    //ChangeToExistingContext(BrowserTabs.Count, tab);
                }

                tab = null;
            }

            GC.Collect();
        }

        //FoxTabViewModel switchedTab = null;
        //FoxTabViewModel tab = null;
        //private void ChangeToExistingContext(int index, FoxTabViewModel ttab)
        //{
        //    try
        //    {
        //        ttab.macroPlayer.StopRequested = true;
        //        browserHost.TabControl.SelectedIndex = index - 1;
        //        switchedTab = BrowserTabs[browserHost.TabControl.SelectedIndex];
        //        tab = ttab;
        //        tab.macroIndex += 1;
        //        switchedTab.macroPlayer.StopRequested = false;

        //        if (switchedTab.macroPlayer == null)
        //        {
        //            switchedTab.OnViewLoaded -= SwitchedTab_OnViewLoaded;
        //            switchedTab.OnViewLoaded += SwitchedTab_OnViewLoaded;
        //        }
        //        else
        //        {
        //            SwitchedTab_OnViewLoaded();
        //        }
        //    }
        //    catch { }
        //}

        //private void SwitchedTab_OnViewLoaded()
        //{
        //    switchedTab.OnInitializedMacrosFromView -= SwitchedTab_OnInitializedMacrosFromView;
        //    switchedTab.OnInitializedMacrosFromView += SwitchedTab_OnInitializedMacrosFromView;
        //    switchedTab.RaiseInitializeMacrosRequest();
        //}

        //private async void SwitchedTab_OnInitializedMacrosFromView(MacroManger m)
        //{
        //    switchedTab.macroPlayer = m;
        //    switchedTab.macroPlayer.CurrentLoopPos = tab.macroPlayer.CurrentLoopPos;
        //    switchedTab.macroPlayer.CurrentJSDatasourceLoopPos = tab.macroPlayer.CurrentJSDatasourceLoopPos;
        //    switchedTab.macroPlayer.MaxLoop = tab.macroPlayer.MaxLoop;
        //    switchedTab.isSettingFromTab = true;
        //    switchedTab.dataCourceLine = tab.dataCourceLine;
        //    switchedTab.iTimesToRun = tab.iTimesToRun;
        //    switchedTab.macroIndex = tab.macroIndex;
        //    switchedTab.macVals = tab.macVals;
        //    switchedTab.macroWaitingFor = tab.macroWaitingFor;
        //    string path = tab.macroPlayer.SelectedMacroPlayingFilePath;
        //    await switchedTab.macroPlayer.SetMacroActiveByPaths(new string[] { path }.ToList());
        //}

        //private void Btvm_OnChangeTabContext(int index, FoxTabViewModel ttab)
        //{
        //    try
        //    {
        //        if (browserHost.TabControl.SelectedIndex == index - 1) return;

        //        ttab.macroPlayer.StopRequested = true;
        //        browserHost.TabControl.SelectedIndex = index - 1;
        //        switchedTab = BrowserTabs[browserHost.TabControl.SelectedIndex];
        //        tab = ttab;

        //        if (switchedTab.macroPlayer == null)
        //        {
        //            switchedTab.OnViewLoaded -= SwitchedTab_OnViewLoaded;
        //            switchedTab.OnViewLoaded += SwitchedTab_OnViewLoaded;
        //        }
        //        else
        //        {
        //            SwitchedTab_OnViewLoaded();
        //        }
        //    }
        //    catch { }
        //}

        //private void Btvm_OnOpenNewTab()
        //{
        //    CreateNewTab("", false);
        //}

        private void Btvm_OnRefreshTabSettings(FoxTabViewModel tab)
        {
            BrowserTabs.Remove(tab);

            FoxTabViewModel btvm = new FoxTabViewModel(tab.AddressEditable, false);
            btvm.Title = tab.AddressEditable;
            if (BrowserTabs.Count > 0)
                btvm.TabMargin = new Thickness(-20, 0, 0, 0);
            else
                btvm.TabMargin = new Thickness(-3, 0, 0, 0);
            setBTVMEvents(btvm);
            //for settings
            btvm.JavaEnabled = tab.JavaEnabled;
            btvm.JavascriptEnabled = tab.JavascriptEnabled;
            btvm.FlashEnabled = tab.FlashEnabled;
            btvm.SetBrowser(tab.AddressEditable);

            BrowserTabs.Add(btvm);
            browserHost.TabControl.SelectedItem = btvm;
        }

        private void Btvm_OnRefreshSessionSettings()
        {
            refreshGsettings();

            foreach (FoxTabViewModel btvm in BrowserTabs)
            {
                btvm.Dispose();
            }

            List<FoxTabViewModel> tmpList = new List<FoxTabViewModel>(BrowserTabs);
            BrowserTabs.Clear();
            foreach (FoxTabViewModel btvm in tmpList)
            {
                CreateNewTab(btvm.AddressEditable);
            }

            tmpList.Clear();
            OnRefreshedSessionSettings();
        }

        public void GotScreenCords(string message)
        {
            SelectedTab.GotScreenCords(message);
        }

        private void refreshGsettings()
        {
            //if(Thread.CurrentThread != Dispatcher.Thread)
            //{
            //    Dispatcher.BeginInvoke(new Action(refreshGsettings));
            //    return;
            //}
            Application.Current.Dispatcher.Invoke(() =>
            {
                FoxInit.SetSettings();
            });
        }
        private void Btvm_OnSetUserAgent(string agent)
        {
            BrowserSettimgs.UserAgentFF = agent;   
            Btvm_OnRefreshSessionSettings();
        }

        private void Btvm_OnClickedSaveSessionToBookmarks()
        {
            List<string> links = new List<string>();

            foreach (FoxTabViewModel btvm in BrowserTabs)
            {
                links.Add(btvm.AddressEditable);
            }

            DragDropMainViewModel.Instance.SaveSession(links);
        }

        private void Btvm_OnClickedDeleteSession()
        {
            MyFilesDatabase.DeleteSession(GloableProfData.PData.ProjectName, true);
        }

        private void Btvm_OnClickedSaveSession()
        {
            List<string> links = new List<string>();

            foreach (FoxTabViewModel btvm in BrowserTabs)
            {
                links.Add(btvm.AddressEditable);
            }

            MyFilesDatabase.SaveSession(GloableProfData.PData.ProjectName, links, true);
        }

        void btvm_OnCreateNewTab(string webSite)
        {
            CreateNewTab(webSite);
        }


        private void Btvm_OnOpenNewTabToUrl(string webSite)
        {
            CreateNewTab(webSite);
        }

        private void Btvm_OnCurateToPBN(string content, string link)
        {
            OnCurateToPBN(content, link);
        }

        private void Btvm_OnAddedToGoViral(string link, string type, List<string> multilinks)
        {
            OnAddedToGoViral(link, type, multilinks);
        }

        private void Btvm_OnSentForSeo(string name, string url)
        {
            OnSentForSeo(name, url);
        }

        private void Btvm_OnClickedReminders()
        {
            OnClickedReminders();
        }
        #endregion

        private void browserHost_OnContentRenderd()
        {
            CheckAndSetOpenTabs();
        }
        private void CheckAndSetOpenTabs()
        {
            Task.Factory.StartNew(() =>
            {
                List<string> sites = MyFilesDatabase.GetSavedSesstion(GloableProfData.PData.ProjectName, true);
                System.Threading.Thread.Sleep(350);
                refreshGsettings();
                Instance_OnSelsectedLauncAll(sites.ToArray());
            });
        }

        private async void Instance_OnSelsectedLauncAll(string[] sites)
        {
            foreach (string site in sites)
            {
                if (site.Contains(",") && !site.Contains(".")) continue;
                CreateNewTab(site);
                if (!GloableProfData.PData.ProxyIP.IsNullOrEmpty())
                {
                    await Task.Run(()=>{
                        while (!FoxInit.DidsetProxy)
                        {
                            Thread.Sleep(500);
                        }
                        Thread.Sleep(250);
                    });
                }
            }
        }

        void Instance_OnDoubleClickedSite(string site)
        {

            BrowserTabs[browserHost.TabControl.SelectedIndex].NavigateToSelectedSite(site);
            // btvm_OnCreateNewTab(site, true);
        }

        public void SetBookmarksEvents(bool set)
        {
            DragDropMainViewModel.Instance.OnDoubleClickedSite -= Instance_OnDoubleClickedSite;
            DragDropMainViewModel.Instance.OnSelsectedLauncAll -= Instance_OnSelsectedLauncAll;

            //MacroManger.OnPlayMacro -= MacroManager_OnPlayMacro; 

            if (set)
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite += Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll += Instance_OnSelsectedLauncAll;

               // MacroManger.OnPlayMacro += MacroManager_OnPlayMacro;
            }
            else
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite -= Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll -= Instance_OnSelsectedLauncAll;

               // MacroManger.OnPlayMacro -= MacroManager_OnPlayMacro;
            }
        }

        public void SearchFor(string query)
        {
            CreateNewTab(query);
            browserHost.TabControl.SelectedIndex = browserHost.TabControl.Items.Count - 1;
        }

        public void LaunchNewWindow(string link, string rssLink)
        {
            GeckoWebBrowser ffpopupMacrosBrowser = new GeckoWebBrowser();
            ffpopupMacrosBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            ffpopupMacrosBrowser.Navigate(link);

            FFBrowserPopup ffpopupMacros = new FFBrowserPopup();
            ffpopupMacros.Text = rssLink;
            ffpopupMacros.SuspendLayout();
            ffpopupMacros.Controls.Add(ffpopupMacrosBrowser);
            ffpopupMacros.ResumeLayout(false);
            ffpopupMacros.PerformLayout();
            ffpopupMacros.Show();
        }




        //private async void MacroManager_OnPlayMacro(MacroManger mPlayer, IIMPlayType isiim, int times)
        //{
        //    if (SelectedTab == null) return;

        //   await SelectedTab.RunMacro(mPlayer, isiim, times);
        //}
    }
}
