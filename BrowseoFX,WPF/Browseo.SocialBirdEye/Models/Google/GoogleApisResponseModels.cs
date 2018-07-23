using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google
{
    #region youtube
    public class GoogleApisResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }

        public pageInfo pageInfo { get; set; }
        public GoogleApisResponseItemsData[] items { get; set; }
    }
    
    public class pageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }
    
    public class GoogleApisResponseItemsData
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public object id { get; set; }
        public bool IsChecked { get; set; }

        public snippet snippet { get; set; }
        public contentDetails contentDetails { get; set; }
        public statistics statistics { get; set; }

        public ObservableCollection<GoogleApisResponseItemsData> Comments { get; set; }

        public GoogleApisResponseItemsData()
        {
            Comments = new ObservableCollection<GoogleApisResponseItemsData>();
        }
    }

    #region snippet

    public class snippet
    {
        //channels.list
        public string title { get; set; }
        public string description { get; set; }
        public string publishedAt { get; set; }
        //channelSections.list
        public string type { get; set; }
        public string style { get; set; }
        public string channelId { get; set; }
        public int position { get; set; }
        //commentThreads.list
        public string videoId { get; set; }
        public string canReply { get; set; }
        public int totalReplyCount { get; set; }
        public string isPublic { get; set; }
        public string authorDisplayName { get; set; }
        public string authorProfileImageUrl { get; set; }
        public string authorChannelUrl { get; set; }
        public string textDisplay { get; set; }
        public string textOriginal { get; set; }
        public string canRate { get; set; }
        public string viewerRating { get; set; }
        public int likeCount { get; set; }
        public string updatedAt { get; set; }
        //playlistItems.list
        public string channelTitle { get; set; }
        public string playlistId { get; set; }
        //videos.list
        public string categoryId { get; set; }
        public string liveBroadcastContent { get; set; }
        public string defaultLanguage { get; set; }
        public string defaultAudioLanguage { get; set; }
        public string[] tags { get; set; }

        public thumbnails thumbnails { get; set; }
        public localized localized { get; set; }
    
        public topLevelComment topLevelComment { get; set; }
        public authorChannelId authorChannelId { get; set; }

        public resourceId resourceId { get; set; }
    }
    
    public class thumbnails
    {
        [JsonProperty("default")]
        public thumbnailsData Default { get; set; }
        public thumbnailsData medium { get; set; }
        public thumbnailsData high { get; set; }
    }
    public class thumbnailsData
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    
    public class localized
    {
        public string title { get; set; }
        public string description { get; set; }
    }

    public class topLevelComment
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }

        public snippet snippet { get; set; }
    }

    public class authorChannelId
    {
        public string value { get; set; }
    }

    public class resourceId
    {
        public string kind { get; set; }
        public string videoId { get; set; }
        public string channelId { get; set; }
    }
    #endregion

    #region contentDetails
    public class contentDetails
    {
        public string videoId { get; set; }
        public string videoPublishedAt { get; set; }
        public int itemCount { get; set; }
        public int totalItemCount { get; set; }
        public int newItemCount { get; set; }
        public string activityType { get; set; }
        //videos.list
        public string duration { get; set; }
        public string dimension { get; set; }
        public string definition { get; set; }
        public string caption { get; set; }
        public string licensedContent { get; set; }
        public string projection { get; set; }

        public relatedPlaylists relatedPlaylists { get; set; }
    }
    public class relatedPlaylists
    {
        public string likes { get; set; }
        public string favorites { get; set; }
        public string uploads { get; set; }
        public string watchHistory { get; set; }
        public string watchLater { get; set; }
    }
    #endregion

    #region statistics
    
    public class statistics
    {
        public string viewCount { get; set; }
        public string commentCount { get; set; }
        public string subscriberCount { get; set; }
        public string hiddenSubscriberCount { get; set; }
        public string videoCount { get; set; }
        public string likeCount { get; set; }
        public string dislikeCount { get; set; }
        public string favoriteCount { get; set; }
    }

    #endregion

    public class id
    {
        public string kind { get; set; }
        public string videoId { get; set; }
        public string playlistId { get; set; }
    }
    #endregion

    #region google plus
    public class GooglePlusApiResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public string title { get; set; }
        public string updated { get; set; }

        public PlusItems[] items { get; set; }
    }

    public class PlusItems
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string title { get; set; }
        public string published { get; set; }
        public string updated { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string verb { get; set; }
        public string selfLink { get; set; }

        public PlusActor actor { get; set; }
        public PlusProvider provider { get; set; }

        [JsonProperty("object")]
        public PlusObject pObject { get; set; }

        public ObservableCollection<PlusItems> Comments { get; set; }
        public PlusTotals plusoners { get; set; }

        public PlusItems()
        {
            Comments = new ObservableCollection<PlusItems>();
        }
    }

    public class PlusActor
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string url { get; set; }
        public PlusImage image { get; set; }
    }

    public class PlusImage
    {
        public string url { get; set; }
    }

    public class PlusObject
    {
        public string id { get; set; }
        public string objectType { get; set; }
        public string content { get; set; }
        public string url { get; set; }

        public PlusActor actor { get; set; }

        public PlusTotals replies { get; set; }
        public PlusTotals plusoners { get; set; }
        public PlusTotals resharers { get; set; }
    }

    public class PlusTotals
    {
        public string totalItems { get; set; }
        public string selfLink { get; set; }
    }

    public class PlusProvider
    {
        public string title { get; set; }
    }
    #endregion
}
