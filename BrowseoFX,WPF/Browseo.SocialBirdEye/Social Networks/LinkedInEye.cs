using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.LinkedIn;
using BrowseoFX_WPF.Core;
using Gecko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks
{

    #region responses
    //https://developer.linkedin.com/docs/oauth2
    public class LinkedInResponse_AccessToken
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
    }
    #endregion

    public class LinkedInRequestUrls
    {
        public const string AuthUrl = "https://www.linkedin.com/oauth/v2/authorization?scope=rw_company_admin%20r_basicprofile%20r_emailaddress%20w_share";
        public const string AccessTokenUrl = "https://www.linkedin.com/oauth/v2/accessToken";

        #region oldurls

        //public static string GetNetworkUserUpdates = "https://api.linkedin.com/v1/people/id=";
        //public static string GetNetworkUpdates = "https://api.linkedin.com/v1/people/~/network/updates";
        //public static string GetGroupUpdates = "https://api.linkedin.com/v1/people/~/group-memberships?membership-state=member&count=100";

        //public static string GetJobSearchTitle = "https://api.linkedin.com/v1/job-search?job-title=";
        //public static string GetJobSearchKeyword = "https://api.linkedin.com/v1/job-search?keywords=";
        //public static string GetUserProfile = "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,headline,picture_url,educations,location,date_of_birth)";
        //public static string StatusUpdate = "https://api.linkedin.com/v1/people/~/current-status";
        //public static string StatusUpdateImage = "https://api.linkedin.com/v1/people/~/shares";
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public static string GetUserProfileUrl = "https://api.linkedin.com/v1/people/~:(id,first-name,headline,last-name,industry,site-standard-profile-request,api-standard-profile-request,member-url-resources,picture-url,current-status,summary,positions,main-address,location,distance,specialties,proposal-comments,associations,honors,interests,educations,phone-numbers,im-accounts,twitter-accounts,date-of-birth,email-address)";
        //public static string GetPeopleSearchUrl = "https://api.linkedin.com/v1/people-search?keyword=";
        //public static string GetPeopleConnectionUrl = "https://api.linkedin.com/v1/people/~/connections";
        //public static string GetCompanyUrl = "https://api.linkedin.com/v1/companies?is-company-admin=true";  
        #endregion
            
        public static string GetNetworkUserUpdates = "https://api.linkedin.com/v1/people/id=";
        public static string GetNetworkUpdates = "https://api.linkedin.com/v1/people/~/network/updates?format=json&count=50&start=0";//"https://api.linkedin.com/v1/people/~/network/updates";
        public static string GetGroupUpdates = "https://api.linkedin.com/v1/people/~/group-memberships?membership-state=member&count=100";

        public static string GetJobSearchTitle = "https://api.linkedin.com/v1/job-search?job-title=";
        public static string GetJobSearchKeyword = "https://api.linkedin.com/v1/job-search?keywords=";
        public static string GetUserProfile = "https://api.linkedin.com/v1/people/~:(id,first-name,last-name,headline,picture_url,educations,location,date_of_birth)";
        public static string StatusUpdate = "https://api.linkedin.com/v1/people/~/current-status";
        public static string StatusUpdateImage = "https://api.linkedin.com/v1/people/~/shares";
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //https://api.linkedin.com/v1/people-search:(people:(id,first-name,last-name,headline,picture-url,industry,positions:(id,title,summary,start-date,end-date,is-current,company:(id,name,type,size,industry,ticker)),educations:(id,school-name,field-of-study,start-date,end-date,degree,activities,notes)),num-results)?first-name=parameter&last-name=parameter

        public static string GetUserProfileUrl = "https://api.linkedin.com/v1/people/~:(id,first-name,headline,last-name,industry,site-standard-profile-request,api-standard-profile-request,member-url-resources,picture-url,current-status,summary,positions,main-address,location,distance,specialties,proposal-comments,associations,honors,interests,educations,phone-numbers,im-accounts,twitter-accounts,date-of-birth,email-address)?format=json";
        public static string GetPeopleSearchUrl = "https://api.linkedin.com/v1/people-search?keyword=";
        public static string GetPeopleConnectionUrl = "https://api.linkedin.com/v1/people/~:(id,num-connections,picture-url)?format=json";
        public static string GetCompanyUrl = "https://api.linkedin.com/v1/companies?is-company-admin=true";

        public static string GetLinkedInCompanyPageUrl = "https://api.linkedin.com/v1/companies?format=json&is-company-admin=true";
        public static string GetCompanyPageDetailUrl = "https://api.linkedin.com/v1/companies/{id}:(id,name,ticker,description)?format=json";

        public static string GetInvitationsPendingUrl = "https://api.linkedin.com/v2/invitations?q=invitee&states=PENDING";
    }

    public class LinkedInEye
    {
        public LinkedInHttpRequests Requests { get; set; }

        public LinkedInEye()
        {
            Requests = new LinkedInHttpRequests();
        }
        internal void OauthUser()
        {
            //https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=123456789&redirect_uri=https%3A%2F%2Fwww.example.com%2Fauth%2Flinkedin&state=987654321&scope=r_basicprofile
            Requests.GetOauthCode();
        }

        internal async void GetUserProfile()
        {
            var userprofile = await Requests.GetUserProfile();
        }

        internal async void GetPeopleConnection()
        {
            var userConnections = await Requests.GetPeopleConnection();
        }

        internal async void GetLinkedInCompanyPage()
        {
            var companyPage = await Requests.GetLinkedInCompanyPage();
        }

        internal async void GetNetworkUpdates()
        {
            var companyPage = await Requests.GetNetworkUpdates();
        }

        internal async void GetInvitationsPending()
        {
            try
            {
                var companyPage = await Requests.GetInvitationsPending();
            }
            catch
            {

            }
        }
    }

    public class LinkedInHttpRequests
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }

        public LinkedInResponse_AccessToken AccessTokenInfo { get; set; }

        public LinkedInHttpRequests()
        {
            //TODO load and save
            ClientID = "86l6y00bo9e46p";
            ClientSecret = "Ri9IOiwNbKjXYBst";
            RedirectUrl = "http%3A%2F%2Flocalhost:1234%2F";
        }

        internal void GetOauthCode()
        {
            var authUrl = LinkedInRequestUrls.AuthUrl + "&response_type=code&client_id="+ ClientID + "&redirect_uri="+ RedirectUrl+ "&state=" + GenUniqueState();

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
                //grant_type=authorization_code&code=987654321&redirect_uri=https%3A%2F%2Fwww.myapp.com%2Fauth%2Flinkedin&client_id=123456789&client_secret=shhdonottell
                string postData = "grant_type=authorization_code&code=" + code + "&client_id=" + ClientID + "&client_secret=" + ClientSecret + "&redirect_uri=" + RedirectUrl;
                string tokenJsonResponse = await Task<string>.Run(() =>
                {
                    return HttpRequestsBase.WebRequest(HttpRequestsBase.Method.POST, LinkedInRequestUrls.AccessTokenUrl, postData);
                });
                AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<LinkedInResponse_AccessToken>(tokenJsonResponse);

            }
            catch
            {
                BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            }
        }
        private string GenUniqueState()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            return GuidString;
        }

        internal async Task<UserProfile> GetUserProfile()
        {
            var url = LinkedInRequestUrls.GetUserProfileUrl + "&" + "oauth2_access_token=" + this.AccessTokenInfo.access_token;
            
            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.GetByWebRequest(url);
            });

            return Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
        }

        internal async Task<PeopleConnection> GetPeopleConnection()
        {
            var url = LinkedInRequestUrls.GetPeopleConnectionUrl + "&" + "oauth2_access_token=" + this.AccessTokenInfo.access_token;

            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.GetByWebRequest(url);
            });

            return Newtonsoft.Json.JsonConvert.DeserializeObject<PeopleConnection>(jsonResponse);
        }

        internal async Task<PeopleConnection> GetLinkedInCompanyPage()
        {
            var url = LinkedInRequestUrls.GetLinkedInCompanyPageUrl + "&" + "oauth2_access_token=" + this.AccessTokenInfo.access_token;

            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.GetByWebRequest(url);
            });

            return Newtonsoft.Json.JsonConvert.DeserializeObject<PeopleConnection>(jsonResponse);
        }

        internal async Task<PeopleConnection> GetNetworkUpdates()
        {
            var url = LinkedInRequestUrls.GetNetworkUserUpdates + "eyFlrODNPa" + "/network/updates?scope=self" + "&" + "oauth2_access_token=" + this.AccessTokenInfo.access_token;

            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.GetByWebRequest(url);
            });

            return Newtonsoft.Json.JsonConvert.DeserializeObject<PeopleConnection>(jsonResponse);
        }

        internal async Task<PeopleConnection> GetInvitationsPending()
        {
            var url = LinkedInRequestUrls.GetInvitationsPendingUrl;//"https://api.linkedin.com/v1/people/~:(id,following,first-name,headline,last-name,industry,site-standard-profile-request,api-standard-profile-request,member-url-resources,picture-url,current-status,summary,positions,main-address,location,distance,specialties,proposal-comments,associations,honors,interests,educations,phone-numbers,im-accounts,twitter-accounts,date-of-birth,email-address)?format=json" + "&" + "oauth2_access_token=" + this.AccessTokenInfo.access_token;
            //string jsonResponse = await Task.Run(() =>
            //{
            //    //try
            //    //{
            //    //    //return HttpRequestsBase.GetByWebRequest(url);
            //    //    return HttpRequestsBase.GetByCurlWebRequest(url, this.AccessTokenInfo.access_token);
            //    //}
            //    //catch(Exception ex)
            //    //{ }

            //    return null;
            //});

            return Newtonsoft.Json.JsonConvert.DeserializeObject<PeopleConnection>("");
        }
    }
}
