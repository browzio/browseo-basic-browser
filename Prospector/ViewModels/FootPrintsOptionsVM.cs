using Organiser.Common.Classes;
using Organiser.Common.Classes.Helpers;
using Organiser.Common.Classes.SocialHelpers;
using Organiser.Common.Windows;
using Prospector.Helpers;
using Prospector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using webhose;
using webhoseio;

namespace Prospector.ViewModels
{
    public class FootPrintsOptionsVM : INotifyPropertyChanged
    {
        public event Action<string,bool> OnClickedSearch = delegate { };//url,isff
        public event Action<string, string, string, string, string> OnSelectedSendToPbn = delegate { };//send to MAsher(link,title,imglink,date,description)

        public const int Comment_Backlinks = 0;
        public const int Forum = 1;
        public const int Guest_Posts = 2;
        public const int Blogs = 3;
        public const int Link_Roundups = 4;
        public const int Custom = 5;
        public const int Webhose = 6;
        public const int DarkWeb = 7;
        public const int Saved = 8;

        #region commands
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

        public ICommand RanckCheck { get; set; }
        //SetMoz
        public ICommand SetMoz { get; set; }
        //RefreshSaved
        public ICommand RefreshSaved { get; set; }
        //SetProxy
        public ICommand SetProxy { get; set; }

        private ICommand export;
        public ICommand Export
        {
            get { return export; }
            set { export = value; }
        }

        //SaveFootprint
        private ICommand saveFootprint;
        public ICommand SaveFootprint
        {
            get { return saveFootprint; }
            set { saveFootprint = value; }
        }

        //DeleteSavedFootprint
        private ICommand deleteSavedFootprint;
        public ICommand DeleteSavedFootprint
        {
            get { return deleteSavedFootprint; }
            set { deleteSavedFootprint = value; }
        }

        public ICommand OnCommandFromKODw { get; set; }
        #endregion


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

        private ObservableCollection<SavedFootprint> saved;
        public ObservableCollection<SavedFootprint> SavedFP
        {
            get { return saved; }
            set { saved = value; }
        }

        private ObservableCollection<SearchResult> listResults;
        public ObservableCollection<SearchResult> ListResults
        {
            get { return listResults; }
            set { listResults = value; }
        }

        //DWGrdOptions
        private ObservableCollection<DWQuerySelectableOptions> dWGrdOptions;
        public ObservableCollection<DWQuerySelectableOptions> DWGrdOptions
        {
            get { return dWGrdOptions; }
            set { dWGrdOptions = value; }
        }

        private List<SearchResult> l_Blogs = new List<SearchResult>();
        private List<SearchResult> l_Forum = new List<SearchResult>();
        private List<SearchResult> l_Guest_Posts = new List<SearchResult>();
        private List<SearchResult> l_Link_Roundups = new List<SearchResult>();
        private List<SearchResult> l_Resource_pages = new List<SearchResult>();
        //private List<SearchResult> l_SponsorDonation_links = new List<SearchResult>();
        private List<SearchResult> l_Comment_Backlinks = new List<SearchResult>();
        private List<SearchResult> l_Custom = new List<SearchResult>();
        private List<SearchResult> l_Saved = new List<SearchResult>();
        private List<SearchResult> l_Webhose = new List<SearchResult>();
        private List<SearchResult> l_DW = new List<SearchResult>();

        private ObservableCollection<int> maxPages;
        public ObservableCollection<int> MaxPages
        {
            get { return maxPages; }
            set { maxPages = value; }
        }


        #region webhose
        private ObservableCollection<Footprint> langs;
        public ObservableCollection<Footprint> Langs
        {
            get { return langs; }
            set { langs = value; }
        }

        private ObservableCollection<Footprint> siteTypesWebHose;
        public ObservableCollection<Footprint> SiteTypesWebHose
        {
            get { return siteTypesWebHose; }
            set { siteTypesWebHose = value; }
        }

        //PerformanceScores
        private ObservableCollection<string> performanceScores;
        public ObservableCollection<string> PerformanceScores
        {
            get { return performanceScores; }
            set { performanceScores = value; }
        }

        #endregion

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

