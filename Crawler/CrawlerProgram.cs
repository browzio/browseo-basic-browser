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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xilium.CefGlue;
namespace Crawler
{
    [AddIn("Crawler", Version = "1.0.0.0", Publisher = "Browseo", Description = "Ninja Crawler")]
    public class Crawler : AddInView.ProcessorAddInView
    {
        private AddInView.HostObject host;
         
        private DemoCefClient cefClient;
        private CefBrowser browser;

        private CrawlerStates crawlerState;

        private LikesData ContinuedLikesCrawl = null;

        private string AccessToken;     

        public Crawler()
        {
            AccessToken = "";
            //Create the loader (a proxy).
            //var assemblyLoader = (SimpleAssemblyLoader)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(typeof(SimpleAssemblyLoader).Assembly.FullName, 
            //  typeof(SimpleAssemblyLoader).FullName);
            //Load an assembly in the LoadFrom context. Note that the Load context will
            //not work unless you correctly set the AppDomain base-dir and private-bin-paths.
            // assemblyLoader.LoadFrom(AppDomain.CurrentDomain.BaseDirectory.Replace(@"AddIns\CrawlerAddIn", "Browseo.BrowserAssemby.dll"));
            //   assemblyLoader.LoadFrom(AppDomain.CurrentDomain.BaseDirectory.Replace(@"AddIns\CrawlerAddIn", "Browseo.WindowsForms.dll"));
            // assemblyLoader.LoadFrom(AppDomain.CurrentDomain.BaseDirectory.Replace(@"AddIns\CrawlerAddIn", "Organiser.Common.dll"));
        }

        public override void Initialize(AddInView.HostObject hostObj)
        {
            host = hostObj;
        }

        public override void SetPersonData(string serializedPdataXml)
        {
            GloableProfData.PData = serializedPdataXml.XmlDeserializeFromString<PersonData>();//.XmlDeserializeFromString(typeof(PersonData)) as PersonData;
        }

        public override void InitializeCefWithCachePath(string path)
        {
            //ConsoleManager.Show();
            Console.WriteLine(path);
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            exePath = exePath.Replace(@"\AddIns\CrawlerAddIn", "");

            // Load CEF. This checks for the correct CEF version.
            CefRuntime.Load(exePath);

            // Start the secondary CEF process.
            var cefMainArgs = new CefMainArgs(new string[0]);
            var cefApp = new DemoCefApp();

            // This is where the code path divereges for child processes.
            if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero) != -1)
            {
                Console.Error.WriteLine("CefRuntime could not the secondary process.");
            }

            // Settings for all of CEF (e.g. process management and control).  
            var cefSettings = new CefSettings
            {
                BrowserSubprocessPath = AppDomain.CurrentDomain.BaseDirectory + "\\Crawler.dll",
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                PersistSessionCookies = true,
                LogSeverity = CefLogSeverity.Disable,
                IgnoreCertificateErrors = true,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36",
                CachePath = path,
                WindowlessRenderingEnabled = true,
                //CachePath = cachePath,
            };

