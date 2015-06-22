using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Collections;
using Organiser.Common.Classes;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using DragDropListview.Helpers;
using DragDropListview.Windows;
using System.Threading;

namespace DragDropListview
{
   public class DragDropMainViewModel : IDropTarget, INotifyPropertyChanged
    {
        public event Action<string> OnDoubleClickedSite;

        private ICommand lVFCMenuClick;
        public ICommand LVFolderCMenuClick
        {
            get { return lVFCMenuClick; }
            set { lVFCMenuClick = value; }
        }

        private ICommand lVFSMenuClick;
        public ICommand LVSiteCMenuClick
        {
            get { return lVFSMenuClick; }
            set { lVFSMenuClick = value; }
        }

        private ICommand selectFolderSelect_Click;
        public ICommand SelectFolderSelect_Click
        {
            get { return selectFolderSelect_Click; }
            set { selectFolderSelect_Click = value; }
        }

        public string ProjectName { get; set; }

        private bool visible_Sites;
        public bool Visible_Sites
        {
            get { return visible_Sites; }
            set
            {
                visible_Sites = value;
                if (visible_Sites)
                    LbFoldersWidth = 150;
                else
                    LbFoldersWidth = 300;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible_Sites"));
                }
            }
        }

        private int sIFoldersSide;
        public int SIFoldersSide
        {
            get { return sIFoldersSide; }
            set
            {
                sIFoldersSide = value;
                try
                {
                    Visible_Sites = FoldersAndSitesList[sIFoldersSide].IsFolder;
                }
                catch { Visible_Sites = false; }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SIFoldersSide"));
                }
            }
        }

        private int sISitesSide;
        public int SISitesSide
        {
            get { return sISitesSide; }
            set
            {
                sISitesSide = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SISitesSide"));
                }
            }
        }

        private double lbFoldersWidth;
        public double LbFoldersWidth
        {
            get { return lbFoldersWidth; }
            set
            {
                lbFoldersWidth = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LbFoldersWidth"));
                }
            }
        }

        public static int LastSelectedIndex { get; set; }
        
        private ObservableCollection<FolderVM> folders;
        public ObservableCollection<FolderVM> FoldersAndSitesList
        {
            get { return folders; }
            set { folders = value; }
        }
        
        public DragDropMainViewModel()
        {
            FoldersAndSitesList = new ObservableCollection<FolderVM>();

            LVFolderCMenuClick = new RelayCommand(FolderMenueItemCLick);
            LVSiteCMenuClick = new RelayCommand(SiteMenueItemCLick);
            SelectFolderSelect_Click = new RelayCommand(SelectFolderSelect_BtnClick);

            LbFoldersWidth = 300;
        }

        private void SiteMenueItemCLick(object param)
        {
            string clickType = param as string;
            switch (clickType)
            {
                case "Edit":
                    EditBookmarkWindow ebm = new EditBookmarkWindow();
                    ebm.SetValues(FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name,
                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Link, FoldersAndSitesList,
                        LastSelectedIndex);
                    ebm.ShowDialog();
                    if (ebm.SaveClicked)
                    {
                        ebm.LastSelectedIndex = LastSelectedIndex;

                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name = ebm.tbName.Text;
                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Link = ebm.tbURL.Text;
                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].BitmapImg = new BitmapImage
                            (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                    }
                    break;

                case "AddSite":
                    EditBookmarkWindow ebmff = new EditBookmarkWindow();
                    ebmff.SetValues("", "", FoldersAndSitesList, LastSelectedIndex);
                    ebmff.ShowDialog();
                    if (ebmff.SaveClicked)
                    {
                        LastSelectedIndex = ebmff.LastSelectedIndex;

                        Bookmark bmark = new Bookmark();
                        bmark.Name = ebmff.tbName.Text;
                        bmark.Link = ebmff.tbURL.Text;
                        bmark.BitmapImg = new BitmapImage
                        (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                        FoldersAndSitesList[SIFoldersSide].Sites.Add(bmark);
                    }
                    break;

                case "Delete":
                    if (MessageBox.Show("Are you sure you would like to delete " + FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name + "?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        FoldersAndSitesList[SIFoldersSide].Sites.RemoveAt(SISitesSide);
                    }
                    break;
            }

            saveAll();
        }

        private void FolderMenueItemCLick(object param)
        {
            string clickType = param as string;
            switch (clickType)
            {
                case "Edit":
                     EditBookmarkWindow ebm = new EditBookmarkWindow();
                    ebm.SetValues(FoldersAndSitesList[SIFoldersSide].Name,
                        FoldersAndSitesList[SIFoldersSide].Link, FoldersAndSitesList, LastSelectedIndex);
                    ebm.ShowDialog();
                    if (ebm.SaveClicked)
                    {
                        LastSelectedIndex = ebm.LastSelectedIndex;
                        FoldersAndSitesList[SIFoldersSide].Name = ebm.tbName.Text;
                        FoldersAndSitesList[SIFoldersSide].Link = ebm.tbURL.Text;
                        if (FoldersAndSitesList[SIFoldersSide].IsFolder)
                            FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                            (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\closed_folder.png"));
                        else
                            FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                            (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                    }
                    break;

                case "AddFolder":
                    EditBookmarkWindow ebmf = new EditBookmarkWindow();
                    ebmf.spUrl.Visibility = Visibility.Collapsed;
                    ebmf.spFolder.Visibility = Visibility.Collapsed;
                    ebmf.Height = 140;
                    ebmf.ShowDialog();
                    if (ebmf.SaveClicked)
                    {
                        FolderVM bookmarkFolder = new FolderVM();
                        bookmarkFolder.Name = ebmf.tbName.Text;
                        bookmarkFolder.IsFolder = true;
                        bookmarkFolder.BitmapImg = new BitmapImage
                        (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\closed_folder.png"));
                        bookmarkFolder.Sites = new ObservableCollection<Bookmark>();
                        FoldersAndSitesList.Add(bookmarkFolder);
                    }
                    break;

                case "AddSite":
                    EditBookmarkWindow ebmff = new EditBookmarkWindow();
                    ebmff.SetValues("", "", FoldersAndSitesList, LastSelectedIndex);
                    ebmff.ShowDialog();
                    if (ebmff.SaveClicked)
                    {
                        LastSelectedIndex = ebmff.LastSelectedIndex;
                        FolderVM bookmarkFolder = new FolderVM();
                        bookmarkFolder.Name = ebmff.tbName.Text;
                        bookmarkFolder.Link = ebmff.tbURL.Text;
                        bookmarkFolder.BitmapImg = new BitmapImage
                        (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                        FoldersAndSitesList.Add(bookmarkFolder);
                    }
                    break;

                case "Delete":
                    if (MessageBox.Show("Are you sure you would like to delete " + FoldersAndSitesList[SIFoldersSide].Name + "?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        LastSelectedIndex = 0;
                        FoldersAndSitesList.RemoveAt(SIFoldersSide);
                    }
                    break;
            }

            saveAll();
        }

        internal void SaveSite(string url, string name, object indexTag)
        {
            int tagIndex = Convert.ToInt32(indexTag);
            if (tagIndex == -1)
            {
                MyFilesDatabase.SaveSiteBookmark(url, name, ProjectName);
                FillList(false);
            }
            else
            {
                Bookmark bmark = new Bookmark();
                bmark.Link = url;
                bmark.Name = name;
                bmark.BitmapImg = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                FoldersAndSitesList[tagIndex].Sites.Add(bmark);
                saveAll();
            }
        }

        void IDropTarget.DragOver(DropInfo dropInfo)
        {
           // if (dropInfo.Data is Bookmark && dropInfo.TargetItem is FolderVM)
            //{
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            //}
        }

        void IDropTarget.Drop(DropInfo dropInfo)
        {
            try
            {
                FolderVM folder = (FolderVM)dropInfo.TargetItem;
                if (dropInfo.Data is Bookmark)
                {
                    Bookmark site = (Bookmark)dropInfo.Data;
                    if (folder == null || !folder.IsFolder)
                        FoldersAndSitesList.Add(new FolderVM()
                        {
                            Link = site.Link,
                            Name = site.Name,
                            BitmapImg = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png")),
                            IsFolder = false
                        });
                    else
                        folder.Sites.Add(site);
                    ((IList)dropInfo.DragInfo.SourceCollection).Remove(site);
                    saveAll();
                    return;
                }
                else if (dropInfo.Data is FolderVM)
                {
                    FolderVM site = (FolderVM)dropInfo.Data;
                    if (site.IsFolder || folder == site || !folder.IsFolder) return;

                    Bookmark bmark = new Bookmark();
                    bmark.Link = site.Link;
                    bmark.Name = site.Name;
                    bmark.BitmapImg = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                    bool ExistsinList = false;
                    if (folder.Sites.Count == 1)
                    {
                        if (folder.Sites[0].Link == bmark.Link && folder.Sites[0].Name == bmark.Name)
                            ExistsinList = true;
                    }
                    if (!ExistsinList)
                    {
                        folder.Sites.Add(bmark);
                        ((IList)dropInfo.DragInfo.SourceCollection).Remove(site);
                    }
                    saveAll();
                    return;
                }
            }
            catch { }
        }

        public void FillList(bool setIndex0 = true)
        {
            FoldersAndSitesList.Clear();

            foreach (string folder in MyFilesDatabase.GetBookmarkedFolders(ProjectName))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder);

                FolderVM bookmarkFolder = new FolderVM();
                bookmarkFolder.Name = dirInfo.Name;
                bookmarkFolder.IsFolder = true;
                bookmarkFolder.BitmapImg = new BitmapImage
                        (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\closed_folder.png"));
                bookmarkFolder.Sites = new ObservableCollection<Bookmark>();
                foreach (string siteLine in MyFilesDatabase.GetBookmarkedSitesByPath(dirInfo.FullName, ProjectName))
                {
                    try
                    {
                        string[] siteNname = siteLine.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                        bookmarkFolder.Sites.Add(new Bookmark()
                        {
                            Link = siteNname[0],
                            Name = siteNname[1],
                            BitmapImg = new BitmapImage
                       (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"))
                        });
                    }
                    catch { }
                }

                FoldersAndSitesList.Add(bookmarkFolder);
            }

            foreach (string siteLine in MyFilesDatabase.GetBookmarkedSitesByProjName(ProjectName))
            {
                try
                {
                    FolderVM bookmarkFolder = new FolderVM();
                    bookmarkFolder.Sites = new ObservableCollection<Bookmark>();
                    string[] siteNname = siteLine.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                    bookmarkFolder.Link = siteNname[0];
                    bookmarkFolder.Name = siteNname[1];
                    bookmarkFolder.BitmapImg = new BitmapImage
                       (new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "\\Images\\new_document.png"));
                    FoldersAndSitesList.Add(bookmarkFolder);
                }
                catch { }
            }

            if (setIndex0)
                SIFoldersSide = 0;
        }

        object mlock = new object();
        private void saveAll()
        {
            //new Thread(() => {
               // lock (mlock)
               // {
            try
            {
                MyFilesDatabase.DeleteBookmarks(ProjectName);
                foreach (FolderVM folderListItem in FoldersAndSitesList)
                {
                    if (folderListItem.IsFolder)
                    {
                        if (folderListItem.Sites.Count > 0)
                        {
                            foreach (Bookmark bmark in folderListItem.Sites)
                            {
                                MyFilesDatabase.AppendBookmarkByFolderAnProjName(ProjectName, folderListItem.Name, bmark.Link, bmark.Name);
                            }
                        }
                        else
                        {
                            MyFilesDatabase.AppendBookmarkByFolderAnProjNameNoSites(ProjectName, folderListItem.Name);
                        }
                    }
                    else
                    {
                        MyFilesDatabase.SaveSiteBookmark(folderListItem.Link, folderListItem.Name, ProjectName);
                    }
                }
            }
            catch { }
               // }
            //}).Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void MigrateOldSites()
        {
            MyFilesDatabase.MigrateOldBookmarks(ProjectName);
        }

        internal void DoubleClickedFolderSide()
        {
            try
            {
                if (FoldersAndSitesList[SIFoldersSide].IsFolder) return;
                OnDoubleClickedSite(FoldersAndSitesList[SIFoldersSide].Link);
            }
            catch { }
        }

        internal void DoubleClickedSitesSide()
        {
            try
            {
                OnDoubleClickedSite(FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Link);
            }
            catch { }
        }

        private void SelectFolderSelect_BtnClick(object param)
        {
            switch ((string)param)
            {
                case "All":
                    foreach (FolderVM folder in FoldersAndSitesList)
                    {
                        folder.IsChecked = true;
                    }
                    break;

                case "None":
                    foreach (FolderVM folder in FoldersAndSitesList)
                    {
                        folder.IsChecked = false;
                    }
                    break;
                default:
                    break;
            }
        }

        internal void EportSitesToTxt()
        {
            try
            {
                ChooseFolderWindow cfw = new ChooseFolderWindow();
                cfw.DataContext = this;
                cfw.ShowDialog();
                if (!cfw.OkClicked) return;

                // Configure save file dialog box
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".txt"; // Default file extension
                dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension 

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                string filename = "";
                if (result == false) return;

                // Save document 
                filename = dlg.FileName;

                if (filename == "") return;
                new Thread(() => {
                    foreach (FolderVM folderListItem in FoldersAndSitesList)
                    {
                        if (folderListItem.IsFolder && folderListItem.IsChecked)
                        {
                            foreach (Bookmark bmark in folderListItem.Sites)
                            {
                                File.AppendAllText(filename, bmark.Link + Environment.NewLine);
                            }
                        }
                        else if (folderListItem.IsChecked)
                        {
                            File.AppendAllText(filename, folderListItem.Link + Environment.NewLine);
                        }
                    }

                    System.Diagnostics.Process.Start(filename);
                }).Start();
            }
            catch { }
        }
    }
}
