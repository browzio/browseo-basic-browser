using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using zFirefoxBrowser;
using zFirefoxBrowser.Helpers;
using zFirefoxBrowser.ViewModels;

namespace AnyProjFFProcess
{
    class Program 
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

        static AnyProjWindow ffWindow;
        static bool closeOnFinish = false;
        static string datasource = "";
        static List<string> paths = null;
        static int TimesToPlay = 1,windowLaunchedNum = 1;
        static SemaphoreSlim SsemaphoreSlim = new SemaphoreSlim(1, 1);

        [STAThread]
        static void Main(string[] args)
        {
            using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            {
               // Debugger.Launch();
                //set up project data
                string projectPath = args[0];
                MyFilesDatabase.SetUpPdaaFromPath(projectPath);

                FoxInit.Init();

                if (args.Length == 3)
                {
                    InitWithMacroPath(args[1], false);
                }
                else
                {
                    closeOnFinish = Convert.ToBoolean(args[1]);

                    datasource = args[2];

                    string macroPath = args[3];
                    if (macroPath.Contains(MyFilesDatabase.SPLITTER))
                    {
                        paths = macroPath.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    windowLaunchedNum = Convert.ToInt32(args[4]);

                    if (args.Length == 6)
                    {
                        TimesToPlay = Convert.ToInt32(args[5]);
                    }

                    MyFilesDatabase.GetSavedSesstion(GloableProfData.PData.ProjectName, true);
                    FoxInit.SetSettings();
                    InitWithMacroPath(macroPath, true);
                }
            }
        }

        private static void InitWithMacroPath(string macroPathorurl, bool isMacroInit)
        {
            string starturl = macroPathorurl;
            if (isMacroInit) starturl = MyFilesDatabase.GetDefultHomePage();
            FoxTabViewModel btvm = null;
            ffWindow = new AnyProjWindow() { Title = GloableProfData.PData.ProjectName };
            ffWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            if (windowLaunchedNum > 5)
            {
                windowLaunchedNum = new Random().Next(1, 5);
            }
            //Width = System.Windows.SystemParameters.WorkArea.Width;
            //Height = System.Windows.SystemParameters.WorkArea.Height;
            switch (windowLaunchedNum)
            {
                case 1:
                    ffWindow.Left = 0;
                    ffWindow.Top = 0;
                    break;

                case 2:
                    ffWindow.Left = System.Windows.SystemParameters.WorkArea.Width / 2;
                    ffWindow.Top = 0;
                    break;

                case 3:
                    ffWindow.Left = 0;
                    ffWindow.Top = System.Windows.SystemParameters.WorkArea.Height / 2;
                    break;

                case 4:
                    ffWindow.Left = System.Windows.SystemParameters.WorkArea.Width / 2;
                    ffWindow.Top = System.Windows.SystemParameters.WorkArea.Height / 2;
                    break;

                case 5:
                    ffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    break;

                default:
                    ffWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    break;
            }
            ffWindow.Loaded += async (s, e) =>
            {
                await FoxInit.AwaitforProxySet();

                btvm = new FoxTabViewModel(starturl);
                btvm.IsFromIA = true;
                ffWindow.DataContext = btvm;
                btvm.OnBringToFrontForPaste += () => 
                {
                    ffWindow.Topmost = true;
                    ffWindow.Topmost = false;
                    ffWindow.Activate();
                };
            };

            if (isMacroInit)
            {
                // ffWindow.browserTabView.IsFromAnyProcess = true;
                // ffWindow.browserTabView.MacroflyOut.IsExpanded = true;
                ffWindow.Topmost = false;
                ffWindow.browserTabView.openMacros_Click(null, null);
                ffWindow.browserTabView.OnInitializedMacros += async () =>
                {
                    if (btvm == null)
                    {
                        await Task.Run(() => { while (btvm == null) { Thread.Sleep(250); } });
                    }
                    MacroManger managerForTab = ffWindow.browserTabView.MacroflyOut.DataContext as MacroManger;
                    managerForTab.DataSourceSlideoutText = datasource;
                    managerForTab.MaxLoop = TimesToPlay;
                    if (managerForTab.MaxLoop < 1) managerForTab.MaxLoop = 1;

                    if (managerForTab != null)
                    {
                        int timesFinished = 0;
                        managerForTab.OnMacroDone += () =>
                        {
                            //try
                            //{
                            //    if (paths != null && paths.Count != 0)
                            //    {
                            //        SsemaphoreSlim.Release();
                            //        return;
                            //    }
                            //}
                            //catch { return; }
                            timesFinished++;
                            if (paths != null && paths.Count > timesFinished) return;

                            if (closeOnFinish)
                            {
                                ffWindow.Close();
                            }
                        };
                        ffWindow.Closing += FfWindow_Closing;
                        if (paths == null)
                        {
                            paths = new List<string>();
                            paths.Add(macroPathorurl);
                        }
                        await managerForTab.SetMacroActiveByPaths(paths);
                        //{
                        //    //for (int i = 0; i < paths.Count; i++)
                        //    //{
                        //    //    for (int j = 0; j < TimesToPlay; j++)
                        //    //    {
                        //    //        await SsemaphoreSlim.WaitAsync();
                        //    //        string fPathToM = paths[i];
                        //    //        managerForTab.SetMacroActiveByPath(fPathToM);
                        //    //    }
                        //    //    paths.RemoveAt(i);
                        //    //}
                        //   await  managerForTab.SetMacroActiveByPaths(paths);
                        //}
                        //else
                        //{
                        //  await managerForTab.SetMacroActiveByPath(macroPathorurl);
                        //}
                    }
                };
            }

            Application app = new Application();
            app.Run(ffWindow);
        }

        private static void FfWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
            {
                try
                {
                    FoxInit.Shutdown();
                }
                catch { }
            }
        }

        //public void Dispose()
        //{
        //    //try
        //    //{
        //    //    if(ffWindow != null)
        //    //    {
        //    //        FoxInit.Shutdown();
        //    //    }
        //    //}
        //    //catch { }
        //}
    }
}
