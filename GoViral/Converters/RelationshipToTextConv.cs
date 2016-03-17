using InstaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GoViral.Converters
{
    class RelationshipToTextConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Relationship r = value as Relationship;
            if (r == null) return "";

            

            string param = parameter as string;
            switch (param)
            {
                case "Follower":
                    switch (r.IncomingStatus)
                    {
                        case IncomingStatus.FollowedBy:
                            return "Following You";
                        case IncomingStatus.RequestedBy:
                            return "Follow Requested";
                        case IncomingStatus.BlockedbyYou:
                            return "Blocked";
                        case IncomingStatus.None:
                            return "Not Following You";
                        default:
                            break;
                    }
                    break;

                case "Folowing":
                    switch (r.OutgoingStatus)
                    {
                        case OutgoingStatus.Follows:
                            return "Following";
                        case OutgoingStatus.Requested:
                            return "Request Sent";
                        case OutgoingStatus.None:
                            return "Not Following";
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }



            if (r.TargetUserIsPrivate) return "User Is Private";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
