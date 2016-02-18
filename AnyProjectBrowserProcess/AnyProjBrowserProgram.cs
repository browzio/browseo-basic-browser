using Organiser.Common.Browser;
using Organiser.Common.Classes;
using PData.FilesReader;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;

namespace AnyProjectBrowserProcess
{
    class Program : IDisposable
    {
        static BrowserForSocialShare browser;
        [STAThread]
        static void Main(string[] args)
        {
            //Debugger.Launch();
            try
            {
                //set up project data
                string projectPath = args[0];
                projectPath = projectPath.Replace(MyFilesDatabase.SPLITTER, " ");
                InitializeProjectData(projectPath);
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
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36",
                NoSandbox = true,
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

        private static void ShowBrowserWindowDialog(string url, string type)
        {
            browser = new BrowserForSocialShare();
            browser.Text = "Loading... Project Name: " + GloableProfData.PData.ProjectName + " IP: " + GloableProfData.PData.ProxyIP + " PORT: " + GloableProfData.PData.ProxyPort;
            browser.SetSocialButtonsVisable(type);
            browser.SetStartUrl(url);
            browser.browserCntrl1.init(url,
                BrowserSettimgs.JavascriptEnabled ? CefState.Enabled : CefState.Disabled,
                BrowserSettimgs.JavaEnabled ? CefState.Enabled : CefState.Disabled,
                BrowserSettimgs.FlashEnabled ? CefState.Enabled : CefState.Disabled);
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
                            if (cbrowser.Browser != null)
                            {
                                var host = cbrowser.Browser.GetHost();
                                if (host != null)
                                {
                                    host.CloseBrowser();
                                    host.Dispose();
                                }

                                cbrowser.Browser.Dispose();
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

        private static void InitializeProjectData(string projectPath)
        {
            string filepath = Path.Combine(projectPath, "ProjectData.ini");
            IniFile ini = new IniFile(filepath);
            GloableProfData.PData = new PersonData();
            try
            {
                GloableProfData.PData.ProjectName = ini.IniReadValue("Data", "ProjectName");
                GloableProfData.PData.ProfileName = ini.IniReadValue("Data", "ProfileName");
                GloableProfData.PData.FirstName = ini.IniReadValue("Data", "FirstName");
                GloableProfData.PData.LastName = ini.IniReadValue("Data", "LastName");
                GloableProfData.PData.Email = ini.IniReadValue("Data", "Email");
                GloableProfData.PData.Password = ini.IniReadValue("Data", "Password");
                GloableProfData.PData.Username = ini.IniReadValue("Data", "Username");
                GloableProfData.PData.ProxyIP = ini.IniReadValue("Data", "ProxyIP");
                GloableProfData.PData.ProxyPort = ini.IniReadValue("Data", "ProxyPort");
                GloableProfData.PData.ProxyUsername = ini.IniReadValue("Data", "ProxyUsername");
                GloableProfData.PData.ProxyPassword = ini.IniReadValue("Data", "ProxyPassword");
                GloableProfData.PData.PhoneNumber = ini.IniReadValue("Data", "PhoneNumber");
                GloableProfData.PData.Street = ini.IniReadValue("Data", "Street");
                GloableProfData.PData.City = ini.IniReadValue("Data", "City");
                GloableProfData.PData.State = ini.IniReadValue("Data", "State");
                GloableProfData.PData.Zip = ini.IniReadValue("Data", "Zip");
                GloableProfData.PData.Country = ini.IniReadValue("Data", "Country");
                GloableProfData.PData.WebAddress = ini.IniReadValue("Data", "WebAddress");
                GloableProfData.PData.Notes = ini.IniReadValue("Data", "Notes");
                try
                {
                    GloableProfData.PData.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                    GloableProfData.PData.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                    GloableProfData.PData.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                }
                catch { }
                GloableProfData.PData.ProjectDir = projectPath;
                try
                {
                    GloableProfData.PData.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }


                MyFilesDatabase.GetSavedSesstion(GloableProfData.PData.ProjectName);
            }
            catch { }
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
