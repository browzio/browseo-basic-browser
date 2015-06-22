namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using Xilium.CefGlue;
    using Xilium.CefGlue.Wrapper;

    class DemoRenderProcessHandler : CefRenderProcessHandler
    {
        internal static bool DumpProcessMessages { get; private set; }

        public DemoRenderProcessHandler()
        {
            MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
        }

        internal CefMessageRouterRendererSide MessageRouter { get; private set; }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextCreated(browser, frame, context);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            MessageRouter.OnContextReleased(browser, frame, context);
        }

        protected override void OnFocusedNodeChanged(CefBrowser browser, CefFrame frame, CefDomNode node)
        {

        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (DumpProcessMessages)
            {
                var arguments = message.Arguments;
                for (var i = 0; i < arguments.Count; i++)
                {
                    var type = arguments.GetValueType(i);
                    object value;
                    switch (type)
                    {
                        case CefValueType.Null: value = null; break;
                        case CefValueType.String: value = arguments.GetString(i); break;
                        case CefValueType.Int: value = arguments.GetInt(i); break;
                        case CefValueType.Double: value = arguments.GetDouble(i); break;
                        case CefValueType.Bool: value = arguments.GetBool(i); break;
                        default: value = null; break;
                    }
                }
            }

            if (message.Name == "InjectData")
            {
                CefFrame mainFrame = browser.GetMainFrame();
                mainFrame.VisitDom(new DemoCefDomVisitor());
            }

            var handled = MessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
            if (handled) return true;

            return false;
        }

        internal class DemoCefDomVisitor : CefDomVisitor
        {
            protected override void Visit(CefDomDocument document)
            {
                foreach (var node in GetHackerNewsTitles(document.Root)) ;
            }

            private IEnumerable<CefDomNode> GetHackerNewsTitles(CefDomNode node)
            {
                CefDomNode child = node.FirstChild;
                if (child != null && IsInputNode(child))
                {
                    //foreach (var item in child.GetAttributes())
                    //{
                    //    item.Key;
                    //}
                }

                while (child != null)
                {
                    foreach (var childNode in GetHackerNewsTitles(child))
                    {
                        yield return childNode;
                    }
                    child = child.NextSibling;
                }
            }

            private bool IsInputNode(CefDomNode node)
            {
                try
                {
                    return node.ElementTagName.ToLower() == "input";
                }
                catch { return false; }
            }
        }
    }
}
