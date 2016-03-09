using InstaSharp.Models;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstModels
{
    public class InstaUserResult : User , INotifyPropertyChanged
    {
        public event Action<InstaUserResult, string> OnRaisedCommandToViewModel = delegate { };
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OnCommandFromView { get; set; }

        //public ObservableCollection<Media> MediaLiked { get; set; }
        public ObservableCollection<InstaMedia> MediaRecent { get; set; }
        public ObservableCollection<InstaUserResult> Folowing { get; set; }
        public ObservableCollection<InstaUserResult> Folowers { get; set; }

        private Relationship relationship;
        public Relationship Relationship
        {
            get { return relationship; }
            set
            {
                relationship = value;
                RaisePropertyChanged("Relationship");
            }
        }


        public InstaUserResult()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            //MediaLiked = new ObservableCollection<Media>();
            MediaRecent = new ObservableCollection<InstaMedia>();
            Folowing = new ObservableCollection<InstaUserResult>();
            Folowers = new ObservableCollection<InstaUserResult>();
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            OnRaisedCommandToViewModel(this, param);
        }

        public void RaisePropertyChanged(string property)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
