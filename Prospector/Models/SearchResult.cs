using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private string published;
        public string Published
        {
            get { return published; }
            set
            {
                published = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Published"));
            }
        }

        private string pageAuthority;
        public string PageAuthority
        {
            get { return pageAuthority; }
            set
            {
                pageAuthority = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PageAuthority"));
            }
        }

        private string domainAuthority;
        public string DomainAuthority
        {
            get { return domainAuthority; }
            set
            {
                domainAuthority = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DomainAuthority"));
            }
        }

        private Visibility authorityVisible;
        public Visibility AuthorityVisible
        {
            get { return authorityVisible; }
            set
            {
                authorityVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AuthorityVisible"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
