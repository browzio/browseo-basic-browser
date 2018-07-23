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

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Views
{
    /// <summary>
    /// Interaction logic for GoogleEyeView.xaml
    /// </summary>
    public partial class GoogleEyeView : UserControl
    {
        public GoogleEyeView()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            grdYoutubeOptions.Visibility = Visibility.Collapsed;
        }

        private void btnYoutubeOptions_Click(object sender, RoutedEventArgs e)
        {
            grdYoutubeOptions.Visibility = Visibility.Visible;
        }

        private void ViewBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            switch (btn.Name)
            {
                case "btnYoutubeView":
                    grdYoutube.Visibility = Visibility.Visible;
                    grdGooglePlus.Visibility = Visibility.Collapsed;
                    break;

                case "btnGPlusView":
                    grdYoutube.Visibility = Visibility.Collapsed;
                    grdGooglePlus.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
