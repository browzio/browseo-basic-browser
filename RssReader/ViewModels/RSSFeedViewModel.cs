using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
using Organiser.Common.Windows;
using RssReader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Threading;

namespace RssReader.ViewModels
{
    public class RSSFeedViewModel : PropertyChangedViewModelBase
    {
        public ObservableCollection<RSSFeedData> RSSFeedDataResults { get; set; }
        private RSSFeedData selectedFeed;
        public RSSFeedData SelectedFeedDataResult
        {
            get { return selectedFeed; }
            set { selectedFeed = value; NotifyOfPropertyChange(); }
        }

        private string feedUrl;
        public string FeedUrl
        {
            get { return feedUrl; }
            set
            {
                feedUrl = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange("FeedUrls");
            }
        }
        
        public string FeedUrls
        {
            get { return FeedUrl.Replace(" ", Environment.NewLine); }
        }

        private bool isLoadingFeedData;
        public bool IsLoadingFeedData
        {
            get { return isLoadingFeedData; }
            set { isLoadingFeedData = value; NotifyOfPropertyChange(); }
        }

        public List<string> Urls { get; set; }

        public bool HasYoutubeFeed
        {
            get
            {
                return RSSFeedDataResults.Any(d => d.SocialStatsReplys.YoutubeReply != null);
            }
        }

        private bool isLoadingSocialStats, isLoadingBitmapImages;

        public RSSFeedViewModel()
        {
            RSSFeedDataResults = new ObservableCollection<Models.RSSFeedData>();
            Urls = new List<string>();
        }

        public override async void OnReceivedCommandFromView(string param)
        {
            switch (param)
            {
                case "ORDERBY_FBSHARES":
                case "ORDERBY_FBLIKES":
                case "ORDERBY_FBCOMMENTS":
                case "ORDERBY_GPLUSONES":
                case "ORDERBY_PINTERESTPINS":
                case "ORDERBY_STUMBLEVIEWS":
                case "ORDERBY_LINKEDINCOUNT":
                case "ORDERBY_BUFFERSHARES":
                case "ORDERBY_REDDITUPS":
                case "ORDERBY_REDDITSCORE":
                case "ORDERBY_YOUTUBEVIEWS":
                case "ORDERBY_YOUTUBERATINGS":
                case "ORDERBY_YOUTUBERATINGAVERAGE":
                    if (IsLoadingFeedData) return;

                    var resulstsList = SocialStatsFunctions.OrderStatsBy(RSSFeedDataResults.ToList(), param);
                    if (resulstsList == null || resulstsList.Count() == 0) return;

                    RSSFeedDataResults.Clear();
                    foreach (var r in resulstsList)
                    {
                        RSSFeedDataResults.Add(r as RSSFeedData);
                        await Task.Delay(5);
                    }
                    //OnListItemChanged(this);
                    break;

                case "GrabAllLinks":
                    var textWindow = new RssFeedsLinksMultiWindow();
                    textWindow.Title = FeedUrl;
                    foreach (var result in RSSFeedDataResults)
                    {
                        textWindow.tbInputedText.Text += result.Link + Environment.NewLine;
                    }
                    textWindow.Show();
                    break;

                case "Curaste":
                    if (SelectedFeedDataResult == null) return;
                    string htmlstring = "<blockquote>";
                    if (!string.IsNullOrEmpty(SelectedFeedDataResult.Title) && !string.IsNullOrWhiteSpace(SelectedFeedDataResult.Title))
                        htmlstring += "<h1>" + SelectedFeedDataResult.Title + "</h1>";
                    if (!string.IsNullOrEmpty(SelectedFeedDataResult.Date) && !string.IsNullOrWhiteSpace(SelectedFeedDataResult.Date))
                        htmlstring += "<p>" + SelectedFeedDataResult.Date + "</p>";
                    if (SelectedFeedDataResult.ImageLink != "https:" && SelectedFeedDataResult.ImageLink != "http:" &&
                        !string.IsNullOrEmpty(SelectedFeedDataResult.ImageLink) && !string.IsNullOrWhiteSpace(SelectedFeedDataResult.ImageLink))
                        htmlstring += "<img src=\"" + SelectedFeedDataResult.ImageLink + "\" />";
                    if (!string.IsNullOrEmpty(SelectedFeedDataResult.Description) && !string.IsNullOrWhiteSpace(SelectedFeedDataResult.Description))
                        htmlstring += "<p>" + SelectedFeedDataResult.Description + "</p>";
                    if (!string.IsNullOrEmpty(SelectedFeedDataResult.Link) && !string.IsNullOrWhiteSpace(SelectedFeedDataResult.Link))
                        htmlstring += "<a href=\"" + SelectedFeedDataResult.Link + " \" > " + SelectedFeedDataResult.Link + " </a>";
                    htmlstring += "</blockquote>";

                    MyFilesDatabase.SetClipboardText(htmlstring);
                    break;

                case "Browser":
                case "BrowserFF":
                    if (SelectedFeedDataResult == null) return;
                    RSSMainWorkspaceViewModel.Instance.OnSelectedLaunchLink(SelectedFeedDataResult.Link, param == "BrowserFF");
                    break;

                case "Masher":
                    if (SelectedFeedDataResult == null) return;
                    RSSMainWorkspaceViewModel.Instance.OnSelectedLaunchLinkMasher(SelectedFeedDataResult.Link);
                    break;

                case "PBNPOSTER":
                    if (SelectedFeedDataResult == null) return;
                    RSSMainWorkspaceViewModel.Instance.OnSelectedSendToPbn(SelectedFeedDataResult.Link, SelectedFeedDataResult.Title, SelectedFeedDataResult.ImageLink, SelectedFeedDataResult.Date, SelectedFeedDataResult.Description);
                    break;

                case "TOSEO":
                    if (SelectedFeedDataResult == null) return;
                    RSSMainWorkspaceViewModel.Instance.OnSelectedSendToSeo(SelectedFeedDataResult.Title, SelectedFeedDataResult.Link);
                    break;

                default:
                    break;
            }
        }

