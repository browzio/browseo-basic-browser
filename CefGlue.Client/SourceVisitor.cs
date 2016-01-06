using System;  

namespace Xilium.CefGlue.Client
{
    public class SourceVisitor : Xilium.CefGlue.CefStringVisitor
    {
        private readonly Action<string> _callback;

        public SourceVisitor(Action<string> callback)
        {
            _callback = callback;
        }

        protected override void Visit(string value)
        {
            _callback(value);
        }
    }
}
