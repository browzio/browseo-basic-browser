using Gecko;
using Gecko.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using zFirefoxXulBrowser.API;

namespace zFirefoxXulBrowser.Controls
{
    /// <summary>
    /// Interaction logic for FFXulBrowserUserControl.xaml
    /// </summary>
    public partial class FFXulBrowserUserControl : UserControl
    {
        public static WebView pageView;

        static bool didInitializeHere = false;

        public FFXulBrowserUserControl()
        {
            InitializeComponent();

            this.Loaded += FFXulBrowserUserControl_Loaded;
        }

        private void FFXulBrowserUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (didInitializeHere) return;
            didInitializeHere = true;

            this.Loaded -= FFXulBrowserUserControl_Loaded;
            
            Initializer.Init();
            
            pageView = new WebView();
           // pageView.GetOrCreateGlobalViewDone += PageView_OnWndProcDone;
            host.Children.Add(pageView);
        }


        //private void PageView_OnWndProcDone()
        //{
        //    pageView.Widget.View.OnClickedUIListener += Window_OnClickedUIListener;

        //    pageView.Widget.BaseWindow.Instance.Repaint(true);

        //    host.UpdateLayout();
        //    pageView.UpdateLayout();
        //}


        //private void Window_OnClickedUIListener(Gecko.GUI.PanelUIListenerState state)
        //{
        //    //host.Width = host.ActualWidth - 400;

        //    //pageView.Widget.BaseWindow.Instance.Repaint(true);

        //    //host.UpdateLayout();
        //    //pageView.UpdateLayout();
        //}
    }
}
