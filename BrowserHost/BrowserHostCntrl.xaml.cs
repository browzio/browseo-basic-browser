using DragDropListview;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
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
using System.Windows.Threading;
using WpfCefDynamBrowser.ViewModels;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;

namespace BrowserHost
{
    /// <summary>
    /// Interaction logic for BrowserHost.xaml
    /// </summary>
    public partial class BrowserHostCntrl : UserControl
    {
        public ObservableCollection<BrowserTabViewModel> BrowserTabs { get; set; }

        public event Action<string, string> OnCurateToPBN = delegate { };
        public event Action<string,string,List<string>> OnAddedToGoViral = delegate { };//link,type,multi
        public event Action OnRefreshedSessionSettings = delegate { };
        public event Action OnClickedReminders = delegate { };
        public event Action<string, string> OnSentForSeo = delegate { };//name,url

        public BrowserHostCntrl()
        {
            InitializeComponent();

            DataContext = this;

            BrowserTabs = new ObservableCollection<BrowserTabViewModel>();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));

            TabControl.Loaded += TabControl_Loaded;
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            TabControl.Loaded -= TabControl_Loaded;

            PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);
            presentationSource.ContentRendered += TabControl_ContentRendered;
        }

        void TabControl_ContentRendered(object sender, EventArgs e)
        {
            // Don't forget to unsubscribe from the event
            ((PresentationSource)sender).ContentRendered -= TabControl_ContentRendered;
            CheckAndSetOpenTabs();
            // ..
        }
        
        private void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (BrowserTabs.Count > 0)
                {
                    //Obtain the original source element for this event
                    var originalSource = (FrameworkElement)e.OriginalSource;

                    BrowserTabViewModel browserViewModel = null;
                    //Remove the matching DataContext from the BrowserTabs collection
                    browserViewModel = (BrowserTabViewModel)originalSource.DataContext;
                    BrowserTabs.Remove(browserViewModel);

                    try
                    {
                        browserViewModel.WebBrowser.Dispose();
                    }
                    catch { }

                    if (BrowserTabs.Count > 0)
                        BrowserTabs[0].TabMargin = new Thickness(-3, 0, 0, 0);
                }
            }
            catch { }
        }
        
        private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewTab("");
        }

        private void CreateNewTab(string url)
        {
            MyFilesDatabase.CheckRamUsage();

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                BrowserTabViewModel btvm = new BrowserTabViewModel(url == "" ? MyFilesDatabase.GetDefultHomePage() : url);
                setBTVMEvents(btvm);
                btvm.Title = url;
                Task.Factory.StartNew(() => { btvm.ReminderCount = MyFilesDatabase.GetRemindersCount(GloableProfData.PData.ProjectName); });
                if (BrowserTabs.Count > 0)
                    btvm.TabMargin = new Thickness(-20, 0, 0, 0);
                else
                    btvm.TabMargin = new Thickness(-3, 0, 0, 0);
                BrowserTabs.Add(btvm);

                TabControl.SelectedIndex = TabControl.Items.Count - 1;
            });
        }

        private void setBTVMEvents(BrowserTabViewModel btvm)
        {
            btvm.OnCreateNewTab += btvm_OnCreateNewTab;
            btvm.OnCurateToPBN += Btvm_OnCurateToPBN;
            btvm.OnAddedToGoViral += Btvm_OnAddedToGoViral;
            btvm.OnClickedSaveSession += Btvm_OnClickedSaveSession;
            btvm.OnClickedDeleteSession += Btvm_OnClickedDeleteSession;
            btvm.OnClickedSaveSessionToBookmarks += Btvm_OnClickedSaveSessionToBookmarks;
            btvm.OnClickedReminders += Btvm_OnClickedReminders;
            btvm.OnRefreshTabSettings += Btvm_OnRefreshTabSettings;
            btvm.OnRefreshSessionSettings += Btvm_OnRefreshSessionSettings;
            btvm.OnSentForSeo += Btvm_OnSentForSeo;
        }

        #region btvm events
        private void Btvm_OnRefreshTabSettings(BrowserTabViewModel tab)
        {
            BrowserTabs.Remove(tab);

            BrowserTabViewModel btvm = new BrowserTabViewModel(tab.AddressEditable, false);
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
            TabControl.SelectedItem = btvm;
        }

        private void Btvm_OnRefreshSessionSettings()
        {
            foreach (BrowserTabViewModel btvm in BrowserTabs)
            {
                btvm.WebBrowser.Dispose();
            }

            List<BrowserTabViewModel> tmpList = new List<BrowserTabViewModel>(BrowserTabs);
            BrowserTabs.Clear();
            foreach (BrowserTabViewModel btvm in tmpList)
            {
                CreateNewTab(btvm.AddressEditable);
            }

            tmpList.Clear();
            OnRefreshedSessionSettings();
        }

        private void Btvm_OnClickedSaveSessionToBookmarks()
        {
            List<string> links = new List<string>();

            foreach (BrowserTabViewModel btvm in BrowserTabs)
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

            foreach (BrowserTabViewModel btvm in BrowserTabs)
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

        private void Btvm_OnAddedToGoViral(string link,string type, List<string> multilinks)
        {
            OnAddedToGoViral(link,type, multilinks);
        }

        private void Btvm_OnSentForSeo(string name, string url)
        {
            OnSentForSeo(name,url);
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
                BrowserTabViewModel btvm = BrowserTabs[i];
                BrowserTabs.Remove(btvm);
                try
                {
                    btvm.WebBrowser.Dispose();
                }
                catch { }
            }
        }

        public void SearchFor(string query)
        {
            CreateNewTab(query);
            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabControl tabControl = sender as TabControl;

                tabControl.Dispatcher.BeginInvoke(
                    new Action(() => UpdateZIndex(sender as TabControl)),
                    System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void UpdateZIndex(TabControl tabControl)
        {
            ItemContainerGenerator icg = tabControl.ItemContainerGenerator;

            if (icg.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                foreach (object o in tabControl.Items)
                {
                    UIElement tabItem = icg.ContainerFromItem(o) as UIElement;
                    if (tabItem != null)
                    {
                        // Set ZIndex
                        Panel.SetZIndex(tabItem, (o == tabControl.SelectedItem ? 100 :
                            90 - tabControl.Items.IndexOf(o)));
                    }
                }
            }

            //Action emptyAction = delegate { };
            //TabControl.Dispatcher.Invoke(DispatcherPriority.Render, emptyAction);
        }

        public void LaunchNewWindowToLink(string link, string rssLink)
        {
            BrowserForSocialShare bfss = new BrowserForSocialShare();
            bfss.Text = "Loading... " + rssLink;
            bfss.browserCntrl1.init(link);
            bfss.Show();
        }

        private void Sviewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;
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
                CreateNewTab(site);
            }
        }

        void Instance_OnDoubleClickedSite(string site)
        {

            BrowserTabs[TabControl.SelectedIndex].NavigateToSelectedSite(site);
           // btvm_OnCreateNewTab(site, true);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            //this.InvalidateVisual();
            //this.UpdateLayout();
            //this.Dispatcher.Invoke(emptyDelegate, DispatcherPriority.Render);
        }

        public void SetRemindersCount()
        {
            int reminderscount = MyFilesDatabase.GetRemindersCount(GloableProfData.PData.ProjectName);
            foreach (var t in BrowserTabs)
            {
                t.ReminderCount = reminderscount;
            }
        }
    }
}
