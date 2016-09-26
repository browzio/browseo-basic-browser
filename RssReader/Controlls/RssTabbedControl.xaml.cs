using Organiser.Common;
using Organiser.Common.Classes;
using Organiser.Common.ViewModels;
using Organiser.Common.Windows;
using RssReader.Helpers;
using RssReader.Models;
using RssReader.Mvvm;
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
        public event Action<string> OnLaunchToMasher = delegate { };//url
        public event Action<string, string> OnSelectedSendToSeo = delegate { };//title,url
        public event Action<string, string, string, string, string> OnSelectedSendToPbn = delegate { };//send to MAsher

        public ObservableCollection<MainViewModel> UserRssTabs { get; set; }

        public ICommand SelectFolderSelect_Click { get; set; }
        ObservableCollection<AvailableTabsAndLinks> AvailrssesForImports;

        public RssTabbedControl()
        {
            InitializeComponent();

            DataContext = this;
            
            UserRssTabs = new ObservableCollection<MainViewModel>();
            
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenRssImport));

            var bc = new BrushConverter();

            AvailrssesForImports = new ObservableCollection<AvailableTabsAndLinks>();
            SelectFolderSelect_Click = new RelayCommand(OnSelectFolderSelect_Click);
        }

        #region importing

        private void OpenRssImport(object sender, ExecutedRoutedEventArgs e)
        {
            ImportBookmarks();
        }

        public async void ImportBookmarks()
        {
            try
            {
                //SelectProfileWindow spw = new SelectProfileWindow();
                //spw.Title = "Select Project";
                //spw.ShowDialog();
                //if (spw.OkClicked)
                //{
                //    AvailrssesForImports.Clear();
                //    foreach (string tabTitle in MyFilesDatabase.GetRssFeedLinksTabsTitlesByName(spw.SelectedProjectName))
                //    {
                //        AvailrssesForImports.Add(new AvailableTabsAndLinks() { Name = tabTitle });
                //    }

                //    ChooseFolderWindow cfw = new ChooseFolderWindow();
                //    cfw.DataContext = this;
                //    cfw.lstItems.ItemsSource = AvailrssesForImports;
                //    cfw.ShowDialog();
                //    if (cfw.OkClicked)
                //    {
                //        foreach (AvailableTabsAndLinks availTabs in AvailrssesForImports)
                //        {
                //            if (availTabs.IsChecked)
                //            {
                //                OnImportedTab(availTabs.Name, MyFilesDatabase.GetRssFeedLinks(spw.SelectedProjectName, availTabs.Name));
                //            }
                //        }
                //    }
                //}

                ChooseProjectsVM cpvm = new ChooseProjectsVM();
                await cpvm.InitProjectsWindowList();
                if (cpvm.ShowListWindowDialog())
                {
                    foreach (var sp in cpvm.SavedProjectsListAdded)
                    {
                        if (!sp.IsChecked || sp.IsFolder) continue;
                        var listNeeded = await Task.Run(()=> { return MyFilesDatabase.GetRssFeedLinksTabsTitlesByName(sp.Name); });
                        AvailrssesForImports.Clear();
                        foreach (string tabTitle in listNeeded)
                        {
                            AvailrssesForImports.Add(new AvailableTabsAndLinks() { Name = tabTitle });
                        }

                        ChooseFolderWindow cfw = new ChooseFolderWindow();
                        cfw.DataContext = this;
                        cfw.lstItems.ItemsSource = AvailrssesForImports;
                        cfw.ShowDialog();
                        if (cfw.OkClicked)
                        {
                            foreach (AvailableTabsAndLinks availTabs in AvailrssesForImports)
                            {
                                if (availTabs.IsChecked)
                                {
                                    var feeds = await Task.Run(() => { return MyFilesDatabase.GetRssFeedLinks(sp.Name, availTabs.Name); });
                                    OnImportedTab(availTabs.Name, feeds);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wow Something went wrong here try again please. " + ex.Message);
                return;
            }
        }

        private void OnSelectFolderSelect_Click(object param)
        {
            switch ((string)param)
            {
                case "All":
                    foreach (AvailableTabsAndLinks folder in AvailrssesForImports)
                    {
                        folder.IsChecked = true;
                    }
                    break;

                case "None":
                    foreach (AvailableTabsAndLinks folder in AvailrssesForImports)
                    {
                        folder.IsChecked = false;
                    }
                    break;
                default:
                    break;
            }
        }

        void OnImportedTab(string tabTitle, List<string> linksList)
        {
            if (UserRssTabs.Count > 0)
                foreach (MainViewModel rssmvm in UserRssTabs)
                    if (rssmvm.TabTitle == tabTitle)
                    {
                        MessageBox.Show("Tab " + tabTitle + " already exists.");
                        return;
                    }

            MainViewModel vm = GetNewVM(tabTitle,true);
            vm.setLinks(linksList);
            UserRssTabs.Add(vm);
        }

        private void Vm_OnSelectedSendToPbn(string link, string title, string imglink, string date, string description)
        {
            OnSelectedSendToPbn(link, title, imglink, date, description);
        }

        void vm_OnLaunchToTabMasher(string link)
        {
            OnLaunchToMasher(link);
        }

        #endregion

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
                        MyFilesDatabase.RemoveDeleteRssTab(GloableProfData.PData, vm.TabTitle);
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
            SetNameAndDataWindow stnw = new SetNameAndDataWindow();
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

                
                UserRssTabs.Add(GetNewVM(stnw.tbInputText.Text, false));
            }
        }

        private MainViewModel GetNewVM(string tabTitle, bool refresh)
        {
            MainViewModel vm = new MainViewModel() { TabTitle = tabTitle };
            vm.OnLaunchToBrowser += vm_OnLaunchToBrowser;
            vm.OnLaunchToTabBrowser += vm_OnLaunchToTabBrowser;
            vm.OnLaunchToTabMasher += vm_OnLaunchToTabMasher;
            vm.OnSelectedSendToPbn += Vm_OnSelectedSendToPbn;
            vm.OnSelectedSendToSeo += Vm_OnSelectedSendToSeo;
             
            if(refresh)
                vm.RefreshRssFeed(false);
            return vm;
        }

        private void Vm_OnSelectedSendToSeo(string title, string url)
        {
            OnSelectedSendToSeo(title, url);
        }

        public void InitTabs()
        {
            List<string> tabList = MyFilesDatabase.GetRssFeedLinksTabsTitle(GloableProfData.PData);
            foreach (string tabTitle in tabList)
            {
                UserRssTabs.Add(GetNewVM(tabTitle, false));
            }
            if (UserRssTabs.Count > 0)
                UserRssTabs[0].RefreshRssFeed(false);
        }



        void vm_OnLaunchToTabBrowser(string url)
        {
            OnLaunchToTabBrowser(url);
        }

        void vm_OnLaunchToBrowser(string link, string rssLink)
        {
            OnLaunchToBrowser(link, rssLink);
        }

        private void tbContrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UserRssTabs[tbContrl.SelectedIndex].RefreshRssFeed(true);
            }
            catch { }
        }
    }
}
