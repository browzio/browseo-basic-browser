using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Organiser.Common.Converters
{
    public class StringTomageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string link = System.Convert.ToString(value);
            link = link.Replace("&amp;", "&");
            link = link.Replace("amp;", "");
            //BitmapImage img = new BitmapImage(new Uri(link), new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore));
            //img.CacheOption = BitmapCacheOption.None;
            //ImageSource image = img;
            //return image;

            var image = new BitmapImage();
            int BytesToRead = 100;
            byte[] bytebuffer = new byte[BytesToRead];

            WebRequest request = WebRequest.Create(new Uri(link, UriKind.Absolute));
            request.Timeout = -1;

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (BinaryReader reader = new BinaryReader(responseStream))
                    {
                        int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            while (bytesRead > 0)
                            {
                                memoryStream.Write(bytebuffer, 0, bytesRead);
                                bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                            }

                            image.BeginInit();
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            image.StreamSource = memoryStream;
                            image.EndInit();
                        }
                    }
                }
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
