using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace GoViral.Models
{
    public enum ClickState
    {
        Right,
        Left
    }

    [KnownType(typeof(Organiser.Common.Classes.RelayCommand))]
    [DataContract(Namespace = "GoViral.Models")]
    public class Folder : ViewModelBase
    {
        public event Action<string> OnLoadInBrowser = delegate { };
        public event Action<Folder,string> OnSelectedCheckStats = delegate { };
        public event Action<Folder> OnSelectedEditOrRemove = delegate { };
        public event Action<ListOption> OnCanceledAStatsCheck = delegate { };
        public event Action<Folder> RaiseSiChanged = delegate { }; 

        [XmlIgnore]
        public ICommand CTMenuClick { get; set; }

        private string folderTitle;
        public string FolderTitle
        {
            get { return folderTitle; }
            set { folderTitle = value; RaisePropertyChanged("FolderTitle"); }
        }

        private bool isEExpanded;
        public bool IsEExpanded
        {
            get { return isEExpanded; }
            set
            {
                isEExpanded = value;
                RaisePropertyChanged("IsEExpanded");
                if (value)
                {
                   // RaiseSiChanged(this);
                }
                else
                {
                    if (SelectedPage != null)
                    {
                        SelectedPage.IsSelected = false;
                    }
                    SISavedLinks = -1;
                    //RaisePropertyChanged("SISavedLinks");
                    //RaisePropertyChanged("SelectedPageFBGraphData");
                    //RaisePropertyChanged("SelectedPageName");
                    //RaisePropertyChanged("SelectedPage");
                }
            }
        }

        private ObservableCollection<ListOption> savedLinksList;
        public ObservableCollection<ListOption> SavedLinksList
        {
            get { return savedLinksList; }
            set { savedLinksList = value; }
        }                          
        public int sISavedLinks;
        public int SISavedLinks
        {
            get
            {
                return sISavedLinks;
            }
            set
            {
                sISavedLinks = value;
                
                if (value == -1) return;

                if (SelectedPage != null)
                    SelectedPage.IsSelected = true;
                RaiseSiChanged(this);
                RaisePropertyChanged("SISavedLinks");
                RaisePropertyChanged("SelectedPageFBGraphData");
                RaisePropertyChanged("SelectedPageName");
                RaisePropertyChanged("SelectedPage");
                if(SelectedPage != null) SelectedPage.RaisePropertyChanged("FBGraphData");
            }
        } 

        public ListOption SelectedPage
        {
            get
            {
                if (SavedLinksList.Count > 0 && SISavedLinks >= 0)
                    return SavedLinksList[SISavedLinks];
                else
                    return null;
            }
        }

        public string SelectedPageName
        {
            get
            {
                if (SavedLinksList.Count > 0 && SISavedLinks >= 0)
                    return SavedLinksList[SISavedLinks].Name;
                else
                    return null;
            }
        } 

        public FacebookGraphData SelectedPageFBGraphData
        {
            get
            {
                if (SavedLinksList.Count > 0 && SISavedLinks >= 0)
                    return SavedLinksList[SISavedLinks].FBGraphData;
                else
                    return null;
            }
        }
        

        public Folder()
        {
            CTMenuClick = new RelayCommand(On_CTMenuClick);
            SavedLinksList = new ObservableCollection<ListOption>(); 
        }

        public void On_CTMenuClick(object param)
        {
            string commandParam = param as string;
            if (commandParam == null) return;
            switch (commandParam)
            {
                case "Edit":
                    OnSelectedEditOrRemove(this);
                    break;

                case "Delete":
                    if (MessageBox.Show("Are you sure you want to delete " + SavedLinksList[SISavedLinks].Name, "Are You Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        SavedLinksList.RemoveAt(SISavedLinks);
                        OnSelectedEditOrRemove(null);
                    }
                    break;

                case "View":
                    if (SISavedLinks != -1 && SavedLinksList.Count > 0)
                    {
                        OnLoadInBrowser(SavedLinksList[SISavedLinks].Url);
                    }
                    break;

                case "Stats":
                    OnSelectedCheckStats(this, SavedLinksList[SISavedLinks].Url);
                    break;

                case "StatsAll":
                    OnSelectedCheckStats(this, null);
                    break;

                case "Cancel":
                    OnCanceledAStatsCheck(SavedLinksList[SISavedLinks]);
                    break;

                case "CancelAll":
                    OnCanceledAStatsCheck(null);
                    break;

                case "ORDER_Likes":  
                case "ORDER_TalkingAbout":   
                    List<ListOption> loOrderd = SavedLinksList.OrderByDescending(l => l.FBGraphData == null ? 0 : commandParam == "ORDER_Likes" ? l.FBGraphData.likes : l.FBGraphData.talking_about_count).ToList();
                    SavedLinksList.Clear();
                    foreach (ListOption lo in loOrderd)
                    {
                        SavedLinksList.Add(lo);
                    }
                    break;

                //case "ORDER_PostsByLikes":
                //case "ORDER_PostsByShares":
                //    if (SelectedPageFBGraphData != null)
                //    {
                //        if (SelectedPageFBGraphData.posts == null || SelectedPageFBGraphData.posts.data == null) return;

                //        List<FacebookGraphPostResult> pdOrderd = SelectedPageFBGraphData.posts.data.OrderByDescending(l => commandParam == "ORDER_PostsByLikes" ?
                //                                                                                                           (l.likes == null ? 0 : l.likes.summary == null ? 0 : l.likes.summary.total_count) :
                //                                                                                                           (l.shares == null ? 0 : l.shares.count)).ToList();
                //        SelectedPageFBGraphData.posts.data.Clear();
                //        foreach (FacebookGraphPostResult pResult in pdOrderd)
                //        {
                //            SelectedPageFBGraphData.posts.data.Add(pResult);
                //        }

                //        RaisePropertyChanged("SelectedPageFBGraphData");
                //    }
                //    break;

                //case "ORDER_Shares":
                //    FacebookGraphData fbgDataShare = SavedLinksList[SISavedLinks].FBGraphData;
                //    if (fbgDataShare == null || fbgDataShare.posts == null || fbgDataShare.posts.data == null) return;

                //    List<FacebookGraphPostResult> prListToOrderShares = fbgDataShare.posts.data.OrderByDescending(s => s.shares == null ? 0 : s.shares.count).ToList();
                //    fbgDataShare.posts.data.Clear();
                //    foreach (FacebookGraphPostResult pResult in prListToOrderShares)
                //    {
                //        fbgDataShare.posts.data.Add(pResult);
                //    }
                //    break;

                default:
                    break;
            }
        }

        public void Raise_OnFBGraphDataChanged()
        {
            RaisePropertyChanged("SelectedPageFBGraphData");
        }
    }
}
