using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Twitter;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using Newtonsoft.Json.Linq;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Security.Cryptography;
using Browseo.SocialBirdEye.ViewModels;
using System.Windows.Input;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks
{
    public static class Globals
    {

        #region Search API Methods
        /// <summary>
        /// Search Trends
        /// </summary>
        public static string TrendsUrl = "http://search.twitter.com/trends.json";
        public static string SearchUrl = "http://search.twitter.com/search.atom?q=";
        public static string SearchTwtUserUrl = "http://search.twitter.com/search.json?q=";
        #endregion

        #region Timeline Methods Urls
        /// <summary>
        /// TimeLine 
        /// </summary>
        public static string MentionUrl = "http://api.twitter.com/1/statuses/mentions.xml?count=";
        public static string HomeTimeLineUrl = "http://api.twitter.com/1/statuses/home_timeline.xml?count=";
        public static string UserTimeLineUrl = "http://api.twitter.com/1/statuses/user_timeline.xml?screen_name=";
        public static string RetweetedByMeUrl = "http://api.twitter.com/1.1/statuses/retweeted_by_me.xml?count=";
        // public static string MentionUrl = "http://api.twitter.com/1/statuses/mentions.xml?count=";

        #endregion

        #region Status Methods Urls
        /// <summary>
        /// User Status Demo
        /// </summary>
        public static string ShowStatusUrl = "http://api.twitter.com/1/statuses/show/";
        public static string UpdateStatusUrl = " http://api.twitter.com/1/statuses/update.xml";
        public static string ShowStatusUrlByScreenName = "http://twitter.com/users/show.xml?screen_name=";
        public static string ReTweetStatus = "http://api.twitter.com/1/statuses/retweet/";
        #endregion

        #region User Methods Urls
        /// <summary>
        /// User Status Demo
        /// </summary>
        public static string FriendStatusUrl = "http://api.twitter.com/1/statuses/friends/";
        public static string FollowerStatusUrl = "http://api.twitter.com/1/statuses/followers/";
        public static string GetAccountSettingsUrl = "https://api.twitter.com/1.1/account/settings.json ";
        public static string GetAccountVerifyCredentialsUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
        public static string PostAccountSettingsUrl = "https://api.twitter.com/1.1/account/settings.json";
        public static string PostAccountUpdateDeliveryDeviceUrl = "https://api.twitter.com/1.1/account/update_delivery_device.json";
        public static string PostAccountUpdateProfileUrl = "https://api.twitter.com/1.1/account/update_profile.json";
        public static string PostAccountUpdateProfileBackgroungImageUrl = "https://api.twitter.com/1.1/account/update_profile.json";
        public static string PostAccountUpdateProfileColorUrl = "https://api.twitter.com/1.1/account/update_profile_colors.json";
        public static string PostAccountUpdateProfileImageUrl = "https://api.twitter.com/1.1/account/update_profile_image.json";
        public static string GetBlocksListUrl = "https://api.twitter.com/1.1/blocks/list.json";
        public static string GetBlocksIdUrl = "https://api.twitter.com/1.1/blocks/ids.json";
        public static string PostBlockCreateUrl = "https://api.twitter.com/1.1/blocks/create.json";
        public static string PostBlocksDestroyUrl = "https://api.twitter.com/1.1/blocks/destroy.json";
        public static string GetUsersLookUpUrl = "https://api.twitter.com/1.1/users/lookup.json";
        public static string GetUsersShowUrl = "https://api.twitter.com/1.1/users/show.json";
        public static string GetUsersSearchUrl = "https://api.twitter.com/1.1/users/search.json";
        public static string GetUsersContributeesUrl = "https://api.twitter.com/1.1/users/contributees.json";
        public static string GetUsersContributorsUrl = "https://api.twitter.com/1.1/users/contributors.json";
        public static string PostAccountRemoveProfileBannerUrl = "https://api.twitter.com/1.1/account/remove_profile_banner.json";
        public static string PostAccountUpdateProfileBannerUrl = "https://api.twitter.com/1.1/account/update_profile_banner.json";
        public static string GetUsersProfileBannerUrl = "https://api.twitter.com/1.1/users/profile_banner.json";

        public static string PostStatusFavoritesById = "https://api.twitter.com/1.1/favorites/create.json";
        public static string PostUserReportAsSpammerById = "https://api.twitter.com/1.1/users/report_spam.json";
        public static string PostFriedshipDestroyById = "https://api.twitter.com/1.1/friendships/destroy.json";


        #endregion

        #region Direct Message Methods 
        /// <summary>
        /// Direct Message
        /// </summary>
        public static string DirectMessageGetByUserUrl = "http://api.twitter.com/1/direct_messages.xml?count=";
        public static string DirectMessageSentByUserUrl = "http://api.twitter.com/1/direct_messages/sent.xml?count=";
        public static string NewDirectMessage = "http://api.twitter.com/1/direct_messages/new.xml";
        public static string DeleteDirectMessage = "http://api.twitter.com/1/direct_messages/destroy/";
        public static string GetDirectMesagesUrl = "https://api.twitter.com/1.1/direct_messages.json";
        public static string GetDirectMessagesSentUrl = "https://api.twitter.com/1.1/direct_messages/sent.json";
        public static string GetDirectMessagesShowUrl = "https://api.twitter.com/1.1/direct_messages/show.json";
        public static string PostDirectMessagesDestroyUrl = "https://api.twitter.com/1.1/direct_messages/destroy.json";
        public static string PostDirectMesagesNewUrl = "https://api.twitter.com/1.1/direct_messages/new.json";
        #endregion

        #region Account Methods
        /// <summary>
        /// Account 
        /// </summary>
        // public static string VerifyCredentialsUrl = "http://api.twitter.com/1/account/verify_credentials.xml";
        public static string VerifyCredentialsUrl = "https://api.twitter.com/1.1/account/verify_credentials.json";
        public static string RateLimitStatusUrl = "http://api.twitter.com/1/account/rate_limit_status.xml";
        #endregion

        #region Friendship Methods
        /// <summary>
        /// Followers
        /// </summary>
        public static string FollowerUrl = "http://api.twitter.com/1/friendships/create.xml?screen_name=";
        public static string UnFollowUrl = "http://api.twitter.com/1/friendships/destroy.xml?screen_name=";
        public static string FollowerUrlById = "http://api.twitter.com/1/friendships/create.xml?user_id=";
        public static string UnFollowUrlById = "http://api.twitter.com/1/friendships/destroy.xml?user_id=";
        public static string GetFriendshipsNoRetweetsIdUrl = "https://api.twitter.com/1.1/friendships/no_retweets/ids.json";
        public static string GetFriendsIdUrl = "https://api.twitter.com/1.1/friends/ids.json";
        public static string GetFollowersIdUrl = "https://api.twitter.com/1.1/followers/ids.json";
        public static string GetFriendshipsLookUpUrl = "https://api.twitter.com/1.1/friendships/lookup.json";
        public static string GetFriendshipsIncomingUrl = "https://api.twitter.com/1.1/friendships/incoming.json";
        public static string GetFrienshipsOutgoingUrl = "https://api.twitter.com/1.1/friendships/outgoing.format";
        public static string PostFriendshipsCreateUrl = "https://api.twitter.com/1.1/friendships/create.json";
        public static string PostFriendshipsDestroyUrl = "https://api.twitter.com/1.1/friendships/destroy.json";
        public static string PostFrienshipsUpdateUrl = "https://api.twitter.com/1.1/friendships/update.json";
        public static string GetFriendshipsShowUrl = "https://api.twitter.com/1.1/friendships/show.json";
        public static string GetFriendsListUrl = "https://api.twitter.com/1.1/friends/list.json";
        public static string GetFollowersListUrl = "https://api.twitter.com/1.1/followers/list.json";
        #endregion

        #region Social Graph Methods
        /// <summary>
        /// Social Graph Methods
        /// </summary>
        public static string FriendsIdUrl = "http://api.twitter.com/1/friends/ids.xml?screen_name=";
        public static string FollowersIdUrl = "http://api.twitter.com/1/followers/ids.xml?screen_name=";
        #endregion

        #region Tweets Method
        public static string StatusesRetweetByIdUrl = "https://api.twitter.com/1.1/statuses/retweets/";
        public static string StatusShowByIdUrl = "https://api.twitter.com/1.1/statuses/show.json?id=";
        public static string StatusDestroyByIdUrl = "https://api.twitter.com/1.1/statuses/destroy/";
        public static string StatusUpdateUrl = "https://api.twitter.com/1.1/statuses/update.json";
        public static string PostStatusesRetweetByIdUrl = "https://api.twitter.com/1.1/statuses/retweet/";
        public static string PostStatusUpdateWithMediaUrl = "https://api.twitter.com/1.1/statuses/update_with_media.json";
        // public static string PostStatusUpdateWithMediaUrl = "https://upload.twitter.com/1/statuses/update_with_media.xml";
        public static string GetStatusesRetweetersByIdUrl = "https://api.twitter.com/1.1/statuses/retweeters/ids.json";
        #endregion

        #region Timeline Methods of version 1.1
        public static string statusesMentionTimelineUrl = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";
        public static string statusesUserTimelineUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";
        public static string statusesHomeTimelineUrl = "https://api.twitter.com/1.1/statuses/home_timeline.json";
        public static string statusesRetweetsOfMeUrl = "https://api.twitter.com/1.1/statuses/retweets_of_me.json";

        #endregion





        #region Search Method of version 1.1
        public static string GetSearchTweetsUrl = "https://api.twitter.com/1.1/search/tweets.json";
        public static string GetUserSuggestions = "https://api.twitter.com/1.1/users/suggestions.json";
        public static string GetUsersSuggestionsSlug = "";
        #endregion
        //private static int _RequestCount;
        //public static int RequestCount
        //{
        //    set
        //    {
        //        _RequestCount = value;
        //    }
        //    get
        //    {
        //        return _RequestCount;
        //    }
        //}

        public static string UploadMedia = "http://upload.twitter.com/1/statuses/update_with_media.xml";


    }

    //Consumer Key (API Key)	VkO69yucdjKYPvYKbTT3GgGUV
    //Consumer Secret (API Secret)	M9LKIxHFxaeLQ16audZbcyZtTbYy1dbocxymVkSEJPC9qHSKvz
    //Owner	eldods
    //Owner ID	564901421
    //Access Token	564901421-Z8eTOjHfxoFJV4fteFw6q0lvU2RS5qTqQJjvo7bM
    //Access Token Secret	FQeAyV5YsuCaMC9qnz33Fn869uGTPYKa2DOlponUZZMTG

    // Generating access tokens
    //Login to the apps.twitter.com interface using your Twitter credentials
    //Create an app or open an existing app that you would like to create access tokens for
    //Navigate to the 'Keys and Access Tokens' page
    //Scroll down and click on the 'Create my access token' button
    //Take note of your access token as you will use in to access certain API endpoints

    public class TwitterEye : ViewModelBase
    {
        public string SaveFile { get { return Path.Combine(BirdsEyeDashboardViewModel.SaveDirectory, "Twitter"); } }

        public ICommand OnCommandFromView { get; set; }

        private TwitterUserInfo twitterUserInfo;
        public TwitterUserInfo TwitterUserInfo
        {
            get { return twitterUserInfo; }
            set { twitterUserInfo = value; NotifyOfPropertyChange(); }
        }


        private string oauthToken;
        public string OauthToken
        {
            get { return oauthToken; }
            set { oauthToken = value; NotifyOfPropertyChange(); }
        }

        private string òauthTokenSecret;
        public string OauthTokenSecret
        {
            get { return òauthTokenSecret; }
            set { òauthTokenSecret = value; NotifyOfPropertyChange(); }
        }

        private string oauthConsumerKey;
        public string OauthConsumerKey
        {
            get { return oauthConsumerKey; }
            set { oauthConsumerKey = value; NotifyOfPropertyChange(); }
        }

        private string oauthConsumerSecret;
        public string OauthConsumerSecret
        {
            get { return oauthConsumerSecret; }
            set { oauthConsumerSecret = value; NotifyOfPropertyChange(); }
        }

        private string screenName;
        public string ScreenName
        {
            get { return screenName; }
            set { screenName = value; NotifyOfPropertyChange(); }
        }

        public TwitterEye()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raisev);
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

                default:
                    break;
            }
        }

        private void ReadTokenKeys()
        {
            Task.Run(()=> 
            {
                if (!Directory.Exists(BirdsEyeDashboardViewModel.SaveDirectory)) return;
                if (!File.Exists(SaveFile)) return;

                var fileContents = File.ReadAllLines(SaveFile);
                if (fileContents.Length < 5) return;

                OauthToken = fileContents[0];
                OauthTokenSecret = fileContents[1];
                OauthConsumerKey = fileContents[2];
                OauthConsumerSecret = fileContents[3];
                ScreenName = fileContents[4];
            });
        }

        private void SaveTokenKeys()
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(BirdsEyeDashboardViewModel.SaveDirectory)) Directory.CreateDirectory(BirdsEyeDashboardViewModel.SaveDirectory);

               File.WriteAllLines(SaveFile, new string[] { OauthToken, OauthTokenSecret , OauthConsumerKey, OauthConsumerSecret, ScreenName });
            });
        }

        public async Task<TwitterUserInfo> LoginAndGetUserInfo()
        {
            return await Task.Run(()=> 
            {
                var json = TwitterEyeHttp.GetUserInfo(this);
                TwitterUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<TwitterUserInfo>(json);
                return TwitterUserInfo;
            });
        }
    }

    public class TwitterEyeHttp
    {
        // oauth application keys
        //public static string oauth_token = "564901421-Z8eTOjHfxoFJV4fteFw6q0lvU2RS5qTqQJjvo7bM";
        //public static string oauth_token_secret = "FQeAyV5YsuCaMC9qnz33Fn869uGTPYKa2DOlponUZZMTG";

        //public static string oauth_consumer_key = "VkO69yucdjKYPvYKbTT3GgGUV";
        //public static string oauth_consumer_secret = "M9LKIxHFxaeLQ16audZbcyZtTbYy1dbocxymVkSEJPC9qHSKvz";

        //username
        //public static string screen_name = "eldods";

        // oauth implementation details
        const string oauth_version = "1.0";
        const string oauth_signature_method = "HMAC-SHA1";

        public static string GetUserInfo(TwitterEye sender)
        {
            // unique request details
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // message api details
            var resource_url = "https://api.twitter.com/1.1/users/show.json";

            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            string baseString = string.Format(baseFormat,
            sender.OauthConsumerKey,
            oauth_nonce,
            oauth_signature_method,
            oauth_timestamp,
            sender.OauthToken,
            oauth_version,
            Uri.EscapeDataString(sender.ScreenName)
            );

            baseString =
                string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            string compositeKey = string.Concat(Uri.EscapeDataString(sender.OauthConsumerSecret),
            "&", Uri.EscapeDataString(sender.OauthTokenSecret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
            "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
            "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
            "oauth_version=\"{6}\"";

            string authHeader = string.Format(headerFormat,
            Uri.EscapeDataString(oauth_nonce),
            Uri.EscapeDataString(oauth_signature_method),
            Uri.EscapeDataString(oauth_timestamp),
            Uri.EscapeDataString(sender.OauthConsumerKey),
            Uri.EscapeDataString(sender.OauthToken),
            Uri.EscapeDataString(oauth_signature),
            Uri.EscapeDataString(oauth_version)
            );


            // make the request
            ServicePointManager.Expect100Continue = false;

            string postBody = "screen_name=" + Uri.EscapeDataString(sender.ScreenName);
            resource_url += "?" + postBody;

            return MakeHttpRequest(resource_url, authHeader);
        }

        private static string MakeHttpRequest(string resource_url, string authHeader)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Proxy = MyFilesDatabase.GetRequestsProxy();

            WebResponse response = request.GetResponse();
            string responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
            response.Close();

            return responseData;
        }

        ////
        //public static string MakePostRequest(SortedDictionary<string, string> requestParameters)
        //{
        //    var resourceUrl = "/1.1/statuses/update.json?include_entities=true HTTP/1.1";
        //    var resultString = "";

        //    var request = (HttpWebRequest)WebRequest.Create(resourceUrl);
        //    request.Method = "POST";
        //    request.Proxy = MyFilesDatabase.GetRequestsProxy();
        //    request.Accept = "*/*";
        //    request.Connection = "close";
        //    request.UserAgent = "OAuth gem v0.4.4";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ProtocolVersion = HttpVersion.Version11;
        //    request.Host = "api.twitter.com";

        //    using (var stream = request.GetRequestStream())
        //    {
        //        byte[] content = Encoding.ASCII.GetBytes(requestParameters.ToWebString());
        //        stream.Write(content, 0, content.Length);
        //    }


        //    //var authHeader = CreateHeader(resourceUrl, "POST", requestParameters);
        //    //request.Headers.Add("Authorization", authHeader);
        //    //var response = request.GetResponse();

        //    //using (var sd = new StreamReader(response.GetResponseStream()))
        //    //{
        //    //    resultString = sd.ReadToEnd();
        //    //    response.Close();
        //    //}

        //    return resultString;
        //}

        //public static string CreateNonce()
        //{
        //    //Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));

        //    Guid g = Guid.NewGuid();
        //    string GuidString = Convert.ToBase64String(g.ToByteArray());
        //    GuidString = GuidString.Replace("=", "");
        //    GuidString = GuidString.Replace("+", "");
        //    return GuidString;
        //}

        //public static string CreateOAuthTimestamp()
        //{

        //    var nowUtc = DateTime.UtcNow;
        //    var timeSpan = nowUtc - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //    var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

        //    return timestamp;
        //}

        //public static string CreateOauthSignature(SortedDictionary<string, string> requestParameters,string method,string resourceUrl, string ConsumerKeySecret, string AccessTokenSecret)
        //{
        //    //firstly we need to add the standard oauth parameters to the sorted list

        //    string oauthSignature = string.Empty;

        //    try
        //    {
        //        var sigBaseString = requestParameters.ToWebString();

        //        var signatureBaseString = string.Concat(method.ToString(), "&", Uri.EscapeDataString(resourceUrl), "&",
        //                                                Uri.EscapeDataString(sigBaseString.ToString()));


        //        //Using this base string, we then encrypt the data using a composite of the 
        //        //secret keys and the HMAC-SHA1 algorithm.
        //        var compositeKey = string.Concat(Uri.EscapeDataString(ConsumerKeySecret), "&",
        //                                         Uri.EscapeDataString(AccessTokenSecret));

        //        using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
        //        {
        //            oauthSignature = Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error : " + ex.StackTrace);
        //    }

        //    return oauthSignature;
        //}


        //public string CreateHeader(string resourceUrl, string method,
        //                            SortedDictionary<string, string> requestParameters)
        //{
        //    var oauthNonce = CreateNonce();
        //    // Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        //    var oauthTimestamp = CreateOAuthTimestamp();
        //    var oauthSignature = CreateOauthSignature(resourceUrl, method, oauthNonce, oauthTimestamp, requestParameters);

        //    //The oAuth signature is then used to generate the Authentication header. 
        //        const string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
        //                                     "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
        //                                     "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
        //                                     "oauth_version=\"{6}\"";
        //        var authHeader = string.Format(headerFormat,
        //                                     Uri.EscapeDataString(oauthNonce),
        //                                     Uri.EscapeDataString(OauthSignatureMethod),
        //                                     Uri.EscapeDataString(oauthTimestamp),
        //                                     Uri.EscapeDataString(ConsumerKey),
        //                                     Uri.EscapeDataString(AccessToken),
        //                                     Uri.EscapeDataString(oauthSignature),
        //                                     Uri.EscapeDataString(OauthVersion)
        //          );
        //        return authHeader;

        //}
    }
}
