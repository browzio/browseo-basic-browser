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

namespace BrowseoFX_WPF.Controls.Addons
{
    /// <summary>
    /// Interaction logic for FBSearchControl.xaml
    /// </summary>
    public partial class FBSearchControl : UserControl
    {
        public FBSearchControl()
        {
            InitializeComponent();
        }

        private void Expander_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            svSavedProjects.ScrollToVerticalOffset(svSavedProjects.VerticalOffset - e.Delta);
        }
    }
}
