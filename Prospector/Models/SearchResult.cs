using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Prospector.Models
{
    public class SearchResult : INotifyPropertyChanged
    {

        private string keyword;
        public string Keyword
        {
            get { return keyword; }
            set
            {
                keyword = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Keyword"));
            }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Link"));
            }
        }

        private string searchEngine;
        public string SearchEngine
        {
            get { return searchEngine; }
            set
            {
                searchEngine = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchEngine"));
            }
        }

        private string position;
        public string Position
        {
            get { return position; }
            set
            {
                position = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Position"));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Description"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
