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

namespace RssReader.Controlls
{
    /// <summary>
    /// Interaction logic for RssListsControl.xaml
    /// </summary>
    public partial class RssListsControl : UserControl
    {
        public RssListsControl()
        {
            InitializeComponent();
        }

        private void RssDisplayerControl_OnScrolled(int delta)
        {
            MyScrollViewer.ScrollToVerticalOffset(MyScrollViewer.VerticalOffset - delta);
        }
    }
}
