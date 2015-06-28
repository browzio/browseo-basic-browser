using BrowserHost;
using BrowserHost.Models;
using Organiser.Common;
using Organiser.Common.Classes;
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
using WindowsInput;
using WpfCefDynamBrowser.ViewModels;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;

namespace WpfCefDynamBrowser.Views
{
    /// <summary>
    /// Interaction logic for BrowserTabView.xaml
    /// </summary>
    public partial class BrowserTabView : UserControl
    {
        private List<string> sites;
        public BrowserTabView()
        {
            InitializeComponent();
            new System.Threading.Thread(() =>
            {
                sites = MyFilesDatabase.GetSites();
            }).Start();

            //dragnDropTreeview.OnLaunchSite += dragnDropTreeview_OnLaunchSite;
            this.Loaded += BrowserTabView_Loaded;
        }

        void BrowserTabView_Loaded(object sender, RoutedEventArgs e)
        {
            dragnDropListview.vm.ProjectName = BrowserInit.pData.ProjectName;
            dragnDropListview.vm.FillList();
            dragnDropListview.vm.OnDoubleClickedSite += vm_OnDoubleClickedSite;
            dragnDropListview.vm.OnListChanged += vm_OnListChanged;
            dragnDropListview.vm.MigrateOldSites();//TODO: takeOut

            (DataContext as BrowserTabViewModel).OnRefreshBookmarksList += BrowserTabView_OnRefreshBookmarksList;
        }

        void vm_OnListChanged()
        {
            (DataContext as BrowserTabViewModel).RaiseAddedBookmark();
        }

        void BrowserTabView_OnRefreshBookmarksList()
        {
            dragnDropListview.vm.RefreshList();
        }

        void vm_OnDoubleClickedSite(string site)
        {
            (DataContext as BrowserTabViewModel).NavigateToSelectedSite(site);
        }

        void dragnDropTreeview_OnLaunchSite(string site)
        {
            (DataContext as BrowserTabViewModel).NavigateToSelectedSite(site);
        }

        private void OnTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void OnTextBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void openFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (!flyOut.IsOpen)
            {
                host.Width = host.ActualWidth - flyOut.ActualWidth;
                host.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                flyOut.IsOpen = true;
            }
        }

        private void flyOut_ClosingFinished(object sender, RoutedEventArgs e)
        {
            host.Width = browserGrd.ActualWidth;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            host.Width = browserGrd.ActualWidth;
            host.Height = browserGrd.ActualHeight;
            if (flyOut.IsOpen && flyOut.IsLoaded)
            {
                host.Width = browserGrd.ActualWidth - (flyOut.ActualWidth);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            KeyUp += OnKeyUp;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
            }
        }

        private void cmbSites_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back) return;

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                new System.Threading.Thread(() =>
                {
                    sites = MyFilesDatabase.GetSites();
                }).Start();
                return;
            }

            string curtext = cmbSites.Text;

            cmbSites.Items.Clear();
            if (!cmbSites.IsDropDownOpen && sites.Count > 0) cmbSites.IsDropDownOpen = true;

            foreach (string site in sites)
            {
                if (site.Contains(curtext))
                    cmbSites.Items.Add(site);
            }
        }

        private void SaveSite_Click(object sender, RoutedEventArgs e)
        {
            dragnDropListview.SaveSite(cmbSites.Text);
        }

        private void btnExportAllBookmarks_Click(object sender, RoutedEventArgs e)
        {
            dragnDropListview.EportSitesToTxt();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (contextMnuBtn == null) return;
            (contextMnuBtn as Button).ContextMenu.IsOpen = false;
        }

        object contextMnuBtn;
        private void Share_Click(object sender, RoutedEventArgs e)
        {
            contextMnuBtn = sender;
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void btnImpotBookmarks_Click(object sender, RoutedEventArgs e)
        {
            SelectProfileWindow spw = new SelectProfileWindow();
            spw.Title = "Select Project";
            spw.ShowDialog();
            if (spw.OkClicked)
            {
                dragnDropListview.vm.MergeBookMarksFromProjectPath(spw.SelectedProjectName);
            }
        }
    }
}
