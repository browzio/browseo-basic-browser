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
            //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
            //headers.Add("dnt", "1"); 
            //request.SetHeaderMap(headers);
            //Console.WriteLine(request.Url);
            return base.OnBeforeBrowse(browser, frame, request, isRedirect);
        }

        protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
            //headers.Add("dnt", "1");     
            //request.SetHeaderMap(headers);
            //if (request.Url.Contains("https://vupload-edge.facebook.com/ajax/video/upload/requests/receive/"))
            //{
            //   System.Collections.Specialized.NameValueCollection headers =  request.GetHeaderMap();
            //}
            //Console.WriteLine(request.Url);
            return base.OnBeforeResourceLoad(browser, frame, request); ;
        }


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
        int onNext;
        protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {



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

    public class DemoCefUrlRequestClient : CefUrlRequestClient
    {
        static int i = 0;
        protected override void OnDownloadData(CefUrlRequest request, Stream data)
        { 

        }

        protected override void OnDownloadProgress(CefUrlRequest request, ulong current, ulong total)
        {  

        }

        protected override void OnRequestComplete(CefUrlRequest request)
        {
            if (i == 0)
            {
                CefRequest nrequest = CefRequest.Create();

                // nrequest.SetHeaderMap(request.GetHeaderMap());
                System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
                headers.Add("Content-Type", "application/x-www-form-urlencoded");
                nrequest.Set("https://vupload-edge.facebook.com/ajax/video/upload/requests/post/%3F__pc%3DEXP1%3ADEFAULT%26__a%3D1", "POST", null, headers);

                CefRuntime.PostTask(CefThreadId.IO, new RequestUrlTask(nrequest));
                i = 1;
            }
        }

        protected override void OnUploadProgress(CefUrlRequest request, ulong current, ulong total)
        {  

        }
    }

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
