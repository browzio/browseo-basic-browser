using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Pinterest;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;
using BrowseoFX_WPF.Core;
using Gecko;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Core
{
    public class AuthTokenActivator
    {
        public object Controller { get; private set; }

        public AuthTokenActivator(object controller)
        {
            Controller = controller;
        }

        public void SetOauthCode()
        {
            var authUrl = "";
            if (Controller is GoogleApisController)
            {
                var apisController = Controller as GoogleApisController;

                authUrl =
                    GoogleRequestsUrls.strAuthentication +
                    "?scope=" + GoogleRequestsUrls.UserScope +
                    "&redirect_uri=" + apisController.ClientKeys.RedirectUrl +
                    "&response_type=code&client_id=" + apisController.ClientKeys.ClientID +
                    "&approval_prompt=force&access_type=offline";
            }
            else if (Controller is PinterestApisController)
            {
                var apisController = Controller as PinterestApisController;

                authUrl =
                    PinterestRequestUrls.AuthUrl +
                    "?response_type=code&scope=read_public,write_public,read_relationships,write_relationships&client_id=" + apisController.ClientKeys.ClientID +
                    "&redirect_uri=" + apisController.ClientKeys.RedirectUrl +
                    "&state=" + GenUniqueState();
               
            }
            else if(Controller is FacebookApisController)
            {
                var apisController = Controller as FacebookApisController;

                authUrl = Social.FACEBOOK_GRAPH_LINK;
            }

            BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            BrowseoFXManager.Instance.GloableWebView.Navigated += GloableWebView_Navigated;

            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(authUrl);
        }
        private async void GloableWebView_Navigated(object sender, GeckoNavigatedEventArgs e)
        {
            try
            {
                if (Controller is GoogleApisController)
                {
                    var apisController = Controller as GoogleApisController;

                    if (e.Url == null || !e.Url.Query.Contains("?code=")) return;
                    BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;

                    var code = e.Url.Query.Replace("?code=", "");

                    string postData = 
                        "code=" + code + 
                        "&client_id=" + apisController.ClientKeys.ClientID + 
                        "&client_secret=" + apisController.ClientKeys.ClientSecret +
                        "&redirect_uri=" + apisController.ClientKeys.RedirectUrl + 
                        "&grant_type=authorization_code";
                    string tokenJsonResponse = await Task<string>.Run(() =>
                    {
                        return HttpRequestsBase.WebRequest(HttpRequestsBase.Method.POST, GoogleRequestsUrls.strRefreshToken, postData);
                    });
                    apisController.AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleApisAccessTokenInfo>(tokenJsonResponse);
                    FileAndFolderFactory.Instance.SaveTokenInfo(tokenJsonResponse, apisController);
                }
                else if (Controller is PinterestApisController)
                {
                    var apisController = Controller as PinterestApisController;
                    if (e.Url == null || !e.Url.Query.Contains("&code=")) return;
                    BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;

                    var code = e.Url.Query.Split(new string[] { "&code=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    string postData = "grant_type=authorization_code&code=" + code + "&client_id=" + apisController.ClientKeys.ClientID + "&client_secret=" + apisController.ClientKeys.ClientSecret;
                    string tokenJsonResponse = await Task<string>.Run(() =>
                    {
                        return HttpRequestsBase.WebRequest(HttpRequestsBase.Method.POST, PinterestRequestUrls.AccessTokenUrl, postData);
                    });
                    apisController.AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PinterestAccessTokenInfo>(tokenJsonResponse);
                    FileAndFolderFactory.Instance.SaveTokenInfo(tokenJsonResponse, apisController);
                }
                else if (Controller is FacebookApisController)
                {
                    var apisController = Controller as FacebookApisController;
                    if (BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.ReadyState == "loading") return;
                    BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
                    string source = (BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DocumentElement as Gecko.DOM.HTML.GeckoHTMLHtmlElement).OuterHtml;

                    var AccessToken = source.Trim().Split(new string[] { "placeholder=\"Paste in an existing Access Token or click &quot;Get User Access Token" }, StringSplitOptions.None)[1];
                    AccessToken = AccessToken.Split(new string[] { "value=" }, StringSplitOptions.None)[1];
                    AccessToken = AccessToken.Remove(AccessToken.IndexOf(@">"));
                    AccessToken = AccessToken.Replace("\"", "");
                    AccessToken = AccessToken.Replace(" type=text", "");

                    if (e.Url.OriginalString == Social.FACEBOOK_GRAPH_LINK)
                        apisController.AccessToken_FB = AccessToken;
                    else
                        apisController.AccessToken_Insta = AccessToken;
                }
            }
            catch
            {
                BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            }
        }

        public async Task GetRefreshToken()
        {
            if (Controller is GoogleApisController)
            {
                var apisController = Controller as GoogleApisController;
                string postData =
                        "client_id=" + apisController.ClientKeys.ClientID +
                        "&client_secret=" + apisController.ClientKeys.ClientSecret +
                        "&redirect_uri=" + apisController.ClientKeys.RedirectUrl +
                        "&refresh_token=" + apisController.AccessTokenInfo.refresh_token +
                        "&grant_type=refresh_token";
                string tokenJsonResponse = await Task<string>.Run(() =>
                {
                    return HttpRequestsBase.WebRequest(HttpRequestsBase.Method.POST, "https://www.googleapis.com/oauth2/v4/token", postData);
                });
                apisController.AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleApisAccessTokenInfo>(tokenJsonResponse);
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
    }
}
