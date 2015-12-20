using GoViral.ViewModels;
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

namespace GoViral.Controls
{
    /// <summary>
    /// Interaction logic for SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        public SearchVM ViewModel { get; set; }

        public SearchUserControl()
        {
            InitializeComponent();

            if(ViewModel == null)
            {
                ViewModel = new SearchVM();
                this.DataContext = ViewModel;
            }
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv == null) return;

            if (lv.Tag == null)
            {
                svLists.ScrollToVerticalOffset(svLists.VerticalOffset - e.Delta);
            }
            else
            { 
                Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
                if (border == null) return;
                ScrollViewer sv = border.Child as ScrollViewer;
                if (sv == null) return;
                sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            }
        }

        private void lvData_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv == null) return;    
            lv.Tag = "Y";
        }

        private void lvData_MouseLeave(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;
            if (lv == null) return;   
            lv.Tag = null;
        }

        private void miCollpse_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;

            var contextMenu = menuItem.Parent as ContextMenu;
            if (contextMenu == null)
                return;

            var listView = contextMenu.PlacementTarget as ListView;
            if (listView == null)
                return;

            var expander = listView.Parent as Expander;
            if (expander == null)
                return;

            expander.IsExpanded = false;
        }
    }
}
