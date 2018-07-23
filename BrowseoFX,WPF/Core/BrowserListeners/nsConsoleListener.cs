using Gecko;
using Gecko.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.BrowserListeners
{
    public class nsConsoleListener : Gecko.Interfaces.nsIConsoleListener, Gecko.Interfaces.nsIObserver
    {
        public static void Init()
        {

            var cobs = new nsConsoleListener();
            var cc = Xpcom.GetService<Gecko.Interfaces.nsIConsoleService>(Gecko.Contracts.ConsoleService);
            cc.RegisterListener(cobs);
            var svc = Xpcom.GetService<Gecko.Interfaces.nsIObserverService>(Gecko.Contracts.ObserverService);
            svc.AddObserver(cobs, "console-api-log-event", false);
        }

        public void Observe(Gecko.Interfaces.nsIConsoleMessage aMessage)
        {
            string message = aMessage.GetMessageAttribute();
            if (message.StartsWith("[JavaScript Error:"))
            {
                Console.WriteLine("[{0}] jserror: {1}", DateTime.UtcNow.ToString("HH:mm:ss"), message);
            }
        }

        void Gecko.Interfaces.nsIObserver.Observe(Gecko.Interfaces.nsISupports aSubject, string aTopic, string aData)
        {
            try
            {
                //var js = GeckoJavascriptBridge.GetService();
                //string s = js.EvaluateToString(aSubject, GeckoPrincipal.SystemPrincipal, "this.wrappedJSObject.arguments + ' [level: ' + this.wrappedJSObject.level + ', file: \"' + this.wrappedJSObject.filename + '\", line: ' + this.wrappedJSObject.lineNumber + ']'");
                Console.WriteLine("[{0}] console ({1}): {2}", DateTime.UtcNow.ToString("HH:mm:ss"), aData, "s");
            }
            catch (Gecko.GeckoJavaScriptException e)
            {
                Console.WriteLine("[{0}] {1}", DateTime.UtcNow.ToString("HH:mm:ss"), e.ToString());
            }
        }
    }
}
