using Organiser.Common.Classes;
using Organiser.Common.ViewModels;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RssReader.ViewModels
{
    public class RSSTabViewModel : PropertyChangedViewModelBase
    {
        public ObservableCollection<RSSFeedViewModel> RSSFeeds { get; set; }
        private RSSFeedViewModel selectedRssFeed;
        public RSSFeedViewModel SelectedRSSFeed
        {
            get { return selectedRssFeed; }
            set { selectedRssFeed = value; NotifyOfPropertyChange(); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; NotifyOfPropertyChange(); }
        }

        private bool loadFeedsGrouped = false;

        //TODO: Define the cancellation token.
        //private CancellationTokenSource source;

        public RSSTabViewModel()
        {
            RSSFeeds = new ObservableCollection<RSSFeedViewModel>();
        }


        public override void OnReceivedCommandFromView(string param)
        {
            switch (param)
            {
                case "OpenRssLinksWindow":
                    RssFeedsLinksMultiWindow rsslw = new RssFeedsLinksMultiWindow();
                    rsslw.Title = "One Feed Per Line";
                    rsslw.buttonLeft.Content = "Load Separately";
                    rsslw.buttonRight.Content = "Load Grouped";
                    rsslw.buttonRight.Visibility = System.Windows.Visibility.Visible;

                    foreach (var feed in RSSFeeds)
                    {
                        if (feed.Urls != null && feed.Urls.Count > 1)
                            foreach (var url in feed.Urls)
                            {
                                rsslw.tbInputedText.Text += url + Environment.NewLine;
                            }
                        else
                            rsslw.tbInputedText.Text += feed.FeedUrl + Environment.NewLine;
                    }
                    rsslw.ShowDialog();
                    if (rsslw.DialogResult == true)
                    {
                        var feeds = rsslw.tbInputedText.Text.Trim().SplitAndRemoveEmpty(Environment.NewLine);
                        if (feeds == null) return;

                        loadFeedsGrouped = rsslw.ButtonRightClicked;

                        var feedsList = feeds.ToList();
                        feedsList.Add(loadFeedsGrouped.ToString());
                        Task.Run(() => { MyFilesDatabase.SaveRssFeedsSiteLinks(feedsList.ToArray(), GloableProfData.PData, Title); });
                        
                        CreateRSSFeedsCollection(feeds.ToList());
                        RefreshRssFeeds();
                    }
                    break;

                case "Refresh":
                     RefreshRssFeeds();
                    break;

                case "Close":
                    if (!Ask("Are you sure you want to delete this tab?")) return;

                    RSSMainWorkspaceViewModel.Instance.OnClickedCloseTab(this);
                    break;

                case "Combine":
                    //var msvm = new MultiSelectionViewModel();
                    //foreach (var feed in RSSFeeds) msvm.Add(feed.FeedUrl);

                    //if(msvm.ShowWindow("Mark the feeds to group"))
                    //{
                       
                    //    var urls = msvm.GetCheckedNameList();
                    //    if (urls.Count == 0) return;
                        
                    //    var feed = CreateNewFeed(msvm.GetCheckedNameString());

                    //    RSSFeeds.Insert(0, feed);

                    //    feed.Urls = urls;
                    //    feed.LoadFeedData();
                    //}
                    break;

                default:
                    break;
            }
        }

        internal void FirstLoadFeeds()
        {
            if (RSSFeeds.Count == 0) LoadFeedsFromFiles();
        }

        private void RefreshRssFeeds()
        {
            Task.Factory.StartNew(() =>
            {
                if (RSSFeeds.Count == 0) return;
                
                if (SelectedRSSFeed == null) SelectedRSSFeed = RSSFeeds[0];

                SelectedRSSFeed.LoadFeedData();
                
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async void LoadFeedsFromFiles()
        {
            var rssFeedsFromFile = await Task.Run(() => { return MyFilesDatabase.GetRssFeedLinks(GloableProfData.PData, Title); });
            if (rssFeedsFromFile.Count == 0) return;

            if (rssFeedsFromFile[0].ToLower().Trim() == "true" || rssFeedsFromFile[0].ToLower().Trim() == "false")
            {
                loadFeedsGrouped = Convert.ToBoolean(rssFeedsFromFile[0].Trim());
                rssFeedsFromFile.RemoveAt(0);
            }

            CreateRSSFeedsCollection(rssFeedsFromFile);
        }

        private void CreateRSSFeedsCollection(List<string> rssFeeds)
        {
            RSSFeeds.Clear();

            foreach (var feedUrl in rssFeeds)
            {
                if (!loadFeedsGrouped) RSSFeeds.Add(CreateNewFeed(feedUrl));
                else
                {
                    if (RSSFeeds.Count == 0) RSSFeeds.Add(CreateNewFeed(feedUrl));
                    else
                    {
                        RSSFeeds[0].FeedUrl += " " + feedUrl;
                        RSSFeeds[0].Urls.Add(feedUrl);
                    }
                }
            }

            if (RSSFeeds.Count > 0) SelectedRSSFeed = RSSFeeds[0];
        }

        private RSSFeedViewModel CreateNewFeed(string feedUrl)
        {
            var feed = new RSSFeedViewModel();
            feed.FeedUrl = feedUrl;
            feed.Urls.Add(feedUrl);
            return feed;
        }
    }
}
