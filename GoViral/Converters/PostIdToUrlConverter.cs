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
            string link = "";

            if (values != null && values.Length == 2)
            {
                try
                {
                    string id = System.Convert.ToString(values[0]);
                    id = id.Split('_')[1];
                    string pageurl = System.Convert.ToString(values[1]);
                    link = pageurl + "posts/" + id;
                }
                catch { }
            }

            return link;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
