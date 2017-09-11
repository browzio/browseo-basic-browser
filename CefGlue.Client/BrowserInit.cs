using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Management;
using System.Threading.Tasks;
using Xilium.CefGlue.Wrapper;

namespace Xilium.CefGlue.Client
{
//    class NetworkManagement
//    {
//        /// <summary>
//        /// Set's a new IP Address and it's Submask of the local machine
//        /// </summary>
//        /// <param name="ip_address">The IP Address</param>
//        /// <param name="subnet_mask">The Submask IP Address</param>
//        /// <remarks>Requires a reference to the System.Management namespace</remarks>
//        public void setIP(string ip_address, string subnet_mask)
//        {
//            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
//            ManagementObjectCollection objMOC = objMC.GetInstances();

//            foreach (ManagementObject objMO in objMOC)
//            {
//                if ((bool)objMO["IPEnabled"])
//                {
//                    try
//                    {
//                        ManagementBaseObject setIP;
//                        ManagementBaseObject newIP =
//                            objMO.GetMethodParameters("EnableStatic");

//                        newIP["IPAddress"] = new string[] { ip_address };
//                        newIP["SubnetMask"] = new string[] { subnet_mask };

//                        setIP = objMO.InvokeMethod("EnableStatic", newIP, null);
//                    }
//                    catch (Exception)
//                    {
//                        throw;
//                    }


//                }
//            }
//        }
//        /// <summary>
//        /// Set's a new Gateway address of the local machine
//        /// </summary>
//        /// <param name="gateway">The Gateway IP Address</param>
//        /// <remarks>Requires a reference to the System.Management namespace</remarks>
//        public void setGateway(string gateway)
//        {
//            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
//            ManagementObjectCollection objMOC = objMC.GetInstances();

//            foreach (ManagementObject objMO in objMOC)
//            {
//                if ((bool)objMO["IPEnabled"])
//                {
//                    try
//                    {
//                        ManagementBaseObject setGateway;
//                        ManagementBaseObject newGateway =
//                            objMO.GetMethodParameters("SetGateways");

//                        newGateway["DefaultIPGateway"] = new string[] { gateway };
//                        newGateway["GatewayCostMetric"] = new int[] { 1 };

//                        setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
//                    }
//                    catch (Exception)
//                    {
//                        throw;
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// Set's the DNS Server of the local machine
//        /// </summary>
//        /// <param name="NIC">NIC address</param>
//        /// <param name="DNS">DNS server address</param>
//        /// <remarks>Requires a reference to the System.Management namespace</remarks>
//        public void setDNS(string NIC, string DNS)
//        {
//            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
//            ManagementObjectCollection objMOC = objMC.GetInstances();

//            foreach (ManagementObject objMO in objMOC)
//            {
//                if ((bool)objMO["IPEnabled"])
//                {
//                    // if you are using the System.Net.NetworkInformation.NetworkInterface you'll need to change this line to if (objMO["Caption"].ToString().Contains(NIC)) and pass in the Description property instead of the name 
//                    if (objMO["Caption"].Equals(NIC))
//                    {
//                        try
//                        {
//                            ManagementBaseObject newDNS =
//                                objMO.GetMethodParameters("SetDNSServerSearchOrder");
//                            newDNS["DNSServerSearchOrder"] = DNS.Split(',');
//                            ManagementBaseObject setDNS =
//                                objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
//                        }
//                        catch (Exception)
//                        {
//                            throw;
//                        }
//                    }
//                }
//            }
//        }
//        /// <summary>
//        /// Set's WINS of the local machine
//        /// </summary>
//        /// <param name="NIC">NIC Address</param>
//        /// <param name="priWINS">Primary WINS server address</param>
//        /// <param name="secWINS">Secondary WINS server address</param>
//        /// <remarks>Requires a reference to the System.Management namespace</remarks>
//        public void setWINS(string NIC, string priWINS, string secWINS)
//        {
//            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
//            ManagementObjectCollection objMOC = objMC.GetInstances();

//            foreach (ManagementObject objMO in objMOC)
//            {
//                if ((bool)objMO["IPEnabled"])
//                {
//                    if (objMO["Caption"].Equals(NIC))
//                    {
//                        try
//                        {
//                            ManagementBaseObject setWINS;
//                            ManagementBaseObject wins =
//                            objMO.GetMethodParameters("SetWINSServer");
//                            wins.SetPropertyValue("WINSPrimaryServer", priWINS);
//                            wins.SetPropertyValue("WINSSecondaryServer", secWINS);

//                            setWINS = objMO.InvokeMethod("SetWINSServer", wins, null);
//                        }
//                        catch (Exception)
//                        {
//                            throw;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}

public class BrowserInit
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

