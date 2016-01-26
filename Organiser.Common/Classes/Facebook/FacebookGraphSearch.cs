using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes.Facebook
{
    #region people
    [Serializable]
    public class PersonsResult
    {
        private ObservableCollection<PersonsResultData> mdata;
        public ObservableCollection<PersonsResultData> data
        {
            get { return mdata; }
            set { mdata = value; }
        }
        public PersonsResult()
        {
            data = new ObservableCollection<PersonsResultData>();
        }
    }
    public class PersonsResultData
    {
        //https://www.facebook.com/app_scoped_user_id/1560959757489587/
        public string name { get; set; }
        public string id { get; set; }
        public string link { get; set; }
        public Picture picture { get; set; }
        public Paging paging { get; set; }
    }
    #endregion

    #region places

    [Serializable]
    public class PlacesResult
    {
        //private ObservableCollection<Data> mdata;
        //public ObservableCollection<Data> data
        //{
        //    get { return mdata; }
        //    set { mdata = value; }
        //}

        private ObservableCollection<PlacesResultData> mdata;
        public ObservableCollection<PlacesResultData> data
        {
            get { return mdata; }
            set { mdata = value; }
        }
        public PlacesResult()
        {
            data = new ObservableCollection<PlacesResultData>();
        }
    }
    public class PlacesResultData
    {
        public string about { get; set; }
        public string category { get; set; }
        public string can_post { get; set; }
        public string description { get; set; }
        public string founded { get; set; }
        public string is_community_page { get; set; }
        public string is_permanently_closed { get; set; }
        public string is_published { get; set; }
        public string is_unclaimed { get; set; }
        public string is_verified { get; set; }
        public string link { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public long talking_about_count { get; set; }
        public string website { get; set; }
        public long likes { get; set; }
        public Picture picture { get; set; }
        public Location location { get; set; }
        public Paging paging { get; set; }
    }

    public class Location
    {
        public string city { get; set; }
        public string country { get; set; }
        public string zip { get; set; }
    }

    #endregion

    #region events

    [Serializable]
    public class EventsResult
    {
        //private ObservableCollection<Data> mdata;
        //public ObservableCollection<Data> data
        //{
        //    get { return mdata; }
        //    set { mdata = value; }
        //}

        private ObservableCollection<EventsResultData> mdata;
        public ObservableCollection<EventsResultData> data
        {
            get { return mdata; }
            set { mdata = value; }
        }
        public EventsResult()
        {
            data = new ObservableCollection<EventsResultData>();
        }
    }
    public class EventsResultData
    {
        //to url https://www.facebook.com/events/787206314630616

        public string description { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string start_time { get; set; }
        public Picture picture { get; set; }
        public Interested interested { get; set; }
        public Invited invited { get; set; }
        public Paging paging { get; set; }
    }
    public class Interested
    {
        public InterestedSummary summary { get; set; }

        public class InterestedSummary
        {
            public long count { get; set; }
        }
    }
    public class Invited
    {
        public InvitedSummary summary { get; set; }

        public class InvitedSummary
        {
            public long noreply_count { get; set; }
            public long maybe_count { get; set; }
            public long declined_count { get; set; }
            public long attending_count { get; set; }
            public long count { get; set; }
        }
    }

    #endregion

    #region groups
    [Serializable]
    public class GroupsResult
    {
        //private IList<Data> mdata;
        //public IList<Data> data
        //{
        //    get { return mdata; }
        //    set { mdata = value; }
        //}

        private ObservableCollection<GroupsResultData> mdata;
        public ObservableCollection<GroupsResultData> data
        {
            get { return mdata; }
            set { mdata = value; }
        }
        public GroupsResult()
        {
            data = new ObservableCollection<GroupsResultData>();
        }
    }
    public class GroupsResultData
    {
        //to url = https://www.facebook.com/groups/787206314630616

        public string description { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string privacy { get; set; }
        public Picture picture { get; set; }
        public Members members { get; set; }
        public Paging paging { get; set; }
    }
    public class Members
    {
        public Summary summary { get; set; }
    }
    #endregion

    #region pages
    [Serializable]
    public class PagesResult
    {
        //private IList<Data> mdata;
        //public IList<Data> data
        //{
        //    get { return mdata; }
        //    set { mdata = value; }
        //}
        //private List<Data> mdata;
        //public List<Data> data
        //{
        //    get { return mdata; }
        //    set { mdata = value; }
        //}
        private ObservableCollection<PagesResultData> mdata;
        public ObservableCollection<PagesResultData> data
        {
            get { return mdata; }
            set { mdata = value; }
        }
        public PagesResult()
        {
            data = new ObservableCollection<PagesResultData>();
        }
    }
    [Serializable]
    public class PagesResultData
    {
        public string about { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string link { get; set; }
        public string founded { get; set; }
        public string can_post { get; set; }
        public string category { get; set; }
        public long talking_about_count { get; set; }
        public long likes { get; set; }
        public Picture picture { get; set; }
        public Paging paging { get; set; }
    }
    #endregion

    #region media
    [Serializable]
    public class MediaResult
    {
        private ObservableCollection<MediaResultData> mdata;
        public ObservableCollection<MediaResultData> data
        {
            get { return mdata; }
            set { mdata = value; }
        }
        public MediaResult()
        {
            data = new ObservableCollection<MediaResultData>();
        }
    }
    [Serializable]
    public class MediaResultData
    {
        public string id { get; set; }
        public string link { get; set; }
        public string about { get; set; }
        public string source { get; set; }
        public string updated_time { get; set; }
        public bool is_video { get; set; }
        public long comment_count { get; set; }
        public long view_count { get; set; }
        public long like_count { get; set; }
    }
    #endregion

    [Serializable]
    public class Picture
    {
        public PicData data { get; set; }
    }

    [Serializable]
    public class PicData
    {
        public string url { get; set; }
    }

    [Serializable]
    public class Paging
    {
        public string next { get; set; }
    }
}
