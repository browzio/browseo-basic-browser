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

namespace RssReader.Views
{
    /// <summary>
    /// Interaction logic for RSSFeedView.xaml
    /// </summary>
    public partial class RSSFeedDataView : UserControl
    {
        public RSSFeedDataView()
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
        }
    }
}
