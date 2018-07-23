using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.LinkedIn
{
    public class UserProfile
    {
        public string distance { get; set; }
        public string emailAddress { get; set; }
        public string firstName { get; set; }
        public string headline { get; set; }
        public string id { get; set; }
        public string industry { get; set; }
        public string lastName { get; set; }
        public string profile_url { get; set; }
        public string pictureUrl { get; set; }

        public Location location { get; set; }

    }

    public class Location
    {
        public string name { get; set; }
    }

    public class Positions
    {
        public string _total { get; set; }
        public PositionValues[] values { get; set; }
    }

    public class PositionValues
    {
        public NameValue company { get; set; }
        public string id { get; set; }
        public string isCurrent { get; set; }
        public NameValue location { get; set; }
        public NameValueDates startDate { get; set; }
        public string summary { get; set; }
        public string title { get; set; }
    }

    public class NameValue
    {
        public string name { get; set; }
    }

    public class NameValueDates
    {
        public string month { get; set; }
        public string year { get; set; }
    }

    //    {
    //  "id": "eyFlrODNPa",
    //  "numConnections": 145,
    //  "pictureUrl": "https://media.licdn.com/dms/image/C5603AQFuACodt_CThQ/profile-displayphoto-shrink_100_100/0?e=1534377600&v=beta&t=0oXbhWU2nbNitmRJVDwImAl7Vhb5Gb9DzPhwydX1quE"
    //}


    public class PeopleConnection
    {
        public string id { get; set; }
        public string numConnections { get; set; }
        public string pictureUrl { get; set; }
    }

}
