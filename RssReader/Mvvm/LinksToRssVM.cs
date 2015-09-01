using HtmlAgilityPack;
using Organiser.Common.Classes;
using RssReader.Helpers;
using RssReader.Models;
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

namespace RssReader.Mvvm
{
    public class LinksToRssVM : INotifyPropertyChanged
    {
        private const string POST_SPLITTER = "({[:]})";

        public ICommand OkClicked { get; set; }
        public ICommand LbContextMenuClicked { get; set; }
        public ICommand ModeClicked { get; set; }

        private ObservableCollection<SavedPostedFeed> savedFeeds;
        public ObservableCollection<SavedPostedFeed> SavedFeeds
        {
            get { return savedFeeds; }
            set { savedFeeds = value; }
        }
        private int sISavedFeeds;
        public int SISavedFeeds
        {
            get { return sISavedFeeds; }
            set
            {
                sISavedFeeds = value;
                if (value == -1)
                {
                    SICategories = 0;
                    InputedText = "";
                    OutputedLinks = "";
                    SiteName = "";
                    IsRssMashup = false;
                    SavedListVisible = Visibility.Collapsed;
                    ResultsVisible = Visibility.Collapsed;

                }
                else if (value >= 0 && SavedFeeds.Count > 0)
                {
                    SICategories = SavedFeeds[value].FeedCategory;
                    InputedText = SavedFeeds[value].FeedLinks;
                    SiteName = SavedFeeds[value].FeedTitle;
                    OutputedLinks = SavedFeeds[value].FeedResult;
                    IsRssMashup = SavedFeeds[value].FeedIsRssMashup;
                    ResultsVisible = Visibility.Visible;
                    SavedListVisible = Visibility.Visible;
                }
                else if (value >= 0 && SavedFeeds.Count <= 0)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        MessageBox.Show(mParent, "No saved posted feeds.");
                    });
                }

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SISavedFeeds"));
            }
        }


        private ObservableCollection<string> catigories;
        public ObservableCollection<string> Catigories
        {
            get { return catigories; }
            set { catigories = value; }
        }
        private int sICategories;
        public int SICategories
        {
            get { return sICategories; }
            set
            {
                sICategories = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SICategories"));
            }
        }

        private string inputedText;
        public string InputedText
        {
            get { return inputedText; }
            set { inputedText = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("InputedText"));
            }
        }

        private string siteName;
        public string SiteName
        {
            get { return siteName; }
            set
            {
                siteName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SiteName"));
            }
        }

        private string outputText;
        public string OutputedLinks
        {
            get { return outputText; }
            set
            {
                outputText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OutputedLinks"));
            }
        }
        
        private bool enableOkBtn;
        public bool EnableOkBtn
        {
            get { return enableOkBtn; }
            set
            {
                enableOkBtn = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("EnableOkBtn"));
            }
        }

        private bool isRssMashup;
        public bool IsRssMashup
        {
            get { return isRssMashup; }
            set
            {
                isRssMashup = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsRssMashup"));
            }
        }

        private Visibility resultsVisible;
        public Visibility ResultsVisible
        {
            get { return resultsVisible; }
            set { resultsVisible = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ResultsVisible"));
            }
        }
        
        private Visibility savedListVisible;
        public Visibility SavedListVisible
        {
            get { return savedListVisible; }
            set
            {
                savedListVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SavedListVisible"));
            }
        }

        private PersonData mPData;
        private Window mParent;

        public LinksToRssVM(PersonData pData, Window parent) 
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Opened Rss Masher");

            mPData = pData;
            mParent = parent;

            Catigories = new ObservableCollection<string>();
            Catigories.Add("Autos-and-Vehicles");
            Catigories.Add("Comedy");
            Catigories.Add("Education");
            Catigories.Add("Film-&-Animation");
            Catigories.Add("Health");
            Catigories.Add("Business");
            Catigories.Add("Gaming");
            Catigories.Add("Howto-&-Style");
            Catigories.Add("Music");
            Catigories.Add("News-&-Politics");
            Catigories.Add("Nonprofits-&-Activism");
            Catigories.Add("People-&-Blogs");
            Catigories.Add("Pets-&-Animals");
            Catigories.Add("Science-&-Technology");
            Catigories.Add("Sports");
            Catigories.Add("Travel & Events");

            SavedFeeds = new ObservableCollection<SavedPostedFeed>();
            getAllSavedFeeds();

            OkClicked = new RelayCommand(btn_OkClicked);
            LbContextMenuClicked = new RelayCommand(ON_LbContextMenuClicked);
            ModeClicked = new RelayCommand(On_NewModeClicked);

            EnableOkBtn = true;
            SISavedFeeds = -1;
        }
        private void On_NewModeClicked(object param)
        {
            switch ((string)param)
            {
                case "New":
                    SISavedFeeds = -1;
                    break;

                case "Edit":
                    SISavedFeeds = 0;
                    break;

                default: 
                    break;
            }
        }


        public void AddMasherLink(string link)
        {
            InputedText += link + Environment.NewLine;
        }

        #region posting

        private void btn_OkClicked(object obj)
        {
            if (string.IsNullOrEmpty(InputedText) || string.IsNullOrWhiteSpace(InputedText) ||
                string.IsNullOrWhiteSpace(SiteName) || string.IsNullOrEmpty(SiteName)) return;
            new System.Threading.Thread(() =>
            {
                if (SiteName.Contains(" "))
                    SiteName = SiteName.Replace(" ", "-");
                if (SiteExists()) return;
                bool failed = false;

                EnableOkBtn = false;
                OutputedLinks = "";
                ResultsVisible = Visibility.Collapsed;

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                try
                {
                    SyndicationFeed feed = new SyndicationFeed(SiteName.Replace("-", " "), "", new Uri("http://rssey.com"));
                    List<SyndicationItem> items = new List<SyndicationItem>();

                    string[] linksArray = InputedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string link in linksArray)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link)) continue;
                            string linkToAdd = link;
                            if (linkToAdd.Contains("https"))
                                linkToAdd = linkToAdd.Replace("https", "http");

                            if (IsRssMashup)
                            {
                                if (linkToAdd.Contains("feed://"))
                                    linkToAdd = linkToAdd.Replace("feed://", "http://");
                                var req = (HttpWebRequest)WebRequest.Create(linkToAdd);
                                req.Method = "GET";
                                req.UserAgent = "Fiddler";
                                var rep = req.GetResponse();

                                using (XmlReader reader = XmlReader.Create(rep.GetResponseStream(), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }))
                                {
                                    SyndicationFeed locFeed = SyndicationFeed.Load(reader);
                                    int counter = 0;
                                    foreach (SyndicationItem item in locFeed.Items)
                                    {
                                        if (counter++ < 10)
                                        {
                                            items.Add(item);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                items.Add(getSysndicationItemFromCrawl(linkToAdd));
                            }
                        }
                        catch
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link))
                            MessageBox.Show(mParent, "Incompatible link " + link);
                    });
                        }
                    }

                    feed.Items = items;
                    Rss20FeedFormatter rss = new Rss20FeedFormatter(feed);

                    string rssDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempForRss");
                    if (!Directory.Exists(rssDir))
                    {
                        Directory.CreateDirectory(rssDir);
                    }

                    #region -- Output feed to a file --
                    string mainFile = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempForRss", "feed.xml");
                    using (FileStream fs = new FileStream(mainFile, FileMode.Create))
                    {
                        using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                        {
                            XmlWriterSettings xs = new XmlWriterSettings();
                            xs.Indent = true;
                            using (XmlWriter xw = XmlWriter.Create(w, xs))
                            {
                                xw.WriteStartDocument();
                                //Atom10FeedFormatter formatter = new Atom10FeedFormatter(feed);
                                //Rss20FeedFormatter formatter = new Rss20FeedFormatter(feed);
                                rss.WriteTo(xw);
                                xw.Close();
                            }
                        }
                    }
                    #endregion

                    #region -- upload file to server
                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential("bedbugsb", "[+=KJIp^T~nf");

                        client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml", "STOR", mainFile);

                        string htmltext = client.DownloadString("http://rssey.com/rsstohtml/rss2html.php?XMLFILE=" + "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName +".xml");
                        string htmlFile = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempForRss", "feed.html");
                        File.WriteAllText(htmlFile, htmltext);
                        client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".html", "STOR", htmlFile);

                        client.Dispose();
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    failed = true;
                    Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBox.Show(mParent, "Upload Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                }
                string thexmlSite = "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml";
                string thehtmlSite = "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".html";

                EnableOkBtn = true;
                OutputedLinks = thexmlSite + Environment.NewLine + thehtmlSite;
                SaveRssSiteLine(thexmlSite);

                if (!failed)
                {
                    ResultsVisible = Visibility.Visible;
                    bool found = false;
                    foreach (SavedPostedFeed savedFeed in SavedFeeds)
                    {
                        if (savedFeed.FeedTitle == SiteName && SICategories == savedFeed.FeedCategory)
                        {
                            savedFeed.FeedLinks = InputedText;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            SavedFeeds.Add(new SavedPostedFeed()
                            {
                                FeedLinks = InputedText,
                                FeedTitle = SiteName,
                                FeedCategory = SICategories,
                                FeedResult = OutputedLinks,
                                FeedIsRssMashup = IsRssMashup
                            });
                        });
                    }

                    saveAFeed();
                }

                Organiser.Common.Classes.UsageTracker.AddTraceCookie("Rss Masher Results: " + OutputedLinks);

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Mouse.OverrideCursor = null;
                });
            }).Start();
        }

        private SyndicationItem getSysndicationItemFromCrawl(string link)
        {
            string title = "", description = "";

            WebClient client = new WebClient();
            string htmContent = client.DownloadString(link);
            client.Dispose();
            title = Regex.Match(htmContent, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

            // HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmContent);

            try
            {
                description = doc.DocumentNode
                                        .SelectSingleNode("//meta[@property='og:description']")
                                        .Attributes["content"].Value;
            }
            catch { description = ""; }
            if (description == "")
            {
                try
                {
                    description = doc.DocumentNode
                                           .SelectSingleNode("//meta[@name='twitter:description']")
                                           .Attributes["content"].Value;
                }
                catch { description = ""; }
            }
            if (description == "")
            {
                try
                {
                    description = doc.DocumentNode.SelectSingleNode("//meta[@name='description']").Attributes["content"].Value;
                }
                catch { }
            }
            SyndicationItem item = new SyndicationItem(title, description, new Uri(link), "", DateTime.Now);
            return item;
        }

        private bool SiteExists(bool isRemove = false)
        {
            string xmlsite = "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadString(xmlsite);
                }
            }
            catch
            {
                return false;
            }

            foreach (string site in GetUsersSavedRssSites())
            {
                if (site == xmlsite)
                {
                    bool okClicked = false;
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        string message = "There Is already a site that you own with this title would you like to overwrite it?";
                        if (isRemove)
                            message = "Are you sure you want to remove this site from our servers?";
                        if (MessageBox.Show(mParent, message, "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            okClicked = true;
                    });
                    if (okClicked)
                    {
                        return isRemove;
                    }
                }
            }
            Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        string message = "A site with this title already exists choose another one.";
                        if (isRemove)
                            message = "Cannot remove this site you did not create it";
                        MessageBox.Show(mParent, message);
                    });
            if (isRemove)
                return false;
            return true;
        }

        public List<string> GetUsersSavedRssSites()
        {
            List<string> sites = new List<string>();

            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "PostedRssSites");
            if (!Directory.Exists(dirPath)) return sites;
            string filePath = Path.Combine(dirPath, "Saved.txt");
            if (!File.Exists(filePath)) return sites;

            foreach (string site in File.ReadAllLines(filePath))
            {
                sites.Add(MyFilesDatabase.DecodeFrom64(site));
            }

            return sites;
        }

        public void SaveRssSiteLine(string xmlSite)
        {
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "PostedRssSites");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string filePath = Path.Combine(dirPath, "Saved.txt");
            File.AppendAllText(filePath, MyFilesDatabase.EncodeTo64(xmlSite) + Environment.NewLine);
        }

        public void RemoveRssSiteLine(string xmlSite)
        {
            List<string> sites = GetUsersSavedRssSites();
            if (sites.Count > 0)
            {
                string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "PostedRssSites");
                if (!Directory.Exists(dirPath)) return;
                string filePath = Path.Combine(dirPath, "Saved.txt");
                if (!File.Exists(filePath)) return;

                File.Delete(filePath);
                foreach (string site in sites)
                {
                    if(site != xmlSite)
                        File.AppendAllText(filePath, MyFilesDatabase.EncodeTo64(site) + Environment.NewLine);
                }
            }
        }
        #endregion

        #region feed maintenence
        private void ON_LbContextMenuClicked(object param)
        {
            string type = (string)param;
            switch (type)
            {
                case "Remove":
                    if (SiteExists(true))
                    {
                        try
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            EnableOkBtn = false;

                            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml");
                            request.Credentials = new NetworkCredential("bedbugsb", "[+=KJIp^T~nf");
                            request.Method = WebRequestMethods.Ftp.DeleteFile;
                            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                            response.Close();

                            FtpWebRequest siterequest = (FtpWebRequest)WebRequest.Create("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".html");
                            siterequest.Credentials = new NetworkCredential("bedbugsb", "[+=KJIp^T~nf");
                            siterequest.Method = WebRequestMethods.Ftp.DeleteFile;
                            FtpWebResponse siteResponse = (FtpWebResponse)siterequest.GetResponse();
                            siteResponse.Close();

                            RemoveRssSiteLine("http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml");
                            SavedFeeds.RemoveAt(SISavedFeeds);
                            saveAFeed();
                            SISavedFeeds = 0;
                            
                            EnableOkBtn = true;
                            Mouse.OverrideCursor = null;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(mParent, "Error Deleteing Site: " + ex.Message);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void getAllSavedFeeds()
        {
            string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "SavedPostedRssByProject", mPData.ProjectName, "Saved.txt");
            if (!File.Exists(filePath)) return;
            foreach (string rssPost in File.ReadAllText(filePath).Split(new string[] { POST_SPLITTER }, StringSplitOptions.None))
            {
                if (string.IsNullOrEmpty(rssPost) || string.IsNullOrWhiteSpace(rssPost)) continue;

                string[] lines = rssPost.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                string links = lines[0];
                string title = lines[1];
                string category = lines[2];
                string isMasher = lines[3];
                string result = lines[4];
                SavedFeeds.Add(new SavedPostedFeed() 
                {
                    FeedLinks = links,
                    FeedTitle = title,
                    FeedCategory = Convert.ToInt32(category),
                    FeedResult = result,
                    FeedIsRssMashup = Convert.ToBoolean(isMasher)
                });
            }
        }

        private void saveAFeed()
        {
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "SavedPostedRssByProject", mPData.ProjectName);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "SavedPostedRssByProject", mPData.ProjectName, "Saved.txt");
            if (File.Exists(filePath)) File.Delete(filePath);

            foreach (SavedPostedFeed savedFeed in SavedFeeds)
            {
                File.AppendAllText(filePath, savedFeed.FeedLinks + MyFilesDatabase.SPLITTER +
                    savedFeed.FeedTitle + MyFilesDatabase.SPLITTER +
                    savedFeed.FeedCategory + MyFilesDatabase.SPLITTER +
                    savedFeed.FeedIsRssMashup + MyFilesDatabase.SPLITTER +
                    savedFeed.FeedResult + POST_SPLITTER);
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
