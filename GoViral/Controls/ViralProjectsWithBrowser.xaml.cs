using Organiser.Common.Classes;
using Organiser.Common.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ProjectsWithBrowser.xaml
    /// </summary>
    public partial class ProjectsWithBrowser : UserControl
    {
        public ProjectsWithBrowser()
        {
            InitializeComponent();
            ucSyncedPosts.OnBrowserNavigateToUrl += UcSyncedPosts_OnBrowserNavigateToUrl;
        }

        private void UcSyncedPosts_OnBrowserNavigateToUrl(string url)
        {
            tbCntrl.SelectedIndex = 0;
            (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(url);
        }

        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                try
                {
                    (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(((sender as Grid).DataContext as GoViral.Models.ListOption).Url);
                }
                catch
                {

                }
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
                Models.ListOption loToFind = ((sender as TextBox).DataContext as Models.ListOption);
                if (loToFind == null) return;
                Models.Folder nextFolder = vm.Folders.SingleOrDefault(f => f.SavedLinksList.SingleOrDefault(lo => lo == loToFind) != null);
                if(nextFolder != null)
                {
                    vm.SIFolders = vm.Folders.IndexOf(nextFolder);
                    nextFolder.SISavedLinks = nextFolder.SavedLinksList.IndexOf(loToFind);    
                }
                //


                //if(vm.SelectedFolder != null)
                //{
                //    vm.SIFolders = vm.Folders.IndexOf(vm.SelectedFolder);
                //    vm.SelectedFolder.SISavedLinks = vm.SelectedFolder.SavedLinksList.IndexOf(loToFind);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region scrolling

        private void lbFolders_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Decorator border = VisualTreeHelper.GetChild((sender as ListView), 0) as Decorator;
            if (border == null) return;
            ScrollViewer sv = border.Child as ScrollViewer;
            if (sv == null) return;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }
        //private void lbFolders_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    MyScrollViewer.ScrollToVerticalOffset(MyScrollViewer.VerticalOffset - e.Delta);
        //}




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
            
            (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(url);
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
                if((this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder != null)
                {
                    if((this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder.SelectedPage != null)
                    {
                        pageName = (this.DataContext as GoViral.ViewModels.GoViralVM).SelectedFolder.SelectedPage.Name;
                    }
                }
                ucSyncedPosts.ViewModel.AddUrlToSavedProjectList(pageName, url,null);
            }
        }

        private void imgVideo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
             if(e.ClickCount == 2)
            {
                try
                {
                    string link = Convert.ToString((sender as Image).Tag);
                    if (!link.Contains("https://www.facebook.com"))
                        link = "https://www.facebook.com" + link;
                    (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(link);
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
                        tag  = "https://www.facebook.com" + tag;
                    }
                    MyFilesDatabase.SetClipboardText(tag);
                    break;

                case "Download":
                    try
                    {
                        string link = (mi.DataContext as Videos.Video).source.Replace("&amp;", "&");
                        (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(link);
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
                if(pic != null)
                {
                    link = pic.picture;
                }
            }

            if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link))
            {
                MessageBox.Show("No image link found to download.");
                return;
            }

            if(link.Contains("&amp;"))
                link = link.Replace("&amp;", "&");
            (this.DataContext as ViewModels.GoViralVM).BeginImageDownload(link);
        }

        private void tbLoadMorePhotos_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Models.Folder folder = (sender as TextBlock).Tag as Models.Folder;
            Models.ListOption option = folder.SelectedPage;
            if (folder == null || option == null) return;
            (this.DataContext as ViewModels.GoViralVM).BeginAllPhotosScrape(folder, option);
        }

        private void tbLoadMoreVideos_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Models.Folder folder = (sender as TextBlock).Tag as Models.Folder;
            Models.ListOption option = folder.SelectedPage;
            if (folder == null || option == null) return;
            (this.DataContext as ViewModels.GoViralVM).BeginAllVideosScrape(folder, option);
        }

        List<KeyValuePair<string, string>> ToSendSyncLinks = new List<KeyValuePair<string, string>>();


        Organiser.Common.Windows.RssFeedsLinksMultiWindow multiWindowForLinksAdd;

        private void cbSync_Click(object sender, RoutedEventArgs e)
        {
            if(multiWindowForLinksAdd == null)
            {
                multiWindowForLinksAdd = new Organiser.Common.Windows.RssFeedsLinksMultiWindow();
                multiWindowForLinksAdd.Closed += MultiWindowForLinksAdd_Closed;
                multiWindowForLinksAdd.Title = "Page Name , Url";
                multiWindowForLinksAdd.Show();
            }
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

            multiWindowForLinksAdd.tbInputedText.Text += pageName + " , " + link + Environment.NewLine;
        }

        private void MultiWindowForLinksAdd_Closed(object sender, EventArgs e)
        {
            if (multiWindowForLinksAdd.OKClicked)
            {
                ucSyncedPosts.ViewModel.AddUrlToSavedProjectList("", "", multiWindowForLinksAdd.tbInputedText.Text);
                multiWindowForLinksAdd = null;
            }
        }
    }
}




