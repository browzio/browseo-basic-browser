using Browser.Common.ViewModels;
using DragDropListview;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Browser.Common.Views
{
    /// <summary>
    /// Interaction logic for BrowserTabView.xaml
    /// </summary>
    public partial class BrowserTabView : UserControl
    {
        public BrowserTabView()
        {
            InitializeComponent();

            this.Loaded += BrowserTabView_Loaded;
            this.SizeChanged += UserControl_SizeChanged;
        }

        void BrowserTabView_Loaded(object sender, RoutedEventArgs e)
        {
            //DragDropMainViewModel.Instance.OnHasReminders += vm_OnHasReminders;
            //DragDropMainViewModel.Instance.GetRemindersCountAndNotify();

            

            //host.Width = browserGrd.ActualWidth;
            //host.Height = browserGrd.ActualHeight;

            //Console.WriteLine(host.Width);
            //Console.WriteLine(host.Height);

            //Console.WriteLine();

            //Console.WriteLine(browserGrd.ActualWidth);
            //Console.WriteLine(browserGrd.ActualHeight);

            //Console.WriteLine();

            //Console.WriteLine(rctangle.ActualWidth);
            //Console.WriteLine(rctangle.ActualHeight);

            if (DataContext is BrowserTabViewModel)
            {
                (DataContext as BrowserTabViewModel).OnShouldChangePropertyAddress += BrowserTabView_OnShouldChangePropertyAddress;
                (DataContext as BrowserTabViewModel).OnInitializeMacrosRequest += BrowserTabView_OnInitializeMacrosRequest;
               (DataContext as BrowserTabViewModel).RevalidateSizes(host.Width, host.Height);
                (DataContext as BrowserTabViewModel).RaiseOnViewLoaded();
            }


        //private static Action EmptyDelegate = delegate () { };
        //host.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        //this.InvalidateVisual();
        //this.UpdateLayout();
        //this.Dispatcher.Invoke(EmptyDelegate, DispatcherPriority.Render);
    }

        private void SocialCrawl_Click(object sender, RoutedEventArgs e)
        {
            SocialStatsExpander.Expanded -= SocialCrawl_Click;

            if (!SocialStatsExpander.IsExpanded)
            {
                SocialStatsExpander.Visibility = Visibility.Visible;
                SocialStatsExpander.IsExpanded = true;
                host.Height = browserGrd.ActualHeight - 300;
                host.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else
            {
                collapseExpander();
            }

            SocialStatsExpander.Expanded += SocialCrawl_Click;
        }

        private void SocialStatsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            SocialStatsExpander.Collapsed -= SocialStatsExpander_Collapsed;
            collapseExpander();
            SocialStatsExpander.Collapsed += SocialStatsExpander_Collapsed;
        }

        void collapseExpander()
        {
            SocialStatsExpander.Visibility = Visibility.Collapsed;
            SocialStatsExpander.IsExpanded = false;
            host.Height = browserGrd.ActualHeight + 300;
            host.VerticalAlignment = VerticalAlignment.Stretch;
        }

        private void openFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (!flyOut.IsExpanded)
            {
                host.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                flyOut.IsExpanded = true;
                flyOut.Visibility = Visibility.Visible;
                host.Width = host.ActualWidth - 330;
               // else host.Width = host.ActualWidth - 330;
            }
        }

        private void btnCloseFlyout_Click(object sender, RoutedEventArgs e)
        {
            flyOut.IsExpanded = false;
            flyOut.Visibility = Visibility.Collapsed;
            host.Width = browserGrd.ActualWidth;
            //else host.Width = host.ActualWidth + 330;
        }

        MacroManger managerForTab;
        public event Action OnInitializedMacros = delegate { };
        //public async void openMacros_Click(object sender, RoutedEventArgs e)
        //{
        //    //System.Diagnostics.Debugger.Launch();
        //    if (!MacroflyOut.IsExpanded)
        //    {
        //        host.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        //        MacroflyOut.IsExpanded = true;
        //        MacroflyOut.Visibility = Visibility.Visible;
        //       if(host.ActualWidth > 330) host.Width = host.ActualWidth - 330;


        //        MacroflyOut.Cursor = Cursors.Wait;
        //        await MacroSettings.InitMacrosSettings();
        //        if (MacroflyOut.DataContext == null || MacroflyOut.DataContext.GetType() != typeof(MacroManger))
        //        {
        //            if (managerForTab == null)
        //            {
        //                managerForTab = new MacroManger();
        //                managerForTab.OnPlayMacro += ManagerForTab_OnPlayMacro;
        //            }
        //            MacroflyOut.DataContext = managerForTab;
        //            try
        //            {
        //                await managerForTab.LoadIMacros(false);
        //            }
        //            catch { "Failed To Load Macro List".Show(); }

        //            OnInitializedMacros();
                   
        //        }
        //        MacroflyOut.Cursor = this.Cursor;
        //    }

        //    // (DataContext as BrowserTabViewModel).RaiseOnInitializedMacrosFromView(MacroflyOut.DataContext as MacroManger);
        //}

        private void BrowserTabView_OnInitializeMacrosRequest()
        {
           // openMacros_Click(null,null);
        }

        private async void ManagerForTab_OnPlayMacro(MacroManger manger, IIMPlayType type, int loop)
        {
            if (DataContext is BrowserTabViewModel)
            {
              await  (DataContext as BrowserTabViewModel).OnPlayMacro(manger, type, loop);
            }
        }

        private void btnCloseMacroFlyout_Click(object sender, RoutedEventArgs e)
        {
            //MacroflyOut.IsExpanded = false;
            //MacroflyOut.Visibility = Visibility.Collapsed;
            //if (!flyOut.IsExpanded) host.Width = browserGrd.ActualWidth;
            //else host.Width = host.ActualWidth + 330;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            host.Width = browserGrd.ActualWidth;
            host.Height = browserGrd.ActualHeight;
            if (flyOut.IsExpanded)
            {
                host.Width = browserGrd.ActualWidth - 330;
            }

            if (SocialStatsExpander.IsExpanded && browserGrd.ActualHeight > 350)
            {
                host.Height = browserGrd.ActualHeight - 300;
            }
        }

        private void cmbSites_PreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right) return;

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                (DataContext as BrowserTabViewModel).NavigateToSelectedSite(cmbSites.textbox.Text);
                Keyboard.ClearFocus();
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
                if (cmbSites.SelectedIndex >= 0)
                    cmbSites.SelectedIndex -= 1;
                return;
            }

            string curtext = cmbSites.textbox.Text;
            string fullCurText = curtext + e.Key.ToString().ToLower();

            cmbSites.Items.Clear();

            cmbSites.Items.Add(curtext);

            foreach (string site in MyFilesDatabase.CookieSites)
            {
                if (cmbSites.Items.Count > 7) break;
                string toCheck = site.Replace("http://", "").Replace("https://", "").Replace("www.", "");
                if (toCheck.Length < curtext.Length) continue;

                if (toCheck.Contains(fullCurText))
                    cmbSites.Items.Add(site);
                //try
                //{
                //    if (toCheck.IndexOf(fullCurText, 0, fullCurText.Length) == 0)
                //        cmbSites.Items.Add(site);
                //}
                //catch { }
            }

            if (!cmbSites.IsDropDownOpen && cmbSites.Items.Count > 0)
            {
                cmbSites.IsDropDownOpen = true;

            }

            if (e.Key != Key.Enter && e.Key != Key.Left && e.Key != Key.Right) cmbSites.SelectedIndex = 0;
        }

        bool cannav = false;
        private void cmbSites_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //canNav = e.LeftButton == MouseButtonState.Pressed && cmbSites.IsDropDownOpen;
            if (e.LeftButton == MouseButtonState.Pressed && cmbSites.IsDropDownOpen &&
                !cmbSites.textbox.Text.IsNullOrEmpty())
            {
                cannav = true;
            }
        }
        private void cmbSites_OnAfterSelectionChanged()
        {
            if (cannav)
            {
                (DataContext as BrowserTabViewModel).NavigateToSelectedSite(cmbSites.textbox.Text);
            }

            cannav = false;
        }

        private void BrowserTabView_OnShouldChangePropertyAddress(string addy)
        {
            if (!cmbSites.IsKeyboardFocusWithin)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    (DataContext as BrowserTabViewModel).ChangeAddressEditable(addy);
                });
            }
        }

        private void SaveSite_Click(object sender, RoutedEventArgs e)
        {
            DragDropMainViewModel.Instance.OpenSaveSiteOptions(cmbSites.Text);
        }

        private void btnExportAllBookmarks_Click(object sender, RoutedEventArgs e)
        {
            DragDropMainViewModel.Instance.EportSitesToTxt();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            btnShare.ContextMenu.IsOpen = false;
        }

        private void btnImpotBookmarks_Click(object sender, RoutedEventArgs e)
        {
            DragDropMainViewModel.Instance.OpenImportBookmarksOptions();
        }

        private void Reminders_Click(object sender, RoutedEventArgs e)
        {
            //DragDropMainViewModel.Instance.OpenReminders();
        }

        private void BtnWithContextMenue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as Button).ContextMenu.IsEnabled = true;
                (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
                (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                (sender as Button).ContextMenu.IsOpen = true;
            }
            catch { }
        }

        private void cmBrowserSettings_Closed(object sender, RoutedEventArgs e)
        {
            BrowserTabViewModel theVM = (DataContext as BrowserTabViewModel);
            if (theVM != null)
            {
                theVM.SettingsMenuClosed();
            }
        }

        private void cmsBtn_Click(object sender, RoutedEventArgs e)
        {
            cmBrowserSettings.IsOpen = false;
        }

        private void cmBrowserSettings_Opened(object sender, RoutedEventArgs e)
        {
            BrowserTabViewModel theVM = (DataContext as BrowserTabViewModel);
            if (theVM != null)
            {
                theVM.SettingsMenuOpen();
            }
        }
    }
}