        public static CefSettings settings;

        public static async void Init(bool limitless, PersonData data = null)
        {
            using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            {
                if (data != null)
                {
                    GloableProfData.PData = data;

                    //try
                    //{
                    //    // Get the IP address and add it to the call context.
                    //    IPAddress ipAddr = (IPAddress)requestHeaders[CommonTransportKeys.IPAddress];
                    //    CallContext.SetData("ClientIP", ipAddr);
                    //}
                    //catch (Exception)
                    //{
                    //}

                    //sinkStack.Push(this, null);
                    //ServerProcessing srvProc = _NextSink.ProcessMessage(sinkStack, requestmessage, requestHeaders,
                    //    requestStream, out responseMessage, out responseHeaders, out responseStream);

                    //IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                    //string IPAddress = string.Empty;
                    //foreach (IPAddress ip in host.AddressList)
                    //{
                    //    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    //    {
                    //        IPAddress = ip.ToString();
                    //        break;
                    //    }
                    //}
                    //Console.WriteLine(IPAddress);
                    //Console.ReadKey();

                    //var config = new WINHTTP_PROXY_INFO();
                    //config.AccessType = AccessType.NoProxy;
                    ////config.Proxy = GloableProfData.PData.ProxyIP + ":" + GloableProfData.PData.ProxyPort;
                    ////config.Bypass = "intranet.com";

                    //var result = WinHttpSetDefaultProxyConfiguration(ref config);
                    //if (!result)
                    //{
                    //    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    //}
                    //else
                    //{
                    //    Console.WriteLine("Successfully modified proxy settings");
                    //}
                }
                //CefRuntime.EnableHighDpiSupport();
                try
                {
                    CefRuntime.Load();
                }
                catch { }


                var mainArgs = new CefMainArgs(new string[] { });
                var app = new DemoApp();
                var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
                if (exitCode != -1) return;

                var exePath = AppDomain.CurrentDomain.BaseDirectory;
                if(!limitless) exePath  = exePath + "\\BrowserAndFeatures.exe";
                else exePath = exePath + "\\Browser.Adapter.exe";
                exePath = exePath.Replace("\\\\", "\\");

                await Task.Run(() => { BrowserSettimgs.UserAgentChrome = MyFilesDatabase.GeChromeAgentRealQuick(GloableProfData.PData.ProjectName); });

                settings = new CefSettings
                {
                    BrowserSubprocessPath = exePath,
                    SingleProcess = false,
                    MultiThreadedMessageLoop = true,
                    PersistSessionCookies = true,
                    // LogSeverity = CefLogSeverity.ErrorReport | CefLogSeverity.Error | CefLogSeverity.Info | CefLogSeverity.Verbose | CefLogSeverity.Warning,
                    IgnoreCertificateErrors = false,
                    // UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2556.0 Safari/537.36",
                    // UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36",
                    UserAgent = BrowserSettimgs.UserAgentChrome,
                    //  RemoteDebuggingPort=123321,
                    NoSandbox = false,
                    ///BackgroundColor = new CefColor(1,11,23,122)
                    LogSeverity = CefLogSeverity.Disable,

                    //ResourcesDirPath = AppDomain.CurrentDomain.BaseDirectory,
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
                
                CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);
                //RegisterSchemes();
                //RegisterMessageRouter();

                //CefRuntime.AddCrossOriginWhitelistEntry("file", "https", "facebook.com", true);
                //CefRuntime.AddWebPluginDirectory(@"C:\Windows\system32\Macromed\Flash\");
                //CefRuntime.RefreshWebPlugins();

                Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_BrowserStart);

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                }
                catch { }
#if DEBUG
                int iiiiiiiiii = 0;
#else
            new Thread(() =>
            {
                string tmpdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp";
                int waited = 0;
                while (!Directory.Exists(tmpdir))
                {
                    Thread.Sleep(500); waited++;
                    if (waited == 10)
                        System.Windows.Application.Current.Dispatcher.Invoke(() => { MyException ex = new MyException("Un Known"); ex.Source = "IMG"; throw ex; });
                }
                waited = 0;
                string fpat = Path.Combine(tmpdir, "tmposbb11");
                while (!File.Exists(fpat))
                {
                    Thread.Sleep(500); waited++;
                    if (waited == 10)
                        System.Windows.Application.Current.Dispatcher.Invoke(() => { MyException ex = new MyException("Un Known"); ex.Source = "IMG"; throw ex; });
                }
                if (File.ReadAllText(fpat).Trim() != "check1")
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { MyException ex = new MyException("Un Known"); ex.Source = "IMG"; throw ex; });
                else
                    File.Delete(fpat);
            }).Start();
