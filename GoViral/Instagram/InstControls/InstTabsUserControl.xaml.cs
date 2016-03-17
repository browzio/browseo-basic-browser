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

namespace GoViral.Instagram.InstControls
{
    /// <summary>
    /// Interaction logic for InstTabsUserControl.xaml
    /// </summary>
    public partial class InstTabsUserControl : UserControl
    {
        public InstTabsUserControl()
        {
            InitializeComponent();
        }

        private void InstaSearchVM_OnSendToDominate(InstModels.InstaResponseLists responseList)
        {
            if(cntrlDominator.DataContext is InstViewModels.InstaDominateVM)
            {
                (cntrlDominator.DataContext as InstViewModels.InstaDominateVM).OnReceivedFromSearch(responseList);
            }
        }

        private void InstaSearchVM_OnSendContentToSorter(string content)
        {
            if(cntrlSorter.DataContext is ViewModels.SyncedProjectsVM)
            {
                (cntrlSorter.DataContext as ViewModels.SyncedProjectsVM).AddUrlToSavedProjectList("", "", content);
            }
        }
    }
}
