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

namespace Organiser.Common.Windows
{
    /// <summary>
    /// Interaction logic for AddLinkDataWindow.xaml
    /// </summary>
    public partial class SetNameAndDataWindow : Window
    {
        public bool OkClicked { get; set; }

        public SetNameAndDataWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            this.Focus();
            tbInputText.Focus();
        }

        private void tbInputText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkClicked = true;
                this.Close();
            }
        }
    }
}
