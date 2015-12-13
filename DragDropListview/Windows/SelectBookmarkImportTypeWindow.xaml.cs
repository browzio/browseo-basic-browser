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

        private void browseoProj_Checked_1(object sender, RoutedEventArgs e)
        {
            browseoProj.Checked -= browseoProj_Checked_1;
            browseoGloable.Checked -= browseoProj_Checked_1;
            fcs.Checked -= browseoProj_Checked_1;
            entBud.Checked -= browseoProj_Checked_1;
            rankWyx.Checked -= browseoProj_Checked_1;

            browseoProj.IsChecked = browseoGloable.IsChecked = fcs.IsChecked = entBud.IsChecked = rankWyx.IsChecked = false;

            (sender as CheckBox).IsChecked = true;

            browseoProj.Checked += browseoProj_Checked_1;
            browseoGloable.Checked += browseoProj_Checked_1;
            fcs.Checked += browseoProj_Checked_1;
            entBud.Checked += browseoProj_Checked_1;
            rankWyx.Checked += browseoProj_Checked_1;
        }
    }
}
