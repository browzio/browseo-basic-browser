using DragDropListview;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCefDynamBrowser.ViewModels;
using Xilium.CefGlue.Client;

namespace BrowserHost
{
    /// <summary>
    /// Interaction logic for ChromeBrowserHostControl.xaml
    /// </summary>
    public partial class ChromeBrowserHostControl : UserControl
    {
        public ObservableCollection<ChromeBrowserTabViewModel> BrowserTabs { get; set; }
        public ChromeBrowserTabViewModel SelectedTab { get; set; }

        public event Action<string, string> OnCurateToPBN = delegate { };
        public event Action<string, string, List<string>> OnAddedToGoViral = delegate { };//link,type,multi
        public event Action OnRefreshedSessionSettings = delegate { };
        public event Action OnClickedReminders = delegate { };
        public event Action<string, string> OnSentForSeo = delegate { };//name,url

        public ChromeBrowserHostControl()
        {
            InitializeComponent();

            DataContext = this;

            BrowserTabs = new ObservableCollection<ChromeBrowserTabViewModel>();
        }


        private void browserHost_OnCloseTab(ExecutedRoutedEventArgs e)
        {
            try
            {
                if (BrowserTabs.Count > 0)
                {
                    //Obtain the original source element for this event
                    var originalSource = (FrameworkElement)e.OriginalSource;

                    ChromeBrowserTabViewModel browserViewModel = null;
                    //Remove the matching DataContext from the BrowserTabs collection
                    browserViewModel = (ChromeBrowserTabViewModel)originalSource.DataContext;
                    BrowserTabs.Remove(browserViewModel);

                    try
                    {
                        browserViewModel.Dispose();
                    }
                    catch { }

                    if (BrowserTabs.Count > 0)
                        BrowserTabs[0].TabMargin = new Thickness(-3, 0, 0, 0);
                }
            }
            catch { }
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
                ChromeBrowserTabViewModel btvm = new ChromeBrowserTabViewModel(url == "" ? MyFilesDatabase.GetDefultHomePage() : url);
                setBTVMEvents(btvm);
                btvm.Title = url;
                Task.Factory.StartNew(() => { btvm.ReminderCount = MyFilesDatabase.GetRemindersCount(GloableProfData.PData.ProjectName); });
                if (BrowserTabs.Count > 0)
                    btvm.TabMargin = new Thickness(-20, 0, 0, 0);
                else
                    btvm.TabMargin = new Thickness(-3, 0, 0, 0);
                BrowserTabs.Add(btvm);

                
            });
        }

        private void setBTVMEvents(ChromeBrowserTabViewModel btvm)
        {
            btvm.OnCreateNewTab += btvm_OnCreateNewTab;
            btvm.OnCurateToPBN += Btvm_OnCurateToPBN;
            btvm.OnAddedToGoViral += Btvm_OnAddedToGoViral;
            btvm.OnClickedSaveSession += Btvm_OnClickedSaveSession;
            btvm.OnSetUserAgent += Btvm_OnSetUserAgent;
            btvm.OnClickedDeleteSession += Btvm_OnClickedDeleteSession;
            btvm.OnClickedSaveSessionToBookmarks += Btvm_OnClickedSaveSessionToBookmarks;
            btvm.OnClickedReminders += Btvm_OnClickedReminders;
            btvm.OnRefreshTabSettingsTab += Btvm_OnRefreshTabSettings;
            btvm.OnRefreshSessionSettings += Btvm_OnRefreshSessionSettings;
            btvm.OnSentForSeo += Btvm_OnSentForSeo;
        }

        #region btvm events
        private void Btvm_OnRefreshTabSettings(ChromeBrowserTabViewModel tab)
        {
            BrowserTabs.Remove(tab);

            ChromeBrowserTabViewModel btvm = new ChromeBrowserTabViewModel(tab.AddressEditable, false);
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
            //foreach (ChromeBrowserTabViewModel btvm in BrowserTabs)
            //{
            //    btvm.Dispose();
            //}

            List<ChromeBrowserTabViewModel> tmpList = new List<ChromeBrowserTabViewModel>(BrowserTabs);
            BrowserTabs.Clear();
            foreach (ChromeBrowserTabViewModel btvm in tmpList)
            {
                CreateNewTab(btvm.AddressEditable);
            }

            //tmpList.Clear();
            OnRefreshedSessionSettings();
        }
        private void Btvm_OnSetUserAgent(string agent)
        {
            BrowserInit.settings.UserAgent = BrowserSettimgs.UserAgentChrome = agent;

            Btvm_OnRefreshSessionSettings();
        }

        private void Btvm_OnClickedSaveSessionToBookmarks()
        {
            List<string> links = new List<string>();

            foreach (ChromeBrowserTabViewModel btvm in BrowserTabs)
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

            foreach (ChromeBrowserTabViewModel btvm in BrowserTabs)
            {
                links.Add(btvm.AddressEditable);
            }

            MyFilesDatabase.SaveSession(GloableProfData.PData.ProjectName, links);
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

        public void CloseAllTabs()
        {
            for (int i = 0; i < BrowserTabs.Count; i++)
            {
                ChromeBrowserTabViewModel btvm = BrowserTabs[i];
                BrowserTabs.Remove(btvm);
                try
                {
                    btvm.Dispose();
                }
                catch { }
            }
        }

        public void SearchFor(string query)
        {
            CreateNewTab(query);
           browserHost.TabControl.SelectedIndex = browserHost.TabControl.Items.Count - 1;
        }

        public void LaunchNewWindowToLink(string link, string rssLink)
        {
            BrowserForSocialShare bfss = new BrowserForSocialShare();
            bfss.Text = "Loading... " + rssLink;
            bfss.browserCntrl1.init(link, BrowserSettimgs.FlashEnabled, BrowserSettimgs.JavascriptEnabled, BrowserSettimgs.JavaEnabled);
            bfss.Show();
        }

        private void CheckAndSetOpenTabs()
        {
            DragDropMainViewModel.Instance.OnDoubleClickedSite += Instance_OnDoubleClickedSite;
            DragDropMainViewModel.Instance.OnSelsectedLauncAll += Instance_OnSelsectedLauncAll;

            Task.Factory.StartNew(() =>
            {
                List<string> sites = MyFilesDatabase.GetSavedSesstion(GloableProfData.PData.ProjectName);
                Thread.Sleep(350);
                Instance_OnSelsectedLauncAll(sites.ToArray());
                //Application.Current.Dispatcher.Invoke((Action)delegate
                //{
                //    //if (sites.Count > 0)
                //    //    TabControl.SelectedIndex = -1;

                //    //Grid gridView = TabControl.ItemsPanel as Grid;
                //    //if (gridView != null)
                //    //{
                //    //    foreach (var column in gridView.Columns)
                //    //    {
                //    //        if (double.IsNaN(column.Width))
                //    //            column.Width = column.ActualWidth;
                //    //        column.Width = double.NaN;
                //    //    }
                //    //}
                //});
            });
        }

        private void Instance_OnSelsectedLauncAll(string[] sites)
        {
            foreach (string site in sites)
            {
                if (site.Contains(",")) continue;
                CreateNewTab(site);
            }
        }

        void Instance_OnDoubleClickedSite(string site)
        {

            BrowserTabs[browserHost.TabControl.SelectedIndex].NavigateToSelectedSite(site);
            // btvm_OnCreateNewTab(site, true);
        }

        public void SetRemindersCount()
        {
            int reminderscount = MyFilesDatabase.GetRemindersCount(GloableProfData.PData.ProjectName);
            foreach (var t in BrowserTabs)
            {
                t.ReminderCount = reminderscount;
            }
        }

        public void SetBookmarksEvents(bool set)
        {
            DragDropMainViewModel.Instance.OnDoubleClickedSite -= Instance_OnDoubleClickedSite;
            DragDropMainViewModel.Instance.OnSelsectedLauncAll -= Instance_OnSelsectedLauncAll;

            //MacroManger.OnPlayMacro -= MacroManger_OnPlayMacro;

            if (set)
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite += Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll += Instance_OnSelsectedLauncAll;

                //MacroManger.OnPlayMacro += MacroManger_OnPlayMacro;
            }
            else
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite -= Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll -= Instance_OnSelsectedLauncAll;

               // MacroManger.OnPlayMacro -= MacroManger_OnPlayMacro;
            }
        }

        private void MacroManger_OnPlayMacro(MacroManger macroListing,bool isiim, int times)
        {
        }

        private void browserHost_OnContentRenderd()
        {
            CheckAndSetOpenTabs();
        }
    }
}
