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

namespace BrowserHost.Views
{
    /// <summary>
    /// Interaction logic for SelectBookmarkImportTypeWindow.xaml
    /// </summary>
    public partial class SelectBookmarkImportTypeWindow : Window
    {
        public bool OkClicked { get; set; }
        public SelectBookmarkImportTypeWindow()
        {
            InitializeComponent();
            browseoProj.Checked += browseoProj_Checked;
            fcs.Checked += fcs_Checked;
        }

        private void browseoProj_Checked(object sender, RoutedEventArgs e)
        {
            fcs.Checked -= fcs_Checked;
            fcs.IsChecked = false;
            fcs.Checked += fcs_Checked;
        }

        private void fcs_Checked(object sender, RoutedEventArgs e)
        {
            browseoProj.Checked -= browseoProj_Checked;
            browseoProj.IsChecked = false;
            browseoProj.Checked += browseoProj_Checked;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
