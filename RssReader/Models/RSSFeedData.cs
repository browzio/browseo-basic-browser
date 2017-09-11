using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
using RssReader.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Threading;

namespace RssReader.Models
{
    public class RSSFeedData : PropertyChangedViewModelBase,
        IHaveSocialStats
    {
        private string imageLink;
        public string ImageLink
        {
            get { return imageLink; }
            set
            {
                imageLink = value;
                if (!imageLink.IsNullOrEmpty() && !imageLink.StartsWith("http"))
                {
                    var linkuri = new Uri(link);

                    imageLink = "http://" + linkuri.Authority + imageLink;

                    //if (imageLink.StartsWith("//")) imageLink = "http:" + imageLink;
                    //else if(imageLink.StartsWith("/")) imageLink = "http:/" + imageLink;
                    //else imageLink = "http://" + imageLink;
                }
                NotifyOfPropertyChange();
            }
        }

        private BitmapImage bitmapImage;
        public BitmapImage BitmapImage
        {
            get { return bitmapImage; }
            set { bitmapImage = value;
                NotifyOfPropertyChange();
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; NotifyOfPropertyChange(); }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set { link = value; NotifyOfPropertyChange(); }
        }

        private string date;
        public string Date
        {
            get { return date; }
            set { date = value; NotifyOfPropertyChange(); }
        }
        
        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; NotifyOfPropertyChange(); }
        }

        private SocialStatsReplys socialStats;
        public SocialStatsReplys SocialStatsReplys
        {
            get { return socialStats; }
            set { socialStats = value; NotifyOfPropertyChange(); }
        }

        public RSSFeedData()
        {
            SocialStatsReplys = new SocialStatsReplys();
        }

        public override void OnReceivedCommandFromView(string param)
        {
            RSSMainWorkspaceViewModel.Instance.OnClickedOpenSocialShareLink(param, Link, ImageLink);
        }

        //public async Task LoadBitmapImg(CancellationToken token)
        //{
        //    if (imageLink.IsNullOrEmpty()) return;

        //    BitmapImage = null;

        //    var imgBitmapImage = new BitmapImage();
        //    int BytesToRead = 100;
        //    byte[] bytebuffer = new byte[BytesToRead];
        //    HttpWebResponse response = null;
        //    Stream responseStream = null;
        //    MemoryStream memoryStream = null;
        //    try
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create(imageLink);
        //        request.Timeout = -1;
        //        request.KeepAlive = false;
        //        request.UserAgent = BrowserSettimgs.UserAgentFF;
        //        request.Proxy = MyFilesDatabase.GetRequestsProxy();

        //        using (token.Register(() => request.Abort(), true))
        //        {
        //            response = (HttpWebResponse)await request.GetResponseAsync();
        //            if (response.StatusCode != HttpStatusCode.OK) return;

        //            responseStream = response.GetResponseStream();
        //            memoryStream = new MemoryStream();
        //            int bytesRead = await responseStream.ReadAsync(bytebuffer, 0, BytesToRead);
        //            while (bytesRead > 0)
        //            {
        //                memoryStream.Write(bytebuffer, 0, bytesRead);
        //                bytesRead = await responseStream.ReadAsync(bytebuffer, 0, BytesToRead);
        //            }
        //        }

        //        var image = new BitmapImage();
        //        image.BeginInit();
        //        memoryStream.Seek(0, SeekOrigin.Begin);

        //        image.StreamSource = memoryStream;
        //        image.EndInit();

        //        BitmapImage = image;
        //        BitmapImage.DownloadCompleted += (s, e) =>
        //        {
        //            response.Dispose();
        //            responseStream.Dispose();
        //            memoryStream.Dispose();
        //        };
        //    }
        //    catch(Exception ex)
        //    {
        //        BitmapImage = null;

        //        if (response != null) response.Dispose();
        //        if (responseStream != null) responseStream.Dispose();
        //        if (memoryStream != null) memoryStream.Dispose();
        //    }
        //}
    }
}
