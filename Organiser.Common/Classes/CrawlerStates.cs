using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes.Crawler
{
    public enum CrawlerStates
    {
        FbGraphCrawl = 0,
        LikesFromPost = 1,
    }
    public class CrawlerState
    {
        public CrawlerStates State { get; set; }
    }
}
