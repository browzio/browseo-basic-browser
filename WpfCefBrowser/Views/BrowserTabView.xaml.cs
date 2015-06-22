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
using WpfCefBrowser.ViewModels;

namespace WpfCefBrowser.Views
{
    /// <summary>
    /// Interaction logic for BrowserTabView.xaml
    /// </summary>
    public partial class BrowserTabView : UserControl
    {
        public static event Action<Xilium.CefGlue.WPF.WpfCefBrowser> OnSetBrowser = delegate { }; 
        public BrowserTabView()
        {
            InitializeComponent();

           // OnSetBrowser(browser);

                    //Address="{Binding Address, Mode=TwoWay}"
                    //Title="{Binding Title, Mode=OneWayToSource}"
                    //WebBrowser="{Binding WebBrowser, Mode=OneWayToSource}"

            //browser.RequestHandler = new RequestHandler();
            //browser.RegisterJsObject("bound", new BoundObject());

            //browser.MenuHandler = new Handlers.MenuHandler();
            //browser.GeolocationHandler = new Handlers.GeolocationHandler();
            //browser.DownloadHandler = new DownloadHandler();

            //CefExample.RegisterTestResources(browser);
        }

        private void OnTextBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void OnTextBoxGotMouseCapture(object sender, MouseEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }
    }
}
