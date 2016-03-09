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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoViral.Instagram.InstControls
{
    /// <summary>
    /// Interaction logic for InstaSearchUserControl.xaml
    /// </summary>
    public partial class InstaSearchUserControl : UserControl
    {
        public InstaSearchUserControl()
        {
            InitializeComponent();
        }

        private void lv_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = ((sender as ListView).Parent as ScrollViewer);
            if (sv == null) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

            if (tb.Text == "Add A Comment") tb.SelectAll();

        }
    }
}
