using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Twitter
{
    public class TwitterApisClientKeys
    {
        public string SaveFile { get { return Path.Combine(FileAndFolderFactory.Instance.SaveDirectory, "Twitter"); } }

        public string OauthToken { get; set; }
        public string OauthTokenSecret { get; set; }
        public string OauthConsumerKey { get; set; }
        public string OauthConsumerSecret { get; set; }
        public string ScreenName { get; set; }
    }
}
