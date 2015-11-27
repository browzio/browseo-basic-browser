using BrowserHost;
using BrowserHost.Models;
using BrowserHost.Views;
using Microsoft.Win32;
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

        static bool initializedddvm = false;
        void BrowserTabView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initializedddvm)
            {
                dragnDropListview.vm.ProjectName = BrowserInit.pData.ProjectName;
                dragnDropListview.vm.mPData = BrowserInit.pData;
                dragnDropListview.vm.FillList();
                dragnDropListview.vm.FillImportsList();
                //dragnDropListview.vm.OnDoubleClickedSite += vm_OnDoubleClickedSite;
                // dragnDropListview.vm.OnListChanged += vm_OnListChanged;
                //dragnDropListview.vm.OnRemindersChanged += vm_OnRemindersChanged;
                //dragnDropListview.vm.MigrateOldSites();//TODO: takeOut

                initializedddvm = true;
            }
            dragnDropListview.vm.OnHasReminders += vm_OnHasReminders;
            dragnDropListview.vm.CheckReminders();

            (DataContext as BrowserTabViewModel).OnRefreshBookmarksList += BrowserTabView_OnRefreshBookmarksList;
            (DataContext as BrowserTabViewModel).OnRefreshReminders += BrowserTabView_OnRefreshReminders;
            (DataContext as BrowserTabViewModel).OnShouldChangePropertyAddress += BrowserTabView_OnShouldChangePropertyAddress;
        }

        void BrowserTabView_OnRefreshReminders()
        {
            dragnDropListview.vm.CheckReminders();
        }

        void vm_OnRemindersChanged()
        {
            (DataContext as BrowserTabViewModel).RaiseRemindersChanged();
        }

        void vm_OnHasReminders(int notificationCount)
        {
            borderNotification.Visibility = Visibility.Visible;
            tbNotificationCount.Text = "" + notificationCount;
            if(notificationCount == 0)
                borderNotification.Visibility = Visibility.Hidden;
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
              //  System.Windows.Forms.SendKeys.Send("{ENTER}");
              //  InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
            }
        }

        private void cmbSites_PreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                (DataContext as BrowserTabViewModel).NavigateToSelectedSite(cmbSites.textbox.Text);
                Keyboard.ClearFocus();
                //new System.Threading.Thread(() =>
                //{
                //    sites = MyFilesDatabase.GetSites();
                //}).Start();
                return;
            }

            cmbSites.Focus();

            if (e.Key == Key.Down)
            {
                cmbSites.SelectedIndex += 1;
                return;
            }
            else if (e.Key == Key.Up)
            {
                if(cmbSites.SelectedIndex >=0 )
                cmbSites.SelectedIndex -= 1;
                return;
            }

            string curtext = cmbSites.Text;

            cmbSites.Items.Clear();

            cmbSites.Items.Add(curtext);

            foreach (string site in sites)
            {
                if (cmbSites.Items.Count > 7) break;
                if (site.Contains(curtext))
                    cmbSites.Items.Add(site);
            }

            if (!cmbSites.IsDropDownOpen && sites.Count > 0 && cmbSites.Items.Count > 0)
            {
                cmbSites.IsDropDownOpen = true;
                
            }
           

            if (e.Key != Key.Enter && e.Key != Key.Left && e.Key != Key.Right) cmbSites.SelectedIndex = 0;
        }

        private void BrowserTabView_OnShouldChangePropertyAddress(string addy)
        {
            if (!cmbSites.IsKeyboardFocusWithin)
            {
                (DataContext as BrowserTabViewModel).ChangeAddressEditable(addy);
            }
        }

        private void SaveSite_Click(object sender, RoutedEventArgs e)
        {
            if (dragnDropListview == null) return;
            dragnDropListview.SaveSite(cmbSites.Text);
        }

        private void btnExportAllBookmarks_Click(object sender, RoutedEventArgs e)
        {
            if (dragnDropListview == null) return;
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
            SelectBookmarkImportTypeWindow bookmarkTypeWindow = new SelectBookmarkImportTypeWindow();
            bookmarkTypeWindow.browseoGloable.Visibility = Visibility.Collapsed;
            bookmarkTypeWindow.ShowDialog();
            if(!bookmarkTypeWindow.OkClicked)return;
            if (bookmarkTypeWindow.browseoProj.IsChecked == true)
            {
                SelectProfileWindow spw = new SelectProfileWindow();
                spw.Title = "Select Project";
                spw.ShowDialog();
                if (spw.OkClicked)
                {
                    dragnDropListview.vm.MergeBookMarksFromProjectPath(spw.SelectedProjectName);
                }
            }
            else if(bookmarkTypeWindow.fcs.IsChecked == true || bookmarkTypeWindow.entBud.IsChecked == true)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.ShowDialog();
                string path = ofd.FileName;
                dragnDropListview.vm.MergeFromImport(path, bookmarkTypeWindow.fcs.IsChecked == true ? DragDropListview.DragDropMainViewModel.IMPORT_TYPE_FCS : DragDropListview.DragDropMainViewModel.IMPORT_TYPE_EB);
            }
        }

        private void Reminders_Click(object sender, RoutedEventArgs e)
        {
            dragnDropListview.vm.OpenReminders();
        }
    }
}
