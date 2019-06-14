using Organiser.Common.Classes;
using Organiser.Common.ViewModels;
using Organiser.Common.Windows;
using RssReader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RssReader.ViewModels
{
    public class RSSMainWorkspaceViewModelMessage
    {
        public string MessageType { get; set; }

        public List<object> Parameters { get; set; }

        public RSSMainWorkspaceViewModelMessage()
        {
            Parameters = new List<object>();
        }
    }
    public class RSSMainWorkspaceViewModel : PropertyChangedViewModelBase
    {
        public event Action<RSSMainWorkspaceViewModelMessage> OnRSSMainWorkspaceViewModelMessage;

        private static RSSMainWorkspaceViewModel instance;
        public static RSSMainWorkspaceViewModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new RSSMainWorkspaceViewModel();

                return instance;
            }
        }

        public ObservableCollection<RSSTabViewModel> RSSTabs { get; set; }
        private RSSTabViewModel selectedRSSTab;
        public RSSTabViewModel SelectedRSSTab
        {
            get { return selectedRSSTab; }
            set
            {
                if (selectedRSSTab != value)
                {
                    selectedRSSTab = value;
                    if (selectedRSSTab != null && selectedRSSTab.RSSFeeds.Count == 0)
                        selectedRSSTab.FirstLoadFeeds();

                    NotifyOfPropertyChange();
                }
            }
        }
        
        private RSSMainWorkspaceViewModel()
        {
            RSSTabs = new ObservableCollection<RSSTabViewModel>();
        }

        public override void OnReceivedCommandFromView(string param)
        {
            switch (param)
            {
                case "New":
                    OnClickedCreateNewTab();
                    break;

                case "Open":
                    OnClickedImportRSSFeeds();
                    break;

                default:
                    break;
            }
        }

        private void OnClickedCreateNewTab()
        {
            SetNameAndDataWindow stnw = new SetNameAndDataWindow();
            stnw.Title = "Create New Tab";
            stnw.tblockInfo.Text = "Enter In Tab Title";
            stnw.ShowDialog();
            if (stnw.OkClicked && !stnw.tbInputText.Text.IsNullOrEmpty())
            {
                if (RSSTabs.Any(t => t.Title.Trim().ToLower() == stnw.tbInputText.Text.Trim().ToLower()))
                {
                    Show($"{stnw.tbInputText.Text} already exists.");
                    return;
                }

                RSSTabs.Add(CreateNewTab(stnw.tbInputText.Text.Trim()));
            }
        }
        
        private async void OnClickedImportRSSFeeds()
        {
            try
            {
                ChooseProjectsVM cpvm = new ChooseProjectsVM();
                await cpvm.InitProjectsWindowList();
                if (cpvm.ShowListWindowDialog())
                {
                    foreach (var sp in cpvm.SavedProjectsListAdded)
                    {
                        if (!sp.IsChecked || sp.IsFolder) continue;

                        var msvm = new MultiSelectionViewModel();

                        var listNeeded = await Task.Run(() => { return MyFilesDatabase.GetRssFeedLinksTabsTitlesByName(sp.Name); });
                        if (listNeeded.Count == 0) return;

                        foreach (string tabTitle in listNeeded)
                        {
                            msvm.SelectionList.Add(new MultiSelectionData() { Name = tabTitle });
                        }

                        if (msvm.ShowWindow("Select Import."))
                        {
                            var failedImports = new List<string>();

                            foreach (var availTab in msvm.SelectionList)
                            {
                                if (availTab.IsChecked)
                                {
                                    var feeds = await Task.Run(() => { return MyFilesDatabase.GetRssFeedLinks(sp.Name, availTab.Name); });

                                    if (RSSTabs.Any(t => t.Title.Trim().ToLower() == availTab.Name.Trim().ToLower()))
                                    {
                                        failedImports.Add(availTab.Name);
                                    }
                                    else
                                    {
                                        await Task.Run(() => { MyFilesDatabase.SaveRssFeedsSiteLinks(feeds.ToArray(), GloableProfData.PData, availTab.Name); });
                                        RSSTabs.Add(CreateNewTab(availTab.Name));
                                    }
                                }
                            }

                            if (failedImports.Count > 0)
                            {
                                Show($"The folowing tabs were not created since those tab titles already exist: {failedImports.ToLinedString()}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Show("Import failed. Error message: " + ex.Message);
                return;
            }
        }

        internal void OnClickedCloseTab(RSSTabViewModel rSSTabViewModel)
        {
            RSSTabs.Remove(rSSTabViewModel);
            Task.Run(() => { MyFilesDatabase.RemoveDeleteRssTab(GloableProfData.PData, rSSTabViewModel.Title); });
        }

        public async void LoadFeedTabs()
        {
            List<string> tabList = await Task.Run(()=> { return MyFilesDatabase.GetRssFeedLinksTabsTitle(GloableProfData.PData); });

            foreach (string tabTitle in tabList)
            {
                RSSTabs.Add(CreateNewTab(tabTitle));
            }

            //SelectedRSSTab = null;
            //if (RSSTabs.Count > 0)
            //{
            //    SelectedRSSTab = RSSTabs[0];
            //    SelectedRSSTab.RefreshRssFeeds();
            //}
        }



        private RSSTabViewModel CreateNewTab(string tabTitle)
        {
            var tab = new RSSTabViewModel();
            tab.Title = tabTitle;

            return tab;
        }



        public void OnSelectedSendToSeo(string title, string link)
        {
            var message = new RSSMainWorkspaceViewModelMessage() { MessageType = "OnSelectedSendToSeo" };
            message.Parameters.Add(title);
            message.Parameters.Add(link);

            OnRSSMainWorkspaceViewModelMessage?.Invoke(message);
        }

        public void OnSelectedSendToPbn(string link, string title, string imageLink, string date, string description)
        {
            var message = new RSSMainWorkspaceViewModelMessage() { MessageType = "OnSelectedSendToPbn" };
            message.Parameters.Add(link);
            message.Parameters.Add(title);
            message.Parameters.Add(imageLink);
            message.Parameters.Add(date);
            message.Parameters.Add(description);

            OnRSSMainWorkspaceViewModelMessage?.Invoke(message);
        }

        public void OnSelectedLaunchLinkMasher(string link)
        {
            var message = new RSSMainWorkspaceViewModelMessage() { MessageType = "OnSelectedLaunchLinkMasher" };
            message.Parameters.Add(link);

            OnRSSMainWorkspaceViewModelMessage?.Invoke(message);
        }

        public void OnSelectedLaunchLink(string link, bool inFiefox)
        {
            var message = new RSSMainWorkspaceViewModelMessage() { MessageType = "OnSelectedLaunchLink" };
            message.Parameters.Add(link);
            message.Parameters.Add(inFiefox);

            OnRSSMainWorkspaceViewModelMessage?.Invoke(message);
        }

        internal async void OnClickedOpenSocialShareLink(string type, string link, string imageLink)
        {
            string fullUrl = Social.GetShareUrl(type, link, imageLink);

            if (MessageBox.Show("Would you like to share this from an external project?",
                "External Project?", 
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ChooseProjectsVM cpvm = new ChooseProjectsVM();
                await cpvm.InitProjectsWindowList();
                if (cpvm.ShowListWindowDialog())
                {
                    int windowsLaunched = 0;
                    foreach (var sp in cpvm.SavedProjectsListAdded)
                    {
                        if (!sp.IsChecked || sp.IsFolder) continue;

                        var info = new ProcessStartInfo
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        if (type.StartsWith("ff_"))
                        {
                            info.Arguments = "\"" + sp.FilePath + "\"" + " " + "\"" + fullUrl + "\" " + windowsLaunched;
                            info.FileName = "BrowseoFX.CMD.exe";
                        }
                        else
                        {
                            info.Arguments = sp.FilePath.Replace(" ", MyFilesDatabase.SPLITTER) + " " + fullUrl.Replace(" ", MyFilesDatabase.SPLITTER) + " " + "SocialEngagerOptimizer";
                            info.FileName = "AnyProjectBrowserProcess.exe";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AnyProjectBrowserProcess.exe"),
                        }

                        Process p = Process.Start(info);
                        ProcessManager.Instance.AddProcess(p);

                        int randLaunchWait = new Random().Next(1, 3) * 1000;
                        await Task.Delay(randLaunchWait < 5000 ? randLaunchWait + 5000 : randLaunchWait);
                        windowsLaunched++;
                    }
                }
            }
            else
            {

                UsageTracker.AddTraceCookie(type + " " + UsageTracker.Usage_Type_ShareFromRss);

                var message = new RSSMainWorkspaceViewModelMessage() { MessageType = "OnClickedOpenSocialShareLink" };
                message.Parameters.Add(fullUrl);
                message.Parameters.Add(link);
                message.Parameters.Add(type.StartsWith("ff_"));

                OnRSSMainWorkspaceViewModelMessage?.Invoke(message);
            }
        }
    }
}
