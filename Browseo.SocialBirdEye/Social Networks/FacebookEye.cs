using Browseo.SocialBirdEye.Helpers;
using Browseo.SocialBirdEye.Models;
using Browseo.SocialBirdEye.ViewModels;
using Facebook;
using Newtonsoft.Json.Linq;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.SocialBirdEye.Social_Networks
{
    public static class Authentication
    {
        public static string getAccessToken(string client_id, string redirect_uri, string client_secret, string code)
        {
            FacebookClient fb = new FacebookClient();
            string profileId = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("client_id", client_id);
            parameters.Add("redirect_uri", redirect_uri);
            parameters.Add("client_secret", client_secret);
            parameters.Add("code", code);
            JsonObject fbaccess_token = null;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;
            fbaccess_token = (JsonObject)fb.Get("/oauth/access_token", parameters);
            return fbaccess_token["access_token"].ToString();
        }

        public static string GetFacebookRedirectLink(string FacebookAuthUrl, string ClientId, string RedirectUrl)
        {
            return FacebookAuthUrl + "&client_id=" + ClientId + "&redirect_uri=" + RedirectUrl + "&response_type=code";
        }
    }

    public class Fbpages
    {
        public static List<Facebookpage> Getfacebookpages(string accesstoken)
        {
            List<Facebookpage> lstpages = new List<Facebookpage>();
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accesstoken;
            dynamic profile = fb.Get("v2.7/me");//v2.1
            dynamic output = fb.Get("v2.7/me/accounts");//v2.1
            foreach (var item in output["data"])
            {
                try
                {
                    Facebookpage objAddFacebookPage = new Facebookpage();
                    objAddFacebookPage.ProfilePageId = item["id"].ToString();
                    try
                    {
                        dynamic postlike = fb.Get("v2.7/" + item["id"] + "?fields=likes,name,username,fan_count");
                        objAddFacebookPage.LikeCount = postlike["fan_count"].ToString();
                    }
                    catch (Exception ex)
                    {
                        objAddFacebookPage.LikeCount = "0";

                    }
                    objAddFacebookPage.Name = item["name"].ToString();
                    objAddFacebookPage.AccessToken = item["access_token"].ToString();
                    try
                    {
                        objAddFacebookPage.Email = profile["email"].ToString();
                    }
                    catch (Exception ex)
                    {
                        objAddFacebookPage.Email = "";
                    }
                    lstpages.Add(objAddFacebookPage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return lstpages;
        }


        public static object getFbPageData(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;

            try
            {
                return fb.Get("v2.7/me?fields=id,name,username,likes,fan_count,cover,emails");
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }
        public static string getFbPageData(string accessToken, string PageId)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                dynamic profile = fb.Get("v2.7/" + PageId + "?fields=id,name");
                return profile["name"].ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static List<FacebookFanAddsViewModel> GetFacebookFanAdds(string profileId, double Since, double Until)
        {
            List<FacebookFanAddsViewModel> FbFansList = new List<FacebookFanAddsViewModel>();

            return FbFansList;
        }
        //public static dynamic subscribed_apps(string accessToken)
        //{
        //    FacebookClient fb = new FacebookClient();
        //    fb.AccessToken = accessToken;
        //    try
        //    {
        //        return fb.Post("v2.7/me/subscribed_apps");//v2.6
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Invalid Access Token";
        //    }
        //}
        public static string getFacebookRecentPost(string fbAccesstoken, string pageId)
        {
            string output = string.Empty;
            string facebookSearchUrl = "https://graph.facebook.com/v1.0/" + pageId + "/posts?limit=30&access_token=" + fbAccesstoken;
            var facebooklistpagerequest = (HttpWebRequest)WebRequest.Create(facebookSearchUrl);
            facebooklistpagerequest.Method = "GET";
            facebooklistpagerequest.Proxy = MyFilesDatabase.GetRequestsProxy();
            facebooklistpagerequest.Credentials = CredentialCache.DefaultCredentials;
            facebooklistpagerequest.AllowWriteStreamBuffering = true;
            facebooklistpagerequest.ServicePoint.Expect100Continue = false;
            facebooklistpagerequest.PreAuthenticate = false;
            try
            {
                using (var response = facebooklistpagerequest.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                    {
                        output = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return output;
        }

        public static string getFacebookPageRecentPost(string fbAccesstoken, string pageId, string curser_next)
        {
            string output = string.Empty;
            string facebookSearchUrl = string.Empty;
            if (string.IsNullOrEmpty(curser_next))
            {
                facebookSearchUrl = "https://graph.facebook.com/v1.0/" + pageId + "/posts?limit=30&access_token=" + fbAccesstoken;
            }
            else
            {
                facebookSearchUrl = curser_next;
            }
            var facebooklistpagerequest = (HttpWebRequest)WebRequest.Create(facebookSearchUrl);
            facebooklistpagerequest.Method = "GET";
            facebooklistpagerequest.Proxy = MyFilesDatabase.GetRequestsProxy();
            facebooklistpagerequest.Credentials = CredentialCache.DefaultCredentials;
            facebooklistpagerequest.AllowWriteStreamBuffering = true;
            facebooklistpagerequest.ServicePoint.Expect100Continue = false;
            facebooklistpagerequest.PreAuthenticate = false;
            try
            {
                using (var response = facebooklistpagerequest.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                    {
                        output = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return output;
        }
        public static string subscribed_apps(string fbAccesstoken, string pageId)
        {
            string output = string.Empty;
            string facebookSearchUrl = "https://graph.facebook.com/v2.7/" + pageId + "/subscribed_apps?access_token=" + fbAccesstoken;
            var facebooklistpagerequest = (HttpWebRequest)WebRequest.Create(facebookSearchUrl);
            facebooklistpagerequest.Method = "POST";
            facebooklistpagerequest.Proxy = MyFilesDatabase.GetRequestsProxy();
            facebooklistpagerequest.Credentials = CredentialCache.DefaultCredentials;
            facebooklistpagerequest.AllowWriteStreamBuffering = true;
            facebooklistpagerequest.ServicePoint.Expect100Continue = false;
            facebooklistpagerequest.PreAuthenticate = false;
            try
            {
                using (var response = facebooklistpagerequest.GetResponse())
                {
                    using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                    {
                        output = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {

            }
            return output;
        }

        public static string schedulePage_Post(string accessToken, string link, string scheduled_publish_time)
        {
            FacebookClient fb = new FacebookClient();
            var args = new Dictionary<string, object>();
            args["link"] = link;
            args["scheduled_publish_time"] = scheduled_publish_time;
            args["published"] = "false";
            fb.AccessToken = accessToken;
            try
            {
                return fb.Post("v2.8/me/feed", args).ToString();//v2.6
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }

    public static class FbUser
    {
        public static object getFbUser(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/me?fields=id,about,bio,birthday,cover,education,email,gender,hometown,name,first_name,last_name,work,picture");//v2.6
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static Int64 getFbFriends(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            dynamic friends = fb.Get("v2.7/me/friends");//v2.1
            try
            {
                return Convert.ToInt64(friends["summary"]["total_count"].ToString());
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static dynamic getFeeds(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/me/feed?limit=99&fields=picture,created_time,message,description,story,from,likes.summary(true),comments.summary(true),type,application");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }


        public static dynamic getFeeds(string accessToken, string facebookid)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/" + facebookid + "/posts?limit=99");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic getFeedDetail(string accessToken, string PostId)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/" + PostId + "?fields=likes.summary(true),comments.summary(true),shares");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }
        public static dynamic postdetails(string accessToken, string PostId)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/" + PostId + "/insights");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic conversations(string accessToken)
        {

            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/me/conversations");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic notifications(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/me/notifications");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic getPostComments(string accessToken, string postid)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/" + postid + "/comments?limit=99");//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }


        public static string postComments(string accessToken, string postid, string message)
        {
            var args = new Dictionary<string, object>();
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            args["message"] = message;
            try
            {
                return fb.Post("v2.7/" + postid + "/comments", args).ToString();//v2.1
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic fbGet(string accessToken, string Url)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get(Url);
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic getPageTaggedPostDetails(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/me/tagged?fields=picture,created_time,message,description,from&limit=99");//v2.6
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }

        public static dynamic getPromotablePostsDetails(string accessToken)
        {
            FacebookClient fb = new FacebookClient();
            fb.AccessToken = accessToken;
            try
            {
                return fb.Get("v2.7/me/promotable_posts?fields=picture,created_time,message,description,from&limit=99");//v2.6
            }
            catch (Exception ex)
            {
                return "Invalid Access Token";
            }
        }





        public static string SetPrivacy(string privacy, FacebookClient fb, string fbUserId)
        {
            try
            {
                JObject Jdata = null;
                string JValue = string.Empty;

                if (!string.IsNullOrEmpty(privacy))
                {
                    if (privacy == "Close Friends")
                    {
                        Jdata = JObject.Parse(fb.Get("/" + fbUserId + "/friendlists/close_friends").ToString());
                        string closefrndid = Jdata["data"][0]["id"].ToString();
                        JValue = "{ \"description\": \"Close Friends\",\"value\": \"CUSTOM\",\"friends\": \"SOME_FRIENDS\",\"networks\": \"\",\"allow\":\"" + closefrndid + "\",\"deny\": \"\"}";
                    }
                    else if (privacy == "Only Me")
                    {
                        JValue = "{\"description\": \"Only Me\",\"value\": \"SELF\",\"friends\": \"\",\"networks\": \"\",\"allow\": \"\",\"deny\": \"\"}";
                    }
                    else if (privacy == "Friends")
                    {
                        JValue = "{\"description\": \"Your friends\",\"value\": \"ALL_FRIENDS\",\"friends\": \"\",\"networks\": \"\",\"allow\": \"\",\"deny\": \"\"}";
                    }
                    else if (privacy == "Friends of Friends")
                    {
                        JValue = "{\"description\": \"Your friends of friends\",\"value\": \"FRIENDS_OF_FRIENDS\",\"friends\": \"\",\"networks\": \"\",\"allow\": \"\",\"deny\": \"\"}";
                    }
                    else if (privacy == "Family")
                    {
                        Jdata = JObject.Parse(fb.Get("/" + fbUserId + "/friendlists/family").ToString());
                        string familyid = Jdata["data"][0]["id"].ToString();
                        JValue = "{\"description\": \"Family\",\"value\": \"CUSTOM\",\"friends\": \"SOME_FRIENDS\",\"networks\": \"\",\"allow\": \"" + familyid + "\",\"deny\": \"\"}";
                    }
                    else if (privacy == "Public")
                    {
                        JValue = "{\"description\": \"Public\",\"value\": \"EVERYONE\",\"friends\": \"\",\"networks\": \"\",\"allow\": \"\",\"deny\": \"\"}";
                    }
                    else if (privacy == "Acquaintances")
                    {
                        Jdata = JObject.Parse(fb.Get("/" + fbUserId + "/friendlists/acquaintances").ToString());
                        string AcquaintancesId = Jdata["data"][0]["id"].ToString();
                        JValue = "{\"description\": \"Acquaintances\",\"value\": \"CUSTOM\",\"friends\": \"SOME_FRIENDS\",\"networks\": \"\",\"allow\": \"" + AcquaintancesId + "\",\"deny\": \"\"}";
                    }
                    return JValue;
                }
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return "";
            }
        }


    }

    public class FacebookEye
    {
        public FacebookEye()
        {
            //FacebookClient.SetDefaultHttpWebRequestFactory(uri => {
            //    var request = new HttpWebRequestWrapper((HttpWebRequest)WebRequest.Create(uri));
            //    request.Proxy = MyFilesDatabase.GetRequestsProxy(); // normal .net IWebProxy
            //    return request;
            //});
        }

        public void GetAccessToken()
        {

        }
    }
}
