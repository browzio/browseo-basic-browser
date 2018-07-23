using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Twitter
{
    public class TwitterUserInfo
    {
        public string id_str { get; set; }
        public string name { get; set; }
        public string screen_name { get; set; }
        public string location { get; set; }
        public string profile_location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string followers_count { get; set; }
        public string friends_count { get; set; }
        public string listed_count { get; set; }
        public string favourites_count { get; set; }
        public string statuses_count { get; set; }
        public string created_at { get; set; }
        public string lang { get; set; }
    }

    public class TweetTimelines
    {
        public string coordinates { get; set; }
        public string truncated { get; set; }
        public string created_at { get; set; }
        public string favorited { get; set; }
        public string id_str { get; set; }
        public string in_reply_to_user_id_str { get; set; }
        public string text { get; set; }
        public string contributors { get; set; }
        public string in_reply_to_status_id_str { get; set; }
        public string geo { get; set; }
        public string retweeted { get; set; }
        public string in_reply_to_user_id { get; set; }
        public string place { get; set; }
        public string source { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public string in_reply_to_status_id { get; set; }
        public string retweet_count { get; set; }
        public string favorite_count { get; set; }


        public entities entities { get; set; }
        public TwitterUserInfo user { get; set; }
    }

    public class TweetTrend
    {
        public string as_of { get; set; }
        public string created_at { get; set; }

        public location[] locations { get; set; }
        public trend[] trends { get; set; }
    }

    public class entities
    {
        public hashtag[] hashtags { get; set; }
        public user_mention[] user_mentions { get; set; }
        public urls[] urls { get; set; }
    }

    public class hashtag
    {
        public string text { get; set; }
    }

    public class user_mention
    {
        public string screen_name { get; set; }
        public string name { get; set; }
        public string id_str { get; set; }
    }

    public class urls
    {
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
    }

    public class location
    {
        public string name { get; set; }
        public string woeid { get; set; }
    }

    public class trend
    {
        public string name { get; set; }
        public string url { get; set; }
        public string promoted_content { get; set; }
        public string query { get; set; }
        public string tweet_volume { get; set; }
    }
}
