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

namespace Prospector
{
    /// <summary>
    /// Interaction logic for SaveFootprintWindow.xaml
    /// </summary>
    public partial class SaveFootprintWindow : Window
    {
        public bool OkClicked { get; set; }

        public SaveFootprintWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            this.Close();
        }
    }
}
