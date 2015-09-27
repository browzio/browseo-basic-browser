using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using Eli.Shapes;
using Organiser.Common.Classes;

namespace WPFPieChart
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public event Action<string> OnOpenDetailedClick = delegate { };//projectName
        private ObservableCollection<AssetClass> classes;

        public Window1()
        {   
            InitializeComponent();   
        }

        public void InitDataContext(List<BacklinkHistoryLine> lineData)
        {
            try
            {
                classes = new ObservableCollection<AssetClass>(AssetClass.ConstructTestData(lineData));
                Application.Current.Dispatcher.Invoke((Action)delegate { 
                this.DataContext = classes;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while trying to retreive the report please try again. " + ex.Message);
                this.Close();
            }
        }

        /// <summary>
        /// Handle clicks on the listview column heading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumn column = ((GridViewColumnHeader)e.OriginalSource).Column;
            piePlotter.PlottedProperty = column.Header.ToString();
        }

        private void AddNewItem(object sender, RoutedEventArgs e)
        {
            AssetClass asset = new AssetClass() { Class = "new class"};
            classes.Add(asset);
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            OnOpenDetailedClick(tbProjName.Text.Trim());
        }

    }
}
