using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Organiser.Common.Browser
{

    public class DemoCefApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            // Debugger.Launch();
            //commandLine.AppendSwitch("disable-gpu", "1");
            //commandLine.AppendSwitch("disable-gpu-compositing", "1");
            //commandLine.AppendSwitch("enable-begin-frame-scheduling", "1");
            //commandLine.AppendSwitch("disable-gpu-vsync", "1");
            //if (string.IsNullOrEmpty(processType))
            //{
            //    commandLine.AppendSwitch("disable-gpu");
            //    commandLine.AppendSwitch("disable-gpu-compositing");
            //    commandLine.AppendSwitch("enable-begin-frame-scheduling");
            //    commandLine.AppendSwitch("disable-smooth-scrolling");
            //}
            if (GloableProfData.PData != null && !string.IsNullOrEmpty(GloableProfData.PData.ProxyIP) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyPort))
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

    public class DemoCefClient : CefClient
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

    public class DemoCefLifeSpanHandler : CefLifeSpanHandler
    {
        public event Action<CefBrowser> OnAfterBrowserCreated = delegate { };
        protected override void OnAfterCreated(CefBrowser browser)
        {
            OnAfterBrowserCreated(browser);
        }
    }

    public class DemoRequestHandler : CefRequestHandler
    {
        //    protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        //    {
        //        //if (request.Url.Contains("https://fbcdn-photos-b-a.akamaihd.net/"))
        //        //{
        //        //    try
        //        //    {
        //        //        foreach (var item in request.GetHeaderMap())
        //        //        {

        //        //        }
        //        //        foreach (var item in request.GetHeaderMap().AllKeys)
        //        //        {
        //        //            foreach (var item1 in request.GetHeaderMap().GetValues(item))
        //        //            {

        //        //            }

        //        //        }
        //        //        foreach (var item in request.GetHeaderMap().Keys)
        //        //        {

        //        //        }
        //        //    }
        //        //    catch { }
        //        //    Debugger.Break();
        //        //}
        //        //try
        //        //{
        //        //    foreach (var item in request.GetHeaderMap())
        //        //    {

        //        //    }
        //        //    foreach (var item in request.GetHeaderMap().AllKeys)
        //        //    {
        //        //        foreach (var item1 in request.GetHeaderMap().GetValues(item))
        //        //        {

        //        //        }

        //        //    }
        //        //    foreach (var item in request.GetHeaderMap().Keys)
        //        //    {

        //        //    }

        //        //    foreach (var pda in request.PostData.GetElements())
        //        //    {

        //        //    }
        //        //}
        //        //catch { }
        //        return base.OnBeforeResourceLoad(browser, frame, request);
        //    }
        //protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefRequestCallback callback)
        //{
        //    if (!BrowserSettimgs.DoNotTrackEnabled)
        //    {
        //        var headers = request.GetHeaderMap();
        //        headers.Add("DNT", "1");
        //        request.SetHeaderMap(headers);

        //        browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", browser.GetMainFrame().Url, 0);
        //    }
        //    browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", browser.GetMainFrame().Url, 0);

        //    return CefReturnValue.Continue;
        //}
        //protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        //{
        //    if (!BrowserSettimgs.DoNotTrackEnabled)
        //    {
        //        var headers = request.GetHeaderMap();
        //        headers.Add("DNT", "1");
        //        request.SetHeaderMap(headers);

        //        frame.ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", request.Url, 0);
        //    }

        //    frame.ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", browser.GetMainFrame().Url, 0);

        //    return base.OnBeforeResourceLoad(browser, frame, request);
        //}

        protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefRequestCallback callback)
        {
            if (!BrowserSettimgs.DoNotTrackEnabled)
            {
                var headers = request.GetHeaderMap();
                headers.Add("DNT", "1");
                request.SetHeaderMap(headers);

                frame.ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", request.Url, 0);
            }

            frame.ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", browser.GetMainFrame().Url, 0);

            return base.OnBeforeResourceLoad(browser, frame, request, callback);
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

    public class DemoCefLoadHandler : CefLoadHandler
    {
        public event Action<string, string> OnGotSourceFromLoadEnd = delegate { };

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
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
                if (browser.GetMainFrame().Url.Contains("photos-keyword") || browser.GetMainFrame().Url.Contains("/search/videos/")
                    || browser.GetMainFrame().Url.Contains("/search/groups/"))
                {
                    maxrecursive = 3;
                    if (browser.GetMainFrame().Url.Contains("photos-keyword") || browser.GetMainFrame().Url.Contains("/search/videos/"))
                        maxrecursive = 75;
                    else if (browser.GetMainFrame().Url.Contains("/search/groups/"))
                        maxrecursive = 3;

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

                if (browser.GetMainFrame().Url.Contains("photos-keyword") || browser.GetMainFrame().Url.Contains("/search/videos/")
                || browser.GetMainFrame().Url.Contains("/search/groups/"))
                {
                    Debugger.Launch();

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

    public class SourceVisitor : Xilium.CefGlue.CefStringVisitor
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

    public class DemoCefRenderHandler : CefRenderHandler
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

        //protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        //{
        //    //throw new NotImplementedException();
        //}

        //protected override void OnScrollOffsetChanged(CefBrowser browser)
        //{
        //}

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
           // throw new NotImplementedException();
        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            return null;
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {

        }
    }
}
