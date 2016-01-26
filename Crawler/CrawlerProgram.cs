using CrawlerContracts;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using Organiser.Common.Classes.Crawler;
using Organiser.Common.Classes.Facebook;
using SocialOrganizer.Models;
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;

namespace Crawler
{
    //public class CrawlerEntry
    //{
    //    public static void Main(string[] args)
    //    {
    //        //Crawler crawling = new Crawler();
    //        //crawling.start();

    //        Crawler crawling = new Crawler();
    //        crawling.InitializeCefWithCachePath(@"C:\Users\eli\AppData\Local\RAWSocialOrganizer\Caches\testing 101");
    //        crawling.SetCrawlerState(Convert.ToInt32(CrawlerStates.FbGraphCrawl));
    //        Console.ReadLine();
    //        crawling.Shutdown();
    //    }
    //}
    //[SecurityPermission(SecurityAction.Demand, Flags =SecurityPermissionFlag.AllFlags)]
    // [AddIn("Crawler", Version = "1.0.0.0", Publisher = "Browseo", Description = "Ninja Crawler")]
    public class Crawler : MarshalByRefObject, IPlugin
    {
        public event Action OnReportInitialized = delegate { };
        public event Action<string> OnReportSerializedResult = delegate { };

        private DemoCefClient cefClient;
        private CefBrowser browser;

        private CrawlerStates crawlerState;
        private CrawlerStates pageType = CrawlerStates.PageType_Pages;//used within 

        private List<PhotosGraphData> allCrawledPhotos = new List<PhotosGraphData>();
        private List<VideosGraphData> allCrawledVideos = new List<VideosGraphData>();
        private List<string> allMediaLinkToCrawl = new List<string>();

        private string AccessToken = "", preRegetTokenUrl = "", preRegetAccessToken = "";
        private object mlock = new object();
        private object mPhotStatslock = new object();

        public Crawler()
        {
            //using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            //{
            //SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX | ErrorModes.SEM_NOOPENFILEERRORBOX);
            //Debugger.Break();
            //}      
            //Debugger.Launch();     

            //AccessToken = "";
            //AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //Create the loader (a proxy).
            //var assemblyLoader = (SimpleAssemblyLoader)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(typeof(SimpleAssemblyLoader).Assembly.FullName, 
            //  typeof(SimpleAssemblyLoader).FullName);
            //Load an assembly in the LoadFrom context. Note that the Load context will
            //not work unless you correctly set the AppDomain base-dir and private-bin-paths.
            // assemblyLoader.LoadFrom(AppDomain.CurrentDomain.BaseDirectory.Replace(@"AddIns\CrawlerAddIn", "Browseo.BrowserAssemby.dll"));
            //   assemblyLoader.LoadFrom(AppDomain.CurrentDomain.BaseDirectory.Replace(@"AddIns\CrawlerAddIn", "Browseo.WindowsForms.dll"));
            // assemblyLoader.LoadFrom(AppDomain.CurrentDomain.BaseDirectory.Replace(@"AddIns\CrawlerAddIn", "Organiser.Common.dll"));
        }

        public void SetCrawlerState(int state)
        {
            crawlerState = (CrawlerStates)state;
        }

        public void SetPersonData(string serializedPdata)
        {
            GloableProfData.PData = serializedPdata.XmlDeserializeFromString<PersonData>();//.XmlDeserializeFromString(typeof(PersonData)) as PersonData;
        }

        public void SetAccessToken(string url)
        {
            browser.GetMainFrame().LoadUrl(url);
        }

        public void InitializeCefWithCachePath(string path)
        {
            //Thread init = new Thread(() =>
            //{

            //ConsoleManager.Show();
            Console.WriteLine(path);
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(exePath);

            // Load CEF. This checks for the correct CEF version.
            CefRuntime.Load();

            // Start the secondary CEF process.
            var cefMainArgs = new CefMainArgs(new string[0]);
            var cefApp = new DemoCefApp();

            // This is where the code path divereges for child processes.
            //Console.WriteLine(CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero));
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero) != -1)
            {
                throw new InvalidOperationException("Runtime could not the secondary process.");
            }

            //Environment.Exit(1);

            // Settings for all of CEF (e.g. process management and control).
            // var subProcessPath = AppDomain.CurrentDomain.BaseDirectory + "\\CrawlerProcess.exe";
            // var subProcessPath = AppDomain.CurrentDomain.BaseDirectory + "\\Crawler.exe";
            var subProcessPath = AppDomain.CurrentDomain.BaseDirectory + "\\BrowserAndFeatures.exe";
            subProcessPath = subProcessPath.Replace("\\\\", "\\");
            Console.WriteLine(subProcessPath);
            var cefSettings = new CefSettings
            {
                BrowserSubprocessPath = subProcessPath,
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                PersistSessionCookies = true,
                LogSeverity = CefLogSeverity.Disable,
                IgnoreCertificateErrors = true,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36",
                CachePath = path,
                WindowlessRenderingEnabled = true,
                NoSandbox = true,
            };

            // Start the browser process (a child process).
            CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.SetAsWindowless(IntPtr.Zero, false);

            // Settings for the browser window itself (e.g. should JavaScript be enabled?).
            var cefBrowserSettings = new CefBrowserSettings();
            //cefBrowserSettings.WebGL = CefState.Disabled;
            //cefBrowserSettings.Plugins = CefState.Disabled;

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            cefClient = new DemoCefClient(1280, 720);
            cefClient.LoadHandler.OnGotSourceFromLoadEnd += LoadHandler_OnGotSourceFromLoadEnd;
            cefClient.LifeSpanHandler.OnAfterBrowserCreated += LifeSpanHandler_OnAfterBrowserCreated;

