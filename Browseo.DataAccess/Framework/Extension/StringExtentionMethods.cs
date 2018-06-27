using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.Browser.Framework.Extension
{
    public static class StringExtentionMethods
    {
        public static bool IsNullOrSpace(this string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }
    }
}
