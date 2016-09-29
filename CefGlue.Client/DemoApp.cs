namespace Xilium.CefGlue.Client
{
    using Organiser.Common.Classes;
    using SocialOrganizer.Models;
    using System;
    using System.IO;
    using System.Windows.Forms;
    using CefGlue;
    using WindowsForms;
    using System.Diagnostics;
    using Wrapper;
    using System.Threading;
    using System.Reflection;
    internal sealed class DemoApp : CefApp
    {
        //public static CefMessageRouterBrowserSide BrowserMessageRouter { get; set; }

        public DemoApp() : base()
        {
            _renderProcessHandler = new DemoCefRenderProcessHandler();
        }
        //private CefBrowserProcessHandler _browserProcessHandler = new DemoBrowserProcessHandler();
        //protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        //{
        //    return _browserProcessHandler;
        //}

        private readonly DemoCefRenderProcessHandler _renderProcessHandler;
        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //var path = AppDomain.CurrentDomain.BaseDirectory;
            //path = Path.GetDirectoryName(path);

            //commandLine.AppendSwitch("resources-dir-path", path);
            //commandLine.AppendSwitch("locales-dir-path", Path.Combine(path, "locales"));

            //commandLine.AppendArgument("dns-prefetch-disable");

            //commandLine.AppendArgument("disable-gpu");
            //commandLine.AppendArgument("disable-gpu-compositing");
            //commandLine.AppendArgument("enable-begin-frame-scheduling");
            //commandLine.AppendArgument("disable-accelerated-2d-canvas");

            //commandLine.AppendArgument("--enable-npapi");
            // CefRuntime.AddWebPluginDirectory(@"C:\Windows\System32\Macromed\Flash\");
            //CefRuntime.AddWebPluginPath(@"C:\Windows\System32\Macromed\Flash\pepflashplayer64_18_0_0_209.dll");
            //CefRuntime.RefreshWebPlugins();

            //if (!System.IO.File.Exists("C:\\file.txt"))
            //  commandLine.AppendSwitch("proxy-server", "23.94.20.30:80");
            //else
            //   commandLine.AppendSwitch("proxy-server", "192.171.233.149:80");

            //System.IO.File.Create("C:\\file.txt");

            //commandLine.AppendArgument("disable-media-stream");
            //commandLine.AppendSwitch("media.peerconnection.enabled", "false");
            //commandLine.AppendArgument("disable-webrtc");
            //commandLine.AppendSwitch("disable-webrtc-encryption");
            //commandLine.AppendArgument("disable-webrtc-hw-decoding");
            //commandLine.AppendArgument("disable-webrtc-hw-encoding");
            //commandLine.AppendArgument("disable-webrtc-hw-encoding");
            //commandLine.AppendSwitch("enable_webrtc", "0");
            //commandLine.AppendSwitch("ENABLE_WEBRTC", "0");
            //commandLine.AppendSwitch("enable-media-stream", "0");
            //commandLine.AppendSwitch("multiple_routes_enabled", "0");


            //settings.CefCommandLineArgs.Add("renderer-process-limit", "1");
            //settings.CefCommandLineArgs.Add("renderer-startup-dialog", "1");
            //settings.CefCommandLineArgs.Add("enable-media-stream", "1"); //Enable WebRTC
            //settings.CefCommandLineArgs.Add("no-proxy-server", "1"); //Don't use a proxy server, always make direct connections. Overrides any other proxy server flags that are passed.
            //settings.CefCommandLineArgs.Add("debug-plugin-loading", "1"); //Dumps extra logging about plugin loading to the log file.
            //settings.CefCommandLineArgs.Add("disable-plugins-discovery", "1"); //Disable discovering third-party plugins. Effectively loading only ones shipped with the browser plus third-party ones as specified by --extra-plugin-dir and --load-plugin switches
            // commandLine.AppendSwitch("enable-npapi", "0"); //Enable NPAPI plugs which were disabled by default in Chromium 43 (NPAPI will be removed completely in Chromium 45)
            //  commandLine.AppendSwitch("enable-system-flash", "0"); //Automatically discovered and load a system-wide installation of Pepper Flash.

            //settings.CefCommandLineArgs.Add("ppapi-flash-path", @"C:\WINDOWS\SysWOW64\Macromed\Flash\pepflashplayer32_18_0_0_209.dll"); //Load a specific pepper flash version (Step 1 of 2)
            //settings.CefCommandLineArgs.Add("ppapi-flash-version", "18.0.0.209"); //Load a specific pepper flash version (Step 2 of 2)

            //NOTE: For OSR best performance you should run with GPU disabled:
            // `--disable-gpu --disable-gpu-compositing --enable-begin-frame-scheduling`
            // (you'll loose WebGL support but gain increased FPS and reduced CPU usage).
            // http://magpcss.org/ceforum/viewtopic.php?f=6&t=13271#p27075
            //commandLine.AppendSwitch("disable-gpu", "1");
            //commandLine.AppendSwitch("disable-gpu-compositing", "1");
            //commandLine.AppendSwitch("enable-begin-frame-scheduling", "1");
            //commandLine.AppendSwitch("disable-gpu-vsync", "1");
            //commandLine.AppendArgument("in-process-gpu");

            //Disables the DirectWrite font rendering system on windows.
            //Possibly useful when experiencing blury fonts.
            //commandLine.AppendSwitch("disable-direct-write", "1");

            // Set command line arguments to enable best performance when off screen rendering
            //https://bitbucket.org/chromiumembedded/cef/commits/e3c1d8632eb43c1c2793d71639f3f5695696a5e8
            //settings.SetOffScreenRenderingBestPerformanceArgs();

            //commandLine.AppendArgument("disable-web-security");
            //commandLine.AppendArgument("allow-file-access-from-files"); 
            //commandLine.AppendArgument("allow-cross-origin-auth-promp");
            //commandLine.AppendSwitch("disable-blink-features", "GetUserMedia");
            //commandLine.AppendSwitch("disable-blink-features", "AudioOutputDevices");
            //commandLine.AppendSwitch("disable-blink-features", "MediaStreamTrack");
            //commandLine.AppendArgument("disable-blink-features"); 
            //commandLine.AppendSwitch("origin-when-crossorigin", "default");
            //string[] args = commandLine.GetArguments();
            //commandLine.AppendArgument("allow-cross-origin-auth-promp");
            //--allow-cross-origin-auth-prom
           // commandLine.AppendSwitch("proxy-server", "[2604:180:2:631:3618:969:4305:c96b]" + ":" + 54343);
            if (GloableProfData.PData != null && !string.IsNullOrEmpty(GloableProfData.PData.ProxyIP) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyIP))
            {
                try
                {
                    commandLine.AppendSwitch("proxy-server", GloableProfData.PData.ProxyIP+":"+GloableProfData.PData.ProxyPort);
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("failed to set proxy");
                }
            }


            //CefRuntime.RefreshWebPlugins();

            //var flashPath = AppDomain.CurrentDomain.BaseDirectory + "\\PepperFlash\\pepflashplayer.dll";
            //flashPath = flashPath.Replace("\\\\", "\\");
            ////var flashPath = @"C:\Windows\System32\Macromed\Flash\FlashUtil64_22_0_0_209_Plugin.dll";
            //commandLine.AppendSwitch("ppapi-flash-path", flashPath);
            //commandLine.AppendSwitch("ppapi-flash-version", "22.0.0.192");
            //commandLine.AppendSwitch("plugin-policy", "allow");
            //commandLine.AppendSwitch("ppapi-flash-args", "enable_hw_video_decode=1,enable_stagevideo_auto=0,enable_trace_to_console=0");
            ////commandLine.AppendArgument("enable-npapi");
            //commandLine.AppendArgument("ppapi-in-process");
            //commandLine.AppendArgument("enable-nacl");
            //commandLine.AppendArgument("disable-flash-3d");
            //commandLine.AppendArgument("disable-flash-stage3d");
            //commandLine.AppendArgument("safe-plugins");



            //commandLine.AppendArgument("enable-native-notifications");
            //commandLine.AppendArgument("enable-web-notification-custom-layouts");


            //commandLine.AppendArgument("disable-renderer-backgrounding");
            //commandLine.AppendArgument("disable-win32k-renderer-lockdown");
            //commandLine.AppendArgument("disable-desktop-capture-picker-new-ui");
            //commandLine.AppendArgument("enable-devtools-experiments");




            //var flashPath = @"C:\Windows\SysWOW64\Macromed\Flash\NPSWF32_22_0_0_209.dll";

            //commandLine.AppendArgument("enable-system-flash");

            //commandLine.AppendArgument("disable-bundled-ppapi-flash");
            // commandLine.AppendArgument("ppapi-out-of-process");
            //  commandLine.AppendArgument("disable-system-flash");
            // commandLine.AppendSwitch("ppapi-flash-args", "enable_hw_video_decode=1");
            // commandLine.AppendArgument("ppapi-flash-args=enable_hw_video_decode=1");
            //// commandLine.AppendArgument("always-authorize-plugins");

            // //commandLine.AppendArgument("disable-flash-stage3d");

            //CefRuntime.RefreshWebPlugins();

        }
    }

    internal sealed class DemoBrowserProcessHandler : CefBrowserProcessHandler
    {
        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            //var exePath = AppDomain.CurrentDomain.BaseDirectory + "\\BrowserAndFeatures.exe";
            //exePath = exePath.Replace("\\\\", "\\");
            //commandLine.SetProgram(exePath);

            //Console.WriteLine("AppendExtraCommandLineSwitches: {0}", commandLine);
            //Console.WriteLine(" Program == {0}", commandLine.GetProgram());

            //// .NET in Windows treat assemblies as native images, so no any magic required.
            //// Mono on any platform usually located far away from entry assembly, so we want prepare command line to call it correctly.
            //if (Type.GetType("Mono.Runtime") != null)
            //{
            //    if (!commandLine.HasSwitch("cefglue"))
            //    {
            //        var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
            //        commandLine.SetProgram(path);

            //        var mono = CefRuntime.Platform == CefRuntimePlatform.Linux ? "/usr/bin/mono" : @"C:\Program Files\Mono-2.10.8\bin\monow.exe";
            //        commandLine.PrependArgument(mono);

            //        commandLine.AppendSwitch("cefglue", "w");
            //    }
            //}

            //Console.WriteLine("  -> {0}", commandLine);
        }
    }

    public class DemoMessageRouterHandler : CefMessageRouterBrowserSide.Handler
    {
        //public override bool OnQuery(CefBrowser browser, CefFrame frame, long queryId, string request, bool persistent, CefMessageRouterBrowserSide.Callback callback)
        //{
        //    if (request == "wait5")
        //    {
        //        new Thread(() =>
        //        {
        //            Thread.Sleep(5000);
        //            callback.Success("success! responded after 5 sec timeout."); // TODO: at this place crash can occurs, if application closed
        //        }).Start();
        //        return true;
        //    }

        //    if (request == "wait5f")
        //    {
        //        new Thread(() =>
        //        {
        //            Thread.Sleep(5000);
        //            callback.Failure(12345, "success! responded after 5 sec timeout. responded as failure.");
        //        }).Start();
        //        return true;
        //    }

        //    if (request == "wait30")
        //    {
        //        new Thread(() =>
        //        {
        //            Thread.Sleep(30000);
        //            callback.Success("success! responded after 30 sec timeout.");
        //        }).Start();
        //        return true;
        //    }

        //    if (request == "noanswer")
        //    {
        //        return true;
        //    }

        //    var chars = request.ToCharArray();
        //    Array.Reverse(chars);
        //    var response = new string(chars);
        //    callback.Success(response);
        //    return true;
        //}

        public override void OnQueryCanceled(CefBrowser browser, CefFrame frame, long queryId)
        {
        }
    }

    //internal sealed class DemoAppSchemeHandlerFactory : CefSchemeHandlerFactory
    //{
    //    protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
    //    {
    //        return new DumpRequestResourceHandler();
    //    }
    //}
    internal sealed class DumpRequestResourceHandler : CefResourceHandler
    {
        private static int _requestNo;

        private byte[] responseData;
        private int pos;


        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            //var requestNo = Interlocked.Increment(ref _requestNo);

            //var response = new StringBuilder();

            //response.AppendFormat("<pre>\n");
            //response.AppendFormat("Requests processed by DemoAppResourceHandler: {0}\n", requestNo);

            //response.AppendFormat("Method: {0}\n", request.Method);
            //response.AppendFormat("URL: {0}\n", request.Url);

            //response.AppendLine();
            //response.AppendLine("Headers:");
            //var headers = request.GetHeaderMap();
            //foreach (string key in headers)
            //{
            //    foreach (var value in headers.GetValues(key))
            //    {
            //        response.AppendFormat("{0}: {1}\n", key, value);
            //    }
            //}
            //response.AppendLine();

            //response.AppendFormat("</pre>\n");

            //responseData = Encoding.UTF8.GetBytes(response.ToString());

            callback.Continue();
            return true;
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            //response.MimeType = "text/html";
            //response.Status = 200;
            //response.StatusText = "OK, hello from handler!";

            //var headers = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            //headers.Add("Cache-Control", "private");
            //response.SetHeaderMap(headers);

            responseLength = responseData.LongLength;
            redirectUrl = null;
        }

        protected override bool ReadResponse(Stream response, int bytesToRead, out int bytesRead, CefCallback callback)
        {
            if (bytesToRead == 0 || pos >= responseData.Length)
            {
                bytesRead = 0;
                return false;
            }
            else
            {
                response.Write(responseData, pos, bytesToRead);
                pos += bytesToRead;
                bytesRead = bytesToRead;
                return true;
            }
        }

        protected override bool CanGetCookie(CefCookie cookie)
        {
            return false;
        }

        protected override bool CanSetCookie(CefCookie cookie)
        {
            return false;
        }

        protected override void Cancel()
        {
        }
    }



    class MyPluginVisitor : CefWebPluginInfoVisitor
    {

        protected override bool Visit(CefWebPluginInfo info, int count, int total)
        {
           // MessageBox.Show("yo");
           return false;
        }
    }

    public class DemoCefRenderProcessHandler : CefRenderProcessHandler
    {
        bool hasToInject;
        PersonData profile;
        bool isTumblr;
        int tumblrcounter;

        //public override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigation_type, bool isRedirect)
        //{
        //    //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
        //    //headers.Add("DNT:", "1");
        //    //request.SetHeaderMap(headers);
        //    return base.OnBeforeNavigation(browser, frame, request, navigation_type, isRedirect);
        //}

       // public static CefV8Value val;
        //internal static bool DumpProcessMessages { get; private set; }
       // internal CefMessageRouterRendererSide MessageRouter { get; private set; }

        public DemoCefRenderProcessHandler()
        {
           // MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
           // MessageBox.Show("yo");
            #region for injection (unused)
            if (message.Name == "NavChange")
            {
                hasToInject = false;
            }
            else if (message.Name.Contains("{||}"))
            {
                this.isTumblr = false;
                tumblrcounter = 0;

                string[] splitPersonDatas = message.Name.Split(new string[] { "{||}" }, StringSplitOptions.None);
                string path = splitPersonDatas[0];
                string isTheMulti = splitPersonDatas[1];
                string selectedMulti = splitPersonDatas[2];
                string isTumblr = splitPersonDatas[3];
                if (isTumblr == "true")
                    this.isTumblr = true;

                profile = new PersonData();

                if (isTheMulti == "false")
                {
                    profile = MyFilesDatabase.SetProfileFromini(path);
                }
                else
                {
                    profile = MyFilesDatabase.GetSubProjectPersonData(selectedMulti);
                }

                hasToInject = true;
            }

            //var handled = MessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
            //if (handled) return true;

            // BrowserCntrl.OnFinishedExecute("");

            return false;
        }


        protected override void OnFocusedNodeChanged(CefBrowser browser, CefFrame frame, CefDomNode node)
        {
            //string jsToExecute = "var all = document.getElementsByTagName('*');" +
            //                      "for (var i=0, max=all.length; i < max; i++) {" +
            //                        "if(all[i].tagName.indexOf('INPUT') > -1){" +
            //                            "for (var j = 0; j < all[i].attributes.length; j++) {" +
            //                                "var attrib = all[i].attributes[j]; " +
            //                                "if(attrib.value.indexOf('password') > -1){" +
            //                                     "all[i].value=123456; break;" +
            //                                 "}" +
            //                            "}" +
            //                        "}" +
            //                      "}";
            //frame.ExecuteJavaScript(jsToExecute, frame.Url, 0);
            //if (!hasToInject) return;
            //if (node == null) return;
            //if (!node.IsFormControlElement) return;
            //try
            //{
            //    foreach (var item in node.GetAttributes())
            //    {
            //        string val = item.Value;
            //        if (val.Contains("first"))
            //        {
            //            Clipboard.SetText(profile.FirstName);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            break;
            //        }
            //        else if (val.Contains("last"))
            //        {
            //            Clipboard.SetText(profile.LastName);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            break;
            //        }
            //        else if (val.Contains("mail"))
            //        {
            //            Clipboard.SetText(profile.Email);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            if (isTumblr) tumblrcounter++;
            //            break;
            //        }
            //        else if (val.Contains("user"))
            //        {
            //            Clipboard.SetText(profile.Username);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            if (isTumblr) tumblrcounter++;
            //            break;
            //        }
            //        else if (val.Contains("phone"))
            //        {
            //            Clipboard.SetText(profile.PhoneNumber);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            break;
            //        }
            //        else if (val.Contains("gender"))
            //        {
            //            Clipboard.SetText(profile.SexList[profile.CmbSelectedIndexSex]);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            break;
            //        }
            //        else if (val.Contains("day"))
            //        {
            //            Clipboard.SetText(profile.DayList[profile.CmbSelectedIndexDay]);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V); 
            //            break;
            //        }
            //        else if (val.Contains("month"))
            //        {
            //            Clipboard.SetText(profile.MonthList[profile.CmbSelectedIndexMonth]);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            break;
            //        }
            //        else if (val.Contains("year"))
            //        {
            //            Clipboard.SetText(profile.BirthdayYear.ToString());
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            break;
            //        }
            //        else if (val.Contains("pass"))
            //        {
            //            Clipboard.SetText(profile.Password);
            //            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            //            if (isTumblr) tumblrcounter++;
            //            break;
            //        }
            //    }

            //    InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);

            //    if (!node.IsFormControlElement || tumblrcounter >= 3)
            //    {
            //        hasToInject = false;
            //    }
            //}
            //catch { }

            #endregion
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
           // MessageRouter.OnContextCreated(browser, frame, context);

            // base.OnContextCreated(browser, frame, context);

            //CefV8Value obyect = context.GetGlobal();
            //CefV8Value str = CefV8Value.CreateString("My Value!");
            //obyect.SetValue("myval", str, CefV8PropertyAttribute.None);

            // CefV8Value.CreateObject(myV8Accesor);
            //obyect.SetValue("myvalue", CefV8AccessControl.Default, CefV8PropertyAttribute.None);

            //obyect.SetValue("register", CefV8Value.CreateFunction("register", myCefV8Handler), CefV8PropertyAttribute.None);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
          //  MessageRouter.OnContextReleased(browser, frame, context);
        }


        MyCustomCefV8Handler myCefV8Handler = new MyCustomCefV8Handler();
       // MyV8Accessor myV8Accesor = new MyV8Accessor();


        protected override void OnWebKitInitialized()
        {

            var nativeFunction = @"nativeImplementation = function(onSuccess) {

                native function MyNativeFunction(onSuccess);

                return MyNativeFunction(onSuccess);

            };";

            CefRuntime.RegisterExtension("myExtension", nativeFunction, myCefV8Handler);
            base.OnWebKitInitialized();

        }

        //protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        //{
        //    base.OnContextCreated(browser, frame, context);


        //}
    }

    internal class MyV8Accessor : CefV8Accessor
    {
        // Variable used for storing the value.
        string myval_;
        

        protected override bool Get(string name, CefV8Value obj, out CefV8Value returnValue, out string exception)
        {
            exception = "";
            returnValue = null;
            if (name == "myvalue")
            {
                // Return the value.
                returnValue = CefV8Value.CreateString(myval_);
                return true;
            }

            // Value does not exist.
            return false;
        }

        protected override bool Set(string name, CefV8Value obj, CefV8Value value, out string exception)
        {
            exception = "";

            if (name == "myvalue")
            {
                if (value.IsString)
                {
                    // Store the value.
                    myval_ = value.GetStringValue();
                }
                else
                {
                    // Throw an exception.
                    exception = "Invalid value type";
                }
                return true;
            }

            // Value does not exist.
            return false;
        }
    }

    public class MyCustomCefV8Handler : CefV8Handler
    {
        //public static string HighlightdHTMLText = "";
       // public static event Action<string> OnFinishedExecute = delegate { };
        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue,

            out string exception)

        {
            //Debugger.Launch();

            if (name == "MyNativeFunction")
            {
                var value = arguments[0];
                if (value.IsString)
                {
                    string dir = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempHTML");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    string file = Path.Combine(dir, "html.txt");

                    File.WriteAllText(file, value.GetStringValue());
                   

                    //OnFinishedExecute(value.GetStringValue());
                    // MessageBox.Show(value.GetStringValue());

                    
                    //var message = CefProcessMessage.Create("one");
                    //var args = message.Arguments;
                    //args.SetString(0, value.GetStringValue());

                    //var context = CefV8Context.GetCurrentContext();
                    //context.GetBrowser().SendProcessMessage(CefProcessId.Renderer, message);

                    // var taskRunner = CefTaskRunner.GetForThread(CefThreadId.UI);

                    //  var callback = arguments[0];

                    //new Thread(() =>
                    //{
                    //Sleep a bit: to test whether the app remains responsive

                    // taskRunner.PostTask(new CefCallbackTask(context, callback));

                    // }).Start();
                }
            }

            //Debugger.Launch();



            //var context = CefV8Context.GetCurrentContext();

            //var taskRunner = CefTaskRunner.GetForCurrentThread();

            //var callback = arguments[0];

            //new Thread(() =>

            //{

            //    //Sleep a bit: to test whether the app remains responsive

            //    Thread.Sleep(3000);

            //    taskRunner.PostTask(new CefCallbackTask(context, callback));

            //}).Start();



            returnValue = CefV8Value.CreateBool(true);

            exception = null;

            return true;

        }

    }

    public class CefCallbackTask : CefTask

    {
       // public static event Action<string> OnFinishedExecute = delegate { };

        private readonly CefV8Context context;

        private readonly CefV8Value callback;



        public CefCallbackTask(CefV8Context context, CefV8Value callback)

        {

            this.context = context;

            this.callback = callback;

        }



        protected override void Execute()

        {

            //var callbackArguments = CreateCallbackArguments();

            //callback.ExecuteFunctionWithContext(context, null, callbackArguments);
            //OnFinishedExecute(callback.GetStringValue());
            //BrowserCntrl.OnFinishedExecute(callback.GetStringValue());
        }



        private CefV8Value[] CreateCallbackArguments()
        {

            //var imageInBase64EncodedString = LoadImage(@"C:\hamb.jpg");



            context.Enter();



           // var imageV8String = CefV8Value.CreateString(imageInBase64EncodedString);

            var featureV8Object = CefV8Value.CreateObject(null);

            var listOfFeaturesV8Array = CefV8Value.CreateArray(1);



            featureV8Object.SetValue("name", CefV8Value.CreateString("V8"), CefV8PropertyAttribute.None);

            featureV8Object.SetValue("isEnabled", CefV8Value.CreateInt(0), CefV8PropertyAttribute.None);

            featureV8Object.SetValue("isFromJSCode", CefV8Value.CreateBool(false), CefV8PropertyAttribute.None);



            listOfFeaturesV8Array.SetValue(0, featureV8Object);

            var yo = "";

            context.Exit();



            return new CefV8Value[] { listOfFeaturesV8Array };

        }



        private string LoadImage(string fileName)
        {

            //using (var memoryStream = new MemoryStream())

            //{

            //    var image = Bitmap.FromFile(fileName);

            //    image.Save(memoryStream, ImageFormat.Png);

            //    byte[] imageBytes = memoryStream.ToArray();

            //    return Convert.ToBase64String(imageBytes);

            //}

            return "123";

        }

    }
}
