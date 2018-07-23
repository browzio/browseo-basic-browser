using BrowseoFX_WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using Organiser.Common.Classes;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Pinterest;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks
{
    #region responses
    public class PinterestResponse_AccessToken
    {
        //{"access_token": "AXAm3NND7jbJp6i8eDL3xgEZuDtsFTWOtqmCm-5E_C4EoqA23QAAAAA", "token_type": "bearer", "scope": ["read_public", "write_public", "read_private", "write_private", "read_relationships", "read_write_all"]}
        public string access_token { get; set; }
        public string token_type { get; set; }
    }
    #endregion

    public class PinterestEye : ViewModelBase
    {
        public PinterestEyeHttpRequests Requests { get; set; }
        public PinterestResponse_AccessToken AccessTokenInfo { get; set; }

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


        private PinterestUserInfo userInfo;
        public PinterestUserInfo UserInfo
        {
            get { return  userInfo; }
            set {  userInfo = value; NotifyOfPropertyChange(); }
        }

        public ObservableCollection<PinterestBoardDataInfoData> UserBoards { get; set; }
        private PinterestBoardDataInfoData selectedUserBoard;
        public PinterestBoardDataInfoData SelectedUserBoard
        {
            get { return selectedUserBoard; }
            set { selectedUserBoard = value; NotifyOfPropertyChange(); }
        }

        public ObservableCollection<PinterestBoardsFollowingDataInfoData> UserBoardsFollowing { get; set; }
        private PinterestBoardsFollowingDataInfoData selectedFollowingBoard;
        public PinterestBoardsFollowingDataInfoData SelectedFollowingBoard
        {
            get { return selectedFollowingBoard; }
            set { selectedFollowingBoard = value; NotifyOfPropertyChange(); }
        }

        public ObservableCollection<PinterestPinsDataInfoData> UserPins { get; set; }
        public ObservableCollection<PinterestFollowersDataInfoData> UserFollowers { get; set; }
        public ObservableCollection<PinterestFollowersDataInfoData> UserFollowing { get; set; }

        public PinterestEye()
        {
            //TODO load and save
            ClientID = "4972947039907227833";
            ClientSecret = "6684bd0881fb029f13badbd2b55c38755489efc68b97ef5b85b7c6b0793ec55d";
            RedirectUrl = "https%3A%2F%2Flocalhost:1234%2F";

            Requests = new PinterestEyeHttpRequests(this);
            UserBoards = new ObservableCollection<PinterestBoardDataInfoData>();
            UserPins = new ObservableCollection<PinterestPinsDataInfoData>();
            UserFollowers = new ObservableCollection<PinterestFollowersDataInfoData>();
            UserFollowing = new ObservableCollection<PinterestFollowersDataInfoData>();
            UserBoardsFollowing = new ObservableCollection<PinterestBoardsFollowingDataInfoData>();

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }
        internal void OauthUser()
        {
            Requests.GetOauthCode();
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            var param = obj as string;
            if (param == null) return;
            try
            {
                switch (param)
                {
                    case "GetSelectedUserBoardPins":
                        if (SelectedUserBoard == null) return;

                        var boardPinsResponse = await Requests.PinterestGetRequest<PinterestPinsDataInfo>(PinterestRequestUrls.PinteresBoardInfo + SelectedUserBoard.id + PinterestRequestUrls.PinteresBoardInfoFields + AccessTokenInfo.access_token);
                        if (boardPinsResponse != null && boardPinsResponse.data != null && boardPinsResponse.data.Length > 0)
                        {
                            SelectedUserBoard.Pins.Clear();
                            foreach (var pin in boardPinsResponse.data)
                            {
                                SelectedUserBoard.Pins.Add(pin);
                            }
                        }
                        break;

                    case "GetSelectedFollowingBoardPins":
                        if (SelectedFollowingBoard == null) return;

                        var followBoardPinsResponse = await Requests.PinterestGetRequest<PinterestPinsDataInfo>(PinterestRequestUrls.PinteresBoardInfo + SelectedFollowingBoard.id + PinterestRequestUrls.PinteresBoardInfoFields + AccessTokenInfo.access_token);
                        if (followBoardPinsResponse != null && followBoardPinsResponse.data != null && followBoardPinsResponse.data.Length > 0)
                        {
                            SelectedFollowingBoard.Pins.Clear();
                            foreach (var pin in followBoardPinsResponse.data)
                            {
                                SelectedFollowingBoard.Pins.Add(pin);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = "Pinterest Eye Dashboard " + param + " threw an Exception: " + ex.Message;

            }
        }

        internal async void MakeRequest(string param)
        {
            try
            {
                switch (param)
                {
                    case "PinterestUserInfo":
                        UserInfo = await Requests.PinterestGetRequest<PinterestUserInfo>(PinterestRequestUrls.PinterestUserInfo + AccessTokenInfo.access_token);
                        break;

                    case "PinterestUserBoards":
                        var userBoardsResponse = await Requests.PinterestGetRequest<PinterestBoardDataInfo>(PinterestRequestUrls.PinterestUserBoards + AccessTokenInfo.access_token);
                        if(userBoardsResponse != null && userBoardsResponse.data!= null && userBoardsResponse.data.Length > 0)
                        {
                            UserBoards.Clear();
                            foreach (var board in userBoardsResponse.data)
                            {
                                UserBoards.Add(board);
                            }
                            if (UserBoards.Count > 0) SelectedUserBoard = UserBoards[0];
                        }
                        break;

                    case "PinterestUserBoardsSuggested":
                        await Requests.PinterestGetRequest<object>(PinterestRequestUrls.UserSuggestedBoardInfo + AccessTokenInfo.access_token);
                        break;

                    case "PinterestUserPins":
                        var userPinsResponse = await Requests.PinterestGetRequest<PinterestPinsDataInfo>(PinterestRequestUrls.PinterestUserPins + AccessTokenInfo.access_token);
                        if (userPinsResponse != null && userPinsResponse.data != null && userPinsResponse.data.Length > 0)
                        {
                            UserPins.Clear();
                            foreach (var pin in userPinsResponse.data)
                            {
                                UserPins.Add(pin);
                            }
                        }
                        break;

                    case "PinterestUserFollowers":
                        var userFollowersResponse = await Requests.PinterestGetRequest<PinterestFollowersDataInfo>(PinterestRequestUrls.PinterestUserFollowers + AccessTokenInfo.access_token);
                        if (userFollowersResponse != null && userFollowersResponse.data != null && userFollowersResponse.data.Length > 0)
                        {
                            UserFollowers.Clear();
                            foreach (var follower in userFollowersResponse.data)
                            {
                                UserFollowers.Add(follower);
                            }
                        }
                        break;

                    case "PinterestBoardsFollowing":
                        var userBoardsFollwersResponse = await Requests.PinterestGetRequest<PinterestBoardsFollowingDataInfo>(PinterestRequestUrls.PinterestBoardsFollowing + AccessTokenInfo.access_token);
                        if (userBoardsFollwersResponse != null && userBoardsFollwersResponse.data != null && userBoardsFollwersResponse.data.Length > 0)
                        {
                            UserBoardsFollowing.Clear();
                            foreach (var board in userBoardsFollwersResponse.data)
                            {
                                UserBoardsFollowing.Add(board);
                            }
                        }
                        break;

                    case "PinterestInterestsFollowing":
                        await Requests.PinterestGetRequest<object>(PinterestRequestUrls.PinterestInterestsFollowing + AccessTokenInfo.access_token);
                        break;

                    case "PinterestUsersFollowing":
                        var userFollowingResponse = await Requests.PinterestGetRequest<PinterestFollowersDataInfo>(PinterestRequestUrls.PinterestUsersFollowing + AccessTokenInfo.access_token);
                        if (userFollowingResponse != null && userFollowingResponse.data != null && userFollowingResponse.data.Length > 0)
                        {
                            UserFollowing.Clear();
                            foreach (var follow in userFollowingResponse.data)
                            {
                                UserFollowing.Add(follow);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = "Pinterest request error: " + param + Environment.NewLine + ex.Message;
                message.Show();
            }
        }
    }

    public class PinterestEyeHttpRequests
    {
        public PinterestEye OwnerEye { get; set; }

        public PinterestEyeHttpRequests(PinterestEye ownerEye)
        {
            OwnerEye = ownerEye;
        }

        internal void GetOauthCode()
        {
            //       https://api.pinterest.com/oauth/?
            //response_type=code&
            //redirect_uri=https://mywebsite.com/connect/pinterest/&
            //client_id=12345&
            //scope=read_public,write_public&
            //state=768uyFys

            BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            BrowseoFXManager.Instance.GloableWebView.Navigated += GloableWebView_Navigated;

            var authUrl = PinterestRequestUrls.AuthUrl + "?response_type=code&scope=read_public,write_public,read_relationships,write_relationships&client_id=" + OwnerEye.ClientID + "&redirect_uri=" + OwnerEye.RedirectUrl + "&state=" + GenUniqueState();


            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(authUrl);
        }
        private async void GloableWebView_Navigated(object sender, GeckoNavigatedEventArgs e)
        {
            try
            {
                if (e.Url == null || !e.Url.Query.Contains("&code=")) return;
                BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;

                var code = e.Url.Query.Split(new string[] { "&code=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //https://api.pinterest.com/v1/oauth/token?
                //grant_type = authorization_code &
                //client_id = 12345 &
                //client_secret = 6789abcd &
                //code = xyz1010
                string postData = "grant_type=authorization_code&code=" + code + "&client_id=" + OwnerEye.ClientID + "&client_secret=" + OwnerEye.ClientSecret;
                string tokenJsonResponse = await Task<string>.Run(() =>
                {
                    return HttpRequestsBase.WebRequest(HttpRequestsBase.Method.POST, PinterestRequestUrls.AccessTokenUrl, postData);
                });
                OwnerEye.AccessTokenInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<PinterestResponse_AccessToken>(tokenJsonResponse);
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

        public async Task<T> PinterestGetRequest<T>(string url)
        {
            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.HttpWebGetRequest(url);
            });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonResponse);
        }

        //internal async Task PinterestUserInfo()
        //{
        //    var url = PinterestRequestUrls.UserInfo + OwnerEye.AccessTokenInfo.access_token;
        //    string jsonResponse = await Task.Run(() =>
        //    {
        //        return HttpRequestsBase.HttpWebGetRequest(url);
        //    });

        //  //  return Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
        //}

        //internal async Task PinterestUserBoardInfo()
        //{
        //    var url = PinterestRequestUrls.UserBoardInfo + OwnerEye.AccessTokenInfo.access_token;
        //    string jsonResponse = await Task.Run(() =>
        //    {
        //        return HttpRequestsBase.HttpWebGetRequest(url);
        //    });

        //    //  return Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
        //}

        //internal async Task PinterestUserBoards()
        //{
        //    var url = PinterestRequestUrls.UserBoardInfo + OwnerEye.AccessTokenInfo.access_token;
        //    string jsonResponse = await Task.Run(() =>
        //    {
        //        return HttpRequestsBase.HttpWebGetRequest(url);
        //    });

        //    //  return Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
        //}

        //internal async Task PinterestUserBoardsSuggested()
        //{
        //    var url = PinterestRequestUrls.UserSuggestedBoardInfo + OwnerEye.AccessTokenInfo.access_token;
        //    string jsonResponse = await Task.Run(() =>
        //    {
        //        return HttpRequestsBase.HttpWebGetRequest(url);
        //    });

        //    //  return Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
        //}

        //internal async Task PinterestUserPins()
        //{
        //    var url = PinterestRequestUrls.UserPins + OwnerEye.AccessTokenInfo.access_token;
        //    string jsonResponse = await Task.Run(() =>
        //    {
        //        return HttpRequestsBase.HttpWebGetRequest(url);
        //    });

        //    //  return Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(jsonResponse);
        //}
    }
}
