using BrowseoFX_WPF.Core;
using BrowseoFX_WPF.Windows;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrowseoFX.CMD
{
    class Program
    {
        //static int windowLaunchedNum = 1;
        static string url = "about:blank";

        [STAThread]
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();

            string projectPath = args[0];
            MyFilesDatabase.SetUpPdaaFromPath(projectPath);
            MyFilesDatabase.SetUpImacroProfileInfo();

            url = args[1];

           int windowLaunchedNum = Convert.ToInt32(args[2]);
            if (windowLaunchedNum > 5)
                windowLaunchedNum = new Random().Next(1, 5);

            if(args.Length >= 4)
            {
                //url = "imacros://run/?m=\"" + url + "\"";
                var datasourcePath = Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "BrowseoIA_DataSource", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(datasourcePath)) Directory.CreateDirectory(datasourcePath);
                
                var datasourceFile = Path.Combine(datasourcePath, "Datasource.txt");
                var datatsourceText = args[3];

                File.WriteAllText(datasourceFile, datatsourceText);
            }

            BrowserWindow window = new BrowserWindow();
            window.Title = GloableProfData.PData.ProjectName;
            window.Width = System.Windows.SystemParameters.WorkArea.Width / 2;
            window.Height = System.Windows.SystemParameters.WorkArea.Height / 2;
            window.Topmost = true;
            window.Closing += Window_Closing;
            switch (windowLaunchedNum)
            {
                case 1:
                    window.Left = 0;
                    window.Top = 0;
                    break;

                case 2:
                    window.Left = System.Windows.SystemParameters.WorkArea.Width / 2;
                    window.Top = 0;
                    break;

                case 3:
                    window.Left = 0;
                    window.Top = System.Windows.SystemParameters.WorkArea.Height / 2;
                    break;

                case 4:
                    window.Left = System.Windows.SystemParameters.WorkArea.Width / 2;
                    window.Top = System.Windows.SystemParameters.WorkArea.Height / 2;
                    break;

                case 5:
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    break;

                default:
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    break;
            }

            BrowseoFXManager.Instance.OnReadyToBrowse += Instance_OnReadyToBrowse;

            Application app = new Application();
            app.Run(window);
        }

        private static void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                BrowseoFXManager.Instance.Shutdown();
            }
            catch { }
        }

        private static void Instance_OnReadyToBrowse()
        {
            BrowseoFXManager.Instance.OnReadyToBrowse -= Instance_OnReadyToBrowse;
            //await Task.Delay(2000);
            BrowseoFXManager.Instance.GloableWebView.Navigate(url);
        }
    }
}
