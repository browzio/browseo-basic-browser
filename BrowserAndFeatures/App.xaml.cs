using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BrowserAndFeatures
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>
    ///     // Starts the application. 
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class App : Application
    {
        public static bool browserinit = true;
        protected override void OnStartup(StartupEventArgs e)
        {
            browserinit = false;
            FeatureCallage.SetPersonData();

          //this.Shutdown();



            base.OnStartup(e);
        }
        //App()
        //{
        //    InitializeComponent();
        //}

        ///// <summary>
        ///// Application Entry Point.
        ///// </summary>
        //[System.STAThreadAttribute()]
        //[System.Diagnostics.DebuggerNonUserCodeAttribute()]
        //[System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        //public static void Main()
        //{
        //    // Add the event handler for handling UI thread exceptions to the event.
        //    System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Form1_UIThreadException);

        //    // Set the unhandled exception mode to force all Windows Forms errors to go through 
        //    // our handler.
        //    System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);

        //    // Add the event handler for handling non-UI thread exceptions to the event. 
        //    AppDomain.CurrentDomain.UnhandledException +=
        //         new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);


        //    BrowserAndFeatures.App app = new BrowserAndFeatures.App();
        //    app.InitializeComponent();
        //    app.Run();
        //}


        //// Handle the UI exceptions by showing a dialog box, and asking the user whether 
        //// or not they wish to abort execution. 
        //private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        //{
        //    MessageBox.Show("handled 2");
        //}

        //// Handle the UI exceptions by showing a dialog box, and asking the user whether 
        //// or not they wish to abort execution. 
        //// NOTE: This exception cannot be kept from terminating the application - it can only  
        //// log the event, and inform the user about it. 
        //private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    MessageBox.Show("handled");
        //}

        //[STAThread]
        //static void Main()
        //{
        //    MainWindow window = new MainWindow();
        //    App app = new App();
        //    app.Run(window);
        //}
    }
}
