using Organiser.Common;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using RssReader.Helpers;
using RssReader.Models;
using RssReader.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace RssReader
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static bool isCloseing
        {
            get;
            set;
        }
        public event Action<string, string,bool> OnLaunchToBrowser = delegate { };//link, rsslink,isff
        public event Action<string,bool> OnLaunchToTabBrowser = delegate { };//url,isff
        public event Action<string, string> OnSelectedSendToSeo = delegate { };//title,url
        public event Action<string> OnLaunchToTabMasher = delegate { };//url
        public event Action<string,string,string,string,string> OnSelectedSendToPbn = delegate { };//send to MAsher
      //  public event Action<string,List<string>> OnImportedTab = delegate { };//tab title, list of rss feeds

        public ICommand DockPannelButtonsClick { get; set; }

        public ObservableCollection<RssList> AllRssFeedsResults { get; set; }

        private string tabTitle;
        public string TabTitle
        {
            get { return tabTitle; }
            set
            {
                tabTitle = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TabTitle"));
            }
        }

        Thread loadingThread;
        bool wasRefresh;

        public MainViewModel()
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_Navigatedtorsstab);

            AllRssFeedsResults = new ObservableCollection<RssList>();

            DockPannelButtonsClick = new RelayCommand(OnDockPannelButtonsClick);
        }

        private void OnDockPannelButtonsClick(object param)
        {
            switch ((string)param)
            {
                case "OpenRssLinksWindow":
                    RssFeedsLinksMultiWindow rsslw = new RssFeedsLinksMultiWindow();
                    rsslw.Title = "One Feed Per Line";
                    List<string> rssFeeds = MyFilesDatabase.GetRssFeedLinks(GloableProfData.PData, TabTitle);
                    if (rssFeeds != null)
                    {
                        foreach (string link in rssFeeds)
                        {
                            rsslw.tbInputedText.Text += link + Environment.NewLine; 
                        }
                    }
                    rsslw.ShowDialog();
                    if (rsslw.ButtonLeftClicked)
                    {
                        MyFilesDatabase.SaveRssFeedsSiteLinks(rsslw.tbInputedText.Text.Trim().SplitAndRemoveEmpty(Environment.NewLine), GloableProfData.PData, TabTitle);
                        RefreshRssFeed(false);
                    }
                    break;

                case "Refresh":
                    wasRefresh = true;
                    RefreshRssFeed(false);
                    break;

            //    case "Import":
            //        SelectProfileWindow spw = new SelectProfileWindow();
            //        spw.Title = "Select Project";
            //        spw.ShowDialog();
            //        if (spw.OkClicked)
            //        {
            //            ObservableCollection<AvailableTabsAndLinks> availrsses = new ObservableCollection<AvailableTabsAndLinks>();
            //            foreach (string tabTitle in MyFilesDatabase.GetRssFeedLinksTabsTitlesByName(spw.SelectedProjectName))
            //            {
            //                availrsses.Add(new AvailableTabsAndLinks() { Name = tabTitle });
            //            }

                //            ChooseFolderWindow cfw = new ChooseFolderWindow();
                //            cfw.DataContext = this;
                //            cfw.lstItems.ItemsSource = availrsses;
                //            cfw.ShowDialog();
                //            if (cfw.OkClicked)
                //            {
                //                foreach (AvailableTabsAndLinks availTabs in availrsses)
                //                {
                //                    if (availTabs.IsChecked)
                //                    {
                //                        OnImportedTab(availTabs.Name, MyFilesDatabase.GetRssFeedLinks(spw.SelectedProjectName, availTabs.Name));
                //                    }
                //                }
                //            }

                //        }
                //break;

                default:
                    break;
            }
        }

        public void RefreshRssFeed(bool checkFoeExists)
        {
            if (checkFoeExists)
            {
                if (AllRssFeedsResults.Count > 0) return;
            }
            if (loadingThread != null && loadingThread.IsAlive && wasRefresh)
            {
                loadingThread.Abort();
                wasRefresh = false;
            }
            try
            {
                AllRssFeedsResults.Clear();
                List<string> rssFeeds = MyFilesDatabase.GetRssFeedLinks(GloableProfData.PData, TabTitle);
                if (rssFeeds == null) return;
                foreach (string link in rssFeeds)
                {
                    if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link)) continue;
                    AllRssFeedsResults.Add(new RssList() { RssLink = link.Trim(), ListResults = new List<RssResult>() });
                }

                loadingThread = new Thread(() =>
                {
                    try
                    {

                        List<RssResult> tempResultsList = new List<RssResult>();
                        List<string> failedLinks = new List<string>();

                        foreach (RssList rssLink in AllRssFeedsResults)
                        {
                            var links = rssLink.RssLink.SplitAndRemoveEmpty(",");
                            foreach (var link in links)
                            {
                                try
                                {
                                    rssLink.PBarVis = true;
                                    rssLink.ListResultVis = false;

                                    string linkToFeed = link;
                                    if (linkToFeed.Contains("feed://"))
                                        linkToFeed = linkToFeed.Replace("feed://", "http://");

                                    var req = (HttpWebRequest)WebRequest.Create(linkToFeed);
                                    req.Method = "GET";
                                    req.UserAgent = "Fiddler";
                                    req.Proxy = MyFilesDatabase.GetRequestsProxy();
                                    var rep = req.GetResponse();

                                    using (XmlReader reader = XmlReader.Create(rep.GetResponseStream(), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
                                    {
                                        SyndicationFeed feed = SyndicationFeed.Load(reader);
                                        foreach (SyndicationItem item in feed.Items)
                                        {
                                            string imgLink = "";

                                            string title = cleanString(item.Title.Text);
                                            if (title.Contains("<")) title = Regex.Replace(title, "<.*?>", string.Empty);
                                            
                                            string description = item.Summary != null ? item.Summary.Text : item.Content != null ? (item.Content as TextSyndicationContent).Text : "";
                                            if (!description.IsNullOrEmpty())
                                            {
                                                Match imgMatch = Regex.Match(description, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
                                                if (imgMatch.Groups.Count >= 2)
                                                    imgLink = imgMatch.Groups[1].Value;

                                                description = Regex.Replace(description, "<.*?>", string.Empty);
                                                description = Regex.Replace(description, @"[\n]{2,}", "\n");
                                                if (description.Length > 600) description = description.Substring(0, 599);

                                                description = cleanString(description);
                                            }

                                            string linkResult = item.Links.Count > 0 ? item.Links[0].Uri.AbsoluteUri : "";
                                            if (linkResult.Contains("https://www.google.com/url?rct=j&sa=t&url=") && linkResult.Contains("&ct="))
                                            {
                                                linkResult = linkResult.Replace("https://www.google.com/url?rct=j&sa=t&url=", "");
                                                if (linkResult.Contains("&ct=")) linkResult = linkResult.Remove(linkResult.IndexOf("&ct="));
                                            }

                                            string date = item.PublishDate == null ? "" : item.PublishDate.ToString();
                                            

                                            
                                           

                                            if (tempResultsList.Count < 20)
                                            {
                                                RssResult result = new RssResult()
                                                {
                                                    Title = title,
                                                    Link = linkResult,
                                                    Date = date,
                                                    Description = description,
                                                    ImageLink = imgLink
                                                };
                                                result.SocialStatsReplys.GetAllStatsFor(linkResult);
                                                
                                                result.OnClickedSendSocialLink += result_OnClickedSendSocialLink;
                                                tempResultsList.Add(result);
                                            }
                                        }
                                    }
                                    rssLink.PBarVis = false;
                                    rssLink.ListResultVis = true;
                                    rssLink.OnSelectedLaunchLink += rssLink_OnSelectedLaunchLink;
                                    rssLink.OnSelectedSendToSeo += RssLink_OnSelectedSendToSeo;
                                    rssLink.OnSelectedLaunchLinkMasher += rssLink_OnSelectedLaunchLinkMasher;
                                    rssLink.OnSelectedSendToPbn += RssLink_OnSelectedSendToPbn; ;
                                    rssLink.OnListItemChanged += RssLink_OnListItemChanged;
                                }
                                catch
                                {
                                    rssLink.PBarVis = false;
                                    rssLink.ListResultVis = true;
                                    failedLinks.Add(rssLink.RssLink);
                                }
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (Action)delegate
                                {

                                    foreach (RssResult r in tempResultsList)
                                    {
                                        rssLink.ListResults.Add(r);
                                    }
                                //rssLink.ListResults.AddRange(tempResultsList);
                                tempResultsList.Clear();
                                // after all.. update the UI with following
                                // rssLink.ListResults.RaiseListChangedEvents = true;
                                //rssLink.ListResults.ResetBindings(); // this forces update of entire list
                            });

                                // rssLink.RaisListPropChanged();
                            }
                        }

                        if (failedLinks.Count > 0 && !isCloseing)
                        {
                            string failed = "";
                            foreach (string failedLink in failedLinks)
                            {
                                failed += failedLink + " ";
                            }

                            if (failed.ToLower().Contains("rss20"))
                                failed += " please change rss20 to atom10";
                            MessageBox.Show("The Following links are incompatible " + failed,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message != null && ex.Message.ToLower().Contains("thread was being aborted")) return;
                        if (!isCloseing)
                            MessageBox.Show("An error occured while refreshing a rss feed please refresh the feed tab to reload it.");
                    }
                });
                loadingThread.Start();
            }
            catch
            {
                MessageBox.Show("An error occured while refreshing a rss feed please refresh the feed tab to reload it.");
            }
        }

        private void RssLink_OnListItemChanged(RssList changedList)
        {
            try
            {
                int indexOfChangedList = AllRssFeedsResults.IndexOf(changedList);
                AllRssFeedsResults.RemoveAt(indexOfChangedList);
                AllRssFeedsResults.Insert(indexOfChangedList, changedList);
            }
            catch
            {
                "Couldnt update list with filtered results please try again".Show();
            }
        }

        private void RssLink_OnSelectedSendToSeo(string title, string url)
        {
            OnSelectedSendToSeo(title, url);
        }

        private void RssLink_OnSelectedSendToPbn(string link, string title, string imagelink, string date, string description)
        {
            OnSelectedSendToPbn(link, title, imagelink, date, description);
        }

        void rssLink_OnSelectedLaunchLinkMasher(string link)
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_SentToMasher+" " + link);
            OnLaunchToTabMasher(link);
        }

        private string cleanString(string stringToClean)
        {
            if (stringToClean.IsNullOrEmpty()) return "";

            string returnstring = "";
            if (stringToClean.Contains('&'))
            {
                bool noAdd = false;
                foreach (char c in stringToClean)
                {
                    if (c == '&') { noAdd = true; continue; }
                    if (noAdd) { if (c == ';') noAdd = false; continue; }
                    returnstring += c;
                }
            }
            else
            {
                returnstring = stringToClean;
            }

            return returnstring;
        }

        void result_OnClickedSendSocialLink(string type, string link, string imageLink)
        {
            string fullUrl = "";

            switch (type)
            {
                case Social.SOCIALTYPE_fb:
                case "ff_"+Social.SOCIALTYPE_fb:
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

                case Social.SOCIALTYPE_digg:
                case "ff_" + Social.SOCIALTYPE_digg:
                    fullUrl = Social.SHARELINK_digg + link;
                    break;

                case Social.SOCIALTYPE_pin:
                case "ff_" + Social.SOCIALTYPE_pin:
                    if (imageLink == "")
                    {
                        MessageBox.Show("The feed needs to link to a image share to pinterest.");
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

            Organiser.Common.Classes.UsageTracker.AddTraceCookie(type + " "+UsageTracker.Usage_Type_ShareFromRss);
            OnLaunchToBrowser(fullUrl, link, type.StartsWith("ff_"));
        }

        void rssLink_OnSelectedLaunchLink(string link,bool isff)
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_SentToBrowserfromrss + " " + link);
            OnLaunchToTabBrowser(link, isff);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void setLinks(List<string> linksList)
        {
            string toSave = "";
            foreach (string link in linksList)
            {
                toSave += link + Environment.NewLine;
            }
            if (toSave != "")
            {
                MyFilesDatabase.SaveRssFeedsSiteLinks(linksList.ToArray(), GloableProfData.PData, TabTitle);
                RefreshRssFeed(false);
            }
        }
    }
}
