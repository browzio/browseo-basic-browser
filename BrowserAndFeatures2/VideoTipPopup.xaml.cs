using mshtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrowserAndFeatures2
{
    /// <summary>
    /// Interaction logic for VideoTipPopup.xaml
    /// </summary>
    public partial class VideoTipPopup : System.Windows.Controls.UserControl
    {
        public string Url { get; set; }
        public string VideoUrl { get; set; }

        public VideoTipPopup()
        {
            InitializeComponent();
            Url = "";
        }

        private void btnOpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Url);
        }

        public void Navigate(string url,string videoUrl)
        {
            if (Url != "") return;
            Url = url;
            VideoUrl = videoUrl;

            string html = @"<iframe width=""613"" height=""613"" src="""+ VideoUrl + @""" frameborder=""0"" allowfullscreen></iframe>";
            //Webview.ign
            // Webview.ScriptErrorsSuppressed = true;
            if (!InternetExplorerBrowserEmulation.IsBrowserEmulationSet())
            {
                InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();
            }
            Webview.Navigate(videoUrl);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //Any code will be executed
            HTMLDocument doc = (HTMLDocument)Webview.Document; //Debuger execute this code line, It initialize HTMLDocument object
            IHTMLElementCollection buttons = doc.getElementsByTagName("video"); //But Debuger don't execute this one
            foreach (IHTMLElement el in buttons)
            {
                el.click();
               break;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = this.ActualWidth - 200;
            if (width > 0)
            {
                grdBrowser.Width = width;
            }

            var height = this.ActualHeight - 250;
            if(height > 0)
            {
                grdBrowser.Height = height;
            }
            
        }
    }
}
