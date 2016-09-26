using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoViral.Controls
{
    /// <summary>
    /// Interaction logic for SelectedPageInfoUserControl.xaml
    /// </summary>
    public partial class SelectedPageInfoUserControl : UserControl
    {
        public event Action<string, string> AddUrlToPostsSync = delegate { };
        public event Action<string> AddUrlForMulti = delegate { };
        public event Action<string> OnNavigateToUrl = delegate { };

        public SelectedPageInfoUserControl()
        {
            InitializeComponent();
        }

        #region scroll
        private ScrollViewer mouseWithinChildScroll;
        private void spSelectedFBData_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (mouseWithinChildScroll == null)
            {
                MyScrollViewerFBContent.ScrollToVerticalOffset(MyScrollViewerFBContent.VerticalOffset - e.Delta);
            }
        }

        private void lvVideos_MouseEnter(object sender, MouseEventArgs e)
        {
            Decorator border = VisualTreeHelper.GetChild((sender as ListView), 0) as Decorator;
            mouseWithinChildScroll = border.Child as ScrollViewer;
        }

        private void lvVideos_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseWithinChildScroll = null;
        }

        private void lvVideos_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (mouseWithinChildScroll != null)
            {
                mouseWithinChildScroll.ScrollToHorizontalOffset(mouseWithinChildScroll.ContentHorizontalOffset - e.Delta);
            }
        }
        #endregion

        private void fbPostImages_Click(object sender, RoutedEventArgs e)
        {
            string full_picture = Convert.ToString((sender as MenuItem).Tag);
            if (string.IsNullOrEmpty(full_picture) || string.IsNullOrWhiteSpace(full_picture)) return;
            full_picture = full_picture.Replace("&amp;", "&");
            (this.DataContext as ViewModels.GoViralVM).BeginImageDownload(full_picture);
        }

        string linkTextForCopy = "";
        private void tbPostLink_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            string url = Convert.ToString((sender as TextBlock).Text);
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) return;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                linkTextForCopy = url;
                return;
            }


            OnNavigateToUrl(url);
        }

        private void miCopyLink_Click(object sender, RoutedEventArgs e)
        {
            string url = Convert.ToString((sender as MenuItem).Tag);
            string header = Convert.ToString((sender as MenuItem).Header);
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                url = linkTextForCopy;
                if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
                    return;
            }
            if (header == "Copy")
            {
                MyFilesDatabase.SetClipboardText(url);
            }
            else if (header == "Sync")
            {
                string pageName = "";
                if ((this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder != null)
                {
                    if ((this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder.SelectedPage != null)
                    {
                        pageName = (this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder.SelectedPage.Name;
                    }
                }
                AddUrlToPostsSync(pageName, url);
            }
        }

        private void imgVideo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                try
                {
                    string link = Convert.ToString((sender as Image).Tag);
                    link = link.Replace("&amp;", "&");
                    link = link.Replace("amp;", "");
                    if (!link.Contains("https://www.facebook.com"))
                        link = "https://www.facebook.com" + link;
                    OnNavigateToUrl(link);
                }
                catch { }
            }
        }

        private void miVids_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            string tag = Convert.ToString(mi.Tag);
            if (string.IsNullOrEmpty(tag) || string.IsNullOrWhiteSpace(tag)) return;

            switch (mi.Name)
            {
                case "Copy":
                    if (!tag.Contains("https://www.facebook.com"))
                    {
                        tag = "https://www.facebook.com" + tag;
                    }
                    tag = tag.Replace("&amp;", "&");
                    tag = tag.Replace("amp;", "");
                    MyFilesDatabase.SetClipboardText(tag);
                    break;

                case "Download":
                    try
                    {
                        string link = (mi.DataContext as Videos.Video).source;
                        link = link.Replace("&amp;", "&");
                        link = link.Replace("amp;", "");
                        (DataContext as ViewModels.GoViralVM).WebBrowser.GetBrowser().GetHost().StartDownload(link);
                        //(DataContext as ViewModels.GoViralVM).WebBrowser.Navigate(link);
                    }
                    catch { MessageBox.Show("No video download link found."); }
                    break;

                default:
                    break;
            }
        }

        private void miPhotosPhotoDownload(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            Images[] images = mi.Tag as Images[];
            string link = "";

            if (images != null)
            {
                int lastImageSize = 0;
                foreach (Images img in images)
                {
                    if ((img.height + img.width) > lastImageSize)
                    {
                        link = img.source;
                        lastImageSize = img.height + img.width;
                    }

                }
            }
            else
            {
                Photos.Photo pic = (mi.DataContext as Photos.Photo);
                if (pic != null)
                {
                    link = pic.picture;
                }
            }

            if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link))
            {
                MessageBox.Show("No image link found to download.");
                return;
            }

            link = link.Replace("&amp;", "&");
            link = link.Replace("amp;", "");
            //if (link.Contains("?"))
            //{
            //    link = link.Split('?')[0];
            //}
            (this.DataContext as ViewModels.GoViralVM).BeginImageDownload(link);
        }

        private void tbLoadMorePhotos_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            string text = (sender as TextBlock).Text;
            Models.Folder folder = (sender as TextBlock).Tag as Models.Folder;
            Models.ListOption option = folder.SelectedPage;
            if (folder == null || option == null) return;
            (this.DataContext as ViewModels.GoViralVM).BeginAllPhotosScrape(folder, option, !text.Contains("Crawl"));
        }

        private void tbLoadMoreVideos_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            string text = (sender as TextBlock).Text;
            Models.Folder folder = (sender as TextBlock).Tag as Models.Folder;
            Models.ListOption option = folder.SelectedPage;
            if (folder == null || option == null) return;
            (this.DataContext as ViewModels.GoViralVM).BeginAllVideosScrape(folder, option, !text.Contains("Crawl"));
        }

        private void cbSync_Click(object sender, RoutedEventArgs e)
        {
            string link = Convert.ToString((sender as Button).Tag);
            if (link == null) return;

            string pageName = "";
            if ((this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder != null)
            {
                if ((this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder.SelectedPage != null)
                {
                    pageName = (this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder.SelectedPage.Name;
                }
            }
            AddUrlForMulti(pageName + " , " + link + Environment.NewLine);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModels.GoViralVM).On_CTMenuClick((sender as MenuItem).Name);
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            Button thisButton = sender as Button;
            if (thisButton == null) return;

            string tag = thisButton.Tag as string;
            if (string.IsNullOrEmpty(tag) || string.IsNullOrWhiteSpace(tag)) return;
            tag = tag.Replace("&amp;","&");
            tag = tag.Replace("amp;","");
            if (tag.Contains(".mp4"))
            {
                //(DataContext as ViewModels.GoViralVM).WebBrowser.Navigate(tag);
                (DataContext as ViewModels.GoViralVM).WebBrowser.GetBrowser().GetHost().StartDownload(tag);
            }
            else
            {
                (DataContext as ViewModels.GoViralVM).BeginImageDownload(tag);
            }
        }
    }
}
