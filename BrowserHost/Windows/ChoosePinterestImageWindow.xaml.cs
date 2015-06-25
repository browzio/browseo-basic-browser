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
using System.Windows.Shapes;

namespace BrowserHost.Windows
{
    /// <summary>
    /// Interaction logic for ChoosePinterestImageWindow.xaml
    /// </summary>
    public partial class ChoosePinterestImageWindow : Window
    {
        public bool OkClicked { get; set; }

        ScrollViewer sv;

        public ChoosePinterestImageWindow()
        {
            InitializeComponent();
           // this.Loaded += ChoosePinterestImageWindow_Loaded;
        }

        void ChoosePinterestImageWindow_Loaded(object sender, RoutedEventArgs e)
        {
           // sv = FindVisualChild<ScrollViewer>(list);
        }

        private void btnScrollLeft_Click(object sender, RoutedEventArgs e)
        {
           //// sv.ScrollToVerticalOffset(sv.HorizontalOffset - 1);
            if (list.SelectedIndex > 1)
            {
                list.SelectedIndex -= 1;
            }
            list.ScrollIntoView(list.SelectedItem);
        }

        private void btnScrollRight_Click(object sender, RoutedEventArgs e)
        {
           // sv.ScrollToVerticalOffset(sv.HorizontalOffset + 1);
            if (list.SelectedIndex < list.Items.Count - 2)
            {
                list.SelectedIndex += 1;
            }
            list.ScrollIntoView(list.SelectedItem);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            this.Close();
        }

    }
}