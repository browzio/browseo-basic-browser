using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models
{
    public class ApiKeysBase
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }

        public virtual string SaveFileKeys { get; }
        public virtual string SaveFileToken { get; }
    }

    public class AccessTokenInfoBase
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
    }
}
