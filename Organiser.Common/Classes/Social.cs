using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Organiser.Common.Classes
{
    public class Social
    {
        public const string FACEBOOK_GRAPH_LINK = "https://developers.facebook.com/tools/explorer/";
        public const string FACEBOOK_PAGES_DEFAULT_URL = "https://www.facebook.com/pages/";
        public const string FACEBOOK_GROUPS_DEFAULT_URL = "https://www.facebook.com/groups/";
        public const string FACEBOOK_EVENTS_DEFAULT_URL = "https://www.facebook.com/events/";
        public const string FACEBOOK_PLACES_DEFAULT_URL = "https://www.facebook.com/places/";
        public const string FACEBOOK_USERS_DEFAULT_URL = "https://www.facebook.com/people/";
        public const string FACEBOOK_PHOTOS_DEFAULT_URL = "https://www.facebook.com/photos/";
        public const string FACEBOOK_VIDEOS_DEFAULT_URL = "https://www.facebook.com/videos/";

        public const string SOCIALTYPE_fb = "facebook";
        public const string SOCIALTYPE_gp = "google";
        public const string SOCIALTYPE_digg = "digg";
        public const string SOCIALTYPE_pin = "pintrest";
        public const string SOCIALTYPE_reddit = "reddit";
        public const string SOCIALTYPE_stumble = "stumbleupon";
        public const string SOCIALTYPE_tumblr = "tumblr";
        public const string SOCIALTYPE_twit = "twitter";
        public const string SOCIALTYPE_wp = "wordpress";

        public const string SHARELINK_facebook = "https://www.facebook.com/sharer/sharer.php?u=";
        public const string SHARELINK_googleplus = "https://plus.google.com/share?url=";
        public const string SHARELINK_digg = "http://digg.com/submit?url=";
        public const string SHARELINK_pintrest = "https://pinterest.com/pin/create/button/?url=";
        public const string SHARELINK_reddit = "http://reddit.com/submit?url=";
        public const string SHARELINK_stumbleupon = "http://www.stumbleupon.com/submit?url=";
        public const string SHARELINK_tumblr = "http://www.tumblr.com/share/link?url=";
        public const string SHARELINK_twitter = "https://twitter.com/home?status=";
        public const string SHARELINK_wordpress = "/wp-admin/press-this.php?u=";//has to add site to beggining https://{usersInput site}
    }
}
