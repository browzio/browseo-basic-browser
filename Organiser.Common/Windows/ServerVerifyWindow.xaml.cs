using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ServerVerifyWindow.xaml
    /// </summary>
    public partial class ServerVerifyWindow : Window
    {
        public bool OKClicked { get; set; }

        public ServerVerifyWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            OKClicked = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
