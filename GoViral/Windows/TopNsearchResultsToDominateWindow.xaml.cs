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

namespace GoViral.Windows
{
    /// <summary>
    /// Interaction logic for TopNsearchResultsToDominateWindow.xaml
    /// </summary>
    public partial class TopNsearchResultsToDominateWindow : Window
    {
        public int MaxNums { get; set; }

        public TopNsearchResultsToDominateWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int maxNums = 0;
            if (Int32.TryParse(tbMax.Text, out maxNums))
            {
                MaxNums = maxNums;
                if (MaxNums > 0)
                {
                    this.DialogResult = true;
                    this.Close();
                    return;
                }
            }

            MessageBox.Show("Enter a valid number larger then 0, " + tbMax.Text + ", is not valid.");
        }
    }
}
