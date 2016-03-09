using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Organiser.Common.Converters
{
    public class EmptyListToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            //if (value == null && param != "NOT") return Visibility.Visible;

            ICollection l = value as ICollection;

            if (param == "NOT")
                return l != null && l.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return l == null || l.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
