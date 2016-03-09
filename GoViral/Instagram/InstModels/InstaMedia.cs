using InstaSharp.Models;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
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
        public ICommand OnCommandFromView { get; set; }

        private string shortcode;
        public string Shortcode
        {
            get { return shortcode; }
            set { shortcode = value; RaisePropertyChanged("Shortcode"); }
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



        public InstaMedia()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            Shortcode = null;

            AddCommentText = "Add A Comment";
        }

        public InstaMedia(Media m) : this()
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
    }
}
