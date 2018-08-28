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

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Views
{
    /// <summary>
    /// Interaction logic for StatsBarView.xaml
    /// </summary>
    public partial class StatsBarView : UserControl
    {
        public string StatImageUrl
        {
            get { return (string)GetValue(StatImageUrlProperty); }
            set { SetValue(StatImageUrlProperty, value); }
        }
        public static readonly DependencyProperty StatImageUrlProperty =
            DependencyProperty.Register("StatImageUrl", typeof(string), typeof(StatsBarView), new UIPropertyMetadata(null));

        public string StatValue
        {
            get { return (string)GetValue(StatValueProperty); }
            set { SetValue(StatValueProperty, value); }
        }
        public static readonly DependencyProperty StatValueProperty =
            DependencyProperty.Register("StatValue", typeof(string), typeof(StatsBarView), new UIPropertyMetadata(null));

        public StatsBarView()
        {
            InitializeComponent();
        }
    }
}
