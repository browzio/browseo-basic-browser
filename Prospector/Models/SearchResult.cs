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

        private string domainScore;
        public string DomainScore
        {
            get { return domainScore; }
            set
            {
                domainScore = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DomainScore"));
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

        private int position;
        public int Position
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
        private string lang;
        public string Lang
        {
            get { return lang; }
            set
            {
                lang = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Lang"));
            }
        }
        private string pnum;
        public string Pnum
        {
            get { return pnum; }
            set
            {
                pnum = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Pnum"));
            }
        }//Ptotal
        private string ptotal;
        public string Ptotal
        {
            get { return ptotal; }
            set
            {
                ptotal = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Ptotal"));
            }
        }
        //PerformanceScore
        private string performanceScore;
        public string PerformanceScore
        {
            get { return performanceScore; }
            set
            {
                performanceScore = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PerformanceScore"));
            }
        }
        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                country = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Country"));
            }
        }
        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
            }
        }
        //Spamscore
        private string spamscore;
        public string Spamscore
        {
            get { return spamscore; }
            set
            {
                spamscore = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Spamscore"));
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

        private Visibility authorityVisible = Visibility.Collapsed;
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

        private Visibility webhoseExtraVisible = Visibility.Collapsed;
        public Visibility WebhoseExtraVisible
        {
            get { return webhoseExtraVisible; }
            set
            {
                webhoseExtraVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("WebhoseExtraVisible"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
