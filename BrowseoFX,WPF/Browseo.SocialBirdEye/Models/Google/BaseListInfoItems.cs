using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google
{
    public class InfoItems2
    {
        public PageInfo pageInfo { get; set; }

        public ObservableCollection<Items> Items { get; set; }

        public InfoItems2()
        {
            Items = new ObservableCollection<Items>();
        }
    }

    public class InfoItems
    {
        public PageInfo pageInfo { get; set; }
        public Items[] items { get; set; }
    }

    public class PageInfo
    {
        public string totalResults { get; set; }
        public string resultsPerPage { get; set; }
    }

    public class Items
    {
        public string id { get; set; }

        public Snippet snippet { get; set; }

        public ContentDetails contentDetails { get; set; }
    }

    public class Snippet
    {
        public string publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string channelTitle { get; set; }
        public string playlistId { get; set; }


        public Localized localized { get; set; }
        public ResourceId resourceId { get; set; }
    }

    public class Localized
    {
        public string title { get; set; }
        public string description { get; set; }

    }

    public class ContentDetails
    {
        public string itemCount { get; set; }
        public string totalItemCount { get; set; }
        public string newItemCount { get; set; }
        public string activityType { get; set; }
        public string videoId { get; set; }
        public string videoPublishedAt { get; set; }
    }

    public class ResourceId
    {
        public string kind { get; set; }
        public string videoId { get; set; }
    }

}
