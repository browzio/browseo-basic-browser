using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Organiser.Common.Converters
{
    public class EmptyTextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is long)
            {
                if (System.Convert.ToInt64(value) == 0) return Visibility.Collapsed;
                return Visibility.Visible;
            }
            if (value is int)
            {
                if (System.Convert.ToInt32(value) == 0) return Visibility.Collapsed;
                return Visibility.Visible;
            }
            string text = value as string;

            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrEmpty(text))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
