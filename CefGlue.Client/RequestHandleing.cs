using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xilium.CefGlue.WindowsForms;
using System.IO;

namespace Xilium.CefGlue.Client
{
    public class RequestHandleing : CefRequestHandler
    {
        private readonly CefWebBrowser _core;
        // public string DoNotTrack { get; set; }

        public RequestHandleing(CefWebBrowser core)
        {
            _core = core;
        }

        //protected override void OnPluginCrashed(CefBrowser browser, string pluginPath)
        //{
        //    _core.InvokeIfRequired(() => _core.OnPluginCrashed(new PluginCrashedEventArgs(pluginPath)));
        //}

        //protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        //{
        //    _core.InvokeIfRequired(() => _core.OnRenderProcessTerminated(new RenderProcessTerminatedEventArgs(status)));
        //}

        protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool isRedirect)
        {
            //DemoApp.BrowserMessageRouter.OnBeforeBrowse(browser, frame);
            //if (request.Method == "GET" || request.Method == "POST")
            //{
            //    System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
            //    headers.Add("DNT", "1");
            //    request.SetHeaderMap(headers);
            //}
            //Console.WriteLine(request.Url);
            //var headers = request.GetHeaderMap();
            // headers.Add("HTTP_DNT", "1");
            //headers.Add("DNT", "1");
            //request.SetHeaderMap(headers);
            // return false;
            //if (!BrowserSettimgs.DoNotTrackEnabled)
            //{
            //    var headers = request.GetHeaderMap();
            //    headers.Add("DNT", "1");
            //    request.SetHeaderMap(headers);

            //    browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", browser.GetMainFrame().Url, 0);
            //}
            //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", browser.GetMainFrame().Url, 0);

            ////userAgent
            //var headersua = request.GetHeaderMap();
            //var useragent = headersua["User-Agent"];
            //if (useragent != null)
            //{
            //    headersua["User-Agent"] = BrowserInit.settings.UserAgent;
            //    request.SetHeaderMap(headersua);
            //}
            ////frame.ExecuteJavaScript("window.__defineGetter__('userAgent', function () { return '" + BrowserInit.settings.UserAgent + "'; });", request.Url, 0);
            ////appVersion
            //frame.ExecuteJavaScript("window.navigator.__defineGetter__('userAgent', function () { return '" + BrowserInit.settings.UserAgent + "'; });", request.Url, 0);
            //frame.ExecuteJavaScript("window.navigator.__defineGetter__('appVersion', function () { return '5.0'; });", request.Url, 0);
            ////frame.ExecuteJavaScript("for (property in navigator) { console.log(property + ' ' + navigator[property]); }", request.Url, 0);

            //frame.ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", request.Url, 0);

            return base.OnBeforeBrowse(browser, frame, request, isRedirect);
        }
        protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefRequestCallback callback)
        {
            frame.ExecuteJavaScript("window.__defineGetter__('Notification', function () { return false; });", frame.Url, 0);

            //frame.ExecuteJavaScript("window.screen.__defineGetter__('width', function () { return 1440; });", frame.Url, 0);
            //frame.ExecuteJavaScript("window.screen.__defineGetter__('height', function () { return 900; });", frame.Url, 0);

            //frame.ExecuteJavaScript("screen.__defineGetter__('availWidth', function () { return 1440; });", frame.Url, 0);
            //frame.ExecuteJavaScript("screen.__defineGetter__('availHeight', function () { return 900; });", frame.Url, 0);

            //frame.ExecuteJavaScript("screen.__defineGetter__('width', function () { return 1440; });", frame.Url, 0);
            //frame.ExecuteJavaScript("screen.__defineGetter__('height', function () { return 900; });", frame.Url, 0);

            //frame.ExecuteJavaScript("screen.__defineGetter__('availWidth', function () { return 1440; });", frame.Url, 0);
            //frame.ExecuteJavaScript("screen.__defineGetter__('availHeight', function () { return 900; });", frame.Url, 0);
            //frame.ExecuteJavaScript("window.__defineGetter__('appVersion', function () { return '5.0'; });", browser.GetMainFrame().Url, 0);

            if (!BrowserSettimgs.DoNotTrackEnabled)
            {
                var headers = request.GetHeaderMap();
                headers.Add("DNT", "1");
                request.SetHeaderMap(headers);

                frame.ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", request.Url, 0);
            }

            ////userAgent
            if (BrowserInit.settings != null && BrowserInit.settings.UserAgent != null && BrowserInit.settings.UserAgent != BrowserSettimgs.UserAgentChrome)
            {
                var headersua = request.GetHeaderMap();
                var useragent = headersua["User-Agent"];
                if (useragent != null)
                {
                    headersua["User-Agent"] = BrowserInit.settings.UserAgent;
                    request.SetHeaderMap(headersua);
                }
                //frame.ExecuteJavaScript("window.__defineGetter__('userAgent', function () { return '" + BrowserInit.settings.UserAgent + "'; });", request.Url, 0);
                //appVersion
                frame.ExecuteJavaScript("window.navigator.__defineGetter__('userAgent', function () { return '" + BrowserSettimgs.UserAgentChrome + "'; });", request.Url, 0);
                frame.ExecuteJavaScript("window.navigator.__defineGetter__('appVersion', function () { return '5.0'; });", browser.GetMainFrame().Url, 0);
                browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('userAgent', function () { return '" + BrowserSettimgs.UserAgentChrome + "'; });", browser.GetMainFrame().Url, 0);
                browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('appVersion', function () { return '5.0'; });", browser.GetMainFrame().Url, 0);
                //frame.ExecuteJavaScript("for (property in navigator) { console.log(property + ' ' + navigator[property]); }", request.Url, 0);
            }
           //if(request.ResourceType == CefResourceType.FontResource)
           // {
           //     callback.Cancel();
           //     return CefReturnValue.Cancel;
           // }
           // frame.ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", request.Url, 0);

            return base.OnBeforeResourceLoad(browser, frame, request, callback);
        }


        //protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        //{
        //    DemoApp.BrowserMessageRouter.OnRenderProcessTerminated(browser);
        //}
        //protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        //{
        //    //frame.ExecuteJavaScript("document.createElement = null;", request.Url, 0);
        //   // frame.ExecuteJavaScript("document.charset = 'windows-1252';", request.Url, 0);
        //    //frame.ExecuteJavaScript("document.defaultCharset = 'undefined';", request.Url, 0);
        //    //            browser.GetMainFrame().ExecuteJavaScript(@"
        //    //var elements = document.getElementsByTagName('img');
        //    //for (var i=0; i<elements.length; i++) 
        //    //{
        //    //    alert(elements[i].src);
        //    //}

        //    ////var list = document.getElementsByTagName('*');
        //    //////alert(list);
        //    //////alert(list.length);
        //    ////for (var i=0; i < list.length; i++) 
        //    ////{
        //    ////list[i].style.display='block';
        //    ////    //for (var j = 0; j < list[i].attributes.length; j++) 
        //    ////    //{
        //    ////    //    var attrib = list[i].attributes[j];

        //    ////    //    alert(attrib.Name);
        //    ////    //    alert(attrib.Value);
        //    ////    //}
        //    ////}
        //    //", browser.GetMainFrame().Url, 0);
        //    //Console.WriteLine(frame.Url);
        //    //Console.WriteLine(request.Url);
        //    if (!BrowserSettimgs.DoNotTrackEnabled)
        //    {
        //        var headers = request.GetHeaderMap();
        //        headers.Add("DNT", "1");
        //        request.SetHeaderMap(headers);

        //        frame.ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", request.Url, 0);
        //    }

        //    ////userAgent
        //    if (BrowserInit.settings != null && BrowserInit.settings.UserAgent != null)
        //    {
        //        var headersua = request.GetHeaderMap();
        //        var useragent = headersua["User-Agent"];
        //        if (useragent != null)
        //        {
        //            headersua["User-Agent"] = BrowserInit.settings.UserAgent;
        //            request.SetHeaderMap(headersua);
        //        }
        //        //frame.ExecuteJavaScript("window.__defineGetter__('userAgent', function () { return '" + BrowserInit.settings.UserAgent + "'; });", request.Url, 0);
        //        //appVersion
        //        frame.ExecuteJavaScript("window.navigator.__defineGetter__('userAgent', function () { return '" + BrowserSettimgs.UserAgentChrome + "'; });", request.Url, 0);
        //        frame.ExecuteJavaScript("window.navigator.__defineGetter__('appVersion', function () { return '5.0'; });", browser.GetMainFrame().Url, 0);
        //        browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('userAgent', function () { return '" + BrowserSettimgs.UserAgentChrome + "'; });", browser.GetMainFrame().Url, 0);
        //        browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('appVersion', function () { return '5.0'; });", browser.GetMainFrame().Url, 0);
        //        //frame.ExecuteJavaScript("for (property in navigator) { console.log(property + ' ' + navigator[property]); }", request.Url, 0);
        //    }

        //    frame.ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", request.Url, 0);

        //    return base.OnBeforeResourceLoad(browser, frame, request);
        //}

        ///// <summary>
        ///// (function() { 
        ///// function g(){$("#rtc_hasMicrophone,#rtc_hasWebcam,#rtc_isDeviceEnumeration").text("false");$("#rtc_device_ids").text("n/a")} 
        ///// function h(b, c, d){ return '<span class="country-flag" style="background-image:url(/img/flags/' + c + '.png)">' + (d ? '<a href="/whois/' + b + '" title="Get IP Address Details" target="_blank">' : "") + b + (d ? "</a>" : "") + "&nbsp;&nbsp;&nbsp;</span>"}
        ///// function n(b){$.ajax({ url: "/xhr/webrtc/" + b,type: "GET",success: function(c){$("#rtc_public").append(h(b, c, !0))} })}$("#webrtc-content table").removeClass("undef"); 
        ///// var p = window.process && "object" == typeof window.process&& window.process.versions && window.process.versions["node-webkit"], e = !!navigator.webkitGetUserMedia;
        ///// if (e) var k = "undefined" !== typeof navigator.userAgent?e && navigator.mozGetUserMedia ? 0 : parseInt(navigator.userAgent.match(/ Chrom(e | ium)\/ ([0 - 9] +)\./)[2]):21;
        ///// var l = !!window.webkitRTCPeerConnection || !!window.mozRTCPeerConnection, m; m = l ? '<span class="bad">!</span> True' : '<span class="good">&#215;</span>
        ///// False';$("#rtc_isWebRTCEnabled").html(m);
        ///// $("#rtc_isAudioContextSupported").text(!!window.AudioContext || !!window.webkitAudioContext);
        ///// $("#rtc_isScreenCapturingSupported").text(e && 26 <= k && (p ? !0 : "https:" == location.protocol));
        ///// $("#rtc_isSctpDataChannelsSupported").text(!!navigator.mozGetUserMedia || e && 25 <= k);$("#rtc_isRtpDataChannelsSupported").text(e && 31 <= k);
        ///// if (window.MediaStreamTrack) { MediaStreamTrack.getSources || (MediaStreamTrack.getSources = MediaStreamTrack.getMediaDevices); window.MediaStreamTrack && MediaStreamTrack.getSources || g(); 
        ///// try { MediaStreamTrack.getSources(
        ///// function(b){ for (var c = { }, d = 0; d < b.length; d++) c[b[d].kind] = !0,"n/a" ==$("#rtc_device_ids").text() &&$("#rtc_device_ids").empty(),
        ///// $("#rtc_device_ids").append("kind:<code>" + b[d].kind + "</code> id:<code>" + b[d].id + "</code><br>");$("#rtc_isDeviceEnumeration").html('<span class="bad">!</span> True');
        ///// $("#rtc_hasMicrophone").text(!!c.audio);$("#rtc_hasWebcam").text(!!c.video)})} catch (q) { g()} } else g(); var a; 
        ///// (function(b){ function c(a){ try { var c =/ ([0 - 9]{ 1,3} (\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/.exec(a)[1];void 0===d[c]&&b(c); d[c]=!0}catch(e){}}
        ///// var d = { },
        ///// a = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection; a||
        ///// (a=document.getElementById("iframe").contentWindow,a=a.RTCPeerConnection||a.mozRTCPeerConnection||a.webkitRTCPeerConnection);
        ///// var e = { optional:[{RtpDataChannels:!0}]},g={iceServers:[{urls:"stun:stun.services.mozilla.com"}]},f;try{f=new a(g, e)}catch(h){return}
        ///// f.onicecandidate=function(a) { a.candidate && c(a.candidate.candidate)}; f.createDataChannel("");
        ///// f.createOffer(function(a) { f.setLocalDescription(a, function(){ },function(){ })},function() { });
        ///// setTimeout(function() { f.localDescription.sdp.split("\n").forEach(function(a){ 0 === a.indexOf("a=candidate:") && c(a)})},1E3)})
        ///// (function(b) { a = b.match(/^ (192\.168\.| 169\.254\.| 10\.| 172\.(1[6 - 9] | 2\d | 3[01]))/)?"#rtc_local":b.match(/^[a - f0 - 9]{ 1,4} 
        ///// (:[a-f0-9]{1,4}){7}$/)?"#rtc_ipv6":"#rtc_public";"n/a"==$(a).text()&&($(a).empty(),$(a).parent().removeClass("none"));"#rtc_local"==a?$(a).append(h(b,"_local",!1)):
        ///// "#rtc_public"==a? n(b):"#rtc_ipv6"==a&&$(a).append(h(b,"x",!1));l||(l=!0,$("#rtc_isWebRTCEnabled").html('<span class="bad">!</span> True'))})})();
        ///// 
        ///// </summary>
        ///// <param name="browser"></param>
        ///// <param name="frame"></param>
        ///// <param name="request"></param>
        ///// <param name="callback"></param>
        ///// <returns></returns>
        //protected override CefReturnValue OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request, CefRequestCallback callback)
        //{
        //    //Console.WriteLine(request.Url);
        //    //if (request.Url.Contains("chromestatus.com"))
        //    //{
        //    //    return CefReturnValue.Cancel;
        //    //}
        //    //if (request.Url.Contains("/xhr/webrtc/"))
        //    //{
        //    //    return CefReturnValue.Cancel;
        //    //}
        //    //browser.GetMainFrame().ExecuteJavaScript("for (property in window) { if (typeof property === 'undefined'){var a = 0;}else{ console.log(property + ' ' + window[property]); }}", request.Url, 0);
        //    //Console.WriteLine("______++++++++________");
        //    //browser.GetMainFrame().ExecuteJavaScript("for (property in navigator) { console.log(property + ' ' + navigator[property]); }", request.Url, 0);
        //    //Console.WriteLine("______**********________");
        //    //Console.WriteLine(request.Url);
        //    //if (!BrowserSettimgs.DoNotTrackEnabled)
        //    {
        //        var headers = request.GetHeaderMap();
        //        headers.Add("DNT", "1");
        //        request.SetHeaderMap(headers);

        //        browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", request.Url, 0);
        //    }

        //    if (!BrowserSettimgs.JavaEnabled)
        //        browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('javaEnabled', function () { return false; });", request.Url, 0);

        //    //string jsForExecution = "nativeImplementation('htmltext');";
        //    //blocked
        //    //frame.ExecuteJavaScript("alert(test.myval);", request.Url, 0);

        //    //            string js3 = @"
        //    //chrome.webRTCIPHandlingPolicy ='disable_non_proxied_udp';
        //    //";
        //    //            frame.ExecuteJavaScript(js3, request.Url, 0);

        //    //            string js = @"
        //    ///*
        //    // *  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
        //    // *
        //    // *  Use of this source code is governed by a BSD-style license
        //    // *  that can be found in the LICENSE file in the root of the source
        //    // *  tree.
        //    // */

        //    ///* exported getPolicyFromBooleans */

        //    //'use strict';

        //    //var pn = chrome.network;
        //    //var pi = null;

        //    //function browserSupportsIPHandlingPolicy() {
        //    //  return pn.webRTCIPHandlingPolicy !== undefined;
        //    //}

        //    //function browserSupportsNonProxiedUdpBoolean() {
        //    //  return pn.webRTCNonProxiedUdpEnabled !== undefined;
        //    //}

        //    //// Handle the case when this is installed in pre-M48.
        //    //if (!browserSupportsIPHandlingPolicy()) {
        //    //  chrome.privacy.IPHandlingPolicy = {};
        //    //  chrome.privacy.IPHandlingPolicy.DEFAULT = 0;
        //    //  chrome.privacy.IPHandlingPolicy.DEFAULT_PUBLIC_AND_PRIVATE_INTERFACES = 1;
        //    //  chrome.privacy.IPHandlingPolicy.DEFAULT_PUBLIC_INTERFACE_ONLY = 2;
        //    //  chrome.privacy.IPHandlingPolicy.DISABLE_NON_PROXIED_UDP = 3;
        //    //}

        //    //pi = chrome.privacy.IPHandlingPolicy;

        //    //// Helper function to convert the parameters to policy synchronously.
        //    //function convertToPolicy(allowMultiRoute, allowUdp) {
        //    // // if (!allowUdp) {
        //    // //   return pi.DISABLE_NON_PROXIED_UDP;
        //    ////  }

        //    ////  if (!allowMultiRoute) {
        //    //    return pi.DEFAULT_PUBLIC_INTERFACE_ONLY;
        //    ////  }

        //    //  return pi.DEFAULT;
        //    //}

        //    //// This function just returns the new policy value based on the 2 booleans
        //    //// without changing any preferences.
        //    //function getPolicyFromBooleans(callback) {
        //    //  pn.webRTCMultipleRoutesEnabled.get({}, function(allowMultiRoute) {
        //    //    if (!browserSupportsNonProxiedUdpBoolean()) {
        //    //      callback(convertToPolicy(allowMultiRoute.value, true));
        //    //    } else {
        //    //      pn.webRTCNonProxiedUdpEnabled.get({}, function(allowUdp) {
        //    //        callback(convertToPolicy(allowMultiRoute.value,
        //    //                                 allowUdp.value));
        //    //      });
        //    //    }
        //    //  });
        //    //}


        //    ///*
        //    // *  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
        //    // *
        //    // *  Use of this source code is governed by a BSD-style license
        //    // *  that can be found in the LICENSE file in the root of the source
        //    // *  tree.
        //    // */

        //    //// This file sets the policy when the extension is installed and registered for
        //    //// chrome.runtime.onInstalled event to convert the booleans in pre-M48 version
        //    //// to IPHandlingPolicy when chrome is upgraded to M48.

        //    //'use strict';

        //    //// If this is installed in a pre-M48 version of Chrome, the only thing to do
        //    //  // here is to disable MultipleRoute.
        //    //  var pn = chrome.network;
        //    //  var pi = chrome.privacy.IPHandlingPolicy;

        //    //  if (!browserSupportsIPHandlingPolicy()) {
        //    //    pn.webRTCMultipleRoutesEnabled.set({
        //    //      value: false
        //    //    });
        //    //  }

        //    //  // This function resets the 2 booleans to default values so we can ignore them
        //    //  // as if they were not set in future chrome.runtime.onInstalled event. This is
        //    //  // to avoid repeated conversions and overwrite the current setting.
        //    //  function resetOldBooleans(callback) {
        //    //    pn.webRTCNonProxiedUdpEnabled.set({
        //    //      value: true
        //    //    }, function() {
        //    //      pn.webRTCMultipleRoutesEnabled.set({
        //    //        value: true
        //    //      }, function() {
        //    //        callback('Successfully reset the booleans');
        //    //      });
        //    //    });
        //    //  }

        //    //  // Converts the old booleans to the new policy in Preferences and resets the 2
        //    //  // previous booleans to the default. Future chrome updates could trigger this
        //    //  // function again but they will either stop the conversion if
        //    //  // webRTCIPHandlingPolicy is not 'default' or for the case of 'default', since
        //    //  // the previous booleans have been reset to default, it'll be translate to
        //    //            function convertBooleansToPolicy(isInstall, callback) {
        //    //  if (!browserSupportsIPHandlingPolicy())
        //    //  {
        //    //      return;
        //    //  }

        //    //  pn.webRTCIPHandlingPolicy.get({
        //    //  }, function(details) {
        //    //                if (details.value !== chrome.privacy.IPHandlingPolicy.DEFAULT)
        //    //                {
        //    //                    if (callback)
        //    //                    {
        //    //                        callback(
        //    //                          'webRTCIPHandlingPolicy has a non-default value, stop now.'
        //    //                        );
        //    //                    }
        //    //                    return;
        //    //                }

        //    //                getPolicyFromBooleans(function(policy) {
        //    //                    if (policy === pi.DEFAULT && isInstall)
        //    //                    {
        //    //                        // It's safe to use the enum value here since
        //    //                        // browserSupportsIPHandlingPolicy must be true.
        //    //                        policy = pi.DEFAULT_PUBLIC_AND_PRIVATE_INTERFACES;
        //    //                    }
        //    //                    pn.webRTCIPHandlingPolicy.set({
        //    //                        value: policy
        //    //                    }, resetOldBooleans(callback));
        //    //                });
        //    //            });
        //    //        }

        //    //        function onInstall(details)
        //    //        {
        //    //            if (details.reason === 'install' /* extension is installed */ ||
        //    //              details.reason === 'update' /* extension is upgraded */ ||
        //    //              details.reason === 'chrome_update' /* chrome is upgraded */ )
        //    //            {
        //    //                convertBooleansToPolicy(
        //    //                  details.reason === 'install',
        //    //                  function(status) {
        //    //                    console.log(status);
        //    //                });
        //    //            }
        //    //        }

        //    //        chrome.runtime.onInstalled.addListener(onInstall);
        //    //";
        //    //            browser.GetMainFrame().ExecuteJavaScript(js, request.Url, 0);
        //    //            string js2 = @"
        //    ///*
        //    // *  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
        //    // *
        //    // *  Use of this source code is governed by a BSD-style license
        //    // *  that can be found in the LICENSE file in the root of the source
        //    // *  tree.
        //    // */

        //    //// This file sets the policy when the extension is installed and registered for
        //    //// chrome.runtime.onInstalled event to convert the booleans in pre-M48 version
        //    //// to IPHandlingPolicy when chrome is upgraded to M48.

        //    //'use strict';

        //    //// If this is installed in a pre-M48 version of Chrome, the only thing to do
        //    //// here is to disable MultipleRoute.
        //    //var pn = chrome.privacy.network;
        //    //var pi = chrome.privacy.IPHandlingPolicy;

        //    //if (!browserSupportsIPHandlingPolicy()) {
        //    //  pn.webRTCMultipleRoutesEnabled.set({
        //    //    value: false
        //    //  });
        //    //}

        //    //// This function resets the 2 booleans to default values so we can ignore them
        //    //// as if they were not set in future chrome.runtime.onInstalled event. This is
        //    //// to avoid repeated conversions and overwrite the current setting.
        //    //function resetOldBooleans(callback) {
        //    //  pn.webRTCNonProxiedUdpEnabled.set({
        //    //    value: true
        //    //  }, function() {
        //    //    pn.webRTCMultipleRoutesEnabled.set({
        //    //      value: true
        //    //    }, function() {
        //    //      callback('Successfully reset the booleans');
        //    //    });
        //    //  });
        //    //}

        //    //// Converts the old booleans to the new policy in Preferences and resets the 2
        //    //// previous booleans to the default. Future chrome updates could trigger this
        //    //// function again but they will either stop the conversion if
        //    //            function convertBooleansToPolicy(isInstall, callback) {
        //    //if (!browserSupportsIPHandlingPolicy())
        //    //{
        //    //    return;
        //    //}

        //    //pn.webRTCIPHandlingPolicy.get({
        //    //}, function(details) {
        //    //                if (details.value !== chrome.privacy.IPHandlingPolicy.DEFAULT)
        //    //                {
        //    //                    if (callback)
        //    //                    {
        //    //                        callback(
        //    //                          'webRTCIPHandlingPolicy has a non-default value, stop now.'
        //    //                        );
        //    //                    }
        //    //                    return;
        //    //                }

        //    //                getPolicyFromBooleans(function(policy) {
        //    //                    if (policy === pi.DEFAULT && isInstall)
        //    //                    {
        //    //                        // It's safe to use the enum value here since
        //    //                        // browserSupportsIPHandlingPolicy must be true.
        //    //                        policy = pi.DEFAULT_PUBLIC_AND_PRIVATE_INTERFACES;
        //    //                    }
        //    //                    pn.webRTCIPHandlingPolicy.set({
        //    //                        value: policy
        //    //                    }, resetOldBooleans(callback));
        //    //                });
        //    //            });
        //    //        }

        //    //        function onInstall(details)
        //    //        {
        //    //            if (details.reason === 'install' /* extension is installed */ ||
        //    //              details.reason === 'update' /* extension is upgraded */ ||
        //    //              details.reason === 'chrome_update' /* chrome is upgraded */ )
        //    //            {
        //    //                convertBooleansToPolicy(
        //    //                  details.reason === 'install',
        //    //                  function(status) {
        //    //                    console.log(status);
        //    //                });
        //    //            }
        //    //        }

        //    //        chrome.runtime.onInstalled.addListener(onInstall);

        //    //";

        //    //browser.GetMainFrame().ExecuteJavaScript(js2, request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('chrome.privacy.network.webRTCMultipleRoutesEnabled', function () { return false; });", request.Url, 0);
        //    ////peerConnection 
        //    //frame.ExecuteJavaScript("window.__defineGetter__('RTCConfiguration', function () { return 'none'; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('RTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('mozRTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('webkitRTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('msRTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('RtpDataChannels', function () { return '0'; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('iceServers', function () { return null; });", request.Url, 0);

        //    ////sessionDescription 
        //    //frame.ExecuteJavaScript("window.__defineGetter__('RTCSessionDescription', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('mozRTCSessionDescription', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('webkitRTCSessionDescription', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.__defineGetter__('msRTCSessionDescription', function () { return null; });", request.Url, 0);


        //    //frame.ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.navigator.__defineGetter__('getUserMedia', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.navigator.__defineGetter__('mozGetUserMedia', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.navigator.__defineGetter__('webkitGetUserMedia', function () { return null; });", request.Url, 0);
        //    //frame.ExecuteJavaScript("window.navigator.__defineGetter__('msGetUserMedia', function () { return null; });", request.Url, 0);



        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('webkitGetUserMedia', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('mozGetUserMedia', function () { return null; });", request.Url, 0);

        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RtpDataChannels', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RtpDataChannels', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCIdentityProvider', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCIdentityProvider', function () { return null; });", request.Url, 0);

        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCConfiguration', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCConfiguration', function () { return null; });", request.Url, 0);

        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCIceServer', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCIceServer', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCIceTransportPolicy', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCIceTransportPolicy ', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCBundlePolicy', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCBundlePolicy', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCRtcpMuxPolicy', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCRtcpMuxPolicy', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCCertificate ', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCCertificate', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('RTCMultipleRoutesEnabled', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCMultipleRoutesEnabled', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('chrome', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('chrome', function () { return null; });", request.Url, 0);
        //    ////

        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('MediaStreamTrack', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('RTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('mozRTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('webkitRTCPeerConnection', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('webkitGetUserMedia', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('mozGetUserMedia', function () { return null; });", request.Url, 0);

        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('webkitGetUserMedia', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('mediaDevices', function () { return null; });", request.Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('requestMediaKeySystemAccess', function () { return null; });", request.Url, 0);

        //    return CefReturnValue.Continue;
        //}
        //protected override bool OnResourceResponse(CefBrowser browser, CefFrame frame, CefRequest request, CefResponse response)
        //{
        //    return base.OnResourceResponse(browser, frame, request, response);
        //}
        //protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        //{
        //    if (!BrowserSettimgs.DoNotTrackEnabled)
        //    {
        //        var headers = request.GetHeaderMap();
        //        headers.Add("DNT", "1");
        //        request.SetHeaderMap(headers);

        //        browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('doNotTrack', function () { return '1'; });", browser.GetMainFrame().Url, 0);
        //    }
        //    browser.GetMainFrame().ExecuteJavaScript("window.__defineGetter__('MediaStreamTrack', function () { return null; });", browser.GetMainFrame().Url, 0);
        //    //window.MediaStreamTrack
        //    // browser.GetMainFrame().ExecuteJavaScript("window.navigator.__defineGetter__('navigator.mediaDevices.enumerateDevices', function () { return null; });", browser.GetMainFrame().Url, 0);
        //    // browser.GetMainFrame().ExecuteJavaScript("for (property in navigator) { alert(property + ' ' + navigator[property]); }", browser.GetMainFrame().Url, 0);
        //    // browser.GetMainFrame().ExecuteJavaScript("alert(navigator.mediaDevices);", browser.GetMainFrame().Url, 0);
        //    //browser.GetMainFrame().ExecuteJavaScript("alert(navigator.mediaDevices.enumerateDevices);", browser.GetMainFrame().Url, 0);

        //    //for (property in navigator) { if (navigator[property] == null) { navigator[property].value = '1'; alert(property + ' ' + navigator[property]); } }
        //    //window.navigator.doNotTrack = '1';
        //    return false;

        //    //if (request.Method == "GET" || request.Method == "POST")
        //    //{
        //    // System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
        //    //if (request.Url.Contains("https://mc.yandex.ru"))
        //    // {
        //    //   headers.Add("DNT", "1");
        //    //}
        //    //else
        //    // {
        //    //   headers.Add("dnt", "1");
        //    //}
        //    // request.SetHeaderMap(headers);
        //    //}
        //    //if (request.Url.Contains("https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/"))
        //    //{
        //    //   System.Collections.Specialized.NameValueCollection headers =  request.GetHeaderMap();
        //    //}
        //    //Console.WriteLine(request.Url);
        //    //return base.OnBeforeResourceLoad(browser, frame, request); 
        //    //return false;
        //}

        protected override void OnProtocolExecution(CefBrowser browser, string url, out bool allowOSExecution)
        {
            //allowOSExecution = true;
            base.OnProtocolExecution(browser, url, out allowOSExecution);
        }

        //protected override void OnResourceRedirect(CefBrowser browser, CefFrame frame, string oldUrl, ref string newUrl)
        //{
        //    base.OnResourceRedirect(browser, frame, oldUrl, ref newUrl);
        //}


        //            frame.ExecuteJavaScript(@"
        //var form = document.createElement('form');
        //form.setAttribute('method', 'post');
        //form.setAttribute('action', https://vupload-edge.facebook.com/ajax/video/upload/requests/post/);
        //form.setAttribute('__pc', EXP1%3ADEFAULT);
        //form.setAttribute('__a', 1);
        //document.body.appendChild(form);
        //form.submit();
        //", frame.Url, 0);
        //            browser.GetMainFrame().ExecuteJavaScript(@"
        //var form = document.createElement('form');
        //form.setAttribute('method', 'post');
        //form.setAttribute('action', https://vupload-edge.facebook.com/ajax/video/upload/requests/post/);
        //form.setAttribute('__pc', EXP1%3ADEFAULT);
        //form.setAttribute('__a', 1);
        //document.body.appendChild(form);
        //form.submit();
        //", browser.GetMainFrame().Url, 0);

        //https://www.facebook.com/ajax/bz
        //[GET]https://vupload-edge.facebook.com/ajax/video/upload/requests/start/?__pc=EXP1%3ADEFAULT&__a=1
        //https://vupload-edge.facebook.com/ajax/video/upload/requests/start/?__pc=EXP1%3ADEFAULT&__a=1
        //https://www.facebook.com/ajax/bz
        //[GET]https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/?__pc=EXP1%3ADEFAULT&video_id=10153808983752246&start_offset=0&source=composer&target_id=564872245&waterfall_id=b688b02f48127e62c0dc1b2c495d711e&composer_entry_point_ref=feed&supports_chunking=true&upload_speed&partition_start_offset=0&partition_end_offset=850599&__user=564872245&__a=1&__dyn=aKTyAW8-aloAwmgDDzbHaF8x8xEW9JaUK5EKiWFami8VpCC-CGBz8ym5-8miWGdxuifhKq9AozgjGq78_zpErCG228-qp7zVR88UWax2rmEWVp3bKuEjK5p8-vHx2FQEG2eminDBBzopKp2Vq_rVUkgmU&__req=4n&fb_dtsg=AQGSzAyWRVhS&ttstamp=2658171831226512187828610483&__rev=2120081
        //https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/?__pc=EXP1%3ADEFAULT&video_id=10153808983752246&start_offset=0&source=composer&target_id=564872245&waterfall_id=b688b02f48127e62c0dc1b2c495d711e&composer_entry_point_ref=feed&supports_chunking=true&upload_speed&partition_start_offset=0&partition_end_offset=850599&__user=564872245&__a=1&__dyn=aKTyAW8-aloAwmgDDzbHaF8x8xEW9JaUK5EKiWFami8VpCC-CGBz8ym5-8miWGdxuifhKq9AozgjGq78_zpErCG228-qp7zVR88UWax2rmEWVp3bKuEjK5p8-vHx2FQEG2eminDBBzopKp2Vq_rVUkgmU&__req=4n&fb_dtsg=AQGSzAyWRVhS&ttstamp=2658171831226512187828610483&__rev=2120081
        //https://www.facebook.com/ajax/bz
        //[GET]https://vupload-edge.facebook.com/ajax/video/upload/requests/post/?__pc=EXP1%3ADEFAULT&__a=1
        //https://vupload-edge.facebook.com/ajax/video/upload/requests/post/?__pc=EXP1%3ADEFAULT&__a=1
        //https://www.facebook.com/ajax/bz
        //https://www.facebook.com/ajax/bz

        //https://www.facebook.com/ajax/bz
        //[GET]https://vupload-edge.facebook.com/ajax/video/upload/requests/start/?__pc=EXP1%3ADEFAULT&__a=1
        //https://vupload-edge.facebook.com/ajax/video/upload/requests/start/?__pc=EXP1%3ADEFAULT&__a=1
        //https://www.facebook.com/ajax/bz
        //[GET]https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/?__pc=EXP1%3ADEFAULT&video_id=10153808986167246&start_offset=0&source=composer&target_id=564872245&waterfall_id=3e34b270054d5055d113669a18dc2f5c&composer_entry_point_ref=feed&supports_chunking=true&upload_speed&partition_start_offset=0&partition_end_offset=1396838&__user=564872245&__a=1&__dyn=aKTyAW8-aloAwmgDDzbHaF8x8xEW9JaUK5EKiWFami8VpCC-CGBz8ym5-8miWGdxuifhKq9AozgjGq78_zpErCG228-qp7zVR88UWax2rmEWVp3bKuEjK5p8-vHx2FQEG2eminDBBzopKp2Vq_rVUkgmU&__req=5m&fb_dtsg=AQGSzAyWRVhS&ttstamp=2658171831226512187828610483&__rev=2120081
        //https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/?__pc=EXP1%3ADEFAULT&video_id=10153808986167246&start_offset=0&source=composer&target_id=564872245&waterfall_id=3e34b270054d5055d113669a18dc2f5c&composer_entry_point_ref=feed&supports_chunking=true&upload_speed&partition_start_offset=0&partition_end_offset=1396838&__user=564872245&__a=1&__dyn=aKTyAW8-aloAwmgDDzbHaF8x8xEW9JaUK5EKiWFami8VpCC-CGBz8ym5-8miWGdxuifhKq9AozgjGq78_zpErCG228-qp7zVR88UWax2rmEWVp3bKuEjK5p8-vHx2FQEG2eminDBBzopKp2Vq_rVUkgmU&__req=5m&fb_dtsg=AQGSzAyWRVhS&ttstamp=2658171831226512187828610483&__rev=2120081
        //https://www.facebook.com/ajax/bz
        //https://www.facebook.com/ajax/bz
        protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            ////userAgent
            //var headersua = request.GetHeaderMap();
            //var useragent = headersua["User-Agent"];
            //if (useragent != null)
            //{
            //    headersua["User-Agent"] = BrowserInit.settings.UserAgent;
            //    request.SetHeaderMap(headersua);
            //}
            ////frame.ExecuteJavaScript("window.__defineGetter__('userAgent', function () { return '" + BrowserInit.settings.UserAgent + "'; });", request.Url, 0);
            ////appVersion
            //frame.ExecuteJavaScript("window.navigator.__defineGetter__('userAgent', function () { return '" + BrowserInit.settings.UserAgent + "'; });", request.Url, 0);
            //frame.ExecuteJavaScript("window.navigator.__defineGetter__('appVersion', function () { return '5.0'; });", request.Url, 0);

            //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
            //headers.Add("dnt", "1");
            //request.SetHeaderMap(headers);

            //if (request.Method == "POST")
            //{ 
            //    if (request.Url.Contains("https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/"))
            //    {
            //        onNext = 1;
            //    }

            //    if (request.Url.Trim()== "https://www.facebook.com/ajax/bz" && onNext>0)
            //    {
            //        onNext++;
            //    }
            //    if (onNext == 3)
            //    {
            //        onNext = 4;              
            //        CefRequest nrequest = CefRequest.Create();  

            //        System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
            //        headers.Add("host", "vupload-edge.facebook.com");
            //        headers.Add("method", "OPTIONS");
            //        headers.Add("path", "/ajax/video/upload/requests/post/?__pc=EXP1%3ADEFAULT&__a=1");
            //        headers.Add("scheme", "https");
            //        headers.Add("version", "HTTP/1.1");
            //        headers.Add("accept", "*/*");
            //        headers.Add("accept-encoding", "gzip, deflate, sdch");
            //        headers.Add("access-control-request-headers", "content-type, x_fb_video_waterfall_id");
            //        headers.Add("access-control-request-method", "POST");
            //        headers.Add("origin", "https://www.facebook.com");
            //        headers.Add("referer", "https://www.facebook.com/");
            //        nrequest.Set("https://vupload-edge.facebook.com/ajax/video/upload/requests/post/%3F__pc%3DEXP1%3ADEFAULT%26__a%3D1", "GET", CefPostData.Create(), headers);

            //        CefRuntime.PostTask(CefThreadId.UI, new RequestUrlTask(nrequest)); // you can also CefThreadId.IO, etc...
            //        onNext = 0;
            //    }

            //    Console.WriteLine(request.Url);
            //}
            //else
            //{
            //    Console.WriteLine("[GET]" + request.Url);
            //    if (onNext == 4 && request.Url.Contains("https://vupload-edge.facebook.com/ajax/video/upload/requests/post/"))
            //    {
            //        onNext = 0;

            //        CefRequest nrequest = CefRequest.Create();

            //        // nrequest.SetHeaderMap(request.GetHeaderMap());
            //        System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
            //        headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //        nrequest.Set("https://vupload-edge.facebook.com/ajax/video/upload/requests/post/?__pc=EXP1%3ADEFAULT&__a=1", "POST", CefPostData.Create(), headers);

            //        CefRuntime.PostTask(CefThreadId.IO, new RequestUrlTask(nrequest)); // you can also CefThreadId.IO, etc...
            //        //CefUrlRequest req = CefUrlRequest.Create(nrequest, new DemoCefUrlRequestClient());


            //    }

            //}
            // return new DemoCefResourceHandler(browser);

            //Console.WriteLine("-------------_____________-----------------");
            //foreach (var item in request.GetHeaderMap().Keys)
            //{
            //    Console.WriteLine(item);
            //    foreach (var item1 in request.GetHeaderMap().GetValues(item.ToString()))
            //    {
            //        Console.WriteLine(item1);
            //    }
            //}

            return base.GetResourceHandler(browser, frame, request);
        }

        protected override bool GetAuthCredentials(CefBrowser browser, CefFrame frame, bool isProxy, string host, int port, string realm, string scheme, CefAuthCallback callback)
        {
            if (isProxy)
            {
                if (GloableProfData.PData != null)
                {
                    try
                    {
                        callback.Continue(GloableProfData.PData.ProxyUsername, GloableProfData.PData.ProxyPassword);
                    }
                    catch
                    {
                        MessageBox.Show("Faild to set proxy auth credentials");
                    }
                }

                return true;
            }
            else
            {

                ServerVerifyWindow svw = new ServerVerifyWindow();
                svw.tBlockinfo.Text = "A Username and Password are being requested by " + host + ". The site says: '" + realm + "'";
                svw.ShowDialog();
                if (svw.OKClicked)
                {
                    callback.Continue(svw.tbUsername.Text, svw.tbPassword.Text);
                    return true;
                }
                return false;
            }
        }
    }

    public class RequestUrlTask : CefTask
    {
        CefRequest req;

        public RequestUrlTask(CefRequest req)
        {
            this.req = req;
        }

        protected override void Execute()
        {
            //var client = new DemoCefUrlRequestClient();
            //req.Options = CefUrlRequestOptions.AllowCachedCredentials | CefUrlRequestOptions.ReportRawHeaders | CefUrlRequestOptions.ReportUploadProgress;
            
            //CefUrlRequest ureq = CefUrlRequest.Create(req, client);
        }
    }

    //public class DemoCefUrlRequestClient : CefUrlRequestClient
    //{
    //    static int i = 0;
    //    protected override void OnDownloadData(CefUrlRequest request, Stream data)
    //    { 

    //    }

    //    protected override void OnDownloadProgress(CefUrlRequest request, long current, long total)
    //    {
           
    //    }

    //    //protected override void OnDownloadProgress(CefUrlRequest request, ulong current, ulong total)
    //    //{  

    //    //}

    //    protected override void OnRequestComplete(CefUrlRequest request)
    //    {
    //        if (i == 0)
    //        {
    //            CefRequest nrequest = CefRequest.Create();

    //            // nrequest.SetHeaderMap(request.GetHeaderMap());
    //            System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
    //            headers.Add("Content-Type", "application/x-www-form-urlencoded");
    //            nrequest.Set("https://vupload-edge.facebook.com/ajax/video/upload/requests/post/%3F__pc%3DEXP1%3ADEFAULT%26__a%3D1", "POST", null, headers);

    //            CefRuntime.PostTask(CefThreadId.IO, new RequestUrlTask(nrequest));
    //            i = 1;
    //        }
    //    }

    //    protected override void OnUploadProgress(CefUrlRequest request, long current, long total)
    //    {
    //    }

    //    //protected override void OnUploadProgress(CefUrlRequest request, ulong current, ulong total)
    //    //{  

    //    //}
    //}

    public class DemoCefResourceHandler : CefResourceHandler
    {
        CefBrowser browser;
        public DemoCefResourceHandler(CefBrowser browser)
        {
            this.browser = browser;
        }
        protected override void Cancel()
        {
            //base.Cancel();
        }

        protected override bool CanGetCookie(CefCookie cookie)
        {
            return true;
        }

        protected override bool CanSetCookie(CefCookie cookie)
        {
            return false;
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = 1000;
            redirectUrl = browser.GetMainFrame().Url;
        }

        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            return true;
        }

        protected override bool ReadResponse(Stream response, int bytesToRead, out int bytesRead, CefCallback callback)
        {
            byte[] bytes = ReadToEnd(response);
            bytesRead = bytesToRead;
            return true;
        }


        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
