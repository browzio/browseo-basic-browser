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
    public class InstaMedia : Media, INotifyPropertyChanged
    {
        public event Action<InstaMedia, string> OnRaisedCommandToViewModel = delegate { };
        [Newtonsoft.Json.JsonIgnore]
        public ICommand OnCommandFromView { get; set; }

        private string shortcode;
        public string Shortcode
        {
            get { return shortcode; }
            set { shortcode = value; RaisePropertyChanged("Shortcode"); }
        }

        
        public int CommentCount
        {
            get { return Comments != null ? Comments.Count : 0; }
            set { if (Comments != null) Comments.Count = value; RaisePropertyChanged("CommentCount"); }
        }

        //CommentsData
        public ObservableCollection<Comment> CommentsData
        {
            get;
            set;
        }


        private string comment;
        public string AddCommentText
        {
            get { return comment; }
            set
            {
                //if (comment == "Add A Comment")
                //    value = value.Replace("Add A Comment", "");
                comment = value;
                RaisePropertyChanged("AddCommentText");
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; RaisePropertyChanged("IsChecked"); }
        }

        object comment_lock = new object();

        public InstaMedia()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            CommentsData = new ObservableCollection<Comment>();
            Shortcode = null;

            AddCommentText = "Add A Comment";
        }

        public InstaMedia(Media m) : this()
        {
            CloneAll(m);
        }

        //public void SetCommentsLock()
        //{
        //    if (Comments.Data == null || comment_lock == null) return;

        //    comment_lock = new object();
        //    System.Windows.Data.BindingOperations.EnableCollectionSynchronization(CommentsData, comment_lock);
        //}

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            if (param == "Clicked")
            {
              if(AddCommentText == "Add A Comment")  AddCommentText = "";
            }
            else
            {
                OnRaisedCommandToViewModel(this, param);
            }
        }

        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void CloneAll(InstaMedia im)
        {
            Type t = im.GetType();
            foreach (FieldInfo fieldInf in t.GetFields())
            {
                fieldInf.SetValue(this, fieldInf.GetValue(im));
            }
            foreach (PropertyInfo propInf in t.GetProperties())
            {
                propInf.SetValue(this, propInf.GetValue(im));
            }

            RaisePropertyChanged("CommentsData");
        }

        internal void CloneAll(Media m)
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

            RaisePropertyChanged("CommentsData");
        }
    }
}
