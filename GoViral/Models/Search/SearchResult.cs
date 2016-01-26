using Organiser.Common.Classes;
using Organiser.Common.Classes.Facebook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace GoViral.Models
{
    [Serializable]
    [XmlType("GoViral.Models.SearchResult")]
    public class SearchResult
    {
        //public ICommand OnClickCommandFromView { get; set; }

        private string kw;
        public string Keyword
        {
            get { return kw; }
            set { kw = value; }
        }

        public SearchResult()
        {
            //OnClickCommandFromView = new RelayCommand(OnClickCommandFromView_Executed);
            PersonsResult = new PersonsResult();
            PlacesResult = new PlacesResult();
            EventsResult = new EventsResult();
            GroupsResult = new GroupsResult();
            PagesResult = new PagesResult();
            MediaResult = new MediaResult();
            MediaResultVideos = new MediaResult();
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

        private MediaResult mediaResult;
        public MediaResult MediaResult
        {
            get
            {
                return mediaResult;
            }
            set
            {
                mediaResult = value;
            }
        }

        private MediaResult mediaResultVideos;
        public MediaResult MediaResultVideos
        {
            get
            {
                return mediaResultVideos;
            }
            set
            {
                mediaResultVideos = value;
            }
        }


        //private void OnClickCommandFromView_Executed(object param)
        //{
        //    switch ((string)param)
        //    {
        //        case "DELETE":

        //            break;

        //        default:
        //            break;
        //    }
        //}

        internal void RemoveThisResultFromData(object selectedItem)
        {
            if (selectedItem is PersonsResultData)
            {
                PersonsResult.data.Remove(selectedItem as PersonsResultData);
            }
            else if (selectedItem is PlacesResultData)
            {
                PlacesResult.data.Remove(selectedItem as PlacesResultData);
            }
            else if (selectedItem is EventsResultData)
            {
                EventsResult.data.Remove(selectedItem as EventsResultData);
            }
            else if (selectedItem is PagesResultData)
            {
                PagesResult.data.Remove(selectedItem as PagesResultData);
            }
            else if (selectedItem is GroupsResultData)
            {
                GroupsResult.data.Remove(selectedItem as GroupsResultData);
            }
            else if (selectedItem is MediaResultData)
            {
                if (MediaResult.data.Contains(selectedItem as MediaResultData))
                {
                    MediaResult.data.Remove(selectedItem as MediaResultData);
                }
                else if (MediaResultVideos.data.Contains(selectedItem as MediaResultData))
                {
                    MediaResultVideos.data.Remove(selectedItem as MediaResultData);
                }
            }
        }

        internal void RemoveThisResultFromDataConditionally(object selectedItem, bool miOrderPrivacyOpen, bool miOrderPrivacyClosed)
        {
            if (selectedItem is GroupsResultData)
            {
                GroupsResultData gDataIQuestion = selectedItem as GroupsResultData;
                if ((miOrderPrivacyOpen && gDataIQuestion.privacy == "OPEN") ||
                    (miOrderPrivacyClosed && gDataIQuestion.privacy == "CLOSED") ||
                    (!miOrderPrivacyOpen && !miOrderPrivacyClosed))
                    GroupsResult.data.Remove(gDataIQuestion);
            }
            else 
            {
                RemoveThisResultFromData(selectedItem);
            }
        }

        internal void ClearAllDataFrom(object dataContext)
        {
            if (dataContext is ObservableCollection<PersonsResultData>)
            {
                PersonsResult.data.Clear();
            }
            else if (dataContext is ObservableCollection<PlacesResultData>)
            {
                PlacesResult.data.Clear();
            }
            else if (dataContext is ObservableCollection<EventsResultData>)
            {
                EventsResult.data.Clear();
            }
            else if (dataContext is ObservableCollection<GroupsResultData>)
            {
                GroupsResult.data.Clear();
            }
            else if (dataContext is ObservableCollection<PagesResultData>)
            {
                PagesResult.data.Clear();
            }
            else if (dataContext is ObservableCollection<MediaResultData>)
            {
                (dataContext as ObservableCollection<MediaResultData>).Clear();
            }
        }
    }

    [Serializable]
    public class SearchResultWithKeyWord : ViewModelBase
    {
        
        private ObservableCollection<SearchResult> sarchResultsList;
        public ObservableCollection<SearchResult> SearchResultsList
        {
            get { return  sarchResultsList; }
            set {  sarchResultsList = value; }
        }

        private int siSearchResultList;
        public int SISearchResultList
        {
            get { return siSearchResultList; }
            set { siSearchResultList = value; RaisePropertyChanged("SISearchResultList"); }
        }



        public SearchResultWithKeyWord()
        {
            SearchResultsList = new ObservableCollection<SearchResult>();
        }

    }
}
