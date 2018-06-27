using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.SocialBirdEye.Models
{
    public class Facebookpage
    {
        public string ProfilePageId { get; set; }
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string LikeCount { get; set; }
        public int connected { get; set; }
        public int friendsCount { get; set; }

    }
}
