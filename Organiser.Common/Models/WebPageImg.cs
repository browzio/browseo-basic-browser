using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserHost.Models
{
    public class WebPageImg : INotifyPropertyChanged
    {
        private string imgUrl;

        public string ImgUrl
        {
            get { return imgUrl; }
            set 
            { 
                imgUrl = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImgUrl"));
            }
        }

        private string webUrl;

        public string WebUrl
        {
            get { return webUrl; }
            set
            {
                webUrl = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("WebUrl"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
