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
    public class RelationshipToEnabledConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Relationship r = value as Relationship;
            if (r == null) return false;
            if (r.TargetUserIsPrivate) return false;

            switch (r.IncomingStatus)
            {
                case IncomingStatus.FollowedBy:
                    return true;
                case IncomingStatus.RequestedBy:
                    return true;
                case IncomingStatus.BlockedbyYou:
                    return false;
                case IncomingStatus.None:
                    break;
                default:
                    break;
            }
            switch (r.OutgoingStatus)
            {
                case OutgoingStatus.Follows:
                    return true;
                case OutgoingStatus.Requested:
                    return false;
                case OutgoingStatus.None:
                    return true;
                default:
                    break;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
