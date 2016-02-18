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

        public ChoosePinterestImageWindow()
        {
            InitializeComponent();;
            this.Closed += ChoosePinterestImageWindow_Closed;
        }

        private void ChoosePinterestImageWindow_Closed(object sender, EventArgs e)
        { 
            list.DataContext = null;
            list.ItemsSource = null;
        }

        private void btnScrollLeft_Click(object sender, RoutedEventArgs e)
        { 
            if (list.SelectedIndex >= 1)
            {
                list.SelectedIndex -= 1;
            }
            list.ScrollIntoView(list.SelectedItem);
        }

        private void btnScrollRight_Click(object sender, RoutedEventArgs e)
        { 
            if (list.SelectedIndex <= list.Items.Count - 2)
            {
                list.SelectedIndex += 1;
            }
            list.ScrollIntoView(list.SelectedItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            this.Close();
        } 
    }
}