using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.ViewModels
{
    public class StreamTabViewModel : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<StreamTabStreamViewModel> StreamTabsStreams { get; set; }

        private string tabTitle;
        public string TabTitle
        {
            get { return tabTitle; }
            set { tabTitle = value; NotifyOfPropertyChange(); }
        }

        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set { isLoggedIn = value; NotifyOfPropertyChange(); }
        }

        public FacebookApisController FacebookApisController { get; set; }

        public StreamTabViewModel(FacebookApisController facebookApisController)
        {
            FacebookApisController = facebookApisController;
            FacebookApisController_OnAccessTokenSet();
            facebookApisController.OnAccessTokenSet += FacebookApisController_OnAccessTokenSet;
            

            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            StreamTabsStreams = new ObservableCollection<StreamTabStreamViewModel>();
        }

        private void FacebookApisController_OnAccessTokenSet()
        {
            IsLoggedIn = FacebookApisController.AccessToken_FB != "";
        }

        private void OnCommandFromView_Raised(object obj)
        {
            switch (obj as string)
            {
                case "Connect":
                    Connect();
                    break;

                case "TimeLine":
                    TimeLine();
                    break;

                case "Page":
                    Page();
                    break;

                case "Group":
                    Group();
                    break;

                case "CloseTab":
                    CloseTab();
                    break;

                default:
                    break;
            }
        }

        private void Connect()
        {
            FacebookApisController.OauthApi();
        }

        private async void TimeLine()
        {
            StreamTabsStreams.Add(new StreamTabStreamViewModel(FacebookApisController, "TimeLine") { });
            await FacebookApisController.RequestTimeline();
        }

        private void Page()
        {
        }

        private void Group()
        {
        }

        private void CloseTab()
        {
        }


    }
}
