using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes.Crawler
{
    public enum CrawlerStates
    {
        [Description("Pages")]
        FbGraphCrawl = 0,
        [Description("Photos")]
        LoadAllPhotos = 1,
        [Description("Photos")]
        LoadAllPhotos_Crawl = 2,
        [Description("Videos")]
        LoadAllVideos = 3,
        [Description("Videos")]
        LoadAllVideos_Crawl = 4,
        [Description("Pages")]
        GraphSearch_Pages = 5,
        [Description("Groups")]
        GraphSearch_Groups = 6,
        [Description("Events")]
        GraphSearch_Events = 7,
        [Description("Places")]
        GraphSearch_Places = 8,
        [Description("Users")]
        GraphSearch_Users = 9,
        [Description("Photos")]
        GraphSearch_Photos = 10,
        [Description("Videos")]
        GraphSearch_Videos = 11,
        PageType_Pages = 12,
        PageType_Groups = 13,
        PageType_Events = 14,
        PageType_Places = 15,
        PageType_Users = 16,
        PageType_Videos = 17,
        PageType_Photos = 18,
    }
}
