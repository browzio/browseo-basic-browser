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
using WPF_WYSIWYG_HTML_Editor.Helpers;

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for SpinWindow.xaml
    /// </summary>
    public partial class SpinWindow : Window
    {
        public event Action<string> OnClickedSpin = delegate { };

        public SpinWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbInputedText.Text != null)
            {
                OnClickedSpin(Spinner.Spin(tbInputedText.Text));
            }
        }
    }
}
