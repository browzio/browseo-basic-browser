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
using DragDropListview.Windows;
using System.Threading;
using Organiser.Common.Windows;
using DragDropListview.Models;
using System.Windows.Media;
using System.Threading.Tasks;
using Organiser.Common;
using Microsoft.Win32;
using Organiser.Common.ViewModels;

namespace DragDropListview
{
    public enum FolderTypes
    {
        Regular,
        Import,
        Session
    }

    public class DragDropMainViewModel : IDropTarget, INotifyPropertyChanged
    {
        public const string IMPORT_TYPE_FCS = "fcs";
        public const string IMPORT_TYPE_EB = "eb";
        public const string IMPORT_TYPE_RANKWYZ = "rankWYZ";
        public const string IMPORT_TYPE_GLOBAL = "global";

        public const string GLOABLEFOLDERNAME = "GloableBookMarks_G_";

        public event Action<int> OnHasReminders = delegate { };//int, amount of reminders     
        public event Action<string> OnDoubleClickedSite = delegate { };//string, site to open
        public event Action<string[]> OnSelsectedLauncAll = delegate { };//string, site to open          


        public List<Reminder> Reminders { get; set; }
        public ObservableCollection<Reminder> RemindersByDate { get; set; }
        public ObservableCollection<Reminder> ReminderDates { get; set; }

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

        //Thread saveThread;
        object mlock = new object();
        private Thread CPThread;

        private ObservableCollection<FolderVM> folders;
        public ObservableCollection<FolderVM> FoldersAndSitesList
        {
            get { return folders; }
            set { folders = value; }
        }

        private static DragDropMainViewModel instance;
        public static DragDropMainViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DragDropMainViewModel();    
                           
                    Task.Factory.StartNew(() =>
                    {
                        instance.FillList();
                        instance.FillImportsList();
                        instance.FillSessionListFromFile();
                        instance.CheckReminders();
                    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                }
                return instance;
            }
        }

        private DragDropMainViewModel()
        {
            FoldersAndSitesList = new ObservableCollection<FolderVM>();
            Reminders = new List<Reminder>();
            RemindersByDate = new ObservableCollection<Reminder>();
            ReminderDates = new ObservableCollection<Reminder>();

            LVFolderCMenuClick = new RelayCommand(FolderMenueItemCLick);
            LVSiteCMenuClick = new RelayCommand(SiteMenueItemCLick);
            SelectFolderSelect_Click = new RelayCommand(SelectFolderSelect_BtnClick);
            LBRemindersByDateCMClick = new RelayCommand(LBRemindersByDateCMClick_click);

            LbFoldersWidth = 300;
        }

