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

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for SelectPlatformWindow.xaml
    /// </summary>
    public partial class SelectPlatformWindow : Window
    {
        public SelectPlatformWindow()
        {
            InitializeComponent();
        }

        private void cbWP_Checked(object sender, RoutedEventArgs e)
        {
            cmbWP.Visibility = Visibility.Visible;
        }
        private void cbWP_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbWP.Visibility = Visibility.Collapsed;
        }

        private void cbDrupal_Checked(object sender, RoutedEventArgs e)
        {
            cmbDrupal.Visibility = Visibility.Visible;
        }

        private void cbDrupal_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbDrupal.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            boxWP.SelectedIndex = 0;
            Boxdruple.SelectedIndex = 0;
        }
    }
}
