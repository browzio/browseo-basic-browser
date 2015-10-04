using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DragDropListview.Windows
{
    /// <summary>
    /// Interaction logic for EditBookmarkWindow.xaml
    /// </summary>
    public partial class EditBookmarkWindow : Window
    {
        public bool SaveClicked { get; set; }
        public int LastSelectedIndex { get; set; }

        public EditBookmarkWindow()
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\browseo (1).ico"));

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SaveClicked = false;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveClicked = true;
            LastSelectedIndex = cmbFolders.SelectedIndex;
            this.Close();
        }

        internal void SetValues(string name, string url, ObservableCollection<FolderVM> folderAndSiteList, int lastSelectedIndex)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = "";
            item.Tag = -1;
            cmbFolders.Items.Add(item);

            tbName.Text = name;
            tbURL.Text = url;
            for (int i = 0; i < folderAndSiteList.Count; i++)
            {
                FolderVM folderNsite = folderAndSiteList[i];
                if (folderNsite.IsFolder)
                {
                    ComboBoxItem folderItem = new ComboBoxItem();
                    folderItem.Content = folderNsite.Name;
                    folderItem.Tag = i;
                    cmbFolders.Items.Add(folderItem);
                }
            }

            cmbFolders.SelectedIndex = 0;

            if (cmbFolders.Items.Count > lastSelectedIndex)
            {
                cmbFolders.SelectedIndex = lastSelectedIndex;
            }
        }

        public bool IsCP { get; set; }

        private void Email_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsCP)
            {
                try
                {
                    (sender as TextBox).IsReadOnly = true;
                    (sender as TextBox).SelectAll();
                    Organiser.Common.Classes.MyFilesDatabase.SetClipboardText((sender as TextBox).Text);
                }
                catch 
                {
                }
            }
        }
    }
}
