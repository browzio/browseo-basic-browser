using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using Organiser.Common.Classes;

namespace Organiser.Common.Converters
{
    public class TextEmptyToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //var list = value as IList;
            //if (list == null || list.Count == 0)
            //    return Visibility.Collapsed;
            try
            {
                var text = value as string;
                var paramtext = parameter as string;
                if (text.IsNullOrEmpty())
                {
                    if (paramtext.IsNullOrEmpty()) return Visibility.Collapsed;
                    else return "";
                }

                if (paramtext.IsNullOrEmpty()) return Visibility.Visible;
                else return paramtext;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
