using Organiser.Common.Windows;
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
        public const string SOCIALTYPE_buffer = "buffer";

        public const string SOCIALTYPE_blogger = "blogger";
        public const string SOCIALTYPE_linkedIn = "linkedIn";

        public const string SOCIALTYPE_scoopit = "scoopit";
        public const string SOCIALTYPE_hootsuite = "hootsuite";

        public const string SHARELINK_facebook = "https://www.facebook.com/sharer/sharer.php?u=";
        public const string SHARELINK_googleplus = "https://plus.google.com/share?url=";
        public const string SHARELINK_digg = "http://digg.com/submit?url=";
        public const string SHARELINK_pintrest = "https://pinterest.com/pin/create/button/?url=";
        public const string SHARELINK_reddit = "http://reddit.com/submit?url=";
        public const string SHARELINK_stumbleupon = "http://www.stumbleupon.com/submit?url=";
        public const string SHARELINK_tumblr = "http://www.tumblr.com/share/link?url=";
        public const string SHARELINK_twitter = "https://twitter.com/home?status=";
        public const string SHARELINK_buffer = "https://buffer.com/add?url=";
        public const string SHARELINK_wordpress = "/wp-admin/press-this.php?u=";//has to add site to beggining https://{usersInput site}

        public const string SHARELINK_blogger = "https://www.blogger.com/blog-this.g?u=";
        public const string SHARELINK_linkedIn = "https://www.linkedin.com/shareArticle?url=";

        public const string SHARELINK_scoopit = "https://www.scoop.it/bookmarklet?url=";
        public const string SHARELINK_hootsuite = "https://hootsuite.com/hootlet/social-share?url=";

        public static string GetShareUrl(string shareType, string link, string imgLink = "")
        {
            link = link.Replace("/?type=3", "");
            string fullUrl = "";

            switch (shareType)
            {
                case Social.SOCIALTYPE_fb:
                case "ff_" + Social.SOCIALTYPE_fb:
                    fullUrl = Social.SHARELINK_facebook + link;
                    break;

                case Social.SOCIALTYPE_gp:
                case "ff_" + Social.SOCIALTYPE_gp:
                    fullUrl = Social.SHARELINK_googleplus + link;
                    break;

                case Social.SOCIALTYPE_buffer:
                case "ff_" + Social.SOCIALTYPE_buffer:
                    fullUrl = Social.SHARELINK_buffer + link;
                    break;

                case Social.SOCIALTYPE_blogger:
                case "ff_" + Social.SOCIALTYPE_blogger:
                    fullUrl = Social.SHARELINK_blogger + link+"&t="+link;
                    break;

                case Social.SOCIALTYPE_linkedIn:
                case "ff_" + Social.SOCIALTYPE_linkedIn:
                    fullUrl = Social.SHARELINK_linkedIn + link;
                    break;

                case Social.SOCIALTYPE_scoopit:
                case "ff_" + Social.SOCIALTYPE_scoopit:
                    //javascript:(function(){scscript=document.createElement('SCRIPT');scscript.type='text/javascript';scscript.src='https://www.scoop.it/resources/bklet/scoop.js?x='+(Math.random());document.getElementsByTagName('head')[0].appendChild(scscript);document.sc_srvurl='https://www.scoop.it'})();
                    //https://www.scoop.it/bookmarklet?url=
                    fullUrl = Social.SHARELINK_scoopit + link;
                    break;

                case Social.SOCIALTYPE_hootsuite:
                case "ff_hootsuite":
                    //javascript:(function(){scscript=document.createElement('SCRIPT');scscript.type='text/javascript';scscript.src='https://www.scoop.it/resources/bklet/scoop.js?x='+(Math.random());document.getElementsByTagName('head')[0].appendChild(scscript);document.sc_srvurl='https://www.scoop.it'})();
                    //https://www.scoop.it/bookmarklet?url=
                    fullUrl = Social.SHARELINK_hootsuite + link;
                    break;

                case Social.SOCIALTYPE_digg:
                case "ff_" + Social.SOCIALTYPE_digg:
                    fullUrl = Social.SHARELINK_digg + link;
                    break;

                case Social.SOCIALTYPE_pin:
                case "ff_" + Social.SOCIALTYPE_pin:
                    if (imgLink == "")
                    {
                        return "";
                    }
                    fullUrl = Social.SHARELINK_pintrest + link + "&media=" + imgLink;
                    break;

                case Social.SOCIALTYPE_reddit:
                case "ff_" + Social.SOCIALTYPE_reddit:
                    fullUrl = Social.SHARELINK_reddit + link;
                    break;

                case Social.SOCIALTYPE_stumble:
                case "ff_" + Social.SOCIALTYPE_stumble:
                    fullUrl = Social.SHARELINK_stumbleupon + link;
                    break;

                case Social.SOCIALTYPE_tumblr:
                case "ff_" + Social.SOCIALTYPE_tumblr:
                    fullUrl = Social.SHARELINK_tumblr + link;
                    break;

                case Social.SOCIALTYPE_twit:
                case "ff_" + Social.SOCIALTYPE_twit:
                    fullUrl = Social.SHARELINK_twitter + link;
                    break;

                case Social.SOCIALTYPE_wp:
                case "ff_" + Social.SOCIALTYPE_wp:
                    Organiser.Common.Windows.SetNameAndDataWindow alw = new Organiser.Common.Windows.SetNameAndDataWindow();
                    alw.tblockInfo.Text = "Enter wordpress site (ex: browzio.wordpress.com):";
                    alw.tbInputText.Text = GloableProfData.PData.WebAddress.IsNullOrEmpty()? "" : GloableProfData.PData.WebAddress;
                    alw.tbInputText.Text = alw.tbInputText.Text.Replace("http://", "");
                    alw.tbInputText.Text = alw.tbInputText.Text.Replace("https://", "");
                    if( alw.tbInputText.Text.EndsWith("/"))  alw.tbInputText.Text = alw.tbInputText.Text.Remove(alw.tbInputText.Text.IndexOf("/"));
                    alw.ShowDialog();
                    if (!alw.OkClicked) return "";
                    string wpUrl = alw.tbInputText.Text;
                    if (!wpUrl.Contains("http"))
                        wpUrl = "https://" + wpUrl;
                    fullUrl = wpUrl + Social.SHARELINK_wordpress + link;
                    break;

                default:
                    fullUrl = link;
                    break;
            }

            return fullUrl;
        }
    }
}
