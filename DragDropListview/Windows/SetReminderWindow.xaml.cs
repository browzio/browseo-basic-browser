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

namespace DragDropListview.Windows
{
    /// <summary>
    /// Interaction logic for SetReminderWindow.xaml
    /// </summary>
    public partial class SetReminderWindow : Window
    {
        public bool OkClicked { get; set; }
        public SetReminderWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;

            this.Close();
        }
    }
}
