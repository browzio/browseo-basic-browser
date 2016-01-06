using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                if (sISavedLinks != value)
                {
                    sISavedLinks = value;
                    RaiseSiChanged(this);
                    RaisePropertyChanged("WebBrowserHost");
                }
            }
        }

        private bool isEExpanded;  
        public bool IsEExpanded
        {
            get { return isEExpanded; }
            set { isEExpanded = value; RaisePropertyChanged("WebBrowserHost"); }
        }



        public Folder()
        {
            CTMenuClick = new RelayCommand(On_CTMenuClick);
            SavedLinksList = new ObservableCollection<ListOption>(); 
            SISavedLinks = -1;
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

                case "ORDER_Likes":
                case "ORDER_Shares":
                    FacebookGraphData fbgData = SavedLinksList[SISavedLinks].FBGraphData;
                    if (fbgData == null || fbgData.posts == null || fbgData.posts.data==null) return;

                    List<FacebookGraphPostResult> prListToOrder = fbgData.posts.data.OrderByDescending(l =>
                    commandParam == "ORDER_Likes" ? 
                    (l.likes == null ? 0 : l.likes.data == null ? 0 : l.likes.data.Count) : 
                    (l.shares == null ? 0 : l.shares.count)).ToList();
                    fbgData.posts.data.Clear();
                    foreach (FacebookGraphPostResult pResult in prListToOrder)
                    {
                        fbgData.posts.data.Add(pResult);
                    }
                    break;

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
    }
}
