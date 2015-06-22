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

namespace RssReader.Controlls
{
    /// <summary>
    /// Interaction logic for RssDisplayerControl.xaml
    /// </summary>
    public partial class RssDisplayerControl : UserControl
    {
        public RssDisplayerControl()
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

        private void ListView_MouseEnter(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = FindVisualChild<ScrollViewer>(sender as ListView);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        private void ListView_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = FindVisualChild<ScrollViewer>(sender as ListView);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
       where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        } 
    }
}
