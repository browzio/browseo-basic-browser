using GoViral.Models;
using GoViral.ViewModels;
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

namespace GoViral.Controls
{
    /// <summary>
    /// Interaction logic for SharedPostsUserControl.xaml
    /// </summary>
    public partial class SharedPostsUserControl : UserControl
    {
        public event Action<string> OnBrowserNavigateToUrl = delegate { };

        public SyncedProjectsVM ViewModel
        {
            get;
            set;
        }
        public SharedPostsUserControl()
        {
            InitializeComponent();
            if (ViewModel == null)
            {
                ViewModel = new SyncedProjectsVM();
                DataContext = ViewModel;
            }  
        }

        private void CheckBox_PreviewMouseUp_ChangeSI(object sender, MouseButtonEventArgs e)
        {
            SyncedProjectData data = null;  
            if (sender is TextBox)
            {
                data = ((sender as TextBox).DataContext as SyncedProjectData);
            }
            else if (sender is CheckBox)
            {
                data = ((sender as CheckBox).DataContext as SyncedProjectData);
            }
            else if (sender is TextBlock)
            {
                data = ((sender as TextBlock).DataContext as SyncedProjectData);
            }
            if (data == null) return;

            ViewModel.SetCorrectSI(data);
        }

        private void exProjects_Expanded(object sender, RoutedEventArgs e)
        {
            SavedSyncProject syncedProj = (sender as Expander).DataContext as SavedSyncProject;
            if (syncedProj == null) return;

            ViewModel.SISavedProjectsList = ViewModel.SavedProjectsList.IndexOf(syncedProj);
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {  
            if(e.ClickCount == 2)
            {
                SyncedProjectData data = null;
                if (sender is TextBox)
                {
                    data = ((sender as TextBox).DataContext as SyncedProjectData);
                }
                else if (sender is Grid)
                {
                    data = ((sender as Grid).DataContext as SyncedProjectData);
                }
                else if (sender is TextBlock)
                {
                    data = ((sender as TextBlock).DataContext as SyncedProjectData);
                }
                if (data == null) return;

                OnBrowserNavigateToUrl(data.Url);
            }
        }


        public static double TbMaxWidth = 0.0;
        private void tbProjName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double aWidth = (sender as TextBlock).ActualWidth;

            if (TbMaxWidth < aWidth)
            {
                TbMaxWidth = aWidth;
                foreach (var p in ViewModel.SavedProjectsList)
                {
                    p.RaisePropertyChanged("Width");
                }  
            }
        }

        private void lvProjectSyndicates_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Border border = (Border)VisualTreeHelper.GetChild(lvProjects, 0);
            if (border == null) return;
            ScrollViewer sv = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            if (sv == null) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }
    }
}
