using BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks_Controllers;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.ViewModels
{
    public class StreamTabStreamViewModel : ViewModelBase
    {
        public string Type { get; set; }

        public FacebookApisController FacebookApisController { get; set; }

        public StreamTabStreamViewModel(FacebookApisController facebookApisController, string type)
        {
            FacebookApisController = facebookApisController;
            Type = type;
        }
    }
}
