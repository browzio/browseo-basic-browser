using InstaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GoViral.Converters
{
    public class RelationshipToImageConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Relationship r = value as Relationship;
            if(r==null || r.TargetUserIsPrivate) return new BitmapImage(new Uri("pack://application:,,,/GoViral;component/Images/ic_person_black_24dp_2x.png"));

            BitmapImage mbp = new BitmapImage(new Uri("pack://application:,,,/GoViral;component/Images/ic_person_add_black_24dp_2x.png"));
            switch (r.IncomingStatus)
            {
                case IncomingStatus.FollowedBy:
                    mbp = new BitmapImage(new Uri("pack://application:,,,/GoViral;component/Images/ic_person_black_24dp_2x.png"));
                    break;
                case IncomingStatus.RequestedBy:
                    break;
                case IncomingStatus.BlockedbyYou:
                    mbp = new BitmapImage(new Uri("pack://application:,,,/GoViral;component/Images/ic_sentiment_very_dissatisfied_black_24dp_2x.png"));
                    break;
                case IncomingStatus.None:
                    break;
                default:
                    break;
            }
            switch (r.OutgoingStatus)
            {
                case OutgoingStatus.Follows:
                    break;
                case OutgoingStatus.Requested:
                    break;
                case OutgoingStatus.None:
                    break;
                default:
                    break;
            }
            return mbp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
