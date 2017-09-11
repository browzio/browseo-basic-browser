using Organiser.Common.Browser;
using Organiser.Common.Classes;
using PData.FilesReader;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;

namespace AnyProjectBrowserProcess
{
    class Program : IDisposable
    {
        [Flags]
        public enum ErrorModes
        {
            Default = 0x0,
            FailCriticalErrors = 0x0001,
            NoGpFaultErrorBox = 0x2, // &lt;- this is the one we need
            NoAlignmentFaultExcept = 0x0004,
            NoOpenFileErrorBox = 0x8000,
            SEM_NOGPFAULTERRORBOX = 0x0002,
        }

        /// <summary>
        /// to change error context
        /// </summary>
        public class ErrorModeContext : IDisposable
        {
            [DllImport("kernel32.dll")]
            static extern FilterDelegate SetUnhandledExceptionFilter(FilterDelegate lpTopLevelExceptionFilter);
            public delegate bool FilterDelegate(Exception ex);

            private readonly int _oldMode;

            public ErrorModeContext(ErrorModes mode)
            {
                FilterDelegate fd = delegate (Exception ex)
                {
                    return true;
                };
                _oldMode = SetErrorMode((int)mode);
            }

            ~ErrorModeContext()
            {
                Dispose(false);
            }

            private void Dispose(bool disposing)
            {
                SetErrorMode(_oldMode);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            [DllImport("kernel32.dll")]
            private static extern int SetErrorMode(int newMode);
        }

        //Application: AnyProjectBrowserProcess.exe Framework Version: v4.0.30319 Description: The process was terminated due to an unhandled exception.Exception Info: System.InvalidOperationException at AnyProjectBrowserProcess.Program.Main(System.String[]) 

        static BrowserForSocialShare browser;
        [STAThread]
        static void Main(string[] args)
        {
            //Debugger.Launch();
            using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            {
                try
                {
                    //set up project data
                    string projectPath = args[0];
                    projectPath = projectPath.Replace(MyFilesDatabase.SPLITTER, " ");
                    MyFilesDatabase.SetUpPdaaFromPath(projectPath);
                }
                catch { }



                // Load CEF. This checks for the correct CEF version.
                CefRuntime.Load();
                // Start the secondary CEF process.
                var cefMainArgs = new CefMainArgs(new string[0]);
                var cefApp = new DemoCefApp();
                // This is where the code path divereges for child processes.
                if (CefRuntime.ExecuteProcess(cefMainArgs, cefApp, IntPtr.Zero) != -1)
                {
                    throw new InvalidOperationException("Runtime could not the secondary process.");
                }




                //set up the browser
                //  string subProcessPath = AppDomain.CurrentDomain.BaseDirectory + "\\AnyProjectBrowserProcess.exe";
                // subProcessPath = subProcessPath.Replace("\\\\", "\\");
                string cachepath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Caches\\" + GloableProfData.PData.ProjectName);
                if (!Directory.Exists(cachepath)) Directory.CreateDirectory(cachepath);
                var cefSettings = new CefSettings
                {
                    BrowserSubprocessPath = "AnyProjectBrowserProcess.exe",
                    SingleProcess = false,
                    MultiThreadedMessageLoop = true,
                    PersistSessionCookies = true,
                    LogSeverity = CefLogSeverity.Disable,
                    IgnoreCertificateErrors = true,
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36",
                    NoSandbox = false,
                    CachePath = cachepath,
                };
                // Start the browser process (a child process).
                CefRuntime.Initialize(cefMainArgs, cefSettings, cefApp, IntPtr.Zero);



                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);



                //show the browser window
                string url = args[1];
                url = url.Replace(MyFilesDatabase.SPLITTER, " ");
                ShowBrowserWindowDialog(url, args[2]);

                CefRuntime.Shutdown();
            }
        }

        private static void ShowBrowserWindowDialog(string url, string type)
        {
            browser = new BrowserForSocialShare();
            browser.Text = "Loading... Project Name: " + GloableProfData.PData.ProjectName + " IP: " + GloableProfData.PData.ProxyIP + " PORT: " + GloableProfData.PData.ProxyPort;
            browser.SetSocialButtonsVisable(type);
            browser.SetStartUrl(url);
            browser.browserCntrl1.init(url, true, true, true);
            Application.Run(browser);

            Shutdown();
        }

        private static void Shutdown()
        {
            try
            {
                if (browser != null)
                {
                    var cbrowser = browser.browserCntrl1.CBrowser;
                    if (cbrowser != null)
                    {
                        try
                        {
                            if (browser.browserCntrl1.GetBrowser() != null)
                            {
                                var host = browser.browserCntrl1.GetBrowser().GetHost();
                                if (host != null)
                                {
                                    host.CloseBrowser();
                                    host.Dispose();
                                }

                                browser.browserCntrl1.GetBrowser().Dispose();
                            }
                        }
                        catch { }

                        cbrowser.Dispose();
                    }

                    browser.Dispose();
                }
            }
            catch { }
        }
        
        public void Dispose()
        {
            Shutdown();
        }
    }
}
