using DragDropListview;
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
        public event Action<string> OnCurateToPBN = delegate { };

        ulong availmem;
        int timesToCheck = 0;

        public BrowserHostCntrl()
        {
            InitializeComponent();

            DataContext = this;

            BrowserTabs = new ObservableCollection<BrowserTabViewModel>();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));

            Microsoft.VisualBasic.Devices.ComputerInfo ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            availmem = ci.AvailablePhysicalMemory;
            availmem = availmem / (1024 * 1024);
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
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
        System.Threading.Thread ramCheckerThread;
        private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (timesToCheck++ >= 5)
            {
                if (ramCheckerThread == null || !ramCheckerThread.IsAlive)
                {
                    ramCheckerThread = new System.Threading.Thread(() =>
                    {
                        double total = 0;
                        bool showedMSgBox = false;
                        foreach (System.Diagnostics.Process process in System.Diagnostics.Process.GetProcessesByName("BrowserAndFeatures"))
                        {
                            var counter = new System.Diagnostics.PerformanceCounter("Process", "Working Set - Private", process.ProcessName);
                            total += counter.RawValue / (1024 * 1024);
                            if ((availmem - total) < 350)
                            {
                                if (!showedMSgBox)
                               Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    MessageBox.Show(
                                        "You have only " + (availmem - total) + "mb of ram space left please close down other applications" +
                                        " to free up ram before continuing. Or refrain from openning more tabs, keep in mind" +
                                        " you will risk your computer and Browseo's performance.");
                                });
                                showedMSgBox = true;
                            }
                        }
                    });
                    ramCheckerThread.Start();
                }
                timesToCheck = 0;
            }

            CreateNewTab();

            //if (TabControl.Items.Count == 1)
            //{
            //    ItemContainerGenerator icg = TabControl.ItemContainerGenerator;
            //    UIElement tabItem = icg.ContainerFromItem(TabControl.Items[0]) as UIElement;
            //    (tabItem as TabItem).Margin = new Thickness(-12, 0, 0, 0);

            //    //Type t = (FindResource("addButtonStyle") as Style).TargetType;
            //    //(t.BaseType as Button).Margin = new Thickness(0); 
            //   // (t as Button).Margin = new Thickness(0, 0, 0, 0);
            //}

            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        private void CreateNewTab(string url = DefaultUrlForAddedTabs)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                BrowserTabViewModel btvm = new BrowserTabViewModel(url);
                btvm.OnCreateNewTab += btvm_OnCreateNewTab;
                btvm.OnAddedBookmark += btvm_OnAddedBookmark;
                btvm.OnRemindersChanged += btvm_OnRemindersChanged;
                btvm.OnCurateToPBN += Btvm_OnCurateToPBN;
                if (BrowserTabs.Count > 0)
                    btvm.TabMargin = new Thickness(-20, 0, 0, 0);
                else
                    btvm.TabMargin = new Thickness(-3, 0, 0, 0);
                BrowserTabs.Add(btvm);

                //ScrollViewer scrollview = FindVisualChild<ScrollViewer>(TabControl);
                //if (scrollview != null)
                //{
                //    scrollview.ScrollToBottom();

                //    Button openTabBtn = FindVisualChild<Button>(TabControl);
                //    if (openTabBtn != null)
                //    {
                //        if (scrollview.ExtentHeight == 33)
                //        {
                //            openTabBtn.Margin = new Thickness(-13, 13, 0, 0);
                //        }
                //        if (scrollview.ContentVerticalOffset == 14)
                //        {
                //            openTabBtn.Margin = new Thickness(-13, -5, 0, 0);
                //        }
                //    }
                //}
                    //scrollview.ScrollToVerticalOffset(9.5);
            });
        }

        private void Btvm_OnCurateToPBN(string content)
        {
            OnCurateToPBN(content);
        }

        void btvm_OnRemindersChanged()
        {
            foreach (BrowserTabViewModel btvm in BrowserTabs)
            {
                btvm.RaiseRefreshRemindersList();
            }
        }

        void btvm_OnAddedBookmark()
        {
            foreach (BrowserTabViewModel btvm in BrowserTabs)
            {
                btvm.RaiseRefreshBookmarksList();
            }
        }

        void btvm_OnCreateNewTab(string webSite, bool shownewTab)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                CreateNewTab(webSite);

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DragDropMainViewModel.Instance.OnDoubleClickedSite += Instance_OnDoubleClickedSite;
        }

        void Instance_OnDoubleClickedSite(string site)
        {

            BrowserTabs[TabControl.SelectedIndex].NavigateToSelectedSite(site);
           // btvm_OnCreateNewTab(site, true);
        }
    }
}
