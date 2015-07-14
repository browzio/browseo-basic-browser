using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Xilium.CefGlue.Client
{
    public static class BrowserInit
    {

        public static PersonData pData;
        public static string SitesFilePath;

        public static void Init(string sitesFilePath, PersonData data = null)
        {
            SitesFilePath = sitesFilePath;
            pData = data;
            try
            {
                CefRuntime.Load();
            }
            catch { }

            var mainArgs = new CefMainArgs(new string[0] { });
            var app = new DemoApp();
            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);

            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var settings = new CefSettings
            {
                BrowserSubprocessPath = exePath + "\\BrowserAndFeatures.exe",
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                PersistSessionCookies = true,
                LogSeverity = CefLogSeverity.Disable,
                IgnoreCertificateErrors = true,
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2227.0 Safari/537.36"
                //NoSandbox = true
                //LogFile = "CefGlue.log",
            };
            if (pData != null)
            {
                string path = Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "Caches\\" + pData.ProjectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                settings.CachePath = path;
            }

            if (!settings.MultiThreadedMessageLoop)
            {
                Application.Idle += (sender, e) => { CefRuntime.DoMessageLoopWork(); };
            }

            CefRuntime.Initialize(mainArgs, settings, app);

            Organiser.Common.Classes.UsageTracker.ProjectName = pData.ProjectName;
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Browser Started");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SetErrorMode(int wMode);

        [DllImport("kernel32.dll")]
        static extern FilterDelegate SetUnhandledExceptionFilter(FilterDelegate lpTopLevelExceptionFilter);
        public delegate bool FilterDelegate(Exception ex);

        [DllImport("kernel32", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        static extern ErrorModes SetErrorMode(ErrorModes uMode);

        [Flags]
        public enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        public static void Shutdown()
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Browser Closed");
            Organiser.Common.Classes.UsageTracker.SaveAllTrackedDataList();

            SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX | ErrorModes.SEM_NOOPENFILEERRORBOX);

           // var threads = Process.GetCurrentProcess().Threads;
           // for (int i = 0; i < threads.Count; i++)
           // {
           //     threads[i].Dispose();
           // }

           //// new Thread(() => {
           //     ProcessModuleCollection mc = Process.GetCurrentProcess().Modules;
           //     foreach (ProcessModule mod in mc)
           //     {
           //         if (mod.ModuleName.ToLower() == "libcef.dll")
           //             FreeLibrary(mod.BaseAddress);
           //     }

           //     CefRuntime.Shutdown();

           //     Process.GetCurrentProcess().Kill();

           //// }).Start();


            //foreach (var process in Process.GetProcessesByName("BrowserAndFeatures.exe"))
            //{
            //    process.Kill();
            //}
        }
    }
}
