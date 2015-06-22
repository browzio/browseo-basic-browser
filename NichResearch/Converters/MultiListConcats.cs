using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NichResearch.Converters
{
    public class MultiListConcats : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string result = "";
                foreach (string item in values)
                {
                    result += item + System.Environment.NewLine;
                }
                return result;
            }
            catch
            {
                return "";
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
