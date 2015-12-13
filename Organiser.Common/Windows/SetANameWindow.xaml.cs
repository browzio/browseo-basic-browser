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
    /// Interaction logic for SetANameWindow.xaml
    /// </summary>
    public partial class SetANameWindow : Window
    {
        public bool OkClicked { get; set; }

        public SetANameWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            this.Close();
        }

        private void tbName_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                Button_Click(null, null);
        }
    }
}
