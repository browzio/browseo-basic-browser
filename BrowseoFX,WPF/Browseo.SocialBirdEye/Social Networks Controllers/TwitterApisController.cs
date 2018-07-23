using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Twitter;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Organiser.Common.Classes;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers
{
    public class TwitterApisController : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        private TwitterApisClientKeys clientKeys;
        public TwitterApisClientKeys ClientKeys
        {
            get { return clientKeys; }
            set
            {
                clientKeys = value;
                NotifyOfPropertyChange();
            }
        }

        private TwitterUserInfo userInfo;
        public TwitterUserInfo UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; NotifyOfPropertyChange(); }
        }

        public ObservableCollection<TweetTimelines> UserTimelines { get; set; }
        public ObservableCollection<TweetTimelines> HomeTimelines { get; set; }
        public ObservableCollection<TweetTimelines> MentionsTimelines { get; set; }

        public ObservableCollection<TweetTrend> TweetTrends { get; set; }

        TwitterEyeHttp eyeHttp;

        public TwitterApisController()
        {
            ClientKeys = new TwitterApisClientKeys();
            FileAndFolderFactory.Instance.LoadKeys(this);
            eyeHttp = new TwitterEyeHttp(this);

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            UserTimelines = new ObservableCollection<TweetTimelines>();
            HomeTimelines = new ObservableCollection<TweetTimelines>();
            MentionsTimelines = new ObservableCollection<TweetTimelines>();

            TweetTrends = new ObservableCollection<TweetTrend>();
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            var param = obj as string;
            if (param == null) return;
            try
            {
                switch (param)
                {
                    case "SetApiKeys":
                        SetApiKeys();
                        break;

                    case "AnalyzeAllData":
                        await AnalyzeAllData();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = "Twitter Apis Error " + param + " threw an Exception: " + ex.Message;

            }
        }

        private void SetApiKeys()
        {
            FileAndFolderFactory.Instance.OpenOrSaveApiKeys(this);
        }

        private async Task AnalyzeAllData()
        {
            UserInfo =
                await eyeHttp.GetInfo<TwitterUserInfo>("https://api.twitter.com/1.1/users/show.json", "screen_name=" + Uri.EscapeDataString(ClientKeys.ScreenName));

            var user_timeline = 
                await eyeHttp.GetInfo<TweetTimelines[]>("https://api.twitter.com/1.1/statuses/user_timeline.json", "screen_name=" + Uri.EscapeDataString(ClientKeys.ScreenName));
            if(user_timeline != null)
            {
                foreach (var item in user_timeline)
                {
                    UserTimelines.Add(item);
                }
            }

            var home_timeline =
                await eyeHttp.GetInfo2<TweetTimelines[]>("https://api.twitter.com/1.1/statuses/home_timeline.json");
            if (home_timeline != null)
            {
                HomeTimelines.Clear();
                foreach (var item in home_timeline)
                {
                    HomeTimelines.Add(item);
                }
            }

            var mentions_timeline = 
                await eyeHttp.GetInfo2<TweetTimelines[]>("https://api.twitter.com/1.1/statuses/mentions_timeline.json");
            if (mentions_timeline != null)
            {
                MentionsTimelines.Clear();
                foreach (var item in mentions_timeline)
                {
                    MentionsTimelines.Add(item);
                }
            }

            //maybe add https://api.twitter.com/1.1/trends/available.json
            //https://developer.twitter.com/en/docs/trends/locations-with-trending-topics/api-reference/get-trends-available
            var trends_RequestsParams = new SortedDictionary<string, string>();
            trends_RequestsParams.Add("id", "1");
            var trends_gloable =
               await eyeHttp.GetInfo2<TweetTrend[]>("https://api.twitter.com/1.1/trends/place.json", "GET", "id=1", trends_RequestsParams);
            if (trends_gloable != null)
            {
                TweetTrends.Clear();
                foreach (var item in trends_gloable)
                {
                    TweetTrends.Add(item);
                }
            }
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
        const string OauthVersion = "1.0";
        const string OauthSignatureMethod = "HMAC-SHA1";

        TwitterApisController sender;

        public TwitterEyeHttp(TwitterApisController sender)
        {
            this.sender = sender;
        }

        public async Task<T> GetInfo<T>(string resource_url, string postBody = "")
        {
            // unique request details
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();
            

            // create oauth signature
            string baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            string baseString = string.Format(baseFormat,
            sender.ClientKeys.OauthConsumerKey,
            oauth_nonce,
            OauthSignatureMethod,
            oauth_timestamp,
            sender.ClientKeys.OauthToken,
            OauthVersion,
            Uri.EscapeDataString(sender.ClientKeys.ScreenName)
            );

            baseString =
                string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            string compositeKey = string.Concat(Uri.EscapeDataString(sender.ClientKeys.OauthConsumerSecret),
            "&", Uri.EscapeDataString(sender.ClientKeys.OauthTokenSecret));

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
            Uri.EscapeDataString(OauthSignatureMethod),
            Uri.EscapeDataString(oauth_timestamp),
            Uri.EscapeDataString(sender.ClientKeys.OauthConsumerKey),
            Uri.EscapeDataString(sender.ClientKeys.OauthToken),
            Uri.EscapeDataString(oauth_signature),
            Uri.EscapeDataString(OauthVersion)
            );


            // make the request
            ServicePointManager.Expect100Continue = false;

            string requestUrl = resource_url;
            if (postBody != "") requestUrl = requestUrl + "?" + postBody;

            var json = await Task.Run(() => { return MakeHttpRequest(requestUrl, authHeader); });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        internal async Task<T> GetInfo2<T>(string url,
            string method = "GET",
            string postBody = "",
            SortedDictionary<string, string> requestParameters = null)
        {
            string oauthNonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));

            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            string oauthTimestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            string ConsumerKey = sender.ClientKeys.OauthConsumerKey;
            string AccessToken = sender.ClientKeys.OauthToken;

            // create oauth signature
            if (requestParameters == null) requestParameters = new SortedDictionary<string, string>();
            var oauthSignature = CreateOauthSignature(url, method, oauthNonce, oauthTimestamp, requestParameters);


            const string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                              "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                              "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                              "oauth_version=\"{6}\"";
            var authHeader = string.Format(headerFormat,
                                         Uri.EscapeDataString(oauthNonce),
                                         Uri.EscapeDataString(OauthSignatureMethod),
                                         Uri.EscapeDataString(oauthTimestamp),
                                         Uri.EscapeDataString(ConsumerKey),
                                         Uri.EscapeDataString(AccessToken),
                                         Uri.EscapeDataString(oauthSignature),
                                         Uri.EscapeDataString(OauthVersion)
              );

            string requestUrl = url;
            if (postBody != "") requestUrl = requestUrl + "?" + postBody;

            var json = await Task.Run(() => { return MakeHttpRequest(requestUrl, authHeader); });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public string CreateOauthSignature(string resourceUrl, string method, string oauthNonce, string oauthTimestamp,
                                    SortedDictionary<string, string> requestParameters)
        {
            //firstly we need to add the standard oauth parameters to the sorted list

            string oauthSignature = string.Empty;

            try
            {
                try
                {
                    requestParameters.Add("oauth_consumer_key", sender.ClientKeys.OauthConsumerKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                try
                {
                    requestParameters.Add("oauth_nonce", oauthNonce);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                try
                {
                    requestParameters.Add("oauth_signature_method", OauthSignatureMethod);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                try
                {
                    requestParameters.Add("oauth_timestamp", oauthTimestamp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                if (!string.IsNullOrEmpty(sender.ClientKeys.OauthToken))
                {
                    try
                    {
                        requestParameters.Add("oauth_token", sender.ClientKeys.OauthToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                try
                {
                    requestParameters.Add("oauth_version", OauthVersion);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                //try
                //{
                //    if (!string.IsNullOrEmpty(OAuthVerifer))

                //        try
                //        {
                //            requestParameters.Add("oauth_verifier", OAuthVerifer);
                //        }
                //        catch (Exception ex)
                //        {
                //            Console.WriteLine(ex.StackTrace);
                //        }
                //}

                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.StackTrace);

                //}


                var sigBaseString = requestParameters.ToWebString();

                var signatureBaseString = string.Concat(method.ToString(), "&", Uri.EscapeDataString(resourceUrl), "&",
                                                        Uri.EscapeDataString(sigBaseString.ToString()));


                //Using this base string, we then encrypt the data using a composite of the 
                //secret keys and the HMAC-SHA1 algorithm.
                var compositeKey = string.Concat(Uri.EscapeDataString(sender.ClientKeys.OauthConsumerSecret), "&",
                                                 Uri.EscapeDataString(sender.ClientKeys.OauthTokenSecret));



                using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
                {
                    oauthSignature = Convert.ToBase64String(
                        hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.StackTrace);
            }

            return oauthSignature;
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
    }
}
