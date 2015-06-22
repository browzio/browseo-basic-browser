using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;

namespace BrowserHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] args;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //args = e.Args;
            //try
            //{
            //    CefRuntime.Load();
            //}
            //catch { }
            ////string[] s = new string[0] { };
            //var mainArgs = new CefMainArgs(new[] { "--force-renderer-accessibility" });
            //var app = new DemoApp();

            //var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);

            //var settings = new CefSettings
            //{
            //    // BrowserSubprocessPath = @"D:\fddima\Projects\Xilium\Xilium.CefGlue\CefGlue.Demo\bin\Release\Xilium.CefGlue.Demo.exe",
            //    SingleProcess = true,
            //    MultiThreadedMessageLoop = true,
            //    LogSeverity = CefLogSeverity.Disable,
            //    LogFile = "CefGlue.log",
            //};


            //CefRuntime.Initialize(mainArgs, settings, app);
        }
    }
}
