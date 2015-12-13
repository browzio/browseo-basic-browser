using CrawlerContracts;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using Organiser.Common.Classes.Crawler;
using SocialOrganizer.Models;
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
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
        //public void start()
        //{
        //    // Load CEF. This checks for the correct CEF version.
        //    CefRuntime.Load();

        //    // Start the secondary CEF process.
        //    var cefMainArgs = new CefMainArgs(new string[0]);
        //    var cefApp = new DemoCefApp();

        //    // This is where the code path divereges for child processes.
        //    if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp) != -1)
        //    {
        //        Console.Error.WriteLine("CefRuntime could not the secondary process.");
        //    }

        //    // Settings for all of CEF (e.g. process management and control).
        //    var cefSettings = new CefSettings
        //    {
        //        SingleProcess = false,
        //        MultiThreadedMessageLoop = true
        //    };

        //    // Start the browser process (a child process).
        //    CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp);

        //    // Instruct CEF to not render to a window at all.
        //    CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
        //    cefWindowInfo.SetAsWindowless(IntPtr.Zero,false);

        //    // Settings for the browser window itself (e.g. should JavaScript be enabled?).
        //    var cefBrowserSettings = new CefBrowserSettings();

        //    // Initialize some the cust interactions with the browser process.
        //    // The browser window will be 1280 x 720 (pixels).
        //    var cefClient = new DemoCefClient(1280, 720);

        //    // Start up the browser instance.
        //    string url = "http://www.reddit.com/";
        //    CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings, url);

        //    // Hang, to let the browser to do its work.
        //    Console.WriteLine("Press a key at any time to end the program.");
        //    Console.ReadKey();

        //    // Clean up CEF.
        //    CefRuntime.Shutdown();
        //}

        public event Action OnReportInitialized = delegate { };
        //event Action IPlugin.OnReportInitialized
        //{
        //    add
        //    {
        //        lock (mlock)
        //        {
        //            OnReportInitialized += value;
        //        }
        //    }

        //    remove
        //    {
        //        lock (mlock)
        //        {
        //            OnReportInitialized -= value;
        //        }
        //    }
        //}
        public event Action<string> OnReportSerializedResult = delegate { };
        //event Action<string> IPlugin.OnReportSerializedResult
        //{
        //    add
        //    {
        //        lock (mlock)
        //        {
        //            OnReportSerializedResult += value;
        //        }
        //    }

        //    remove
        //    {
        //        lock (mlock)
        //        {
        //            OnReportSerializedResult -= value;
        //        }
        //    }
        //}


        private DemoCefClient cefClient;
        private CefBrowser browser;

        private CrawlerStates crawlerState;

        private List<PhotosGraphData> allCrawledPhotos = new List<PhotosGraphData>();
        private List<VideosGraphData> allCrawledVideos = new List<VideosGraphData>();

        private string AccessToken = "", preRegetTokenUrl = "", preRegetAccessToken = "";
        private object mlock = new object();

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
            string pageName = getPageNameFromUrl(url);
            preRegetAccessToken = AccessToken;

            switch (crawlerState)
            {
                case CrawlerStates.FbGraphCrawl:
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName + "?fields=" +
                                        @"about,description,keywords,id,link,emails,new_like_count,founded,can_post,category,talking_about_count,likes,
                                        photos.limit(50){picture,id,link,updated_time,images,likes.limit(1).summary(true),comments.limit(1).summary(true)},
                                        videos.limit(50){permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,sharedposts,likes.limit(1).summary(true),comments.limit(1).summary(true)},
                                        posts.limit(100){caption,description,picture,full_picture,shares,link,message,via,source,updated_time,likes.limit(1).summary(true)}
                                        &access_token=" + AccessToken;

                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.LoadAllPhotos:
                    allCrawledPhotos.Clear();
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName +
                                       "?fields=photos.limit(50){picture,id,link,updated_time,images,likes.limit(1).summary(true),comments.limit(1).summary(true)}&access_token=" + AccessToken;
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.LoadAllVideos:
                    allCrawledVideos.Clear();
                    preRegetTokenUrl = "https://graph.facebook.com/v2.5/" + pageName +
                                      "?fields=videos.limit(50){permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,sharedposts,likes.limit(1).summary(true),comments.limit(1).summary(true)}&access_token=" + AccessToken;
                    browser.GetMainFrame().LoadUrl(preRegetTokenUrl);
                    break;

                case CrawlerStates.UploadVideoFromFile:
                    #region uloadLink
                    //using (System.Net.WebClient client = new System.Net.WebClient())
                    //{
                    //    client.Proxy = MyFilesDatabase.GetRequestsProxy();
                    //    byte[] response =
                    //    client.UploadValues("https://graph-video.facebook.com/564872245/videos", new System.Collections.Specialized.NameValueCollection()
                    //    {
                    //        { "file_url", "https://video.xx.fbcdn.net/hvideo-xap1/v/t43.1792-2/12345014_965592126849084_2001058288_n.mp4?efg=eyJybHIiOjE1MDAsInJsYSI6MTAyNCwidmVuY29kZV90YWciOiJzdmVfaGQifQ%3D%3D&rl=1500&vabr=441&oh=8067e5b5d2e1b95c0313c91cdbfa571f&oe=568F52A6" },
                    //        { "access_token", AccessToken } ,
                    //        { "embeddable", "true"},
                    //    });

                    //    string result = System.Text.Encoding.UTF8.GetString(response);
                    //}


                    //string link = "https://video.xx.fbcdn.net/hvideo-xtf1/v/t43.1792-2/1416059_953832134670875_796264961_n.mp4?efg=eyJybHIiOjE1MDAsInJsYSI6MTAyNCwidmVuY29kZV90YWciOiJzdmVfaGQifQ%3D%3D&rl=1500&vabr=728&oh=cbdccd93fbcabdbbc5f35d6d7939364f&oe=569042B6";
                    //link = link.Replace("?", "%3F");
                    //link = link.Replace("=", "%3D");
                    //link = link.Replace("&", "%26");
                    //string uploadVideoUrl = "https://graph-video.facebook.com/564872245/videos?file_url=" + link + "&access_token=" + AccessToken;
                    //using (CefRequest request = CefRequest.Create())
                    //{
                    //    System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
                    //    headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    //    request.Set(uploadVideoUrl, "POST", null, headers);

                    //    browser.GetMainFrame().LoadRequest(request);
                    //}
                    #endregion

                    //string filePath = @"C:\Users\eli\Desktop\11234179_10153357986963057_1600224766_n.mp4";
                    //fileBytes = File.ReadAllBytes(filePath);
                    //string uploadFromLocalLink = "https://graph-video.facebook.com/v2.5/564872245/videos?upload_phase=start&file_size="+ fileBytes.Length+ "&access_token=" + AccessToken;
                    //sendPostData(uploadFromLocalLink);
                    break;

                case CrawlerStates.LikesFromPost:
                    //ContinuedLikesCrawl = null;
                    //browser.GetMainFrame().LoadUrl(
                    //    "https://graph.facebook.com/v2.5/" + url +
                    //    "?fields=likes.limit(1000000000)&access_token=" + AccessToken);
                    break;

                default:
                    break;
            }
        }

        private string getPageNameFromUrl(string url)
        {
            string pageName = url;

            try
            {
                pageName = pageName.Split(new string[] { @"https://www.facebook.com/" }, StringSplitOptions.None)[1];

                if (pageName.Contains("pages/"))
                {
                    pageName = pageName.Split(new string[] { @"pages/" }, StringSplitOptions.None)[1];
                }

                pageName = pageName.Replace("//", "/");

                if (pageName.Contains("/"))
                {
                    pageName = pageName.Remove(pageName.IndexOf("/"));
                }

                //edit for https://www.facebook.com/Body-building-motivation-457074090989432/
                if (pageName.Contains("-"))
                {
                    string[] nameNums = pageName.Split('-');
                    long tryparseResult = 0;
                    int tryparseintResult = 0;
                    string sToTry = nameNums[nameNums.Length - 1];
                    if (!Int32.TryParse(sToTry, out tryparseintResult) && Int64.TryParse(sToTry, out tryparseResult))
                    {
                        nameNums[nameNums.Length - 1] = "";
                        string newPageName = "";
                        foreach (string s in nameNums)
                        {
                            newPageName += s + "-";
                        }
                        pageName = newPageName.Remove(newPageName.IndexOf("--"));
                    }

                }
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

                string json = source.Split(new string[] { @"<html><head></head><body>" }, StringSplitOptions.None)[1];
                json = json.Split('>')[1];
                json = json.Remove(json.IndexOf("</pre"));


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

                    case CrawlerStates.LikesFromPost:
                        //AddToLikes(json);
                        break;

                    case CrawlerStates.UploadVideoFromFile:
                        //UploadingVideo(json);
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
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

                var jsonfile = JsonConvert.DeserializeObject<FacebookGraphData>(source);
                OnReportSerializedResult(jsonfile.XmlSerializeToString());
                //NavigateToUrl("https://www.facebook.com/VapingCheap/");
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
            //IntPtr browserWindowHandle = browser.GetHost().GetWindowHandle();
            //if (browserWindowHandle != IntPtr.Zero)
            //    NativeMethods.SetWindowPos(browserWindowHandle, IntPtr.Zero, 0, 0, 1280, 720, SetWindowPosFlags.NoZOrder);

            //browser.GetHost().SetFocus(true);
            //frame.Browser.GetHost().SetFocus(true);
            //SetForegroundWindow(browser.GetHost().GetWindowHandle());
            //SetForegroundWindow(frame.Browser.GetHost().GetWindowHandle());

            // A single CefBrowser instance can handle multiple requests
            //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
            if (frame.IsMain)
            {
                Console.WriteLine("START: {0}", browser.GetMainFrame().Url);
            }
        }
        //protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        //{
        //    if (!isLoading && browser.GetMainFrame() != null)
        //    {
        //        Console.WriteLine("END: {0}, {1}", browser.GetMainFrame().Url, isLoading);
        //        SourceVisitor Visitor = new SourceVisitor(browser.GetMainFrame().Url,
        //            (text, url) =>
        //            {
        //                OnGotSourceFromLoadEnd(text, url);
        //            });
        //        browser.GetMainFrame().GetSource(Visitor);
        //    }
        //}
        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            if (frame.IsMain)
            {
                Console.WriteLine("END: {0}, {1}", browser.GetMainFrame().Url, httpStatusCode);
                SourceVisitor Visitor = new SourceVisitor(browser.GetMainFrame().Url,
                    (text, url) =>
                    {
                        OnGotSourceFromLoadEnd(text, url);
                    });
                browser.GetMainFrame().GetSource(Visitor);
            }
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
