using Gecko;
using Gecko.CustomMarshalers;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.Browser
{
    public class nsBrowserHandler :
 nsIBrowserHandler,
 nsIContentHandler,
 nsICommandLineValidator
    {
        private static nsBrowserHandler _instancs;
        public static nsBrowserHandler Instance
        {
            get
            {
                if (_instancs == null) _instancs = new nsBrowserHandler();
                return _instancs;
            }
        }

        #region nsICommandLineHandler
        public void GetHelpInfoAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase result)
        {

        }


        public void Handle([MarshalAs(UnmanagedType.Interface)] nsICommandLine cmdLine)
        {

            // In the past, when an instance was not already running, the -remote
            // option returned an error code. Any script or application invoking the
            // -remote option is expected to be handling this case, otherwise they
            // wouldn't be doing anything when there is no Firefox already running.
            // Making the -remote option always return an error code makes those
            // scripts or applications handle the situation as if Firefox was not
            // already running.

            using (var aFlag = new nsAString())
            {
                //handleFlag
                //browser
                //remote
                //preferences
                //silent
                //private-window
                //private
                aFlag.SetData("browser");
                if (cmdLine.HandleFlag(aFlag, false))
                {

                }

                aFlag.SetData("remote");
                if (cmdLine.HandleFlag(aFlag, false))
                {

                }

                aFlag.SetData("preferences");
                if (cmdLine.HandleFlag(aFlag, false))
                {

                }

                aFlag.SetData("silent");
                if (cmdLine.HandleFlag(aFlag, false))
                {

                }

                aFlag.SetData("private-window");
                if (cmdLine.HandleFlag(aFlag, false))
                {

                }

                aFlag.SetData("private");
                if (cmdLine.HandleFlag(aFlag, false))
                {

                }

                //handleFlagWithParam
                //new-window
                //new-tab
                //chrome
                //search
                //file
                using (var result = new nsAString(""))
                {
                    string resString = "";

                    aFlag.SetData("new-window");
                    result.SetData("");
                    cmdLine.HandleFlagWithParam(aFlag, false, result);
                    resString = result.ToString();
                    if (resString != "" && resString != null)
                    {

                    }

                    aFlag.SetData("new-tab");
                    result.SetData("");
                    cmdLine.HandleFlagWithParam(aFlag, false, result);
                    resString = result.ToString();
                    if (resString != "" && resString != null)
                    {

                    }

                    aFlag.SetData("chrome");
                    result.SetData("");
                    cmdLine.HandleFlagWithParam(aFlag, false, result);
                    resString = result.ToString();
                    if (resString != "" && resString != null)
                    {

                    }

                    aFlag.SetData("search");
                    result.SetData("");
                    cmdLine.HandleFlagWithParam(aFlag, false, result);
                    resString = result.ToString();
                    if (resString != "" && resString != null)
                    {

                    }

                    aFlag.SetData("file");
                    result.SetData("");
                    cmdLine.HandleFlagWithParam(aFlag, false, result);
                    resString = result.ToString();
                    if (resString != "" && resString != null)
                    {

                    }
                }
            }
        }
        #endregion


        public void GetDefaultArgsAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase result)
        {
            result.SetData("about:home");
        }

        public void GetFeatures([MarshalAs(UnmanagedType.Interface)] nsICommandLine aCmdLine, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase result)
        {
        }

        public void GetStartPageAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase result)
        {
            result.SetData("about:home");
        }

        public void SetDefaultArgsAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase value)
        {

        }

        public void SetStartPageAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AUTF8StringMarshaler))] nsAUTF8StringBase value)
        {
        }











        public void HandleContent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string aContentType, [MarshalAs(UnmanagedType.Interface)] nsIInterfaceRequestor aWindowContext, [MarshalAs(UnmanagedType.Interface)] nsIRequest aRequest)
        {

        }









        public void Validate([MarshalAs(UnmanagedType.Interface)] nsICommandLine aCommandLine)
        {
            using (var aFlag = new nsAString())
            {
                // Other handlers may use osint so only handle the osint flag if the url
                // flag is also present and the command line is valid.
                aFlag.SetData("osint");
                var osintFlagIdx = aCommandLine.FindFlag(aFlag, false);

                aFlag.SetData("url");
                var urlFlagIdx = aCommandLine.FindFlag(aFlag, false);

                if (urlFlagIdx > -1 && (osintFlagIdx > -1 || aCommandLine.GetStateAttribute() == nsICommandLineConsts.STATE_REMOTE_EXPLICIT))
                {
                    //var urlParam = aCommandLine.getArgument(urlFlagIdx + 1);
                    //if (aCommandLine.length != urlFlagIdx + 2 || / firefoxurl:/.test(urlParam))
                    //        throw NS_ERROR_ABORT;
                    //var isDefault = false;
                    //try
                    //{
                    //    var url = Services.urlFormatter.formatURLPref("app.support.baseURL") +
                    //              "win10-default-browser";
                    //    if (urlParam == url)
                    //    {
                    //        isDefault = ShellService.isDefaultBrowser(false, false);
                    //    }
                    //}
                    //catch (ex) { }
                    //if (isDefault)
                    //{
                    //    // Firefox is already the default HTTP handler.
                    //    // We don't have to show the instruction page.
                    //    throw NS_ERROR_ABORT;
                    //}
                    //cmdLine.handleFlag("osint", false)
                }
            }
        }
    }
}
