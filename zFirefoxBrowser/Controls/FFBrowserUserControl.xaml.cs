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
using zFirefoxBrowser.ViewModels;

namespace zFirefoxBrowser.Controls
{
    /// <summary>
    /// Interaction logic for FFBrowserUserControl.xaml
    /// </summary>
    public partial class FFBrowserUserControl : UserControl
    {
        public ObservableCollection<FoxTabViewModel> BrowserTabs { get; set; }

        public event Action<string, string> OnCurateToPBN = delegate { };
        public event Action<string, string, List<string>> OnAddedToGoViral = delegate { };//link,type,multi
        public event Action OnRefreshedSessionSettings = delegate { };
        public event Action OnClickedReminders = delegate { };
        public event Action<string, string> OnSentForSeo = delegate { };//name,url

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
                }
            }
            catch { }
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
        private void CreateNewTab(string url)
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

               browserHost.TabControl.SelectedIndex = browserHost.TabControl.Items.Count - 1;
            });
        }

        private void setBTVMEvents(FoxTabViewModel btvm)
        {
            btvm.OnCreateNewTab += btvm_OnCreateNewTab;
            btvm.OnCurateToPBN += Btvm_OnCurateToPBN;
            btvm.OnAddedToGoViral += Btvm_OnAddedToGoViral;
            btvm.OnClickedSaveSession += Btvm_OnClickedSaveSession;
            btvm.OnClickedDeleteSession += Btvm_OnClickedDeleteSession;
            btvm.OnClickedSaveSessionToBookmarks += Btvm_OnClickedSaveSessionToBookmarks;
            btvm.OnClickedReminders += Btvm_OnClickedReminders;
            //btvm.OnRefreshTabSettingsTab += Btvm_OnRefreshTabSettings;
            btvm.OnRefreshSessionSettings += Btvm_OnRefreshSessionSettings;
            btvm.OnSentForSeo += Btvm_OnSentForSeo;
        }
        #region btvm events
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
        private void refreshGsettings()
        {
            if(Thread.CurrentThread != Dispatcher.Thread)
            {
                Dispatcher.BeginInvoke(new Action(refreshGsettings));
                return;
            }
            if (!BrowserSettimgs.DoNotTrackEnabled)
            {
                GeckoPreferences.Default["privacy.donottrackheader.enabled"] = true;
                GeckoPreferences.Default["services.sync.prefs.sync.privacy.donottrackheader.enabled"] = true;
            }
            else
            {
                GeckoPreferences.Default["privacy.donottrackheader.enabled"] = false;
                GeckoPreferences.Default["services.sync.prefs.sync.privacy.donottrackheader.enabled"] = false;
            }

            if (BrowserSettimgs.FlashEnabled)
            {
                //plugin.state.flash
                GeckoPreferences.Default["plugin.state.flash"] = 2;
            }
            else
            {
                GeckoPreferences.Default["plugin.state.flash"] = 0;
            }

            if (BrowserSettimgs.JavaEnabled)
            {
                //plugin.state.java;1
                GeckoPreferences.Default["plugin.state.java"] = 1;
            }
            else
            {
                GeckoPreferences.Default["plugin.state.java"] = 0;
            }

            if (BrowserSettimgs.JavascriptEnabled)
            {
                //javascript.enabled;true
                GeckoPreferences.Default["javascript.enabled"] = true;
            }
            else
            {
                GeckoPreferences.Default["javascript.enabled"] = false;
            }

            if (BrowserSettimgs.WebRTCEnabled)
            {
                GeckoPreferences.Default["media.peerconnection.enabled"] = true;
            }
            else
            {
                GeckoPreferences.Default["media.peerconnection.enabled"] = false;
            }
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
            MyFilesDatabase.DeleteSession(GloableProfData.PData.ProjectName);
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

        private void Instance_OnSelsectedLauncAll(string[] sites)
        {
            foreach (string site in sites)
            {
                CreateNewTab(site);
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

            if (set)
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite += Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll += Instance_OnSelsectedLauncAll;
            }
            else
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite -= Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll -= Instance_OnSelsectedLauncAll;
            }
        }
    }
}
