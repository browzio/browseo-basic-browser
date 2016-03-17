using InstaSharp.Models;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoViral.Instagram.InstModels
{
    public class InstaUser : User , INotifyPropertyChanged
    {
        public event Action<InstaUser, string> OnRaisedCommandToViewModel = delegate { };
        [Newtonsoft.Json.JsonIgnore]
        public ICommand OnCommandFromView { get; set; }

        //public ObservableCollection<Media> MediaLiked { get; set; }
        public ObservableCollection<InstaMedia> MediaRecent { get; set; }
        public ObservableCollection<InstaUser> Folowing { get; set; }
        public ObservableCollection<InstaUser> Folowers { get; set; }

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

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; RaisePropertyChanged("IsChecked"); }
        }


        public InstaUser()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            //MediaLiked = new ObservableCollection<Media>();
            MediaRecent = new ObservableCollection<InstaMedia>();
            Folowing = new ObservableCollection<InstaUser>();
            Folowers = new ObservableCollection<InstaUser>();
        }

        public InstaUser(User m) : this()
        {
            Type t = m.GetType();
            foreach (FieldInfo fieldInf in t.GetFields())
            {
                fieldInf.SetValue(this, fieldInf.GetValue(m));
            }
            foreach (PropertyInfo propInf in t.GetProperties())
            {
                propInf.SetValue(this, propInf.GetValue(m));
            }
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            OnRaisedCommandToViewModel(this, param);
        }

        public InstaUser createInsaUserFromUser(User u)
        {
            InstaUser ur = new InstaUser()
            {
                Bio = u.Bio,
                Counts = u.Counts,
                FullName = u.FullName,
                Id = u.Id,
                ProfilePicture = u.ProfilePicture,
                Username = u.Username,
                Website = u.Website,
            };
            ur.OnRaisedCommandToViewModel += OnRaisedCommandToViewModel;

            return ur;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string property)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        internal void CloneAll(InstaUser ur)
        {
            Type t = ur.GetType();
            foreach (FieldInfo fieldInf in t.GetFields())
            {
                fieldInf.SetValue(this, fieldInf.GetValue(ur));
            }
            foreach (PropertyInfo propInf in t.GetProperties())
            {
                propInf.SetValue(this, propInf.GetValue(ur));
            }

            RaisePropertyChanged("MediaRecent");
        }
    }
}
