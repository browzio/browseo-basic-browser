namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using Xilium.CefGlue;

    public class CefWebClient : CefClient
    {
        private readonly CefWebBrowser _core;
        private readonly CefWebLifeSpanHandler _lifeSpanHandler;
        private readonly CefWebDisplayHandler _displayHandler;
        private readonly CefWebLoadHandler _loadHandler;
        private readonly CefRequestHandler _requestHandler;
       // private readonly CefRequestHandler _requestHandler;
        private readonly CefRenderProcessHandler _RenderProcessHandler;
        private readonly ContextMenue _contextMenueHandler;

        public CefWebClient(CefWebBrowser core, CefRequestHandler requestHandler = null, CefRenderProcessHandler RenderProcessHandler = null)
        {
            _core = core;
            _lifeSpanHandler = new CefWebLifeSpanHandler(_core);
            _displayHandler = new CefWebDisplayHandler(_core);
            _loadHandler = new CefWebLoadHandler(_core);
            _contextMenueHandler = new ContextMenue(_core);

            if (requestHandler == null)
                _requestHandler = new CefWebRequestHandler(_core);
            else
                _requestHandler = requestHandler;
        }

        protected CefWebBrowser Core { get { return _core; } }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override CefRequestHandler GetRequestHandler()
        {
            return _requestHandler;
        }

         protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return base.GetRenderHandler();
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            return _contextMenueHandler;
        }

        class ContextMenue : CefContextMenuHandler
        {
            CefWebBrowser m_core;

            public ContextMenue(CefWebBrowser core)
            {
                m_core = core;
            }

            protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId, CefEventFlags eventFlags)
            {
                m_core.OnContextMenuItemSelected(commandId);
                return base.OnContextMenuCommand(browser, frame, state, commandId, eventFlags);
            }
        }
    }
}
