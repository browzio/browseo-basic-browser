using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Windows;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Organiser.Common.Classes.MyFilesDatabase;

namespace Browseo.SocialBirdEye.ViewModels
{
    public class BirdsEyeDashboardViewModel : ViewModelBase
    {
        #region singletone
        private static BirdsEyeDashboardViewModel instance;
        public static BirdsEyeDashboardViewModel Instance
        {
            get
            {
                if (instance == null) instance = new BirdsEyeDashboardViewModel();
                return instance;
            }
        }

        public void ShowWindow()
        {
            var birdsEyeWindow = new BirdsEyeDashboardWindow();
            birdsEyeWindow.DataContext = this;
            birdsEyeWindow.Show();
        }
        #endregion

        public static string SaveDirectory { get { return Path.Combine(MyFilesDatabase.GetBaseDir(), "BirdsEyeKeys", GloableProfData.PData.ProjectName); } }
        
        public TwitterEye Controller_Twitter { get; set; }
        public InstagramEye Controller_Instagram { get; set; }
        public GoogleEye Controller_Google { get; set; }
        public LinkedInEye Controller_LinkedIn { get; set; }
        public PinterestEye Controller_Pinterest { get; set; }
        
        public GoogleApisController GoogleApisController { get; set; }
        public PinterestApisController PinterestApisController { get; set; }
        public TwitterApisController TwitterApisController { get; set; }
        public FacebookApisController FacebookApisController { get; set; }

        public ICommand OnCommandFromView { get; set; }

        private BirdsEyeDashboardViewModel()
        {
            GoogleApisController = new GoogleApisController();
            PinterestApisController = new PinterestApisController();
            TwitterApisController = new TwitterApisController();
            FacebookApisController = new FacebookApisController();

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

                    case "TwitterInfo":
                        var twitterUserInfo = await Controller_Twitter.LoginAndGetUserInfo();
                        break;

                    #region instagram
                    case "InstagramInfo":
                        var instagramUserInfo = await Controller_Instagram.GetInstagramInfo();
                        break;

                    case "OauthInstagram":
                        Controller_Instagram.GetAccessToken();
                        break;
                    #endregion

                    #region google


                    //case "GetYoutubeActivities":
                    //    GoogleApisController.GetYoutubeActivities();
                    //    break;

                    //case "GetYoutubePlaylists":
                    //    GoogleApisController.GetYoutubePlaylists();
                    //    break;

                    //case "GetYoutubeChannelSections":
                    //    GoogleApisController.GetYoutubeChannelSections();
                    //    break;

                    //case "GetYoutubeSubscriptions":
                    //    GoogleApisController.GetYoutubeSubscriptions();
                    //    break;







                    case "GooglePlusSummary":
                        Controller_Google.GooglePlusSummary();
                        break;

                    case "YoutubeChannels":
                        Controller_Google.YoutubeChannels();
                        break;
                    #endregion

                    #region linkedIn

                    case "LinkedInOauth":
                        Controller_LinkedIn.OauthUser();
                        break;

                    case "GetUserProfile":
                        Controller_LinkedIn.GetUserProfile();
                        break;

                    case "GetPeopleConnection":
                        Controller_LinkedIn.GetPeopleConnection();
                        break;

                    case "GetLinkedInCompanyPage":
                        Controller_LinkedIn.GetLinkedInCompanyPage();
                        break;

                    case "GetNetworkUpdates":
                        Controller_LinkedIn.GetNetworkUpdates();
                        break;

                    case "GetInvitationsPending":
                        Controller_LinkedIn.GetInvitationsPending();
                        break;

                    #endregion

                    #region Pinterest

                    case "PinterestOauth":
                        Controller_Pinterest.OauthUser();
                        break;

                    case "PinterestUserInfo":
                    case "PinterestUserBoards":
                    case "PinterestUserBoardsSuggested":
                    case "PinterestUserPins":
                    case "PinterestUserFollowers":
                    case "PinterestBoardsFollowing":
                    case "PinterestInterestsFollowing":
                    case "PinterestUsersFollowing":
                        Controller_Pinterest.MakeRequest(param);
                        break;
                    #endregion

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = "Birds Eye Dashboard " + param + " threw an Exception: " + ex.Message;

            }
        }
    }
}
