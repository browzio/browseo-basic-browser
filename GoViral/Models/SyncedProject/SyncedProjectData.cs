using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoViral.Models
{
    public class SyncedProjectData : ViewModelBase
    {              
        private bool isShared;  
        public bool IsShared
        {
            get { return isShared; }
            set { isShared = value; RaisePropertyChanged("IsShared"); }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; RaisePropertyChanged("Url"); }
        }

        private string pageName;
        public string PageName
        {
            get { return pageName; }
            set { pageName = value; RaisePropertyChanged("PageName"); }
        }

        private string fromProject;
        public string FromProject
        {
            get { return fromProject; }
            set { fromProject = value; RaisePropertyChanged("FromProject"); }
        }

        //
    }
}
