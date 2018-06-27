using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BrowserAndFeatures2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        bool debug = false;

        public static bool browserinit = true;

        protected override void OnStartup(StartupEventArgs e)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            if (debug)
            {
                browserinit = false;
                FeatureCallage2.SetPersonData();
            }
            else
            {
                this.Shutdown();

            }
        }
    }
}