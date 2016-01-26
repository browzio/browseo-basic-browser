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
    public class PostIdToUrlConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string paID_poID = values[0] as string; //pagid_postid
                if (paID_poID != null)
                {
                    string pageID = paID_poID.Split('_')[0];
                    string id = paID_poID.Split('_')[1];

                    string pageurl = values[1] as string;//page url
                    if(pageurl != null)
                    {
                        string pageNid = pageID + "/" + id;
                        string postlink = "https://www.facebook.com/" + pageID + "/posts/" + id;

                        if (pageurl.Contains(Social.FACEBOOK_EVENTS_DEFAULT_URL))
                        {
                            postlink = Social.FACEBOOK_EVENTS_DEFAULT_URL + pageNid;
                        }
                        else if (pageurl.Contains(Social.FACEBOOK_GROUPS_DEFAULT_URL))
                        {
                            postlink = Social.FACEBOOK_GROUPS_DEFAULT_URL + pageNid;
                        }
                        else if (pageurl.Contains(Social.FACEBOOK_USERS_DEFAULT_URL))
                        {
                            postlink = Social.FACEBOOK_USERS_DEFAULT_URL + pageNid;
                        }

                        return postlink;
                    }
                }
            }
            catch { }

            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
