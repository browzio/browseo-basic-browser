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
            if (r == null)return "Follow";
            if (r.TargetUserIsPrivate) return "User Is Private";
            string returnstring = "Follow";
            switch (r.IncomingStatus)
            {
                case IncomingStatus.FollowedBy:
                    return "Following You";
                case IncomingStatus.RequestedBy:
                    return "Reqested You";
                case IncomingStatus.BlockedbyYou:
                    return "Blocked You";
                default:
                    break;
            }
            switch (r.OutgoingStatus)
            {
                case OutgoingStatus.Follows:
                    return "Un Follow";
                case OutgoingStatus.Requested:
                    return "Cancel Request";
                case OutgoingStatus.None:
                default:
                    break;
            }
            return returnstring;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
