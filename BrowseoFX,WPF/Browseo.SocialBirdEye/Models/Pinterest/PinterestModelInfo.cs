using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Pinterest
{
    #region boards following

    public class PinterestBoardsFollowingDataInfo
    {
        public PinterestBoardsFollowingDataInfoData[] data { get; set; }
        public PinteresInfoDataPage page { get; set; }
        
    }

    public class PinterestBoardsFollowingDataInfoData
    {
        public string name { get; set; }
        public PinteresInfoDataCreator creator { get; set; }
        public string url { get; set; }
        public string created_at { get; set; }
        public string privacy { get; set; }
        public string reason { get; set; }
        public PinteresInfoDataCounts counts { get; set; }
        public string id { get; set; }

        public PinterestInfoDataImage image { get; set; }

        public ObservableCollection<PinterestPinsDataInfoData> Pins { get; set; }
        public PinterestBoardsFollowingDataInfoData()
        {
            Pins = new ObservableCollection<PinterestPinsDataInfoData>();
        }
    }
    #endregion

    #region followers
    //    {
    //"data": [{
    //    "bio": "",
    //    "first_name": "Shelley",
    //    "last_name": "Arth",
    //    "account_type": "individual",
    //    "url": "https://www.pinterest.com/shelleyarth/",
    //    "created_at": "2018-01-18T10:14:33",
    //    "image": {
    //        "60x60": {
    //            "url": "https://i.pinimg.com/60x60_RS/e1/2a/d7/e12ad7ad9054a79b9b82a1347506501e.jpg",
    //            "width": 60,
    //            "height": 60
    //        }
    //    },
    //    "counts": {
    //        "pins": 73,
    //        "following": 24580,
    //        "followers": 1963,
    //        "boards": 16
    //    },
    //    "id": "824299675455656180"
    //}]
    //    }
    public class PinterestFollowersDataInfo
    {
        public PinterestFollowersDataInfoData[] data { get; set; }
        public PinteresInfoDataPage page { get; set; }
    }

    public class PinterestFollowersDataInfoData
    {
        public string id { get; set; }
        public string bio { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string account_type { get; set; }
        public string url { get; set; }
        public string created_at { get; set; }
        public string username { get; set; }

        public PinterestInfoDataImage image { get; set; }
        public PinteresInfoDataCounts counts { get; set; }
    }
    #endregion

    #region pins
    //    {
    //    "data": [{
    //        "attribution": {
    //            "title": "TECHO MOVIL POLICARBONATO MANUAL",
    //            "url": "http://www.youtube.com/watch?v=gtY13LfQMbE",
    //            "provider_icon_url": "https://s.pinimg.com/images/api/attrib/youtube@2x.png",
    //            "author_name": "tolbeach",
    //            "provider_favicon_url": "https://s.pinimg.com/images/api/attrib/youtube.png",
    //            "author_url": "https://www.youtube.com/user/tolbeach",
    //            "provider_name": "youtube"
    //        },
    //        "creator": {
    //            "url": "https://www.pinterest.com/elidadia/",
    //            "first_name": "Eli",
    //            "last_name": "Dadia",
    //            "id": "333899897281497782"
    //        },
    //        "url": "https://www.pinterest.com/pin/333899759868260378/",
    //        "media": {
    //            "type": "video"
    //        },
    //        "created_at": "2018-04-25T13:02:53",
    //        "note": "TECHO MOVIL POLICARBONATO MANUAL - YouTube",
    //        "color": "#8a694e",
    //        "link": "https://www.pinterest.com/r/pin/333899759868260378/4970896124924670632/41719d78363f56c142dd373621ad38e90ea17d89752035c610741b7a3820bef7",
    //        "board": {
    //            "url": "https://www.pinterest.com/elidadia/board-1/",
    //            "id": "333899828562415644",
    //            "name": "board 1"
    //        },
    //        "image": {
    //            "original": {
    //                "url": "https://i.pinimg.com/originals/50/a7/07/50a707ebc3634f49d277758539458897.jpg",
    //                "width": 480,
    //                "height": 360
    //            }
    //        },
    //        "counts": {
    //            "saves": 0,
    //            "comments": 0
    //        },
    //        "id": "333899759868260378",
    //        "metadata": {
    //            "link": {
    //                "locale": "es",
    //                "title": "TECHO MOVIL POLICARBONATO MANUAL",
    //                "site_name": "YouTube",
    //                "description": "ESPACIOS PROTEGIDOS TOLBEACH. EL TECHO MOVIL DE POLICARBONATO ES EL MEJOR SISTEMA PARA AMPLIAR SU VIVIENDA DE FORMA SENCILLA SIN PERMISOS ESPECIALES. CON TOT...",
    //                "favicon": "https://i.pinimg.com/favicons/bafe84dd639f0820c254e9cc6c202ff5b4e1d1f37a2b99397c028218.png?3a880420311ad60097059ffc0fc53393"
    //            }
    //        }]
    //    }
    public class PinterestPinsDataInfo
    {
        public PinterestPinsDataInfoData[] data { get; set; }
        public PinteresInfoDataPage page { get; set; }
    }

    public class PinterestPinsDataInfoData
    {
        public string id { get; set; }
        public string url { get; set; }
        public string created_at { get; set; }
        public string note { get; set; }
        public string color { get; set; }
        public string link { get; set; }
        public string original_link { get; set; }

        public PinteresInfoDataAttribution attribution { get; set; }
        public PinteresInfoDataCreator creator { get; set; }
        public PinteresInfoDataMedia media { get; set; }
        public PinteresInfoDataBoard board { get; set; }
        public PinterestInfoDataImage image { get; set; }
        public PinteresInfoDataCounts counts { get; set; }
    }
    #endregion

    #region boardInfo
    //    {
    //    "data": [{
    //        "description": "",
    //        "creator": {
    //            "url": "https://www.pinterest.com/elidadia/",
    //            "first_name": "Eli",
    //            "last_name": "Dadia",
    //            "id": "333899897281497782"
    //        },
    //        "url": "https://www.pinterest.com/elidadia/sweet-stuff/",
    //        "created_at": "2018-04-25T12:17:21",
    //        "image": {
    //            "60x60": {
    //                "url": null,
    //                "width": 60,
    //                "height": 60
    //            }
    //        },
    //        "counts": {
    //            "pins": 0,
    //            "collaborators": 0,
    //            "followers": 61
    //        },
    //        "id": "333899828562682742",
    //        "name": "sweet stuff"
    //    }]
    //}
    public class PinterestBoardDataInfo
    {
        public PinterestBoardDataInfoData[] data { get; set; }
        public PinteresInfoDataPage page { get; set; }
    }

    public class PinterestBoardDataInfoData
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string created_at { get; set; }

        public PinterestInfoDataImage image { get; set; }
        public PinteresInfoDataCounts counts { get; set; }
        public PinteresInfoDataCreator creator { get; set; }

        public ObservableCollection<PinterestPinsDataInfoData> Pins { get; set; }
        public PinterestBoardDataInfoData()
        {
            Pins = new ObservableCollection<PinterestPinsDataInfoData>();
        }
    }
    #endregion

    #region userInfo
    //{
    //    "data": {
    //        "username": "elidadia",
    //        "bio": "col1",
    //        "first_name": "Eli",
    //        "last_name": "Dadia",
    //        "created_at": "2015-03-04T19:53:31",
    //        "image": {
    //            "60x60": {
    //                "url": "https://i.pinimg.com/60x60_RS/c9/92/e9/c992e950d9f0388b0c7edddf074eab8b.jpg",
    //                "width": 60,
    //                "height": 60
    //            }
    //        },
    //        "counts": {
    //            "pins": 26,
    //            "following": 47,
    //            "followers": 63,
    //            "boards": 10
    //        },
    //        "id": "333899897281497782"
    //    }
    //}
    public class PinterestUserInfo
    {
        public PinterestUserInfoData data { get; set; }
    }

    public class PinterestUserInfoData
    {
        public string id { get; set; }
        public string username { get; set; }
        public string bio { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string created_at { get; set; }

        public PinterestInfoDataImage image { get; set; }
        public PinteresInfoDataCounts counts { get; set; }
    }
    #endregion

    public class PinterestInfoDataImage
    {
        [JsonProperty("60x60")]
        public PinterestInfoDataImageData sixtyBysixty { get; set; }
        public PinterestInfoDataImageData original { get; set; }
    }

    public class PinterestInfoDataImageData
    {
        public string url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
    }

    public class PinteresInfoDataCounts
    {
        public string pins { get; set; }
        public string following { get; set; }
        public string followers { get; set; }
        public string boards { get; set; }
        public string saves { get; set; }
        public string comments { get; set; }
        public string collaborators { get; set; }
    }


    public class PinteresInfoDataCreator
    {
        public string url { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id { get; set; }
    }

    public class PinteresInfoDataMedia
    {
        public string type { get; set; }
    }

    public class PinteresInfoDataAttribution
    {
        public string title { get; set; }
        public string url { get; set; }
        public string provider_icon_url { get; set; }
        public string author_name { get; set; }
        public string provider_favicon_url { get; set; }
        public string author_url { get; set; }
        public string provider_name { get; set; }
    }
    public class PinteresInfoDataBoard
    {
        public string url { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class PinteresInfoDataPage
    {
        public string cursor { get; set; }
        public string next { get; set; }
    }
}
