using Organiser.Common.Classes;
using Organiser.Common.ViewModels;
using Organiser.Common.Windows;
using RssReader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal void OnClickedOpenSocialShareLink(string type, string link, string imageLink)
        {
            string fullUrl = "";
            switch (type)
            {
                case Social.SOCIALTYPE_fb:
                case "ff_" + Social.SOCIALTYPE_fb:
                    fullUrl = Social.SHARELINK_facebook + link;
                    break;

                case Social.SOCIALTYPE_gp:
                case "ff_" + Social.SOCIALTYPE_gp:
                    fullUrl = Social.SHARELINK_googleplus + link;
                    break;

                case Social.SOCIALTYPE_buffer:
                case "ff_" + Social.SOCIALTYPE_buffer:
                    fullUrl = Social.SHARELINK_buffer + link;
                    break;

                case "Scoopit":
                case "ff_Scoopit":
                    //javascript:(function(){scscript=document.createElement('SCRIPT');scscript.type='text/javascript';scscript.src='https://www.scoop.it/resources/bklet/scoop.js?x='+(Math.random());document.getElementsByTagName('head')[0].appendChild(scscript);document.sc_srvurl='https://www.scoop.it'})();
                    //https://www.scoop.it/bookmarklet?url=
                    fullUrl = "https://www.scoop.it/bookmarklet?url=" + link;
                    break;

                case "hootsuite":
                case "ff_hootsuite":
                    //javascript:(function(){scscript=document.createElement('SCRIPT');scscript.type='text/javascript';scscript.src='https://www.scoop.it/resources/bklet/scoop.js?x='+(Math.random());document.getElementsByTagName('head')[0].appendChild(scscript);document.sc_srvurl='https://www.scoop.it'})();
                    //https://www.scoop.it/bookmarklet?url=
                    fullUrl = "https://hootsuite.com/hootlet/social-share?url=" + link;
                    break;

                case Social.SOCIALTYPE_digg:
                case "ff_" + Social.SOCIALTYPE_digg:
                    fullUrl = Social.SHARELINK_digg + link;
                    break;

                case Social.SOCIALTYPE_pin:
                case "ff_" + Social.SOCIALTYPE_pin:
                    if (imageLink == "")
                    {
                        Show("This page needs a image link share to pinterest.");
                        return;
                    }
                    fullUrl = Social.SHARELINK_pintrest + link + "&media=" + imageLink;
                    break;

                case Social.SOCIALTYPE_reddit:
                case "ff_" + Social.SOCIALTYPE_reddit:
                    fullUrl = Social.SHARELINK_reddit + link;
                    break;

                case Social.SOCIALTYPE_stumble:
                case "ff_" + Social.SOCIALTYPE_stumble:
                    fullUrl = Social.SHARELINK_stumbleupon + link;
                    break;

                case Social.SOCIALTYPE_tumblr:
                case "ff_" + Social.SOCIALTYPE_tumblr:
                    fullUrl = Social.SHARELINK_tumblr + link;
                    break;

                case Social.SOCIALTYPE_twit:
                case "ff_" + Social.SOCIALTYPE_twit:
                    fullUrl = Social.SHARELINK_twitter + link;
                    break;

                case Social.SOCIALTYPE_wp:
                case "ff_" + Social.SOCIALTYPE_wp:
                    SetNameAndDataWindow alw = new SetNameAndDataWindow();
                    alw.tblockInfo.Text = "Enter wordpress site (browzio.wordpress.com):";
                    alw.ShowDialog();
                    if (!alw.OkClicked) return;
                    string wpUrl = alw.tbInputText.Text;
                    if (!wpUrl.Contains("http"))
                        wpUrl = "https://" + wpUrl;
                    fullUrl = wpUrl + Social.SHARELINK_wordpress + link;
                    break;

                default:
                    fullUrl = link;
                    break;
            }

            UsageTracker.AddTraceCookie(type + " " + UsageTracker.Usage_Type_ShareFromRss);

            var message = new RSSMainWorkspaceViewModelMessage() { MessageType = "OnClickedOpenSocialShareLink" };
            message.Parameters.Add(fullUrl);
            message.Parameters.Add(link);
            message.Parameters.Add(type.StartsWith("ff_"));

            OnRSSMainWorkspaceViewModelMessage?.Invoke(message);
        }
    }
}
