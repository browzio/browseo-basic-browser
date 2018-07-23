using BrowseoFX_WPF.Crawlers;
using Gecko.GUI;
using Gecko.Windows;
using Gecko.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BrowseoFX_WPF.Windows
{
    /// <summary>
    /// Interaction logic for FBrowserWindow.xaml
    /// </summary>
    public partial class FBrowserWindow : Window , IWebviewListenerService
    {
        #region IWebviewListenerService
        public void BuiltWindowCore(GeckoXULWindow _widget)
        {
        }

        public void GloableBrowserDocumentLoaded()
        {
        }

        public void GloableViewLoadDone()
        {
        }
        #endregion

        public WebView MainWebView { get; set; }

        public WebView StartMainWebView()
        {
            MainWebView = new WebView();
            MainWebView.WebviewListenerService = this;
            MainWebView.Loaded += MainWebView_Loaded;
            return MainWebView;
        }

        private Action EmptyDelegate = delegate () { };
        private void MainWebView_Loaded(object sender, RoutedEventArgs e)
        {
            host.UpdateLayout();
            MainWebView.UpdateLayout();
            MainWebView.Widget.BaseWindow.Instance.Repaint(true);

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            host.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            MainWebView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }

        public FBrowserWindow()
        {
            InitializeComponent();

            Loaded += FBrowserWindow_Loaded;
            Closed += FBrowserWindow_Closed;
        }

        private void FBrowserWindow_Closed(object sender, EventArgs e)
        {
            MainWebView.Dispose();
        }

        private void FBrowserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            host.Children.Add(StartMainWebView());
        }
    }
}
