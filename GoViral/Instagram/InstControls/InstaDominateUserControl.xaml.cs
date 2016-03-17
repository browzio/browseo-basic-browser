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

namespace GoViral.Instagram.InstControls
{
    /// <summary>
    /// Interaction logic for InstaDominateUserControl.xaml
    /// </summary>
    public partial class InstaDominateUserControl : UserControl
    {
        public InstaDominateUserControl()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = ((sender as ListView).Parent as ScrollViewer);
            if (sv == null) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }

        private void lv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = ((sender as Expander).Parent as Grid).Parent as ScrollViewer;
            if (sv == null) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            (sender as Expander).Width = 500;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            (sender as Expander).Width = double.NaN;
        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (sender as Button).ContextMenu.IsEnabled = true;
                (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
                (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                (sender as Button).ContextMenu.IsOpen = true;
            }
            catch { }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = (sender as MenuItem).Parent as ContextMenu;
            Expander exp = contextMenu.PlacementTarget as Expander;
            if (exp == null) return;

            exp.IsExpanded = false;
        }
    }
}
