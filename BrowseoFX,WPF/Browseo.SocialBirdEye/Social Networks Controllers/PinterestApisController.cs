using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Pinterest;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers
{
    public class PinterestRequestUrls
    {
        public const string AuthUrl = "https://api.pinterest.com/oauth/";
        public const string AccessTokenUrl = "https://api.pinterest.com/v1/oauth/token";

        public static string PinterestUserInfo = "https://api.pinterest.com/v1/me?fields=id,username,first_name,last_name,bio,created_at,counts,image&access_token=";
        public static string PinterestUserBoards = "https://api.pinterest.com/v1/me/boards?fields=id,name,url,description,creator,created_at,counts,image&access_token=";
        public static string PinterestUserPins = "https://api.pinterest.com/v1/me/pins?fields=id,link,url,creator,board,created_at,note,color,counts,media,attribution,image,metadata&access_token=";

        public static string UserSuggestedBoardInfo = "https://api.pinterest.com/v1/me/boards/suggested/?access_token=";

        public static string PinterestUserFollowers = "https://api.pinterest.com/v1/me/followers?fields=first_name%2Cid%2Clast_name%2Curl%2Ccounts%2Ccreated_at%2Cimage%2Caccount_type%2Cbio&access_token=";
        public static string PinterestBoardsFollowing = "https://api.pinterest.com/v1/me/following/boards/?fields=id%2Cname%2Curl%2Ccounts%2Ccreated_at%2Ccreator%2Cdescription%2Cprivacy%2Creason&access_token=";
        public static string PinterestInterestsFollowing = "https://api.pinterest.com/v1/me/following/interests?access_token=";
        public static string PinterestUsersFollowing = "https://api.pinterest.com/v1/me/following/users/?fields=first_name%2Cid%2Clast_name%2Curl%2Caccount_type%2Cbio%2Ccounts%2Ccreated_at%2Cimage%2Cusername&access_token=";

        public static string PinteresBoardInfo = "https://api.pinterest.com/v1/boards/";
        public static string PinteresBoardInfoFields = "/pins/?fields=id%2Clink%2Cnote%2Curl%2Coriginal_link%2Cattribution%2Ccolor%2Ccounts%2Ccreated_at%2Ccreator%2Cmetadata&access_token=";
    }

    public class PinterestApisController : ViewModelBase, IHaveApiKeys
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

        private PinterestUserInfo userInfo;
        public PinterestUserInfo UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; NotifyOfPropertyChange(); }
        }

        private PinterestBoardDataInfo boardDataInfo;
        public PinterestBoardDataInfo UserBoards
        {
            get { return boardDataInfo; }
            set { boardDataInfo = value; NotifyOfPropertyChange(); }
        }

        private PinterestPinsDataInfo userPins;
        public PinterestPinsDataInfo UserPins
        {
            get { return userPins; }
            set { userPins = value; NotifyOfPropertyChange(); }
        }

        private PinterestBoardsFollowingDataInfo boardsfollowing;
        public PinterestBoardsFollowingDataInfo BoardsFollowing
        {
            get { return boardsfollowing; }
            set { boardsfollowing = value; NotifyOfPropertyChange(); }
        }

        private PinterestFollowersDataInfo userFollowers;
        public PinterestFollowersDataInfo UserFollowers
        {
            get { return userFollowers; }
            set { userFollowers = value; NotifyOfPropertyChange(); }
        }

        private PinterestFollowersDataInfo userFollowing;
        public PinterestFollowersDataInfo UserFollowing
        {
            get { return userFollowing; }
            set { userFollowing = value; NotifyOfPropertyChange(); }
        }

        public PinterestApisController()
        {
            ClientKeys = new PinterestClientKeys();
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
                    case "SetApiKeys":
                        SetApiKeys();
                        break;

                    case "OauthApi":
                        OauthPinterestApis();
                        break;

                    case "AnalyzeAllPinterestData":
                        await AnalyzeAllPinterestData();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = "Pinterest Apis Error " + param + " threw an Exception: " + ex.Message;

            }
        }

        private async Task AnalyzeAllPinterestData()
        {
            //await HttpRequestsBase.GetUrlRequest<object>(PinterestRequestUrls.PinterestInterestsFollowing + AccessTokenInfo.access_token);

            UserInfo = await HttpRequestsBase.GetUrlRequest<PinterestUserInfo>(PinterestRequestUrls.PinterestUserInfo + AccessTokenInfo.access_token);
            UserBoards = await HttpRequestsBase.GetUrlRequest<PinterestBoardDataInfo>(PinterestRequestUrls.PinterestUserBoards + AccessTokenInfo.access_token);
            UserPins = await HttpRequestsBase.GetUrlRequest<PinterestPinsDataInfo>(PinterestRequestUrls.PinterestUserPins + AccessTokenInfo.access_token);
            UserFollowers = await HttpRequestsBase.GetUrlRequest<PinterestFollowersDataInfo>(PinterestRequestUrls.PinterestUserFollowers + AccessTokenInfo.access_token);
            BoardsFollowing = await HttpRequestsBase.GetUrlRequest<PinterestBoardsFollowingDataInfo>(PinterestRequestUrls.PinterestBoardsFollowing + AccessTokenInfo.access_token);
            UserFollowing = await HttpRequestsBase.GetUrlRequest<PinterestFollowersDataInfo>(PinterestRequestUrls.PinterestUsersFollowing + AccessTokenInfo.access_token);
        }

        private void OauthPinterestApis()
        {
            new AuthTokenActivator(this).SetOauthCode();
        }

        private void SetApiKeys()
        {
            FileAndFolderFactory.Instance.OpenOrSaveApiKeys(this);
        }
    }
}
