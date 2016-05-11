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
            commandLine.AppendArgument("allow-file-access-from-files"); 
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

        //public override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigation_type, bool isRedirect)
        //{
        //    //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
        //    //headers.Add("DNT:", "1");
        //    //request.SetHeaderMap(headers);
        //    return base.OnBeforeNavigation(browser, frame, request, navigation_type, isRedirect);
        //}

        public static CefV8Value val;

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
            //            string js = @"
            //chromiumImp = function(onSuccess) {

            // native function MychromiumImp(onSuccess);

            //                return MychromiumImp(onSuccess);";
            //            CefRuntime.RegisterExtension("mytestExtension", js, null);

            //            string js3 = @"
            //			chrome.webRTCIPHandlingPolicy ='disable_non_proxied_udp';
            //";
            //            CefRuntime.RegisterExtension("testExtension", js3, null);
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

            // Define the extension contents.
    //        string extensionCode =
    //          "var test;"+
    //"if (!test)"+
    //"  test = {};"+
    //"(function() {"+
    //"  test.myval = 'My Value!';"+
    //"})();";

    //        // Register the extension.
    //        CefRuntime.RegisterExtension("v8/test", extensionCode, null);

            string extensionCode =
@"
(function() {
var uncaught_exception_handler = (function(define, require, requireNative, requireAsync, exports, console, privates,$Array, $Function, $JSON, $Object, $RegExp, $String, $Error) {'use strict';// Copyright 2014 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

// Handles uncaught exceptions thrown by extensions. By default this is to
// log an error message, but tests may override this behaviour.
var handler = function(message, e) {
  console.error(message);
};

/**
 * Append the error description and stack trace to |message|.
 *
 * @param {string} message - The prefix of the error message.
 * @param {Error|*} e - The thrown error object. This object is potentially
 *   unsafe, because it could be generated by an extension.
 * @param {string=} priorStackTrace - The stack trace to be appended to the
 *   error message. This stack trace must not include stack frames of |e.stack|,
 *   because both stack traces are concatenated. Overlapping stack traces will
 *   confuse extension developers.
 * @return {string} The formatted error message.
 */
function formatErrorMessage(message, e, priorStackTrace) {
  if (e)
    message += ': ' + safeErrorToString(e, false);

  var stack;
  try {
    // If the stack was set, use it.
    // |e.stack| could be void in the following common example:
    // throw 'Error message';
    stack = $String.self(e && e.stack);
        } catch (e) {}

  // If a stack is not provided, capture a stack trace.
  if (!priorStackTrace && !stack)
    stack = getStackTrace();

        stack = filterExtensionStackTrace(stack);
  if (stack)
    message += '\n' + stack;

  // If an asynchronouse stack trace was set, append it.
  if (priorStackTrace)
    message += '\n' + priorStackTrace;

  return message;
}

    function filterExtensionStackTrace(stack)
    {
        if (!stack)
            return '';
        // Remove stack frames in the stack trace that weren't associated with the
        // extension, to not confuse extension developers with internal details.
        stack = $String.split(stack, '\n');
        stack = $Array.filter(stack, function(line) {
            return $String.indexOf(line, 'chrome-extension://') >= 0;
        });
        return $Array.join(stack, '\n');
    }

    function getStackTrace()
    {
        var e = { };
  $Error.captureStackTrace(e, getStackTrace);
        return e.stack;
    }

    function getExtensionStackTrace()
    {
        return filterExtensionStackTrace(getStackTrace());
    }

    /**
     * Convert an object to a string.
     *
     * @param {Error|*} e - A thrown object (possibly user-supplied).
     * @param {boolean=} omitType - Whether to try to serialize |e.message| instead
     *   of |e.toString()|.
     * @return {string} The error message.
     */
    function safeErrorToString(e, omitType)
    {
        try
        {
            return $String.self(omitType && e.message || e);
        }
        catch (e)
        {
            // This error is exceptional and could be triggered by
            // throw {toString: function() { throw 'Haha' } };
            return '(cannot get error message)';
        }
    }

//    /**
//     * Formats the error message and invokes the error handler.
//     *
//     * @param {string} message - Error message prefix.
//     * @param {Error|*} e - Thrown object.
//     * @param {string=} priorStackTrace - Error message suffix.
//     * @see formatErrorMessage
//     */
//    exports.$set('handle', function(message, e, priorStackTrace)
//    {
//        message = formatErrorMessage(message, e, priorStackTrace);
//        handler(message, e);
//    });

//// |newHandler| A function which matches |handler|.
//exports.$set('setHandler', function(newHandler)
//    {
//        handler = newHandler;
//    });

//exports.$set('getStackTrace', getStackTrace);
//    exports.$set('getExtensionStackTrace', getExtensionStackTrace);
//    exports.$set('safeErrorToString', safeErrorToString);
})();


var messaging_utils = (function(define, require, requireNative, requireAsync, exports, console, privates,$Array, $Function, $JSON, $Object, $RegExp, $String, $Error) {'use strict';// Copyright 2014 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

// Routines used to normalize arguments to messaging functions.

function alignSendMessageArguments(args, hasOptionsArgument) {
  // Align missing (optional) function arguments with the arguments that
  // schema validation is expecting, e.g.
  //   extension.sendRequest(req)     -> extension.sendRequest(null, req)
  //   extension.sendRequest(req, cb) -> extension.sendRequest(null, req, cb)
  if (!args || !args.length)
    return null;
  var lastArg = args.length - 1;

  // responseCallback (last argument) is optional.
  var responseCallback = null;
  if (typeof args[lastArg] == 'function')
    responseCallback = args[lastArg--];

  var options = null;
  if (hasOptionsArgument && lastArg >= 1) {
    // options (third argument) is optional. It can also be ambiguous which
    // argument it should match. If there are more than two arguments remaining,
    // options is definitely present:
    if (lastArg > 1) {
      options = args[lastArg--];
    } else {
      // Exactly two arguments remaining. If the first argument is a string,
      // it should bind to targetId, and the second argument should bind to
      // request, which is required. In other words, when two arguments remain,
      // only bind options when the first argument cannot bind to targetId.
      if (!(args[0] === null || typeof args[0] == 'string'))
        options = args[lastArg--];
    }
  }

  // request (second argument) is required.
  var request = args[lastArg--];

  // targetId (first argument, extensionId in the manifest) is optional.
  var targetId = null;
  if (lastArg >= 0)
    targetId = args[lastArg--];

  if (lastArg != -1)
    return null;
  if (hasOptionsArgument)
    return [targetId, request, options, responseCallback];
  return [targetId, request, responseCallback];
}

//exports.$set('alignSendMessageArguments', alignSendMessageArguments);

})();

var Port = (function($Object, $Function, privates, cls, superclass) {'use strict';
  function Port() {
    var privateObj = $Object.create(cls.prototype);
    $Function.apply(cls, privateObj, arguments);
    privateObj.wrapper = this;
    privates(this).impl = privateObj;
  };
  if (superclass) {
    Port.prototype = Object.create(superclass.prototype);
  }
  return Port;
})();

var Event = (function($Object, $Function, privates, cls, superclass) {'use strict';
  function Event() {
    var privateObj = $Object.create(cls.prototype);
    $Function.apply(cls, privateObj, arguments);
    privateObj.wrapper = this;
    privates(this).impl = privateObj;
  };
  if (superclass) {
    Event.prototype = Object.create(superclass.prototype);
  }
  return Event;
})();


var utils = (function(define, require, requireNative, requireAsync, exports, console, privates,$Array, $Function, $JSON, $Object, $RegExp, $String, $Error) {'use strict';// Copyright 2014 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

//var createClassWrapper = requireNative('utils').createClassWrapper;
//var nativeDeepCopy = requireNative('utils').deepCopy;
//var schemaRegistry = requireNative('schema_registry');
//var CHECK = requireNative('logging').CHECK;
//var DCHECK = requireNative('logging').DCHECK;
//var WARNING = requireNative('logging').WARNING;

/**
 * An object forEach. Calls |f| with each (key, value) pair of |obj|, using
 * |self| as the target.
 * @param {Object} obj The object to iterate over.
 * @param {function} f The function to call in each iteration.
 * @param {Object} self The object to use as |this| in each function call.
 */
function forEach(obj, f, self) {
  for (var key in obj) {
    if ($Object.hasOwnProperty(obj, key))
      $Function.call(f, self, key, obj[key]);
  }
}

/**
 * Assuming |array_of_dictionaries| is structured like this:
 * [{id: 1, ... }, {id: 2, ...}, ...], you can use
 * lookup(array_of_dictionaries, 'id', 2) to get the dictionary with id == 2.
 * @param {Array<Object<?>>} array_of_dictionaries
 * @param {string} field
 * @param {?} value
 */
function lookup(array_of_dictionaries, field, value) {
  var filter = function (dict) {return dict[field] == value;};
  var matches = array_of_dictionaries.filter(filter);
  if (matches.length == 0) {
    return undefined;
  } else if (matches.length == 1) {
    return matches[0]
  } else {
    throw new Error('Failed lookup of field '' + field + '' with value '' +
                    value + ''');
        }
    }

    function loadTypeSchema(typeName, defaultSchema)
    {
        var parts = $String.split(typeName, '.');
        if (parts.length == 1)
        {
            if (defaultSchema == null)
            {
                WARNING('Trying to reference '' + typeName + '' ' +
                        'with neither namespace nor default schema.');
                return null;
            }
            var types = defaultSchema.types;
        }
        else {
            var schemaName = $Array.join($Array.slice(parts, 0, parts.length - 1), '.');
            var types = schemaRegistry.GetSchema(schemaName).types;
        }
        for (var i = 0; i < types.length; ++i)
        {
            if (types[i].id == typeName)
                return types[i];
        }
        return null;
    }

    /**
     * Takes a private class implementation |cls| and exposes a subset of its
     * methods |functions| and properties |properties| and |readonly| in a public
     * wrapper class that it returns. Within bindings code, you can access the
     * implementation from an instance of the wrapper class using
     * privates(instance).impl, and from the implementation class you can access
     * the wrapper using this.wrapper (or implInstance.wrapper if you have another
     * instance of the implementation class).
     * @param {string} name The name of the exposed wrapper class.
     * @param {Object} cls The class implementation.
     * @param {{superclass: ?Function,
     *          functions: ?Array<string>,
     *          properties: ?Array<string>,
     *          readonly: ?Array<string>}} exposed The names of properties on the
     *     implementation class to be exposed. |superclass| represents the
     *     constructor of the class to be used as the superclass of the exposed
     *     class; |functions| represents the names of functions which should be
     *     delegated to the implementation; |properties| are gettable/settable
     *     properties and |readonly| are read-only properties.
     */
    function expose(name, cls, exposed)
    {
        var publicClass = createClassWrapper(name, cls, exposed.superclass);

        if ('functions' in exposed) {
    $Array.forEach(exposed.functions, function(func) {
                publicClass.prototype[func] = function() {
                    var impl = privates(this).impl;
                    return $Function.apply(impl[func], impl, arguments);
                };
            });
        }

        if ('properties' in exposed) {
    $Array.forEach(exposed.properties, function(prop) {
      $Object.defineProperty(publicClass.prototype, prop, {
                    enumerable: true,
        get: function() {
                        return privates(this).impl[prop];
                    },
        set: function(value) {
                        var impl = privates(this).impl;
                        delete impl[prop];
                        impl[prop] = value;
                    }
                });
            });
        }

        if ('readonly' in exposed) {
    $Array.forEach(exposed.readonly, function(readonly) {
      $Object.defineProperty(publicClass.prototype, readonly, {
                    enumerable: true,
        get: function() {
                        return privates(this).impl[readonly];
                    },
      });
            });
        }

        return publicClass;
    }

    /**
     * Returns a deep copy of |value|. The copy will have no references to nested
     * values of |value|.
     */
    function deepCopy(value)
    {
        return nativeDeepCopy(value);
    }

    /**
     * Wrap an asynchronous API call to a function |func| in a promise. The
     * remaining arguments will be passed to |func|. Returns a promise that will be
     * resolved to the result passed to the callback or rejected if an error occurs
     * (if chrome.runtime.lastError is set). If there are multiple results, the
     * promise will be resolved with an array containing those results.
     *
     * For example,
     * promise(chrome.storage.get, 'a').then(function(result) {
     *   // Use result.
     * }).catch(function(error) {
     *   // Report error.message.
     * });
     */
    function promise(func)
    {
        var args = $Array.slice(arguments, 1);
        DCHECK(typeof func == 'function');
        return new Promise(function(resolve, reject) {
    args.push(function() {
      if (chrome.runtime.lastError)
        {
            reject(new Error(chrome.runtime.lastError));
            return;
        }
        if (arguments.length <= 1)
            resolve(arguments[0]);
        else
            resolve($Array.slice(arguments));
    });
    $Function.apply(func, null, args);
  });
}

//exports.$set('forEach', forEach);
//exports.$set('loadTypeSchema', loadTypeSchema);
//exports.$set('lookup', lookup);
//exports.$set('expose', expose);
//exports.$set('deepCopy', deepCopy);
//exports.$set('promise', promise);

})();

(function(define, require, requireNative, requireAsync, exports, console, privates,$Array, $Function, $JSON, $Object, $RegExp, $String, $Error) {'use strict';// Copyright 2014 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

// -----------------------------------------------------------------------------
// NOTE: If you change this file you need to touch
// extension_renderer_resources.grd to have your change take effect.
// -----------------------------------------------------------------------------

//==============================================================================
// This file contains a class that implements a subset of JSON Schema.
// See: http://www.json.com/json-schema-proposal/ for more details.
//
// The following features of JSON Schema are not implemented:
// - requires
// - unique
// - disallow
// - union types (but replaced with 'choices')
//
// The following properties are not applicable to the interface exposed by
// this class:
// - options
// - readonly
// - title
// - description
// - format
// - default
// - transient
// - hidden
//
// There are also these departures from the JSON Schema proposal:
// - function and undefined types are supported
// - null counts as 'unspecified' for optional values
// - added the 'choices' property, to allow specifying a list of possible types
//   for a value
// - by default an 'object' typed schema does not allow additional properties.
//   if present, 'additionalProperties' is to be a schema against which all
//   additional properties will be validated.
//==============================================================================

            var loadTypeSchema = require('utils').loadTypeSchema;
            var CHECK = requireNative('logging').CHECK;

            function isInstanceOfClass(instance, className) {
                while ((instance = instance.__proto__))
                {
                    if (instance.constructor.name == className)
                        return true;
                }
                return false;
            }
            

            function isOptionalValue(value) {
                return typeof(value) === 'undefined' || value === null;
            }

            function enumToString(enumValue) {
                if (enumValue.name === undefined)
                    return enumValue;

                return enumValue.name;
            }

            /**
             * Validates an instance against a schema and accumulates errors. Usage:
             *
             * var validator = new JSONSchemaValidator();
             * validator.validate(inst, schema);
             * if (validator.errors.length == 0)
             *   console.log('Valid!');
             * else
             *   console.log(validator.errors);
             *
             * The errors property contains a list of objects. Each object has two
             * properties: 'path' and 'message'. The 'path' property contains the path to
             * the key that had the problem, and the 'message' property contains a sentence
             * describing the error.
             */
            function JSONSchemaValidator() {
                this.errors = [];
                this.types = [];
            }

            JSONSchemaValidator.messages = {
                invalidEnum: 'Value must be one of: [*].',
  propertyRequired: 'Property is required.',
  unexpectedProperty: 'Unexpected property.',
  arrayMinItems: 'Array must have at least * items.',
  arrayMaxItems: 'Array must not have more than * items.',
  itemRequired: 'Item is required.',
  stringMinLength: 'String must be at least * characters long.',
  stringMaxLength: 'String must not be more than * characters long.',
  stringPattern: 'String must match the pattern: *.',
  numberFiniteNotNan: 'Value must not be *.',
  numberMinValue: 'Value must not be less than *.',
  numberMaxValue: 'Value must not be greater than *.',
  numberIntValue: 'Value must fit in a 32-bit signed integer.',
  numberMaxDecimal: 'Value must not have more than * decimal places.',
  invalidType: 'Expected '*' but got '*'.',
  invalidTypeIntegerNumber:
                'Expected 'integer' but got 'number', consider using Math.round().',
  invalidChoice: 'Value does not match any valid type choices.',
  invalidPropertyType: 'Missing property type.',
  schemaRequired: 'Schema value required.',
  unknownSchemaReference: 'Unknown schema reference: *.',
  notInstance: 'Object must be an instance of *.'
            };

            /**
             * Builds an error message. Key is the property in the |errors| object, and
             * |opt_replacements| is an array of values to replace '*' characters with.
             */
            JSONSchemaValidator.formatError = function(key, opt_replacements) {
                var message = this.messages[key];
                if (opt_replacements)
                {
                    for (var i = 0; i < opt_replacements.length; i++)
                    {
                        message = message.replace('*', opt_replacements[i]);
                    }
                }
                return message;
            };

            /**
             * Classifies a value as one of the JSON schema primitive types. Note that we
             * don't explicitly disallow 'function', because we want to allow functions in
             * the input values.
             */
            JSONSchemaValidator.getType = function(value) {
                var s = typeof value;

                if (s == 'object')
                {
                    if (value === null)
                    {
                        return 'null';
                    }
                    else if (Object.prototype.toString.call(value) == '[object Array]')
                    {
                        return 'array';
                    }
                    else if (Object.prototype.toString.call(value) ==
                             '[object ArrayBuffer]')
                    {
                        return 'binary';
                    }
                }
                else if (s == 'number')
                {
                    if (value % 1 == 0)
                    {
                        return 'integer';
                    }
                }

                return s;
            };

            /**
             * Add types that may be referenced by validated schemas that reference them
             * with '$ref': <typeId>. Each type must be a valid schema and define an
             * 'id' property.
             */
            JSONSchemaValidator.prototype.addTypes = function(typeOrTypeList) {
                function addType(validator, type) {
                    if (!type.id)
                        throw new Error('Attempt to addType with missing 'id' property');
                    validator.types[type.id] = type;
                }

  if (typeOrTypeList instanceof Array) {
                    for (var i = 0; i < typeOrTypeList.length; i++)
                    {
                        addType(this, typeOrTypeList[i]);
                    }
                } else {
                    addType(this, typeOrTypeList);
                }
            }

            /**
             * Returns a list of strings of the types that this schema accepts.
             */
            JSONSchemaValidator.prototype.getAllTypesForSchema = function(schema) {
                var schemaTypes = [];
                if (schema.type)
    $Array.push(schemaTypes, schema.type);
                if (schema.choices)
                {
                    for (var i = 0; i < schema.choices.length; i++)
                    {
                        var choiceTypes = this.getAllTypesForSchema(schema.choices[i]);
                        schemaTypes = $Array.concat(schemaTypes, choiceTypes);
                    }
                }
                var ref = schema['$ref'];
                if (ref) {
                    var type = this.getOrAddType(ref);
                    CHECK(type, 'Could not find type ' + ref);
                    schemaTypes = $Array.concat(schemaTypes, this.getAllTypesForSchema(type));
                }
                return schemaTypes;
            };

            JSONSchemaValidator.prototype.getOrAddType = function(typeName) {
                if (!this.types[typeName])
                    this.types[typeName] = loadTypeSchema(typeName);
                return this.types[typeName];
            };

            /**
             * Returns true if |schema| would accept an argument of type |type|.
             */
            JSONSchemaValidator.prototype.isValidSchemaType = function(type, schema) {
                if (type == 'any')
                    return true;

                // TODO(kalman): I don't understand this code. How can type be 'null'?
                if (schema.optional && (type == 'null' || type == 'undefined'))
                    return true;

                var schemaTypes = this.getAllTypesForSchema(schema);
                for (var i = 0; i < schemaTypes.length; i++)
                {
                    if (schemaTypes[i] == 'any' || type == schemaTypes[i] ||
                        (type == 'integer' && schemaTypes[i] == 'number'))
                        return true;
                }

                return false;
            };

            /**
             * Returns true if there is a non-null argument that both |schema1| and
             * |schema2| would accept.
             */
            JSONSchemaValidator.prototype.checkSchemaOverlap = function(schema1, schema2) {
                var schema1Types = this.getAllTypesForSchema(schema1);
                for (var i = 0; i < schema1Types.length; i++)
                {
                    if (this.isValidSchemaType(schema1Types[i], schema2))
                        return true;
                }
                return false;
            };

            /**
             * Validates an instance against a schema. The instance can be any JavaScript
             * value and will be validated recursively. When this method returns, the
             * |errors| property will contain a list of errors, if any.
             */
            JSONSchemaValidator.prototype.validate = function(instance, schema, opt_path) {
                var path = opt_path || '';

                if (!schema)
                {
                    this.addError(path, 'schemaRequired');
                    return;
                }

                // If this schema defines itself as reference type, save it in this.types.
                if (schema.id)
                    this.types[schema.id] = schema;

                // If the schema has an extends property, the instance must validate against
                // that schema too.
                if (schema.extends)
                    this.validate(instance, schema.extends, path);

                // If the schema has a $ref property, the instance must validate against
                // that schema too. It must be present in this.types to be referenced.
                var ref = schema['$ref'];
                if (ref) {
                    if (!this.getOrAddType(ref))
                        this.addError(path, 'unknownSchemaReference', [ref ]);
                    else
                        this.validate(instance, this.getOrAddType(ref), path)
                }

                // If the schema has a choices property, the instance must validate against at
                // least one of the items in that array.
                if (schema.choices)
                {
                    this.validateChoices(instance, schema, path);
                    return;
                }

                // If the schema has an enum property, the instance must be one of those
                // values.
                if (schema.enum) {
    if (!this.validateEnum(instance, schema, path))
      return;
  }

  if (schema.type && schema.type != 'any') {
    if (!this.validateType(instance, schema, path))
      return;

    // Type-specific validation.
    switch (schema.type) {
      case 'object':
        this.validateObject(instance, schema, path);
        break;
      case 'array':
        this.validateArray(instance, schema, path);
        break;
      case 'string':
        this.validateString(instance, schema, path);
        break;
      case 'number':
      case 'integer':
        this.validateNumber(instance, schema, path);
        break;
    }
    }
};

/**
 * Validates an instance against a choices schema. The instance must match at
 * least one of the provided choices.
 */
JSONSchemaValidator.prototype.validateChoices =
    function(instance, schema, path)
{
    var originalErrors = this.errors;

    for (var i = 0; i < schema.choices.length; i++)
    {
        this.errors = [];
        this.validate(instance, schema.choices[i], path);
        if (this.errors.length == 0)
        {
            this.errors = originalErrors;
            return;
        }
    }

    this.errors = originalErrors;
    this.addError(path, 'invalidChoice');
};

/**
 * Validates an instance against a schema with an enum type. Populates the
 * |errors| property, and returns a boolean indicating whether the instance
 * validates.
 */
JSONSchemaValidator.prototype.validateEnum = function(instance, schema, path)
{
    for (var i = 0; i < schema.enum.length; i++) {
    if (instance === enumToString(schema.enum[i]))
      return true;
  }

  this.addError(path, 'invalidEnum',
                [schema.enum.map(enumToString).join(', ')]);
  return false;
};

/**
 * Validates an instance against an object schema and populates the errors
 * property.
 */
JSONSchemaValidator.prototype.validateObject =
    function(instance, schema, path)
{
    if (schema.properties)
    {
    for (var prop in schema.properties)
        {
            // It is common in JavaScript to add properties to Object.prototype. This
            // check prevents such additions from being interpreted as required
            // schema properties.
            // TODO(aa): If it ever turns out that we actually want this to work,
            // there are other checks we could put here, like requiring that schema
            // properties be objects that have a 'type' property.
            if (!$Object.hasOwnProperty(schema.properties, prop))
        continue;

            var propPath = path ? path + '.' + prop : prop;
            if (schema.properties[prop] == undefined)
            {
                this.addError(propPath, 'invalidPropertyType');
            }
            else if (prop in instance && !isOptionalValue(instance[prop])) {
                this.validate(instance[prop], schema.properties[prop], propPath);
            } else if (!schema.properties[prop].optional)
            {
                this.addError(propPath, 'propertyRequired');
            }
        }
    }

    // If 'instanceof' property is set, check that this object inherits from
    // the specified constructor (function).
    if (schema.isInstanceOf)
    {
        if (!isInstanceOfClass(instance, schema.isInstanceOf))
            this.addError(propPath, 'notInstance', [schema.isInstanceOf]);
    }

    // Exit early from additional property check if 'type':'any' is defined.
    if (schema.additionalProperties &&
        schema.additionalProperties.type &&
        schema.additionalProperties.type == 'any')
    {
        return;
    }

  // By default, additional properties are not allowed on instance objects. This
  // can be overridden by setting the additionalProperties property to a schema
  // which any additional properties must validate against.
  for (var prop in instance)
    {
        if (schema.properties && prop in schema.properties)
      continue;

        // Any properties inherited through the prototype are ignored.
        if (!$Object.hasOwnProperty(instance, prop))
      continue;

        var propPath = path ? path + '.' + prop : prop;
        if (schema.additionalProperties)
            this.validate(instance[prop], schema.additionalProperties, propPath);
        else
            this.addError(propPath, 'unexpectedProperty');
    }
};

/**
 * Validates an instance against an array schema and populates the errors
 * property.
 */
JSONSchemaValidator.prototype.validateArray = function(instance, schema, path)
{
    var typeOfItems = JSONSchemaValidator.getType(schema.items);

    if (typeOfItems == 'object')
    {
        if (schema.minItems && instance.length < schema.minItems)
        {
            this.addError(path, 'arrayMinItems', [schema.minItems]);
        }

        if (typeof schema.maxItems != 'undefined' &&
            instance.length > schema.maxItems)
        {
            this.addError(path, 'arrayMaxItems', [schema.maxItems]);
        }

        // If the items property is a single schema, each item in the array must
        // have that schema.
        for (var i = 0; i < instance.length; i++)
        {
            this.validate(instance[i], schema.items, path + '.' + i);
        }
    }
    else if (typeOfItems == 'array')
    {
        // If the items property is an array of schemas, each item in the array must
        // validate against the corresponding schema.
        for (var i = 0; i < schema.items.length; i++)
        {
            var itemPath = path ? path + '.' + i : String(i);
            if (i in instance && !isOptionalValue(instance[i])) {
            this.validate(instance[i], schema.items[i], itemPath);
        } else if (!schema.items[i].optional)
        {
            this.addError(itemPath, 'itemRequired');
        }
    }

    if (schema.additionalProperties)
    {
        for (var i = schema.items.length; i < instance.length; i++)
        {
            var itemPath = path ? path + '.' + i : String(i);
            this.validate(instance[i], schema.additionalProperties, itemPath);
        }
    }
    else {
        if (instance.length > schema.items.length)
        {
            this.addError(path, 'arrayMaxItems', [schema.items.length]);
        }
    }
}
};

/**
 * Validates a string and populates the errors property.
 */
JSONSchemaValidator.prototype.validateString =
    function(instance, schema, path)
{
    if (schema.minLength && instance.length < schema.minLength)
        this.addError(path, 'stringMinLength', [schema.minLength]);

    if (schema.maxLength && instance.length > schema.maxLength)
        this.addError(path, 'stringMaxLength', [schema.maxLength]);

    if (schema.pattern && !schema.pattern.test(instance))
        this.addError(path, 'stringPattern', [schema.pattern]);
};

/**
 * Validates a number and populates the errors property. The instance is
 * assumed to be a number.
 */
JSONSchemaValidator.prototype.validateNumber =
    function(instance, schema, path)
{
    // Forbid NaN, +Infinity, and -Infinity.  Our APIs don't use them, and
    // JSON serialization encodes them as 'null'.  Re-evaluate supporting
    // them if we add an API that could reasonably take them as a parameter.
    if (isNaN(instance) ||
        instance == Number.POSITIVE_INFINITY ||
        instance == Number.NEGATIVE_INFINITY)
        this.addError(path, 'numberFiniteNotNan', [instance]);

    if (schema.minimum !== undefined && instance < schema.minimum)
        this.addError(path, 'numberMinValue', [schema.minimum]);

    if (schema.maximum !== undefined && instance > schema.maximum)
        this.addError(path, 'numberMaxValue', [schema.maximum]);

    // Check for integer values outside of -2^31..2^31-1.
    if (schema.type === 'integer' && (instance | 0) !== instance)
        this.addError(path, 'numberIntValue', []);

    if (schema.maxDecimal && instance * Math.pow(10, schema.maxDecimal) % 1)
        this.addError(path, 'numberMaxDecimal', [schema.maxDecimal]);
};

/**
 * Validates the primitive type of an instance and populates the errors
 * property. Returns true if the instance validates, false otherwise.
 */
JSONSchemaValidator.prototype.validateType = function(instance, schema, path)
{
    var actualType = JSONSchemaValidator.getType(instance);
    if (schema.type == actualType ||
        (schema.type == 'number' && actualType == 'integer'))
    {
        return true;
    }
    else if (schema.type == 'integer' && actualType == 'number')
    {
        this.addError(path, 'invalidTypeIntegerNumber');
        return false;
    }
    else {
        this.addError(path, 'invalidType', [schema.type, actualType]);
        return false;
    }
};

/**
 * Adds an error message. |key| is an index into the |messages| object.
 * |replacements| is an array of values to replace '*' characters in the
 * message.
 */
JSONSchemaValidator.prototype.addError = function(path, key, replacements)
{
  $Array.push(this.errors, {
        path: path,
    message: JSONSchemaValidator.formatError(key, replacements)
  });
};

/**
 * Resets errors to an empty list so you can call 'validate' again.
 */
JSONSchemaValidator.prototype.resetErrors = function()
{
    this.errors = [];
};

//exports.$set('JSONSchemaValidator', JSONSchemaValidator);

})();

})();";


            // Register the extension.
//            CefRuntime.RegisterExtension("v8/test",
//                @"(function() {

//chrome.webRTCMultipleRoutesEnabled= false;

//        })();", null);

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