//private void lbsavedList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
//{
//    //ListView lv = FindChild<ListView>(sender as ListView, "lvPosts");
//    //Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
//    //ScrollViewer scrollViewer = border.Child as ScrollViewer;

//    //int MouseX = (int)Mouse.GetPosition(lv).X;
//    //int MouseY = (int)Mouse.GetPosition(lv).Y;

//    //if (MouseY > 0 && MouseY < lv.ActualHeight && (
//    //    (scrollViewer.ContentVerticalOffset != 0 && scrollViewer.ContentVerticalOffset != scrollViewer.ScrollableHeight) ||
//    //    (scrollViewer.ContentVerticalOffset == 0 && e.Delta<0) || (scrollViewer.ContentVerticalOffset == scrollViewer.ScrollableHeight && e.Delta > 0)
//    //        ))
//    //{
//    //if (e.Delta > 0)
//    //{
//    //    if (lv.SelectedIndex < lv.Items.Count - 2)
//    //        lv.SelectedIndex += 1;
//    //}
//    //else
//    //{
//    //    if (lv.SelectedIndex > 1)
//    //        lv.SelectedIndex -= 1;
//    //}

//    //scrollViewer.ScrollToVerticalOffset(e.Delta > 0 ? scrollViewer.VerticalOffset - 1 : scrollViewer.VerticalOffset + 1);
//    //e.Handled = true;
//    //if (e.Delta > 0)
//    //{
//    //    //scrollViewer.ScrollToVerticalOffset(e.Delta);
//    //    //ScrollBar.LineDownCommand.Execute(null, scrollViewer as IInputElement);
//    //}
//    //if (e.Delta < 0)
//    //{
//    //    ScrollBar.LineUpCommand.Execute(null, scrollViewer as IInputElement);
//    //}
//    // }
//    //else
//    //{                                                       
//    //if (mouseWithinChildScroll == null   ||
//    //    (mouseWithinChildScroll.ContentVerticalOffset == 0 && e.Delta > 0) ||
//    //    (mouseWithinChildScroll.ContentVerticalOffset == mouseWithinChildScroll.ScrollableHeight && e.Delta > 0))
//    //{
//    if (mouseWithinChildScroll == null)
//    {
//        MyScrollViewer.ScrollToVerticalOffset(MyScrollViewer.VerticalOffset - e.Delta);
//    }
//    // e.Handled = true;
//    // }
//    //else
//    //    e.Handled = true;
//    //}
//}
//private void btnLoadMoreLikes_Click(object sender, RoutedEventArgs e)
//{
//    try
//    {
//        //ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
//        //ListBox foundlb = FindChild<ListBox>(this, "lbsavedList");
//        // vm.LoadAllLikes(((sender as Button).DataContext as FacebookGraphPostResult), (sender as Button).Tag.ToString());
//    }
//    catch
//    {
//        MessageBox.Show("No more likes found");
//    }
//}
//private void setCorrectSI(object sender)
//{
//    //Task.Factory.StartNew(() =>
//    //{
//    //    lock (sender)
//    //    {
//    try
//    {
//        ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
//        Models.ListOption loToFind = null;
//        if (sender is Expander)
//        {
//            loToFind = ((sender as Expander).DataContext as Models.ListOption);
//        }
//        else if (sender is TextBox)
//        {
//            loToFind = ((sender as TextBox).DataContext as Models.ListOption);
//        }
//        else if (sender is StackPanel)
//        {
//            loToFind = ((sender as StackPanel).DataContext as Models.ListOption);
//        }

//        if (loToFind == null) return;
//        Task.Factory.StartNew(() =>
//        {
//            lock (loToFind)
//            {
//                foreach (Models.Folder f in vm.Folders)
//                {
//                    Models.ListOption lo = f.SavedLinksList.SingleOrDefault(s => s == loToFind);
//                    if (lo != null)
//                    {
//                        vm.SIFolders = vm.Folders.IndexOf(f);
//                        f.SISavedLinks = f.SavedLinksList.IndexOf(lo);
//                        break;
//                    }
//                }
//            }
//        });
//    }
//    catch (Exception ex)
//    {
//    }
//    //    }
//    //});
//}
