using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoViral.Models
{
    public class ListOption : ViewModelBase
    {
        public event Action OnFBGraphDataChanged = delegate { };
        private string name;  
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; RaisePropertyChanged("Url"); }
        }

        private bool isSelected;
        public bool IsSelected

        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged("IsSelected"); }
        }

        private FacebookGraphData fBGraphData;
        public FacebookGraphData FBGraphData
        {
            get { return fBGraphData; }
            set { fBGraphData = value; RaisePropertyChanged("FBGraphData"); OnFBGraphDataChanged(); }
        } 
    }
}
