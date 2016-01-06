using BrowserHost.Models;
using BrowserHost.Windows;
using Organiser.Common.Classes;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xilium.CefGlue.Client;

namespace BrowserHost.ViewModels
{
    public class PinterestImagePickerVM
    {
        public event Action<string> OnLaunchSharePopup = delegate { };

        private ObservableCollection<WebPageImg> webPageImages;
        public ObservableCollection<WebPageImg> WebPageImages
        {
            get { return webPageImages; }
            set { webPageImages = value; }
        }
        private int sLImageLink;
        public int SLImageLink
        {
            get { return sLImageLink; }
            set { sLImageLink = value; }
        }

        public SourceVisitor Visitor { get; private set; }

        Thread loadImgThread;

        public PinterestImagePickerVM()
        {
            WebPageImages = new ObservableCollection<WebPageImg>();    
        }

        public void VisitSource(string AddressEditable)
        {
            if (loadImgThread != null) return;

            Visitor = new SourceVisitor(text =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    loadImgThread = new Thread(() =>
                    {
                        WebPageImages.Clear();
                        try
                        {
                            foreach (Match m in Regex.Matches(text, "<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                            {
                                // if (WebPageImages.Count > 10) break;

                                try
                                {
                                    string src = m.Groups[1].Value;
                                    if (src.ToLower().Contains(".png") || src.ToLower().Contains(".jpg") || src.ToLower().Contains(".jpeg"))
                                    {
                                        if (!src.Contains("http") && !src.Contains("https"))
                                        {
                                            src = src.Replace("//", "");
                                            src = "http://" + src;
                                        }

                                        WebPageImages.Add(new WebPageImg()
                                        {
                                            ImgUrl = src,
                                            WebUrl = AddressEditable
                                        });
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }

                        if (WebPageImages.Count > 0)
                        {
                            ChoosePinterestImageWindow cpiw = new ChoosePinterestImageWindow();
                            cpiw.DataContext = this;
                            cpiw.ShowDialog();
                            string link = Social.SHARELINK_pintrest + AddressEditable + "&media=" + WebPageImages[SLImageLink].ImgUrl;
                            WebPageImages.Clear();
                            WebPageImages = null;
                            if (cpiw.OkClicked)
                            {
                                if (SLImageLink >= 0)
                                {
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        OnLaunchSharePopup(link);
                                    }));
                                }
                            }

                            GC.Collect();
                        }
                        else
                        {
                            MessageBox.Show("No shareable images found on page.");
                        }
                    });

                    loadImgThread.SetApartmentState(ApartmentState.STA);
                    loadImgThread.Start();
                }));
            });
        }  
    } 
}
