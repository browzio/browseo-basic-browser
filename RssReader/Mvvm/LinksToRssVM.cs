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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Xml;
using Xilium.CefGlue.Client;

namespace RssReader.Mvvm
{
    public class LinksToRssVM : INotifyPropertyChanged
    {
        private const string POST_SPLITTER = "({[:]})";

        public ICommand BtnClicked { get; set; }
        public ICommand LbContextMenuClicked { get; set; }    

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
                    ResultsVisible = Visibility.Collapsed;
                    BrowserVisible = 0;
                }
                else if (value >= 0 && SavedFeeds.Count > 0)
                {
                    SICategories = SavedFeeds[value].FeedCategory;
                    InputedText = SavedFeeds[value].FeedLinks;
                    SiteName = SavedFeeds[value].FeedTitle;
                    OutputedLinks = SavedFeeds[value].FeedResult;
                    IsRssMashup = SavedFeeds[value].FeedIsRssMashup;
                    ResultsVisible = Visibility.Visible;

                    try
                    {
                        BrowserPreviewStatus = "Loading " + OutputedLinks.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[1];
                        WebBrowser.Navigate(OutputedLinks.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[1]);
                        BrowserVisible = 600;
                    }
                    catch { }
                }
                else if (value >= 0 && SavedFeeds.Count <= 0)
                {
                    MessageBox.Show("No saved posted feeds.");
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

        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (wfh == null)
                {
                    WebBrowser = new Xilium.CefGlue.Client.BrowserCntrl();
                    WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
                    WebBrowser.init("");
                    wfh = new WindowsFormsHost() { Child = WebBrowser };
                }
                return wfh;
            }
            set
            {
                wfh = value; if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("WebBrowserHost")); ;
            }
        }

        private void WebBrowser_OnBrowserLoadingChanged(bool isLoading)
        {
            if (!isLoading)
            {
                BrowserPreviewStatus = "Preview.";
            }
        }

        public BrowserCntrl WebBrowser { get; set; }

        private int browserVisible;
        public int BrowserVisible
        {
            get { return browserVisible; }
            set
            {
                browserVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BrowserVisible"));
            }
        }

        // BrowserPreviewStatus
        private string browserPreviewStatus;
        public string BrowserPreviewStatus
        {
            get { return browserPreviewStatus; }
            set
            {
                browserPreviewStatus = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BrowserPreviewStatus"));
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

        private string status;    
        public string Status
        {
            get { return status; }
            set { status = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
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

        public LinksToRssVM() 
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Opened Rss Masher");

            Catigories = new ObservableCollection<string>();
            Catigories.Add("AutosAndVehicles");
            Catigories.Add("Comedy");
            Catigories.Add("Education");
            Catigories.Add("FilmAndAnimation");
            Catigories.Add("Health");
            Catigories.Add("Business");
            Catigories.Add("Gaming");
            Catigories.Add("HowtoAndStyle");
            Catigories.Add("Music");
            Catigories.Add("NewsAndPolitics");
            Catigories.Add("NonprofitsAndActivism");
            Catigories.Add("PeopleAndBlogs");
            Catigories.Add("PetsAndAnimals");
            Catigories.Add("ScienceAndTechnology");
            Catigories.Add("Sports");
            Catigories.Add("TravelAndEvents");

            SavedFeeds = new ObservableCollection<SavedPostedFeed>();
            //getAllSavedFeeds();

            BtnClicked = new RelayCommand(btn_Clicked);
            LbContextMenuClicked = new RelayCommand(ON_LbContextMenuClicked);

            EnableOkBtn = true;
            SISavedFeeds = -1;
        }

        public void AddMasherLink(string link)
        {
            InputedText += link + Environment.NewLine;
        }

        private void btn_Clicked(object obj)
        {
            switch ((string)obj)
            {
                case "Refresh":
                    getAllSavedFeeds();
                    break;

                case "MashIT":
                    mashIt();
                    break;

                case "ClearData":
                    SISavedFeeds = -1;
                    break;
                default:
                    break;
            }
        }
         
        #region posting
        private void mashIt()
        {
            if (string.IsNullOrEmpty(InputedText) || string.IsNullOrWhiteSpace(InputedText) ||string.IsNullOrWhiteSpace(SiteName) || string.IsNullOrEmpty(SiteName))
                return;
            new System.Threading.Thread(() =>
            {
                if (SiteName.Contains(" "))
                    SiteName = SiteName.Replace(" ", "-");
                if (SiteExists()) return;
                bool failed = false;

                EnableOkBtn = false;
                OutputedLinks = "";
                string errors = "";
                ResultsVisible = Visibility.Collapsed;

                try
                {
                    Status = "Getting links ready.";

                    SyndicationFeed feed = new SyndicationFeed(SiteName.Replace("-", " "), "", new Uri("http://rssey.com"));
                    List<SyndicationItem> items = new List<SyndicationItem>();

                    string[] linksArray = InputedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string link in linksArray)
                    {
                        try
                        { 
                            if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link)) continue;

                            Status = "Adding link " + link;

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
                            if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link))
                                errors += "Incompatible link: " + link + Environment.NewLine;
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
                    Status = "Getting file ready.";
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

                        Status = "Uploading " + Catigories[SICategories] + "/" + SiteName + ".xml"; 

                        client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml", "STOR", mainFile);

                        string htmltext = client.DownloadString("http://rssey.com/rsstohtml/rss2html.php?XMLFILE=" + "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml");
                        string htmlFile = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempForRss", "feed.html");
                        File.WriteAllText(htmlFile, htmltext);

                        Status = "Uploading " + Catigories[SICategories] + "/" + SiteName + ".html";
                        client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".html", "STOR", htmlFile);

                        client.Dispose();
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    failed = true;
                    MessageBox.Show("Upload Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                string thexmlSite = "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml";
                string thehtmlSite = "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".html";

               
                OutputedLinks = thexmlSite + Environment.NewLine + thehtmlSite;
                SaveRssSiteLine(thexmlSite);

                if (!failed)
                {
                    ResultsVisible = Visibility.Visible;

                    SavedPostedFeed currentFeed = SavedFeeds.SingleOrDefault(t => t.FeedTitle == SiteName && SICategories == t.FeedCategory);
                    SavedPostedFeed feedToSave = new SavedPostedFeed()
                    {
                        FeedLinks = InputedText,
                        FeedTitle = SiteName,
                        FeedCategory = SICategories,
                        FeedResult = OutputedLinks,
                        FeedIsRssMashup = IsRssMashup,
                        ForeColorIsLocalProject = System.Windows.Media.Brushes.Black,
                        ProjectName = "(" + GloableProfData.PData.ProjectName + ")"
                    };

                    if(currentFeed!=null)
                    {
                        removeThisFeed(currentFeed);   
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            SavedFeeds.Remove(currentFeed);
                        });
                    }

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        SavedFeeds.Add(feedToSave);
                    });
                    saveTheFeed(feedToSave);
                }

                if(errors != "")
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        FlexibleMessageBox.Show(errors);
                    });
                }
               
                Organiser.Common.Classes.UsageTracker.AddTraceCookie("Rss Masher Results: " + OutputedLinks);

                EnableOkBtn = true;
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
                    string message1 = "There Is already a site that you own with this title would you like to overwrite it?";
                    if (isRemove)
                        message1 = "Are you sure you want to remove this site from our servers?";
                    if (MessageBox.Show(message1, "Overwrite?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        return isRemove;
                    }
                }
            }
            string message = "A site with this title already exists choose another one.";
            if (isRemove)
                message = "Cannot remove this site you did not create it";
            MessageBox.Show(message);
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
                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                EnableOkBtn = false;
                                try
                                {
                                    Status = "Deleting " + Catigories[SICategories] + "/" + SiteName + ".xml";
                                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml");
                                    request.Credentials = new NetworkCredential("bedbugsb", "[+=KJIp^T~nf");
                                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                                    response.Close();
                                }
                                catch { }

                                try
                                {
                                    Status = "Deleting " + Catigories[SICategories] + "/" + SiteName + ".html";
                                    FtpWebRequest siterequest = (FtpWebRequest)WebRequest.Create("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".html");
                                    siterequest.Credentials = new NetworkCredential("bedbugsb", "[+=KJIp^T~nf");
                                    siterequest.Method = WebRequestMethods.Ftp.DeleteFile;
                                    FtpWebResponse siteResponse = (FtpWebResponse)siterequest.GetResponse();
                                    siteResponse.Close();
                                }
                                catch { } 

                                Status = "Cleaning up.";
                                RemoveRssSiteLine("http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml");
                                SavedPostedFeed removedFeed = SavedFeeds[SISavedFeeds];
                                removeThisFeed(removedFeed);

                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    SavedFeeds.Remove(removedFeed);
                                });
                                SISavedFeeds = -1;
                                EnableOkBtn = true;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error Deleteing Site: " + ex.Message);
                            }
                        });
                    }
                    else
                    {
                        RemoveRssSiteLine("http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + ".xml");

                        SavedPostedFeed removedFeed = SavedFeeds[SISavedFeeds];
                        removeThisFeed(removedFeed);

                        SavedFeeds.Remove(removedFeed);
                        SISavedFeeds = -1;
                        EnableOkBtn = true;
                    }
                    break;

                default:
                    break;
            }
        }

        private void removeThisFeed(SavedPostedFeed removedFeed)
        {
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "SavedPostedRssByProject", removedFeed.ProjectName.Replace("(", "").Replace(")", ""));
            if (!Directory.Exists(dirPath)) return;

            string filePath = Path.Combine(dirPath, "Saved.txt");
            if (!File.Exists(filePath)) return;

            List<SavedPostedFeed> currentFeeds = new List<SavedPostedFeed>();

            foreach (string rssPost in File.ReadAllText(filePath).Split(new string[] { POST_SPLITTER }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(rssPost) || string.IsNullOrWhiteSpace(rssPost)) continue;

                string[] lines = rssPost.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                SavedPostedFeed feedFromFile = new SavedPostedFeed()
                {
                    FeedLinks = lines[0],
                    FeedTitle = lines[1],
                    FeedCategory = Convert.ToInt32(lines[2]),
                    FeedResult = lines[4],
                    FeedIsRssMashup = Convert.ToBoolean(lines[3]),
                    ProjectName = removedFeed.ProjectName,
                };
                if (feedFromFile.FeedTitle != removedFeed.FeedTitle &&
                    feedFromFile.FeedCategory != removedFeed.FeedCategory)
                currentFeeds.Add(feedFromFile);
            }

            File.Delete(filePath);
            foreach (SavedPostedFeed feed in currentFeeds)
            {
                saveTheFeed(feed);
            } 
        }

        private void saveTheFeed(SavedPostedFeed savedFeed)
        {
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "SavedPostedRssByProject", savedFeed.ProjectName.Replace("(","").Replace(")",""));
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            string filePath = Path.Combine(dirPath, "Saved.txt");

            File.AppendAllText(filePath, savedFeed.FeedLinks + MyFilesDatabase.SPLITTER +
                savedFeed.FeedTitle + MyFilesDatabase.SPLITTER +
                savedFeed.FeedCategory + MyFilesDatabase.SPLITTER +
                savedFeed.FeedIsRssMashup + MyFilesDatabase.SPLITTER +
                savedFeed.FeedResult + POST_SPLITTER);
        }

        public void CheckNRefreshList()
        {
            if (SavedFeeds.Count <= 0)
                getAllSavedFeeds();
        }

        private void getAllSavedFeeds()
        {
            string savedPostsDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "SavedPostedRssByProject");
            if (!Directory.Exists(savedPostsDir)) return;
            SavedFeeds.Clear();

            foreach (DirectoryInfo dir in new DirectoryInfo(savedPostsDir).GetDirectories())
            {
                string filePath = Path.Combine(dir.FullName, "Saved.txt");
                if (!File.Exists(filePath)) continue;

                addtoListFromSavedPath(filePath, dir.Name == GloableProfData.PData.ProjectName, dir.Name);
            }
        }

        private void addtoListFromSavedPath(string filePath, bool isProject, string projectName)
        {
            foreach (string rssPost in File.ReadAllText(filePath).Split(new string[] { POST_SPLITTER }, StringSplitOptions.RemoveEmptyEntries))
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
                    FeedIsRssMashup = Convert.ToBoolean(isMasher),
                    ForeColorIsLocalProject = isProject ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.Blue,
                    ProjectName = "("+projectName+")",
                });
            }
        }
        #endregion

        public void DisposeBrowser()
        {
            //WebBrowserHost.Child.Dispose();
            //WebBrowserHost.Child = null;
            //WebBrowserHost.Dispose();
            //WebBrowser.DisposeBrowserComponents();
            WebBrowser.Dispose();
            //WebBrowserHost = null;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
