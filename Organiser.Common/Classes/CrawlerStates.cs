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
        LoadAllPhotos = 1,
        LoadAllVideos = 2,
        GraphSearch_Pages = 3,
        GraphSearch_Groups = 4,
        GraphSearch_Events = 5,
        GraphSearch_Places = 6,
        GraphSearch_Users = 7,
    }
}
