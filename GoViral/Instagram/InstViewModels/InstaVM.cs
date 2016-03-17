using InstaSharp;
using InstaSharp.Endpoints;
using InstaSharp.Models;
using InstaSharp.Models.Responses;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xilium.CefGlue.Client;

namespace GoViral.Instagram.InstViewModels
{
    internal class InstaVM : ViewModelBase
    {
        public const string PATHDOMINATEFILE = "SavedInfo";
        public const string PATHSEARCHFILE = "SavedSearchInfo";
        public const string LINK_INSTAGRAM = "https://www.instagram.com/";

        public ICommand OnCommandFromView { get; set; }

        public static string ClientID = "5a95395c66cd4555b85802a91cfb230d";
        public static string ClientSecret = "d213300494224857a5afc81fa98fab5a";
        public static string RedirectUri = "http://localhost:1969/";

        public InstagramConfig InstaConfig = new InstagramConfig(ClientID, ClientSecret, RedirectUri, "");
        public OAuthResponse InstaResponse;

        private UserInfo user;
        public UserInfo User
        {
            get { return user; }
            set { user = value; RaisePropertyChanged("User"); }
        }

        private BrowserForSocialShare bfss;

        private static InstaVM instance;
        public static InstaVM Instance
        {
            get
            {
                if (instance == null) instance = new InstaVM();

                return instance;
            }
        }


        private int rateLimit;
        public int RateLimit
        {
            get { return rateLimit; }
            set { rateLimit = value;
                RaisePropertyChanged("RateLimit"); }
        }

        private int rateLimitRemaining;
        public int RateLimitRemaining
        {
            get { return rateLimitRemaining; }
            set { rateLimitRemaining = value;
                RaisePropertyChanged("RateLimitRemaining"); }
        }


        private InstaVM()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        public void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            switch (param)
            {
                case "LOGIN":
                    LogUserIn();
                    break;

                default:
                    break;
            }
        }

        private void LogUserIn()
        {
            var scopes = new List<OAuth.Scope>();
            scopes.Add(OAuth.Scope.Basic);
            scopes.Add(OAuth.Scope.Public_Content);
            scopes.Add(OAuth.Scope.Follower_List);
            scopes.Add(OAuth.Scope.Comments);
            scopes.Add(OAuth.Scope.Relationships);
            scopes.Add(OAuth.Scope.Likes);

            var link = OAuth.AuthLink(InstaConfig.OAuthUri + "authorize", InstaConfig.ClientId, InstaConfig.RedirectUri, scopes, OAuth.ResponseType.Code);

            if (bfss == null || bfss.IsDisposed)
            {
                bfss = new BrowserForSocialShare();
                bfss.browserCntrl1.init(link);
            }
            else
            {
                bfss.browserCntrl1.Navigate(link);
            }
            bfss.Text = "Login Loading...";
            bfss.Show();
            bfss.browserCntrl1.OnBrowserAddressChanged += BrowserCntrl1_OnBrowserAddressChanged;
            //bfss.FormClosed += (s, e) => { bfss = null; };
        }

        private void BrowserCntrl1_OnBrowserAddressChanged(string changedLink)
        {
            if (changedLink.Contains("http://localhost:1969/?code="))
            {
                string code = changedLink.Remove(0, changedLink.IndexOf("=") + 1);

                if (code.IsNullOrEmpty()) "Did not successfully retreive login".Show();
                else OAuthUserCode(code);

                if (bfss != null)
                {
                    bfss.browserCntrl1.OnBrowserAddressChanged -= BrowserCntrl1_OnBrowserAddressChanged;
                    bfss.Hide();
                }
            }
        }

        public async void OAuthUserCode(string code)
        {
            try
            {
                // add this code to the auth object
                var auth = new OAuth(InstaConfig);

                // now we have to call back to instagram and include the code they gave us
                // along with our client secret
                var oauthResponse = await auth.RequestToken(code);

                // both the client secret and the token are considered sensitive data, so we won't be
                // sending them back to the browser. we'll only store them temporarily.  If a user's session times
                // out, they will have to click on the authenticate button again - sorry bout yer luck.
                InstaResponse = oauthResponse;

                // all done, lets redirect to the home controller which will send some intial data to the app
                User = InstaResponse.User;

                RateLimit = RateLimitRemaining = 0;
            }
            catch (Exception ex)
            {
                ex.Message.Show();
            }
        }


        //Comments
        public InstaSharp.Endpoints.Comments GetEndpointComments()
        {
            return new InstaSharp.Endpoints.Comments(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse);
        }

        //Likes
        public InstaSharp.Endpoints.Likes GetEndpointLikes()
        {
            return new InstaSharp.Endpoints.Likes(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse);
        }

        //Tags
        public Tags GetEndpointTags()
        {
            return new Tags(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse);
        }

        //Relationships
        public Relationships GetEndpointRelationships()
        {
            return new Relationships(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse);
        }

        //Users
        public Users GetEndpointUsers()
        {
            return new Users(InstaVM.Instance.InstaConfig, InstaVM.Instance.InstaResponse);
        }

        public void CheckShouldnotContinueRequest(object requestObject, object data, Response response)
        {
            InstaVM.Instance.RateLimit = response.RateLimitLimit;
            InstaVM.Instance.RateLimitRemaining = response.RateLimitRemaining;

            if (response.RateLimitLimit == 0 || response.RateLimitRemaining == 0) throw new Exception("You have exceeded the rate limit for api usage on this profile.");

            if (requestObject == null) throw new Exception("Request could not be completed because the request data was modified during the request");
            if (data == null || response == null || response.Meta == null) throw new Exception("Request replyed with an empy result");

            if (response.Meta.Code.ToString() != "OK")
            {
                if (response.Meta.ErrorMessage != null && response.Meta.ErrorType != null)
                    throw new Exception("Request Failed Error Message: " + response.Meta.ErrorMessage + Environment.NewLine +
                                                       "Error Type: " + response.Meta.ErrorType);

                throw new Exception(response.Meta.Code.ToString());
            }
        }
    }
}
