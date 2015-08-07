using ProjectsList.Helpers;
using Prospector.Helpers;
using Prospector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Prospector.ViewModels
{
    public class FootPrintsOptionsVM : INotifyPropertyChanged
    {
        public event Action<string> OnClickedSearch = delegate { };

        public const int Comment_Backlinks = 0;
        public const int Forum = 1;
        public const int Guest_Posts = 2;
        public const int Blogs = 3;
        public const int Link_Roundups = 4;
        public const int Custom = 5;

        private ICommand startSearch;
        public ICommand StartSearch
        {
            get { return startSearch; }
            set { startSearch = value; }
        }

        private ICommand clear;
        public ICommand Clear
        {
            get { return clear; }
            set { clear = value; }
        }

        private ICommand sendToBrowser;
        public ICommand SendToBrowser
        {
            get { return sendToBrowser; }
            set { sendToBrowser = value; }
        }

        private ICommand export;
        public ICommand Export
        {
            get { return export; }
            set { export = value; }
        }


        #region collections  
        private ObservableCollection<Footprint> websitesForBlogs;
        public ObservableCollection<Footprint> WebsitesForBlogs
        {
            get { return websitesForBlogs; }
            set { websitesForBlogs = value; }
        }

        private ObservableCollection<Footprint> tLDs;
        public ObservableCollection<Footprint> TLDs
        {
            get { return tLDs; }
            set { tLDs = value; }
        }

        private ObservableCollection<Footprint> timeFrames;
        public ObservableCollection<Footprint> TimeFrames
        {
            get { return timeFrames; }
            set { timeFrames = value; }
        }

        private ObservableCollection<Footprint> comments;
        public ObservableCollection<Footprint> Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        private ObservableCollection<SearchResult> listResults;
        public ObservableCollection<SearchResult> ListResults
        {
            get { return listResults; }
            set { listResults = value; }
        }

        private List<SearchResult> l_Blogs = new List<SearchResult>();
        private List<SearchResult> l_Forum = new List<SearchResult>();
        private List<SearchResult> l_Guest_Posts = new List<SearchResult>();
        private List<SearchResult> l_Link_Roundups = new List<SearchResult>();
        private List<SearchResult> l_Resource_pages = new List<SearchResult>();
        private List<SearchResult> l_SponsorDonation_links = new List<SearchResult>();
        private List<SearchResult> l_Comment_Backlinks = new List<SearchResult>();
        private List<SearchResult> l_Custom = new List<SearchResult>();

        private ObservableCollection<int> maxPages;
        public ObservableCollection<int> MaxPages
        {
            get { return maxPages; }
            set { maxPages = value; }
        }
        #endregion 

        private int sIListResults;
        public int SIListResults
        {
            get { return sIListResults; }
            set
            {
                sIListResults = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SIListResults"));
                }
            }
        }

        private int cmbMaxPAgesIndex;
        public int CmbMaxPAgesIndex
        {
            get { return cmbMaxPAgesIndex; }
            set
            {
                cmbMaxPAgesIndex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CmbMaxPAgesIndex"));
                }
            }
        }

        private int cmbTimeframeIndex;
        public int CmbTimeframeIndex
        {
            get { return cmbTimeframeIndex; }
            set { 
                cmbTimeframeIndex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CmbTimeframeIndex"));
                }
            }
        }

        private int tCSelectedTabIndex;
        public int TCSelectedTabIndex
        {
            get { return tCSelectedTabIndex; }
            set {
                tCSelectedTabIndex = value;
                FootPrintString = Keyword;
                switch (tCSelectedTabIndex)
                {
                    case Blogs:
                        RBOrientation = Orientation.Vertical;
                        Visible_Custom = true;
                        Visible_CommentSettings = true;
                        createSitesListBlog();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Blogs)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    case Forum:
                        RBOrientation = Orientation.Vertical;
                        Visible_Custom = true;
                        Visible_CommentSettings = false;
                        createSitesListForum();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Forum)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    case Guest_Posts:
                        RBOrientation = Orientation.Horizontal;
                        Visible_Custom = true;
                        Visible_CommentSettings = false;
                        createSitesGuests();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Guest_Posts)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    case Link_Roundups:
                        RBOrientation = Orientation.Vertical;
                        Visible_Custom = true;
                        Visible_CommentSettings = false;
                        createSitesListRoundups();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Link_Roundups)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    //case Resource_pages:
                    //    Visible_CommentSettings = false;
                    //    createSitesListResources();
                    //    ListResults.Clear();
                    //    foreach (SearchResult result in l_Resource_pages)
                    //    {
                    //        ListResults.Add(result);
                    //    }
                    //    break;

                    //case SponsorDonation_links:
                    //    Visible_CommentSettings = false;
                    //    createSitesListSponsor();
                    //    ListResults.Clear();
                    //    foreach (SearchResult result in l_SponsorDonation_links)
                    //    {
                    //        ListResults.Add(result);
                    //    }
                    //    break;

                    case Comment_Backlinks:
                        RBOrientation = Orientation.Vertical;
                        Visible_Custom = true;
                         Visible_CommentSettings = false;
                        createSitesListComments();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Comment_Backlinks)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    case Custom:
                        FootPrintString = "";
                        RBOrientation = Orientation.Vertical;
                        Visible_Custom = false;
                        Visible_CommentSettings = false;
                        createSitesListComments();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Custom)
                        {
                            ListResults.Add(result);
                        }
                        break;
                }
                createTLDsList();
                setFootprintText();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TCSelectedTabIndex"));
                }
            }
        }

        private bool visible_CommentSettings;
        public bool Visible_CommentSettings
        {
            get { return visible_CommentSettings; }
            set
            {
                visible_CommentSettings = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_CommentSettings"));
                }
            }
        }

        private bool visible_Custom;
        public bool Visible_Custom
        {
            get { return visible_Custom; }
            set
            {
                visible_Custom = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_Custom"));
                }
            }
        }

        private string keyword;
        public string Keyword
        {
            get { return keyword; }
            set
            {
                keyword = value;
                setFootprintText();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Keyword"));
                }
            }
        }

        private string footPrint;
        public string FootPrintString
        {
            get { return footPrint; }
            set
            {
                footPrint = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FootPrintString"));
                }
            }
        }

        private bool checked_KeywordInUrl;
        public bool Checked_KeywordInUrl
        {
            get { return checked_KeywordInUrl; }
            set
            {
                checked_KeywordInUrl = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Checked_KeywordInUrl"));
                }
            }
        }

        private bool checked_KeywordExactMatch;
        public bool Checked_KeywordExactMatch
        {
            get { return checked_KeywordExactMatch; }
            set
            {
                checked_KeywordExactMatch = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Checked_KeywordExactMatch"));
                }
            }
        }

        public FootPrintsOptionsVM()
        {
            StartSearch = new RelayCommand(search);
            SendToBrowser = new RelayCommand(sendLinkToBrowser);
            Clear = new RelayCommand(clearResultsList);
            Export = new RelayCommand(exportLinks);

            WebsitesForBlogs = new ObservableCollection<Footprint>();
            Visible_Custom = true;
            Visible_CommentSettings = false;
            createSitesListComments();

            TLDs = new ObservableCollection<Footprint>();
            createTLDsList();

            TimeFrames = new ObservableCollection<Footprint>();
            createTimeframeOptions();

            Comments = new ObservableCollection<Footprint>();
            createCommentsSettings();
            CmbTimeframeIndex = 0;

            ListResults = new ObservableCollection<SearchResult>();

            MaxPages = new ObservableCollection<int>();
            MaxPages.Add(1);
            MaxPages.Add(2);
            MaxPages.Add(3);

            RBOrientation = Orientation.Vertical;
        }

        #region list options
        private void createCommentsSettings()
        {
            Comments.Add(new Footprint() { Option = "Comments Open", Query = "%22post a comment%22", Type = Footprint.TYPE_Comments });
            Comments.Add(new Footprint() { Option = "Comments Closed", Query = "%22comments closed%22", Type = Footprint.TYPE_Comments });
            Comments.Add(new Footprint() { Option = "Must be logged in", Query = "%22you must be logged in%22", Type = Footprint.TYPE_Comments });
            foreach (Footprint f in Comments)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }

        private void createTimeframeOptions()
        {
            TimeFrames.Add(new Footprint() { Option = "Any Time",Query="" });
            TimeFrames.Add(new Footprint() { Option = "Past Hour", Query = "&tbs=qdr:h" });
            TimeFrames.Add(new Footprint() { Option = "Past 24 Hours", Query = "&tbs=qdr:d" });
            TimeFrames.Add(new Footprint() { Option = "Past Week", Query = "&tbs=qdr:w" });
            TimeFrames.Add(new Footprint() { Option = "Past Month", Query = "&tbs=qdr:m" });
            TimeFrames.Add(new Footprint() { Option = "Past Year", Query = "&tbs=qdr:y" });
        }

        private void createTLDsList()
        {
            TLDs.Clear();
            TLDs.Add(new Footprint() { Option = "Any" , Checked = true});
            TLDs.Add(new Footprint() { Option = ".COM", Query ="site:com",Type = Footprint.TYPE_TLDs });
            TLDs.Add(new Footprint() { Option = ".EDU", Query = "site:edu", Type = Footprint.TYPE_TLDs });
            TLDs.Add(new Footprint() { Option = ".GOV", Query = "site:gov", Type = Footprint.TYPE_TLDs });
            TLDs.Add(new Footprint() { Option = ".NET", Query = "site:net", Type = Footprint.TYPE_TLDs });
            TLDs.Add(new Footprint() { Option = ".ORG", Query = "site:org", Type = Footprint.TYPE_TLDs });
            foreach (Footprint f in TLDs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }


        private Orientation rBOrientation;
        public Orientation RBOrientation
        {
            get { return rBOrientation; }
            set
            {
                rBOrientation = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RBOrientation"));
            }
        }

        private void createSitesListBlog()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "Any", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "BlogEngine.NET", Query = "%22Powered by BlogEngine.NET%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Expression Engine", Query = "%22powered by expressionengine%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Typepad", Query = "%22powered by Typepad%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Wordpress", Query = "%22powered by Wordpress%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }
        private void createSitesListForum()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "Any", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "ip.board", Query = "%22powered by ip.board%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Fireboard", Query = "%22powered by Fireboard%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "pupbb3", Query = "%22powered by phpbb%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "SMF", Query = "%22powered by SMF%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "vbuletin", Query = "%22powered by vbulletin%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }
        private void createSitesGuests()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "Accepting gues posts", Query = "%22accepting guest posts%22", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "Become a contributor", Query = "%22become a contributor%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Become a guest writer", Query = "%22become a guest writer%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Contribute to our site", Query = "%22contribute to our site%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Contributor guidelines", Query = "%22contributor guidelines%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Guest bloggers wanted", Query = "%22guest bloggers wanted%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "guest post courtesy of", Query = "%22guest post courtesy of%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "guest post guidelines", Query = "%22guest post guidelines%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "guest post opportunities", Query = "%22guest post opportunities%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "guest posts wanted", Query = "%22guest posts wanted%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "I’ve been featured on", Query = "%22I’ve been featured on%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "my guest blogs", Query = "%22my guest blogs%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "my posts on other blogs", Query = "%22my posts on other blogs%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "sites I’ve written for", Query = "%22sites I’ve written for%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "submit article", Query = "%22submit article%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "submit blog post", Query = "%22submit blog post%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "submit guest post", Query = "%22submit guest post%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "submit your content", Query = "%22submit your content%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "this is a guest post by", Query = "%22this is a guest post by%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "This post was written by", Query = "%22This post was written by%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "write for us", Query = "%22write for us%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "writers wanted", Query = "%22writers wanted%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }
        private void createSitesListRoundups()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "Best articles of the week", Query = "%22best articles of the week%22", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "Best posts of the week", Query = "%22best posts of the week%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Dailt link roundup", Query = "%22daily link roundup%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Friday link roundup", Query = "%22friday link roundup%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Link roundup", Query = "%22link roundup%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Monday link roundup", Query = "%22monday link roundup%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Top posts this week", Query = "%22top posts this week%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Weekly link roundup", Query = "%22weekly link roundup%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }
        private void createSitesListResources()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "inititle:resources", Query = "intitle:resources", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "inurl:resources", Query = "inurl:resources" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Links", Query = "links" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Recommended sites", Query = "%22recommended sites%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Resources", Query = "%22resources%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Resources pages", Query = "%22resource pages%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }
        private void createSitesListSponsor()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "Contributors page",Query="%22contributors page%22", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "Donate to us", Query = "%22donate to us%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Sponsors page", Query = "%22sponsors page%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }
        private void createSitesListComments()
        {
            WebsitesForBlogs.Clear();
            WebsitesForBlogs.Add(new Footprint() { Option = "Comment Luv Premium", Query = "%22This blog uses premium CommentLuv%22 -%22The version of CommentLuv on this site is no longer supported.%22", Checked = true });
            WebsitesForBlogs.Add(new Footprint() { Option = "Do-follow comments", Query = "%22Notify me of follow-up comments?%22+%22Submit the word you see below:%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Intense Debate", Query = "%22if you have a website, link to it here%22 %22post a new comment%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "KeywordLuv", Query = "%22Enter YourName@YourKeywords%22" });
            WebsitesForBlogs.Add(new Footprint() { Option = "Livefyre", Query = "%22get livefyre%22 %22comment help%22 -%22Comments have been disabled for this post%22" });
            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }

        void f_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
            {
                setFootprintText();
            }
        }

        #endregion

        private void setFootprintText()
        {
            FootPrintString = Keyword;

            if (Checked_KeywordInUrl)
                FootPrintString = "%22" + FootPrintString + "%22";

            foreach (Footprint siteChoice in WebsitesForBlogs)
            {
                if (siteChoice.Checked)
                {
                    FootPrintString += " " + siteChoice.Query;
                    break;
                }
            }

            if (tCSelectedTabIndex == Blogs)
            {
                int chekkedcount = 0;
                foreach (Footprint commentSetting in Comments)
                {
                    if (commentSetting.Checked)
                    {
                        if (chekkedcount == 0)
                            FootPrintString += " " + commentSetting.Query;
                        else
                            FootPrintString += " -" + commentSetting.Query;
                        chekkedcount++;
                    }
                }
            }

            foreach (Footprint domainTld in TLDs)
            {
                if (domainTld.Checked)
                {
                    FootPrintString += " " + domainTld.Query;
                    break;
                }
            }

            FootPrintString = FootPrintString.Replace("%22", "\"");
        }

        private void search(object param)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            string Query = FootPrintString;
            Query = Query.Replace("\"", "%22");
            Query = Query.Trim();
            Query = Query.Replace(' ', '+');
            Query = String.Format(@"http://google.com/search?v=1.0&q={0}", Query);
            Query = Query + TimeFrames[CmbTimeframeIndex].Query;

            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Prospector Search " + Query);

            GetKeywordRankings(Query, MaxPages[CmbMaxPAgesIndex], false);

            Mouse.OverrideCursor = null;
        }

        private void sendLinkToBrowser(object obj)
        {
            
            try
            {
                OnClickedSearch(ListResults[SIListResults].Link);
                Organiser.Common.Classes.UsageTracker.AddTraceCookie("Prospector Sent Link To browser " + ListResults[SIListResults].Link);
            }
            catch
            {
                try
                {
                    OnClickedSearch(ListResults[SIListResults - 1].Link);
                    Organiser.Common.Classes.UsageTracker.AddTraceCookie("Prospector Sent Link To browser " + ListResults[SIListResults - 1].Link);
                }
                catch
                {
                    MessageBox.Show("Couldnt open link.");
                }
            }
        }

        private void clearResultsList(object obj)
        {
            ListResults.Clear();
            switch (tCSelectedTabIndex)
            {
                case Blogs:
                    l_Blogs.Clear();
                    break;

                case Forum:
                    l_Forum.Clear();
                    break;

                case Guest_Posts:
                    l_Guest_Posts.Clear();
                    break;

                case Link_Roundups:
                    l_Link_Roundups.Clear();
                    break;

                //case Resource_pages:
                //    l_Resource_pages.Clear();
                //    break;

                //case SponsorDonation_links:
                //    l_SponsorDonation_links.Clear();
                //    break;

                case Comment_Backlinks:
                    l_Comment_Backlinks.Clear();
                    break;

                case Custom:
                    l_Custom.Clear();
                    break;
            }
        }

        private void exportLinks(object param)
        {
            try
            {
                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                string filename = "";
                if (result == false) return;

                // Save document 
                filename = dlg.FileName;

                if (filename == "") return;
                new Thread(() =>
                {
                    foreach (SearchResult res in ListResults)
                    {
                        File.AppendAllText(filename, res.Link + Environment.NewLine);
                    }

                    System.Diagnostics.Process.Start(filename);
                }).Start();
            }
            catch { }
        }
        
        public void GetKeywordRankings(string link, int maxSearchPages, bool? includeBing)
        {
            GoogleCrawler googleCrawler = new GoogleCrawler(link, maxSearchPages);
            googleCrawler.LinkWasAddedToList += new Action<SearchResult, bool>(Crawler_LinkWasAdded);
            googleCrawler.OnPageCountUpdate += new Action<int, int>(Crawler_OnPageAdded);
            googleCrawler.OnReturnResults += new Action<bool>(Crawler_OnesultsReturned);
            googleCrawler.FindResults(1);
        }

        private void Crawler_OnesultsReturned(bool wasProxy)
        {
            //there was an error
        }

        private void Crawler_OnPageAdded(int arg1, int arg2)
        {
            //progress

            //if (IsIndeterminate) IsIndeterminate = false;
            //PBarValu = ((double)pageNum / (taskcount)) * 100;
        }

        private void Crawler_LinkWasAdded(SearchResult result, bool found)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                ListResults.Add(result);
                switch (tCSelectedTabIndex)
                {
                    case Blogs:
                        l_Blogs.Add(result);
                        break;

                    case Forum:
                        l_Forum.Add(result);
                        break;

                    case Guest_Posts:
                        l_Guest_Posts.Add(result);
                        break;

                    case Link_Roundups:
                        l_Link_Roundups.Add(result);
                        break;

                    //case Resource_pages:
                    //    l_Resource_pages.Add(result);
                    //    break;

                    //case SponsorDonation_links:
                    //    l_SponsorDonation_links.Add(result);
                    //    break;

                    case Comment_Backlinks:
                        l_Comment_Backlinks.Add(result);
                        break;

                    case Custom:
                        l_Custom.Add(result);
                        break;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
