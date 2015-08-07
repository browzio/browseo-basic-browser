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

namespace Youtuber.Controls
{
    /// <summary>
    /// Interaction logic for YputuberMainUserControl.xaml
    /// </summary>
    public partial class YputuberMainUserControl : UserControl
    {
        public YputuberMainUserControl()
        {
            InitializeComponent();
            DataContext = new YoutuberVM();
        }
    }
}
