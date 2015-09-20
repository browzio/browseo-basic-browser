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
    /// Interaction logic for SetProxyWindow.xaml
    /// </summary>
    public partial class SetProxyWindow : Window
    {
        public bool OKClicked { get; set; }

        public SetProxyWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mozDir = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Prospector", "Proxy");
                if (!System.IO.Directory.Exists(mozDir)) System.IO.Directory.CreateDirectory(mozDir);

                string filePath = System.IO.Path.Combine(mozDir, "proxy.txt");
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                System.IO.File.AppendAllText(filePath, txtIP.Text + MyFilesDatabase.SPLITTER + txtPORT.Text + MyFilesDatabase.SPLITTER + txtUser.Text + MyFilesDatabase.SPLITTER + txtPass.Text);

                OKClicked = true;
            }
            catch { }
            OKClicked = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