            // Start up the browser instance.                                                   
            CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings);
            //});
            // init.SetApartmentState(ApartmentState.STA);
            //init.Start();
        }

        private void LifeSpanHandler_OnAfterBrowserCreated(CefBrowser cefBrowser)
        {
            browser = cefBrowser;
            //IntPtr browserWindowHandle = browser.GetHost().GetWindowHandle();
            //if(browserWindowHandle != IntPtr.Zero)
            //    NativeMethods.SetWindowPos(browserWindowHandle, IntPtr.Zero, 0, 0, 1280, 720, SetWindowPosFlags.NoZOrder);

            Console.WriteLine("LifeSpanHandler_OnAfterBrowserCreated " + browser);
            SetAccessToken(Social.FACEBOOK_GRAPH_LINK);
            // OnReportInitialized();            
        }

        public void NavigateToUrl(string url)
        {
            if (url.Contains("/?ref=br_rs")) url = url.Replace("/?ref=br_rs", "");
            if (url.Contains("?ref=br_rs")) url = url.Replace("?ref=br_rs", "");

            string pageName = url;
            string urltillId = url;

            switch (crawlerState)
            {
                case CrawlerStates.FbGraphCrawl:
                case CrawlerStates.LoadAllPhotos:
                case CrawlerStates.LoadAllVideos:
                case CrawlerStates.LoadAllPhotos_Crawl:
                case CrawlerStates.LoadAllVideos_Crawl:
                    if (!url.Contains("https://www.facebook.com/"))
                    {
                        OnReportSerializedResult("N/A");
                        return;
                    }
                    pageName = getPageNameOrIdFromUrl(url);
                    urltillId = url.Remove(url.LastIndexOf("/")+1);
                    preRegetAccessToken = AccessToken;
                    break;
                default:
                    break;
            }


            switch (crawlerState)
            {
                case CrawlerStates.FbGraphCrawl:
                    //Debugger.Launch();
                    //maybe to add = keywords,emails,new_like_count,description,sharedposts
                    pageType = CrawlerStates.GraphSearch_Pages;
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName + "?fields=" +
                                        @"about,id,link,founded,can_post,category,talking_about_count,likes,
                                        photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        videos.limit(30){permalink_url,picture,id,views,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        posts.limit(100){caption,description,picture,full_picture,shares,link,message,via,source,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)},
                                        feed.limit(70){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;
                    if (urltillId.Contains(Social.FACEBOOK_GROUPS_DEFAULT_URL))
                    {
                        pageType = CrawlerStates.PageType_Groups;
                        preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName + "?fields=" +
                                        @"description,name,privacy,updated_time,
                                        members.limit(0).summary(true),
                                        feed.limit(100){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;
                    }
                    else if (urltillId.Contains(Social.FACEBOOK_EVENTS_DEFAULT_URL))
                    {
                        pageType = CrawlerStates.PageType_Events;
                        preRegetTokenUrl = "https://graph.facebook.com/v2.3/" + pageName + "?fields="+
                                          @"description,location,privacy,start_time,ticket_uri,timezone,updated_time,
                                            interested.limit(0).summary(true),
                                            invited.limit(0).summary(true),
                                            feed.limit(100){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                            &access_token=" + AccessToken;
                    }
                    else if (urltillId.Contains(Social.FACEBOOK_PLACES_DEFAULT_URL))
                    {
                        pageType = CrawlerStates.PageType_Places;
                        preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName + "?fields=" +
                                        @"about,id,name,category,can_post,description,founded,is_community_page,is_permanently_closed,is_published,is_unclaimed,is_verified,link,talking_about_count,website,likes,location,
                                        photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        albums{photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)}},
                                        videos.limit(30){permalink_url,picture,views,id,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        posts.limit(100){caption,description,picture,full_picture,shares,link,message,via,source,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)},
                                        feed.limit(70){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;
                    }
                    else if (urltillId.Contains(Social.FACEBOOK_PHOTOS_DEFAULT_URL))
                    {
                        pageType = CrawlerStates.PageType_Photos;
                        preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName + @"?fields=
                                            created_time,link,name,source,updated_time,album,from,picture,images,likes.limit(0).summary(true),comments.limit(200).summary(true)&access_token=" + AccessToken;
                    }
                    else if (urltillId.Contains(Social.FACEBOOK_VIDEOS_DEFAULT_URL))
                    {
                        pageType = CrawlerStates.PageType_Videos;
                        preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName + @"?fields=
                                            picture,id,views,embed_html,source,updated_time,description,created_time,likes.limit(0).summary(true),comments.limit(200).summary(true)&access_token=" + AccessToken;
                    }
                    else if (urltillId.Contains(Social.FACEBOOK_USERS_DEFAULT_URL))
                    {
                        pageType = CrawlerStates.PageType_Users;
                        OnReportSerializedResult("N/A");
                        return;
                    }

                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.LoadAllPhotos:
                    allCrawledPhotos.Clear();
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName +
                                       "?fields=photos.limit(50){picture,id,link,updated_time,images,likes.limit(0).summary(true),comments.limit(0).summary(true)}&access_token=" + AccessToken;
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.LoadAllPhotos_Crawl:
                    allMediaLinkToCrawl.Clear();
                    allCrawledPhotos.Clear();
                    if (urltillId.Contains(Social.FACEBOOK_GROUPS_DEFAULT_URL))
                    {
                        browser.GetMainFrame().LoadUrl(Social.FACEBOOK_GROUPS_DEFAULT_URL + pageName + "/photos/");
                    }
                    else
                    {
                        OnReportSerializedResult("N/A");
                    }
                    break;

                case CrawlerStates.LoadAllVideos:
                    allCrawledVideos.Clear();
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName +
                                      "?fields=videos.limit(50){permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)}&access_token=" + AccessToken;
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.LoadAllVideos_Crawl:
                    allMediaLinkToCrawl.Clear();
                    allCrawledVideos.Clear();
                    if (urltillId.Contains(Social.FACEBOOK_GROUPS_DEFAULT_URL))
                    {
                        browser.GetMainFrame().LoadUrl(Social.FACEBOOK_GROUPS_DEFAULT_URL + pageName + "/photos/?filter=videos");
                    }
                    else
                    {
                        OnReportSerializedResult("N/A");
                    }
                    break;

                case CrawlerStates.GraphSearch_Pages:
                    //search?q=bodybuilding&type=page&limit=500&fields=about,description,id,link,founded,can_post,category,talking_about_count,likes,picture{url}
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/search?q=" + pageName +
                        "&type=page&limit=500&fields=about,description,id,link,name,founded,can_post,category,talking_about_count,likes,picture{url}&access_token=" + AccessToken; 
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.GraphSearch_Groups:
                    //search?q=bodybuilding&type=group&limit=500&fields=description,id,name,picture{url},members.limit(0).summary(true),privacy 
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/search?q=" + pageName +
                       "&type=group&limit=500&fields=description,id,name,picture{url},members.limit(0).summary(true),privacy&access_token=" + AccessToken; 
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.GraphSearch_Events:
                    //v2.3 search?q=bodybuilding&type=event&limit=500&fields=description,id,picture{url},date,interested.limit(0).summary(true),invited.limit(0).summary(true) 
                    preRegetTokenUrl = "https://graph.facebook.com/v2.3/search?q=" + pageName +
                       "&type=event&limit=500&fields=description,id,name,picture{url},date,interested.limit(0).summary(true),invited.limit(0).summary(true)&access_token=" + AccessToken; 
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.GraphSearch_Places:
                    //search?q=bodybuilding&type=place&limit=300&fields=about,category,can_post,description,founded,is_community_page,is_permanently_closed,is_published,is_unclaimed,is_verified,link,talking_about_count,website,likes,picture{url},location
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/search?q=" + pageName +
                       "&type=place&limit=200&fields=about,id,name,category,can_post,description,founded,is_community_page,is_permanently_closed,is_published,is_unclaimed,is_verified,link,talking_about_count,website,likes,picture{url},location&access_token=" + AccessToken;
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.GraphSearch_Users:
                    //search?q=bodybuilding&type=user&limit=500&fields=name,id,link,picture //other then that need to crawl
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/search?q=" + pageName +
                       "&type=user&limit=500&fields=name,id,link,picture&access_token=" + AccessToken;
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.GraphSearch_Photos:
                    browser.GetMainFrame().LoadUrl("https://www.facebook.com/search/photos/?q=" + pageName);
                    break;

                case CrawlerStates.GraphSearch_Videos:
                    allCrawledVideos.Clear();
                    allMediaLinkToCrawl.Clear();
                    browser.GetMainFrame().LoadUrl("https://www.facebook.com/search/videos/?q=" + pageName);
                    break;

                default:
                    break;
            }
        }

        private string getPageNameOrIdFromUrl(string url)
        {
            string pageName = url;

            try
            {
                pageName = url.Substring(url.LastIndexOf("/") + 1);
                if (url.Contains("-"))
                {
                    string id = url.Substring(url.LastIndexOf("-") + 1);
                    long tryparseResult = 0;
                    if (Int64.TryParse(id, out tryparseResult))
                    {
                        pageName = id;
                    }
                }

                //pageName = pageName.Split(new string[] { @"https://www.facebook.com/" }, StringSplitOptions.None)[1];

                //if (pageName.Contains("pages/"))
                //{
                //    pageName = pageName.Split(new string[] { @"pages/" }, StringSplitOptions.None)[1];
                //}

                //pageName = pageName.Replace("//", "/");

                //if (pageName.Contains("/"))
                //{
                //    pageName = pageName.Remove(pageName.IndexOf("/"));
                //}

                ////edit for https://www.facebook.com/Body-building-motivation-457074090989432/
                //if (pageName.Contains("-"))
                //{
                //    string[] nameNums = pageName.Split('-');
                //    long tryparseResult = 0;
                //    int tryparseintResult = 0;
                //    string sToTry = nameNums[nameNums.Length - 1];
                //    if (!Int32.TryParse(sToTry, out tryparseintResult) && Int64.TryParse(sToTry, out tryparseResult))
                //    {
                //        nameNums[nameNums.Length - 1] = "";
                //        string newPageName = "";
                //        foreach (string s in nameNums)
                //        {
                //            newPageName += s + "-";
                //        }
                //        pageName = newPageName.Remove(newPageName.IndexOf("--"));
                //    }

                //}
            }
            catch { }

            return pageName;
        }

        private void LoadHandler_OnGotSourceFromLoadEnd(string source, string url)
        {
            try
            {
                if (url == Social.FACEBOOK_GRAPH_LINK)
                {

                    string accessTiken = source.Split(new string[] { @"{""accessToken"":""" }, StringSplitOptions.None)[1];
                    AccessToken = accessTiken.Remove(accessTiken.IndexOf(@""""));
                    if (preRegetTokenUrl == "")
                    {
                        preRegetAccessToken = AccessToken;
                        OnReportInitialized();
                    }
                    else
                    {
                        browser.GetMainFrame().LoadUrl(preRegetTokenUrl.Replace("access_token=" + preRegetAccessToken, "access_token=" + AccessToken));
                        preRegetAccessToken = AccessToken;
                        preRegetTokenUrl = "";
                    }
                    //NavigateToUrl("https://www.facebook.com/VapingCheap/");
                    return;
                }

                string json = "";
                if (source.Contains("<html><head></head><body>"))
                {
                    json = source.Split(new string[] { @"<html><head></head><body>" }, StringSplitOptions.None)[1];
                    json = json.Split('>')[1];
                    json = json.Remove(json.IndexOf("</pre"));
                }

                if (json.Contains("Error validating access token: Session has expired on"))
                {
                    SetAccessToken(Social.FACEBOOK_GRAPH_LINK);
                    return;
                }

                switch (crawlerState)
                {
                    case CrawlerStates.FbGraphCrawl:
                        GetAllPageStats(json, url);
                        break;

                    case CrawlerStates.LoadAllPhotos:
                        GetMorePhotos(json);
                        break;

                    case CrawlerStates.LoadAllVideos:
                        GetMoreVideos(json);
                        break;

                    case CrawlerStates.LoadAllPhotos_Crawl:
                        GetPhotosViaHtmlCrawl(source, json, url);
                        break;
                    case CrawlerStates.LoadAllVideos_Crawl:
                        GetVideosViaHtmlCrawl(source, json, url);
                        break;

                    case CrawlerStates.GraphSearch_Pages:
                    case CrawlerStates.GraphSearch_Groups: 
                    case CrawlerStates.GraphSearch_Events:   
                    case CrawlerStates.GraphSearch_Places: 
                    case CrawlerStates.GraphSearch_Users:
                        OnReportSerializedResult(json);
                        break;

                    case CrawlerStates.GraphSearch_Photos:
                        GetPhotosFromSearchCrawl(source);
                        break;

                    case CrawlerStates.GraphSearch_Videos:
                        GetVideosFromSearchCrawl(source, json, url);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                OnReportSerializedResult("N/A");
            }
        }





        private void GetVideosFromSearchCrawl(string source, string json, string url)
        {
            try
            {
                //Debugger.Launch();
                if (url.Contains("/search/videos/"))
                {
                    source = source.Replace("&quot;", "");
                    source = source.Replace("quot;", "");

                    //<div id="BrowseResultsContainer">
                    string firstResponders = source.Substring(source.IndexOf("id=\"BrowseResultsContainer\">"));
                    firstResponders = firstResponders.Substring(0, firstResponders.IndexOf("result_below_fold"));
                    foreach (var d in getIdsFromVideoScrape(firstResponders))
                    {
                        if (!allMediaLinkToCrawl.Contains(d)) allMediaLinkToCrawl.Add(d);
                    }

                    //result_below_fold
                    string secondResponders = source.Substring(source.IndexOf("result_below_fold"));
                    secondResponders = secondResponders.Remove(secondResponders.IndexOf("fbBrowseScrollingPagerContainer"));
                    foreach (var d in getIdsFromVideoScrape(secondResponders))
                    {
                        if (!allMediaLinkToCrawl.Contains(d)) allMediaLinkToCrawl.Add(d);
                    }

                    //fbBrowseScrollingPagerContainer
                    List<string> afterScrolledData = source.Split(new string[] { "fbBrowseScrollingPagerContainer" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    afterScrolledData.RemoveAt(0);
                    foreach (var item in afterScrolledData)
                    {
                        foreach (var d in getIdsFromVideoScrape(item))
                        {
                            if (!allMediaLinkToCrawl.Contains(d)) allMediaLinkToCrawl.Add(d);
                        }
                    }
                }

                if (allMediaLinkToCrawl.Count > 0)
                {
                    if (!url.Contains("/search/videos/"))
                    {
                        if (allCrawledPhotos.Count == 0)
                        {
                            VideosGraphData itemToReply = new VideosGraphData();
                            itemToReply.videos = new Videos();
                            itemToReply.videos.data = new ObservableCollection<Videos.Video>();
                            allCrawledVideos.Add(itemToReply);
                        }

                        if (!json.Contains("Unsupported get request."))
                        {
                            Videos.Video photo = JsonConvert.DeserializeObject<Videos.Video>(json);
                            allCrawledVideos[0].videos.data.Add(photo);
                        }
                    }


                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + allMediaLinkToCrawl[0] +
                        "?fields=permalink_url,picture,id,length,embed_html,source,updated_time,views,description,title,likes.limit(0).summary(true),comments.limit(0).summary(true)&access_token=" + AccessToken;
                    allMediaLinkToCrawl.RemoveAt(0);
                    browser.GetFocusedFrame().LoadUrl(preRegetTokenUrl);
                }
                else
                {
                    MediaResult resultToReply = new MediaResult();
                    if (allCrawledVideos.Count > 0)
                    {
                        foreach (var d in allCrawledVideos[0].videos.data)
                        {
                            resultToReply.data.Add(new MediaResultData()
                            {
                                about = d.description,
                                 comment_count = d.comments == null ? 0 : d.comments.summary == null ? 0 : d.comments.summary.total_count,
                                  id = d.id,
                                   like_count = d.likes == null ? 0 : d.likes.summary == null ? 0 : d.likes.summary.total_count,
                                    link = d.picture,
                                     source = d.source,
                                      updated_time = d.updated_time,
                                       view_count = d.views,
                                        is_video = true
                            });
                        }
                    }
                    OnReportSerializedResult(resultToReply.XmlSerializeToString());
                }

            }
            catch
            {
                OnReportSerializedResult("N/A");
            }
        }

        private List<string> getIdsFromVideoScrape(string source)
        {
            List<string> thisIdList = new List<string>();
            try
            {
                List<string> sourceAfterSplit = source.Split(new string[] { "data-bt=\"{id:" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                sourceAfterSplit.RemoveAt(0);
                foreach (string line in sourceAfterSplit)
                {
                    string id = line.Remove(line.IndexOf(","));
                    id = id.Trim();
                    thisIdList.Add(id);
                }
            }
            catch { }
            return thisIdList;
        }

        private void GetPhotosFromSearchCrawl(string source)
        {
            try
            {
                MediaResult resultToReply = new MediaResult();

                source = source.Replace("&quot;", "");
                source = source.Replace("quot;", "");

                //<div id="BrowseResultsContainer">
                string firstResponders = source.Substring(source.IndexOf("id=\"BrowseResultsContainer\">"));
                firstResponders = firstResponders.Substring(0, firstResponders.IndexOf("result_below_fold"));
                foreach (var d in getDataFromEditedPhotoScrapeSource(firstResponders))
                {
                    resultToReply.data.Add(d);
                }

                //result_below_fold
                string secondResponders = source.Substring(source.IndexOf("result_below_fold"));
                secondResponders = secondResponders.Remove(secondResponders.IndexOf("fbBrowseScrollingPagerContainer"));
                foreach (var d in getDataFromEditedPhotoScrapeSource(secondResponders))
                {
                    resultToReply.data.Add(d);
                }

                //fbBrowseScrollingPagerContainer
                List<string> afterScrolledData = source.Split(new string[] { "fbBrowseScrollingPagerContainer" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                afterScrolledData.RemoveAt(0);
                foreach (var item in afterScrolledData)
                {
                    foreach (var d in getDataFromEditedPhotoScrapeSource(item))
                    {
                        resultToReply.data.Add(d);
                    }
                }

                OnReportSerializedResult(resultToReply.XmlSerializeToString());
            }
            catch
            {
                OnReportSerializedResult("N/A");
            }
        }

        private List<MediaResultData> getDataFromEditedPhotoScrapeSource(string firstResponders)
        {
            List<MediaResultData> resultAfterCrawl = new List<MediaResultData>();

            try
            {
                List<string> images = firstResponders.Split(new string[] { "img\" src=\"" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                images.RemoveAt(0);
                foreach (string img in images)
                {
                    string link = img.Remove(img.IndexOf("\""));
                    if (img.Contains("background-image: url("))
                    {
                        link = img.Substring(img.IndexOf("background-image: url("));
                        link = link.Replace("background-image: url(", "");
                        link = link.Remove(link.IndexOf(");"));
                    }
                    string id = link.Substring(0, link.IndexOf("?")).Split('_')[1];

                    string about = img.Substring(img.IndexOf("alt=\"")).Replace("alt=\"", "");
                    about = about.Remove(about.IndexOf("\""));

                    long views_count = 0;
                    long likes_count = 0;
                    long comments_count = 0;
                    if (img.Contains("<div class=\"_37_g\">"))
                    {
                        string views = img.Substring(img.IndexOf("<div class=\"_37_g\">"));
                        views = views.Replace("<div class=\"_37_g\">", "");
                        views = views.Substring(views.IndexOf("</div>"));
                        views = views.Replace("</div>", "");
                        views = views.Replace("Views", "");
                        views = views.Replace(",", "");
                        views = views.Remove(views.IndexOf("<"));
                        views = views.Trim();
                        Int64.TryParse(views,out views_count);
                    }
                    else
                    {
                        string[] likesThenComments = img.Split(new string[] { "<div class=\"_50f3\">" }, StringSplitOptions.RemoveEmptyEntries);
                        if (likesThenComments.Length >= 3)
                        {
                            string likes = likesThenComments[1].Remove(likesThenComments[1].IndexOf("</div>"));
                            Int64.TryParse(getCountstring(likes), out likes_count);

                            string comments = likesThenComments[2].Remove(likesThenComments[2].IndexOf("</div>"));
                            Int64.TryParse(getCountstring(comments), out comments_count);
                        }
                    }

                    resultAfterCrawl.Add(new MediaResultData()
                    {
                        link = link,
                        id = id,
                        about = about,
                        view_count = views_count,
                        like_count = likes_count,
                        comment_count = comments_count,
                    });
                }
            }
            catch { }

            return resultAfterCrawl;
        }

        private string getCountstring(string data)
        {
            data = data.ToLower();
            if (data.Contains("k")) data = data.Replace("k", "000");
            if (data.Contains("m")) data = data.Replace("m", "000000");
            if (data.Contains("."))
            {
                data.Replace(".", "");
                data = data.Replace("000", "00");
            }
            return data = data.Trim();
        }




        private void GetMoreVideos(string json)
        {
            try
            {
                var jsonfile = JsonConvert.DeserializeObject<VideosGraphData>(json);
                if (jsonfile.videos == null)
                    jsonfile.videos = JsonConvert.DeserializeObject<Videos>(json);

                allCrawledVideos.Add(jsonfile);

                if (jsonfile == null || jsonfile.videos == null || string.IsNullOrEmpty(jsonfile.videos.paging.next) || string.IsNullOrWhiteSpace(jsonfile.videos.paging.next))
                {
                    OnReportSerializedResult(allCrawledVideos.XmlSerializeToString());
                }
                else
                {
                    //https://graph.facebook.com/v2.5/170365236335791/photos?access_token=CA..&pretty=0&fields=picture%2Cid%2Clink%2Cupdated_time%2Cimages%2Clikes.limit%281%29.summary%28true%29%2Ccomments.limit%281%29.summary%28true%29&limit=50&after=MjE4MjQxNTU4MjE0ODI1
                    //string nexLink = jsonfile.photos.paging.next;
                    //string tillQmark = nexLink.Split('?')[0]; 
                    //string after = nexLink.Substring(nexLink.LastIndexOf('='));
                    //preRegetAccessToken = AccessToken;
                    //preRegetTokenUrl = tillQmark + "?access_token=" + AccessToken + "&pretty=0&fields=picture%2Cid%2Clink%2Cupdated_time%2Cimages%2Clikes.limit%281%29.summary%28true%29%2Ccomments.limit%281%29.summary%28true%29&limit=50&after=" + after;
                    preRegetTokenUrl = jsonfile.videos.paging.next;
                    preRegetTokenUrl = preRegetTokenUrl.Replace("&amp;", "&");
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                }
            }
            catch (Exception ex)
            {
                if (allCrawledVideos.Count > 0)
                    OnReportSerializedResult(allCrawledVideos.XmlSerializeToString());
                else
                    OnReportSerializedResult("N/A");
            }
        }

        private void GetMorePhotos(string json)
        {
            try
            {
                var jsonfile = JsonConvert.DeserializeObject<PhotosGraphData>(json);
                if (jsonfile.photos == null)
                    jsonfile.photos = JsonConvert.DeserializeObject<Photos>(json);

                allCrawledPhotos.Add(jsonfile);

                if (jsonfile == null || jsonfile.photos == null || string.IsNullOrEmpty(jsonfile.photos.paging.next) || string.IsNullOrWhiteSpace(jsonfile.photos.paging.next))
                {
                    OnReportSerializedResult(allCrawledPhotos.XmlSerializeToString());
                }
                else
                {
                    //https://graph.facebook.com/v2.5/170365236335791/photos?access_token=CA..&pretty=0&fields=picture%2Cid%2Clink%2Cupdated_time%2Cimages%2Clikes.limit%281%29.summary%28true%29%2Ccomments.limit%281%29.summary%28true%29&limit=50&after=MjE4MjQxNTU4MjE0ODI1
                    //string nexLink = jsonfile.photos.paging.next;
                    //string tillQmark = nexLink.Split('?')[0]; 
                    //string after = nexLink.Substring(nexLink.LastIndexOf('='));
                    //preRegetAccessToken = AccessToken;
                    //preRegetTokenUrl = tillQmark + "?access_token=" + AccessToken + "&pretty=0&fields=picture%2Cid%2Clink%2Cupdated_time%2Cimages%2Clikes.limit%281%29.summary%28true%29%2Ccomments.limit%281%29.summary%28true%29&limit=50&after=" + after;
                    preRegetTokenUrl = jsonfile.photos.paging.next;
                    preRegetTokenUrl = preRegetTokenUrl.Replace("&amp;", "&");
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                }
            }
            catch (Exception ex)
            {
                if (allCrawledPhotos.Count > 0)
                    OnReportSerializedResult(allCrawledPhotos.XmlSerializeToString());
                else
                    OnReportSerializedResult("N/A");
            }
        }

        private void GetAllPageStats(string source, string url)
        {
            try
            {
                switch (pageType)
                {
                    case CrawlerStates.PageType_Videos:
                    case CrawlerStates.PageType_Photos:
                        var j = JsonConvert.DeserializeObject<FacebookGraphDataForMedia>(source);
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
                                                likes = j.likes == null ? 0 : j.likes.summary == null ? 0: j.likes.summary.total_count,
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
                        OnReportSerializedResult(data.XmlSerializeToString());
                        return;
                    default:
                        break;
                }
                var jsonfile = JsonConvert.DeserializeObject<FacebookGraphData>(source);
                if(jsonfile.albums != null && jsonfile.albums.data != null && jsonfile.albums.data.Count > 0)
                {
                    foreach (var album in jsonfile.albums.data)
                    {
                        if (album == null || album.photos == null || album.photos.data.Count == 0) continue;
                        if (jsonfile.photos == null) jsonfile.photos = new Photos();
                        if (jsonfile.photos.data == null) jsonfile.photos.data = new ObservableCollection<Photos.Photo>();
                        foreach (var p in album.photos.data)
                        {
                            jsonfile.photos.data.Add(p);
                        }
                    }
                }
                OnReportSerializedResult(jsonfile.XmlSerializeToString());
            }
            catch (Exception ex)
            {
                OnReportSerializedResult("N/A");
            }
        }





        private void GetPhotosViaHtmlCrawl(string source, string json, string url)
        {
            try
            {
                //Debugger.Launch();
                //?fields=source,link,picture,comments.limit(0).summary(true),likes.limit(0).summary(true)
                if (url.Contains("/photos/"))
                {
                    List<string> htmlSplit1 = source.Split(new string[] { "<a class=\"uiMediaThumb uiScrollableThumb uiMediaThumbLarge\" href=\"" }, StringSplitOptions.None).ToList();
                    htmlSplit1.RemoveAt(0);
                    foreach (string split in htmlSplit1)
                    {
                        string postId = split;
                        postId = postId.Remove(postId.IndexOf("\""));
                        postId = postId.Replace("&amp;", "&");
                        postId = postId.Replace("amp;", "");
                        postId = postId.Substring(postId.LastIndexOf("fbid="));
                        postId = postId.Replace("fbid=", "");
                        postId = postId.Remove(postId.IndexOf("&"));

                        if (!allMediaLinkToCrawl.Contains(postId)) allMediaLinkToCrawl.Add(postId);
                    }
                }

                if (allMediaLinkToCrawl.Count > 0)
                {
                    if (!url.Contains("/photos/"))
                    {
                        if (allCrawledPhotos.Count == 0)
                        {
                            PhotosGraphData itemToReply = new PhotosGraphData();
                            itemToReply.photos = new Photos();
                            itemToReply.photos.data = new System.Collections.ObjectModel.ObservableCollection<Photos.Photo>();
                            allCrawledPhotos.Add(itemToReply);
                        }

                        Photos.Photo photo = JsonConvert.DeserializeObject<Photos.Photo>(json);
                        allCrawledPhotos[0].photos.data.Add(photo);
                    }


                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + allMediaLinkToCrawl[0] + "?fields=link,picture,updated_time,images,comments.limit(0).summary(true),likes.limit(0).summary(true)&access_token=" + AccessToken;
                    allMediaLinkToCrawl.RemoveAt(0);
                    browser.GetFocusedFrame().LoadUrl(preRegetTokenUrl);
                }
                else
                {
                    OnReportSerializedResult(allCrawledPhotos.XmlSerializeToString());
                }
            }
            catch (Exception ex)
            {
                OnReportSerializedResult("N/A");
            }
        }

        private void GetVideosViaHtmlCrawl(string source, string json, string url)
        {
            try
            {
               // Debugger.Launch();
                //?fields=permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)
                if (url.Contains("/photos/"))
                {
                    List<string> htmlSplit1 = source.Split(new string[] { "<a class=\"uiVideoLink uiScrollableThumb uiVideoLinkLarge\" href=\"" }, StringSplitOptions.None).ToList();
                    htmlSplit1.RemoveAt(0);
                    foreach (string split in htmlSplit1)
                    {
                        string postId = split;
                        postId = postId.Substring(postId.IndexOf("name=\""));
                        postId = postId.Replace("name=\"", "");
                        postId = postId.Remove(postId.IndexOf("\""));
                        postId = postId.Replace("&amp;", "&");
                        postId = postId.Replace("amp;", "");

                       if(!allMediaLinkToCrawl.Contains(postId)) allMediaLinkToCrawl.Add(postId);
                    }
                }

                if (allMediaLinkToCrawl.Count > 0)
                {
                    if (!url.Contains("/photos/"))
                    {
                        if (allCrawledPhotos.Count == 0)
                        {
                            VideosGraphData itemToReply = new VideosGraphData();
                            itemToReply.videos = new Videos();
                            itemToReply.videos.data = new ObservableCollection<Videos.Video>();
                            allCrawledVideos.Add(itemToReply);
                        }

                        Videos.Video photo = JsonConvert.DeserializeObject<Videos.Video>(json);
                        allCrawledVideos[0].videos.data.Add(photo);
                    }


                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + allMediaLinkToCrawl[0] +
                        "?fields=permalink_url,picture,id,length,embed_html,source,views,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)&access_token=" + AccessToken;
                    allMediaLinkToCrawl.RemoveAt(0);
                    browser.GetFocusedFrame().LoadUrl(preRegetTokenUrl);
                }
                else
                {
                    OnReportSerializedResult(allCrawledVideos.XmlSerializeToString());
                }
            }
            catch (Exception ex)
            {
                OnReportSerializedResult("N/A");
            }
        }





        public void Shutdown()
        {
            var host = browser.GetHost();
            if (host != null)
            {
                host.CloseBrowser();
                host.Dispose();
            }

            if (browser != null)
                browser.Dispose();

            CefRuntime.Shutdown();
        }

        public virtual object GetService(Type serviceType)
        {
            if (serviceType.IsAssignableFrom(GetType())) return this;
            return null;
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }
    }

    internal class DemoCefApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //commandLine.AppendSwitch("disable-gpu", "1");
            //commandLine.AppendSwitch("disable-gpu-compositing", "1");
            //commandLine.AppendSwitch("enable-begin-frame-scheduling", "1");
            //commandLine.AppendSwitch("disable-gpu-vsync", "1");
            if (GloableProfData.PData != null && !string.IsNullOrEmpty(GloableProfData.PData.ProxyIP) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyIP))
            {
                try
                {
                    Console.WriteLine("setting proxy");
                    commandLine.AppendSwitch("proxy-server", GloableProfData.PData.ProxyIP + ":" + GloableProfData.PData.ProxyPort);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed to set proxy");
                }
            }
        }
    }




    internal class DemoCefClient : CefClient
    {
        private readonly DemoCefLoadHandler _loadHandler;
        public DemoCefLoadHandler LoadHandler { get { return _loadHandler; } }

        private readonly DemoCefLifeSpanHandler _lifeSpanHandler;
        public DemoCefLifeSpanHandler LifeSpanHandler { get { return _lifeSpanHandler; } }

        private readonly DemoCefRenderHandler _renderHandler;
        private readonly DemoRequestHandler _requestHandler;
        private readonly DemoCefFocusHandler _focusHandler;

        public DemoCefClient(int windowWidth, int windowHeight)
        {
            _renderHandler = new DemoCefRenderHandler(windowWidth, windowHeight);
            _loadHandler = new DemoCefLoadHandler();
            _requestHandler = new DemoRequestHandler();
            _lifeSpanHandler = new DemoCefLifeSpanHandler();
            _focusHandler = new DemoCefFocusHandler();
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override CefRequestHandler GetRequestHandler()
        {
            return _requestHandler;
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefFocusHandler GetFocusHandler()
        {
            return _focusHandler;
        }
    }

    public class DemoCefFocusHandler : CefFocusHandler
    {
        protected override void OnGotFocus(CefBrowser browser)
        {
            // Console.ReadLine();
            base.OnGotFocus(browser);
        } 

        protected override bool OnSetFocus(CefBrowser browser, CefFocusSource source)
        {
            //Console.ReadLine();
            browser.GetHost().SetFocus(true);
            return base.OnSetFocus(browser, source);
        }

        protected override void OnTakeFocus(CefBrowser browser, bool next)
        {
            //Console.ReadLine();
            base.OnTakeFocus(browser, next);
        }
    }

    internal class DemoCefLifeSpanHandler : CefLifeSpanHandler
    {
        internal event Action<CefBrowser> OnAfterBrowserCreated = delegate { };
        protected override void OnAfterCreated(CefBrowser browser)
        {
            OnAfterBrowserCreated(browser);
        }
    }

    public class DemoRequestHandler : CefRequestHandler
    {
        protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            //if (request.Url.Contains("https://fbcdn-photos-b-a.akamaihd.net/"))
            //{
            //    try
            //    {
            //        foreach (var item in request.GetHeaderMap())
            //        {

            //        }
            //        foreach (var item in request.GetHeaderMap().AllKeys)
            //        {
            //            foreach (var item1 in request.GetHeaderMap().GetValues(item))
            //            {

            //            }

            //        }
            //        foreach (var item in request.GetHeaderMap().Keys)
            //        {

            //        }
            //    }
            //    catch { }
            //    Debugger.Break();
            //}
            //try
            //{
            //    foreach (var item in request.GetHeaderMap())
            //    {

            //    }
            //    foreach (var item in request.GetHeaderMap().AllKeys)
            //    {
            //        foreach (var item1 in request.GetHeaderMap().GetValues(item))
            //        {

            //        }

            //    }
            //    foreach (var item in request.GetHeaderMap().Keys)
            //    {

            //    }

            //    foreach (var pda in request.PostData.GetElements())
            //    {

            //    }
            //}
            //catch { }
            return base.OnBeforeResourceLoad(browser, frame, request);
        }
        protected override bool GetAuthCredentials(CefBrowser browser, CefFrame frame, bool isProxy, string host, int port, string realm, string scheme, CefAuthCallback callback)
        {
            if (isProxy)
            {
                if (GloableProfData.PData != null)
                {
                    try
                    {
                        Console.WriteLine("setting proxy credentials");
                        callback.Continue(GloableProfData.PData.ProxyUsername, GloableProfData.PData.ProxyPassword);
                    }
                    catch
                    {
                        Console.WriteLine("Faild to set proxy auth credentials");
                    }
                }
            }

            return true;
        }
    }

    internal class DemoCefLoadHandler : CefLoadHandler
    {
        internal event Action<string, string> OnGotSourceFromLoadEnd = delegate { };

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            // A single CefBrowser instance can handle multiple requests
            //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
            if (frame.Url.Contains("generic.php/GroupPhotosetPagelet"))
            {
                getAllotOfPhotosRecursive(browser);
            }
            if (frame.IsMain)
            {
                Console.WriteLine("START: {0}", browser.GetMainFrame().Url);
            }
        }
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            if (frame.IsMain)
            {
                Console.WriteLine("END: {0}, {1}", browser.GetMainFrame().Url, httpStatusCode);
                if (browser.GetMainFrame().Url.Contains("/photos/") || browser.GetMainFrame().Url.Contains("/search/videos/"))
                {
                    maxrecursive = 3;
                    if (browser.GetMainFrame().Url.Contains("/search/photos/") || browser.GetMainFrame().Url.Contains("/search/videos/")) maxrecursive = 75;
                    recursiveScrollCalls = 0;
                    getAllotOfPhotosRecursive(browser);
                }
                else
                {
                    SourceVisitor Visitor = new SourceVisitor(browser.GetMainFrame().Url,
                    (text, url) =>
                    {
                        OnGotSourceFromLoadEnd(text, url);
                    });
                    browser.GetMainFrame().GetSource(Visitor);
                }
            }
        }

        static int recursiveScrollCalls = 0;
        static int maxrecursive = 3;
        private void getAllotOfPhotosRecursive(CefBrowser browser)
        {
            recursiveScrollCalls++;
            browser.GetMainFrame().ExecuteJavaScript("window.scrollTo(0,document.body.scrollHeight);", browser.GetMainFrame().Url, 0);
            Thread.Sleep(300);

            SourceVisitor Visitor = new SourceVisitor(browser.GetMainFrame().Url,
            (text, url) =>
            {

                if (browser.GetMainFrame().Url.Contains("/search/photos/") || browser.GetMainFrame().Url.Contains("/search/videos/"))
                {
                    if (recursiveScrollCalls >= maxrecursive)
                    {
                        OnGotSourceFromLoadEnd(text, url);
                    }
                    else
                    {
                        getAllotOfPhotosRecursive(browser);
                    }
                }
                else
                {
                    if (text.Contains("iframe class=\"hidden_elem\""))
                    {
                        if (recursiveScrollCalls >= maxrecursive)
                        {
                            OnGotSourceFromLoadEnd(text, url);
                        }
                        else
                        {
                            getAllotOfPhotosRecursive(browser);
                        }
                    }
                    else
                    {
                        OnGotSourceFromLoadEnd(text, url);
                    }
                }
            });
            browser.GetMainFrame().GetSource(Visitor);
        }
    }

    internal class SourceVisitor : Xilium.CefGlue.CefStringVisitor
    {
        private readonly Action<string, string> _callback;
        private readonly string url;

        public SourceVisitor(string url, Action<string, string> callback)
        {
            this.url = url;
            _callback = callback;
        }

        protected override void Visit(string value)
        {
            _callback(value, url);
        }
    }

    internal class DemoCefRenderHandler : CefRenderHandler
    {
        private readonly int _windowHeight;
        private readonly int _windowWidth;

        public DemoCefRenderHandler(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return GetViewRect(browser, ref rect);
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect.X = 0;
            rect.Y = 0;
            rect.Width = _windowWidth;
            rect.Height = _windowHeight;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            // Save the provided buffer (a bitmap image) as a PNG.
            //var bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb, buffer);
            //bitmap.Save("LastOnPaint.png", ImageFormat.Png);
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser)
        {
        }
    }
}



//    case CrawlerStates.UploadVideoFromFile:
//    #region uloadLink
//    //using (System.Net.WebClient client = new System.Net.WebClient())
//    //{
//    //    client.Proxy = MyFilesDatabase.GetRequestsProxy();
//    //    byte[] response =
//    //    client.UploadValues("https://graph-video.facebook.com/564872245/videos", new System.Collections.Specialized.NameValueCollection()
//    //    {
//    //        { "file_url", "https://video.xx.fbcdn.net/hvideo-xap1/v/t43.1792-2/12345014_965592126849084_2001058288_n.mp4?efg=eyJybHIiOjE1MDAsInJsYSI6MTAyNCwidmVuY29kZV90YWciOiJzdmVfaGQifQ%3D%3D&rl=1500&vabr=441&oh=8067e5b5d2e1b95c0313c91cdbfa571f&oe=568F52A6" },
//    //        { "access_token", AccessToken } ,
//    //        { "embeddable", "true"},
//    //    });

//    //    string result = System.Text.Encoding.UTF8.GetString(response);
//    //}


//    //string link = "https://video.xx.fbcdn.net/hvideo-xtf1/v/t43.1792-2/1416059_953832134670875_796264961_n.mp4?efg=eyJybHIiOjE1MDAsInJsYSI6MTAyNCwidmVuY29kZV90YWciOiJzdmVfaGQifQ%3D%3D&rl=1500&vabr=728&oh=cbdccd93fbcabdbbc5f35d6d7939364f&oe=569042B6";
//    //link = link.Replace("?", "%3F");
//    //link = link.Replace("=", "%3D");
//    //link = link.Replace("&", "%26");
//    //string uploadVideoUrl = "https://graph-video.facebook.com/564872245/videos?file_url=" + link + "&access_token=" + AccessToken;
//    //using (CefRequest request = CefRequest.Create())
//    //{
//    //    System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
//    //    headers.Add("Content-Type", "application/x-www-form-urlencoded");

//    //    request.Set(uploadVideoUrl, "POST", null, headers);

//    //    browser.GetMainFrame().LoadRequest(request);
//    //}
//    #endregion

//    //string filePath = @"C:\Users\eli\Desktop\11234179_10153357986963057_1600224766_n.mp4";
//    //fileBytes = File.ReadAllBytes(filePath);
//    //string uploadFromLocalLink = "https://graph-video.facebook.com/v2.5/564872245/videos?upload_phase=start&file_size="+ fileBytes.Length+ "&access_token=" + AccessToken;
//    //sendPostData(uploadFromLocalLink);
//    break;

//case CrawlerStates.LikesFromPost:
//    //ContinuedLikesCrawl = null;
//    //browser.GetMainFrame().LoadUrl(
//    //    "https://graph.facebook.com/v2.5/" + url +
//    //    "?fields=likes.limit(1000000000)&access_token=" + AccessToken);
//    break;



//internal class DemoCefApp : CefApp
//{
//}

//internal class DemoCefClient : CefClient
//{
//    private readonly DemoCefLoadHandler _loadHandler;
//    private readonly DemoCefRenderHandler _renderHandler;

//    public DemoCefClient(int windowWidth, int windowHeight)
//    {
//        _renderHandler = new DemoCefRenderHandler(windowWidth, windowHeight);
//        _loadHandler = new DemoCefLoadHandler();
//    }

//    protected override CefRenderHandler GetRenderHandler()
//    {
//        return _renderHandler;
//    }

//    protected override CefLoadHandler GetLoadHandler()
//    {
//        return _loadHandler;
//    }
//}

//internal class DemoCefLoadHandler : CefLoadHandler
//{
//    protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
//    {
//        // A single CefBrowser instance can handle multiple requests
//        //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
//        if (frame.IsMain)
//        {
//            Console.WriteLine("START: {0}", browser.GetMainFrame().Url);
//        }
//    }

//    protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
//    {
//        if (frame.IsMain)
//        {
//            Console.WriteLine("END: {0}, {1}", browser.GetMainFrame().Url, httpStatusCode);
//        }
//    }
//}

//internal class DemoCefRenderHandler : CefRenderHandler
//{
//    private readonly int _windowHeight;
//    private readonly int _windowWidth;

//    public DemoCefRenderHandler(int windowWidth, int windowHeight)
//    {
//        _windowWidth = windowWidth;
//        _windowHeight = windowHeight;
//    }

//    protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
//    {
//        return GetViewRect(browser, ref rect);
//    }

//    protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
//    {
//        screenX = viewX;
//        screenY = viewY;
//        return true;
//    }

//    protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
//    {
//        rect.X = 0;
//        rect.Y = 0;
//        rect.Width = _windowWidth;
//        rect.Height = _windowHeight;
//        return true;
//    }

//    protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
//    {
//        return false;
//    }

//    protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
//    {
//    }

//    protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
//    {
//        // Save the provided buffer (a bitmap image) as a PNG.
//        var bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb, buffer);
//        bitmap.Save("LastOnPaint.png", ImageFormat.Png);
//    }

//    protected override void OnScrollOffsetChanged(CefBrowser browser)
//    {
//    }

//    protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
//    { 
//    }
//}