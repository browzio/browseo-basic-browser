using DragDropListview.Windows;
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

namespace DragDropListview.Controls
{
    /// <summary>
    /// Interaction logic for DraggableListBox.xaml
    /// </summary>
    public partial class DraggableListBox : UserControl
    {
        public DragDropMainViewModel vm { get; set; }

        public DraggableListBox()
        {
            InitializeComponent();

            vm = DragDropMainViewModel.Instance;
            DataContext = vm;
        }

        public bool SaveSite(string site)
        {
            if (vm == null) return false;
            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.SetValues(site, site, vm.FoldersAndSitesList, DragDropMainViewModel.LastSelectedIndex);
            ebm.ShowDialog();

            if (ebm.SaveClicked)
            {
                DragDropMainViewModel.LastSelectedIndex = ebm.LastSelectedIndex;
                vm.SaveSite(ebm.tbURL.Text, ebm.tbName.Text, (ebm.cmbFolders.SelectedItem as ComboBoxItem).Tag, DateTime.Now.ToString());
                return true;
            }

            return false;
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (vm == null) return;
            vm.DoubleClickedFolderSide();
        }

        private void siteSide_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (vm == null) return;
            vm.DoubleClickedSitesSide();
        }

        public void EportSitesToTxt()
        {
            if (vm == null) return;
            vm.EportSitesToTxt();
        }
    }
}
