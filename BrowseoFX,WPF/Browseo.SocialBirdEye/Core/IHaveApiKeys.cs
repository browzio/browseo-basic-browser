using BrowseoFX_WPF.Browseo.SocialBirdEye.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Core
{
    public interface IHaveApiKeys
    {
        ApiKeysBase ClientKeys { get; set; }
        AccessTokenInfoBase AccessTokenInfo { get; set; }
    }
}
