using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_WYSIWYG_HTML_Editor.Helpers
{
    public static class Spinner
    {
        private readonly static Random Randomizer = new Random();

        public static string Spin(string content)
        {
            const char OPEN_BRACE = '{';
            const char CLOSE_BRACE = '}';
            const char DELIMITER = '|';

            var start = content.IndexOf(OPEN_BRACE);
            var end = content.IndexOf(CLOSE_BRACE);

            if (start == -1 && end == -1 || start == -1 || end < start)
            {
                return content;
            }

            if (end == -1)
            {
                throw new ArgumentException("Unbalanced brace. Missing closing brace.");
            }

            var substring = content.Substring(start + 1, content.Length - (start + 1));
            var rest = Spin(substring);
            end = rest.IndexOf(CLOSE_BRACE);

            if (end == -1)
            {
                // var error = substring.Length > 25 ? substring.Substring(substring.Length - 20) : substring;
                //var error = substring.Length > 25 ? substring.Remove(substring.Length - 20) : substring;
                throw new ArgumentException("Unbalanced brace. In chunk " + Environment.NewLine + substring);
            }

            var splits = rest.Substring(0, end).Split(DELIMITER);
            var item = splits[Randomizer.Next(0, splits.Length)];
            return content.Substring(0, start) + item + Spin(rest.Substring(end + 1, rest.Length - (end + 1)));
        }
    }  
}
