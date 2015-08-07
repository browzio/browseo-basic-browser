using HtmlAgilityPack;
using Organiser.Common.Classes;
using RssReader.Helpers;
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
        public ICommand OkClicked { get; set; }

        private ObservableCollection<string> catigories;
        public ObservableCollection<string> Catigories
        {
            get { return catigories; }
            set { catigories = value; }
        }
        public int SICategories { get; set; }

        public string InputedText { get; set; }

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

            OkClicked = new RelayCommand(btn_OkClicked);

            EnableOkBtn = true;
            ResultsVisible = Visibility.Collapsed;
        }

        private void btn_OkClicked(object obj)
        {
            if (string.IsNullOrEmpty(InputedText) || string.IsNullOrWhiteSpace(InputedText) || 
                string.IsNullOrWhiteSpace(SiteName) || string.IsNullOrEmpty(SiteName)) return;
            new Thread(() =>
            {
                if (SiteName.Contains(" "))
                    SiteName = SiteName.Replace(" ", "-");
                if (SiteExists()) return;
                bool failed = false;

                EnableOkBtn = false;
                OutputedLinks = "";
                ResultsVisible = Visibility.Collapsed;

                App.Current.Dispatcher.Invoke((Action)delegate 
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });

                try
                {
                    SyndicationFeed feed = new SyndicationFeed(SiteName.Replace("-"," "), "", new Uri("http://rssey.com"));
                    List<SyndicationItem> items = new List<SyndicationItem>();

                    string[] linksArray = InputedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string link in linksArray)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(link)|| string.IsNullOrWhiteSpace(link)) continue;
                            string linkToAdd = link;
                            if (linkToAdd.Contains("https"))
                                linkToAdd = linkToAdd.Replace("https", "http");
                            items.Add(getSysndicationItemFromCrawl(linkToAdd));
                        }
                        catch
                        {
                            if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link))
                            MessageBox.Show("Incompatible link " + link);
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

                        client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + Catigories[SICategories] + "/" + SiteName, "STOR", mainFile);

                        string htmltext = client.DownloadString("http://rssey.com/rsstohtml/rss2html.php?XMLFILE=" + "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName);
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
                    MessageBox.Show("Upload Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if(!failed)
                    ResultsVisible = Visibility.Visible;
                EnableOkBtn = true;
                OutputedLinks = "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName + Environment.NewLine +
                    "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName+".html";
                    //"http://rssey.com/rsstohtml/rss2html.php?XMLFILE=" + "http://rssey.com/" + Catigories[SICategories] + "/" + SiteName;
                App.Current.Dispatcher.Invoke((Action)delegate
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

        private bool SiteExists()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadString("http://rssey.com/" + Catigories[SICategories] + "/" + SiteName);
                }
            }
            catch
            {
                return false;
            }
            MessageBox.Show("A site with this title already exists choose another one.");
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
