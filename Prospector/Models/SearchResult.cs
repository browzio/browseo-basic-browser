using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
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
    public class SearchResult : ViewModelBase, IHaveSocialStats
    {

        private string keyword;
        public string Keyword
        {
            get { return keyword; }
            set
            {
                keyword = value; RaisePropertyChanged("Keyword");
            }
        }

        private string domainScore;
        public string DomainScore
        {
            get { return domainScore; }
            set
            {
                domainScore = value; RaisePropertyChanged("DomainScore");
            }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set
            {
                link = value; RaisePropertyChanged("Link");
            }
        }

        private string searchEngine;
        public string SearchEngine
        {
            get { return searchEngine; }
            set
            {
                searchEngine = value; RaisePropertyChanged("SearchEngine");
            }
        }

        private int position;
        public int Position
        {
            get { return position; }
            set
            {
                position = value; RaisePropertyChanged("Position");
            }
        }

        private int timesKwFound;
        public int TimesKwFound
        {
            get { return timesKwFound; }
            set
            {
                timesKwFound = value; RaisePropertyChanged("TimesKwFound");
            }
        }


        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value; RaisePropertyChanged("Title");
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value; RaisePropertyChanged("Description");
            }
        }

        private string published;
        public string Published
        {
            get { return published; }
            set
            {
                published = value; RaisePropertyChanged("Published");
            }
        }
        private string lang;
        public string Lang
        {
            get { return lang; }
            set
            {
                lang = value; RaisePropertyChanged("Lang");
            }
        }
        private string pnum;
        public string Pnum
        {
            get { return pnum; }
            set
            {
                pnum = value; RaisePropertyChanged("Pnum");
            }
        }//Ptotal
        private string ptotal;
        public string Ptotal
        {
            get { return ptotal; }
            set
            {
                ptotal = value; RaisePropertyChanged("Ptotal");
            }
        }
        //PerformanceScore
        private string performanceScore;
        public string PerformanceScore
        {
            get { return performanceScore; }
            set
            {
                performanceScore = value; RaisePropertyChanged("PerformanceScore");
            }
        }
        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                country = value; RaisePropertyChanged("Country");
            }
        }
        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                type = value; RaisePropertyChanged("Type");
            }
        }
        //Spamscore
        private string spamscore;
        public string Spamscore
        {
            get { return spamscore; }
            set
            {
                spamscore = value; RaisePropertyChanged("Spamscore");
            }
        }

        private string pageAuthority;
        public string PageAuthority
        {
            get { return pageAuthority; }
            set
            {
                pageAuthority = value; RaisePropertyChanged("PageAuthority");
            }
        }

        private string domainAuthority;
        public string DomainAuthority
        {
            get { return domainAuthority; }
            set
            {
                domainAuthority = value; RaisePropertyChanged("DomainAuthority");
            }
        }

        private Visibility authorityVisible = Visibility.Collapsed;
        public Visibility AuthorityVisible
        {
            get { return authorityVisible; }
            set
            {
                authorityVisible = value; RaisePropertyChanged("AuthorityVisible");
            }
        }

        private Visibility webhoseExtraVisible = Visibility.Collapsed;
        public Visibility WebhoseExtraVisible
        {
            get { return webhoseExtraVisible; }
            set
            {
                webhoseExtraVisible = value; RaisePropertyChanged("WebhoseExtraVisible");
            }
        }

        private Visibility dwExtras = Visibility.Visible;
        public Visibility DwExtras
        {
            get { return dwExtras; }
            set
            {
                dwExtras = value; RaisePropertyChanged("DwExtras");
            }
        }

        private SocialStatsReplys socialStatsReplys;
        public SocialStatsReplys SocialStatsReplys
        {
            get { return socialStatsReplys; }
            set { socialStatsReplys = value; RaisePropertyChanged("SocialStatsReplys"); }
        }
    }
}
