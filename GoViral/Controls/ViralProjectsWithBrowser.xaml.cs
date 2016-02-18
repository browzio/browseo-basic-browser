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
        ViewModels.GoViralVM ViewModel;
        public ProjectsWithBrowser()
        {
            InitializeComponent();

            ViewModel = new ViewModels.GoViralVM();
            DataContext = ViewModel;
        }

        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                try
                {
                    NavigateToUrl(((sender as Grid).DataContext as GoViral.Models.ListOption).Url);
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
                //ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
                Models.ListOption loToFind = ((sender as TextBox).DataContext as Models.ListOption);
                if (loToFind == null) return;
                Models.Folder nextFolder = ViewModel.Folders.SingleOrDefault(f => f.SavedLinksList.SingleOrDefault(lo => lo == loToFind) != null);
                if(nextFolder != null)
                {
                    ViewModel.SIFolders = ViewModel.Folders.IndexOf(nextFolder);
                    nextFolder.SISavedLinks = nextFolder.SavedLinksList.IndexOf(loToFind);    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private void lbFolders_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Decorator border = VisualTreeHelper.GetChild((sender as ListView), 0) as Decorator;
            if (border == null) return;
            ScrollViewer sv = border.Child as ScrollViewer;
            if (sv == null) return;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }

        private void NavigateToUrl(string url)
        {
            if (tbCntrl.SelectedIndex != 0) tbCntrl.SelectedIndex = 0;
            if (url.Contains("/?ref=br_rs")) url = url.Replace("/?ref=br_rs", "");

            string urltillId = url.Remove(url.LastIndexOf("/"));
            string name = url.Substring(url.LastIndexOf("/") + 1);
            if (url.Contains("-"))
            {
                string id = url.Substring(url.LastIndexOf("-") + 1);
                long tryparseResult = 0;
                if (Int64.TryParse(id, out tryparseResult))
                {
                    url = "https://www.facebook.com/" + id;
                }
                else
                {
                    url = "https://www.facebook.com/" + name;
                }
            }
            ViewModel.WebBrowser.Navigate(url);
        }


        #region ucSelectedPageInfo
        private void selectedPageInfoUC_AddUrlToPostsSync(string pageName, string url)
        {
            ucSyncedPosts.ViewModel.AddUrlToSavedProjectList(pageName, url, null);
        }

        private void ucSelectedPageInfo_AddUrlForMulti(string text)
        {
            if (multiWindowForLinksAdd == null)
            {
                multiWindowForLinksAdd = new Organiser.Common.Windows.RssFeedsLinksMultiWindow();
                multiWindowForLinksAdd.Closed += MultiWindowForLinksAdd_Closed;
                multiWindowForLinksAdd.Title = "Page Name , Url";
                multiWindowForLinksAdd.Show();
            }
            text = text.RemoveAmps();
            multiWindowForLinksAdd.tbInputedText.Text += text;
        }

        private void ucSelectedPageInfo_OnNavigateToUrl(string url)
        {
            NavigateToUrl(url);
        }
        #endregion


        #region ucSyncedPosts
       // List<KeyValuePair<string, string>> ToSendSyncLinks = new List<KeyValuePair<string, string>>();


        Organiser.Common.Windows.RssFeedsLinksMultiWindow multiWindowForLinksAdd;

        private void MultiWindowForLinksAdd_Closed(object sender, EventArgs e)
        {
            if (multiWindowForLinksAdd.OKClicked)
            {
                ucSyncedPosts.ViewModel.AddUrlToSavedProjectList("", "", multiWindowForLinksAdd.tbInputedText.Text);
                multiWindowForLinksAdd = null;
            }
        }

        private void UcSyncedPosts_OnBrowserNavigateToUrl(string url)
        {
            NavigateToUrl(url);
        }
        #endregion



        #region ucSearch
        private void ucSearch_OnOpenInBrowserRequested(string url)
        {
            NavigateToUrl(url);
        }

        private void ucSearch_OnStoreForDominationRequested(string link, List<string> multi)
        {
            ViewModel.AsyncAddLinkToList(link,"", multi, showLinksWindow: false);
        }

        private void ucSearch_OnOpenInBrowserForDownloadRequested(string source)
        {
            ViewModel.WebBrowser.CBrowser.Browser.GetHost().StartDownload(source);
            // (this.DataContext as ViewModels.GoViralVM).WebBrowser.Navigate(source);
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ucSyncedPosts.ViewModel == null)
            {
                ucSyncedPosts.ViewModel = new ViewModels.SyncedProjectsVM(ViewModels.SyncedProjectsVM.TypeOfGoViral);
                ucSyncedPosts.DataContext = ucSyncedPosts.ViewModel;
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
