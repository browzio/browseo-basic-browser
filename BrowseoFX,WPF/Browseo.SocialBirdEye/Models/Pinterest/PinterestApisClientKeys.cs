using BrowseoFX_WPF.Browseo.SocialBirdEye.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Pinterest
{
    public class PinterestClientKeys : ApiKeysBase
    {
        public override string SaveFileKeys { get { return Path.Combine(FileAndFolderFactory.Instance.SaveDirectory, "Pinterest"); } }
        public override string SaveFileToken { get { return Path.Combine(FileAndFolderFactory.Instance.SaveDirectory, "PinterestToken"); } }
    }

    public class PinterestAccessTokenInfo : AccessTokenInfoBase
    {
        //public string access_token { get; set; }
        //public string expires_in { get; set; }
        //public string refresh_token { get; set; }
        //public string token_type { get; set; }
    }
}
