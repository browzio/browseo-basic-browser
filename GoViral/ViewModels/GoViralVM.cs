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
using BrowserHost;
using Gecko;
using Gecko.Interfaces;
using System.Net;
using System.Runtime.InteropServices;

namespace GoViral.ViewModels
{
    public class GoViralVM : MarshalByRefObject, INotifyPropertyChanged 
    {

        private ColaborationViewModel colaboratorTabVM;
        public ColaborationViewModel ColaboratorTabVM
        {
            get { return colaboratorTabVM; }
            set { colaboratorTabVM = value; RaisePropertyChanged("ColaboratorTabVM"); }
        }

        public event Action<string> OnSelectedTabNavigate;
        public event Action OnDominateAll;

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


        private int tCMainIndex;
        public int TCMainIndex
        {
            get { return tCMainIndex; }
            set { tCMainIndex = value; RaisePropertyChanged("TCMainIndex"); }
        }


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
            set
            {
                if(value != null)
                {
                    if (Folders.Contains(value))
                        SIFolders = Folders.IndexOf(value);
                }
            }
        }

        //private WindowsFormsHost wfh;
        //public WindowsFormsHost WebBrowserHost
        //{
        //    get
        //    {
        //        if (wfh == null)
        //        {
        //            RefreshBrowser();
        //        }
        //        return wfh;
        //    }
        //    set
        //    {
        //        wfh = value; RaisePropertyChanged("WebBrowserHost");
        //    }
        //}
        //public BrowserCntrl WebBrowser { get; set; }
        private string browserPreviewStatus;
        public string BrowserPreviewStatus
        {
            get { return browserPreviewStatus; }
            set { browserPreviewStatus = value; RaisePropertyChanged("BrowserPreviewStatus"); }
        }

        //public BaseBrowserViewModel WebBrowserControler { get; set; }
        bool canceledCrawl = false;

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

        public string AccessToken { get; set; }

