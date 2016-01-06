using Organiser.Common.Classes;
using ProjectsList.Models;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Organiser.Common
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SelectProfileWindow : Window
    {
        public bool OkClicked;
        public string SelectedProfileFilePath;
        public string SelectedProjectName;

        private  ObservableCollection<string> profilesList;
        public  ObservableCollection<string> ProfilesList
        {
            get { return profilesList; }
            set { profilesList = value; }
        }

        List<KeyValuePair<string, string>> directoryValues;

        string projectName;
        string projectDir;
        string webAddress;
        int lastProfileIndex;
        private bool isSelectProjWindow;

        public SelectProfileWindow(string projName, string projDir, int lastIndex, string webAddress)
        {
            InitializeComponent();
            projectName = projName;
            projectDir = projDir;
            lastProfileIndex = lastIndex;
            this.webAddress = webAddress; 
            ProfilesList = new ObservableCollection<string>();
            DataContext = this;
        }

        public SelectProfileWindow()
        {
            InitializeComponent();
            ProfilesList = new ObservableCollection<string>();
            DataContext = this;
            isSelectProjWindow = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OkClicked = true;

            foreach (var item in directoryValues)
            {
                if (item.Key == cmProfiles.SelectedItem.ToString())
                {
                    SelectedProfileFilePath = item.Value;
                    SelectedProjectName = item.Key;
                    break;
                }
            }


            System.Threading.Thread.Sleep(10);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {  
            if (!isSelectProjWindow)
            {
                directoryValues = MyFilesDatabase.GetSubProjectsFolders(projectDir, projectName);
            }
            else
            {
                directoryValues = MyFilesDatabase.GetAllProjectsAndDirs();
            }

            foreach (var item in directoryValues)
            {  
                string pname = item.Key;
                if (pname.Contains("_folder_")) continue;

                if (pname.Contains("_tier_"))
                    pname = pname.Replace("_tier_", "");
                
                ProfilesList.Add(pname); 
            }

            if (lastProfileIndex > 0 && lastProfileIndex < cmProfiles.Items.Count)
                cmProfiles.SelectedIndex = lastProfileIndex;
            else
                cmProfiles.SelectedIndex = 0;
        } 
    }
}
