namespace Xilium.CefGlue.Client
{
    using Organiser.Common.Classes;
    using SocialOrganizer.Models;
    using System;
    using System.IO;
    using System.Windows.Forms;
    using CefGlue;
    using WindowsForms;

    internal sealed class DemoApp : CefApp
    {
        public DemoApp() : base()
        {
            _renderProcessHandler = new DemoCefRenderProcessHandler();
        }
        private readonly DemoCefRenderProcessHandler _renderProcessHandler; 

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {

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

            //Disables the DirectWrite font rendering system on windows.
            //Possibly useful when experiencing blury fonts.
            //settings.CefCommandLineArgs.Add("disable-direct-write", "1");

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
            //--allow-cross-origin-auth-promp
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

            //commandLine.AppendArgument("disable-system-flash");
            //commandLine.AppendArgument("disable-bundled-ppapi-flash");
            //commandLine.AppendArgument("disable-flash-3d");
            //commandLine.AppendArgument("disable-flash-stage3d");
            //commandLine.AppendArgument("disable-flash-stage3d");
            
        }
    }



    class MyPluginVisitor : CefWebPluginInfoVisitor
    {

        protected override bool Visit(CefWebPluginInfo info, int count, int total)
        {
            MessageBox.Show("yo");
           return false;
        }
    }

    public class DemoCefRenderProcessHandler : CefRenderProcessHandler
    {
        bool hasToInject;
        PersonData profile;
        bool isTumblr;
        int tumblrcounter;

        public override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigation_type, bool isRedirect)
        {
            //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
            //headers.Add("DNT:", "1");
            //request.SetHeaderMap(headers);
            return base.OnBeforeNavigation(browser, frame, request, navigation_type, isRedirect);
        }

        public static CefV8Value val;

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            MessageBox.Show("yo");
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
            base.OnContextCreated(browser, frame, context);

            //CefV8Value obyect = context.GetGlobal();
            //CefV8Value str = CefV8Value.CreateString("My Value!");
            //obyect.SetValue("myval", str, CefV8PropertyAttribute.None);

            // CefV8Value.CreateObject(myV8Accesor);
            //obyect.SetValue("myvalue", CefV8AccessControl.Default, CefV8PropertyAttribute.None);

            //obyect.SetValue("register", CefV8Value.CreateFunction("register", myCefV8Handler), CefV8PropertyAttribute.None);
        }

        MyCustomCefV8Handler myCefV8Handler = new MyCustomCefV8Handler();
       // MyV8Accessor myV8Accesor = new MyV8Accessor();


        protected override void OnWebKitInitialized()
        {

            base.OnWebKitInitialized();

            //var nativeFunction =
            //                    @"var test;
            //                    if(!test)
            //                        test = {};
            //                    (function(){
            //                        test.myfunc = function() {
            //                            native function myfunc();
            //                            return myfunc();
            //                         }
            //                    })();";

            var nativeFunction = @"nativeImplementation = function(onSuccess) {

                native function MyNativeFunction(onSuccess);

                return MyNativeFunction(onSuccess);

            };";

            CefRuntime.RegisterExtension("myExtension", nativeFunction, myCefV8Handler);
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
            if(name == "MyNativeFunction")
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
