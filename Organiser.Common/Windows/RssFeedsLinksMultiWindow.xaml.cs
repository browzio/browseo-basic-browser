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
    /// Interaction logic for RssFeedsLinksMultiWindow.xaml
    /// </summary>
    public partial class RssFeedsLinksMultiWindow : Window
    {
        public bool ButtonLeftClicked { get; set; }
        public bool ButtonRightClicked { get; set; }

        public RssFeedsLinksMultiWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonLeftClicked = true;
            try
            {
                this.DialogResult = true;
            }
            catch { }
            this.Close();
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            ButtonRightClicked = true;
            try
            {
                this.DialogResult = true;
            }
            catch { }
            this.Close();
        }
    }
}
