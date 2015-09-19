using Organiser.Common.Classes;
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

namespace Prospector
{
    /// <summary>
    /// Interaction logic for SaveMozKeysWindow.xaml
    /// </summary>
    public partial class SaveMozKeysWindow : Window
    {
        public bool OKClicked { get; set; }

        public SaveMozKeysWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mozDir = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Prospector", "ApiKeys");
                if (!System.IO.Directory.Exists(mozDir)) System.IO.Directory.CreateDirectory(mozDir);

                string filePath = System.IO.Path.Combine(mozDir, "moz.txt");
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                System.IO.File.AppendAllText(filePath, tbID.Text + MyFilesDatabase.SPLITTER + tbSecret.Text);

                OKClicked = true;
            }
            catch { }

            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
