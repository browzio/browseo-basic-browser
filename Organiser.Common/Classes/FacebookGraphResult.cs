using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes
{
    [Serializable]
    public class FacebookGraphData
    {
        public string about { get; set; }
        public string id { get; set; }
        public string link { get; set; }
        public string founded { get; set; }
        public bool can_post { get; set; }
        public string category { get; set; }
        public int likes { get; set; }

        public Videos videos { get; set; }
        public Photos photos { get; set; }
        public Posts posts { get; set; } 
    }

    public class Videos
    {
        public Video[] data { get; set; }
        public Paging paging { get; set; }
   

        public class Video
        {
            public string permalink_url { get; set; }
            public string picture { get; set; }
            public string id { get; set; }
            public double length { get; set; }
            public string embed_html { get; set; }
            public string source { get; set; }
            public string updated_time { get; set; }
            public string description { get; set; }
            public bool embeddable { get; set; }
            public Likes likes { get; set; }
            public SharedPosts sharedposts { get; set; }
        }

        public class SharedPosts
        {
            private ObservableCollection<SharedPost> mydata;  
            public ObservableCollection<SharedPost> data
            {
                get { return mydata; }
                set { mydata = value; }
            }

            public Paging paging { get; set; }

            public class SharedPost
            {
                public string story { get; set; }
                public string created_time { get; set; }
                public string id { get; set; }
            }
        }

        public class Comments
        {
            private ObservableCollection<Coment> mydata;
            public ObservableCollection<Coment> data
            {
                get { return mydata; }
                set { mydata = value; }
            }

            public Paging paging { get; set; }

            public class Coment
            {
                public string id { get; set; }
            }
        }
    }

    public class Photos
    {
        public Photo[] data { get; set; }
        public Paging paging { get; set; }

        public class Photo
        {
            public string created_time { get; set; }
            public string id { get; set; }

        }
    }

    public class Posts
    {
        private ObservableCollection<FacebookGraphPostResult> mydata;

        public ObservableCollection<FacebookGraphPostResult> data
        {
            get { return mydata; }
            set { mydata = value; }
        }

        //public FacebookGraphPostResult[] data
        //{
        //    get;
        //    set;
        //}
    }

    public class FacebookGraphPostResult : ViewModelBase
    {
        [JsonProperty("caption")]
        public string caption { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("icon")]
        public string icon { get; set; }

        [JsonProperty("link")]
        public string link { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("updated_time")]
        public string updated_time { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        public string full_picture { get; set; }
        public string picture { get; set; }

        public Shares shares { get; set; }

        public Likes likes { get; set; }
    }

    public class Shares
    {
        [JsonProperty("count")]
        public int count { get; set; }
    }

    public class Likes
    {
        private ObservableCollection<Data> mydata;

        public ObservableCollection<Data> data
        {
            get { return mydata; }
            set { mydata = value; }
        }

        public Paging paging { get; set; }

        public class Data
        {
            public string id { get; set; }
        }
    }

    public class LikesData
    {
        public Likes likes { get; set; }
    }

    public class Paging
    {
        [JsonProperty("next")]
        public string next { get; set; }
    }
}
