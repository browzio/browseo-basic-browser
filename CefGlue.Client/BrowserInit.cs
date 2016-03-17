using Organiser.Common.Classes;
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
        public static void Init(PersonData data = null)
        {
            if (data != null)
            {
                GloableProfData.PData = data;
            }

            try
            {
                CefRuntime.Load();
            }
            catch { }

            var mainArgs = new CefMainArgs(new string[0] { });
            var app = new DemoApp();
            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);

            var exePath = AppDomain.CurrentDomain.BaseDirectory + "\\BrowserAndFeatures.exe";
            exePath = exePath.Replace("\\\\","\\");
            var settings = new CefSettings
            {
                BrowserSubprocessPath = exePath,
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                PersistSessionCookies = true,
                LogSeverity = CefLogSeverity.Disable,
                IgnoreCertificateErrors = true,
               // UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2556.0 Safari/537.36",
               // UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36",
                 UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36",
                //  RemoteDebuggingPort=123321,
                NoSandbox = true
                //LogFile = "CefGlue.log",
            };
            //settings.CommandLineArgsDisabled = true;
            
            if (GloableProfData.PData != null)
            {
                string path = Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "Caches\\" + GloableProfData.PData.ProjectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                settings.CachePath = path;
            }

            if (!settings.MultiThreadedMessageLoop)
            {
                Application.Idle += (sender, e) => { CefRuntime.DoMessageLoopWork(); };
            }
            
            CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);

            //CefRuntime.AddCrossOriginWhitelistEntry("file", "https", "facebook.com", true);
            //CefRuntime.AddWebPluginDirectory(@"C:\Windows\system32\Macromed\Flash\");
            //CefRuntime.RefreshWebPlugins();

            Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_BrowserStart );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //CefRuntime.AddWebPluginDirectory(@"C:\Windows\system32\Macromed\Flash");
            //CefRuntime.AddWebPluginPath(@"C:\Windows\System32\Macromed\Flash\pepflashplayer64_18_0_0_209.dll");
            //CefRuntime.RefreshWebPlugins();
        }

        public static void SetPersonData(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            if (GloableProfData.PData == null)
            {
                GloableProfData.PData = new PersonData()
                {
                    BirthdayYear = birthdayYear,
                    Children = children,
                    City = city,
                    CmbSelectedIndexDay = cmbSelectedIndexDay,
                    CmbSelectedIndexMonth = cmbSelectedIndexMonth,
                    CmbSelectedIndexSex = cmbSelectedIndexSex,
                    Country = country,
                    Dir = dir,
                    Email = email,
                    FilePath = filePath,
                    FirstName = firstName,
                    InMonney = inMonney,
                    InPBNVault = inPBNVault,
                    LastName = lastName,
                    Notes = notes,
                    Password = password,
                    PhoneNumber = phoneNumber,
                    ProfileName = profileName,
                    ProjectDir = projectDir,
                    ProjectName = projectName,
                    ProxyIP = proxyIP,
                    ProxyPassword = proxyPassword,
                    ProxyPort = proxyPort,
                    ProxyUsername = proxyUsername,
                    SIPBNType = sIPBNType,
                    State = state,
                    Street = street,
                    Username = username,
                    WebAddress = webAddress,
                    Zip = zip,
                };
            }
            else
            {
                GloableProfData.PData.BirthdayYear = birthdayYear;
                GloableProfData.PData.Children = children;
                GloableProfData.PData.City = city;
                GloableProfData.PData.CmbSelectedIndexDay = cmbSelectedIndexDay;
                GloableProfData.PData.CmbSelectedIndexMonth = cmbSelectedIndexMonth;
                GloableProfData.PData.CmbSelectedIndexSex = cmbSelectedIndexSex;
                GloableProfData.PData.Country = country;
                GloableProfData.PData.Dir = dir;
                GloableProfData.PData.Email = email;
                GloableProfData.PData.FilePath = filePath;
                GloableProfData.PData.FirstName = firstName;
                GloableProfData.PData.InMonney = inMonney;
                GloableProfData.PData.InPBNVault = inPBNVault;
                GloableProfData.PData.LastName = lastName;
                GloableProfData.PData.Notes = notes;
                GloableProfData.PData.Password = password;
                GloableProfData.PData.PhoneNumber = phoneNumber;
                GloableProfData.PData.ProfileName = profileName;
                GloableProfData.PData.ProjectDir = projectDir;
                GloableProfData.PData.ProjectName = projectName;
                GloableProfData.PData.ProxyIP = proxyIP;
                GloableProfData.PData.ProxyPassword = proxyPassword;
                GloableProfData.PData.ProxyPort = proxyPort;
                GloableProfData.PData.ProxyUsername = proxyUsername;
                GloableProfData.PData.SIPBNType = sIPBNType;
                GloableProfData.PData.State = state;
                GloableProfData.PData.Street = street;
                GloableProfData.PData.Username = username;
                GloableProfData.PData.WebAddress = webAddress;
                GloableProfData.PData.Zip = zip; 
            }
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
            //try
            //{
            //    Organiser.Common.Classes.UsageTracker.AddTraceCookie("Browser Closed");
            //    Organiser.Common.Classes.UsageTracker.SaveAllTrackedDataList();
            //}
            //catch { }
            SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX | ErrorModes.SEM_NOOPENFILEERRORBOX);
            CefRuntime.Shutdown();

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
