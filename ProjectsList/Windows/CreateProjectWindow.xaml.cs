using ProjectsList.Helpers;
using SocialOrganizer.Models;
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

namespace ProjectsList.Windows
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public bool isSave;

        private bool isMulty;
        private string oldPath;

        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        public void init(string txt, string txttitle,bool isMultyProfile, bool Projectprofile)
        {
            tbbutton.Text = txt;
            isMulty = isMultyProfile;
            if (Projectprofile) isMulty = false;
            this.Title = txttitle;
            oldPath = System.IO.Path.Combine(FilesDatabase.GetBaseDir(), "Projects", (DataContext as PersonData).ProjectName, (DataContext as PersonData).ProfileName == null ? "" : (DataContext as PersonData).ProfileName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isMulty)
            {
                if (System.IO.Directory.Exists(oldPath))
                    System.IO.Directory.Delete(oldPath, true);

                FilesDatabase.CreatSubProjectUser(DataContext as PersonData);
            }
            isSave = true;
            this.Close();
        }
    }
}