        //CmbPerformanceScoresIndex
        private int cmbPerformanceScoresIndex;
        public int CmbPerformanceScoresIndex
        {
            get { return cmbPerformanceScoresIndex; }
            set
            {
                cmbPerformanceScoresIndex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CmbPerformanceScoresIndex"));
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
                Visible_SavedFP = false;
                Visible_savebtn = true;
                Visible_WebHose = false;
                NotVisible_WebHose = true;
                Visible_DW = false;
                DwExtras = Visibility.Visible;
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
                        Visible_DW = true;
                        Visible_CommentSettings = false;
                        ListResults.Clear();
                        foreach (SearchResult result in l_Custom)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    case Saved:
                        if (SavedFP.Count == 0)
                        {
                            createSavedOptions();
                        }
                        WebsitesForBlogs.Clear();
                        Visible_SavedFP = true;
                        FootPrintString = "";
                        RBOrientation = Orientation.Vertical;
                        Visible_Custom = false;
                        Visible_CommentSettings = false;
                        Visible_savebtn = false;
                        SISavedFP = 0;
                        ListResults.Clear();
                        foreach (SearchResult result in l_Saved)
                        {
                            ListResults.Add(result);
                        }
                        return;
                        
                    case Webhose:
                        FootPrintString = "";
                        RBOrientation = Orientation.Horizontal;
                        Visible_Custom = false;
                        Visible_CommentSettings = false;
                        Visible_WebHose = true;
                        NotVisible_WebHose = false;
                        Visible_savebtn = false;
                        createSitesListWebhose();
                        ListResults.Clear();
                        foreach (SearchResult result in l_Webhose)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    case DarkWeb:
                        DwExtras = Visibility.Collapsed;
                        CheckForTBPath();
                        RaiseTextChangedDW();
                        ListResults.Clear();
                        foreach (SearchResult result in l_DW)
                        {
                            ListResults.Add(result);
                        }
                        break;

                    default:
                        break;
                }
                createTLDsList();
                setFootprintText("");
                //if (TCSelectedTabIndex != Saved)
                //setFootprintText("");
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TCSelectedTabIndex"));
                }
            }
        }

        private void CheckForTBPath()
        {
            if (!TorPath.IsNullOrEmpty()) return;

            Task.Run(() =>
            {
                var saveFolderTBPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Paths");
                if (!Directory.Exists(saveFolderTBPath)) return;

                var saveFileTBPath = Path.Combine(saveFolderTBPath, "TB");
                if (!File.Exists(saveFileTBPath)) return;
                TorPath = File.ReadAllText(saveFileTBPath);
            });
        }


        #region visible
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

        //Visible_WebHose
        private bool visible_WebHose;
        public bool Visible_WebHose
        {
            get { return visible_WebHose; }
            set
            {
                visible_WebHose = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_WebHose"));
                }
            }
        }

        //NotVisible_WebHose
        private bool notVisible_WebHose;
        public bool NotVisible_WebHose
        {
            get { return notVisible_WebHose; }
            set
            {
                notVisible_WebHose = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("NotVisible_WebHose"));
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

        private bool Visible_dw;
        public bool Visible_DW
        {
            get { return Visible_dw; }
            set
            {
                Visible_dw = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_DW"));
                }
            }
        }

        //Visible_SavedFP
        private bool visible_SavedFP;
        public bool Visible_SavedFP
        {
            get { return visible_SavedFP; }
            set
            {
                visible_SavedFP = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_SavedFP"));
                }
            }
        }
        //Visible_savebtn
        private bool visible_savebtn;
        public bool Visible_savebtn
        {
            get { return visible_savebtn; }
            set
            {
                visible_savebtn = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_savebtn"));
                }
            }
        }
        #endregion

        //SISavedFP
        private int sISavedFP;
        public int SISavedFP
        {
            get { return sISavedFP; }
            set
            {
                sISavedFP = value;
                try
                {
                    if (value == 0 && SavedFP.Count > 0)
                    {
                        FootPrintString = SavedFP[0].Footprint;
                    }
                    else if (value >= 1 && SavedFP.Count > 0 && value-1 >0)
                    {
                        FootPrintString = SavedFP[value-1].Footprint;
                    }
                }
                catch { }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SISavedFP"));
                }
            }
        }

        private string keyword;
        public string Keyword
        {
            get { return keyword; }
            set
            {
                var old = keyword;
                keyword = value;
                RaiseTextChangedDW();
                setFootprintText(old);
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

        //QueryStringDW
        private string queryStringDW;
        public string QueryStringDW
        {
            get { return queryStringDW; }
            set
            {
                queryStringDW = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("QueryStringDW"));
                }
            }
        }

        //SpecificSites
        private string specificSites;
        public string SpecificSites
        {
            get { return specificSites; }
            set
            {
                specificSites = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpecificSites"));
                }
            }
        }

        //SpecificKws
        private string specificKws;
        public string SpecificKws
        {
            get { return specificKws; }
            set
            {
                specificKws = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpecificKws"));
                }
            }
        }

        //CountryCode
        private string countryCode;
        public string CountryCode
        {
            get { return countryCode; }
            set
            {
                countryCode = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CountryCode"));
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

        private bool checked_PostsWithVids;
        public bool Checked_PostsWithVids
        {
            get { return checked_PostsWithVids; }
            set
            {
                checked_PostsWithVids = value;
                if (value)
                {
                    Checked_PostsWithVidsAll = false;
                    Checked_PostsWithNoVids = false;
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Checked_PostsWithVids"));
                }
            }
        }

        private bool checked_PostsWithNoVids;
        public bool Checked_PostsWithNoVids
        {
            get { return checked_PostsWithNoVids; }
            set
            {
                checked_PostsWithNoVids = value;
                if (value)
                {
                    Checked_PostsWithVidsAll = false;
                    Checked_PostsWithVids = false;
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Checked_PostsWithNoVids"));
                }
            }
        }
        private bool checked_PostsWithVidsAll;
        public bool Checked_PostsWithVidsAll
        {
            get { return checked_PostsWithVidsAll; }
            set
            {
                checked_PostsWithVidsAll = value;
                if (value)
                {
                    Checked_PostsWithNoVids = false;
                    Checked_PostsWithVids = false;
                }

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Checked_PostsWithVidsAll"));
                }
            }
        }
        //ResponseSize
        private int responseSize;
        public int ResponseSize
        {
            get { return responseSize; }
            set
            {
                responseSize = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ResponseSize"));
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

        //KWinTitleIsChecked
        private bool kWinTitleIsChecked;
        public bool KWinTitleIsChecked
        {
            get { return kWinTitleIsChecked; }
            set
            {
                kWinTitleIsChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("KWinTitleIsChecked"));
                }
            }
        }

        //UseDarkWebIsChecked
        private bool useDarkWebIsChecked;
        public bool UseDarkWebIsChecked
        {
            get { return useDarkWebIsChecked; }
            set
            {
                useDarkWebIsChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UseDarkWebIsChecked"));
                }
            }
        }

        //KWinContentIsChecked
        private bool kWinContentIsChecked;
        public bool KWinContentIsChecked
        {
            get { return kWinContentIsChecked; }
            set
            {
                kWinContentIsChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("KWinContentIsChecked"));
                }
            }
        }
        //

        //UseProxy
        private bool useProxy;
        public bool UseProxy
        {
            get { return useProxy; }
            set
            {
                useProxy = value;
                if (value)
                {
                    if (string.IsNullOrEmpty(WebPageRequests.pIP) || string.IsNullOrWhiteSpace(WebPageRequests.pIP))
                    {
                        MessageBox.Show("Set your proxy details.");
                        UseProxy = false;
                    }
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("UseProxy"));
                }
            }
        }

        //IsNotSerching
        private bool isNotSerching;
        public bool IsNotSerching
        {
            get { return isNotSerching; }
            set
            {
                isNotSerching = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsNotSerching"));
                }
            }
        }

        //IsDWLive
        private bool isDWLive;
        public bool IsDWLive
        {
            get { return isDWLive; }
            set
            {
                isDWLive = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsDWLive"));
                }
                RaiseTextChangedDW();
            }
        }

        private Visibility dwExtras = Visibility.Visible;
        public Visibility DwExtras
        {
            get { return dwExtras; }
            set
            {
                dwExtras = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DwExtras"));
                }
            }
        }

        //TorPath
        private string torPath;
        public string TorPath
        {
            get { return torPath; }
            set
            {
                torPath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TorPath"));
                }
            }
        }

        //TorSavePath

        private List<string> inProxyFileTextArr;
        string inFileText;
        int proxyIndex;
        

        public FootPrintsOptionsVM()
        {
            StartSearch = new RelayCommand(search);
            SendToBrowser = new RelayCommand(sendLinkToBrowser);
            Clear = new RelayCommand(clearResultsList);
            Export = new RelayCommand(exportLinks);
            SaveFootprint = new RelayCommand(OnSaveFootprintClicked);
            DeleteSavedFootprint = new RelayCommand(DeleteSavedFootprintClicked);
            RanckCheck = new RelayCommand(OnRankCheckCkicked);
            SetMoz = new RelayCommand(OnSetMozClicked);
            RefreshSaved = new RelayCommand(OnRefreshSavewdFootprints);
            SetProxy = new RelayCommand(OnSetProxy);
            OnCommandFromKODw = new RelayCommand(OnCommandFromKODw_Raised);


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

            SavedFP = new ObservableCollection<SavedFootprint>();
            //createSavedOptions();

            ListResults = new ObservableCollection<SearchResult>();

            MaxPages = new ObservableCollection<int>();
            MaxPages.Add(10);
            MaxPages.Add(20);
            MaxPages.Add(30);

            Langs = new ObservableCollection<Footprint>();
            SiteTypesWebHose = new ObservableCollection<Footprint>();
            PerformanceScores = new ObservableCollection<string>();
            addAllLangsForWebhose();
            CmbPerformanceScoresIndex = 0;

            DWGrdOptions = new ObservableCollection<DWQuerySelectableOptions>();
            CreateDWOptionsList();

            RBOrientation = Orientation.Vertical;

            IsNotSerching = true;
            Visible_savebtn = true;
            NotVisible_WebHose = true;
            Checked_PostsWithVidsAll = true;
            ResponseSize = 25;

            MyFilesDatabase.SetMozIds();
            setProxyDetailes();

            //createSavedOptions();
        }

        private async void OnCommandFromKODw_Raised(object obj)
        {
            switch ((obj as string))
            {
                case "TorPath":
                    using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                    {
                        // openFileDialog.InitialDirectory = @"C:\";
                        openFileDialog.RestoreDirectory = true;

                        System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            TorPath = openFileDialog.FileName;
                        }
                    }

                    await Task.Run(()=> 
                    {
                        var saveFolderTBPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Paths");
                        if (!Directory.Exists(saveFolderTBPath)) Directory.CreateDirectory(saveFolderTBPath);

                        var saveFileTBPath = Path.Combine(saveFolderTBPath, "TB");
                        File.WriteAllText(saveFileTBPath, TorPath);
                    });
                    break;
                    

                default:
                    break;
            }
        }

        private void CreateDWOptionsList()
        {
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "Site", Tooltip = "Limit the results to a specific onion site or sites." });
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "Title", Tooltip = "A textual Boolean query describing the keywords that should (or shouldn’t) appear in the thread title." });
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "External Links", Tooltip = "Search for pages that included links to another site. (note that you must escape the http:// part of the URL like so: http\\:\\/\\/)" });
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "With Thread Title", Tooltip = "A textual Boolean query describing the keywords that should appear in the thread title." });
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "Without Thread Title", Tooltip = "A textual Boolean query describing the keywords that shouldn’t appear in the thread title." });
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "Thread Section Title", Tooltip = "A textual Boolean query describing the keywords that should (or shouldn’t) appear in the site’s section where the post was published" });
            DWGrdOptions.Add(new DWQuerySelectableOptions() { Type = "Thread URL", Tooltip = "Get all the posts of a specific thread (note that you must escape the http:// part of the URL like so: http\\:\\/\\/)." });
            //
        }

        private void OnSetProxy(object param)
        {
            SetProxyWindow spw = new SetProxyWindow();
            spw.tbInputedText.Text = inFileText;
            spw.ShowDialog();
            if (spw.OKClicked)
            {
                setProxyDetailes();
            }
        }

        private void setProxyDetailes()
        {
            new Thread(() => {
                try
                {
                    //pIP, pPort, pUser, pPass
                    proxyIndex = 0;

                    string mDir = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Prospector", "Proxy");
                    if (!System.IO.Directory.Exists(mDir)) return;

                    string filePath = System.IO.Path.Combine(mDir, "proxy.txt");
                    if (!System.IO.File.Exists(filePath)) return;

                    inFileText = File.ReadAllText(filePath);
                    string[] pDetailes = null;

                    if (inFileText.Contains(MyFilesDatabase.SPLITTER))
                    {
                        pDetailes = inFileText.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);

                        switch (pDetailes.Length - 1)
                        {
                            case 0:
                                inProxyFileTextArr = new string[] { pDetailes[0] }.ToList();
                                inFileText = pDetailes[0];
                                File.WriteAllText(filePath, pDetailes[0] + Environment.NewLine);
                                break;
                            case 1:
                                inProxyFileTextArr = new string[] { pDetailes[0] + ":".ToList() }.ToList();
                                inFileText = pDetailes[0] + ":" + pDetailes[1];
                                File.WriteAllText(filePath, pDetailes[0] + ":" + pDetailes[1] + Environment.NewLine);
                                break;
                            case 2:
                                inProxyFileTextArr = new string[] { pDetailes[0] + ":" + pDetailes[1] + ":" + pDetailes[2] }.ToList();
                                inFileText = pDetailes[0] + ":" + pDetailes[1] + ":" + pDetailes[2];
                                File.WriteAllText(filePath, pDetailes[0] + ":" + pDetailes[1] + ":" + pDetailes[2] + Environment.NewLine);
                                break;
                            case 3:
                                inProxyFileTextArr = new string[] { pDetailes[0] + ":" + pDetailes[1] + ":" + pDetailes[2] + ":" + pDetailes[3] }.ToList();
                                inFileText = pDetailes[0] + ":" + pDetailes[1] + ":" + pDetailes[2] + ":" + pDetailes[3];
                                File.WriteAllText(filePath, pDetailes[0] + ":" + pDetailes[1] + ":" + pDetailes[2] + ":" + pDetailes[3] + Environment.NewLine);
                                break;

                            default:
                                break;
                        }
                        
                    }
                    else
                    {
                        inProxyFileTextArr = inFileText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList(); 
                        pDetailes = inProxyFileTextArr[0].Split(':');
                    }

                    setWebRequestsProxy(pDetailes);
                }
                catch { }
            }).Start();
        }

        private void setWebRequestsProxy(string[] pDetailes)
        {
            switch (pDetailes.Length - 1)
            {
                case 0:
                    WebPageRequests.pIP = pDetailes[0];
                    break;
                case 1:
                    WebPageRequests.pIP = pDetailes[0];
                    WebPageRequests.pPort = pDetailes[1];
                    break;
                case 2:
                    WebPageRequests.pIP = pDetailes[0];
                    WebPageRequests.pPort = pDetailes[1];
                    WebPageRequests.pUser = pDetailes[2];
                    break;
                case 3:
                    WebPageRequests.pIP = pDetailes[0];
                    WebPageRequests.pPort = pDetailes[1];
                    WebPageRequests.pUser = pDetailes[2];
                    WebPageRequests.pPass = pDetailes[3];
                    break;

                default:
                    break;
            }
        }

        #region list options
        private void addAllLangsForWebhose()
        {
            #region langs
            Langs.Add(new Footprint()
            {
                Option = "Any",
                Checked = true
            });
            Langs.Add(new Footprint()
            {
                Option = "english",
                LangForWebhose = webhose.Languages.english
            });
            Langs.Add(new Footprint()
            {
                Option = "arabic",
                LangForWebhose = webhose.Languages.arabic
            });
            Langs.Add(new Footprint()
            {
                Option = "bulgarian",
                LangForWebhose = webhose.Languages.bulgarian
            });
            Langs.Add(new Footprint()
            {
                Option = "catalan",
                LangForWebhose = webhose.Languages.catalan
            });
            Langs.Add(new Footprint()
            {
                Option = "chinese",
                LangForWebhose = webhose.Languages.chinese
            });
            Langs.Add(new Footprint()
            {
                Option = "croatian",
                LangForWebhose = webhose.Languages.croatian
            });
            Langs.Add(new Footprint()
            {
                Option = "croatian",
                LangForWebhose = webhose.Languages.croatian
            });
            Langs.Add(new Footprint()
            {
                Option = "czech",
                LangForWebhose = webhose.Languages.czech
            });
            Langs.Add(new Footprint()
            {
                Option = "danish",
                LangForWebhose = webhose.Languages.danish
            });
            Langs.Add(new Footprint()
            {
                Option = "estonian",
                LangForWebhose = webhose.Languages.estonian
            });
            Langs.Add(new Footprint()
            {
                Option = "finnish",
                LangForWebhose = webhose.Languages.finnish
            });
            Langs.Add(new Footprint()
            {
                Option = "finnish",
                LangForWebhose = webhose.Languages.finnish
            });
            Langs.Add(new Footprint()
            {
                Option = "french",
                LangForWebhose = webhose.Languages.french
            });
            Langs.Add(new Footprint()
            {
                Option = "finnish",
                LangForWebhose = webhose.Languages.finnish
            });
            Langs.Add(new Footprint()
            {
                Option = "german",
                LangForWebhose = webhose.Languages.german
            });
            Langs.Add(new Footprint()
            {
                Option = "greek",
                LangForWebhose = webhose.Languages.greek
            });
            Langs.Add(new Footprint()
            {
                Option = "hebrew",
                LangForWebhose = webhose.Languages.hebrew
            });
            Langs.Add(new Footprint()
            {
                Option = "hungarian",
                LangForWebhose = webhose.Languages.hungarian
            });
            Langs.Add(new Footprint()
            {
                Option = "icelandic",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "indonesian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "italian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "japanese",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "korean",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "latvian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "lithuanian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "norwegian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "persian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "polish",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "portuguese",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "romanian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "russian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "serbian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "slovak",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "slovenian",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "spanish",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "swedish",
                LangForWebhose = webhose.Languages.icelandic
            });
            Langs.Add(new Footprint()
            {
                Option = "turkish",
                LangForWebhose = webhose.Languages.icelandic
            });
            #endregion

            #region site types
            SiteTypesWebHose.Add(new Footprint()
            {
                Option = "Any",
                Checked = true
            });

            SiteTypesWebHose.Add(new Footprint()
            {
                Option = "blogs",
                SiteTypeWebhose = SiteTypes.blogs
            });

            SiteTypesWebHose.Add(new Footprint()
            {
                Option = "discussions",
                SiteTypeWebhose = SiteTypes.discussions
            });

            SiteTypesWebHose.Add(new Footprint()
            {
                Option = "news",
                SiteTypeWebhose = SiteTypes.news
            });
            #endregion

            #region performance score
            PerformanceScores.Add("Any");
            for (int i = 0; i < 11; i++)
            {
                PerformanceScores.Add(Convert.ToString(i));
            }
            #endregion
        }

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

        private void createSitesListWebhose()
        {
            WebsitesForBlogs.Clear();

            foreach (Footprint f in WebsitesForBlogs)
            {
                f.PropertyChanged += f_PropertyChanged;
            }
        }

        private void createSavedOptions()
        {
            try
            {
                new Thread(() =>
                {
                    string savedDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "Prospector", "SavedFootPrints");
                    if (!Directory.Exists(savedDir)) return;

                    string filePath = Path.Combine(savedDir, "SaveFootprints.txt");
                    if (!File.Exists(filePath)) return;


                    string[] fileLines = File.ReadAllLines(filePath);
                    
                        foreach (string line in fileLines)
                        {
                            string[] lineData = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                var sf = new SavedFootprint() { Name = lineData[0], Footprint = lineData[1] };
                                if (lineData.Length >= 3) sf.Type = lineData[2];
                                SavedFP.Add(sf);
                            });
                        }
                }).Start();
            }
            catch { }
        }

        void f_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
            {
                setFootprintText("");
            }
        }

        #endregion

        private void setFootprintText(string old)
        {
            if(tCSelectedTabIndex != Custom && tCSelectedTabIndex != Saved) FootPrintString = Keyword;
            else
            {
                if (FootPrintString.IsNullOrEmpty()) FootPrintString = Keyword;
                else
                {
                    var regex = new Regex(Regex.Escape(old));
                    FootPrintString = regex.Replace(FootPrintString, keyword, 1);
                }
                return;
            }

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

            if(FootPrintString != null)
                FootPrintString = FootPrintString.Replace("%22", "\"");
        }

        internal void RaiseTextChangedDW()
        {
            QueryStringDW = Keyword;
            foreach (var option in DWGrdOptions)
            {
                foreach (var optionvalue in option.DWSelectedSiteOptions)
                {
                    if (optionvalue.Value.IsNullOrEmpty()) continue;

                    QueryStringDW += " " + optionvalue.Type + ":" + optionvalue.Value;
                }
            }

            if(IsDWLive) QueryStringDW += " is_live:true";
        }

        private void search(object param)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            IsNotSerching = false;

            new Thread( () =>
            {
                try {
                    #region darkweb
                    if(TCSelectedTabIndex == FootPrintsOptionsVM.DarkWeb)
                    {
                        SearchDW(true,false,false);
                        //   Console.WriteLine(output["DarkwebPosts"][0]["source"]["site"]);

                    }
                    #endregion

                    #region webhose
                    else if (TCSelectedTabIndex == FootPrintsOptionsVM.Webhose)
                    {
                        WebhoseRequest clientRequest = new WebhoseRequest("d8010e66-8d57-4242-a2e1-22e2ad61a45f");
                        WebhoseQuery clientQuery = new WebhoseQuery();

                        clientQuery.AddAllTerms(Keyword); // what you want to search
                                                          //clientQuery.AddSomeTerms("apple iphone", "samsung", "esny"); // words that may be in the search

                        if (!Langs.Any(l => l.Checked && l.Option == "Any"))
                        {
                            foreach (Footprint fp in Langs)
                            {
                                if (fp.Checked)
                                {
                                    clientQuery.AddLanguages(fp.LangForWebhose);
                                }
                            }
                        }

                        if (!TLDs.Any(t => t.Option == "Any" && t.Checked))
                        {
                            foreach (Footprint fp in TLDs)
                            {
                                if (fp.Checked)
                                {
                                    clientQuery.AddSiteSuffix(fp.Option.Replace('.', ' ').ToLower().Trim());
                                }
                            }
                        }

                        //SiteTypesWebHose
                        if (!SiteTypesWebHose.Any(t => t.Option == "Any" && t.Checked))
                        {
                            foreach (Footprint fp in SiteTypesWebHose)
                            {
                                if (fp.Checked)
                                {
                                    clientQuery.AddSiteTypes(fp.SiteTypeWebhose);
                                }
                            }
                        }

                        // Limit the results to a specific site suffix
                        if (!string.IsNullOrEmpty(SpecificSites) && !string.IsNullOrWhiteSpace(SpecificSites))
                        {
                            string[] splitsites = SpecificSites.Split(',');
                            clientQuery.AddSites(splitsites);
                        }

                        //SpecificKws
                        if (!string.IsNullOrEmpty(SpecificKws) && !string.IsNullOrWhiteSpace(SpecificKws))
                        {
                            string[] splitsites = SpecificKws.Split(',');
                            clientQuery.AddSomeTerms(splitsites);
                        }

                        if (CmbPerformanceScoresIndex > 0)
                        {
                            clientQuery.PerformanceScore = Convert.ToInt32(PerformanceScores[CmbPerformanceScoresIndex]);
                        }

                        if (KWinTitleIsChecked)
                        {
                            clientQuery.Title = Keyword;
                        }

                        if (KWinContentIsChecked)
                        {
                            clientQuery.BodyText = Keyword;
                        }

                        if (Checked_PostsWithVidsAll) clientQuery.HasVideo = null;
                        if (Checked_PostsWithNoVids) clientQuery.HasVideo = false;
                        if (Checked_PostsWithVids) clientQuery.HasVideo = true;

                        // filtring by country  
                        if (!string.IsNullOrWhiteSpace(CountryCode) && !string.IsNullOrEmpty(CountryCode))
                             clientQuery.AddCountries(CountryCode);

                        clientQuery.ResponseSize = ResponseSize;


                        WebhoseResponse1 responceWithQuery = clientRequest.getResponse1(clientQuery);
                        responceWithQuery.posts = responceWithQuery.posts.OrderByDescending(r => r.thread.performance_score == null ? 0 : Convert.ToInt32(r.thread.performance_score)).ThenByDescending(r => r.thread.domain_rank == null ? 0 : Convert.ToInt32(r.thread.domain_rank)).ToArray();
                        foreach (WebhosePost1 post in responceWithQuery.posts)
                        {
                            string description = post.text;
                            if(!string.IsNullOrEmpty(description) && !string.IsNullOrWhiteSpace(description))
                            {
                                if (description.Length > 500) description = description.Substring(0, 500);
                                description = description.Trim();
                            }

                            SearchResult sResult = new SearchResult()
                            {
                                Title = post.title,
                                Keyword = Keyword,
                                Link = post.url,
                                Description = description,
                                SearchEngine = "Webhose",
                                Position = post.ord_in_thread,
                                Published = post.published,
                                DomainScore = "Domain Rank: " + post.thread.domain_rank,
                                WebhoseExtraVisible = Visibility.Visible,
                                PerformanceScore = "Performance: " + post.thread.performance_score,
                                Lang =  post.language,
                                 Type = "Type: " + post.thread.site_type,
                                   Pnum = "Participants: " + post.thread.participants_count,
                                    Country = "Country: " + post.thread.country,
                                     Ptotal = "Replies: " + post.thread.replies_count,
                                Spamscore = "Spam Score: " + post.thread.spam_score,


                            };
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                ListResults.Add(sResult);
                                l_Webhose.Add(sResult);
                            });
                        }
                        Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });

                        foreach (var result in ListResults)
                        {
                            if (result.SocialStatsReplys != null) continue;

                            result.SocialStatsReplys = new SocialStatsReplys();
                            result.SocialStatsReplys.GetAllStatsFor(result.Link);
                        }

                        Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_KKSearch + " " + Keyword);
                        
                    }
                    #endregion
                    else
                    {
                        if (TCSelectedTabIndex == FootPrintsOptionsVM.Saved)
                        {
                            if (SavedFP[SISavedFP].Type == "DARKWEB")
                            {
                                QueryStringDW = FootPrintString;
                                SearchDW(false, true, false);
                                IsNotSerching = true;
                                Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                                return;
                            }
                            //UseDarkWebIsChecked   Console.WriteLine(output["DarkwebPosts"][0]["source"]["site"]);

                        }
                        else if (TCSelectedTabIndex == FootPrintsOptionsVM.Custom && UseDarkWebIsChecked)
                        {
                            QueryStringDW = FootPrintString;
                            SearchDW(false,false,true);
                            IsNotSerching = true;
                            Application.Current.Dispatcher.Invoke(() => { Mouse.OverrideCursor = null; });
                            return;

                        }
                        if (UseProxy && inProxyFileTextArr != null)
                        {
                            string[] pDetailes = inProxyFileTextArr[proxyIndex].Split(':');
                            setWebRequestsProxy(pDetailes);
                            if (proxyIndex < inProxyFileTextArr.Count - 1)
                            {
                                proxyIndex++;
                            }
                            else
                            {
                                proxyIndex = 0;
                            }
                        }
                        string Query = FootPrintString;
                        Query = Query.Replace("\"", "%22");
                        Query = Query.Trim();
                        Query = Query.Replace(' ', '+');
                        Query = String.Format(@"https://google.com/search?v=1.0&q={0}", Query);
                        Query = Query + TimeFrames[CmbTimeframeIndex].Query;

                        Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_ProspectorSearch + " " + Query);

                        GetKeywordRankings(Query, CmbMaxPAgesIndex + 1, false);

                        foreach (var result in ListResults)
                        {
                            if (result.SocialStatsReplys != null) continue;

                            result.SocialStatsReplys = new SocialStatsReplys();
                            result.SocialStatsReplys.GetAllStatsFor(result.Link);
                            Task.Run(async()=> 
                            {
                                string websitehtml = await WebRequests.AsyncDownloadStringWithProfileProxy(result.Link);
                                websitehtml = websitehtml.ToLower();
                                string kwToFind = Keyword.IsNullOrEmpty() ? FootPrintString.Contains(' ') ? FootPrintString.Remove(FootPrintString.IndexOf(' ')).ToLower() : FootPrintString.ToLower() : Keyword.ToLower();
                                result.TimesKwFound = websitehtml.SplitAndRemoveEmpty(kwToFind).Length - 1;
                            });
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("somthing went wrong during the serch. " +ex.Message);
                }
                IsNotSerching = true;
                Application.Current.Dispatcher.Invoke(()=>{ Mouse.OverrideCursor = null; });
            }).Start();
        }

        private async void SearchDW(bool raiseTextChanged, bool addtosaved, bool addtocusto)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
           if(raiseTextChanged) RaiseTextChangedDW();

            var client = new WebhoseClient(token: "d8010e66-8d57-4242-a2e1-22e2ad61a45f");
            var query_params = new Dictionary<string, string>
                            {
                                {
                                    "q",
                                    QueryStringDW
                                    //"tech site:onion.com title:titletech language:arabic published:>1525726800000 crawled:<1525467600000 is_live:true external_links:https://www.linkedin.com* site_type:news site_type:blogs thread.title:Blockchain thread.section_title:Bitcoin thread.url:http://fhacksnplmzxaaoo.onion/showthread.php thread.published:1525122000000"
                                }
                            };
            var output = await client.QueryAsync("darkwebFilter", query_params);

            //  Console.WriteLine(output["DarkwebPosts"][0]["text"]);
            //  Console.WriteLine(output["DarkwebPosts"][0]["title"]);


            // Get the next batch of posts

            output = await output.GetNextAsync();
            foreach (var post in output["DarkwebPosts"])
            {
                var description = post["text"].ToString();
                if (description.Length > 500) description = description.Remove(500);
                SearchResult sResult = new SearchResult()
                {
                    Title = post["title"].ToString(),
                    Keyword = Keyword,
                    Link = post["url"].ToString(),
                    Description = description,
                    SearchEngine = "Webhose",
                    Published = post["thread"]["published"].ToString(),
                    DwExtras = Visibility.Collapsed,
                };

                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    ListResults.Add(sResult);
                    if (addtosaved)
                        l_Saved.Add(sResult);
                    else if (addtocusto)
                        l_Custom.Add(sResult);
                    else
                        l_DW.Add(sResult);
                });
            }
        }

        private void sendLinkToBrowser(object obj)
        {
            try
            {
                string commandParam = obj as string;
                if (commandParam == null) return;
                switch (commandParam)
                {
                    case "PBNPOSTER":
                        try
                        {
                            OnSelectedSendToPbn(ListResults[SIListResults].Link, ListResults[SIListResults].Title, null, ListResults[SIListResults].Published, ListResults[SIListResults].Description);
                            Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_ProspectorToPBne + " " + ListResults[SIListResults].Link);
                        }
                        catch
                        {
                            try
                            {
                                OnSelectedSendToPbn(ListResults[SIListResults - 1].Link, ListResults[SIListResults - 1].Title, null, ListResults[SIListResults - 1].Published, ListResults[SIListResults - 1].Description);
                                Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_ProspectorToPBne + " " + ListResults[SIListResults - 1].Link);
                            }
                            catch
                            {
                                MessageBox.Show("Couldnt curate link.");
                            }
                        }
                        break;

                    case "Curaste":
                        try
                        {
                            string htmlstring = "<blockquote>";
                            if (!string.IsNullOrEmpty(ListResults[SIListResults].Title) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Title))
                                htmlstring += "<h1>" + ListResults[SIListResults].Title + "</h1>";
                            if (!string.IsNullOrEmpty(ListResults[SIListResults].Description) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Description))
                                htmlstring += "<p>" + ListResults[SIListResults].Description + "</p>";
                            if (!string.IsNullOrEmpty(ListResults[SIListResults].Link) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Link))
                                htmlstring += "<a href=\"" + ListResults[SIListResults].Link + " \" > " + ListResults[SIListResults].Link + " </a>";
                            htmlstring += "</blockquote>";

                            MyFilesDatabase.SetClipboardText(htmlstring);
                        }
                        catch
                        {
                            try
                            {
                                string htmlstring = "<blockquote>";
                                if (!string.IsNullOrEmpty(ListResults[SIListResults - 1].Title) && !string.IsNullOrWhiteSpace(ListResults[SIListResults - 1].Title))
                                    htmlstring += "<H1>" + ListResults[SIListResults - 1].Title + "</H1>";
                                if (!string.IsNullOrEmpty(ListResults[SIListResults - 1].Description) && !string.IsNullOrWhiteSpace(ListResults[SIListResults - 1].Description))
                                    htmlstring += "<P>" + ListResults[SIListResults - 1].Description + "</P>";
                                if (!string.IsNullOrEmpty(ListResults[SIListResults - 1].Link) && !string.IsNullOrWhiteSpace(ListResults[SIListResults - 1].Link))
                                    htmlstring += "<A href=\"" + ListResults[SIListResults - 1].Link + " \" > " + ListResults[SIListResults - 1].Link + " </a>";
                                htmlstring += "</blockquote>";

                                MyFilesDatabase.SetClipboardText(htmlstring);
                            }
                            catch
                            {
                                MessageBox.Show("Failed to set clipboard curation.");
                            }
                        }
                        break;

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
                        var resulstsList = SocialStatsFunctions.OrderStatsBy(ListResults.ToList(), commandParam);
                        if (resulstsList == null || resulstsList.Count() == 0) return;

                        ListResults.Clear();
                        foreach (var r in resulstsList) ListResults.Add(r as SearchResult);
                        break;

                    case "ORDERBY_KEYWORDFOUND":
                        var orderdTempByTimesFound = ListResults.OrderByDescending(r => r.TimesKwFound);
                        ListResults.Clear();
                        foreach (var r in orderdTempByTimesFound) ListResults.Add(r);
                        break;

                    default:
                        if(commandParam == "BROWSERTOR")
                        {
                            try
                            {
                                var exePath = TorPath;
                                exePath = exePath.Replace("\\\\", "\\");

                                var pTCPath = exePath.Replace("firefox.exe", @"TorBrowser\Data\Browser\") + GloableProfData.PData.ProjectName;

                                Process process = new Process();
                                process.StartInfo.FileName = exePath;
                                process.StartInfo.Arguments = "-new-instance -allow-remote -new-tab -url \"" + ListResults[SIListResults].Link + "\"";
                                process.StartInfo.UseShellExecute = true;
                                process.Start();
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                OnClickedSearch(ListResults[SIListResults].Link, commandParam == "BROWSERFF");
                                Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_ProspectorToBrowser + " " + ListResults[SIListResults].Link);
                            }
                            catch
                            {
                                try
                                {
                                    OnClickedSearch(ListResults[SIListResults - 1].Link, commandParam == "BROWSERFF");
                                    Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_ProspectorToBrowser + " " + ListResults[SIListResults - 1].Link);
                                }
                                catch
                                {
                                    MessageBox.Show("Couldnt open link.");
                                }
                            }
                        }
                        break;
                }
            }
            catch { }
        }

        internal void SendToTheBrowser(string link)
        {
            OnClickedSearch(link,false);
        }

        private void OnRankCheckCkicked(object param)
        {
            if (MozscapeAPI.mozId == "" || MozscapeAPI.mozSecret == "")
            {
                //Application.Current.Dispatcher.Invoke((Action)delegate
                //{
                //    Mouse.OverrideCursor = null;
                //});
                MessageBox.Show("Set your moz id and secret first.");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;
            new Thread(() =>
            {
                try
                {
                    switch ((string)param)
                    {
                        case "MOZ":
                           
                            /*instantiate a new mozscapeAPI object*/
                            MozscapeAPI mozAPI = new MozscapeAPI();

                            /*build our API URL */
                            string strAPIURL = mozAPI.CreateAPIURL(MozscapeAPI.mozId, MozscapeAPI.mozSecret, 1, "url metrics", ListResults[SIListResults].Link, "");

                            /*get the results string */
                            string strResults = mozAPI.FetchResults(strAPIURL);

                            /*parse the results string. The ParseURLMetrics function returns a MozscapeURLLinkMetrics objects */
                            MozscapeLinkMetric msURLMetrics = mozAPI.ParseURLMetrics(strResults);

                            /*access the object values*/
                            //string title = msURLMetrics.ut;
                            //string canonicalURL = msURLMetrics.uu;
                            //string subdomain = msURLMetrics.ufq;
                            //string rootDomain = msURLMetrics.upl;
                            //string strExternalLinks = msURLMetrics.ueid;
                            //string subdomainLinks = msURLMetrics.feid;
                            //string rootDomains = msURLMetrics.peid;
                            //string equityLinks = msURLMetrics.ujid;
                            //string subdomainsLinking = msURLMetrics.uifq;
                            //string rootDomainLinkins = msURLMetrics.uipl;
                            //string links = msURLMetrics.uid;
                            //string subdomainSubdomainsLinking = msURLMetrics.fid;
                            //string rootDomainRootDomainsLinking = msURLMetrics.pid;
                            //string mozRankURLRAW = msURLMetrics.umrr;
                            //string mozRankURLto10 = msURLMetrics.umrp;
                            //string mozRankSubDomain10 = msURLMetrics.fmrp;
                            //string mozRankSubDomainRAW = msURLMetrics.fmrr;
                            //string mozRankRoot10 = msURLMetrics.pmrp;
                            //string mozRankRootRAW = msURLMetrics.pmrr;
                            //string mozTrust10 = msURLMetrics.utrp;
                            //string mozTrustRAW = msURLMetrics.utrr;
                            //string mozTrustSub10 = msURLMetrics.ftrp;
                            //string mozTrustSubRAW = msURLMetrics.ftrr;
                            //string mozTrustRootRAW = msURLMetrics.ptrr;
                            //string mozTrustRoot10 = msURLMetrics.ptrp;

                            // string httpStatisCode = msURLMetrics.us;

                            string pageAuthority = msURLMetrics.upa;
                            string domainAuthority = msURLMetrics.pda;

                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                ListResults[SIListResults].PageAuthority = "PA: " + pageAuthority;
                                ListResults[SIListResults].DomainAuthority = "DA: " + domainAuthority;
                                ListResults[SIListResults].AuthorityVisible = Visibility.Visible;
                                Mouse.OverrideCursor = null;
                            });

                            //ListResults[SIListResults].AuthorityVisible = Visibility.Visible;

                            //string externalLinks = msURLMetrics.ued;
                            //string timeLastCrawled = msURLMetrics.ulc;
                            //var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            //DateTime dt = epoch.AddSeconds(Convert.ToInt32(timeLastCrawled));
                            //timeLastCrawled = dt.ToString();
                            break;

                        case "MOZALL":
                            MozscapeAPI mozAPI1 = new MozscapeAPI();
                            foreach (SearchResult res in ListResults)
                            {
                                if (res.AuthorityVisible == Visibility.Visible) continue;

                                string strAPIURL1 = mozAPI1.CreateAPIURL(MozscapeAPI.mozId, MozscapeAPI.mozSecret, 1, "url metrics", res.Link, "");
                                string strResults1 = mozAPI1.FetchResults(strAPIURL1);
                                MozscapeLinkMetric msURLMetrics1 = mozAPI1.ParseURLMetrics(strResults1);

                                string pageAuthority1 = msURLMetrics1.upa;
                                string domainAuthority1 = msURLMetrics1.pda;
                                Thread.Sleep(1100);
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    res.PageAuthority = "PA: " + pageAuthority1;
                                    res.DomainAuthority = "DA: " + domainAuthority1;
                                    res.AuthorityVisible = Visibility.Visible;
                                    Mouse.OverrideCursor = null;
                                });
                            }

                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("If this is a moz restriction wait between 5 - 10 seconds before using there api again. Error: " + ex.Message);
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Mouse.OverrideCursor = null;
                    });
                }
            }).Start();
        }

        private void OnRefreshSavewdFootprints(object param)
        {
            SavedFP.Clear();
            createSavedOptions();
        }

        //OnSetMozClicked
        private void OnSetMozClicked(object obj)
        {
            SaveMozKeysWindow smw = new SaveMozKeysWindow();
            smw.tbSecret.Text = MozscapeAPI.mozSecret;
            smw.tbID.Text = MozscapeAPI.mozId;
            smw.ShowDialog();
            if (smw.OKClicked)
            {
                MyFilesDatabase.SetMozIds();
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

                case Webhose:
                    l_Webhose.Clear();
                    break;

                case Saved:
                    l_Saved.Clear();
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

        private void OnSaveFootprintClicked(object param)
        {
            if (string.IsNullOrWhiteSpace(FootPrintString) || string.IsNullOrEmpty(FootPrintString))
            {
                MessageBox.Show("Create a footprint first.");
                return;
            }
            SaveFootprintWindow sfw = new SaveFootprintWindow();
            sfw.ShowDialog();
            if (sfw.OkClicked)
            {
                if (!string.IsNullOrWhiteSpace(sfw.tbName.Text) && !string.IsNullOrEmpty(sfw.tbName.Text))
                {
                    SavedFP.Clear();
                    switch (param as string)
                    {
                        case "DARKWEB":
                            SavedFP.Add(new SavedFootprint() { Name = sfw.tbName.Text, Footprint = QueryStringDW, Type = "DARKWEB" });
                            break;

                        default:
                            SavedFP.Add(new SavedFootprint() { Name = sfw.tbName.Text, Footprint = FootPrintString, Type= "default" });
                            break;
                    }

                    saveSavedFootprints(true);
                }
            }
        }

        

        private void DeleteSavedFootprintClicked(object param)
        {
            try
            {
                SavedFP.RemoveAt(SISavedFP);
            }
            catch { }
            saveSavedFootprints(false);
        }

        private void saveSavedFootprints(bool reAdd)
        {
            new Thread(() =>
            {
                try
                {
                    string savedDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "Prospector", "SavedFootPrints");
                    if (!Directory.Exists(savedDir)) Directory.CreateDirectory(savedDir);

                    string filePath = Path.Combine(savedDir, "SaveFootprints.txt");
                    if (File.Exists(filePath))
                    {
                        string[] fileLines = File.ReadAllLines(filePath);

                        foreach (string line in fileLines)
                        {
                            string[] lineData = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                            SavedFootprint fpFromFile = new SavedFootprint() { Name = lineData[0], Footprint = lineData[1] };
                            if(lineData.Length >=3)
                            {
                                fpFromFile.Type = lineData[2];
                            }
                            foreach (SavedFootprint fp in SavedFP)
                            {
                                if (fp.Name == fpFromFile.Name) continue;
                            }
                            if (reAdd)
                            {
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    SavedFP.Add(fpFromFile);
                                });
                                SISavedFP = 0;
                            }
                        }
                    }

                    string fileContents = "";
                    foreach (SavedFootprint sfp in SavedFP)
                    {
                        fileContents += sfp.Name + MyFilesDatabase.SPLITTER + sfp.Footprint + MyFilesDatabase.SPLITTER + sfp.Type + Environment.NewLine;
                    }

                    File.WriteAllText(filePath, fileContents);
                }
                catch { }
            }).Start();
        }
        
        public void GetKeywordRankings(string link, int maxSearchPages, bool? includeBing)
        {
            GoogleCrawler googleCrawler = new GoogleCrawler(link, maxSearchPages);
            googleCrawler.LinkWasAddedToList += new Action<SearchResult, bool>(Crawler_LinkWasAdded);
            googleCrawler.OnPageCountUpdate += new Action<int, int>(Crawler_OnPageAdded);
            googleCrawler.OnReturnResults += new Action<bool>(Crawler_OnesultsReturned);
            googleCrawler.FindResults(UseProxy, 1);
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
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                result.AuthorityVisible = Visibility.Collapsed;

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

                    case Saved:
                        l_Saved.Add(result);
                        break;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
