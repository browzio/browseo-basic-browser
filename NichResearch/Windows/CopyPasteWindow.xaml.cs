using NichResearch.Models;
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
using WindowsInput;

namespace NichResearch.Windows
{
    /// <summary>
    /// Interaction logic for CopyPasteWindow.xaml
    /// </summary>
    public partial class CopyPasteWindow : Window
    {
        List<string> windows = new List<string>();

        public static bool HasToPaste;
        public CopyPasteWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var grid = sender as DataGrid;
                if (grid.CurrentCell.Column.Header.ToString() == "Title")
                {
                    Clipboard.SetText((grid.CurrentCell.Item as CopyPasteItem).Title);
                }
                else
                {
                    Clipboard.SetText((grid.CurrentCell.Item as CopyPasteItem).Link);
                }
                HasToPaste = true;
            }
            catch { }
        }
    }
}
