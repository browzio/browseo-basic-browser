using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GoViral.Converters
{
    public class PhotosVidsCountToTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values.Length == 5)
                {
                    string pageurl = values[0] as string;
                    if(pageurl == null) return "Search For More...";
                    if (pageurl.Contains(Social.FACEBOOK_EVENTS_DEFAULT_URL))
                    {
                        return "";
                    }
                    else if (pageurl.Contains(Social.FACEBOOK_GROUPS_DEFAULT_URL))
                    {
                        return "Crawl It...";
                    }
                    else if (pageurl.Contains(Social.FACEBOOK_USERS_DEFAULT_URL))
                    {
                        return "";
                    }
                    else
                    {
                        if (values[2] != System.Windows.DependencyProperty.UnsetValue)
                        {
                            System.Collections.ObjectModel.ObservableCollection<Organiser.Common.Classes.Photos.Photo> photoArr = values[1] as System.Collections.ObjectModel.ObservableCollection<Organiser.Common.Classes.Photos.Photo>;
                            System.Collections.ObjectModel.ObservableCollection<Organiser.Common.Classes.Videos.Video> VidArr = values[1] as System.Collections.ObjectModel.ObservableCollection<Organiser.Common.Classes.Videos.Video>;

                            int count = System.Convert.ToInt32(values[2]);

                            Organiser.Common.Classes.Paging paging = values[3] as Organiser.Common.Classes.Paging;

                            if (photoArr != null && count > 0 && paging != null)
                                return "";
                            if (VidArr != null && count > 0 && paging != null)
                                return "";
                        }
                    }
                }
            }
            catch { }
            return "Search For More...";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { };
        }
    }
}
