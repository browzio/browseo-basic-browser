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
using HostView;
using System.Security.Permissions;

namespace GoViral.ViewModels
{
    public class GoViralVM : INotifyPropertyChanged
    {
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
                if (sIFolders != value)
                {

                    sIFolders = value;
                    RaisePropertyChanged("SIFolders");
                }
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

                                           
        private CrawlerHost mCrawlerHost;

        private int lastSelectedIndex = -1;
        private Task PopulateListTask;
        private Task InitAddinTask;
        private object mLock = new object();

        public GoViralVM()
        {
            OnBtnClicked = new RelayCommand(On_OnBtnClicked);
            CTMenuClick = new RelayCommand(On_CTMenuClick);

            Folders = new ObservableCollection<Folder>();

            PopulateListTask = Task.Factory.StartNew(() =>
            {
                PopulatList();
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            PBarVisible = Visibility.Collapsed;
            IsIndeterminate = true;
        }

        #region command raised methods
        private void On_CTMenuClick(object param)
        {
            if (Folders.Count == 0) return;
            switch ((string)param)
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

                case "ORDER_Likes":
                    orderFolderByLikes(Folders[SIFolders]);
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

                case "SAVE":
                    Task.Factory.StartNew(()=> { SaveList(); });
                    break;

                default:
                    break;
            }
        }
        #endregion

        private Folder orderFolderByLikes(Folder folderToOrder)
        {
            List<ListOption> orderd = folderToOrder.SavedLinksList.OrderByDescending(s => s.FBGraphData.likes).ToList();
            folderToOrder.SavedLinksList.Clear();
            foreach (ListOption o in orderd)
            {
                folderToOrder.SavedLinksList.Add(o);
                if (o.FBGraphData == null || o.FBGraphData.posts == null || o.FBGraphData.posts.data == null) continue;

                List<FacebookGraphPostResult> res = o.FBGraphData.posts.data.OrderByDescending(p => p == null ? 0 : p.likes == null ? 0 : p.likes.data == null ? 0 : p.likes.data.Count).ToList();
                o.FBGraphData.posts.data.Clear();
                foreach (FacebookGraphPostResult p in res)
                {
                    folderToOrder.SavedLinksList[folderToOrder.SavedLinksList.IndexOf(o)].FBGraphData.posts.data.Add(p);
                }
            }

            return folderToOrder;
        }

        internal void BeginImageDownload(string full_picture)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                PBarVisible = Visibility.Visible;
                LoadingStatus = "Image Download Began";

                MyFilesDatabase.DownloadImage(full_picture);

                LoadingStatus = "Done";
                PBarVisible = Visibility.Collapsed;
            });
        }

        #region crawler

        void Folder_OnSelectedCheckStats(Folder folder, string url)
        {
            if (InitAddinTask == null || InitAddinTask.IsCompleted)
            {
                InitAddinTask = Task.Factory.StartNew(() =>
                {
                    if (!initializeCrawler())
                    {
                        return;
                    }

                    if (url != null)
                    {
                        addLinkForCrawlerAddInn(url, folder, folder.SavedLinksList[folder.SISavedLinks],null, CrawlerStates.FbGraphCrawl);
                    }
                    else
                    {
                        foreach (ListOption option in folder.SavedLinksList)
                        {
                            addLinkForCrawlerAddInn(option.Url, folder, option,null, CrawlerStates.FbGraphCrawl);
                        }
                    }

                    mCrawlerHost.IninAdin(CrawlerStates.FbGraphCrawl);
                });
            }
        }

        internal void LoadAllLikes(FacebookGraphPostResult facebookGraphPostResult, string url)
        {
            if (InitAddinTask == null || InitAddinTask.IsCompleted)
            {
                InitAddinTask = Task.Factory.StartNew(() =>
                {
                    if (!initializeCrawler())
                    {
                        return;
                    }

                    addLinkForCrawlerAddInn(url, null, null, facebookGraphPostResult, CrawlerStates.LikesFromPost);
                    mCrawlerHost.IninAdin(CrawlerStates.LikesFromPost);
                });
            }
        }

        private void addLinkForCrawlerAddInn(string url, Folder folder, ListOption option, FacebookGraphPostResult facebookGraphPostResult, CrawlerStates state)
        {
            CrawlerPreInitState crawlSearchState = new CrawlerPreInitState() { url = url, folder = folder, option = option, graphResult = facebookGraphPostResult , state = state};
            mCrawlerHost.PreInitStates.Add(crawlSearchState);
        }

        private bool initializeCrawler()
        {
            if (LoadingStatus != null && LoadingStatus.Contains("Initializing Crawler..."))
                return false;

            PBarVisible = Visibility.Visible;
            LoadingStatus = "Initializing Crawler..."; 
            if (mCrawlerHost == null)
            {   
                string path = AppDomain.CurrentDomain.BaseDirectory;
                Console.WriteLine(path);
                if (path == @"C:\Users\eli\Desktop\move\xilium-xilium.cefglue-335450e6011d\BrowserAndFeatures\bin\x86\Debug\" ||
                    path == @"C:\Users\eli\Desktop\move\plugins\WpfHost\bin\Release" ||
                    path == @"C:\Users\eli\Desktop\move\plugins\WpfHost\bin\Debug" ||
                    path == @"C:\Users\eli\Desktop\move\All Browseo Install Files")
                {
                    string[] ss = AddInStore.Update(path);// (epath);
                    string[] kk = AddInStore.RebuildAddIns(path);
                }

                try
                {
                    IList<AddInToken> tokens = AddInStore.FindAddIns(typeof(HostView.ProcessorHostView), path);
                    AddInToken crawlerToken = tokens.SingleOrDefault(t => t.AddInFullName == "Crawler.Crawler");

                    if (crawlerToken != null)
                    {
                        ProcessorHostView crawlerAddin = crawlerToken.Activate<HostView.ProcessorHostView>(AddInSecurityLevel.FullTrust);

                        mCrawlerHost = new CrawlerHost(crawlerAddin);
                        mCrawlerHost.ReportProgress += CrawlerHost_ReportProgress;
                        mCrawlerHost.OnReportGotGraphData += CrawlerHost_OnReportGotGraphData;
                        mCrawlerHost.OnReportGotLikesData += CrawlerHost_OnReportGotLikesData;
                    }
                    else
                    {
                        MessageBox.Show("Could not start crawl process, could not find process tokens.");
                        PBarVisible = Visibility.Collapsed;
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("Could not start crawl process, could not find process tokens.");
                    PBarVisible = Visibility.Collapsed;
                    return false;
                }
            }


            return true;
        }

        private void CrawlerHost_OnReportGotLikesData(string serializedXMLResult, CrawlerPreInitState preinintState)
        {
            if (serializedXMLResult != "N/A")
            {
                LikesData likes = serializedXMLResult.XmlDeserializeFromString<LikesData>();

                if (preinintState.graphResult != null && likes != null && likes.likes != null)
                {
                    preinintState.likesData = likes;
                    mCrawlerHost.PostRecrawlLikesStates.Add(preinintState);
                }
            }

            removePreInitState(preinintState);

            
        }

        private void removePreInitState(CrawlerPreInitState preinintState)
        {
            mCrawlerHost.PreInitStates.Remove(preinintState);

            LoadingStatus += "END: " + preinintState.url;

            if (mCrawlerHost.PreInitStates.Count == 0)
            {
                if (mCrawlerHost.PostInitRecrawlStates.Count > 0)
                {
                    foreach (CrawlerPreInitState postRecrawlState in mCrawlerHost.PostInitRecrawlStates)
                    {
                        mCrawlerHost.PreInitStates.Add(postRecrawlState);
                    }

                    mCrawlerHost.PostInitRecrawlStates.Clear();
                    mCrawlerHost.IninAdin(CrawlerStates.LikesFromPost);
                }
                else
                {
                    try
                    {
                        IsIndeterminate = true;
                        LoadingStatus = "Ordering Pages And Posts By Likes";

                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            if (mCrawlerHost.PostRecrawlLikesStates.Count > 0)
                            {
                                foreach (CrawlerPreInitState postReLikesState in mCrawlerHost.PostRecrawlLikesStates)
                                {
                                    if (postReLikesState.graphResult.likes == null)
                                        postReLikesState.likesData = new LikesData();
                                    if (postReLikesState.graphResult.likes.data == null)
                                        postReLikesState.graphResult.likes.data = new ObservableCollection<Likes.Data>();

                                    postReLikesState.graphResult.likes.data.Clear();
                                    foreach (Likes.Data like in postReLikesState.likesData.likes.data)
                                    {
                                        postReLikesState.graphResult.likes.data.Add(like);
                                    }
                                }
                                mCrawlerHost.PostRecrawlLikesStates.Clear();
                            }

                            try
                            {
                                List<Folder> orderdFolders = new List<Folder>();
                                foreach (Folder f in Folders)
                                {
                                    orderdFolders.Add(orderFolderByLikes(f));
                                }

                                Folders.Clear();
                                foreach (Folder f in orderdFolders)
                                {
                                    Folders.Add(f);
                                }
                                orderdFolders.Clear();
                            }
                            catch { } 
                        });
                        

                        PBarVisible = Visibility.Collapsed;
                    }
                    catch { }
                }
            }
        }

        private void CrawlerHost_ReportProgress(string status)
        {
            if (PBarVisible == Visibility.Collapsed) pBarVisible = Visibility.Visible;

            LoadingStatus = status;
            if (mCrawlerHost.PreInitStates.Count > 0)
            {
                if(mCrawlerHost.PreInitStates.Any(s=>s.state== CrawlerStates.LikesFromPost))
                {
                    IsIndeterminate = false;

                    double total = mCrawlerHost.totalToCrawl - mCrawlerHost.PreInitStates.Count;
                    total = total / mCrawlerHost.totalToCrawl;  
                    PBarValue = total * 100;

                    LoadingStatus = LoadingStatus + Environment.NewLine + "Gathering all likes...";
                }
            }
        }

        private void CrawlerHost_OnReportGotGraphData(string serializedXMLResult, CrawlerPreInitState preinintState)
        {
            if (serializedXMLResult != "N/A")
            {
                FacebookGraphData fbgData = serializedXMLResult.XmlDeserializeFromString<FacebookGraphData>();
                preinintState.option.FBGraphData = fbgData; 
            }
            else
            {
                MessageBox.Show("Couldnt crawl page " + preinintState.url);
                removePreInitState(preinintState);
                return;
            }
            if (preinintState.option.FBGraphData != null && preinintState.option.FBGraphData.posts != null && preinintState.option.FBGraphData.posts.data != null)
            {
                foreach (FacebookGraphPostResult post in preinintState.option.FBGraphData.posts.data)
                {
                    if (post.likes == null) continue;
                    if (post.likes.data == null) continue;

                    if (post.likes.data.Count == 25)
                    {
                        mCrawlerHost.PostInitRecrawlStates.Add(new CrawlerPreInitState() {url = post.id, folder = null, option = null, graphResult = post, state = CrawlerStates.LikesFromPost });
                    }
                }
            }
            removePreInitState(preinintState);
        }

        private void Folder_OnCanceledAStatsCheck(ListOption option)
        {
            if (mCrawlerHost != null)
            {

                try
                {
                    List<CrawlerPreInitState> theseStates = new List<CrawlerPreInitState>();

                    foreach (CrawlerPreInitState preState in mCrawlerHost.PreInitStates)
                    {
                        switch (preState.state)
                        {
                            case CrawlerStates.FbGraphCrawl:
                                if (preState.option != null)
                                {
                                    if (preState.option == option)
                                    {
                                        theseStates.Add(preState);
                                    }
                                }
                                break;
                            case CrawlerStates.LikesFromPost:
                                if (preState.graphResult != null)
                                {
                                    try
                                    {
                                        if (option.FBGraphData.posts.data.Any(p => p == preState.graphResult))
                                        {
                                            theseStates.Add(preState);
                                        }
                                    }
                                    catch { }
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    foreach (CrawlerPreInitState state in theseStates)
                    {
                        mCrawlerHost.PreInitStates.Remove(state);
                    }
                }
                catch
                {
                    if (mCrawlerHost.PreInitStates.Count > 0)
                    {
                        mCrawlerHost.PreInitStates.RemoveAt(0);
                    }
                }

                mCrawlerHost.navigateToNextUrl();
                if (mCrawlerHost.PreInitStates.Count == 0)
                    PBarVisible = Visibility.Collapsed;
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
                    PBarVisible = Visibility.Visible;
                    LoadingStatus = "Saving Do Not Close Project";

                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                    string saveToFilePath = Path.Combine(saveToDir, "info");
                    //if (File.Exists(saveToFilePath)) File.Delete(saveToFilePath);

                    File.WriteAllText(saveToFilePath,Folders.XmlSerializeToString());

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
            try
            {
                string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(saveToDir)) return;

                string saveToFilePath = Path.Combine(saveToDir, "info");
                if (!File.Exists(saveToFilePath)) return;

                
                ObservableCollection<Folder> data = File.ReadAllText(saveToFilePath).XmlDeserializeFromString<ObservableCollection<Folder>>();
                foreach (Folder folder in data)
                {
                    setFolderEvents(folder);
                    folder.CTMenuClick = new RelayCommand(folder.On_CTMenuClick);
                    Folders.Add(folder);
                }

                //using (FileStream inStream = File.OpenRead(saveToFilePath))
                //{
                //    DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<Folder>));
                //    ObservableCollection<Folder> data = (ObservableCollection<Folder>)serializer.ReadObject(inStream);
                //    foreach (Folder folder in data)
                //    {
                //        setFolderEvents(folder);
                //        folder.CTMenuClick = new RelayCommand(folder.On_CTMenuClick);
                //        Folders.Add(folder);
                //    }
                //}
            }
            catch (Exception ex)
            {
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
            SIFolders = Folders.IndexOf(folder);
        }

        public async void AsyncAddLinkToList(string link)
        {
            if (!PopulateListTask.IsCompleted)
            {
                await PopulateListTask;
            }

            string name = "";
            try
            {
                name = link.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[2];
            }
            catch { }

            SIFolders = lastSelectedIndex;
            if (SIFolders == -1) SIFolders = 0;

            if (Folders.Count == 0)
            {

                MessageBox.Show("You need to create a folder before pushing links to it.");
                addNewFolder();
                return;
            }

            ToFolderChooserWindow fcw = new ToFolderChooserWindow() { DataContext = this };
            fcw.tbName.Text = name;
            fcw.tbUrl.Text = link;
            if (fcw.ShowDialog() == true)
            {
                lastSelectedIndex = SIFolders;

                Folders[SIFolders].SavedLinksList.Add(new ListOption() { Name = fcw.tbName.Text, Url = fcw.tbUrl.Text });

                Task.Factory.StartNew(() =>
                {
                    SaveList();
                });
            }

        }

        private void addNewFolder()
        {
            Task.Factory.StartNew(() =>
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
                        return;
                    }

                    Folder folder = new Folder() { FolderTitle = setFolderNAmeWindow.tbInputText.Text };
                    setFolderEvents(folder);
                    Folders.Add(folder);
                    SaveList();
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
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

                Folders[SIFolders].SavedLinksList.Add(new ListOption() { Name = fcw.tbName.Text, Url = fcw.tbUrl.Text });
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
#endregion

        public void DisposeBrowser()
        {
            if(WebBrowser!=null)
                WebBrowser.Dispose();

            if (mCrawlerHost != null)
                mCrawlerHost.ShutDown();
        }



#region propchanged
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
#endregion
    }

    class CrawlerHost : HostView.HostObject
    { 
        public event Action<string> ReportProgress = delegate { };  
        public event Action<string, CrawlerPreInitState> OnReportGotGraphData = delegate { };//likes, url
        public event Action<string, CrawlerPreInitState> OnReportGotLikesData = delegate { };//likes, url

        public List<CrawlerPreInitState> PreInitStates { get; set; }
        public List<CrawlerPreInitState> PostInitRecrawlStates { get; set; }
        public List<CrawlerPreInitState> PostRecrawlLikesStates { get; set; }

        private ProcessorHostView crawlerAddin;

        public int Initialized = 0;
        public int totalToCrawl;

        public CrawlerHost(ProcessorHostView crawlerAddin)
        {
            PreInitStates = new List<CrawlerPreInitState>();
            PostInitRecrawlStates = new List<CrawlerPreInitState>();
            PostRecrawlLikesStates = new List<CrawlerPreInitState>();

            this.crawlerAddin = crawlerAddin;
        }

        internal void IninAdin(CrawlerStates crawlState)
        {
            totalToCrawl = PreInitStates.Count;

            if (Initialized == 0)
            {
                Initialized = 1;
                crawlerAddin.Initialize(this);
                crawlerAddin.SetCrawlerState(Convert.ToInt32(crawlState));
                crawlerAddin.SetPersonData(GloableProfData.PData.XmlSerializeToString());
                crawlerAddin.InitializeCefWithCachePath(path: Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "Caches\\" + GloableProfData.PData.ProjectName));
                
            }
            else
            {
                crawlerAddin.SetCrawlerState(Convert.ToInt32(crawlState));
                navigateToNextUrl();
            }
        }

#region crawler callbacks
        public override void ReportInitialized()
        {
            if (Initialized == 1)
            {
                Initialized = 2;
                crawlerAddin.SetAccessToken(Social.FACEBOOK_GRAPH_LINK);
            }
            else
            {
                navigateToNextUrl();
            }
        }

        public override void ReportSerializedResult(string serializedXML)
        {
            if (PreInitStates.Count > 0)
            {
                OnReportGotGraphData(serializedXML, PreInitStates[0]);
                navigateToNextUrl();
            }
        }
#endregion

        public void navigateToNextUrl()
        {
            if (PreInitStates != null && PreInitStates.Count > 0)
            {
                ReportProgress("START: " + PreInitStates[0].url);
                crawlerAddin.SetCrawlerState(Convert.ToInt32(PreInitStates[0].state));
                crawlerAddin.NavigateToUrl(PreInitStates[0].url);
            }
        }

        public void ShutDown()
        {
            crawlerAddin.Shutdown(); 
        }

        public override void ReportSerializedLikesResult(string serializedXML)
        {
            if (PreInitStates.Count > 0)
            {
                OnReportGotLikesData(serializedXML, PreInitStates[0]);
                navigateToNextUrl();
            }
        }
    }

    class CrawlerPreInitState
    {
        public CrawlerStates state = CrawlerStates.FbGraphCrawl;
        public LikesData likesData;
        public FacebookGraphPostResult graphResult;
        public Folder folder;
        public ListOption option;
        public string url;
    }
}
