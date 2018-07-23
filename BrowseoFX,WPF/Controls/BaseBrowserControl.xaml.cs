using BrowseoFX_WPF.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrowseoFX_WPF.Controls
{
    /// <summary>
    /// Interaction logic for BaseBrowserControl.xaml
    /// </summary>
    public partial class BaseBrowserControl : UserControl
    {
        private BaseBrowserViewModel baseBrowser;
        public BaseBrowserViewModel BaseBrowser
        {
            get
            {
                if (baseBrowser == null) baseBrowser = new BaseBrowserViewModel();
                return baseBrowser;
            }
        }
        
        public BaseBrowserControl()
        {
            InitializeComponent();

            this.Loaded += BrowserControl_Loaded;
        }



        private void BrowserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BrowserControl_Loaded;
            BaseBrowser.OnMainWebviewLoaded += BaseBrowser_OnMainWebviewLoaded;
            host.Children.Add(BaseBrowser.StartMainWebView());
        }


        private Action EmptyDelegate = delegate () { };
        public void BaseBrowser_OnMainWebviewLoaded()
        {
            host.UpdateLayout();
            BaseBrowser.MainWebView.UpdateLayout();
            //try
            //{
            //    BaseBrowser.MainWebView.Widget.BaseWindow.Instance.Repaint(true);
            //}
            //catch { }

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            host.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            BaseBrowser.MainWebView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
