using Browseo.SocialBirdEye.ViewModels;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Instagram;
using BrowseoFX_WPF.Core;
using Gecko;
using Gecko.Interfaces;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Organiser.Common.Classes.MyFilesDatabase;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks
{
    //https://www.youtube.com/account_advanced
    //YouTube User ID: W_zzxH-JsAOKGgrsgUMa8A
    //YouTube Channel ID: UCW_zzxH-JsAOKGgrsgUMa8A

    //https://developers.google.com/identity/protocols/OAuth2ServiceAccount
    //client id 288579351497-ohju479r3halvi17sj0go99jjqsvgpjc.apps.googleusercontent.com
    //Client secret	 OtvXS_hkuO5Juso9BIjYVii0
    //api key AIzaSyAnBqsOACojWLHvuHSdSdAIKqYmo-rol-Q
    //https://www.slickremix.com/docs/get-api-key-for-youtube/

    //YYYY-MM-DD format.

    #region responses
    public class GoogleResponse_AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
    }


    public class GoogleResponse_ChannelsList
    {
        public GoogleResponse_ChannelsListData[] items { get; set; }
    }

    public class GoogleResponse_ChannelsListData
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public GoogleResponse_Snippet snippet { get; set; }
        public GoogleResponse_statistics statistics { get; set; }
        public GoogleResponse_ContentDetails contentDetails { get; set; }

        public ObservableCollection<InfoItems2> Playlists { get; set; }
        public ObservableCollection<InfoItems2> Subscriptions { get; set; }
        public ObservableCollection<InfoItems2> Uploads { get; set; }

        public GoogleResponse_ChannelsListData()
        {
            Playlists = new ObservableCollection<InfoItems2>();
            Subscriptions = new ObservableCollection<InfoItems2>();
            Uploads = new ObservableCollection<InfoItems2>();
        }
    }

    public class GoogleResponse_Snippet
    {
        public string title { get; set; }
        public string description { get; set; }
        public string publishedAt { get; set; }
    }

    public class GoogleResponse_statistics
    {
        public string viewCount { get; set; }
        public string commentCount { get; set; }
        public string subscriberCount { get; set; }
        public string hiddenSubscriberCount { get; set; }
        public string videoCount { get; set; }
    }

    public class GoogleResponse_ContentDetails
    {
        public GoogleResponse_RelatedPlaylists relatedPlaylists { get; set; }
    }

    public class GoogleResponse_RelatedPlaylists
    {
        public string likes { get; set; }
        public string favorites { get; set; }
        public string uploads { get; set; }
        public string watchHistory { get; set; }
        public string watchLater { get; set; }
    }

    #endregion
    public class GoogleRequestsUrls
    {
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

    public class GoogleEye : ViewModelBase
    {
        public string SaveFile { get { return Path.Combine(BirdsEyeDashboardViewModel.SaveDirectory, "Google"); } }

        public ICommand OnCommandFromView { get; set; }

        private string clientID;
        public string ClientID
        {
            get { return clientID; }
            set { clientID = value; NotifyOfPropertyChange(); }
        }

        private string clientSecret;
        public string ClientSecret
        {
            get { return clientSecret; }
            set { clientSecret = value; NotifyOfPropertyChange(); }
        }

        private string redirectUrl;
        public string RedirectUrl
        {
            get { return redirectUrl; }
            set { redirectUrl = value; NotifyOfPropertyChange(); }
        }


        private ObservableCollection<PlusItems> plusActivities;
        public ObservableCollection<PlusItems> PlusActivities
        {
            get { return plusActivities; }
            set { plusActivities = value; }
        }
        private PlusItems selectedPlusActivitie;
        public PlusItems SelectedPlusActivitie
        {
            get { return selectedPlusActivitie; }
            set { selectedPlusActivitie = value; NotifyOfPropertyChange(); }
        }


        public ObservableCollection<GoogleResponse_ChannelsListData> YoutubesChannels { get; set; }
        private GoogleResponse_ChannelsListData selectedYoutubeChannel;
        public GoogleResponse_ChannelsListData SelectedYoutubeChannel
        {
            get { return selectedYoutubeChannel; }
            set { selectedYoutubeChannel = value; NotifyOfPropertyChange(); }
        }

        //access token class
        public GoogleResponse_AccessToken AccessTokenInfo { get; set; }
        
        //baisic Channel list info and stats
        public GoogleResponse_ChannelsList ChannelStatsNInfo { get; set; }

        private GoogleApisHttpRequests HttpRequests { get; set; }

        public GoogleEye()
        {
            HttpRequests = new GoogleApisHttpRequests(this);
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raisev);

            PlusActivities = new ObservableCollection<PlusItems>();
            YoutubesChannels = new ObservableCollection<GoogleResponse_ChannelsListData>();

            ReadTokenKeys();
        }

        private void OnCommandFromView_Raisev(object obj)
        {
            var param = obj as string;
            if (param == null) return;

            switch (param)
            {
                case "SaveFile":
                    SaveTokenKeys();
                    break;

                case "GetSelectedActivityComments":
                    GetSelectedActivityComments();
                    break;

                case "ListPlaylistsForChannel":
                    GetListPlaylistsForChannel();
                    break;

                case "YoutubeSubscriptions":
                    YoutubeSubscriptions();
                    break;

                case "ListUploads":
                    ListUploads();
                    break;

                default:
                    break;
            }
        }

        private void ReadTokenKeys()
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(BirdsEyeDashboardViewModel.SaveDirectory)) return;
                if (!File.Exists(SaveFile)) return;

                var fileContents = File.ReadAllLines(SaveFile);
                if (fileContents.Length < 3) return;

                ClientID = fileContents[0];
                ClientSecret = fileContents[1];
                RedirectUrl = fileContents[2];
            });
        }

        private void SaveTokenKeys()
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(BirdsEyeDashboardViewModel.SaveDirectory)) Directory.CreateDirectory(BirdsEyeDashboardViewModel.SaveDirectory);

                File.WriteAllLines(SaveFile, new string[] { ClientID, ClientSecret, RedirectUrl });
            });
        }


        public void OauthGoogleApis()
        {
            HttpRequests.GetOauthCode();
        }

        internal async void GooglePlusSummary()
        {
            //var activities = await HttpRequests.GetUrlRequest<GooglePlusActivitiesInfo>(GoogleRequestsUrls.Req_GPlusActivitiesList + "?maxResults=100&access_token=" + AccessTokenInfo.access_token);
            //if (activities != null && activities.items != null && activities.items.Length > 0)
            //{
            //    PlusActivities.Clear();
            //    foreach (var activity in activities.items)
            //    {
            //        PlusActivities.Add(activity);
            //    }

            //    if (SelectedPlusActivitie == null) SelectedPlusActivitie = PlusActivities[0];
            //}
        }

        private async void GetSelectedActivityComments()
        {
            if (SelectedPlusActivitie == null) return;

            //var url = GoogleRequestsUrls.Req_ActivitiesComments + SelectedPlusActivitie.id + "/comments?maxResults=100&access_token=" + AccessTokenInfo.access_token;
            //var comments = await HttpRequests.GetUrlRequest<GooglePlusActivitiesInfo>(url);
            //if (comments != null && comments.items != null && comments.items.Length > 0)
            //{
            //    SelectedPlusActivitie.Comments.Clear();
            //    foreach (var activity in comments.items)
            //    {
            //        SelectedPlusActivitie.Comments.Add(activity);
            //    }
            //}
        }


        internal async void YoutubeChannels()
        {
            var url = "https://www.googleapis.com/youtube/v3/channels?part=snippet%2CcontentDetails%2Cstatistics&mine=true&access_token=" + AccessTokenInfo.access_token;
            var comments = await HttpRequests.GetUrlRequest<GoogleResponse_ChannelsList>(url);
            if (comments != null && comments.items != null && comments.items.Length > 0)
            {
                YoutubesChannels.Clear();
                foreach (var activity in comments.items)
                {
                    YoutubesChannels.Add(activity);
                }
                if (YoutubesChannels.Count > 0) SelectedYoutubeChannel = YoutubesChannels[0];
            }
        }

        private async void GetListPlaylistsForChannel()
        {
            if (SelectedYoutubeChannel == null) return;

            var url = GoogleRequestsUrls.Req_ActitivtyPlaylists + "?part=snippet%2CcontentDetails&channelId=" + SelectedYoutubeChannel.id + "&maxResults=50&access_token=" + AccessTokenInfo.access_token;
            var comments = await HttpRequests.GetUrlRequest<InfoItems>(url);
            if (comments != null && comments.items != null && comments.items.Length > 0)
            {
                SelectedYoutubeChannel.Playlists.Clear();
                var playlistInfo = new InfoItems2();
                playlistInfo.pageInfo = comments.pageInfo;
                foreach (var activity in comments.items)
                {
                    playlistInfo.Items.Add(activity);
                }
                SelectedYoutubeChannel.Playlists.Add(playlistInfo);
            }
        }

        internal async void YoutubeSubscriptions()
        {
            if (SelectedYoutubeChannel == null) return;

            var url = GoogleRequestsUrls.Req_ActitivtySubscriptions + "?part=snippet%2CcontentDetails&channelId=" + SelectedYoutubeChannel.id + "&maxResults=50&access_token=" + AccessTokenInfo.access_token;
            var reply = await HttpRequests.GetUrlRequest<InfoItems>(url);
            if (reply != null && reply.items != null && reply.items.Length > 0)
            {
                SelectedYoutubeChannel.Subscriptions.Clear();
                var infoItem = new InfoItems2();
                infoItem.pageInfo = reply.pageInfo;
                foreach (var item in reply.items)
                {
                    infoItem.Items.Add(item);
                }
                SelectedYoutubeChannel.Subscriptions.Add(infoItem);
            }
        }

        private async void ListUploads()
        {
            if (SelectedYoutubeChannel == null) return;

            var url = GoogleRequestsUrls.Req_ListUploads + "?part=snippet%2CcontentDetails&playlistId=" + SelectedYoutubeChannel.contentDetails.relatedPlaylists.uploads + "&maxResults=50&access_token=" + AccessTokenInfo.access_token;
            var reply = await HttpRequests.GetUrlRequest<InfoItems>(url);
            if (reply != null && reply.items != null && reply.items.Length > 0)
            {
                SelectedYoutubeChannel.Uploads.Clear();
                var infoItem = new InfoItems2();
                infoItem.pageInfo = reply.pageInfo;
                foreach (var item in reply.items)
                {
                    infoItem.Items.Add(item);
                }
                SelectedYoutubeChannel.Uploads.Add(infoItem);
            }
        }
    }

    public class GoogleApisHttpRequests
    {
        public const string userScope = "https://www.googleapis.com/auth/youtube%20https://www.googleapis.com/auth/youtube.readonly%20https://www.googleapis.com/auth/yt-analytics-monetary.readonly%20https://www.googleapis.com/auth/yt-analytics.readonly%20https://www.googleapis.com/auth/yt-analytics.readonly"
            + "%20https://www.googleapis.com/auth/plus.login%20https://www.googleapis.com/auth/plus.me%20https://www.googleapis.com/auth/plus.circles.read%20https://www.googleapis.com/auth/plus.circles.write%20https://www.googleapis.com/auth/plus.profiles.read%20https://www.googleapis.com/auth/plus.stream.read%20https://www.googleapis.com/auth/plus.media.upload%20https://www.googleapis.com/auth/plus.stream.write";

        ////id: 891209702907-lum9f3a5o4lfcfugmbppdc1706ktisqh.apps.googleusercontent.com
        //public string ClientID { get; set; }

        ////secret: vJTm2TtKqRdCbafbUpOXQSSq
        //public string ClientSecret { get; set; }

        ////redirect: http://localhost:1234/
        //public string RedirectUrl { get; set; }

        public GoogleEye RequestsController { get; set; }

        public GoogleApisHttpRequests(GoogleEye controller)
        {
            RequestsController = controller;
            //TODO load and save keys
            //ClientID = "891209702907-i6c5ql0i0f2q9mk1e84ktddul1j8fi7b.apps.googleusercontent.com";
            //ClientSecret = "iSGozQDYCKNDl6wzmEqKwgR-";
            //RedirectUrl = "http://localhost:1234/";
        }

        internal void GetOauthCode()
        {
            var authUrl = GoogleRequestsUrls.strAuthentication + "?scope="+ userScope + "&redirect_uri=" + RequestsController.RedirectUrl + "&response_type=code&client_id=" + RequestsController.ClientID + "&approval_prompt=force&access_type=offline";



            BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            BrowseoFXManager.Instance.GloableWebView.Navigated += GloableWebView_Navigated;

            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(authUrl);
        }
        private async void GloableWebView_Navigated(object sender, GeckoNavigatedEventArgs e)
        {
            try
            {
                if (e.Url == null || !e.Url.Query.Contains("?code=")) return;
                BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;

                var code = e.Url.Query.Replace("?code=", "");

                string postData = "code=" + code + "&client_id=" + RequestsController.ClientID + "&client_secret=" + RequestsController.ClientSecret + "&redirect_uri=" + RequestsController.RedirectUrl + "&grant_type=authorization_code";
                string tokenJsonResponse = await Task<string>.Run(() =>
                {
                    return HttpRequestsBase.WebRequest(HttpRequestsBase.Method.POST, GoogleRequestsUrls.strRefreshToken, postData);
                });
                RequestsController.AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleResponse_AccessToken>(tokenJsonResponse);


                //string statsNInfoJson = await Task.Run(() =>
                //{
                //    return HttpRequestsBase.GetByWebRequest(GoogleRequestsUrls.Req_SnippetNStats + AccessTokenInfo.access_token);
                //});
                //ChannelStatsNInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleResponse_ChannelsList>(statsNInfoJson);

            }
            catch
            {
                BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            }
        }

        public async Task<T> GetUrlRequest<T>(string url)
        {
            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.HttpWebGetRequest(url);
            });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonResponse);
        }
    }
}
