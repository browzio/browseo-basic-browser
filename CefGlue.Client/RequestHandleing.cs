using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xilium.CefGlue.WindowsForms;

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
            base.OnBeforeBrowse(browser, frame, request, isRedirect);
            return false;
        }   

        protected override bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            //System.Collections.Specialized.NameValueCollection headers = request.GetHeaderMap();
            //headers.Add("dnt", "1");     
            //request.SetHeaderMap(headers);
            base.OnBeforeResourceLoad(browser, frame, request);
            return false;
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
}
