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
    public class InstaTag : Tag, INotifyPropertyChanged
    {
        public event Action<InstaTag, string> OnRaisedCommandToViewModel = delegate { };
        [Newtonsoft.Json.JsonIgnore]
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<InstaMedia> MediaRecent { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; RaisePropertyChanged("IsChecked"); }
        }

        public InstaTag()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            MediaRecent = new ObservableCollection<InstaMedia>();
        }

        public InstaTag(Tag m) : this()
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

        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal void CloneAll(InstaTag ta)
        {
            Type t = ta.GetType();
            foreach (FieldInfo fieldInf in t.GetFields())
            {
                fieldInf.SetValue(this, fieldInf.GetValue(ta));
            }
            foreach (PropertyInfo propInf in t.GetProperties())
            {
                propInf.SetValue(this, propInf.GetValue(ta));
            }

            RaisePropertyChanged("MediaRecent");
        }
    }
}
