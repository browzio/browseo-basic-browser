using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xilium.CefGlue;
using Xilium.CefGlue.Wrapper;

namespace Browmium.WPF.WinForms
{
    internal sealed class BrowmiumApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if (GloableProfData.PData != null && !string.IsNullOrEmpty(GloableProfData.PData.ProxyIP) && !string.IsNullOrWhiteSpace(GloableProfData.PData.ProxyIP))
            {
                try
                {
                    commandLine.AppendSwitch("proxy-server", GloableProfData.PData.ProxyIP + ":" + GloableProfData.PData.ProxyPort);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("failed to set proxy");
                }
            }
        }
    }

    public class BrowserLibraryInit 
    {
        private static BrowserLibraryInit instance;
        public static BrowserLibraryInit Instance
        {
            get
            {
                if (instance == null) instance = new BrowserLibraryInit();
                return instance;
            }
        }

        private const string DumpRequestDomain = "dump-request.demoapp.cefglue.xilium.local";

        public CefSettings Settings { get; set; }

        public async void PlatformInitialize(PersonData data)
        {
            if(data != null) GloableProfData.PData = data;

            try
            {
                CefRuntime.Load();
            }
            catch (DllNotFoundException ex)
            {
                //MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (CefRuntimeException ex)
            {
                //MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var mainArgs = new CefMainArgs(new string[] { });
            var app = new BrowmiumApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
            if (exitCode != -1)
                return;

            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var localFolder = Path.GetDirectoryName(new Uri(codeBase).LocalPath);
            var browserProcessPath = Path.Combine(localFolder, "BrowserModules.exe");

            await Task.Run(() => { BrowserSettimgs.UserAgentChrome = MyFilesDatabase.GeChromeAgentRealQuick(GloableProfData.PData.ProjectName); });

            Settings = new CefSettings
            {
                BrowserSubprocessPath = browserProcessPath,
                SingleProcess = false,
                MultiThreadedMessageLoop = true,
                IgnoreCertificateErrors = false,
                NoSandbox = false,
                UserAgent = BrowserSettimgs.UserAgentChrome,
                LogSeverity = CefLogSeverity.Disable,
            };
            if (GloableProfData.PData != null)
            {
                string path = Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "Caches\\" + GloableProfData.PData.ProjectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                Settings.CachePath = path;
            }

            CefRuntime.Initialize(mainArgs, Settings, app, IntPtr.Zero);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Settings.MultiThreadedMessageLoop)
            {
                Application.Idle += (sender, e) => { CefRuntime.DoMessageLoopWork(); };
            }
        }

        public void ShutDown()
        {
            CefRuntime.Shutdown();
        }
    }
}
