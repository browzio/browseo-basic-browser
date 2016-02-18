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

namespace Organiser.Common.Controlls
{
    /// <summary>
    /// Interaction logic for SocialButtonsUserControl.xaml
    /// </summary>
    public partial class SocialButtonsUserControl : UserControl
    {
        public event Action<string> OnClickedSocialButtons = delegate { };
        public SocialButtonsUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OnClickedSocialButtons((sender as Button).Name);
        }
    }
}
