
using Xilium.CefGlue;

namespace WpfCefBrowser
{
    public static class BrowserInit
    {
        public static void Init()
        {
            try
            {
                CefRuntime.Shutdown();
                CefRuntime.Load();
            }
            catch { }
            string[] s = new string[0] { };
            var mainArgs = new CefMainArgs(s);
            var app = new CefAppInit();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);

            var settings = new CefSettings
            {
                // BrowserSubprocessPath = @"D:\fddima\Projects\Xilium\Xilium.CefGlue\CefGlue.Demo\bin\Release\Xilium.CefGlue.Demo.exe",
                SingleProcess = true,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Disable,
                LogFile = "CefGlue.log",
            };

            CefRuntime.Initialize(mainArgs, settings, app);
        }
    }

    internal sealed class CefAppInit : CefApp
    {

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //23.94.20.23:80:simon:twatzz1836
            //23.94.23.105:80:simon:twatzz1836
            //173.232.99.54:80:simon:twatzz1836
            //23.94.20.30:80:simon:twatzz1836
            //192.171.233.149:80:simon:twatzz1836
            //173.232.116.111:80:simon:twatzz1836
            //173.44.58.101:80:simon:twatzz1836
            //23.94.243.249:80:simon:twatzz1836
            //173.232.99.26:80:simon:twatzz1836
            //23.94.23.75:80:simon:twatzz1836

            //if (!System.IO.File.Exists("C:\\file.txt"))
            //    commandLine.AppendSwitch("proxy-server", "23.94.20.30:80");
            //else
            //    commandLine.AppendSwitch("proxy-server", "192.171.233.149:80");

            //System.IO.File.Create("C:\\file.txt");

            base.OnBeforeCommandLineProcessing(processType, commandLine);
        }
    }
}
