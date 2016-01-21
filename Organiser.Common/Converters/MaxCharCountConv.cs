using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Organiser.Common.Converters
{
    public class MaxCharCountConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (text != null)
            {
                int maxLen = System.Convert.ToInt32(parameter);

                if (text.Length > maxLen)
                    text = text.Substring(0, maxLen);

                return text;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
