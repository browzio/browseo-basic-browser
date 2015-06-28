using Organiser.Common.Classes;
using Organiser.Common.Windows;
using RssReader.Windows;
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

namespace RssReader.Controlls
{
    /// <summary>
    /// Interaction logic for RssTabbedControl.xaml
    /// </summary>
    public partial class RssTabbedControl : UserControl
    {
        public event Action<string, string> OnLaunchToBrowser = delegate { };//link, rsslink
        public event Action<string> OnLaunchToTabBrowser = delegate { };//url

        public ObservableCollection<MainViewModel> UserRssTabs { get; set; }

        private PersonData mProfile;

        public RssTabbedControl()
        {
            InitializeComponent();

            DataContext = this;
            
            UserRssTabs = new ObservableCollection<MainViewModel>();
            
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));

            var bc = new BrushConverter();
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {

        }

        private void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (UserRssTabs.Count > 0)
            {
                try
                {
                    var originalSource = (FrameworkElement)e.OriginalSource;

                    MainViewModel vm = (MainViewModel)originalSource.DataContext;
                    if (vm != null)
                    {
                        UserRssTabs.Remove(vm);
                        MyFilesDatabase.RemoveDeleteRssTab(mProfile, vm.TabTitle);
                    }
                }
                catch { }
            }
        }

        private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewTab();

            tbContrl.SelectedIndex = tbContrl.Items.Count - 1;
        }

        private void CreateNewTab()
        {
            AddLinkDataWindow stnw = new AddLinkDataWindow();
            stnw.Title = "Create New Tab";
            stnw.tblockInfo.Text = "Name for tab.";
            stnw.ShowDialog();
            if (stnw.OkClicked)
            {
                foreach (MainViewModel mvm in UserRssTabs)
                {
                    if (mvm.TabTitle == stnw.tbInputText.Text)
                    {
                        MessageBox.Show("There already exists a tab with this name.");
                        return;
                    }
                }

                MainViewModel vm = new MainViewModel() { TabTitle = stnw.tbInputText.Text };
                vm.OnLaunchToBrowser += vm_OnLaunchToBrowser;
                vm.OnLaunchToTabBrowser += vm_OnLaunchToTabBrowser;
                vm.OnImportedTab += vm_OnImportedTab;
                vm.SetProfileData(mProfile);
                UserRssTabs.Add(vm);
            }
        }

        void vm_OnImportedTab(string tabTitle, List<string> linksList)
        {
            MainViewModel vm = new MainViewModel() { TabTitle = tabTitle };
            vm.OnLaunchToBrowser += vm_OnLaunchToBrowser;
            vm.OnLaunchToTabBrowser += vm_OnLaunchToTabBrowser;
            vm.OnImportedTab += vm_OnImportedTab;
            vm.SetProfileData(mProfile);
            vm.setLinks(linksList);
            UserRssTabs.Add(vm);
        }

        public void SetProfileData(PersonData profile)
        {
            if (mProfile == null)
            {
                mProfile = profile;
                List<string> tabList = MyFilesDatabase.GetRssFeedLinksTabsTitle(mProfile);
                foreach (string tabTitle in tabList)
                {
                    MainViewModel vm = new MainViewModel() { TabTitle = tabTitle };
                    vm.OnLaunchToBrowser += vm_OnLaunchToBrowser;
                    vm.OnLaunchToTabBrowser += vm_OnLaunchToTabBrowser;
                    vm.OnImportedTab += vm_OnImportedTab;
                    vm.SetProfileData(mProfile);
                    UserRssTabs.Add(vm);
                }
            }
        }

        void vm_OnLaunchToTabBrowser(string url)
        {
            OnLaunchToTabBrowser(url);
        }

        void vm_OnLaunchToBrowser(string link, string rssLink)
        {
            OnLaunchToBrowser(link, rssLink);
        }
    }
}
