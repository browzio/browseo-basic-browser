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
    /// Interaction logic for SocialHeaderView.xaml
    /// </summary>
    public partial class SocialHeaderView : UserControl
    {
        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }
        public static readonly DependencyProperty ImageUrlProperty =
            DependencyProperty.Register("ImageUrl", typeof(string), typeof(SocialHeaderView), new UIPropertyMetadata(null));

        public string FromName
        {
            get { return (string)GetValue(FromNameProperty); }
            set { SetValue(FromNameProperty, value); }
        }
        public static readonly DependencyProperty FromNameProperty =
            DependencyProperty.Register("FromName", typeof(string), typeof(SocialHeaderView), new UIPropertyMetadata(null));

        public string ProfileUrl
        {
            get { return (string)GetValue(ProfileUrlProperty); }
            set { SetValue(ProfileUrlProperty, value); }
        }
        public static readonly DependencyProperty ProfileUrlProperty =
            DependencyProperty.Register("ProfileUrl", typeof(string), typeof(SocialHeaderView), new UIPropertyMetadata(null));

        public string CreatedAtFull
        {
            get { return (string)GetValue(CreatedAtFullProperty); }
            set { SetValue(CreatedAtFullProperty, value); }
        }
        public static readonly DependencyProperty CreatedAtFullProperty =
            DependencyProperty.Register("CreatedAtFull", typeof(string), typeof(SocialHeaderView), new UIPropertyMetadata(null));


        public SocialHeaderView()
        {
            InitializeComponent();
        }
    }
}
