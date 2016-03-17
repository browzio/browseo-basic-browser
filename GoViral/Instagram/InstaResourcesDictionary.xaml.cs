using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoViral.Instagram
{
    public partial class InstaResourcesDictionary
    {
        //private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    ScrollViewer sv = ((sender as ListView).Parent as ScrollViewer);
        //    if (sv == null) return;

        //    sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        //}

        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }


        //private void lv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    ScrollViewer sv = ((sender as Expander).Parent as Grid).Parent as ScrollViewer;
        //    if (sv == null) return;

        //    sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        //}

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

            if (tb.Text == "Add A Comment") tb.SelectAll();

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
