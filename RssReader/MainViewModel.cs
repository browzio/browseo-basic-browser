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
                            //using (WebClient client = new WebClient())
                           // {
                              //  client.Encoding = Encoding.UTF8;

                                string data = "";
                                string linkToFeed = rssLink.RssLink;
                                if (linkToFeed.Contains("feed://"))
                                    linkToFeed = linkToFeed.Replace("feed://", "http://");
                                //data = client.DownloadString(linkToFeed);

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
                                        string link = item.Links.Count > 0 ? item.Links[item.Links.Count -1].Uri.AbsoluteUri : "";
                                        string date = item.PublishDate == null ? "" : item.PublishDate.ToString();
                                        string description = "";
                                        try
                                        {
                                             description = item.Summary != null ?
                                                Regex.Replace(item.Summary.Text, "<.*?>", string.Empty) :
                                                Regex.Replace((item.Content as TextSyndicationContent).Text, "<.*?>", string.Empty);
                                        }
                                        catch { }
                                        if (description.Length > 600)
                                        {
                                            description = description.Substring(0, 599);
                                        }
                                        string fixedDescription = "";
                                        if (description.Contains('&'))
                                        {
                                            bool noAdd = false;
                                            foreach (char c in description)
                                            {
                                                if (c == '&') { noAdd = true; continue; }
                                                if (noAdd) { if (c == ';') noAdd = false; continue; }
                                                fixedDescription += c;
                                            }
                                        }
                                        else
                                        {
                                            fixedDescription = description;
                                        }

                                        string fixedTitle = "";
                                        if (title.Contains('&'))
                                        {
                                            bool noAddtitle = false;
                                            foreach (char c in title)
                                            {
                                                if (c == '&') { noAddtitle = true; continue; }
                                                if (noAddtitle) { if (c == ';') noAddtitle = false; continue; }
                                                fixedTitle += c;
                                            }
                                        }
                                        else
                                        {
                                            fixedTitle = title;
                                        }

                                        App.Current.Dispatcher.Invoke((Action)delegate
                                        {
                                            RssResult result = new RssResult()
                                            {
                                                Title = fixedTitle,
                                                Link = link,
                                                Date = date,
                                                Description = fixedDescription
                                            };
                                            result.OnClickedSendSocialLink += result_OnClickedSendSocialLink;
                                            rssLink.ListResults.Add(result);
                                        });
                                        if (rssLink.ListResults.Count > 20) break;
                                    }
                                }
                            //}
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

        void result_OnClickedSendSocialLink(string type, string link)
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
                    fullUrl = Social.SHARELINK_pintrest + link;
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
    }
}
