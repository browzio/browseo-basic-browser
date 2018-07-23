using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BrowseoFX_WPF.Core.DataAccess
{
    public static class StringExtentions
    {
        public static string ToLowerString(this bool val)
        {
            return val.ToString().ToLower();
        }

        public static bool IsNullOrSpace(this string val)
        {
            return string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val);
        }
    }

    public static class EnumExtensions
    {
        public static string GetDescription(this PanelUIListenerState val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
