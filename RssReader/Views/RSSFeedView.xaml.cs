using BrowseoFX_WPF.Core;
using Organiser.Common.Classes;
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
    public partial class RSSFeedView : UserControl
    {

        public RSSFeedView()
        {
            InitializeComponent();
        }

        private void lv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
            if (border == null) return;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            if (scrollViewer == null) return;

            var amount = e.Delta > 0 ? 1 : -1;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - amount);
        }
    }
}