        private void SiteMenueItemCLick(object param)
        {
            string clickType = param as string;
            bool wascp = false;

            FolderTypes folderType = FolderTypes.Regular;
            if (FoldersAndSitesList.Count > 0 && SIFoldersSide >= 0)
            {
                folderType = FoldersAndSitesList[SIFoldersSide].TypeOfFolder;
            }

            switch (clickType)
            {
                case "Edit":
                    EditBookmarkWindow ebm = new EditBookmarkWindow();
                    ebm.SetValues(FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name,
                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Link, FoldersAndSitesList,
                        LastSelectedIndex);
                    if (FoldersAndSitesList[SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                    {
                        ebm.spFolder.Visibility = Visibility.Collapsed;
                        ebm.spImports.Visibility = Visibility.Visible;

                        ebm.Email.Text = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Email;
                        ebm.Username.Text = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Username;
                        ebm.Password.Text = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Password;
                    }
                    ebm.ShowDialog();
                    if (ebm.SaveClicked)
                    {
                        ebm.LastSelectedIndex = LastSelectedIndex;

                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name = ebm.tbName.Text;
                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Link = ebm.tbURL.Text;
                        if (FoldersAndSitesList[SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                        {
                            FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Email = ebm.Email.Text;
                            FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Username = ebm.Username.Text;
                            FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Password = ebm.Password.Text;
                        }
                        FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].BitmapImg = new BitmapImage
                            (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                    }
                    break;

                case "CPWindow":
                    wascp = true;

                    CPThread = new Thread(() =>
                    {
                        EditBookmarkWindow ebmffff = new EditBookmarkWindow();
                        ebmffff.IsCP = true;
                        ebmffff.SetValues(FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name,
                            FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Link, FoldersAndSitesList,
                            LastSelectedIndex);
                        ebmffff.spButtons.Visibility = Visibility.Collapsed;
                        if (FoldersAndSitesList[SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                        {
                            ebmffff.spFolder.Visibility = Visibility.Collapsed;
                            ebmffff.spImports.Visibility = Visibility.Visible;

                            ebmffff.Email.Text = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Email;
                            ebmffff.Username.Text = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Username;
                            ebmffff.Password.Text = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Password;
                        }
                        ebmffff.Topmost = true;
                        ebmffff.ResizeMode = ResizeMode.CanResize;
                        ebmffff.ShowDialog();
                    });

                    CPThread.SetApartmentState(ApartmentState.STA);
                    CPThread.Start();
                    break;

                case "AddSite":
                    EditBookmarkWindow ebmff = new EditBookmarkWindow();
                    ebmff.SetValues("", "", FoldersAndSitesList, LastSelectedIndex);
                    if (FoldersAndSitesList[SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                    {
                        ebmff.spFolder.Visibility = Visibility.Collapsed;
                        ebmff.spImports.Visibility = Visibility.Visible;
                    }
                    ebmff.ShowDialog();
                    if (ebmff.SaveClicked)
                    {
                        LastSelectedIndex = ebmff.LastSelectedIndex;

                        Bookmark bmark = new Bookmark();
                        bmark.Name = ebmff.tbName.Text;
                        bmark.Link = ebmff.tbURL.Text;
                        bmark.ImportType = FoldersAndSitesList[SIFoldersSide].ImportType;

                        if (FoldersAndSitesList[SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                        {
                            bmark.Email = ebmff.Email.Text;
                            bmark.Username = ebmff.Username.Text;
                            bmark.Password = ebmff.Password.Text;
                            bmark.IsImported = true;
                        }

                        bmark.DateTimeStamp = DateTime.Now.ToString();
                        bmark.BitmapImg = new BitmapImage
                        (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                        FoldersAndSitesList[SIFoldersSide].Sites.Add(bmark);
                    }
                    break;

                case "Delete":
                    if (MessageBox.Show("Are you sure you would like to delete " + FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name + "?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        FoldersAndSitesList[SIFoldersSide].Sites.RemoveAt(SISitesSide);
                    }
                    break;

                case "Reminder":
                    SetReminderWindow srw = new SetReminderWindow();
                    srw.ShowDialog();
                    if (srw.OkClicked)
                    {
                        saveReminder(srw.tbInputedText.Text, srw.dtReminder.Text, false);
                    }
                    break;
            }

            if (!wascp)
            {
                switch (folderType)
                {
                    case FolderTypes.Regular:
                        saveAll();
                        break;
                    case FolderTypes.Import:
                        saveAllImportedSites();
                        break;
                    case FolderTypes.Session:
                        SaveSessionBookmarksToFile();
                        break;
                    default:
                        break;
                }

            }
        }

        public void ImportFromMulyLinks()
        {
            Thread uiImportThread = new Thread(() =>
            {
                EditBookmarkWindow ebm = new EditBookmarkWindow();
                ebm.spUrl.Visibility = Visibility.Collapsed;
                ebm.SetValues("", "", FoldersAndSitesList, 0);
                ebm.ShowDialog();

                if (ebm.SaveClicked)
                {
                    int tagIndex = Convert.ToInt32((ebm.cmbFolders.SelectedItem as ComboBoxItem).Tag);
                    string folderNewName = ebm.tbName.Text;
                    bool noName = false;
                    if (tagIndex == -1 && (string.IsNullOrEmpty(folderNewName) || string.IsNullOrWhiteSpace(folderNewName)))
                    {
                        if (MessageBox.Show("Select a name for a new folder or a folder from the dropdown list before you improt these links. if you continue it will import them as base links without a folder. Continue? ",
                            "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            noName = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    RssFeedsLinksMultiWindow muliwindow = new RssFeedsLinksMultiWindow();
                    muliwindow.ShowDialog();

                    if (muliwindow.OKClicked)
                    {

                        string[] links = muliwindow.tbInputedText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);


                        foreach (string link in links)
                        {
                            if (noName)
                            {
                                SaveSite(link, link, -1, DateTime.Now.ToString(), false);
                                continue;
                            }
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                if (string.IsNullOrEmpty(folderNewName) || string.IsNullOrWhiteSpace(folderNewName))
                                {
                                    SaveSite(link, link, tagIndex, DateTime.Now.ToString(), false);
                                }
                                else
                                {
                                    FolderVM bookmarkFolder = new FolderVM();
                                    bookmarkFolder.Name = folderNewName;
                                    bookmarkFolder.IsFolder = true;
                                    bookmarkFolder.TypeOfFolder = FolderTypes.Regular;
                                    bookmarkFolder.DateTimeStamp = DateTime.Now.ToString();
                                    bookmarkFolder.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/closed_folder.png"));
                                    bookmarkFolder.Sites = new ObservableCollection<Bookmark>();

                                    FoldersAndSitesList.Add(bookmarkFolder);
                                    tagIndex = FoldersAndSitesList.Count - 1;
                                    folderNewName = "";
                                    SaveSite(link, link, tagIndex, DateTime.Now.ToString(), false);
                                }
                            });
                        }

                        if (noName)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                FillList(false);
                            });
                        }
                        saveAll();

                    }
                }
            });

            uiImportThread.SetApartmentState(ApartmentState.STA);
            uiImportThread.Start();
        }

        private void FolderMenueItemCLick(object param)
        {
            string clickType = param as string;

            FolderTypes folderType = FolderTypes.Regular;
            if (FoldersAndSitesList.Count > 0 && SIFoldersSide >= 0)
            {
                try
                {
                    folderType = FoldersAndSitesList[SIFoldersSide].TypeOfFolder;
                }
                catch
                {
                    folderType = FolderTypes.Regular;
                }
            }

            switch (clickType)
            {
                case "Edit":
                    if (FoldersAndSitesList.Count == 0 || SIFoldersSide < 0 || SIFoldersSide > FoldersAndSitesList.Count - 1) return;
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
                        {
                            if (FoldersAndSitesList[SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                            {
                                if (FoldersAndSitesList[SIFoldersSide].ImportType == IMPORT_TYPE_GLOBAL)
                                    FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                                (new Uri("pack://application:,,,/Organiser.Common;component/Image/icon-global.png"));
                                else
                                    FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                                    (new Uri("pack://application:,,,/Organiser.Common;component/Image/closed_folder.png"));
                            }
                            else
                            {
                                if (FoldersAndSitesList[SIFoldersSide].ImportType == IMPORT_TYPE_FCS)
                                {
                                    FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                                (new Uri("pack://application:,,,/Organiser.Common;component/Image/fcs icon.png"));
                                }

                                if (FoldersAndSitesList[SIFoldersSide].ImportType == IMPORT_TYPE_EB)
                                {
                                    FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                                (new Uri("pack://application:,,,/Organiser.Common;component/Image/enterprise buddy.ico"));
                                }
                            }
                        }
                        else
                            FoldersAndSitesList[SIFoldersSide].BitmapImg = new BitmapImage
                            (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                    }
                    break;

                case "AddFolder":
                    SelectBookmarkImportTypeWindow bookmarkTypeWindow = new SelectBookmarkImportTypeWindow();
                    bookmarkTypeWindow.ShowDialog();
                    if (!bookmarkTypeWindow.OkClicked) return;

                    EditBookmarkWindow ebmf = new EditBookmarkWindow();
                    ebmf.spUrl.Visibility = Visibility.Collapsed;
                    ebmf.spFolder.Visibility = Visibility.Collapsed;
                    ebmf.Height = 140;
                    ebmf.ShowDialog();
                    if (!ebmf.SaveClicked) return;

                    FolderVM bookmarkFolder_S = new FolderVM();
                    bookmarkFolder_S.Name = ebmf.tbName.Text;
                    bookmarkFolder_S.IsFolder = true;
                    bookmarkFolder_S.TypeOfFolder = FolderTypes.Regular;
                    bookmarkFolder_S.DateTimeStamp = DateTime.Now.ToString();
                    bookmarkFolder_S.Sites = new ObservableCollection<Bookmark>();

                    if (bookmarkTypeWindow.browseoProj.IsChecked == true)
                    {
                        bookmarkFolder_S.BitmapImg = new BitmapImage
                        (new Uri("pack://application:,,,/Organiser.Common;component/Image/closed_folder.png"));
                    }
                    else if (bookmarkTypeWindow.fcs.IsChecked == true)
                    {
                        bookmarkFolder_S.TypeOfFolder = FolderTypes.Import;
                        bookmarkFolder_S.ImportType = IMPORT_TYPE_FCS;
                        bookmarkFolder_S.BitmapImg = new BitmapImage
                            (new Uri("pack://application:,,,/Organiser.Common;component/Image/fcs icon.png"));
                    }
                    else if (bookmarkTypeWindow.entBud.IsChecked == true)
                    {
                        bookmarkFolder_S.TypeOfFolder = FolderTypes.Import;
                        bookmarkFolder_S.ImportType = IMPORT_TYPE_EB;
                        bookmarkFolder_S.BitmapImg = new BitmapImage
                            (new Uri("pack://application:,,,/Organiser.Common;component/Image/enterprise buddy.ico"));
                    }
                    else if (bookmarkTypeWindow.rankWyx.IsChecked == true)
                    {
                        bookmarkFolder_S.TypeOfFolder = FolderTypes.Import;
                        bookmarkFolder_S.ImportType = IMPORT_TYPE_RANKWYZ;
                        bookmarkFolder_S.BitmapImg = new BitmapImage
                            (new Uri("pack://application:,,,/Organiser.Common;component/Image/rankwyz-icon-check.png"));
                    }
                    else if (bookmarkTypeWindow.browseoGloable.IsChecked == true)
                    {
                        //IMPORT_TYPE_GLOBAL
                        bookmarkFolder_S.ImportType = IMPORT_TYPE_GLOBAL;
                        bookmarkFolder_S.BitmapImg = new BitmapImage
                            (new Uri("pack://application:,,,/Organiser.Common;component/Image/icon-global.png"));
                    }

                    folderType = bookmarkFolder_S.TypeOfFolder;
                    FoldersAndSitesList.Add(bookmarkFolder_S);
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
                        bookmarkFolder.DateTimeStamp = DateTime.Now.ToString();
                        bookmarkFolder.BitmapImg = new BitmapImage
                        (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                        FoldersAndSitesList.Add(bookmarkFolder);
                    }
                    break;

                case "Delete":
                    if (FoldersAndSitesList.Count == 0 || SIFoldersSide < 0 || SIFoldersSide > FoldersAndSitesList.Count - 1) return;
                    if (MessageBox.Show("Are you sure you would like to delete " + FoldersAndSitesList[SIFoldersSide].Name + "?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        LastSelectedIndex = 0;
                        deleteImportedFolder();
                        FoldersAndSitesList.RemoveAt(SIFoldersSide);
                    }
                    break;

                case "LaunchToBrowser":
                    if (FoldersAndSitesList.Count == 0 || SIFoldersSide < 0 || SIFoldersSide > FoldersAndSitesList.Count - 1) return;
                    OnSelsectedLauncAll(FoldersAndSitesList[SIFoldersSide].Sites.Select(s => s.Link).ToArray());
                    break;

                case "Reminder":
                    if (FoldersAndSitesList.Count == 0 || SIFoldersSide < 0 || SIFoldersSide > FoldersAndSitesList.Count - 1) return;
                    SetReminderWindow srw = new SetReminderWindow();
                    srw.ShowDialog();
                    if (srw.OkClicked)
                    {
                        saveReminder(srw.tbInputedText.Text, srw.dtReminder.Text, true);
                    }
                    break;

                case "Refresh":
                    RefreshList();
                    return;
            }

            switch (folderType)
            {
                case FolderTypes.Regular:
                    saveAll();
                    break;
                case FolderTypes.Import:
                    saveAllImportedSites();
                    break;
                case FolderTypes.Session:
                    SaveSessionBookmarksToFile();
                    break;
                default:
                    break;
            }
        }

        public void OpenSaveSiteOptions(string site)
        {
            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.SetValues(site, site, FoldersAndSitesList, LastSelectedIndex);
            ebm.ShowDialog();

            if (ebm.SaveClicked)
            {
                LastSelectedIndex = ebm.LastSelectedIndex;
                SaveSite(ebm.tbURL.Text, ebm.tbName.Text, (ebm.cmbFolders.SelectedItem as ComboBoxItem).Tag, DateTime.Now.ToString());
            }
        }

        internal void SaveSite(string url, string name, object indexTag, string saveTimeStamp, bool saveAllThem = true)
        {
            int tagIndex = Convert.ToInt32(indexTag);
            FolderTypes folderType = FolderTypes.Regular;
            if (tagIndex == -1)
            {
                MyFilesDatabase.SaveSiteBookmark(url, name, GloableProfData.PData.ProjectName, saveTimeStamp);
                if(saveAllThem)
                    FillList(false);
            }
            else
            {
                Bookmark bmark = new Bookmark();
                bmark.Link = url;
                bmark.Name = name;
                bmark.DateTimeStamp = saveTimeStamp;
                bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                FoldersAndSitesList[tagIndex].Sites.Add(bmark);
                folderType = FoldersAndSitesList[tagIndex].TypeOfFolder;
            }
            if (saveAllThem)
            {
                switch (folderType)
                {
                    case FolderTypes.Regular:
                        saveAll();
                        break;
                    case FolderTypes.Import:
                        saveAllImportedSites();
                        break;
                    case FolderTypes.Session:
                        SaveSessionBookmarksToFile();
                        break;
                    default:
                        break;
                }        
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
                if (folder.TypeOfFolder == FolderTypes.Import)
                {
                    if (dropInfo.Data is Bookmark)
                    {
                        Bookmark site = (Bookmark)dropInfo.Data;
                        if (folder != null && folder.IsFolder && folder.ImportType == site.ImportType)
                        {
                            folder.Sites.Add(site);
                            ((IList)dropInfo.DragInfo.SourceCollection).Remove(site);
                            saveAllImportedSites();
                        }
                        return;
                    }
                }
                else if(folder.TypeOfFolder == FolderTypes.Regular)
                {
                    if (dropInfo.Data is Bookmark)
                    {
                        Bookmark site = (Bookmark)dropInfo.Data;
                        if (site.IsImported) return;

                        if (folder == null || !folder.IsFolder)
                            FoldersAndSitesList.Add(new FolderVM()
                            {
                                Link = site.Link,
                                Name = site.Name,
                                DateTimeStamp = site.DateTimeStamp,
                                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png")),
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
                        if (site.IsFolder || folder == site || !folder.IsFolder || site.TypeOfFolder == FolderTypes.Import) return;

                        Bookmark bmark = new Bookmark();
                        bmark.Link = site.Link;
                        bmark.Name = site.Name;
                        bmark.DateTimeStamp = site.DateTimeStamp;
                        bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
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
                else if (folder.TypeOfFolder == FolderTypes.Session)
                {
                    if (dropInfo.Data is Bookmark)
                    {
                        Bookmark site = (Bookmark)dropInfo.Data;
                        if (folder != null && folder.IsFolder && site.ImportType == folder.ImportType)
                        {
                            folder.Sites.Add(site);
                            ((IList)dropInfo.DragInfo.SourceCollection).Remove(site);
                            SaveSessionBookmarksToFile();
                        }
                        return;
                    }
                }
            }
            catch { }
        }

        public void FillList(bool setIndex0 = true, string projName = "")
        {
            string fromProj = GloableProfData.PData.ProjectName;
            if (projName == "")
            {
                FoldersAndSitesList.RemoveAllThese(item => item.TypeOfFolder != FolderTypes.Import);
            }
            else
            {
                fromProj = projName;
            }

            ReFillList(fromProj);

            if (setIndex0)
                SIFoldersSide = 0;
            if (FoldersAndSitesList.Count == 1)
            {
                SIFoldersSide = 0;
               // Visible_Sites = true;
            }
        }

        private void saveAll()
        {
            //  saveThread = new Thread(() =>
            //   {
            //      lock (mlock)
            //     {
            try
            {
                MyFilesDatabase.DeleteBookmarks(GloableProfData.PData.ProjectName);
                MyFilesDatabase.DeleteBookmarks(GLOABLEFOLDERNAME);
                foreach (FolderVM folderListItem in FoldersAndSitesList)
                {
                    if (folderListItem.TypeOfFolder == FolderTypes.Import || folderListItem.TypeOfFolder == FolderTypes.Session) continue;

                    if (folderListItem.IsFolder && folderListItem.ImportType != IMPORT_TYPE_GLOBAL)
                    {
                        if (folderListItem.Sites.Count > 0)
                        {
                            foreach (Bookmark bmark in folderListItem.Sites)
                            {
                                MyFilesDatabase.AppendBookmarkByFolderAnProjName(GloableProfData.PData.ProjectName, folderListItem.Name, bmark.Link, bmark.Name, bmark.DateTimeStamp);
                            }
                        }
                        else
                        {
                            MyFilesDatabase.AppendBookmarkByFolderAnProjNameNoSites(GloableProfData.PData.ProjectName, folderListItem.Name);
                        }
                    }
                    else if (folderListItem.IsFolder && folderListItem.ImportType == IMPORT_TYPE_GLOBAL)
                    {
                        if (folderListItem.Sites.Count > 0)
                        {
                            foreach (Bookmark bmark in folderListItem.Sites)
                            {
                                MyFilesDatabase.AppendBookmarkByFolderAnProjName(GLOABLEFOLDERNAME, folderListItem.Name, bmark.Link, bmark.Name, bmark.DateTimeStamp);
                            }
                        }
                        else
                        {
                            MyFilesDatabase.AppendBookmarkByFolderAnProjNameNoSites(GLOABLEFOLDERNAME, folderListItem.Name);
                        }
                    }
                    else if (!folderListItem.IsFolder && folderListItem.ImportType != IMPORT_TYPE_GLOBAL)
                    {
                        MyFilesDatabase.SaveSiteBookmark(folderListItem.Link, folderListItem.Name, GloableProfData.PData.ProjectName, folderListItem.DateTimeStamp);
                    }
                }

                // OnListChanged();
            }
            catch { }
            //  }
            //});
            // saveThread.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void EportSitesToTxt()
        {
            try
            {
                ChooseFolderWindow cfw = new ChooseFolderWindow();
                cfw.DataContext = this;
                cfw.lstItems.ItemsSource = FoldersAndSitesList;
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
               // new Thread(() => {
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
              //  }).Start();
            }
            catch { }
        }

        public void MergeBookMarksFromProjectPath(string projName)
        {
            DragDropMainViewModel ddmvm = new DragDropMainViewModel();
            ddmvm.FillList(true, projName);
            ChooseFolderWindow cfw = new ChooseFolderWindow();
            cfw.DataContext = ddmvm;
            cfw.lstItems.ItemsSource = ddmvm.FoldersAndSitesList;
            cfw.ShowDialog();
            if (cfw.OkClicked)
            {
                foreach (FolderVM folderListItem in ddmvm.FoldersAndSitesList)
                {
                    if (folderListItem.IsChecked)
                    {
                        FoldersAndSitesList.Add(folderListItem);
                    }
                }
                saveAll();
            }

            if (FoldersAndSitesList.Count > 0)
            {
                SIFoldersSide = 0;
                Visible_Sites = true;
            }

        }

        public void RefreshList()
        {
            //if (saveThread != null && saveThread.IsAlive) saveThread.Join();
            Task.Factory.StartNew(() =>
            {
                FoldersAndSitesList.Clear();
                ReFillList(GloableProfData.PData.ProjectName);
                FillImportsList();
                FillSessionListFromFile();
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());   
        }

        private void ReFillList(string ProjectName)
        {
            foreach (KeyValuePair<string,string> folder in MyFilesDatabase.GetBookmarkedFolders(ProjectName))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder.Key);

                FolderVM bookmarkFolder = new FolderVM();
                bookmarkFolder.Name = dirInfo.Name;
                bookmarkFolder.IsFolder = true;
                bookmarkFolder.TypeOfFolder = FolderTypes.Regular;
                bookmarkFolder.DateTimeStamp = folder.Value;
                bookmarkFolder.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/closed_folder.png"));
                bookmarkFolder.Sites = new ObservableCollection<Bookmark>();
                foreach (string siteLine in MyFilesDatabase.GetBookmarkedSitesByPath(dirInfo.FullName, ProjectName))
                {
                    try
                    {
                        string[] siteNname = siteLine.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                       Bookmark bmark = new Bookmark()
                        {
                            Link = siteNname[0],
                            Name = siteNname[1],
                            BitmapImg = new BitmapImage
                       (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"))
                        };
                       if (siteNname.Length == 3)
                           bmark.DateTimeStamp = siteNname[2];
                       bookmarkFolder.Sites.Add(bmark);
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
                    bookmarkFolder.TypeOfFolder = FolderTypes.Regular;
                    bookmarkFolder.Sites = new ObservableCollection<Bookmark>();
                    string[] siteNname = siteLine.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                    bookmarkFolder.Link = siteNname[0];
                    bookmarkFolder.Name = siteNname[1];
                    if (siteNname.Length == 3)
                        bookmarkFolder.DateTimeStamp = siteNname[2];
                    bookmarkFolder.BitmapImg = new BitmapImage
                       (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                    FoldersAndSitesList.Add(bookmarkFolder);
                }
                catch { }
            }

            foreach (KeyValuePair<string, string> folder in MyFilesDatabase.GetBookmarkedFolders(GLOABLEFOLDERNAME))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folder.Key);

                FolderVM bookmarkFolder = new FolderVM();
                bookmarkFolder.Name = dirInfo.Name;
                bookmarkFolder.IsFolder = true;
                bookmarkFolder.ImportType = IMPORT_TYPE_GLOBAL;
                bookmarkFolder.DateTimeStamp = folder.Value;
                bookmarkFolder.TypeOfFolder = FolderTypes.Regular;
                bookmarkFolder.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/icon-global.png"));
                bookmarkFolder.Sites = new ObservableCollection<Bookmark>();
                foreach (string siteLine in MyFilesDatabase.GetBookmarkedSitesByPath(dirInfo.FullName, GLOABLEFOLDERNAME))
                {
                    try
                    {
                        string[] siteNname = siteLine.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                        Bookmark bmark = new Bookmark()
                        {
                            Link = siteNname[0],
                            Name = siteNname[1],
                            BitmapImg = new BitmapImage
                        (new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"))
                        };
                        if (siteNname.Length == 3)
                            bmark.DateTimeStamp = siteNname[2];
                        bookmarkFolder.Sites.Add(bmark);
                    }
                    catch { }
                }

                FoldersAndSitesList.Add(bookmarkFolder);
            } 
        }

        public async void OpenImportBookmarksOptions()
        {
            SelectBookmarkImportTypeWindow bookmarkTypeWindow = new SelectBookmarkImportTypeWindow();
            bookmarkTypeWindow.browseoGloable.Visibility = Visibility.Collapsed;
            bookmarkTypeWindow.ShowDialog();
            if (!bookmarkTypeWindow.OkClicked) return;
            if (bookmarkTypeWindow.browseoProj.IsChecked == true)
            {
                //SelectProfileWindow spw = new SelectProfileWindow();
                //spw.Title = "Select Project";
                //spw.ShowDialog();
                //if (spw.OkClicked)
                //{
                //    MergeBookMarksFromProjectPath(spw.SelectedProjectName);
                //}
                ChooseProjectsVM cpvm = new ChooseProjectsVM();
                await cpvm.InitProjectsWindowList();
                if (cpvm.ShowListWindowDialog())
                {
                    foreach (var sp in cpvm.SavedProjectsListAdded)
                    {
                        if (!sp.IsChecked || sp.IsFolder) continue;

                        MergeBookMarksFromProjectPath(sp.Name);
                    }
                }
            }
            else if (bookmarkTypeWindow.browseoFolder.IsChecked == true)
            {
                ImportFromMulyLinks();
            }
            else if (bookmarkTypeWindow.fcs.IsChecked == true || bookmarkTypeWindow.entBud.IsChecked == true || bookmarkTypeWindow.rankWyx.IsChecked == true)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.ShowDialog();
                string path = ofd.FileName;

                string importType = DragDropMainViewModel.IMPORT_TYPE_FCS;
                if (bookmarkTypeWindow.entBud.IsChecked == true)
                    importType = DragDropMainViewModel.IMPORT_TYPE_EB;
                else if (bookmarkTypeWindow.rankWyx.IsChecked == true)
                    importType = DragDropMainViewModel.IMPORT_TYPE_RANKWYZ;

                MergeFromImport(path, importType);
            }
        }

        #region reminders

        const string SPLITTER = "[|!|]";
        const string REMINDER_SPLITTER = "{[|!|]}";

        private ICommand lBRemindersByDateCMClick;
        public ICommand LBRemindersByDateCMClick
        {
            get { return lBRemindersByDateCMClick; }
            set { lBRemindersByDateCMClick = value; }
        }

        private void saveReminder(string inputedText, string dateTimeForReminder, bool fromfolder)
        {
            DateTime dt;
            if (dateTimeForReminder == null || dateTimeForReminder == "" || !DateTime.TryParse(dateTimeForReminder, out dt))
            {
                MessageBox.Show("Date for reminder was not set.");
                return;
            }

            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Reminders", "BookmarkReminders", GloableProfData.PData.ProjectName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            string nameOfItem = FoldersAndSitesList[SIFoldersSide].Name;
            if (!fromfolder)
                nameOfItem = FoldersAndSitesList[SIFoldersSide].Sites[SISitesSide].Name;
            nameOfItem = nameOfItem.Replace("/", "_");
            nameOfItem = nameOfItem.Replace(":", "-");

            string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Reminders", "BookmarkReminders", GloableProfData.PData.ProjectName, nameOfItem + ".txt");
            try
            {
                File.AppendAllText(filePath, inputedText + SPLITTER + dateTimeForReminder + SPLITTER + "false" + REMINDER_SPLITTER);
            }
            catch 
            {
                MessageBox.Show("Error saveing reminder."+
                    " Rename the bookmark and try again."+
                    @" The name must not include < > : """+@" / \ | ? *");
            }

            CheckReminders();
           // OnRemindersChanged();
        }

        public void CheckReminders()
        {
            Reminders.Clear();
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Reminders", "BookmarkReminders", GloableProfData.PData.ProjectName);
            if (Directory.Exists(dirPath))
            {
                foreach (string filePath in Directory.GetFiles(dirPath))
                {
                    FileInfo fInfo = new FileInfo(filePath);
                    string fileText = File.ReadAllText(filePath);
                    string[] lines = fileText.Split(new string[] { REMINDER_SPLITTER }, StringSplitOptions.None);

                    foreach (FolderVM folderListItem in FoldersAndSitesList)
                    {
                        if (folderListItem.IsFolder)
                        {
                            foreach (Bookmark bmark in folderListItem.Sites)
                            {
                                if (fInfo.Name.Replace("_", "/").Replace("-", ":").Replace(".txt", "") == bmark.Name)
                                {
                                    foreach (string line in lines)
                                    {
                                        string[] reminderLines = line.Split(new string[]{SPLITTER}, StringSplitOptions.None);
                                        if (reminderLines[0] == "") break;
                                        Reminders.Add(new Reminder() 
                                        {
                                            ReminderText = reminderLines[0],
                                            ReminderDate = reminderLines[1],
                                            ResolvedText = reminderLines[2],
                                            ForeColorComplete = reminderLines[2] == "false" ?  Brushes.Orange : Brushes.Green,
                                            ReminderName = bmark.Name
                                        });
                                    }
                                }
                            }
                        }
                        if (fInfo.Name.Replace("_", "/").Replace("-", ":").Replace(".txt", "") == folderListItem.Name)
                        {
                            foreach (string line in lines)
                            {
                                string[] reminderLines = line.Split(new string[] { SPLITTER }, StringSplitOptions.None);
                                if (reminderLines[0] == "") break;
                                Reminders.Add(new Reminder()
                                {
                                    ReminderText = reminderLines[0],
                                    ReminderDate = reminderLines[1],
                                    ResolvedText = reminderLines[2],
                                    ForeColorComplete = reminderLines[2] == "false" ? Brushes.Orange : Brushes.Green,
                                    ReminderName = folderListItem.Name
                                });
                            }
                        }
                    }
                }
            }

            if (Reminders.Count > 0)
            {
                List<Reminder> orderd = Reminders.OrderBy((r2) => r2.ReminderDate).ToList();
                Reminders.Clear();
                foreach (Reminder rem in orderd)
                {
                    Reminders.Add(rem);
                }

                GetRemindersCountAndNotify();
            }
            
        }

        public void GetRemindersCountAndNotify()
        {
            int unresolvedCount = 0;
            foreach (Reminder rem in Reminders)
            {
                if (rem.ResolvedText == "false")
                {
                    unresolvedCount++;
                    DateTime dt;
                    DateTime.TryParse(rem.ReminderDate, out dt);
                    if (dt < DateTime.Today)
                    {
                        rem.ForeColorComplete = Brushes.Red;
                    }
                }
            }
            if (unresolvedCount > 0)
                OnHasReminders(unresolvedCount);
        }

        public void OpenReminders()
        {
            ReminderDates.Clear();
            RemindersByDate.Clear();

            foreach (Reminder reminder in Reminders)
            {
                bool add = true;
                foreach (Reminder reminderDate in ReminderDates)
                {
                    if(reminder.ReminderDate == reminderDate.ReminderDate)
                    {
                        add = false;
                        break;
                    }
                }
                if(add)
                    ReminderDates.Add(reminder);
            }

            ViewRemindersWindow vrw = new ViewRemindersWindow();
            vrw.DataContext = this;

            vrw.lbDates.SelectedIndex = 0;
            SIReminderDate = 0;
            SIRemindersByDate = 0;

            updateRemindersByDate();

            vrw.ShowDialog();

        }

        private void LBRemindersByDateCMClick_click(object param)
        {
            try
            {
                string clickType = param as string;
                switch (clickType)
                {
                    case "Resolve":
                        Reminder remToResolve = null;
                        foreach (Reminder remin in Reminders)
                        {
                            if (remin == RemindersByDate[SIRemindersByDate])
                            {
                                remToResolve = remin;
                                break;
                            }
                        }
                        if (remToResolve != null)
                        {
                            remToResolve.ResolvedText = "true";
                            remToResolve.ForeColorComplete = Brushes.Green;
                        }
                        break;

                    case "Delete":
                        Reminder remToRemove = null;
                        foreach (Reminder remin in Reminders)
                        {
                            if (remin.ForeColorComplete == RemindersByDate[SIRemindersByDate].ForeColorComplete &&
                                remin.ReminderDate == RemindersByDate[SIRemindersByDate].ReminderDate &&
                                remin.ReminderName == RemindersByDate[SIRemindersByDate].ReminderName &&
                                remin.ReminderText == RemindersByDate[SIRemindersByDate].ReminderText &&
                                remin.ResolvedText == RemindersByDate[SIRemindersByDate].ResolvedText)
                            {
                                remToRemove = remin;
                                break;
                            }
                        }
                        if (remToRemove != null)
                        {
                            Reminders.Remove(remToRemove);
                            RemindersByDate.RemoveAt(SIRemindersByDate);
                            if (RemindersByDate.Count == 0)
                            {
                                ReminderDates.RemoveAt(SIReminderDate);
                            }
                            SIRemindersByDate = 0;
                        }

                       // updateRemindersByDate();
                        break;
                }

                int unresolvedCount = 0;
                foreach (Reminder rem in Reminders)
                {
                    if (rem.ResolvedText == "false")
                    {
                        unresolvedCount++;
                    }
                }
                OnHasReminders(unresolvedCount);

                SaveAllByReminders();
            }
            catch 
            {
                MessageBox.Show("Action did not complete correctly please make sure"+
                " the reminder is selected before choosing an action on it.");
                return;
            }
        }

        private void SaveAllByReminders()
        {
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Reminders", "BookmarkReminders", GloableProfData.PData.ProjectName);
            if (Directory.Exists(dirPath))
                Directory.Delete(dirPath, true);

            Directory.CreateDirectory(dirPath);

            foreach (Reminder rem in Reminders)
            {
                string nameOfItem = rem.ReminderName;
                nameOfItem = nameOfItem.Replace("/", "_");
                nameOfItem = nameOfItem.Replace(":", "-");

                string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Reminders", "BookmarkReminders", GloableProfData.PData.ProjectName, nameOfItem + ".txt");
                File.AppendAllText(filePath, rem.ReminderText + SPLITTER + rem.ReminderDate + SPLITTER + rem.ResolvedText + REMINDER_SPLITTER);
            }

            CheckReminders();
        }

        private int sIReminderDate;
        public int SIReminderDate
        {
            get { return sIReminderDate; }
            set
            {
                    sIReminderDate = value;
                    updateRemindersByDate();
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SIReminderDate"));
                    }
            }
        }

        private void updateRemindersByDate()
        {
            RemindersByDate.Clear();
            foreach (Reminder reminder in Reminders)
            {
                if (SIReminderDate == -1) break;
                if (reminder.ReminderDate == ReminderDates[SIReminderDate].ReminderDate)
                    RemindersByDate.Add(reminder);
            }
        }

        private int sIRemindersByDate;
       

        public int SIRemindersByDate
        {
            get { return sIRemindersByDate; }
            set
            {
                sIRemindersByDate = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SIRemindersByDate"));
                }
            }
        }

        #endregion

        #region imports

        public void MergeFromImport(string path, string type)
        {
            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.spUrl.Visibility = ebm.spFolder.Visibility = ebm.spName.Visibility = Visibility.Collapsed;
            ebm.spCmbName.Visibility = Visibility.Visible;
            string dirForCustomImport = Path.Combine(MyFilesDatabase.GetBaseDir(), "CustomImports", GloableProfData.PData.ProjectName, type);
            if (Directory.Exists(dirForCustomImport))
            {
                DirectoryInfo dInfo = new DirectoryInfo(dirForCustomImport);
                foreach (DirectoryInfo item in dInfo.GetDirectories())
                {
                    ebm.cmbImportedFolders.Items.Add(item.Name);
                }
            }
            ebm.cmbImportedFolders.SelectedIndex = 0;
            ebm.ShowDialog();
            if (!ebm.SaveClicked) return;
            string folderName = ebm.cmbImportedFolders.Text;
            switch (type)
            {
                #region fcs
                case IMPORT_TYPE_FCS:
                        bool createNewFvm = true;
                        FolderVM fcsVm = null;
                        foreach (FolderVM fvm in FoldersAndSitesList)
                        {
                            if (fvm.TypeOfFolder == FolderTypes.Import && fvm.ImportType == IMPORT_TYPE_FCS && fvm.Name == folderName)
                            {
                                createNewFvm = false;
                                fcsVm = fvm;
                                break;
                            }
                        }

                        if (createNewFvm)
                        {
                            fcsVm = new FolderVM()
                            {
                                Name = folderName,
                                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/fcs icon.png")),
                                IsFolder = true,
                                TypeOfFolder = FolderTypes.Import,
                                ImportType = IMPORT_TYPE_FCS
                            };

                            FoldersAndSitesList.Add(fcsVm);
                        }
                        if (FoldersAndSitesList.Count == 1)
                        {
                            SIFoldersSide = 0;
                            Visible_Sites = true;
                        }

                        //new Thread(() =>
                        //{
                            try
                            {
                                foreach (string line in File.ReadAllLines(path))
                                {
                                    string[] split = line.Split(',');
                                    string SITENAME = split[0];
                                    string EMAIL = split[1];
                                    string USERNAME = split[2];
                                    string PASSWORD = split[3];
                                    string URL = split[4];
                                   // App.Current.Dispatcher.Invoke((Action)delegate
                                    //{
                                        Bookmark bmark = new Bookmark();
                                        bmark.Link = URL;
                                        bmark.Name = SITENAME;
                                        bmark.Email = EMAIL;
                                        bmark.Username = USERNAME;
                                        bmark.Password = PASSWORD;
                                        bmark.IsImported = true;
                                        bmark.ImportType = type;
                                        bmark.DateTimeStamp = DateTime.Now.ToString();
                                        bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));

                                        fcsVm.Sites.Add(bmark);
                                    //});
                                }

                                saveAllImportedSites();
                            }
                            catch 
                            {
                                MessageBox.Show("Ooops something went wrong on import some of the sites may not have been added check the file and try again (if the folder was added delete it before trying again).");
                            }
                       // }).Start();
                    break;
                #endregion

                #region eb
                case IMPORT_TYPE_EB:
                        bool createNeweb = true;
                        FolderVM ebVm = null;
                        foreach (FolderVM fvm in FoldersAndSitesList)
                        {
                            if (fvm.TypeOfFolder == FolderTypes.Import && fvm.ImportType == IMPORT_TYPE_EB && fvm.Name == folderName)
                            {
                                createNeweb = false;
                                ebVm = fvm;
                                break;
                            }
                        }

                        if (createNeweb)
                        {
                            ebVm = new FolderVM()
                            {
                                Name = folderName,
                                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/enterprise buddy.ico")),
                                IsFolder = true,
                                TypeOfFolder = FolderTypes.Import,
                                ImportType = IMPORT_TYPE_EB
                            };

                            FoldersAndSitesList.Add(ebVm);
                        }
                        if (FoldersAndSitesList.Count == 1)
                        {
                            SIFoldersSide = 0;
                            Visible_Sites = true;
                        }
                            try
                            {
                                foreach (string line in File.ReadAllLines(path))
                                {
                                    string[] split = line.Split(',');
                                    string SITENAME = split[0];
                                    string URL = split[1];
                                    string EMAIL = split[2];
                                    string PASSWORD = split[3];
                                    string USERNAME = split[4];

                                    Bookmark bmark = new Bookmark();
                                    bmark.Link = URL;
                                    bmark.Name = SITENAME;
                                    bmark.Email = EMAIL;
                                    bmark.Username = USERNAME;
                                    bmark.Password = PASSWORD;
                                    bmark.IsImported = true;
                                    bmark.ImportType = type;
                                    bmark.DateTimeStamp = DateTime.Now.ToString();
                                    bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));

                                    ebVm.Sites.Add(bmark);
                                }

                                saveAllImportedSites();
                            }
                            catch 
                            {
                                MessageBox.Show("Ooops something went wrong on import some of the sites may not have been added check the file and try again (if the folder was added delete it before trying again).");
                            }
                    break;
                #endregion

                #region rankWYZ
                case IMPORT_TYPE_RANKWYZ:
                    bool createNeweRankWyz = true;
                    FolderVM ebVmRank = null;
                    foreach (FolderVM fvm in FoldersAndSitesList)
                    {
                        if (fvm.TypeOfFolder == FolderTypes.Import && fvm.ImportType == IMPORT_TYPE_RANKWYZ && fvm.Name == folderName)
                        {
                            createNeweRankWyz = false;
                            ebVmRank = fvm;
                            break;
                        }
                    }

                    if (createNeweRankWyz)
                    {
                        ebVmRank = new FolderVM()
                        {
                            Name = folderName,
                            BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/rankwyz-icon-check.png")),
                            IsFolder = true,
                            TypeOfFolder = FolderTypes.Import,
                            ImportType = IMPORT_TYPE_RANKWYZ
                        };

                        FoldersAndSitesList.Add(ebVmRank);
                    }
                    if (FoldersAndSitesList.Count == 1)
                    {
                        SIFoldersSide = 0;
                        Visible_Sites = true;
                    }
                    try
                    {
                        foreach (string line in File.ReadAllLines(path))
                        {
                            string[] split = line.Split(',');
                            string SITENAME = split[0];
                            string URL = split[0];
                            string EMAIL = split[1];
                            string USERNAME = split[1];
                            string PASSWORD = split[2];

                            Bookmark bmark = new Bookmark();
                            bmark.Link = URL;
                            bmark.Name = SITENAME;
                            bmark.Email = EMAIL;
                            bmark.Username = USERNAME;
                            bmark.Password = PASSWORD;
                            bmark.IsImported = true;
                            bmark.ImportType = type;
                            bmark.DateTimeStamp = DateTime.Now.ToString();
                            bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));

                            ebVmRank.Sites.Add(bmark);
                        }

                        saveAllImportedSites();
                    }
                    catch
                    {
                        MessageBox.Show("Ooops something went wrong on import some of the sites may not have been added check the file and try again (if the folder was added delete it before trying again).");
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }

        private void saveAllImportedSites()
        {
            string dirForCustomImport = Path.Combine(MyFilesDatabase.GetBaseDir(), "CustomImports", GloableProfData.PData.ProjectName);
            if (Directory.Exists(dirForCustomImport)) DeleteDirectory(dirForCustomImport);

            foreach (FolderVM folderListItem in FoldersAndSitesList)
            {
                if (folderListItem.TypeOfFolder != FolderTypes.Import) continue;
                string dirpath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CustomImports", GloableProfData.PData.ProjectName, folderListItem.ImportType, folderListItem.Name);
                if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
                string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CustomImports", GloableProfData.PData.ProjectName, folderListItem.ImportType, folderListItem.Name, "SavedImports.txt");

                if (File.Exists(filePath)) File.Delete(filePath);

                foreach (Bookmark bmark in folderListItem.Sites)
                {
                    File.AppendAllText(filePath,
                        bmark.DateTimeStamp + SPLITTER +
                        bmark.Email + SPLITTER + 
                        bmark.Link + SPLITTER +
                        bmark.Name + SPLITTER +
                        bmark.Password + SPLITTER + 
                        bmark.Username + Environment.NewLine);
                }
            }
        }

        public void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public void FillImportsList()
        {
            string dirForCustomImport = Path.Combine(MyFilesDatabase.GetBaseDir(), "CustomImports", GloableProfData.PData.ProjectName);
            if (!Directory.Exists(dirForCustomImport)) return;

            DirectoryInfo dInfo = new DirectoryInfo(dirForCustomImport);
            foreach (DirectoryInfo item in dInfo.GetDirectories())
            {
                foreach (DirectoryInfo dInfoWithFiles in item.GetDirectories())
                {
                    foreach (FileInfo file in dInfoWithFiles.GetFiles())
                    {
                        FolderVM fcsVm = null;
                        if (item.Name == IMPORT_TYPE_FCS)
                        {
                            fcsVm = new FolderVM()
                            {
                                Name = dInfoWithFiles.Name,
                                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/fcs icon.png")),
                                IsFolder = true,
                                TypeOfFolder = FolderTypes.Import,
                                ImportType = IMPORT_TYPE_FCS
                            };
                        }

                        if (item.Name == IMPORT_TYPE_EB)
                        {
                            fcsVm = new FolderVM()
                            {
                                Name = dInfoWithFiles.Name,
                                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/enterprise buddy.ico")),
                                IsFolder = true,
                                TypeOfFolder = FolderTypes.Import,
                                ImportType = IMPORT_TYPE_EB
                            };
                        }

                        if (item.Name == IMPORT_TYPE_RANKWYZ)
                        {
                            fcsVm = new FolderVM()
                            {
                                Name = dInfoWithFiles.Name,
                                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/rankwyz-icon-check.png")),
                                IsFolder = true,
                                TypeOfFolder = FolderTypes.Import,
                                ImportType = IMPORT_TYPE_RANKWYZ
                            };
                        }
                        foreach (string line in File.ReadAllLines(file.FullName))
                            {
                                string[] split = line.Split(new string[] { SPLITTER }, StringSplitOptions.None);
                                string dt = split[0];
                                string SITENAME = split[3];
                                string EMAIL = split[1];
                                string USERNAME = split[5];
                                string PASSWORD = split[4];
                                string URL = split[2];
                                Bookmark bmark = new Bookmark();
                                bmark.Link = URL;
                                bmark.Name = SITENAME;
                                bmark.Email = EMAIL;
                                bmark.Username = USERNAME;
                                bmark.Password = PASSWORD;
                                bmark.IsImported = true;
                                bmark.DateTimeStamp = dt;
                                bmark.ImportType = item.Name;
                                bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
                                fcsVm.Sites.Add(bmark);
                            }
                            FoldersAndSitesList.Add(fcsVm);
                       // }
                    }
                }
            }
            //foreach (string filepath in Directory.GetFiles(dirForCustomImport))
            //{
            //    if (filepath.Contains(IMPORT_TYPE_FCS))
            //    {
            //        FolderVM fcsVm = new FolderVM()
            //        {
            //            Name = "FCS Network",
            //            BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/fcs icon.png")),
            //            IsFolder = true,
            //            IsImported = true,
            //            ImportType = IMPORT_TYPE_FCS
            //        };
            //        foreach (string line in File.ReadAllLines(filepath))
            //        {
            //            string[] split = line.Split(new string[] { SPLITTER }, StringSplitOptions.None);
            //            string dt = split[0];
            //            string SITENAME = split[3];
            //            string EMAIL = split[1];
            //            string USERNAME = split[5];
            //            string PASSWORD = split[4];
            //            string URL = split[2];
            //            Bookmark bmark = new Bookmark();
            //            bmark.Link = URL;
            //            bmark.Name = SITENAME;
            //            bmark.Email = EMAIL;
            //            bmark.Username = USERNAME;
            //            bmark.Password = PASSWORD;
            //            bmark.IsImported = true;
            //            bmark.DateTimeStamp = dt;
            //            bmark.BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png"));
            //            fcsVm.Sites.Add(bmark);
            //        }
            //        FoldersAndSitesList.Add(fcsVm);
            //    }
            //}

            if (FoldersAndSitesList.Count == 1)
            {
                SIFoldersSide = 0;
                Visible_Sites = true;
            }
        }

        private void deleteImportedFolder()
        {
            try
            {
                string dirForCustomImport = Path.Combine(MyFilesDatabase.GetBaseDir(), "CustomImports", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(dirForCustomImport)) return;

                string filePath = Path.Combine(dirForCustomImport, FoldersAndSitesList[SIFoldersSide].ImportType, FoldersAndSitesList[SIFoldersSide].Name);
                if (Directory.Exists(filePath)) Directory.Delete(filePath, true);
            }
            catch { }
        }

        #endregion

        #region sessions
        public void SaveSession(List<string> links)
        {
            SetNameAndDataWindow setNameWindow = new SetNameAndDataWindow();
            setNameWindow.tblockInfo.Text = "Create a name for the folder.";
            setNameWindow.ShowDialog();
            if (!setNameWindow.OkClicked) return;
            if(FoldersAndSitesList.Any(t => t.Name == setNameWindow.tbInputText.Text))
            {
                MessageBox.Show("Choose another name for the session this one already exists in your bookmarks collection.");
                return;
            }
            FolderVM folder = new FolderVM()
            {
                Name = setNameWindow.tbInputText.Text,
                DateTimeStamp = DateTime.Now.ToString(),
                TypeOfFolder = FolderTypes.Session,
                Sites = new ObservableCollection<Bookmark>(),
                IsFolder = true,
                BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/restore.png")),
            };
            FoldersAndSitesList.Add(folder);
            foreach (string link in links)
            {
                folder.Sites.Add(new Bookmark()
                {
                    BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png")),
                    DateTimeStamp = DateTime.Now.ToString(),
                    Link = link,
                    Name = link
                });
            }

            SaveSessionBookmarksToFile();
        }

        private void SaveSessionBookmarksToFile()
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=> 
            {
                foreach (FolderVM folder in FoldersAndSitesList.Where(f=>f.TypeOfFolder == FolderTypes.Session))
                {
                    MyFilesDatabase.SaveBookmarkedSession(GloableProfData.PData.ProjectName, folder.Name, folder.Sites.Select(t => t.Link).ToArray(),folder.Sites.Select(t => t.Name).ToArray(),folder.Sites.Select(t=>t.DateTimeStamp).ToArray());
                }
            });
        }

        public void FillSessionListFromFile()
        {
            string directory = Path.Combine(MyFilesDatabase.GetBaseDir(), "BookmarkSessions", GloableProfData.PData.ProjectName);
            if (!Directory.Exists(directory)) return;

            DirectoryInfo diInfo = new DirectoryInfo(directory);
            foreach (DirectoryInfo dir in diInfo.GetDirectories())
            {
                FolderVM folder = new FolderVM()
                {
                    Name = dir.Name,
                    DateTimeStamp = dir.CreationTime.ToString(),
                    TypeOfFolder = FolderTypes.Session,
                    Sites = new ObservableCollection<Bookmark>(),
                    IsFolder = true,
                    BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/restore.png")),
                };
                FoldersAndSitesList.Add(folder);

                string file = Path.Combine(dir.FullName, "sites.txt");
                if(File.Exists(file))
                foreach (string link in File.ReadAllLines(file))
                {
                    string[] linkSplit = link.Split(new string[] { MyFilesDatabase.SPLITTER},StringSplitOptions.RemoveEmptyEntries);
                    folder.Sites.Add(new Bookmark()
                    {
                        BitmapImg = new BitmapImage(new Uri("pack://application:,,,/Organiser.Common;component/Image/new_document.png")),
                        Link = linkSplit[0],
                        Name = linkSplit[1],
                        DateTimeStamp = linkSplit[2],
                    });
                }
            }
        }

        #endregion
    }
}
