using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;

namespace Browser.Common.Convertrs
{
    public class RichToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FlowDocument fd = new FlowDocument();

            Paragraph para = new Paragraph();

            Hyperlink link = new Hyperlink();
            link.IsEnabled = true;
            link.Inlines.Add("");
            link.NavigateUri = new Uri("");
            // link.RequestNavigate += (sender, args) => Process.Start(args.Uri.ToString());

            para.Inlines.Add("TextBeforeLink");
            para.Inlines.Add(link);
            para.Inlines.Add("TextAfterLink");

            fd.Blocks.Add(para);

            return fd;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
