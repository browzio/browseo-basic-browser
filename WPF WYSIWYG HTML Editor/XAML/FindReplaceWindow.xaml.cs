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

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for FindReplaceWindow.xaml
    /// </summary>
    public partial class FindReplaceWindow : Window
    {
        bool clickedOnce;
        public event Action<string, bool> OnClickedFind = delegate { };
        public event Action<string,string, bool> OnClickedFindReplace = delegate { };
        public event Action<string,string> OnClickedReplaceAll = delegate { };

        public FindReplaceWindow()
        {
            InitializeComponent();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            OnClickedFind(tbFindText.Text, clickedOnce);
            clickedOnce = true;
        }

        private void btnFR_Click(object sender, RoutedEventArgs e)
        {
            OnClickedFindReplace(tbFindText.Text, tbReplace.Text, clickedOnce);
            clickedOnce = true;
        }

        private void btnReplaceA_Click(object sender, RoutedEventArgs e)
        {
            OnClickedReplaceAll(tbFindText.Text, tbReplace.Text);
            clickedOnce = true;
        }
    }
}
