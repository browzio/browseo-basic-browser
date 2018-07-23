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
    /// Interaction logic for KeyValDisplayControl.xaml
    /// </summary>
    public partial class KeyValDisplayControl : UserControl
    {
        public string ParamTitle
        {
            get { return (string)GetValue(ParamTitleProperty); }
            set { SetValue(ParamTitleProperty, value); }
        }
        public static readonly DependencyProperty ParamTitleProperty =
            DependencyProperty.Register("ParamTitle", typeof(string), typeof(KeyValDisplayControl), new UIPropertyMetadata("Title: "));

        public string ParamValue
        {
            get { return (string)GetValue(ParamValueProperty); }
            set { SetValue(ParamValueProperty, value); }
        }
        public static readonly DependencyProperty ParamValueProperty =
            DependencyProperty.Register("ParamValue", typeof(string), typeof(KeyValDisplayControl), new UIPropertyMetadata(null));

        public KeyValDisplayControl()
        {
            InitializeComponent();

           // DataContext = this;
        }
    }
}
