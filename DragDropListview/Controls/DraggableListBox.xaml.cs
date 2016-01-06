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
        public DraggableListBox()
        {
            InitializeComponent();
            DataContext = DragDropMainViewModel.Instance;
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DragDropMainViewModel.Instance.DoubleClickedFolderSide();
        }

        private void siteSide_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DragDropMainViewModel.Instance.DoubleClickedSitesSide();
        }

        public void EportSitesToTxt()
        {
            DragDropMainViewModel.Instance.EportSitesToTxt();
        }
    }
}
