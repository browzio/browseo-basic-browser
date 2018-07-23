using Browseo.SocialBirdEye.ViewModels;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Windows;
using Newtonsoft.Json.Linq;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers
{
    public class GoogleRequestsUrls
    {
        public const string UserScope = "https://www.googleapis.com/auth/contacts.readonly%20https://www.googleapis.com/auth/contacts%20https://www.googleapis.com/auth/youtubepartner-channel-audit%20https://www.googleapis.com/auth/youtubepartner%20https://www.googleapis.com/auth/youtube.upload%20https://www.googleapis.com/auth/youtube.force-ssl%20https://www.googleapis.com/auth/youtube%20https://www.googleapis.com/auth/youtube.readonly%20https://www.googleapis.com/auth/yt-analytics-monetary.readonly%20https://www.googleapis.com/auth/yt-analytics.readonly%20https://www.googleapis.com/auth/yt-analytics.readonly"
    + "%20https://www.googleapis.com/auth/plus.login%20https://www.googleapis.com/auth/plus.me%20https://www.googleapis.com/auth/plus.circles.read%20https://www.googleapis.com/auth/plus.circles.write%20https://www.googleapis.com/auth/plus.profiles.read%20https://www.googleapis.com/auth/plus.stream.read%20https://www.googleapis.com/auth/plus.media.upload%20https://www.googleapis.com/auth/plus.stream.write";

        #region default

        public const string strAuthentication = "https://accounts.google.com/o/oauth2/auth";
        public const string strRefreshToken = "https://accounts.google.com/o/oauth2/token";

        #endregion

        #region requests

        public const string Req_SnippetNStats = "https://www.googleapis.com/youtube/v3/channels?part=snippet%2Cstatistics&mine=true&access_token=";

        public const string Req_GPlusActivitiesList = "https://www.googleapis.com/plus/v1/people/me/activities/public";
        public const string Req_ActivitiesComments = "https://www.googleapis.com/plus/v1/activities/";
        public const string Req_ActitivtyPlaylists = "https://www.googleapis.com/youtube/v3/playlists";
        public const string Req_ActitivtySubscriptions = "https://www.googleapis.com/youtube/v3/subscriptions";
        public const string Req_ListUploads = "https://www.googleapis.com/youtube/v3/playlistItems";
        #endregion
    }

    public class GoogleApisController : ViewModelBase, IHaveApiKeys
    {
        public ICommand OnCommandFromView { get; set; }

        private ApiKeysBase clientKeys;
        public ApiKeysBase ClientKeys
        {
            get { return clientKeys; }
            set { clientKeys = value; NotifyOfPropertyChange(); }
        }

        private AccessTokenInfoBase accessTokenInfo;
        public AccessTokenInfoBase AccessTokenInfo
        {
            get { return accessTokenInfo; }
            set
            {
                accessTokenInfo = value;
                NotifyOfPropertyChange();
                RaisePropertyChanged("HasAccessToken");
            }
        }
        public bool HasAccessToken { get { return AccessTokenInfo != null; } }


        #region youtube
        private GoogleApisResponse channels;
        public GoogleApisResponse Channels
        {
            get { return channels; }
            set { channels = value; NotifyOfPropertyChange(); }
        }

        private GoogleApisResponse videos;
        public GoogleApisResponse Videos
        {
            get { return videos; }
            set { videos = value; NotifyOfPropertyChange(); }
        }

        //private GoogleApisResponse channelSections;
        //public GoogleApisResponse ChannelSections
        //{
        //    get { return channelSections; }
        //    set { channelSections = value; NotifyOfPropertyChange(); }
        //}

        //private GoogleApisResponse activities;
        //public GoogleApisResponse Activities
        //{
        //    get { return activities; }
        //    set { activities = value; NotifyOfPropertyChange(); }
        //}

        //private GoogleApisResponse playlists;
        //public GoogleApisResponse Playlists
        //{
        //    get { return playlists; }
        //    set { playlists = value; NotifyOfPropertyChange(); }
        //}

        //private GoogleApisResponse subscriptions;
        //public GoogleApisResponse Subscriptions
        //{
        //    get { return subscriptions; }
        //    set { subscriptions = value; NotifyOfPropertyChange(); }
        //}

        private GoogleApisResponse likedVideos;
        public GoogleApisResponse LikedVideos
        {
            get { return likedVideos; }
            set { likedVideos = value; NotifyOfPropertyChange(); }
        }

        private GoogleApisResponse mostPopularVideos;
        public GoogleApisResponse MostPopularVideos
        {
            get { return mostPopularVideos; }
            set { mostPopularVideos = value; NotifyOfPropertyChange(); }
        }
        #endregion

        #region google plus

        private GooglePlusApiResponse plusActivities;
        public GooglePlusApiResponse PlusActivities
        {
            get { return plusActivities; }
            set { plusActivities = value; NotifyOfPropertyChange(); }
        }
        #endregion

        public GoogleApisController()
        {
            ClientKeys = new GoogleApisClientKeys();

            FileAndFolderFactory.Instance.LoadKeys(this);

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            var param = obj as string;
            if (param == null) return;
            try
            {
                switch (param)
                {
                    #region top level
                    case "SetApiKeys":
                        SetApiKeys();
                        break;

                    case "OauthGoogleApis":
                        OauthGoogleApis();
                        break;

                    #region youtube
                    case "GetYoutubeChannels":
                        await GetYoutubeChannels();
                        break;

                    case "GetYoutubeLikedVideos":
                        await GetYoutubeLikedVideos();
                        break;

                    case "GetYoutubeMostPopularVideos":
                        await GetYoutubeMostPopularVideos();
                        break;

                    case "GetYoutubeVideos":
                        await GetYoutubeVideos();
                        break;
                    #endregion

                    #region google plus
                    case "GetGooglePlusActivities":
                        await GetGooglePlusActivities();
                        break;
                    #endregion

                    #endregion

                    default:
                        break;
                }
            }
            catch (System.Net.WebException e)
            {
                if(e.Message == "The remote server returned an error: (401) Unauthorized.")
                {
                    try
                    {
                        if (HasAccessToken)
                        {
                            await new AuthTokenActivator(this).GetRefreshToken();
                            OnCommandFromView_Raised(obj);
                            return;
                        }
                    }
                    catch
                    {
                        AccessTokenInfo = null;
                        OauthGoogleApis();
                    }
                }
            }
            catch (Exception ex)
            {
                var message = "Google Apis Error " + param + " threw an Exception: " + ex.Message;

            }
        }

        #region methods from from top level

        internal void SetApiKeys()
        {
            FileAndFolderFactory.Instance.OpenOrSaveApiKeys(this);
        }

        internal void OauthGoogleApis()
        {
            new AuthTokenActivator(this).SetOauthCode();
        }

        #region youtube
        internal async Task GetYoutubeChannels()
        {
            var url = "https://www.googleapis.com/youtube/v3/channels?part=snippet%2CcontentDetails%2Cstatistics&maxResults=50&mine=true&access_token=" + AccessTokenInfo.access_token;
            Channels = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);

            //comments channel
            await GetAndAddLoopedResponse(Channels, "https://www.googleapis.com/youtube/v3/commentThreads?part=snippet%2Creplies&maxResults=100&channelId=");
            //comments allThreadsRelatedToChannelId
            await GetAndAddLoopedResponse(Channels, "https://www.googleapis.com/youtube/v3/commentThreads?part=snippet%2Creplies&maxResults=100&allThreadsRelatedToChannelId=");
        }

        internal async Task GetYoutubeVideos()
        {
            var url = "https://www.googleapis.com/youtube/v3/search?part=snippet&forMine=true&maxResults=50&type=video&access_token=" + AccessTokenInfo.access_token;
            Videos = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);

            //stats
            await GetAndAddLoopedResponse(Videos, "https://www.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails%2Cstatistics&id=");
            //comments
            await GetAndAddLoopedResponse(Videos, "https://www.googleapis.com/youtube/v3/commentThreads?part=snippet%2Creplies&maxResults=100&videoId=");
        }

        //internal async void GetYoutubeChannelSections()
        //{
        //    var url = "https://www.googleapis.com/youtube/v3/channelSections?part=snippet%2CcontentDetails&mine=true&access_token=" + AccessTokenInfo.access_token;
        //    ChannelSections = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);
        //}

        //internal async void GetYoutubeActivities()
        //{
        //    var url = "https://www.googleapis.com/youtube/v3/activities?part=snippet%2CcontentDetails&maxResults=50&mine=true&access_token=" + AccessTokenInfo.access_token;
        //    Activities = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);
        //}

        //internal async void GetYoutubePlaylists()
        //{
        //    var url = "https://www.googleapis.com/youtube/v3/playlists?part=snippet%2CcontentDetails&maxResults=50&mine=true&access_token=" + AccessTokenInfo.access_token;
        //    Playlists = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);
        //}

        //internal async void GetYoutubeSubscriptions()
        //{
        //    var url = "https://www.googleapis.com/youtube/v3/subscriptions?part=snippet%2CcontentDetails&maxResults=50&mine=true&access_token=" + AccessTokenInfo.access_token;
        //    Subscriptions = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);
        //}

        internal async Task GetYoutubeLikedVideos()
        {
            var url = "https://www.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails%2Cstatistics&maxResults=50&myRating=like&access_token=" + AccessTokenInfo.access_token;
            LikedVideos = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);

            //comments 
            await GetAndAddLoopedResponse(LikedVideos, "https://www.googleapis.com/youtube/v3/commentThreads?part=snippet%2Creplies&maxResults=100&videoId=");
        }

        internal async Task GetYoutubeMostPopularVideos()
        {
            var url = "https://www.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails%2Cstatistics&chart=mostPopular&maxResults=50&access_token=" + AccessTokenInfo.access_token;
            MostPopularVideos = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);

            //comments 
            await GetAndAddLoopedResponse(MostPopularVideos, "https://www.googleapis.com/youtube/v3/commentThreads?part=snippet%2Creplies&maxResults=100&videoId=");
        }
        #endregion

        #region google plus
        private async Task GetGooglePlusActivities()
        {
            var url = "https://www.googleapis.com/plus/v1/people/me/activities/public?maxResults=100&access_token=" + AccessTokenInfo.access_token;
            PlusActivities = await HttpRequestsBase.GetUrlRequest<GooglePlusApiResponse>(url);
            if (PlusActivities != null && PlusActivities.items != null)
            {
                foreach (var item in plusActivities.items)
                {
                    url = "https://www.googleapis.com/plus/v1/activities/" + item.id + "/comments?maxResults=100&access_token=" + AccessTokenInfo.access_token;
                    var reply = await HttpRequestsBase.GetUrlRequest<GooglePlusApiResponse>(url);
                    if(reply!=null && reply.items != null)
                    {
                        foreach (var replyItem in reply.items)
                        {
                            item.Comments.Add(replyItem);
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

        #region from response items

        private async Task GetAndAddLoopedResponse(GoogleApisResponse response, string urlRequest)
        {
            if (response != null && response.items != null)
            {
                foreach (var responseItem in response.items)
                {
                    var id = "";
                    if (responseItem.id == null) continue;
                    if (responseItem.id is JObject)
                    {
                        var responseid = (responseItem.id as JObject).ToObject<id>();
                        if (responseid == null) continue;
                        id = responseid.videoId;
                    }
                    else if (responseItem.id is string)
                    {
                        id = responseItem.id as string;
                    }

                    var url = urlRequest + id + "&access_token=" + AccessTokenInfo.access_token;

                    var idResponse = await HttpRequestsBase.GetUrlRequest<GoogleApisResponse>(url);
                    if (idResponse == null || idResponse.items == null) continue;

                    foreach (var comment in idResponse.items)
                    {
                        responseItem.Comments.Add(comment);
                    }
                }
            }
        }

        #endregion
    }
}