#endif
            }
        }

        static bool is64BitProcess = (IntPtr.Size == 8);
        static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return true;
                    }
                    return !retVal;
                }
            }
            else
            {
                return true;
            }
        }

        //private static void RegisterSchemes()
        //{
        //    CefRuntime.RegisterSchemeHandlerFactory("http", "dump-request-"+GloableProfData.PData.ProjectName, new DemoAppSchemeHandlerFactory());
        //}

        //private static void RegisterMessageRouter()
        //{
        //    if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
        //    {
        //        PostTask(CefThreadId.UI, RegisterMessageRouter);
        //        return;
        //    }

        //    // window.cefQuery({ request: 'my_request', onSuccess: function(response) { console.log(response); }, onFailure: function(err,msg) { console.log(err, msg); } });
        //   // DemoApp.BrowserMessageRouter = new CefMessageRouterBrowserSide(new CefMessageRouterConfig());
        //    ///DemoApp.BrowserMessageRouter.AddHandler(new DemoMessageRouterHandler());
        //}

        public static void PostTask(CefThreadId threadId, Action action)
        {
            CefRuntime.PostTask(threadId, new ActionTask(action));
        }

        internal sealed class ActionTask : CefTask
        {
            public Action _action;

            public ActionTask(Action action)
            {
                _action = action;
            }

            protected override void Execute()
            {
                _action();
                _action = null;
            }
        }

        public delegate void Action();

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


        //[DllImport("kernel32.dll", SetLastError = true)]
        //static extern int SetErrorMode(int wMode);

        //[DllImport("kernel32.dll")]
        //static extern FilterDelegate SetUnhandledExceptionFilter(FilterDelegate lpTopLevelExceptionFilter);
        //public delegate bool FilterDelegate(Exception ex);

        //[DllImport("kernel32", SetLastError = true)]
        //static extern bool FreeLibrary(IntPtr hModule);

        //[DllImport("kernel32.dll")]
        //static extern ErrorModes SetErrorMode(ErrorModes uMode);

        //[Flags]
        //public enum ErrorModes : uint
        //{
        //    SYSTEM_DEFAULT = 0x0,
        //    SEM_FAILCRITICALERRORS = 0x0001,
        //    SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
        //    SEM_NOGPFAULTERRORBOX = 0x0002,
        //    SEM_NOOPENFILEERRORBOX = 0x8000
        //}

        public static void Shutdown()
        {
            //try
            //{
            //    Organiser.Common.Classes.UsageTracker.AddTraceCookie("Browser Closed");
            //    Organiser.Common.Classes.UsageTracker.SaveAllTrackedDataList();
            //}
            //catch { }

            using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            {
                CefRuntime.Shutdown();
            }

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
