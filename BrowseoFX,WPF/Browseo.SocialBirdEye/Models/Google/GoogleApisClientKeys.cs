using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using Delimon.Win32.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Google
{
    public class GoogleApisClientKeys : ApiKeysBase
    {
        public override string SaveFileKeys { get { return Path.Combine(FileAndFolderFactory.Instance.SaveDirectory, "Google"); } }
        public override string SaveFileToken { get { return Path.Combine(FileAndFolderFactory.Instance.SaveDirectory, "GoogleToken"); } }
    }

    public class GoogleApisAccessTokenInfo : AccessTokenInfoBase
    {
        //public string access_token { get; set; }
        //public string expires_in { get; set; }
        //public string refresh_token { get; set; }
        //public string token_type { get; set; }
    }
}