            // Start the browser process (a child process).
            CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);

            // Instruct CEF to not render to a window at all.
            CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
            cefWindowInfo.Handle = IntPtr.Zero;

            // Settings for the browser window itself (e.g. should JavaScript be enabled?).
            var cefBrowserSettings = new CefBrowserSettings();

            // Initialize some the cust interactions with the browser process.
            // The browser window will be 1280 x 720 (pixels).
            cefClient = new DemoCefClient(1280, 720);
            cefClient.LoadHandler.OnGotSourceFromLoadEnd += LoadHandler_OnGotSourceFromLoadEnd;
            cefClient.LifeSpanHandler.OnAfterBrowserCreated += LifeSpanHandler_OnAfterBrowserCreated;
            // Start up the browser instance.                                                   
           CefBrowserHost.CreateBrowser(cefWindowInfo, cefClient, cefBrowserSettings);

            // Hang, to let the browser to do its work.
           // Console.WriteLine("Press a key at any time to end the program.");
           // Console.ReadKey();
        }

        private void LifeSpanHandler_OnAfterBrowserCreated(CefBrowser cefBrowser)
        {
            browser = cefBrowser; 
            host.ReportInitialized();
        }

        public override void NavigateToUrl(string url)
        {
            switch (crawlerState)
            {
                case CrawlerStates.FbGraphCrawl: 
                    string pageName = url;
                    pageName = pageName.Split(new string[] { @"https://www.facebook.com/" }, StringSplitOptions.None)[1];
                    if(pageName.Contains("pages/"))
                        pageName = pageName.Split(new string[] { @"pages/" }, StringSplitOptions.None)[1];
                    pageName = pageName.Remove(pageName.IndexOf("/"));            
 //"videos.limit(100)%7Bpermalink_url%2Cpicture%2Cid%2Clength%2Cembed_html%2Csource%2Cupdated_time%2Cdescription%2Cembeddable%2Ctitle%2Csharedposts%2Clikes%2Ccomments%7D"
                    browser.GetMainFrame().LoadUrl(
                        "https://graph.facebook.com/v2.5/"+ pageName +
                        "?fields=about%2Cdescription%2Cid%2Ckeywords%2Clink%2Cemails%2Cnew_like_count%2Cfounded%2Ccan_post%2Ccategory%2Clikes%7Btalking_about_count%2Clink%2Cid%7D%2Cphotos%2Cvideos.limit(100)%7Bpermalink_url%2Cpicture%2Cid%2Clength%2Cembed_html%2Csource%2Cupdated_time%2Cdescription%2Cembeddable%2Ctitle%2Csharedposts%2Clikes%2Ccomments%7D%2Cposts.limit(100)%7Blikes%2Ccaption%2Cdescription%2Cicon%2Cpicture%2Cfull_picture%2Cshares%2Clink%2Cmessage%2Cvia%2Csource%2Cupdated_time%7D&access_token=" + AccessToken);
                    break;

                case CrawlerStates.LikesFromPost:
                    ContinuedLikesCrawl = null;
                    browser.GetMainFrame().LoadUrl(
                        "https://graph.facebook.com/v2.5/" + url +
                        "?fields=likes.limit(1000000000)&access_token=" + AccessToken);
                    break;

                default:
                    break;
            }  
        }

        private void LoadHandler_OnGotSourceFromLoadEnd(string source, string url)
        {
            try
            {
                if (url == Social.FACEBOOK_GRAPH_LINK)
                {
                    string accessTiken = source.Split(new string[] { @"{""accessToken"":""" }, StringSplitOptions.None)[1];
                    AccessToken = accessTiken.Remove(accessTiken.IndexOf(@""""));
                    host.ReportInitialized();
                    return;
                }
                string json = source.Split(new string[] { @"<html><head></head><body>" }, StringSplitOptions.None)[1];
                json = json.Split('>')[1];
                json = json.Remove(json.IndexOf("</pre"));

                switch (crawlerState)
                {
                    case CrawlerStates.FbGraphCrawl: 
                        GetLikesFromPage(json, url);
                        break;

                    case CrawlerStates.LikesFromPost:
                        AddToLikes(json);
                        //browser.GetMainFrame().LoadUrl(url);
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }
        }

        private void AddToLikes(string json)
        {
            try
            {
                var likes = JsonConvert.DeserializeObject<LikesData>(json);
                if (ContinuedLikesCrawl == null)
                    ContinuedLikesCrawl = likes;
                else
                {
                    Likes likess = null;

                    if (likes.likes == null)
                    {
                         likess = JsonConvert.DeserializeObject<Likes>(json);
                    }
                    foreach (Likes.Data d in likess == null?likes.likes.data: likess.data)
                    {
                        ContinuedLikesCrawl.likes.data.Add(d);
                    }
                    ContinuedLikesCrawl.likes.paging = likess == null? likes.likes.paging: likess.paging;
                }
                if (ContinuedLikesCrawl.likes.paging != null)
                {
                    if (!string.IsNullOrEmpty(ContinuedLikesCrawl.likes.paging.next) && !string.IsNullOrWhiteSpace(ContinuedLikesCrawl.likes.paging.next))
                    {
                        string url = ContinuedLikesCrawl.likes.paging.next.Replace("&amp;", "&");
                        url = url.Replace("/likes?limit=1000000000", "?fields=likes.limit(1000000000)");
                        browser.GetMainFrame().LoadUrl(ContinuedLikesCrawl.likes.paging.next.Replace("&amp;","&"));
                        return;
                    }
                }

                    host.ReportSerializedLikesResult(ContinuedLikesCrawl.XmlSerializeToString()); 
            }
            catch
            {
                if(ContinuedLikesCrawl == null)
                host.ReportSerializedLikesResult("N/A");
                else
                    host.ReportSerializedLikesResult(ContinuedLikesCrawl.XmlSerializeToString());
            }
        }

        private void GetLikesFromPage(string source, string url)
        {
            try
            {
                var jsonfile = JsonConvert.DeserializeObject<FacebookGraphData>(source);
                host.ReportSerializedResult(jsonfile.XmlSerializeToString());

                //foreach (Newtonsoft.Json.Linq.JToken post in jsonfile["data"])
                //{ 

                //}
                //string likes = source;
                //if(likes.Contains(@"<div class=""_75e"""))
                //{
                //    likes = likes.Split(new string[] { @"<div class=""_75e""" }, StringSplitOptions.None)[1];
                //    likes = likes.Split(new string[] { @">" }, StringSplitOptions.None)[1];
                //}
                //else if (likes.Contains(@"likers"">"))
                //{
                //    likes = likes.Split(new string[] { @"likers"">" }, StringSplitOptions.None)[1];
                //}  
                //likes = likes.Remove(likes.IndexOf("</"));


                //HtmlDocument doc = new HtmlDocument();
                //doc.LoadHtml(source);
                //foreach (HtmlNode item in doc.DocumentNode.ChildNodes)
                //{

                //}
                //string posts = source;
                //posts = posts.Split(new string[] { @"<div class=""_5ay5"">" }, StringSplitOptions.None)[1];
                //string[] postslist = posts.Split(new string[] { @"<div></div>" }, StringSplitOptions.None);


                // Console.WriteLine(likes);
            }
            catch (Exception ex)
            {
                host.ReportSerializedResult("N/A");
            }
        }



        public override void ShutDown()
        {
            if (browser != null)
            {
                browser.Dispose();
            }

            // Clean up CEF.
            CefRuntime.Shutdown();

            //Assembly[] pids = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (Assembly a in pids)
            //{
            //    a.un
            //}
            //foreach (Process p in pids)
            //{
            //    p.Kill();
            //}
            //Finally unload the AppDomain.
            //AppDomain.Unload(AppDomain.CurrentDomain);
        }

        public override void SetAccessToken(string url)
        {
            browser.GetMainFrame().LoadUrl(url);
        }

        public override void SetCrawlerState(int state)
        {
            this.crawlerState = (CrawlerStates)state;
        }
    }

    internal class SimpleAssemblyLoader : MarshalByRefObject
    {
        public void Load(string path)
        {
            ValidatePath(path);

            Assembly.Load(path);
        }

        public void LoadFrom(string path)
        {
            ValidatePath(path);

            Assembly.LoadFrom(path);
        }

        private void ValidatePath(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!System.IO.File.Exists(path))
                throw new ArgumentException(String.Format("path \"{0}\" does not exist", path));
        }
    }

    internal class DemoCefApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
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
                                                            
        public DemoCefClient(int windowWidth, int windowHeight)
        {
            _renderHandler = new DemoCefRenderHandler(windowWidth, windowHeight);
            _loadHandler = new DemoCefLoadHandler();
            _requestHandler = new DemoRequestHandler();
            _lifeSpanHandler = new DemoCefLifeSpanHandler();
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
        internal event Action<string,string> OnGotSourceFromLoadEnd = delegate { };

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
            // A single CefBrowser instance can handle multiple requests
            //   for a single URL if there are frames (i.e. <FRAME>, <IFRAME>).
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
            var bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppRgb, buffer);
            bitmap.Save("LastOnPaint.png", ImageFormat.Png);
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser)
        {
        }
    }
}
