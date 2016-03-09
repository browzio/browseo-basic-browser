using GoViral.Models;
using GoViral.Windows;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Xilium.CefGlue.Client;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using Xilium.CefGlue;
using System.Diagnostics;
using System.Messaging;
using System.AddIn.Hosting;
using System.ComponentModel;
using Organiser.Common.Classes.Crawler;
using System.Security.Permissions;
using CrawlerContracts.PluginHosting;
using CrawlerContracts;
using System.Runtime.Remoting;
using GoViral.Helpers;

namespace GoViral.ViewModels
{
    public class GoViralVM : MarshalByRefObject, INotifyPropertyChanged 
    {
        #region propchanged and marshal
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        public override object InitializeLifetimeService()
        {
            return null; //live forever
        }
        #endregion

        public ICommand OnBtnClicked { get; set; }
        public ICommand CTMenuClick { get; set; }


        private ObservableCollection<Folder> folders;
        public ObservableCollection<Folder> Folders
        {
            get { return folders; }
            set { folders = value; }
        }
        public int sIFolders;
        public int SIFolders
        {
            get
            {
                return sIFolders;
            }
            set
            {
                if (value == -1)
                {
                    SIFolders = 0;
                }

                sIFolders = value;
                RaisePropertyChanged("SIFolders");
                RaisePropertyChanged("SelectedFolder");
            }
        }
        public Folder SelectedFolder
        {
            get
            {
                if (Folders != null && Folders.Count > 0 && SIFolders > -1)
                    return Folders[SIFolders];
                else
                    return null;
            }
        }

        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (wfh == null)
                {
                    RefreshBrowser();
                }
                return wfh;
            }
            set
            {
                wfh = value; RaisePropertyChanged("WebBrowserHost");
            }
        }
        public BrowserCntrl WebBrowser { get; set; }
        private string browserPreviewStatus;
        public string BrowserPreviewStatus
        {
            get { return browserPreviewStatus; }
            set { browserPreviewStatus = value; RaisePropertyChanged("BrowserPreviewStatus"); }
        }


        #region pbar
        private Visibility pBarVisible;
        public Visibility PBarVisible
        {
            get { return pBarVisible; }
            set { pBarVisible = value; RaisePropertyChanged("PBarVisible"); }
        }
        //IsIndeterminate
        private bool sIndeterminate;
        public bool IsIndeterminate
        {
            get { return sIndeterminate; }
            set { sIndeterminate = value; RaisePropertyChanged("IsIndeterminate"); }
        }
        //PBarValue 
        private double pBarValue;
        public double PBarValue
        {
            get { return pBarValue; }
            set { pBarValue = value; RaisePropertyChanged("PBarValue"); }
        }
        private string loadingStatus; 
        public string LoadingStatus
        {
            get { return loadingStatus; }
            set { loadingStatus = value; RaisePropertyChanged("LoadingStatus"); }
        }
        #endregion

        private CrawlerHost mCrawlerHost;

       // private Task PopulateListTask; 
        private TaskScheduler uiContextScheduler;

        private int lastSelectedIndex = -1;
        private string resultsErrors = "";

        private object mLock = new object();

        public GoViralVM()
        {
            OnBtnClicked = new RelayCommand(On_OnBtnClicked);
            CTMenuClick = new RelayCommand(On_CTMenuClick);

            Folders = new ObservableCollection<Folder>();

            //PopulateListTask = Task.Factory.StartNew(() =>
            //{
            //    PopulatList();
            //}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            new Thread(PopulatList).Start();

            uiContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            PBarVisible = Visibility.Collapsed;
            IsIndeterminate = true;
        }

        #region command raised methods
        public void On_CTMenuClick(object param)
        {
            if (Folders.Count == 0) return;
            string commandParam = param as string;
            if (commandParam == null) return;
            switch (commandParam)
            {
                case "Edit":
                    Task.Factory.StartNew(() =>
                    {
                        SetNameAndDataWindow setFolderNAmeWindow = new SetNameAndDataWindow();
                        setFolderNAmeWindow.Title = "Folder Option";
                        setFolderNAmeWindow.tblockInfo.Visibility = Visibility.Collapsed;
                        setFolderNAmeWindow.tbInputText.Text = Folders[SIFolders].FolderTitle;
                        setFolderNAmeWindow.ShowDialog();
                        if (setFolderNAmeWindow.OkClicked && !string.IsNullOrEmpty(setFolderNAmeWindow.tbInputText.Text) && !string.IsNullOrWhiteSpace(setFolderNAmeWindow.tbInputText.Text))
                        {
                            if (setFolderNAmeWindow.tbInputText.Text != Folders[SIFolders].FolderTitle)
                            {
                                if (Folders.Any(f => f.FolderTitle.ToLower().Trim() == setFolderNAmeWindow.tbInputText.Text.ToLower().Trim()))
                                {
                                    MessageBox.Show(setFolderNAmeWindow.tbInputText.Text + " Already exists, use a different name.");
                                    return;
                                }

                                Folders[SIFolders].FolderTitle = setFolderNAmeWindow.tbInputText.Text;
                                SaveList();
                            }
                        }
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                    break;

                case "Delete":
                    if(MessageBox.Show("Are you sure you want to delete " + Folders[SIFolders].FolderTitle, "Are You Sure?", MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Folders.RemoveAt(SIFolders);

                        Task.Factory.StartNew(() =>
                        {
                            SaveList();
                        });
                    }
                    break;

                case "ORDER_PostsByLikes":
                case "ORDER_PostsByShares":
                case "ORDER_PostsByComments":
                    if (SelectedFolder != null && SelectedFolder.SelectedPageFBGraphData != null)
                    {
                        if (SelectedFolder.SelectedPageFBGraphData.posts != null && SelectedFolder.SelectedPageFBGraphData.posts.data != null)
                        {
                            List<FacebookGraphPostResult> pdOrderd = SelectedFolder.SelectedPageFBGraphData.posts.data.OrderByDescending(l =>
                            commandParam == "ORDER_PostsByLikes" ?
                            l.likes == null ? 0 : l.likes.summary == null ? 0 : l.likes.summary.total_count :
                            commandParam == "ORDER_PostsByComments" ?
                            l.comments == null ? 0 : l.comments.summary == null ? 0 : l.comments.summary.total_count :
                            l.shares == null ? 0 : l.shares.count).ToList();

                            SelectedFolder.SelectedPageFBGraphData.posts.data.Clear();
                            foreach (FacebookGraphPostResult pResult in pdOrderd)
                            {
                                SelectedFolder.SelectedPageFBGraphData.posts.data.Add(pResult);
                            }
                        }

                        if (SelectedFolder.SelectedPageFBGraphData.feed != null && SelectedFolder.SelectedPageFBGraphData.feed.data != null)
                        {
                            List<FeedData> pdOrderd = SelectedFolder.SelectedPageFBGraphData.feed.data.OrderByDescending(l =>
                                commandParam == "ORDER_PostsByLikes" ?
                                l.likes == null ? 0 : l.likes.summary == null ? 0 : l.likes.summary.total_count :
                                commandParam == "ORDER_PostsByComments" ?
                                l.comments == null ? 0 : l.comments.summary == null ? 0 : l.comments.summary.total_count:
                                l.shares == null ? 0 : l.shares.count).ToList();

                            SelectedFolder.SelectedPageFBGraphData.feed.data.Clear();
                            foreach (FeedData fResult in pdOrderd)
                            {
                                SelectedFolder.SelectedPageFBGraphData.feed.data.Add(fResult);
                            }
                        }

                        RaisePropertyChanged("SelectedFolder");
                    }
                    break;

                case "ORDERPICS_LIKES":
                case "ORDERPICS_COMMENTS":
                    if (SelectedFolder != null && SelectedFolder.SelectedPageFBGraphData != null)
                    {
                        if (SelectedFolder.SelectedPageFBGraphData.photos != null && SelectedFolder.SelectedPageFBGraphData.photos.data != null && SelectedFolder.SelectedPageFBGraphData.photos.data.Count > 0)
                        {
                            List<Photos.Photo> orderdPhotos = SelectedFolder.SelectedPageFBGraphData.photos.data.OrderByDescending(d => 
                            commandParam == "ORDERPICS_LIKES" ? 
                            d.likes == null? 0 : d.likes.summary == null ? 0 : d.likes.summary.total_count :
                            d.comments == null? 0 : d.comments.summary == null? 0 : d.comments.summary.total_count).ToList();

                            SelectedFolder.SelectedPageFBGraphData.photos.data.Clear();
                            foreach (var d in orderdPhotos)
                            {
                                SelectedFolder.SelectedPageFBGraphData.photos.data.Add(d);
                            }
                        }
                    }
                        break;

                case "ORDERVIDS_LIKES":
                case "OORDERVIDS_COMMENTS":
                case "OORDERVIDS_VIEWS":
                    if (SelectedFolder != null && SelectedFolder.SelectedPageFBGraphData != null)
                    {
                        if (SelectedFolder.SelectedPageFBGraphData.videos != null && SelectedFolder.SelectedPageFBGraphData.videos.data != null && SelectedFolder.SelectedPageFBGraphData.videos.data.Count > 0)
                        {
                            List<Videos.Video> orderdVids = SelectedFolder.SelectedPageFBGraphData.videos.data.OrderByDescending(d =>
                            commandParam == "ORDERVIDS_LIKES" ?
                            d.likes == null ? 0 : d.likes.summary == null ? 0 : d.likes.summary.total_count :
                            commandParam == "OORDERVIDS_COMMENTS" ?
                            d.comments == null ? 0 : d.comments.summary == null ? 0 : d.comments.summary.total_count :
                            d.views).ToList();

                            SelectedFolder.SelectedPageFBGraphData.videos.data.Clear();
                            foreach (var d in orderdVids)
                            {
                                SelectedFolder.SelectedPageFBGraphData.videos.data.Add(d);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void On_OnBtnClicked(object param)
        {
            switch ((string)param)
            {
                case "NEWFOLDER":
                    addNewFolder();
                    break;

                case "MULTILINKS":
                    OpenMultyLinks(null, showWindow: true);
                    break;

                case "SAVE":
                    Task.Factory.StartNew(SaveList);
                    break;


                case "DEDUPE":
                    List<string> links = new List<string>();
                    List<ListOption> listsToRemove = new List<ListOption>();

                    foreach (Folder f in Folders)
                    {
                        listsToRemove.Clear();
                        foreach (ListOption lo in f.SavedLinksList)
                        {
                            if (links.Contains(lo.Url))
                            {
                                listsToRemove.Add(lo);
                            }
                            else
                            {
                                links.Add(lo.Url);
                            }
                        }
                        foreach (ListOption lo in listsToRemove)
                        {
                            f.SavedLinksList.Remove(lo);
                        }
                    }
                    break;

                case "REFRESHTOKEN":
                    WebBrowser.Navigate(Social.FACEBOOK_GRAPH_LINK);
                    break;

                default:
                    break;
            }
        }
        #endregion

        private void OpenMultyLinks(List<string> links, bool showWindow)
        {
            if (displayYouNeedToAddFolderMessage()) return;

            //Application.Current.Dispatcher.Invoke((Action)delegate { });
            SIFolders = lastSelectedIndex;
            if (SIFolders == -1) SIFolders = 0;

            Folder folder = Folders[SIFolders];

            ToFolderChooserWindow fcw = new ToFolderChooserWindow() { DataContext = this };
            fcw.dpName.Visibility = Visibility.Collapsed;
            fcw.dpUrl.Visibility = Visibility.Collapsed;
            if (fcw.ShowDialog() == false) return;

            if (showWindow)
            {
                RssFeedsLinksMultiWindow linksWindow = new RssFeedsLinksMultiWindow();
                if (links != null)
                {
                    foreach (string link in links)
                    {
                        linksWindow.tbInputedText.Text += link + Environment.NewLine;
                    }
                }
                linksWindow.ShowDialog();
                if (linksWindow.OKClicked)
                {
                    string[] splitLinks = linksWindow.tbInputedText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string link in splitLinks)
                    {
                        string pageName = getPageNameFromUrl(link);
                        addNewLoToFolder(Folders[SIFolders], pageName, link, null);
                    }
                }
            }
            else
            {
                foreach (string link in links)
                {
                    string pageName = getPageNameFromUrl(link);
                    addNewLoToFolder(Folders[SIFolders], pageName, link, null);
                }
            }
        }

        private string getPageNameFromUrl(string link)
        {
            string pageName = link;

            if (pageName.Contains("https://www.facebook.com/"))
            {
                pageName = link.Split(new string[] { @"https://www.facebook.com/" }, StringSplitOptions.None)[1];
                if (link.Contains("pages/"))
                    pageName = pageName.Split(new string[] { @"pages/" }, StringSplitOptions.None)[1];
                else if (link.Contains("groups/"))
                    pageName = pageName.Split(new string[] { @"groups/" }, StringSplitOptions.None)[1];
                else if (link.Contains("events/"))
                    pageName = pageName.Split(new string[] { @"events/" }, StringSplitOptions.None)[1];
                else if (link.Contains("places/"))
                    pageName = pageName.Split(new string[] { @"places/" }, StringSplitOptions.None)[1];

                pageName = pageName.Replace("//", "/");
                pageName = pageName.Replace("/", "");
            }

            return pageName;
        }

        internal void BeginImageDownload(string full_picture)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                PBarVisible = Visibility.Visible;
                LoadingStatus = "";

                MyFilesDatabase.DownloadImage(full_picture);

                LoadingStatus = "Done";
                PBarVisible = Visibility.Collapsed;
            });
        }

        void Folder_OnSelectedCheckStats(Folder folder, string url)
        {
            new Thread(() =>
            {
                if (!initializeCrawler())
                {
                    return;
                }

                if (url != null)
                {
                    addLinkForCrawlerAddInn(url, folder, folder.SavedLinksList[folder.SISavedLinks], null, CrawlerStates.FbGraphCrawl);
                }
                else
                {
                    foreach (ListOption option in folder.SavedLinksList)
                    {
                        addLinkForCrawlerAddInn(option.Url, folder, option, null, CrawlerStates.FbGraphCrawl);
                    }
                }

                mCrawlerHost.IninAdin();
            }).Start(); 
        }

        public void BeginAllPhotosScrape(Folder folder, ListOption option, bool useGraph)
        {
            new Thread(() =>
            {
                if (!initializeCrawler())
                {
                    return;
                }
                addLinkForCrawlerAddInn(option.Url, folder, option, null, useGraph ? CrawlerStates.LoadAllPhotos : CrawlerStates.LoadAllPhotos_Crawl);
                mCrawlerHost.IninAdin();
            }).Start();
        }

        internal void BeginAllVideosScrape(Folder folder, ListOption option, bool useGraph)
        {
            new Thread(() =>
            {
                if (!initializeCrawler())
                {
                    return;
                }
                addLinkForCrawlerAddInn(option.Url, folder, option, null, useGraph? CrawlerStates.LoadAllVideos : CrawlerStates.LoadAllVideos_Crawl);
                mCrawlerHost.IninAdin();
            }).Start();
        }

        private void addLinkForCrawlerAddInn(string url, Folder folder, ListOption option, FacebookGraphPostResult facebookGraphPostResult, CrawlerStates state)
        {
            CrawlerPreInitState crawlSearchState = new CrawlerPreInitState() { url = url, folder = folder, option = option, graphResult = facebookGraphPostResult, state = state };
            mCrawlerHost.PreInitStates.Add(crawlSearchState);
        }

        private void Folder_OnCanceledAStatsCheck(ListOption option)
        {
            if (mCrawlerHost != null)
            {
                if (option != null)
                {
                    foreach (CrawlerPreInitState state in mCrawlerHost.PreInitStates.FindAll(s => s.option == option))
                    {
                        mCrawlerHost.PreInitStates.Remove(state);
                    }
                }
                else
                {
                    mCrawlerHost.PreInitStates.Clear();
                }

                if (mCrawlerHost.PreInitStates.Count == 0)
                    PBarVisible = Visibility.Collapsed;
                else
                    mCrawlerHost.navigateToNextUrl();
            }
        }

        #region crawler

        private bool initializeCrawler()
        {
            lock (mLock)
            {
                if (LoadingStatus != null && LoadingStatus.Contains("Initializing Crawler..."))
                    return false;

                PBarVisible = Visibility.Visible;
                LoadingStatus = "Initializing Crawler...";

                if (mCrawlerHost == null)
                {
                    try
                    {
                        mCrawlerHost = new CrawlerHost();
                        mCrawlerHost.OnReportProgress += CrawlerHost_ReportProgress;
                        mCrawlerHost.OnReportGotGraphData += CrawlerHost_OnReportGotGraphData;
                        mCrawlerHost.OnReportFatalError += MCrawlerHost_OnReportFatalError;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not start crawl process. " + ex.Message);
                        PBarVisible = Visibility.Collapsed;
                        LoadingStatus = "Crawler Init Faild.";
                        mCrawlerHost = null;
                        return false;
                    }
                }

                return true;
            }
        }

        private void MCrawlerHost_OnReportFatalError(string userMessage, string fullExceptionText)
        {
            MessageBox.Show("Crawler Has Stoped. " + userMessage + Environment.NewLine + fullExceptionText);
            Folder_OnCanceledAStatsCheck(null);  
            mCrawlerHost = null;
        }

        private void CrawlerHost_ReportProgress(string status)
        {
            if (PBarVisible == Visibility.Collapsed) pBarVisible = Visibility.Visible;

            LoadingStatus = status;
        }

        private void CrawlerHost_OnReportGotGraphData(string serializedXMLResult, CrawlerPreInitState preinintState)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (serializedXMLResult != "N/A")
                    {
                        switch (preinintState.state)
                        {
                            case CrawlerStates.FbGraphCrawl:
                                FacebookGraphData fbgData = serializedXMLResult.XmlDeserializeFromString<FacebookGraphData>();
                                preinintState.option.FBGraphData = fbgData;
                                UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_FacebookCralEvent + " crawled page " + fbgData.name);
                                break;
                            case CrawlerStates.LoadAllPhotos:
                            case CrawlerStates.LoadAllPhotos_Crawl:
                                if (preinintState.option != null && preinintState.option.FBGraphData != null)
                                {
                                    if (preinintState.option.FBGraphData.photos == null)
                                    {
                                        preinintState.option.FBGraphData.photos = new Photos();
                                    }
                                    if (preinintState.option.FBGraphData.photos.data == null)
                                    {
                                        preinintState.option.FBGraphData.photos.data = new ObservableCollection<Photos.Photo>();
                                    }

                                    List<PhotosGraphData> allcrawledPhotos = serializedXMLResult.XmlDeserializeFromString<List<PhotosGraphData>>();
                                    int numToBeGreaterThen = 0;
                                    if (preinintState.state == CrawlerStates.LoadAllPhotos) numToBeGreaterThen = 1;
                                    if (allcrawledPhotos.Count >= numToBeGreaterThen)
                                    {
                                        preinintState.option.FBGraphData.photos.data.Clear();
                                        foreach (PhotosGraphData pd in allcrawledPhotos)
                                        {
                                            if (pd.photos == null) continue;
                                            foreach (Photos.Photo p in pd.photos.data)
                                            {
                                                preinintState.option.FBGraphData.photos.data.Add(p);
                                            }
                                        }

                                        preinintState.option.FBGraphData.photos.paging = null;
                                        RaisePropertyChanged("SelectedFolder");
                                    }      
                                }
                                break;
                            case CrawlerStates.LoadAllVideos:
                            case CrawlerStates.LoadAllVideos_Crawl:
                                if (preinintState.option != null && preinintState.option.FBGraphData != null)
                                {
                                    if(preinintState.option.FBGraphData.videos == null)
                                    {
                                        preinintState.option.FBGraphData.videos = new Videos();
                                    }
                                    if (preinintState.option.FBGraphData.videos.data == null)
                                    {
                                        preinintState.option.FBGraphData.videos.data = new ObservableCollection<Videos.Video>();
                                    }

                                    List<VideosGraphData> allcrawledVideos = serializedXMLResult.XmlDeserializeFromString<List<VideosGraphData>>();
                                    int numToBeGreaterThenVidt = 0;
                                    if (preinintState.state == CrawlerStates.LoadAllVideos) numToBeGreaterThenVidt = 1;
                                    if (allcrawledVideos.Count > numToBeGreaterThenVidt)
                                    {
                                        preinintState.option.FBGraphData.videos.data.Clear();
                                        foreach (VideosGraphData vd in allcrawledVideos)
                                        {
                                            if (vd.videos == null) continue;
                                            foreach (Videos.Video v in vd.videos.data)
                                            {
                                                preinintState.option.FBGraphData.videos.data.Add(v);
                                            }
                                        }
                                        preinintState.option.FBGraphData.videos.paging = null;
                                        RaisePropertyChanged("SelectedFolder");
                                    }
                                    
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    { 
                        resultsErrors += "Could not crawl " + preinintState.url + Environment.NewLine;             
                    }
                }
                catch
                {
                }
                removePreInitState(preinintState);

                if (mCrawlerHost.PreInitStates.Count > 0)
                    mCrawlerHost.navigateToNextUrl();
            }, CancellationToken.None, TaskCreationOptions.None, uiContextScheduler);
        }

        private void removePreInitState(CrawlerPreInitState preinintState)
        {
            mCrawlerHost.PreInitStates.Remove(preinintState);

            LoadingStatus += " END";

            if (mCrawlerHost.PreInitStates.Count == 0)
            {
                IsIndeterminate = true;
                PBarVisible = Visibility.Collapsed;

                if (resultsErrors != "")
                {
                    FlexibleMessageBox.Show(resultsErrors);
                    resultsErrors = "";
                }
            }
        }
        #endregion

        #region save populate and init folders lists
        private void SaveList()
        {
            lock (mLock)
            {
                try
                {
                    bool wasvisible = PBarVisible == Visibility.Visible;
                    if(!wasvisible)
                        PBarVisible = Visibility.Visible; 
                    LoadingStatus = "Saving Do Not Close Project";

                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                    string saveToFilePath = Path.Combine(saveToDir, "info");
                    //if (File.Exists(saveToFilePath)) File.Delete(saveToFilePath);
                   // try
                   // {
                        string sss = Folders.XmlSerializeToString();
                        File.WriteAllText(saveToFilePath, sss);
                    //}
                    //catch(OutOfMemoryException)
                    //{
                    //    File.Delete(saveToFilePath);
                    //    foreach (string chunk in Folders.XmlSerializeToStringChunks())
                    //    {
                    //        File.AppendAllText(saveToFilePath, chunk);
                    //    }
                    //}
                    //MemoryStream sessionData = new MemoryStream();
                    //DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<Folder>));
                    //serializer.WriteObject(sessionData, Folders);

                    //using (FileStream fileStream = File.OpenWrite(saveToFilePath))
                    //{
                    //    sessionData.Seek(0, SeekOrigin.Begin);
                    //    sessionData.CopyTo(fileStream);
                    //    fileStream.Flush();
                    //}

                    LoadingStatus = "Done";    
                    if (!wasvisible)
                        PBarVisible = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    PBarVisible = Visibility.Collapsed;
                    MessageBox.Show("Error saving " + ex.Message);
                }
            }
        }

        public void PopulatList()
        {
            lock (mLock)
            {
                try
                {
                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(saveToDir)) return;

                    string saveToFilePath = Path.Combine(saveToDir, "info");
                    if (!File.Exists(saveToFilePath)) return;

                    // File.ReadAllText(saveToFilePath);

                    ObservableCollection<Folder> data = File.ReadAllText(saveToFilePath).XmlDeserializeFromString<ObservableCollection<Folder>>();
                    // Folder needsToBeSelected = null;
                    foreach (Folder folder in data)
                    {
                        setFolderEvents(folder);
                        folder.CTMenuClick = new RelayCommand(folder.On_CTMenuClick);
                        if (folder.SavedLinksList != null)
                        {
                            foreach (ListOption lo in folder.SavedLinksList)
                            {
                                lo.OnFBGraphDataChanged += folder.Raise_OnFBGraphDataChanged;
                                //if (lo.IsSelected)
                                //{
                                //    needsToBeSelected = folder;
                                //}
                            }
                        }
                        folder.SISavedLinks = 0;
                        Application.Current.Dispatcher.Invoke(delegate { Folders.Add(folder); });
                    }
                    if (SelectedFolder != null)
                    {
                        SelectedFolder.IsEExpanded = true;
                    }
                    RaisePropertyChanged("SelectedFolder");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void setFolderEvents(Folder folder)
        {
            folder.OnLoadInBrowser += Folder_OnLoadInBrowser;
            folder.OnSelectedCheckStats += Folder_OnSelectedCheckStats;
            folder.OnSelectedEditOrRemove += Folder_OnSelectedEditOrRemove;
            folder.OnCanceledAStatsCheck += Folder_OnCanceledAStatsCheck;
            folder.RaiseSiChanged += Folder_RaiseSiChanged;
        }

        private void Folder_RaiseSiChanged(Folder folder)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (Folders != null && Folders.Count > 0)
                    {
                        if (SIFolders != Folders.IndexOf(folder))
                            SIFolders = Folders.IndexOf(folder);
                        else
                            return;
                        if (folder.SISavedLinks == -1) return;   

                        foreach (Folder f in Folders)
                        {
                            if (f != folder)
                            {
                                foreach (ListOption o in f.SavedLinksList)
                                {
                                    if (o.IsSelected)
                                    {
                                        o.IsSelected = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        /// <summary>
        /// for sending links to be stored
        /// </summary>
        /// <param name="link">link to store if null will use multi</param>
        /// <param name="multi">to activate this as a multy add cannot be null if link is null</param>
        public void AsyncAddLinkToList(string link, string type, List<string> multi, bool showLinksWindow)
        {
            //if (!PopulateListTask.IsCompleted)
            //{
            //    await PopulateListTask;
            //}
            if(link == null)
            {
                OpenMultyLinks(multi, showLinksWindow);
                return;
            }

            if (link.Contains("/?ref=br_rs")) link = link.Replace("/?ref=br_rs", "");
            string name = "";
            try
            {
                name = link.Substring(link.LastIndexOf("/") + 1);
            }
            catch { }

            SIFolders = lastSelectedIndex;
            if (SIFolders == -1) SIFolders = 0;

            if (displayYouNeedToAddFolderMessage()) return;

            ToFolderChooserWindow fcw = new ToFolderChooserWindow() { DataContext = this };
            fcw.tbName.Text = name;
            fcw.tbUrl.Text = link;
            if (fcw.ShowDialog() == true)
            {
                lastSelectedIndex = SIFolders;

                addNewLoToFolder(Folders[SIFolders], fcw.tbName.Text, fcw.tbUrl.Text, null);

                new Thread(SaveList).Start();
            }

        }

        private void addNewLoToFolder(Folder folder, string name, string url, FacebookGraphData facebookGraphData)
        {
            if(url.Contains("/?ref=br_rs")) url = url.Replace("/?ref=br_rs", "");

            ListOption lo = new ListOption() { Name = name, Url = url };
            if(facebookGraphData != null)
            {
                lo.FBGraphData = facebookGraphData;
            }
            lo.OnFBGraphDataChanged += Folders[SIFolders].Raise_OnFBGraphDataChanged;
            folder.SavedLinksList.Add(lo);
        }

        private bool displayYouNeedToAddFolderMessage()
        {
            if (Folders.Count == 0 && !File.Exists(Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName, "info")))
            {
                MessageBox.Show("You need to create a folder before pushing links to it.");
                addNewFolder();
            }

            return Folders.Count == 0;
        }

        private void addNewFolder()
        {

            SetNameAndDataWindow setFolderNAmeWindow = new SetNameAndDataWindow();
            setFolderNAmeWindow.Title = "Create Name";
            setFolderNAmeWindow.tblockInfo.Text = "Write in the name for the folder you want to create.";
            setFolderNAmeWindow.ShowDialog();
            if (setFolderNAmeWindow.OkClicked && !string.IsNullOrEmpty(setFolderNAmeWindow.tbInputText.Text) && !string.IsNullOrWhiteSpace(setFolderNAmeWindow.tbInputText.Text))
            {
                if (Folders.Any(f => f.FolderTitle.ToLower().Trim() == setFolderNAmeWindow.tbInputText.Text.ToLower().Trim()))
                {
                    MessageBox.Show(setFolderNAmeWindow.tbInputText.Text + " Already exists, use a different name.");
                    addNewFolder();
                    return;
                }

                Folder folder = new Folder() { FolderTitle = setFolderNAmeWindow.tbInputText.Text };
                setFolderEvents(folder);
                Folders.Add(folder);

                Task.Factory.StartNew(SaveList);
            }
        }

        void Folder_OnSelectedEditOrRemove(Folder folder)
        {
            if (folder != null)
            {
                SIFolders = Folders.IndexOf(folder);

                ToFolderChooserWindow fcw = new ToFolderChooserWindow() { DataContext = this };
                fcw.tbName.Text = folder.SavedLinksList[folder.SISavedLinks].Name;
                fcw.tbUrl.Text = folder.SavedLinksList[folder.SISavedLinks].Url;
                if (fcw.ShowDialog() == false) return;

                FacebookGraphData dataToCopyOver = null;
                if(folder.SavedLinksList[folder.SISavedLinks].FBGraphData != null)
                {
                    dataToCopyOver = folder.SavedLinksList[folder.SISavedLinks].FBGraphData.XmlSerializeToString().XmlDeserializeFromString<FacebookGraphData>();
                } 
                addNewLoToFolder(Folders[SIFolders], fcw.tbName.Text, fcw.tbUrl.Text, dataToCopyOver);
                folder.SavedLinksList.Remove(folder.SavedLinksList[folder.SISavedLinks]);
            }

            Task.Factory.StartNew(() =>
            {
                SaveList();
            });
        }
        #endregion

        #region browser load and events
        void Folder_OnLoadInBrowser(string url)
        {
            WebBrowser.Navigate(url);
        }

        void WebBrowser_OnBrowserLoadingChanged(bool isLoading)
        {
            if (!isLoading)
            {
                BrowserPreviewStatus = "Loaded.";
            }
            else
            {
                BrowserPreviewStatus = "Loading...";
            }
        }
        

        public void RefreshBrowser()
        {
            string url = "";
            if (WebBrowser != null)
            {
                try
                {
                    if (WebBrowser.CBrowser != null && WebBrowser.CBrowser.Browser != null && WebBrowser.CBrowser.Browser.GetMainFrame() != null)
                    {
                        url = WebBrowser.CBrowser.Browser.GetMainFrame().Url;
                    }
                }
                catch { }
                WebBrowser.DisposeBrowserComponents();
            }

            if(wfh != null)
            {
                wfh.Child.Dispose();
            }

            WebBrowser = new Xilium.CefGlue.Client.BrowserCntrl();
            WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
            WebBrowser.init(url);
            if (wfh == null)
                wfh = new WindowsFormsHost();

            wfh.Child = WebBrowser;
            RaisePropertyChanged("WebBrowserHost");

            //WebBrowser.Reload();
        }

        public void DisposeBrowser()
        {
            if(WebBrowser!=null)
                WebBrowser.DisposeBrowserComponents();

            if (mCrawlerHost != null)
                mCrawlerHost.Shutdown();
        }

        #endregion
    }
}



//internal void LoadAllLikes(FacebookGraphPostResult facebookGraphPostResult, string url)
//{
//    if (InitAddinTask == null || InitAddinTask.IsCompleted)
//    {
//        InitAddinTask = Task.Factory.StartNew(() =>
//        {
//            if (!initializeCrawler())
//            {
//                return;
//            }

//            addLinkForCrawlerAddInn(url, null, null, facebookGraphPostResult, CrawlerStates.LikesFromPost);
//            mCrawlerHost.IninAdin(CrawlerStates.LikesFromPost);
//        });
//    }
//}
//private void CrawlerHost_OnReportGotLikesData(string serializedXMLResult, CrawlerPreInitState preinintState)
//{
//    if (serializedXMLResult != "N/A")
//    {
//        LikesData likes = serializedXMLResult.XmlDeserializeFromString<LikesData>();

//        if (preinintState.graphResult != null && likes != null && likes.likes != null)
//        {
//            preinintState.likesData = likes;
//            mCrawlerHost.PostRecrawlLikesStates.Add(preinintState);
//        }
//    }

//    removePreInitState(preinintState);
//}
//private bool initializeCrawler()
//{
//    //if (LoadingStatus != null && LoadingStatus.Contains("Initializing Crawler..."))
//    //    return false;

//    //PBarVisible = Visibility.Visible;
//    //LoadingStatus = "Initializing Crawler..."; 

//    // if (mCrawlerHost == null)
//    //{   
//    //string path = AppDomain.CurrentDomain.BaseDirectory;
//    //Console.WriteLine(path);
//    //if (path == @"C:\Users\eli\Desktop\move\xilium-xilium.cefglue-335450e6011d\BrowserAndFeatures\bin\x86\Debug\" ||
//    //    path == @"C:\Users\eli\Desktop\move\plugins\WpfHost\bin\Release" ||
//    //    path == @"C:\Users\eli\Desktop\move\plugins\WpfHost\bin\Debug" ||
//    //    path == @"C:\Users\eli\Desktop\move\All Browseo Install Files")
//    //{
//    //    string[] ss = AddInStore.Update(path);// (epath);
//    //    string[] kk = AddInStore.RebuildAddIns(path);
//    //}

//    try
//    {
//        //IList<AddInToken> tokens = AddInStore.FindAddIns(typeof(HostView.ProcessorHostView), path);
//        //AddInToken crawlerToken = tokens.SingleOrDefault(t => t.AddInFullName == "Crawler.Crawler");

//        //if (crawlerToken != null)
//        //{    
//        //    ProcessorHostView crawlerAddin = crawlerToken.Activate<HostView.ProcessorHostView>(AddInSecurityLevel.FullTrust);

//        //    mCrawlerHost = new CrawlerHost(crawlerAddin);
//        //    mCrawlerHost.ReportProgress += CrawlerHost_ReportProgress;
//        //    mCrawlerHost.OnReportGotGraphData += CrawlerHost_OnReportGotGraphData;
//        //    //mCrawlerHost.OnReportGotLikesData += CrawlerHost_OnReportGotLikesData;
//        //}
//        //else
//        //{
//        //    MessageBox.Show("Could not start crawl process, could not find process tokens.");
//        //    PBarVisible = Visibility.Collapsed;
//        //    return false;
//        //}

//        mCrawlerHost = new CrawlerHost();
//        mCrawlerHost.ReportProgress += CrawlerHost_ReportProgress;
//        mCrawlerHost.OnReportGotGraphData += CrawlerHost_OnReportGotGraphData;
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show("Could not start crawl process, could not find process tokens.");
//        PBarVisible = Visibility.Collapsed;
//        return false;
//    }
//    //}


//    return true;
//}
//if (mCrawlerHost.PreInitStates.Count == 0)
//{
//    //if (mCrawlerHost.PostInitRecrawlStates.Count > 0)
//    //{
//    //    foreach (CrawlerPreInitState postRecrawlState in mCrawlerHost.PostInitRecrawlStates)
//    //    {
//    //        mCrawlerHost.PreInitStates.Add(postRecrawlState);
//    //    }

//    //    mCrawlerHost.PostInitRecrawlStates.Clear();
//    //    mCrawlerHost.IninAdin(CrawlerStates.LikesFromPost);
//    //}
//    //else
//    //{
//        try
//        {
//            IsIndeterminate = true;
//            //LoadingStatus = "Ordering Pages And Posts By Likes";

//            //Application.Current.Dispatcher.Invoke((Action)delegate
//            //{
//            //    if (mCrawlerHost.PostRecrawlLikesStates.Count > 0)
//            //    {
//            //        foreach (CrawlerPreInitState postReLikesState in mCrawlerHost.PostRecrawlLikesStates)
//            //        {
//            //            if (postReLikesState.graphResult.likes == null)
//            //                postReLikesState.likesData = new LikesData();
//            //            if (postReLikesState.graphResult.likes.data == null)
//            //                postReLikesState.graphResult.likes.data = new ObservableCollection<Likes.Data>();

//            //            postReLikesState.graphResult.likes.data.Clear();
//            //            foreach (Likes.Data like in postReLikesState.likesData.likes.data)
//            //            {
//            //                postReLikesState.graphResult.likes.data.Add(like);
//            //            }
//            //        }
//            //        mCrawlerHost.PostRecrawlLikesStates.Clear();
//            //    }

//            //    try
//            //    {
//            //        List<Folder> orderdFolders = new List<Folder>();
//            //        foreach (Folder f in Folders)
//            //        {
//            //            orderdFolders.Add(orderFolderByLikes(f));
//            //        }

//            //        Folders.Clear();
//            //        foreach (Folder f in orderdFolders)
//            //        {
//            //            Folders.Add(f);
//            //        }
//            //        orderdFolders.Clear();
//            //    }
//            //    catch { } 
//            //});


//            PBarVisible = Visibility.Collapsed;
//        }
//        catch { }
//   // }
//}
//if (preinintState.option.FBGraphData != null && preinintState.option.FBGraphData.posts != null && preinintState.option.FBGraphData.posts.data != null)
//{
//    foreach (FacebookGraphPostResult post in preinintState.option.FBGraphData.posts.data)
//    {
//        if (post.likes == null) continue;
//        if (post.likes.data == null) continue;

//        if (post.likes.data.Count == 25)
//        {
//            mCrawlerHost.PostInitRecrawlStates.Add(new CrawlerPreInitState() { url = post.id, folder = null, option = null, graphResult = post, state = CrawlerStates.LikesFromPost });
//        }
//    }
//}
//if (mCrawlerHost.PreInitStates.Count > 0)
//{
//    if(mCrawlerHost.PreInitStates.Any(s=>s.state== CrawlerStates.LikesFromPost))
//    {
//        IsIndeterminate = false;

//        double total = mCrawlerHost.totalToCrawl - mCrawlerHost.PreInitStates.Count;
//        total = total / mCrawlerHost.totalToCrawl;  
//        PBarValue = total * 100;

//        LoadingStatus = LoadingStatus + Environment.NewLine + "Gathering all likes...";
//    }
//}
//try
//{
//    List<CrawlerPreInitState> theseStates = new List<CrawlerPreInitState>();

//    foreach (CrawlerPreInitState preState in mCrawlerHost.PreInitStates)
//    {
//        switch (preState.state)
//        {
//            case CrawlerStates.FbGraphCrawl:
//                if (preState.option != null)
//                {
//                    if (preState.option == option)
//                    {
//                        theseStates.Add(preState);
//                    }
//                }
//                break;
//            case CrawlerStates.LikesFromPost:
//                if (preState.graphResult != null)
//                {
//                    try
//                    {
//                        if (option.FBGraphData.posts.data.Any(p => p == preState.graphResult))
//                        {
//                            theseStates.Add(preState);
//                        }
//                    }
//                    catch { }
//                }
//                break;
//            default:
//                break;
//        }
//    }

//    foreach (CrawlerPreInitState state in theseStates)
//    {
//        mCrawlerHost.PreInitStates.Remove(state);
//    }
//}
//catch
//{
//    if (mCrawlerHost.PreInitStates.Count > 0)
//    {
//        mCrawlerHost.PreInitStates.RemoveAt(0);
//    }
//}
//class CrawlerHost : MarshalByRefObject, IHost
//{
//    public event Action<string> ReportProgress = delegate { };
//    public event Action<string, CrawlerPreInitState> OnReportGotGraphData = delegate { };//likes, url
//    public event Action<string, CrawlerPreInitState> OnReportGotLikesData = delegate { };//likes, url

//    public List<CrawlerPreInitState> PreInitStates { get; set; }
//    //public List<CrawlerPreInitState> PostInitRecrawlStates { get; set; }
//    //public List<CrawlerPreInitState> PostRecrawlLikesStates { get; set; }

//    public int HostProcessId { get { return Process.GetCurrentProcess().Id; } }

//    private PluginProcessProxy crawlerPlugin;

//    public int Initialized = 0;
//    public int totalToCrawl;

//    public CrawlerHost()
//    {
//        PreInitStates = new List<CrawlerPreInitState>();
//        //PostInitRecrawlStates = new List<CrawlerPreInitState>();
//        //PostRecrawlLikesStates = new List<CrawlerPreInitState>();

//        //crawlerPlugin = new PluginProcessProxy(new PluginStartupInfo()
//        //{
//        //    FullAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Crawler.dll"),
//        //    MainClass = "Crawler.Crawler",
//        //    Name = "BrowseoNinjaCrawler",
//        //    AssemblyName = "Crawler",
//        //}, this);

//        //RemotingServices.Marshal(this, "BrowseoNinjaCrawlerHost", typeof(CrawlerContracts.IHost));
//        //crawlerPlugin.StartPluginProcess();
//        //crawlerPlugin.LoadPlugin();    
//    }

//    internal void IninAdin(CrawlerStates crawlState)
//    {
//        totalToCrawl = PreInitStates.Count;

//        if (Initialized == 0)
//        {
//            Initialized = 1;
//            //crawlerAddin.Initialize(this);
//            crawlerPlugin.SetCrawlerState(Convert.ToInt32(crawlState));
//            //crawlerPlugin.SetPersonData(GloableProfData.PData.XmlSerializeToString());
//            crawlerPlugin.InitializeCefWithCachePath(path: Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "Caches\\" + GloableProfData.PData.ProjectName));

//        }
//        else
//        {
//            //crawlerAddin.SetCrawlerState(Convert.ToInt32(crawlState));
//            navigateToNextUrl();
//        }
//    }

//    public void navigateToNextUrl()
//    {
//        if (PreInitStates != null && PreInitStates.Count > 0)
//        {
//            new Thread(() =>
//            {
//                ReportProgress("START: " + PreInitStates[0].url);
//                //crawlerAddin.SetCrawlerState(Convert.ToInt32(PreInitStates[0].state));
//                //crawlerAddin.NavigateToUrl(PreInitStates[0].url);
//            }).Start();
//        }
//    }

//    public void ShutDown()
//    {
//        // crawlerAddin.Shutdown(); 
//    }

//    public void ReportFatalError(string userMessage, string fullExceptionText)
//    {
//        Console.WriteLine(userMessage + " " + fullExceptionText);
//    }

//    public object GetService(Type serviceType)
//    {
//        if (serviceType.IsAssignableFrom(GetType())) return this;
//        return null;
//    }

//    public void ReportInitialized()
//    {
//        if (Initialized == 1)
//        {
//            Console.WriteLine("Initialized.");
//            Initialized = 2;
//            //crawlerAddin.SetAccessToken(Social.FACEBOOK_GRAPH_LINK);
//        }
//        else
//        {
//            navigateToNextUrl();
//        }
//    }

//    public override object InitializeLifetimeService()
//    {
//        return null; // live forever
//    }

//    //public override void ReportSerializedResult(string serializedXML)
//    //{
//    //    if (PreInitStates.Count > 0)
//    //    {
//    //        OnReportGotGraphData(serializedXML, PreInitStates[0]);
//    //        navigateToNextUrl();
//    //    }
//    //}

//    //public override void ReportSerializedLikesResult(string serializedXML)
//    //{
//    //    if (PreInitStates.Count > 0)
//    //    {
//    //        OnReportGotLikesData(serializedXML, PreInitStates[0]);
//    //        navigateToNextUrl();
//    //    }
//    //}
//}

//[Serializable]
//class CrawlerPreInitState
//{
//    public CrawlerStates state = CrawlerStates.FbGraphCrawl;
//    public LikesData likesData;
//    public FacebookGraphPostResult graphResult;
//    public Folder folder;
//    public ListOption option;
//    public string url;
//}
