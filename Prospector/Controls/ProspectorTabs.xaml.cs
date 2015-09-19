using Prospector.ViewModels;
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

namespace Prospector.Controls
{
    /// <summary>
    /// Interaction logic for ProspectorTabs.xaml
    /// </summary>
    public partial class ProspectorTabs : UserControl
    {
        string currentSenderContent = "";

        public ProspectorTabs()
        {
            InitializeComponent();
            DataContext = new FootPrintsOptionsVM();
            var bc = new BrushConverter();
            btnComment.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = btnComment.Content.ToString();
        }

        private void btnComment_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Comment_Backlinks;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnComment.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");

            currentSenderContent = (sender as Button).Content.ToString();
        }

        private void btnForum_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Forum;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnForum.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = (sender as Button).Content.ToString();
        }

        private void btnGuest_Posts_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Guest_Posts;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnGuest_Posts.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = (sender as Button).Content.ToString();
        }

        private void btnBlog_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Blogs;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnBlog.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = (sender as Button).Content.ToString();
        }

        private void btnLink_Roundups_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Link_Roundups;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnLink_Roundups.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = (sender as Button).Content.ToString();
        }

        private void btnCustom_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Custom;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnCustom.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = (sender as Button).Content.ToString();
        }


        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            (sender as Button).Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            string currentHeader = (sender as Button).Content.ToString();
            if (currentSenderContent == currentHeader) return;

            var bc = new BrushConverter();
            (sender as Button).Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
        }

        
        private void setButtonsWhite()
        {
            var bc = new BrushConverter();
            btnComment.Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
            btnForum.Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
            btnGuest_Posts.Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
            btnBlog.Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
            btnLink_Roundups.Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
            btnCustom.Background = (Brush)bc.ConvertFrom("#FFFDFDFD");
        }

        private void btnSaved_Click(object sender, RoutedEventArgs e)
        {
            tbContrl.SelectedIndex = FootPrintsOptionsVM.Saved;
            setButtonsWhite();
            var bc = new BrushConverter();
            btnSaved.Background = (Brush)bc.ConvertFrom("#FFD7D7D7");
            currentSenderContent = (sender as Button).Content.ToString();
        }
    }
}
