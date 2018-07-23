using BrowseoFX_WPF.Core;
using BrowseoFX_WPF.Core.DataAccess;
using Gecko;
using Gecko.Windows;
using Gecko.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrowseoFX_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.Loaded += MainWindow_Loaded;
           this.Closing += MainWindow_Closing;
            //var t = new System.Windows.Controls.UserControl();

            ////ensure win32 handle is created
            //var handle = new WindowInteropHelper(this).EnsureHandle();

            ////set window background
            //var result = SetClassLong(handle, GCL_HBRBACKGROUND, GetSysColorBrush(COLOR_WINDOW));
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BrowseoFXManager.Instance.Shutdown();
        }

        //public static IntPtr SetClassLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        //{
        //    //check for x64
        //    if (IntPtr.Size > 4)
        //        return SetClassLongPtr64(hWnd, nIndex, dwNewLong);
        //    else
        //        return new IntPtr(SetClassLongPtr32(hWnd, nIndex, unchecked((uint)dwNewLong.ToInt32())));
        //}

        //private const int GCL_HBRBACKGROUND = -10;
        //private const int COLOR_WINDOW = 5;

        //[DllImport("user32.dll", EntryPoint = "SetClassLong")]
        //public static extern uint SetClassLongPtr32(IntPtr hWnd, int nIndex, uint dwNewLong);

        //[DllImport("user32.dll", EntryPoint = "SetClassLongPtr")]
        //public static extern IntPtr SetClassLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        //[DllImport("user32.dll")]
        //static extern IntPtr GetSysColorBrush(int nIndex);

        //private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //        await BrowseoFXManager.Instance.Init();
        //    host.Children.Add(BrowseoFXManager.Instance.GloableWebView);
        //}


        //private void Host_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (BrowseoFXManager.Instance.GloableWebView == null ||
        //        BrowseoFXManager.Instance.GloableWebView.Widget == null ||
        //        BrowseoFXManager.Instance.GloableWebView.Widget.BaseWindow == null) return;

        //    BrowseoFXManager.Instance.GloableWebView.Widget.BaseWindow.Instance.Repaint(true);

        //    host.UpdateLayout();
        //    BrowseoFXManager.Instance.GloableWebView.UpdateLayout();
        //}
    }
}