        public GoViralVM()
        {
            ColaboratorTabVM = new ColaborationViewModel(this);
            ColaboratorTabVM.LoadColabiratedProjects();

            OnBtnClicked = new RelayCommand(On_OnBtnClicked);
            CTMenuClick = new RelayCommand(On_CTMenuClick);

            Folders = new ObservableCollection<Folder>();

            //PopulateListTask = Task.Factory.StartNew(() =>
            //{
            //    PopulatList();
            //}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
            new Thread(PopulatList).Start(GloableProfData.PData.ProjectName);

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
                                //SaveList();
                            }
                        }
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                    break;

                case "Delete":
                    if (SIFolders == -1 || SIFolders > Folders.Count) return;
                    try
                    {
                        if (MessageBox.Show("Are you sure you want to delete " + Folders[SIFolders].FolderTitle, "Are You Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            Folders.RemoveAt(SIFolders);

                            //Task.Factory.StartNew(() =>
                            //{
                            //    SaveList();
                            //});
                        }
                    }
                    catch { }
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

                case "DominateAll":
                    OnDominateAll?.Invoke();
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
                    //BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(Social.FACEBOOK_GRAPH_LINK);
                    // WebBrowser.Navigate(Social.FACEBOOK_GRAPH_LINK);

                    //WebBrowserControler.MainWebView.Navigated += MainWebView_Navigated_Token;
                    //WebBrowserControler.SelectedTabNavigate(Social.FACEBOOK_GRAPH_LINK);

                    OnSelectedTabNavigate?.Invoke(Social.FACEBOOK_GRAPH_LINK);
                    break;

                default:
                    break;
            }
        }


        internal void RaiseOnSelectedTabNavigate(string url)
        {
            OnSelectedTabNavigate?.Invoke(url);
        }

        //private void MainWebView_Navigated_Token(object sender, Gecko.GeckoNavigatedEventArgs e)
        //{
        //    if (WebBrowserControler.SelectedContentDocument.ReadyState == "loading") return;
        //    WebBrowserControler.MainWebView.Navigated -= MainWebView_Navigated_Token;
        //    string source = (WebBrowserControler.SelectedContentDocument.DocumentElement as Gecko.DOM.HTML.GeckoHTMLHtmlElement).OuterHtml;

        //    string accessTiken = source.Trim().Split(new string[] { "placeholder=\"Paste in an existing Access Token or click &quot;Get User Access Token" }, StringSplitOptions.None)[1];
        //    accessTiken = accessTiken.Split(new string[] { "value=" }, StringSplitOptions.None)[1];
        //    accessTiken = accessTiken.Remove(accessTiken.IndexOf(@">"));
        //    AccessToken = accessTiken.Replace("\"", "");
        //    AccessToken = AccessToken.Replace(" type=text", "");
        //}
        #endregion

        public async void Folder_OnSelectedCheckStats(Folder folder, string url)
        {
            if (AccessToken.IsNullOrEmpty())
            {
                "You must refresh the Access Token before continuing.".Show();
                return;
            }
            canceledCrawl = false;
            LoadingStatus = "Start";
            //new Thread(() =>
            //{
            //    if (!initializeCrawler())
            //    {
            //        return;
            //    }

            //    if (url != null)
            //    {
            //        addLinkForCrawlerAddInn(url, folder, folder.SavedLinksList[folder.SISavedLinks], null, CrawlerStates.FbGraphCrawl);
            //    }
            //    else
            //    {
            //        foreach (ListOption option in folder.SavedLinksList)
            //        {
            //            addLinkForCrawlerAddInn(option.Url, folder, option, null, CrawlerStates.FbGraphCrawl);
            //        }
            //    }

            //    mCrawlerHost.IninAdin();
            //}).Start(); 

            //checkOnePage
            if (url != null)
            {
                //FBConverseoOGCrawler crawler = new FBConverseoOGCrawler();
                //crawler.StartWindow();
                //crawler.ReadStats(url);

                //using (var windowMediator = Xpcom.GetService2<nsIXMLHttpRequest>(Contracts.XmlHttpRequest))
                //{

                //}

                await ReadStats(url, folder, folder.SelectedPage);
            }
            //check all pages in folder
            else
            {
                foreach (ListOption option in folder.SavedLinksList)
                {
                    LoadingStatus = " STARTING " + option.Name;
                    await ReadStats(option.Url, folder, option);
                    LoadingStatus = "End";
                    PBarVisible = Visibility.Collapsed;
                    if (canceledCrawl) return;
                    //addLinkForCrawlerAddInn(option.Url, folder, option, null, CrawlerStates.FbGraphCrawl);
                }
            }

            LoadingStatus = "End";
        }

        private async Task ReadStats(string url,Folder folder, ListOption option)
        {
            IsIndeterminate = true;
            PBarVisible = Visibility.Visible;

            if (url.Contains("/?ref=br_rs")) url = url.Replace("/?ref=br_rs", "");
            if (url.Contains("?ref=br_rs")) url = url.Replace("?ref=br_rs", "");

            string pageName = url;
            string urltillId = url;
            string fullOgUrl = "";

            pageName = getPageNameOrIdFromUrl(url);
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrWhiteSpace(pageName))
            {
                return;
            }
            if (url.Contains("/")) urltillId = url.Remove(url.LastIndexOf("/") + 1);


            if (url.StartsWith("https://www.facebook.com/pages/"))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + "?fields=" +
                                            @"about,id,link,founded,can_post,category,talking_about_count,likes,
                                        photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        videos.limit(30){permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        posts.limit(100){caption,description,picture,full_picture,shares,link,message,via,source,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)},
                                        feed.limit(70){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;
            }
            else if (url.StartsWith(Social.FACEBOOK_GROUPS_DEFAULT_URL))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + "?fields=" +
                                            @"description,name,privacy,updated_time,
                                        members.limit(0).summary(true),
                                        feed.limit(100){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;

               // return;
            }
            else if (url.StartsWith(Social.FACEBOOK_EVENTS_DEFAULT_URL))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + "?fields=" +
                                              @"description,location,privacy,start_time,ticket_uri,timezone,updated_time,
                                            interested.limit(0).summary(true),
                                            invited.limit(0).summary(true),
                                            feed.limit(100){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                            &access_token=" + AccessToken;

               // return;
            }
            else if (url.StartsWith(Social.FACEBOOK_PLACES_DEFAULT_URL))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + "?fields=" +
                                            @"about,id,name,category,can_post,description,founded,is_community_page,is_permanently_closed,is_published,is_unclaimed,is_verified,link,talking_about_count,website,likes,location,
                                        photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        albums{photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)}},
                                        videos.limit(30){permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        posts.limit(100){caption,description,picture,full_picture,shares,link,message,via,source,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)},
                                        feed.limit(70){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;
            }
            else if (url.StartsWith(Social.FACEBOOK_PHOTOS_DEFAULT_URL))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + @"?fields=
                                            created_time,link,name,source,updated_time,album,from,picture,images,likes.limit(0).summary(true),comments.limit(200).summary(true)&access_token=" + AccessToken;
            }
            else if (url.StartsWith(Social.FACEBOOK_VIDEOS_DEFAULT_URL))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + @"?fields=
                                            picture,id,embed_html,source,updated_time,description,created_time,likes.limit(0).summary(true),comments.limit(200).summary(true)&access_token=" + AccessToken;
            }

            try
            {
                nsICookieService CookieMan = Xpcom.GetService<nsICookieService>("@mozilla.org/cookieService;1");
                var cookies = Xpcom.QueryInterface<nsICookieService>(CookieMan);
                Marshal.ReleaseComObject(CookieMan);

                var uri = IOService.GetService().CreateNsIUri(fullOgUrl);
                string cookie = cookies.GetCookieString(uri, null); //i've implemented my own cookie service

                WebClient webClient = new WebClient();
                webClient.Headers.Add(HttpRequestHeader.Cookie, cookie);
                //webClient.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                //webClient.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                //webClient.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                webClient.Headers.Add(HttpRequestHeader.UserAgent, BrowserSettimgs.UserAgent_CurrentFFBuild);
                webClient.Proxy = MyFilesDatabase.GetRequestsProxy();

                await Task.Run(() =>
                {
                    try
                    {
                        string json = webClient.DownloadString(fullOgUrl);

                        var j = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookGraphDataForMedia>(json);
                        FacebookGraphData data = new FacebookGraphData()
                        {
                            about = j.about,
                            album = j.album,
                            albums = j.albums,
                            can_post = j.can_post,
                            category = j.category,
                            comments = j.comments,
                            description = j.description,
                            embed_html = j.embed_html,
                            feed = j.feed,
                            founded = j.founded,
                            id = j.id,
                            images = j.images,
                            interested = j.interested,
                            invited = j.invited,
                            is_community_page = j.is_community_page,
                            is_permanently_closed = j.is_permanently_closed,
                            is_published = j.is_published,
                            is_unclaimed = j.is_unclaimed,
                            is_verified = j.is_verified,
                            length = j.length,
                            likes = j.likes == null ? 0 : j.likes.summary == null ? 0 : j.likes.summary.total_count,
                            link = j.link,
                            members = j.members,
                            name = j.name,
                            paging = j.paging,
                            permalink_url = j.permalink_url,
                            photos = j.photos,
                            picture = j.picture,
                            posts = j.posts,
                            privacy = j.privacy,
                            source = j.source,
                            start_time = j.start_time,
                            talking_about_count = j.talking_about_count,
                            timezone = j.timezone,
                            updated_time = j.updated_time,
                            videos = j.videos,
                            views = j.views,
                            website = j.website,
                        };

                        option.FBGraphData = data;

                        //data.posts = new Posts();
                        //data.posts.data = new ObservableCollection<FacebookGraphPostResult>();

                        //option.VirtulizingFBGraphData = new FacebookGraphData();
                        //option.VirtulizingFBGraphData.posts = new Posts();
                        //option.VirtulizingFBGraphData.posts.data = new ObservableCollection<FacebookGraphPostResult>();

                        //foreach (var post in j.posts.data)
                        //{
                        //    option.VirtulizingFBGraphData.posts.data.Add(post);

                        //    if (data.posts.data.Count > 5) continue;

                        //    data.posts.data.Add(post);
                        //}

                        PBarVisible = Visibility.Collapsed;
                    }
                    catch(Exception ex)
                    {
                        var message = option.Name + " cannot be loaded due to missing permissions, or does not support this operation. Please read the Graph API documentation at https://developers.facebook.com/docs/graph-api " + ex.Message;
                        message.Show();
                    }
                });

                webClient.Dispose();
            }
            catch { }

            folder.Raise_OnFBGraphDataChanged();
            PBarVisible = Visibility.Collapsed;
        }

        internal void LoadMorePosts()
        {
            //TODO: Maybe laaaaater
        }

        private string getPageNameOrIdFromUrl(string url)
        {
            try
            {
                if (!url.Contains("https://www.facebook.com/")) return "";

                string pageName = url;
                pageName = url.Substring(url.LastIndexOf("/") + 1);
                if (url.Contains("-"))
                {
                    string id = url.Substring(url.LastIndexOf("-") + 1);
                    //long tryparseResult = 0;
                    //if (Int64.TryParse(id, out tryparseResult))
                    //{
                       pageName = id;
                    //}
                }

                return pageName;
            }
            catch
            {
                return "";
            }
        }

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
                if (linksWindow.ButtonLeftClicked)
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
            canceledCrawl = true;
            PBarVisible = Visibility.Collapsed;

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
                    if (!wasvisible)
                        PBarVisible = Visibility.Visible;
                    LoadingStatus = "Saving Do Not Close Project";

                    ColaboratorTabVM.SaveImportedCheckedProjects();
                    //string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                    //if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                    //string saveToFilePath = Path.Combine(saveToDir, "info");
                    //string allfoldersxml = Folders.XmlSerializeToString();
                    //File.WriteAllText(saveToFilePath, allfoldersxml);


                    //List<KeyValuePair<string, Folder>> projectsFolders = new List<KeyValuePair<string, Folder>>();
                    //foreach (var folder in Folders)
                    //{
                    //    projectsFolders.Add(new KeyValuePair<string, Folder>(folder.ProjectsFolderName, folder));
                    //}
                    //foreach (var projFolder in projectsFolders)
                    //{
                    //    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", projFolder.Key);
                    //    if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                    //    string saveToFilePath = Path.Combine(saveToDir, "info");
                    //    File.WriteAllText(saveToFilePath, projFolder.XmlSerializeToString());
                    //}

                    List<KeyValuePair<string, ObservableCollection<Folder>>> linkedList = new List<KeyValuePair<string, ObservableCollection<Folder>>>();
                    foreach (var folder in Folders)
                    {
                        ObservableCollection<Folder> foldersToSave = new ObservableCollection<Folder>();
                        foreach (var projectFolder in Folders)
                        {
                            if (projectFolder.ProjectsFolderName == folder.ProjectsFolderName)
                            {
                                foldersToSave.Add(projectFolder);
                            }
                        }


                        var projectsFolders = linkedList.FirstOrDefault(f => f.Key == folder.ProjectsFolderName);
                        if(projectsFolders.Key == null || projectsFolders.Value == null)
                        {
                            linkedList.Add(new KeyValuePair<string, ObservableCollection<Folder>>(folder.ProjectsFolderName, foldersToSave));
                        }
                    }
                    foreach (var item in linkedList)
                    {
                        string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", item.Key);
                        if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                        string saveToFilePath = Path.Combine(saveToDir, "info");
                        File.WriteAllText(saveToFilePath, item.Value.XmlSerializeToString());
                    }

                    //foreach (var folder in Folders)
                    //{
                    //    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", folder.ProjectsFolderName);
                    //    if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                    //    string saveToFilePath = Path.Combine(saveToDir, "info");

                    //    ObservableCollection<Folder> foldersToSave = new ObservableCollection<Folder>();
                    //    foreach (var projectFolder in Folders)
                    //    {
                    //        if(projectFolder.ProjectsFolderName == folder.ProjectsFolderName)
                    //        {
                    //            foldersToSave.Add(projectFolder);
                    //        }
                    //    }

                    //    File.WriteAllText(saveToFilePath, foldersToSave.XmlSerializeToString());
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
        
        public void PopulatList(object projectname)
        {
            lock (mLock)
            {
                try
                {
                    var projectsFolderName = projectname as string;
                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", projectsFolderName);
                    if (!Directory.Exists(saveToDir)) return;
                    var foldersFileInfo = new DirectoryInfo(saveToDir);

                    foreach (var file in foldersFileInfo.GetFiles())
                    {
                        if (file.Name == "info")
                        {

                            ObservableCollection<Folder> data = File.ReadAllText(file.FullName).XmlDeserializeFromString<ObservableCollection<Folder>>();
                            // Folder needsToBeSelected = null;
                            foreach (Folder mFolder in data)
                            {
                                SetUpFolder(mFolder, projectsFolderName);
                            }
                        }
                        //else if (file.Extension.EndsWith("info"))
                        //{
                        //    Folder mFolder = File.ReadAllText(file.FullName).XmlDeserializeFromString<Folder>();
                        //    SetUpFolder(mFolder, file.Name.Replace(".info", ""));
                        //}
                    }
                    
                    
                    if (SelectedFolder != null)
                    {
                        SelectedFolder.IsEExpanded = true;
                    }

                    TCMainIndex = 0;
                    RaisePropertyChanged("SelectedFolder");
                    //ColaboratorTabVM.OnCommandFromView_Raised("LoadCheckedIntoDominator");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void SetUpFolder(Folder mFolder, string projectsFolderName)
        {
            var isFolderExisting = Folders.FirstOrDefault(f => f.FolderTitle == mFolder.FolderTitle);
            Folders.Remove(isFolderExisting);

            mFolder.ProjectsFolderName = projectsFolderName;
            setFolderEvents(mFolder);
            mFolder.CTMenuClick = new RelayCommand(mFolder.On_CTMenuClick);
            if (mFolder.SavedLinksList != null)
            {
                foreach (ListOption lo in mFolder.SavedLinksList)
                {
                    lo.OnFBGraphDataChanged += mFolder.Raise_OnFBGraphDataChanged;
                    //if (lo.IsSelected)
                    //{
                    //    needsToBeSelected = folder;
                    //}
                }
            }
            mFolder.SISavedLinks = 0;
            Application.Current.Dispatcher.Invoke(delegate { Folders.Add(mFolder); });
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

                //new Thread(SaveList).Start();
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

                //Task.Factory.StartNew(SaveList);
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

            //Task.Factory.StartNew(() =>
            //{
            //    SaveList();
            //});
        }
        #endregion

        #region browser load and events
        void Folder_OnLoadInBrowser(string url)
        {
            //WebBrowser.Navigate(url);

            //BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(url);

            // WebBrowserControler.SelectedTabNavigate(url);

            OnSelectedTabNavigate?.Invoke(url);
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
            //if (WebBrowser != null)
            //{
            //    try
            //    {
            //        if (WebBrowser.CBrowser != null && WebBrowser.GetBrowser() != null && WebBrowser.GetBrowser().GetMainFrame() != null)
            //        {
            //            url = WebBrowser.GetBrowser().GetMainFrame().Url;
            //        }
            //    }
            //    catch { }
            //    WebBrowser.DisposeBrowserComponents();
            //}

            //if(wfh != null)
            //{
            //    wfh.Child.Dispose();
            //}

            //WebBrowser = new Xilium.CefGlue.Client.BrowserCntrl();
            //WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
            //WebBrowser.OnBrowserContextMenuClicked += WebBrowser_OnBrowserContextMenuClicked;
            //WebBrowser.OnBrowserStatusChanged += WebBrowser_OnBrowserStatusChanged;
            //WebBrowser.init(url, BrowserSettimgs.FlashEnabled, BrowserSettimgs.JavascriptEnabled, BrowserSettimgs.JavaEnabled);
            //if (wfh == null)
            //    wfh = new WindowsFormsHost();

            //wfh.Child = WebBrowser;
            RaisePropertyChanged("WebBrowserHost");

            //WebBrowser.Reload();
        }

        //private void WebBrowser_OnBrowserStatusChanged(string oMessage)
        //{
        //    if (oMessage == null) return;
        //    HuverLink = oMessage;
        //}

        //#region contextmenue
        //public string HuverLink { get; set; }
        //public event Action<string,string> OnSentForSeo = delegate { };
        //public event Action<string,string> OnCurateToPBN = delegate { };
        //public event Action<string> OnCreateNewTab = delegate { };
        //private void WebBrowser_OnBrowserContextMenuClicked(int contextMenueItemID)
        //{
        //    switch (contextMenueItemID)
        //    {
        //        case 333:
        //            if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
        //            {
        //                string sitename = WebBrowser.CurrAddress.Replace("http://", "");
        //                sitename = sitename.Replace("https://", "");
        //                sitename = sitename.Replace("www.", "");
        //                if (sitename.Contains("."))
        //                {
        //                    sitename = sitename.Remove(sitename.IndexOf("."));
        //                }
        //                OnSentForSeo(sitename, HuverLink);
        //            }
        //            //WebBrowser.CBrowser.Browser.GetHost().SendFocusEvent
        //            // WebBrowser.CBrowser.Browser.GetHost().ShowDevTools(CefWindowInfo.Create(), new DemoClient(), new CefBrowserSettings() { }, new CefPoint(110,110));
        //            break;

        //        case 111:
        //            try
        //            {
        //                var host = WebBrowser.GetBrowser().GetHost();
        //                var wi = CefWindowInfo.Create();
        //                wi.SetAsPopup(IntPtr.Zero, "DevTools");
        //                host.ShowDevTools(wi, new DevToolsWebClient(), new CefBrowserSettings(), new CefPoint(0, 0));
        //            }
        //            catch { }
        //            break;

        //        #region curate
        //        case 666:
        //        case 222:
        //            try
        //            {
        //                if (WebBrowser.GetTheMainFrame() == null || WebBrowser.GetTheMainFrame().Url == null) return;

        //                string dir = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempHTML");
        //                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        //                string file = Path.Combine(dir, "html.txt");
        //                if (File.Exists(file)) File.Delete(file);

        //                //the javascript
        //                string jsForExecution = "var range = window.getSelection().getRangeAt(0)," +
        //                                        "content = range.extractContents()," +
        //                                        "span = document.createElement('SPAN');" +
        //                                        "span.appendChild(content);" +
        //                                        "var htmltext = span.innerHTML.toString();" +
        //                                        "range.insertNode(span);" +
        //                                        "nativeImplementation(htmltext);";
        //                WebBrowser.GetTheMainFrame().ExecuteJavaScript(jsForExecution, WebBrowser.GetTheMainFrame().Url, 0);



        //                System.Threading.Tasks.Task.Factory.StartNew(() =>
        //                {
        //                    while (!File.Exists(file))
        //                    {
        //                        System.Threading.Thread.Sleep(150);
        //                    }

        //                    if (contextMenueItemID == 666)
        //                    {
        //                        OnCurateToPBN(File.ReadAllText(file), WebBrowser.CurrAddress);
        //                    }
        //                    else
        //                    {
        //                        string thecontent = "<blockquote>" + File.ReadAllText(file) + "<br />";
        //                        if (!string.IsNullOrEmpty(WebBrowser.CurrAddress) && !string.IsNullOrWhiteSpace(WebBrowser.CurrAddress))
        //                            thecontent += "<a href=\"" + WebBrowser.CurrAddress + " \" > " + WebBrowser.CurrAddress + " </a>";
        //                        thecontent += "</blockquote>";
        //                        Application.Current.Dispatcher.Invoke(delegate
        //                        {
        //                            MyFilesDatabase.SetClipboardText(thecontent);
        //                        });
        //                    }
        //                    File.Delete(file);
        //                });

        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //            break;
        //        #endregion

        //        #region newTab
        //        case 999:
        //            if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
        //            {
        //                OnCreateNewTab(HuverLink);
        //            }
        //            break;
        //        #endregion

        //        #region copy link
        //        case 888:
        //            if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
        //            {
        //                MyFilesDatabase.SetClipboardText(HuverLink);
        //            }
        //            break;
        //        #endregion

        //        #region imageDownload
        //        case 777:
        //            System.Threading.Tasks.Task.Factory.StartNew(() =>
        //            {
        //                string imgUrl = "";
        //                if (WebBrowser.GetTheMainFrame() != null && WebBrowser.GetTheMainFrame().Url != null)
        //                {
        //                    string url = WebBrowser.GetTheMainFrame().Url;

        //                    imgUrl = ChromeBrowserHostControl.GetImageUrl(url);
        //                }

        //                if (imgUrl == "")
        //                {
        //                    if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
        //                    {
        //                        string url = HuverLink;
        //                        if (url.ToLower().Contains("imgurl=") && url.ToLower().Contains("google."))
        //                        {
        //                            url = url.Split(new string[] { "imgurl=" }, StringSplitOptions.None)[1];
        //                            if (url.Contains("%253"))
        //                            {
        //                                url = url.Remove(url.IndexOf("%253"));
        //                            }
        //                            if (url.Contains("&imgrefurl"))
        //                            {
        //                                url = url.Remove(url.IndexOf("&imgrefurl"));
        //                            }
        //                        }

        //                        if (url.Contains("%3A"))
        //                        {
        //                            url = url.Replace("%3A", ":");
        //                        }
        //                        if (url.Contains("%2F"))
        //                        {
        //                            url = url.Replace("%2F", "/");
        //                        }
        //                        if (url.Contains("%2520"))
        //                        {
        //                            url = url.Replace("%2520", " ");
        //                        }
        //                        if (url.Contains("%20"))
        //                        {
        //                            url = url.Replace("%20", " ");
        //                        }
        //                        imgUrl = ChromeBrowserHostControl.GetImageUrl(url);
        //                    }
        //                }

        //                if (imgUrl == "")
        //                {
        //                    MessageBox.Show("No image found to download. Make sure the mouse is over a image and try again, or open the image as a tab and then download it.");
        //                    return;
        //                }

        //                MyFilesDatabase.DownloadImage(imgUrl);
        //            });
        //            break;
        //        #endregion

        //        //#region go viral
        //        //case 555:
        //        //    if ((string.IsNullOrEmpty(HuverLink) && string.IsNullOrWhiteSpace(HuverLink)) ||
        //        //        (string.IsNullOrEmpty(AddressEditable) && string.IsNullOrWhiteSpace(AddressEditable)))
        //        //    {
        //        //        MessageBox.Show("Cant complete action make sure the mouse pointer is hovering over the link you want.");
        //        //        return;
        //        //    }
        //        //    string linkToGet = HuverLink;
        //        //    string link = HuverLink;
        //        //    WebBrowser.GetTheMainFrame().GetSource(new SourceVisitor(htmlSource =>
        //        //    {
        //        //        try
        //        //        {
        //        //            string splitter = getsplitter();
        //        //            if (AddressEditable.Contains("facebook.com/groups/?category=membership"))
        //        //            {
        //        //                string fromsource = linkToGet.Replace(Social.FACEBOOK_GROUPS_DEFAULT_URL, "/groups/");
        //        //                fromsource = htmlSource.Substring(htmlSource.IndexOf(fromsource));
        //        //                string name = fromsource.Substring(fromsource.IndexOf(">") + 1);
        //        //                name = name.Remove(name.IndexOf("<"));

        //        //                string id = fromsource.Substring(fromsource.IndexOf("id="));
        //        //                id = id.Replace("id=", "");
        //        //                id = id.Remove(id.IndexOf("\""));

        //        //                link = Social.FACEBOOK_GROUPS_DEFAULT_URL + name + "-" + id;
        //        //            }
        //        //            else
        //        //            {
        //        //                linkToGet = linkToGet.Replace(Social.FACEBOOK_GROUPS_DEFAULT_URL, "/groups/");
        //        //                linkToGet = linkToGet.Replace(Social.FACEBOOK_EVENTS_DEFAULT_URL, "/events/");
        //        //                linkToGet = linkToGet.Replace("?ref=br_rs&action_history=null", "?ref=br_rs&amp;action_history=null");
        //        //                link = getLinkFromUrlAndSource(linkToGet, htmlSource, splitter);
        //        //            }

        //        //            Application.Current.Dispatcher.Invoke(delegate
        //        //            {
        //        //                RaiseOnAddedToGoViral(link, "", null);
        //        //            });
        //        //        }
        //        //        catch (Exception ex)
        //        //        {
        //        //            MessageBox.Show("Couldnt pull data.");
        //        //        }
        //        //    }));
        //        //    break;

        //        //case 444:
        //        //    SourceVisitor visitor = new SourceVisitor(htmlSource =>
        //        //    {
        //        //        try
        //        //        {
        //        //            List<string> linksToReturn = new List<string>();
        //        //            if (AddressEditable.Contains("facebook.com/groups/?category=membership"))
        //        //            {
        //        //                List<string> links = htmlSource.Split(new string[] { "group_browse_new" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        //                links.RemoveAt(0);
        //        //                foreach (var linkl in links)
        //        //                {
        //        //                    string name = "", id = "";
        //        //                    try
        //        //                    {
        //        //                        name = linkl.Substring(linkl.IndexOf(">") + 1);
        //        //                        name = name.Remove(name.IndexOf("<"));

        //        //                        id = linkl.Substring(linkl.IndexOf("id=") + 3);
        //        //                        id = id.Remove(id.IndexOf("\""));

        //        //                        Convert.ToInt64(id);
        //        //                    }
        //        //                    catch
        //        //                    { continue; }

        //        //                    linksToReturn.Add(Social.FACEBOOK_GROUPS_DEFAULT_URL + name + "-" + id);
        //        //                }

        //        //            }
        //        //            else
        //        //            {
        //        //                string splitter = getsplitter();

        //        //                List<string> links = htmlSource.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        //                links.RemoveAt(0);


        //        //                foreach (string linkl in links)
        //        //                {
        //        //                    string linkToGetl = linkl.Remove(linkl.IndexOf("\""));
        //        //                    string linkToAdd = getLinkFromUrlAndSource(linkToGetl, htmlSource, splitter);
        //        //                    linksToReturn.Add(linkToAdd);
        //        //                }
        //        //            }

        //        //            Application.Current.Dispatcher.Invoke((Action)delegate
        //        //            {
        //        //                RaiseOnAddedToGoViral(null, "", linksToReturn);
        //        //            });
        //        //        }
        //        //        catch
        //        //        {
        //        //            MessageBox.Show("Couldnt pull pages.");
        //        //        }
        //        //    });
        //        //    WebBrowser.GetTheMainFrame().GetSource(visitor);
        //        //    break;
        //        //#endregion

        //        default:
        //            break;
        //    }
        //}
        ////private class DevToolsWebClient : CefClient
        ////{
        ////}
        //#endregion

        public void DisposeBrowser()
        {
            //if(WebBrowser!=null)
            //    WebBrowser.DisposeBrowserComponents();

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
