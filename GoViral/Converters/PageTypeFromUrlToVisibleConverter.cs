using Organiser.Common.Classes;
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
    public class PageTypeFromUrlToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string pageurl = value as string;

            if (pageurl.Contains(Social.FACEBOOK_EVENTS_DEFAULT_URL))
            {
                return Visibility.Collapsed;
            }
            else if (pageurl.Contains(Social.FACEBOOK_GROUPS_DEFAULT_URL))
            {
               return Visibility.Visible;
            }
            else if (pageurl.Contains(Social.FACEBOOK_USERS_DEFAULT_URL))
            {
                return Visibility.Collapsed;
            }
            else if (pageurl.Contains(Social.FACEBOOK_PHOTOS_DEFAULT_URL))
            {
                return Visibility.Collapsed;
            }
            else if (pageurl.Contains(Social.FACEBOOK_VIDEOS_DEFAULT_URL))
            {
                return Visibility.Collapsed;
            }
            else if (pageurl.Contains(Social.FACEBOOK_PLACES_DEFAULT_URL))
            {
                return Visibility.Visible;
            }
            else if (pageurl.Contains(Social.FACEBOOK_PAGES_DEFAULT_URL))
            {
                return Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
