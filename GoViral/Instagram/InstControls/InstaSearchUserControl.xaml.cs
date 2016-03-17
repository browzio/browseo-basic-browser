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
    /// Interaction logic for InstaSearchUserControl.xaml
    /// </summary>
    public partial class InstaSearchUserControl : UserControl
    {
        public InstaSearchUserControl()
        {
            InitializeComponent();

            this.MouseMove += InstaSearchUserControl_MouseMove;
            this.PreviewMouseUp += InstaSearchUserControl_PreviewMouseUp;
        }

        private void lv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = ((sender as ListView).Parent as ScrollViewer);
            if (sv == null) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }

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

        Grid currentResizeGrid = null;

        private void resize_grdTags(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            this.Cursor = Cursors.SizeWE;
            currentResizeGrid = grdTags;
            oldVal = e.GetPosition(this).X;
            oldWidth = currentResizeGrid.ActualWidth;
        }

        private void resize_grdUsers(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            this.Cursor = Cursors.SizeWE;
            currentResizeGrid = grdUsers;
            oldVal = e.GetPosition(this).X;
            oldWidth = currentResizeGrid.ActualWidth;
        }

        private void resize_grdMedia(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            this.Cursor = Cursors.SizeWE;
            currentResizeGrid = grdMedia;
            oldVal = e.GetPosition(this).X;
            oldWidth = currentResizeGrid.ActualWidth;
        }

        double oldVal=0, newVal=0.0,oldWidth=0;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = (sender as MenuItem).Parent as ContextMenu;
            Expander exp= contextMenu.PlacementTarget as Expander;
            if (exp == null) return;

            exp.IsExpanded = false;
        }

        private void InstaSearchUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Cursor == null || 
                this.Cursor != Cursors.SizeWE || 
                currentResizeGrid == null || 
                Mouse.OverrideCursor == Cursors.Wait) return;

            newVal = e.GetPosition(this).X;

            if (newVal != oldVal && oldWidth + (newVal - oldVal) > 50)
            {
                currentResizeGrid.SetValue(Grid.WidthProperty, oldWidth + (newVal - oldVal));
            }
        }


        private void InstaSearchUserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.Cursor == Cursors.SizeWE) this.Cursor = null;
            if (currentResizeGrid != null) currentResizeGrid = null;
        }
    }
}
