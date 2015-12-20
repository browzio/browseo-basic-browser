using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GoViral.Models
{
    [KnownType(typeof(Organiser.Common.Classes.RelayCommand))]
    [DataContract(Namespace = "GoViral.Models")]
    public class SavedSyncProject  : ViewModelBase
    {
        //public string CreatedFromProjectName { get; set; }

        private string projectName; 
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; RaisePropertyChanged("ProjectName"); }
        }
        public string Name { get { return projectName; } }

        public string UrlsToSyndicateMessage
        {
            get
            {
                if (SyndicatedPostsList == null)
                {
                    return "0 Urls To Syndicate";
                }

                return SyndicatedPostsList.Count + " Urls To Syndicate";
            }
        }

        private string isSyncedMessage;
        public string IsSyncedMessage
        {
            get { return isSyncedMessage; }
            set { isSyncedMessage = value; RaisePropertyChanged("IsSyncedMessage"); }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                //if (!value)
                //{
                //if (ProjectName == GloableProfData.PData.ProjectName)
                //      IsChecked = true;
                //} 
                RaisePropertyChanged("IsChecked");
            }
        }

        private double width;
        public double Width
        {
            get { return Controls.SharedPostsUserControl.TbMaxWidth; }
        }

        private ObservableCollection<SyncedProjectData> syndicatedPostsList;   
        public ObservableCollection<SyncedProjectData> SyndicatedPostsList
        {
            get { return syndicatedPostsList; }
            set { syndicatedPostsList = value; }
        }
        private int sISyndicatedPostsList; 
        public int SISyndicatedPostsList
        {
            get { return sISyndicatedPostsList; }
            set { sISyndicatedPostsList = value; RaisePropertyChanged("SISyndicatedPostsList"); }
        }

        public SavedSyncProject()
        {
            SyndicatedPostsList = new ObservableCollection<SyncedProjectData>();
            SyndicatedPostsList.CollectionChanged += SyndicatedPostsList_CollectionChanged;
        }

        private void SyndicatedPostsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("UrlsToSyndicateMessage");
        }
    }
}
