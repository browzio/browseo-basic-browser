using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Organiser.Common.Classes.SocialHelpers
{
    public class SocialStatsURLRequest
    {
        //public const string FB_URL = "https://graph.facebook.com/v2.8/";
        //public const string FB_TOKEN = "EAAHJ7QkNbSEBABM2SOZBvYkYq4WjpZAjWtzlhncoOl0hZAJQFZA505CIZAoFDMpHJFZCqWrVZCBZCZAqmZCN6lNOx1Pqc86PQoSHqoNbxZC05tI3ECnCZB4uOWPifFwl6c21BsfbCHkWjUH1kM7qWulk20GpE23JebRj1hkZD";

        public const string FB_SHARES_URL = "https://graph.facebook.com/v2.8/";
        public const string FB_QUERI = "?fields=share,og_object{likes.limit(0).summary(true)}&access_token=503494873017633|F7xbKhlQGZGhPc8ThMU6pDdyiro";
        //EAAHJ7QkNbSEBABM2SOZBvYkYq4WjpZAjWtzlhncoOl0hZAJQFZA505CIZAoFDMpHJFZCqWrVZCBZCZAqmZCN6lNOx1Pqc86PQoSHqoNbxZC05tI3ECnCZB4uOWPifFwl6c21BsfbCHkWjUH1kM7qWulk20GpE23JebRj1hkZD";

        //need to crawl in browser
        public const string GOOGLE_PLUSONE_URL = "https://plusone.google.com/_/+1/fastbutton?url=";

        public const string PINTREST_PINS_URL = "http://api.pinterest.com/v1/urls/count.json?callback=receiveCount&url=";

        public const string STUMBLE_VIEWS_URL = "http://www.stumbleupon.com/services/1.01/badge.getinfo?url=";

        public const string LINKEDIN_URL = "https://www.linkedin.com/countserv/count/share?format=json&url=";

        public const string BUFFER_URL = "https://api.bufferapp.com/1/links/shares.json?url=";

        public const string REDDIT_URL = "https://www.reddit.com/api/info.json?url=";

        public static T GetStatsReply<T>(string url)
        {
            var source = "";
            using (var client = new WebClient())
            {
                if (!GloableProfData.PData.ProxyIP.IsNullOrEmpty() && !GloableProfData.PData.ProxyPort.IsNullOrEmpty())
                    client.Proxy = new WebProxy(GloableProfData.PData.ProxyIP, Convert.ToInt32(GloableProfData.PData.ProxyPort));
                if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty() && !GloableProfData.PData.ProxyPassword.IsNullOrEmpty())
                    client.Proxy.Credentials = new NetworkCredential(GloableProfData.PData.ProxyUsername, GloableProfData.PData.ProxyPassword);

                if (typeof(T) == typeof(FacebookReply))
                {
                    url = FB_SHARES_URL + url + FB_QUERI;
                }
                else if (typeof(T) == typeof(GoogleReply))
                {
                    url = GOOGLE_PLUSONE_URL + url;
                }
                else if (typeof(T) == typeof(PinterestReply))
                {
                    url = PINTREST_PINS_URL + url;
                }
                else if (typeof(T) == typeof(StumbleReply))
                {
                    url = STUMBLE_VIEWS_URL + url;
                }
                else if (typeof(T) == typeof(LinkedInReply))
                {
                    url = LINKEDIN_URL + url;
                }
                else if (typeof(T) == typeof(BufferReply))
                {
                    url = BUFFER_URL + url;
                }
                else if (typeof(T) == typeof(RedditReply))
                {
                    url = REDDIT_URL + url;
                }

                try
                {
                    source = client.DownloadString(url);
                }
                catch
                {
                    return default(T);
                }
            }
            if (typeof(T) == typeof(GoogleReply))
            {
                var arr = source.Split(new string[] { "<div id=\"aggregateCount\" class=\"Oy\">" }, StringSplitOptions.None);
                if (arr.Length > 1)
                {
                    source = arr[1];
                    source = source.Remove(source.IndexOf('<'));
                    return (T)(object)new GoogleReply() { PlusOnes = source };
                }
                else
                {
                    return (T)(object)new GoogleReply() { PlusOnes = "0" };
                }

            }
            else
            {
                if (typeof(T) == typeof(PinterestReply))
                {
                    source = source.Replace("receiveCount(", "");
                    source = source.Remove(source.LastIndexOf(")"));
                }

                return JsonConvert.DeserializeObject<T>(source);
            }
        }
    }

    public class FacebookReply
    {
        public Share share { get; set; }
        public OG_Object og_object { get; set; }

        public class Share
        {
            public string comment_count { get; set; }
            public string share_count { get; set; }
        }

        public class OG_Object
        {
            public Likes likes { get; set; }
            public class Likes
            {
                public Summary summary { get; set; }
                public class Summary
                {
                    public string total_count { get; set; }
                }
            }
        }
    }

    public class GoogleReply
    {
        public string PlusOnes { get; set; }
    }

    public class YoutubeReply
    {
        public string starRating_count { get; set; }
        public string starRating_average { get; set; }

        public string statistics_views { get; set; }
    }

    public class PinterestReply
    {
        public string count { get; set; }
    }

    public class StumbleReply
    {
        public Result result { get; set; }

        public class Result
        {
            public string views { get; set; }
        }
    }

    public class LinkedInReply
    {
        public string count { get; set; }
        public string fCnt { get; set; }
        public string fCntPlusOne { get; set; }
    }

    public class BufferReply
    {
        public string shares { get; set; }
    }

    public class RedditReply
    {
        public Data data { get; set; }
        public class Data
        {
            private ObservableCollection<Child> mchildren;
            public ObservableCollection<Child> children
            {
                get { return mchildren; }
                set { mchildren = value; }
            }
            public class Child
            {
                public ChildData data { get; set; }
                public class ChildData
                {
                    public string subreddit { get; set; }
                    public string author { get; set; }
                    public string permalink { get; set; }

                    public string num_comments { get; set; }
                    public string score { get; set; }
                    public string ups { get; set; }
                    public string downs { get; set; }

                    //combines reddit.com and the permalink
                    public string FullUrl { get { return "https://www.reddit.com" + permalink; } }
                }
            }
        }
    }

    public interface IHaveSocialStats
    {
        SocialStatsReplys SocialStatsReplys { get; set; }
    }

    public class SocialStatsFunctions
    {
        public static IEnumerable<IHaveSocialStats> OrderStatsBy(IEnumerable<IHaveSocialStats> statsList, string orderbyType)
        {
            IEnumerable<IHaveSocialStats> orderd = null;

            if (orderbyType == "ORDERBY_FBSHARES")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.FacebookReply == null ? 0 : result.SocialStatsReplys.FacebookReply.share == null ? 0 :
                                                             result.SocialStatsReplys.FacebookReply.share.share_count.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.FacebookReply.share.share_count));
            else if (orderbyType == "ORDERBY_FBLIKES")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.FacebookReply == null ? 0 : 
                                                             result.SocialStatsReplys.FacebookReply.og_object == null ? 0 : result.SocialStatsReplys.FacebookReply.og_object.likes == null ? 0 : result.SocialStatsReplys.FacebookReply.og_object.likes.summary == null ? 0 :
                                                             result.SocialStatsReplys.FacebookReply.og_object.likes.summary.total_count.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.FacebookReply.og_object.likes.summary.total_count));
            else if (orderbyType == "ORDERBY_FBCOMMENTS")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.FacebookReply == null ? 0 : result.SocialStatsReplys.FacebookReply.share == null ? 0 :
                                                             result.SocialStatsReplys.FacebookReply.share.comment_count.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.FacebookReply.share.comment_count));

            else if (orderbyType == "ORDERBY_GPLUSONES")
                 orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.GoogleReply == null ? 0 :
                                                              result.SocialStatsReplys.GoogleReply.PlusOnes.IsNullOrEmpty() ? 0 : result.SocialStatsReplys.GoogleReply.PlusOnes.ToLower().Contains("k") ? 10000 : Convert.ToInt32(result.SocialStatsReplys.GoogleReply.PlusOnes));
            
            else if (orderbyType == "ORDERBY_PINTERESTPINS")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.PinterestReply == null ? 0 :
                                                             result.SocialStatsReplys.PinterestReply.count.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.PinterestReply.count));

            else if (orderbyType == "ORDERBY_STUMBLEVIEWS")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.StumbleReply == null ? 0 : result.SocialStatsReplys.StumbleReply.result == null ? 0 :
                                                             result.SocialStatsReplys.StumbleReply.result.views.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.StumbleReply.result.views));

            else if (orderbyType == "ORDERBY_LINKEDINCOUNT")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.LinkedInReply == null ? 0 :
                                                             result.SocialStatsReplys.LinkedInReply.count.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.LinkedInReply.count));

            else if (orderbyType == "ORDERBY_BUFFERSHARES")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.BufferReply == null ? 0 :
                                                             result.SocialStatsReplys.BufferReply.shares.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.BufferReply.shares));

            else if (orderbyType == "ORDERBY_REDDITUPS")
                orderd = statsList.OrderByDescending(result =>  result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.RedditReply == null ? 0 : 
                result.SocialStatsReplys.RedditReply.data == null ? 0 : result.SocialStatsReplys.RedditReply.data.children == null ? 0 : result.SocialStatsReplys.RedditReply.data.children.Count == 0 ? 0 :
                                                        (int)result.SocialStatsReplys.RedditReply.data.children.Average(c=>c.data == null ? 0 : c.data.ups.IsNullOrEmpty() ? 0 : Convert.ToInt32(c.data.ups)));
            else if (orderbyType == "ORDERBY_REDDITSCORE")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.RedditReply == null ? 0 :
                 result.SocialStatsReplys.RedditReply.data == null ? 0 : result.SocialStatsReplys.RedditReply.data.children == null ? 0 : result.SocialStatsReplys.RedditReply.data.children.Count == 0 ? 0 :
                                         result.SocialStatsReplys.RedditReply.data.children.Average(c => c.data == null ? 0 : c.data.score.IsNullOrEmpty() ? 0 : Convert.ToInt32(c.data.score)));

            else if (orderbyType == "ORDERBY_YOUTUBEVIEWS")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.YoutubeReply == null ? 0 :
                                                             result.SocialStatsReplys.YoutubeReply.statistics_views.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.YoutubeReply.statistics_views));
            else if (orderbyType == "ORDERBY_YOUTUBERATINGS")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.YoutubeReply == null ? 0 :
                                                             result.SocialStatsReplys.YoutubeReply.starRating_count.IsNullOrEmpty() ? 0 : Convert.ToInt32(result.SocialStatsReplys.YoutubeReply.starRating_count));
            else if (orderbyType == "ORDERBY_YOUTUBERATINGAVERAGE")
                orderd = statsList.OrderByDescending(result => result.SocialStatsReplys == null ? 0 : result.SocialStatsReplys.YoutubeReply == null ? 0 :
                                                             result.SocialStatsReplys.YoutubeReply.starRating_average.IsNullOrEmpty() ? 0 : Convert.ToDouble(result.SocialStatsReplys.YoutubeReply.starRating_average));

            return orderd;
        }
    }

    public class SocialStatsReplys : ViewModelBase
    {
        public event Action OnFinishedAll;

        private int timesCralled = 0;

        private FacebookReply facebookReply;
        public FacebookReply FacebookReply
        {
            get { return facebookReply; }
            set { facebookReply = value; RaisePropertyChanged("FacebookReply"); }
        }

        private GoogleReply googleReply;
        public GoogleReply GoogleReply
        {
            get { return googleReply; }
            set { googleReply = value; RaisePropertyChanged("GoogleReply"); }
        }

        private YoutubeReply youtubeReply;
        public YoutubeReply YoutubeReply
        {
            get { return youtubeReply; }
            set { youtubeReply = value; RaisePropertyChanged("YoutubeReply"); }
        }

        private PinterestReply pinterestReply;
        public PinterestReply PinterestReply
        {
            get { return pinterestReply; }
            set { pinterestReply = value; RaisePropertyChanged("PinterestReply"); }
        }

        private StumbleReply stumbleReply;
        public StumbleReply StumbleReply
        {
            get { return stumbleReply; }
            set { stumbleReply = value; RaisePropertyChanged("StumbleReply"); }
        }

        private LinkedInReply linkedInReply;
        public LinkedInReply LinkedInReply
        {
            get { return linkedInReply; }
            set { linkedInReply = value; RaisePropertyChanged("LinkedInReply"); }
        }

        private BufferReply bufferReply;
        public BufferReply BufferReply
        {
            get { return bufferReply; }
            set { bufferReply = value; RaisePropertyChanged("BufferReply"); }
        }

        private RedditReply redditReply;
        public RedditReply RedditReply
        {
            get { return redditReply; }
            set { redditReply = value; RaisePropertyChanged("RedditReply"); }
        }

        public string StatsUrl { get; set; }

        public void GetAllStatsFor(string link = "")
        {
            if (link == "") link = StatsUrl;

            timesCralled = 0;
            Task.Run(() =>
            {
                BufferReply = SocialStatsURLRequest.GetStatsReply<BufferReply>(link);
                RaiseIfAddedAll();
            });
            Task.Run(() =>
            {
                FacebookReply = SocialStatsURLRequest.GetStatsReply<FacebookReply>(link);
                RaiseIfAddedAll();
            });
            Task.Run(() =>
            {
                GoogleReply = SocialStatsURLRequest.GetStatsReply<GoogleReply>(link);
                RaiseIfAddedAll();
            });
            Task.Run(() =>
            {
                LinkedInReply = SocialStatsURLRequest.GetStatsReply<LinkedInReply>(link);
                RaiseIfAddedAll();
            });
            Task.Run(() =>
            {
                PinterestReply = SocialStatsURLRequest.GetStatsReply<PinterestReply>(link);
                RaiseIfAddedAll();
            });
            Task.Run(() =>
            {
                StumbleReply = SocialStatsURLRequest.GetStatsReply<StumbleReply>(link);
                RaiseIfAddedAll();
            });
            Task.Run(() =>
            {
               RedditReply = SocialStatsURLRequest.GetStatsReply<RedditReply>(link);
                RaiseIfAddedAll();
            });
        }

        public async Task AsyncGetAllStatsFor(string link = "")
        {
            timesCralled = 0;

            if (link == "") link = StatsUrl;

            List<Task> statsTasks = new List<Task>();
            statsTasks.Add(
            Task.Run(() =>
            {
                BufferReply = SocialStatsURLRequest.GetStatsReply<BufferReply>(link);
                RaiseIfAddedAll();
            }));
            statsTasks.Add(
            Task.Run(() =>
            {
                FacebookReply = SocialStatsURLRequest.GetStatsReply<FacebookReply>(link);
                RaiseIfAddedAll();
            }));
            statsTasks.Add(
            Task.Run(() =>
            {
                GoogleReply = SocialStatsURLRequest.GetStatsReply<GoogleReply>(link);
                RaiseIfAddedAll();
            }));
            statsTasks.Add(
            Task.Run(() =>
            {
                LinkedInReply = SocialStatsURLRequest.GetStatsReply<LinkedInReply>(link);
                RaiseIfAddedAll();
            }));
            statsTasks.Add(
             Task.Run(() =>
            {
                PinterestReply = SocialStatsURLRequest.GetStatsReply<PinterestReply>(link);
                RaiseIfAddedAll();
            }));
            statsTasks.Add(
            Task.Run(() =>
            {
                RedditReply = SocialStatsURLRequest.GetStatsReply<RedditReply>(link);
                RaiseIfAddedAll();
            }));
            statsTasks.Add(
            Task.Run(() =>
            {
                StumbleReply = SocialStatsURLRequest.GetStatsReply<StumbleReply>(link);
                RaiseIfAddedAll();
            }));

            await Task.WhenAll(statsTasks);
        }

        public async Task AsyncAllAwaitGetAllStatsFor(string link = "")
        {
            if (link == "") link = StatsUrl;

            await Task.Run(() =>
            {
                BufferReply = SocialStatsURLRequest.GetStatsReply<BufferReply>(link);
                FacebookReply = SocialStatsURLRequest.GetStatsReply<FacebookReply>(link);
                GoogleReply = SocialStatsURLRequest.GetStatsReply<GoogleReply>(link);
                LinkedInReply = SocialStatsURLRequest.GetStatsReply<LinkedInReply>(link);
                PinterestReply = SocialStatsURLRequest.GetStatsReply<PinterestReply>(link);
                RedditReply = SocialStatsURLRequest.GetStatsReply<RedditReply>(link);
                StumbleReply = SocialStatsURLRequest.GetStatsReply<StumbleReply>(link);

                OnFinishedAll?.Invoke();
            });
        }

        private void RaiseIfAddedAll()
        {
            timesCralled++;
            if (timesCralled != 7) return;

            OnFinishedAll?.Invoke();
        }


    }
}
