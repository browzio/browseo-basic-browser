using InstaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GoViral.Converters
{
    public class RelationshipToVisibleConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Relationship r = value as Relationship;
            if (r == null) return Visibility.Visible;
            if (r.TargetUserIsPrivate) return Visibility.Collapsed;

            string param = parameter as string;
            switch (param)
            {
                case "FOLLOW":
                    switch (r.OutgoingStatus)
                    {
                        case OutgoingStatus.Follows:
                        case OutgoingStatus.Requested:
                            return Visibility.Collapsed;

                        case OutgoingStatus.None:
                            return Visibility.Visible;

                        default:
                            break;
                    }
                    break;

                case "FOLLOWUn":
                    switch (r.OutgoingStatus)
                    {
                        case OutgoingStatus.Follows:
                        case OutgoingStatus.Requested:
                            return Visibility.Visible;

                        case OutgoingStatus.None:
                            return Visibility.Collapsed;

                        default:
                            break;
                    }
                    break;

                case "FOLLOWAccept":
                    switch (r.IncomingStatus)
                    {
                        case IncomingStatus.None:
                        case IncomingStatus.BlockedbyYou:
                        case IncomingStatus.FollowedBy:
                            return Visibility.Collapsed;

                        case IncomingStatus.RequestedBy:
                            return Visibility.Visible;

                        default:
                            break;
                    }
                    break;

                case "FOLLOWBlock":
                    return Visibility.Visible;

                case "FOLLOWUnBlock":
                    if (r.IncomingStatus == IncomingStatus.BlockedbyYou) return Visibility.Visible;
                    else return Visibility.Collapsed;

                default:
                    break;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
