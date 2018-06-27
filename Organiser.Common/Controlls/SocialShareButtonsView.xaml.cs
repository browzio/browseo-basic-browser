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

namespace Organiser.Common.Controlls
{
    /// <summary>
    /// Interaction logic for SocialShareButtonsView.xaml
    /// </summary>
    public partial class SocialShareButtonsView : UserControl
    {
        public event Action<string,string, string> OnClickedShareButton;
        public SocialShareButtonsView()
        {
            InitializeComponent();
        }

        object contextMnuBtn;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            contextMnuBtn = sender;
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (contextMnuBtn == null) return;
            (contextMnuBtn as Button).ContextMenu.IsOpen = false;

            var btn = sender as Button;
            if (btn == null) return;
            var commandParam = btn.CommandParameter as string;

            var result = DataContext as Organiser.Common.Classes.FacebookGraphPostResult;
            if (result != null)
            {
                //var postUrl = Tag as string;
                //if (postUrl == null) return;

                OnClickedShareButton?.Invoke(commandParam, result.link, result.picture);
            }

            var feedResult = DataContext as Organiser.Common.Classes.FeedData;
            if (feedResult == null) return;


            OnClickedShareButton?.Invoke(commandParam, feedResult.link, feedResult.picture);
        }

        public void SetBtnChromeVisiblility(Visibility visibility)
        {
            btnShareChrome.Visibility = visibility;
        }
    }
}
