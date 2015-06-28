using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        private const string DefaultUrlForAddedTabs = "https://www.google.com";

        public ObservableCollection<BrowserTabViewModel> BrowserTabs { get; set; }

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
            CreateNewTab();

            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        private void CreateNewTab(string url = DefaultUrlForAddedTabs)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                BrowserTabViewModel btvm = new BrowserTabViewModel(url);
                btvm.OnCreateNewTab += btvm_OnCreateNewTab;
                btvm.OnAddedBookmark += btvm_OnAddedBookmark;
                if (BrowserTabs.Count > 0)
                    btvm.TabMargin = new Thickness(-20, 0, 0, 0);
                else
                    btvm.TabMargin = new Thickness(-3, 0, 0, 0);
                BrowserTabs.Add(btvm);
            });
        }

        void btvm_OnAddedBookmark()
        {
            foreach (BrowserTabViewModel btvm in BrowserTabs)
            {
                btvm.RaiseRefreshBookmarksList();
            }
        }

        void btvm_OnCreateNewTab(string obj, bool shownewTab)
        {
             App.Current.Dispatcher.Invoke((Action)delegate
            {
              CreateNewTab(obj);

              int oldindex = TabControl.SelectedIndex;
              TabControl.SelectedIndex = TabControl.Items.Count - 1;

              //if (!shownewTab)
              //{
                 // System.Threading.Thread.Sleep(2500);
                  //TabControl.SelectedIndex = oldindex;
              //}

            });
        }

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
            bfss.ShowDialog();
        }
    }
}
