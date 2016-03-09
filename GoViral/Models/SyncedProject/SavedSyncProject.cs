using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace GoViral.Models
{
    [KnownType(typeof(Organiser.Common.Classes.RelayCommand))]
    [DataContract(Namespace = "GoViral.Models")]
    public class SavedSyncProject  : ViewModelBase
    {
        [XmlIgnore]
        public ICommand OnCommandRaisedInView { get; set; }
        //public string CreatedFromProjectName { get; set; }

        private string projectName; 
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; RaisePropertyChanged("ProjectName"); }
        }
        public string Name { get { return projectName; } }
        public string TypeOfSync { get; set; }

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

        //object mLock = new object();

        public SavedSyncProject()
        {
            SyndicatedPostsList = new ObservableCollection<SyncedProjectData>();
            SyndicatedPostsList.CollectionChanged += SyndicatedPostsList_CollectionChanged;

            OnCommandRaisedInView = new RelayCommand(OnCommandRaisedInView_Activated);
        }

        private void OnCommandRaisedInView_Activated(object param)
        {
            switch ((string)param)
            {
                case "OpenInProjBrowser":
                    new Thread(launchInBrowser).Start();
                    break;
                default:
                    break;
            }
        }

        private void launchInBrowser()
        {
            try
            {
                UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_SEOEvent + " url " + SyndicatedPostsList[SISyndicatedPostsList].Url);
                string projpath = MyFilesDatabase.FindProjectDirByName(Name, "");
                string url = SyndicatedPostsList[SISyndicatedPostsList].Url;

                var info = new ProcessStartInfo
                {
                    Arguments = projpath.Replace(" ", MyFilesDatabase.SPLITTER) + " " + url.Replace(" ", MyFilesDatabase.SPLITTER) + " " + TypeOfSync,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "AnyProjectBrowserProcess.exe"//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AnyProjectBrowserProcess.exe"),
                };

                
                Process p = Process.Start(info);
                ProcessManager.Instance.AddProcess(p);
                p.WaitForExit();
                ProcessManager.Instance.DisposeAndRemoveProcess(p);
            }
            catch
            {
                //MessageBox.Show("Failed to launch in browser.");
            }
        }

        private void SyndicatedPostsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("UrlsToSyndicateMessage");
        }
    }
}