        internal void LoadFeedData()
        {
            if (IsLoadingFeedData) return;

            try
            {
                IsLoadingFeedData = true;

                Invoke(() => { RSSFeedDataResults.Clear(); });

                foreach (var url in Urls)
                {
                    try
                    {
                        string linkToFeed = url;
                        if (linkToFeed.Contains("feed://"))
                            linkToFeed = linkToFeed.Replace("feed://", "http://");

                        var req = (HttpWebRequest)WebRequest.Create(linkToFeed);
                        req.Method = "GET";
                        req.KeepAlive = false;
                        req.UserAgent = BrowserSettimgs.UserAgentFF;
                        req.Proxy = MyFilesDatabase.GetRequestsProxy();
                        
                            using (var rep = req.GetResponse())
                            {
                               // if (rep.StatusCode != HttpStatusCode.OK) return;
                                using (XmlReader reader = XmlReader.Create(rep.GetResponseStream(), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
                                {
                                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                                    if (null == feed) return;

                                    var length = feed.Items.Count();
                                    if (length > 20) length = 20;
                                    for (int i = 0; i < length; i++)
                                    {
                                        var item = feed.Items.ElementAt(i);
                                        RSSFeedData resultData = new RSSFeedData();

                                        string title = cleanString(item.Title.Text);
                                        if (title.Contains("<")) title = Regex.Replace(title, "<.*?>", string.Empty);

                                        string date = item.PublishDate == null ? "" : item.PublishDate.ToString();

                                        string linkResult = "", urlForStats = "";
                                        linkResult = urlForStats = item.Links.Count > 0 ? item.Links[0].Uri.AbsoluteUri : "";
                                        if (linkResult.Contains("https://www.google.com/url?rct=j&sa=t&url=") && linkResult.Contains("&ct="))
                                        {
                                            linkResult = linkResult.Replace("https://www.google.com/url?rct=j&sa=t&url=", "");
                                            linkResult = linkResult.Remove(linkResult.IndexOf("&ct="));
                                            urlForStats = linkResult;
                                        }
                                        if (!item.Id.IsNullOrEmpty())
                                        {
                                            if (item.Id.StartsWith("http://www.openvine.com/"))
                                            {
                                                urlForStats = linkResult = item.Id;
                                            }
                                            else if (item.Id.StartsWith("yt:video:"))
                                            {
                                                var videoId = item.Id.Replace("yt:video:", "");
                                                urlForStats = "https://youtu.be/" + videoId;
                                                resultData.SocialStatsReplys.YoutubeReply = new YoutubeReply();
                                            }
                                        }
                                        if (item.ElementExtensions != null)
                                        {
                                            foreach (var extension in item.ElementExtensions)
                                            {
                                                if (!extension.OuterNamespace.IsNullOrEmpty() && extension.OuterNamespace == "http://rssnamespace.org/feedburner/ext/1.0")
                                                {
                                                    var extReader = extension.GetReader();
                                                    if (extReader != null)
                                                    {
                                                        if (extReader.Name == "feedburner:origLink")
                                                        {
                                                            if (extReader.Read() && !extReader.Value.IsNullOrEmpty())
                                                            {
                                                                linkResult = urlForStats = extReader.Value;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        string description = "", imgLink = "";
                                        if (item.Summary != null)
                                        {
                                            description = item.Summary.Text;
                                        }
                                        else if (item.Content != null)
                                        {
                                            description = (item.Content as TextSyndicationContent).Text;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                foreach (var ext in item.ElementExtensions)
                                                {
                                                    if (ext.OuterName.ToLower() == "group")
                                                    {
                                                        var innerXml = ext.GetReader().ReadInnerXml();
                                                        System.Xml.Linq.XNode xelement = System.Xml.Linq.XElement.ReadFrom(ext.GetReader());

                                                        var elements = ((System.Xml.Linq.XContainer)xelement).FirstNode.NodesAfterSelf();
                                                        foreach (var e in elements)
                                                        {
                                                            var elem = e as System.Xml.Linq.XElement;
                                                            if (elem == null) continue;

                                                            if (elem.Name.LocalName == "thumbnail")
                                                            {
                                                                imgLink = elem.FirstAttribute.Value;
                                                            }
                                                            else if (elem.Name.LocalName == "description")
                                                            {
                                                                description = elem.Value;
                                                            }
                                                            else if (elem.Name.LocalName == "community")
                                                            {
                                                                var nextNodes = elem.FirstNode.NodesAfterSelf();
                                                                foreach (var n in nextNodes)
                                                                {
                                                                    var nn = n as System.Xml.Linq.XElement;
                                                                    if (nn == null) continue;

                                                                    if (nn.Name.LocalName == "starRating")
                                                                    {
                                                                        //description += Environment.NewLine + Environment.NewLine + "Rating: ";
                                                                        foreach (var a in nn.Attributes())
                                                                        {
                                                                            if (a.Name.LocalName == "count") resultData.SocialStatsReplys.YoutubeReply.starRating_count = a.Value;
                                                                            else if (a.Name.LocalName == "average") resultData.SocialStatsReplys.YoutubeReply.starRating_average = a.Value;
                                                                        }
                                                                    }
                                                                    else if (nn.Name.LocalName == "statistics")
                                                                    {
                                                                        //description += Environment.NewLine + Environment.NewLine + "Statistics: ";
                                                                        foreach (var a in nn.Attributes())
                                                                        {
                                                                            if (a.Name.LocalName == "views") resultData.SocialStatsReplys.YoutubeReply.statistics_views = a.Value;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch { }
                                        }
                                        if (!description.IsNullOrEmpty())
                                        {
                                            Match imgMatch = Regex.Match(description, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
                                            if (imgMatch.Groups.Count >= 2 && imgLink.IsNullOrEmpty()) imgLink = imgMatch.Groups[1].Value;

                                            description = Regex.Replace(description, "<.*?>", string.Empty);
                                            description = Regex.Replace(description, @"[\n]{2,}", "\n");
                                            if (description.Length > 600) description = description.Substring(0, 599);

                                            description = cleanString(description);
                                        }


                                        resultData.Title = title;
                                        resultData.Link = linkResult;
                                        resultData.Date = date;
                                        resultData.Description = description;
                                        resultData.ImageLink = imgLink;
                                        resultData.SocialStatsReplys.StatsUrl = urlForStats;
                                    

                                        Invoke(() =>
                                        {
                                            RSSFeedDataResults.Add(resultData);
                                        });

                                        Task.Delay(5);
                                    }
                                }
                            }
                        
                    }
                    catch
                    { }
                }

            }
            catch (Exception ex)
            {
                Task.Delay(5);
            }

            LoadFeedDataStats();
            LoadBitmapImgs();
        }

        private void LoadBitmapImgs()
        {
            isLoadingBitmapImages = true;

            foreach (var item in RSSFeedDataResults)
            {
                if (item.ImageLink.IsNullOrEmpty()) continue;

                Invoke(() => { item.BitmapImage = null; });

                int BytesToRead = 100;
                byte[] bytebuffer = new byte[BytesToRead];
                HttpWebResponse response = null;
                Stream responseStream = null;
                MemoryStream memoryStream = null;
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(item.ImageLink);
                    request.Timeout = -1;
                    request.KeepAlive = false;
                    request.UserAgent = BrowserSettimgs.UserAgentFF;
                    request.Proxy = MyFilesDatabase.GetRequestsProxy();

                    //using (token.Register(() => request.Abort(), true))
                    //{
                    response = (HttpWebResponse) request.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK) continue;

                    responseStream = response.GetResponseStream();
                    memoryStream = new MemoryStream();
                    int bytesRead = responseStream.Read(bytebuffer, 0, BytesToRead);
                    while (bytesRead > 0)
                    {
                        memoryStream.Write(bytebuffer, 0, bytesRead);
                        bytesRead = responseStream.Read(bytebuffer, 0, BytesToRead);
                    }
                    //}
                    
                    Invoke(() =>
                    {
                        try
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            image.StreamSource = memoryStream;
                            image.EndInit();

                            item.BitmapImage = image;
                            item.BitmapImage.DownloadCompleted += (s, e) =>
                            {
                                response.Dispose();
                                responseStream.Dispose();
                                memoryStream.Dispose();
                            };
                        }
                        catch{ item.ImageLink = ""; }
                    });

                }
                catch (Exception ex)
                {
                    item.BitmapImage = null;

                    if (response != null) response.Dispose();
                    if (responseStream != null) responseStream.Dispose();
                    if (memoryStream != null) memoryStream.Dispose();
                }
            }
            isLoadingBitmapImages = false;

            if (!isLoadingSocialStats) IsLoadingFeedData = false;
        }

        internal async void LoadFeedDataStats()
        {
            isLoadingSocialStats = true;
            foreach (var item in RSSFeedDataResults)
            {
                 await item.SocialStatsReplys.AsyncAllAwaitGetAllStatsFor();
            }
            isLoadingSocialStats = false;

            NotifyOfPropertyChange("HasYoutubeFeed");

           if(!isLoadingBitmapImages) IsLoadingFeedData = false;
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
    }
}
