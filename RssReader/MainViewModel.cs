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
using System.Xml;

namespace RssReader
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event Action<string, string> OnLaunchToBrowser = delegate { };//link, rsslink
        public event Action<string> OnLaunchToTabBrowser = delegate { };//url
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

        private PersonData mProfile;

        Thread loadingThread;

        public MainViewModel()
        {
            AllRssFeedsResults = new ObservableCollection<RssList>();

            DockPannelButtonsClick = new RelayCommand(OnDockPannelButtonsClick);
        }



        public void SetProfileData(PersonData profile)
        {
            mProfile = profile;
            RefreshRssFeed();
        }

        private void OnDockPannelButtonsClick(object param)
        {
            switch ((string)param)
            {
                case "OpenRssLinksWindow":
                    RssFeedsLinksMultiWindow rsslw = new RssFeedsLinksMultiWindow();
                    List<string> rssFeeds = MyFilesDatabase.GetRssFeedLinks(mProfile, TabTitle);
                    if (rssFeeds != null)
                    {
                        foreach (string link in rssFeeds)
                        {
                            rsslw.tbInputedText.Text += link + Environment.NewLine; 
                        }
                    }
                    rsslw.ShowDialog();
                    if (rsslw.OKClicked)
                    {
                        MyFilesDatabase.SaveRssFeedsSiteLinks(rsslw.tbInputedText.Text.Trim(), mProfile, TabTitle);
                        RefreshRssFeed();
                    }
                    break;

                case "Refresh":
                    RefreshRssFeed();
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

        public void RefreshRssFeed()
        {
            if (loadingThread != null && loadingThread.IsAlive)
            {
                loadingThread.Abort();
            }
            AllRssFeedsResults.Clear();
            List<string> rssFeeds = MyFilesDatabase.GetRssFeedLinks(mProfile, TabTitle);
            if (rssFeeds == null) return;
            foreach (string link in rssFeeds)
            {
                if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link)) continue;
                AllRssFeedsResults.Add(new RssList() { RssLink = link.Trim(), ListResults = new ObservableCollection<RssResult>() });
            }
            loadingThread = new Thread(() =>
            {
                List<string> failedLinks = new List<string>();

                foreach (RssList rssLink in AllRssFeedsResults)
                {
                    try
                    {
                        rssLink.PBarVis = true;
                        rssLink.ListResultVis = false;
                        string linkToFeed = rssLink.RssLink;
                        if (linkToFeed.Contains("feed://"))
                            linkToFeed = linkToFeed.Replace("feed://", "http://");

                        var req = (HttpWebRequest)WebRequest.Create(linkToFeed);
                        req.Method = "GET";
                        req.UserAgent = "Fiddler";
                        var rep = req.GetResponse();

                        using (XmlReader reader = XmlReader.Create(rep.GetResponseStream(), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
                        {
                            SyndicationFeed feed = SyndicationFeed.Load(reader);
                            foreach (SyndicationItem item in feed.Items)
                            {
                                string title = item.Title.Text;
                                string link = item.Links.Count > 0 ? item.Links[item.Links.Count - 1].Uri.AbsoluteUri : "";
                                string date = item.PublishDate == null ? "" : item.PublishDate.ToString();
                                string description = "";
                                string imgLink = "";
                                try
                                {
                                    description = item.Summary != null ? item.Summary.Text : (item.Content as TextSyndicationContent).Text;
                                    Match imgMatch = Regex.Match(description, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
                                    if(imgMatch.Groups.Count >= 2)
                                        imgLink = imgMatch.Groups[1].Value;
                                    description = Regex.Replace(description, "<.*?>", string.Empty);
                                }
                                catch { }

                                if (description.Length > 600)
                                {
                                    description = description.Substring(0, 599);
                                }

                                string fixedDescription = cleanString(description);
                                string fixedTitle = cleanString(title);

                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    RssResult result = new RssResult()
                                    {
                                        Title = fixedTitle,
                                        Link = link,
                                        Date = date,
                                        Description = fixedDescription,
                                        ImageLink = imgLink
                                    };
                                    result.OnClickedSendSocialLink += result_OnClickedSendSocialLink;
                                    rssLink.ListResults.Add(result);
                                });
                                if (rssLink.ListResults.Count > 20) break;
                            }
                        }
                        rssLink.PBarVis = false;
                        rssLink.ListResultVis = true;
                        rssLink.OnSelectedLaunchLink += rssLink_OnSelectedLaunchLink;
                    }
                    catch
                    {
                        rssLink.PBarVis = false;
                        rssLink.ListResultVis = true;
                        failedLinks.Add(rssLink.RssLink);
                    }
                }

                if (failedLinks.Count > 0)
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
            });
            loadingThread.Start();
        }

        private string cleanString(string stringToClean)
        {
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
                    fullUrl = Social.SHARELINK_facebook + link;
                    break;

                case Social.SOCIALTYPE_gp:
                    fullUrl = Social.SHARELINK_googleplus + link;
                    break;

                case Social.SOCIALTYPE_digg:
                    fullUrl = Social.SHARELINK_digg + link;
                    break;

                case Social.SOCIALTYPE_pin:
                    fullUrl = Social.SHARELINK_pintrest + link + "&media=" + imageLink;
                    break;

                case Social.SOCIALTYPE_reddit:
                    fullUrl = Social.SHARELINK_reddit + link;
                    break;

                case Social.SOCIALTYPE_stumble:
                    fullUrl = Social.SHARELINK_stumbleupon + link;
                    break;

                case Social.SOCIALTYPE_tumblr:
                    fullUrl = Social.SHARELINK_tumblr + link;
                    break;

                case Social.SOCIALTYPE_twit:
                    fullUrl = Social.SHARELINK_twitter + link;
                    break;

                case Social.SOCIALTYPE_wp:
                    AddLinkDataWindow alw = new AddLinkDataWindow();
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

            OnLaunchToBrowser(fullUrl, link);
        }

        void rssLink_OnSelectedLaunchLink(string link)
        {
            OnLaunchToTabBrowser(link);
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
                MyFilesDatabase.SaveRssFeedsSiteLinks(toSave.Trim(), mProfile, TabTitle);
                RefreshRssFeed();
            }
        }


    }
}
