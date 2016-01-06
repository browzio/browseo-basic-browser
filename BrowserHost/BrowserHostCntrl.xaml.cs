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
using WpfCefDynamBrowser.ViewModels;
using Xilium.CefGlue.Client;

namespace BrowserHost
{
    /// <summary>
    /// Interaction logic for BrowserHost.xaml
    /// </summary>
    public partial class BrowserHostCntrl : UserControl
    {
       // private const string DefaultUrlForAddedTabs = "https://www.google.com/";

        public ObservableCollection<BrowserTabViewModel> BrowserTabs { get; set; }
        public event Action<string, string> OnCurateToPBN = delegate { };
        public event Action<string> OnAddedToGoViral = delegate { };//link

        public BrowserHostCntrl()
        {
            InitializeComponent();

            DataContext = this;

            BrowserTabs = new ObservableCollection<BrowserTabViewModel>();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));
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
            MyFilesDatabase.CheckRamUsage();
                                            
            CreateNewTab(MyFilesDatabase.GetDefultHomePage());

            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        private void CreateNewTab(string url)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                BrowserTabViewModel btvm = new BrowserTabViewModel(url);
                setBTVMEvents(btvm);
                btvm.Title = url;
                if (BrowserTabs.Count > 0)
                    btvm.TabMargin = new Thickness(-20, 0, 0, 0);
                else
                    btvm.TabMargin = new Thickness(-3, 0, 0, 0);
                BrowserTabs.Add(btvm);
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
            btvm.OnRefreshTabSettings += Btvm_OnRefreshTabSettings;
            btvm.OnRefreshSessionSettings += Btvm_OnRefreshSessionSettings;
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
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                CreateNewTab(webSite);
                int oldindex = TabControl.SelectedIndex;
                TabControl.SelectedIndex = TabControl.Items.Count - 1;
            });
        }

        private void Btvm_OnCurateToPBN(string content, string link)
        {
            OnCurateToPBN(content, link);
        }

        private void Btvm_OnAddedToGoViral(string link)
        {
            OnAddedToGoViral(link);
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

        public void CheckAndSetOpenTabs()
        {
            Task.Factory.StartNew(() =>
            {
                DragDropMainViewModel.Instance.OnDoubleClickedSite += Instance_OnDoubleClickedSite;
                DragDropMainViewModel.Instance.OnSelsectedLauncAll += Instance_OnSelsectedLauncAll;
                string[] sites = MyFilesDatabase.GetSavedSesstion(GloableProfData.PData.ProjectName);
                Instance_OnSelsectedLauncAll(sites);
                if (sites.Length > 0)
                    TabControl.SelectedIndex = -1;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
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
    }
}
