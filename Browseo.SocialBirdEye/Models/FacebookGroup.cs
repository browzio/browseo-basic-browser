using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.SocialBirdEye.Models
{
    public class FacebookGroup
    {
        public string ProfileGroupId { get; set; }
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
