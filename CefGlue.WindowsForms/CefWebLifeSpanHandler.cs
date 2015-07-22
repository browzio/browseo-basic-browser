namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class CefWebLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly CefWebBrowser _core;

        public CefWebLifeSpanHandler(CefWebBrowser core)
        {
            _core = core;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);

        	_core.InvokeIfRequired(() => _core.OnBrowserAfterCreated(browser));
        }

        protected override bool DoClose(CefBrowser browser)
        {
            // TODO: ... dispose core
            return true;
        }

		protected override void OnBeforeClose(CefBrowser browser)
		{
			if (_core.InvokeRequired)
				_core.BeginInvoke((Action)_core.OnBeforeClose);
			else
				_core.OnBeforeClose();
		}

        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref bool noJavascriptAccess)
		{
             if (targetUrl != null)
            {
                string target = targetUrl.ToLower();
                if ((target.Contains("facebook") && target.Contains("popup")) || target.Contains("login") || target.Contains("oauth") || target.Contains("signup"))
                {
                    client = new DummyWebClient();
                    return false;
                }
            }

			var e = new BeforePopupEventArgs(frame, targetUrl, targetFrameName, popupFeatures, windowInfo, client, settings,
								 noJavascriptAccess);

			_core.InvokeIfRequired(() => _core.OnBeforePopup(e));

			client = e.Client;
			noJavascriptAccess = e.NoJavascriptAccess;

			return true;
		}

        private sealed class DummyWebClient : CefClient { }
    }
}
