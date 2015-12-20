using Organiser.Common.Classes;
using Organiser.Common.Classes.Facebook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GoViral.Models
{
    [Serializable]
    [XmlType("GoViral.Models.SearchResult")]
    public class SearchResult 
    {
        private string kw;
        public string Keyword
        {
            get { return kw; }
            set { kw = value; }
        }

        public SearchResult()
        {
            PersonsResult = new PersonsResult();
            PlacesResult = new PlacesResult();
            EventsResult = new EventsResult();
            GroupsResult = new GroupsResult();
            PagesResult = new PagesResult();
        }

        private PersonsResult personsResult;
        public PersonsResult PersonsResult
        {
            get
            {
                return personsResult;
            }
            set
            {
                personsResult = value;               
            }
        }

        private PlacesResult placesResult;
        public PlacesResult PlacesResult
        {
            get
            {
                return placesResult;
            }
            set
            {
                placesResult = value;                
            }
        }

        private EventsResult eventsResult;
        public EventsResult EventsResult
        {
            get
            {
                return eventsResult;
            }
            set
            {
                eventsResult = value;                
            }
        }

        private GroupsResult groupsResult;
        public GroupsResult GroupsResult
        {
            get
            {
                return groupsResult;
            }
            set
            {
                groupsResult = value;                  
            }
        }

        private PagesResult pagesResult;
        public PagesResult PagesResult
        {
            get
            {
                return pagesResult;
            }
            set
            {
                pagesResult = value;                  
            }
        }        
    }

    [Serializable]
    public class SearchResultWithKeyWord
    {
        
        private ObservableCollection<SearchResult> sarchResultsList;

        public ObservableCollection<SearchResult> SearchResultsList
        {
            get { return  sarchResultsList; }
            set {  sarchResultsList = value; }
        }


        public SearchResultWithKeyWord()
        {
            SearchResultsList = new ObservableCollection<SearchResult>();
        }

    }
}
