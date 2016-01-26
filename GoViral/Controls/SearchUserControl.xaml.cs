using GoViral.ViewModels;
using GoViral.Windows;
using Organiser.Common.Classes;
using Organiser.Common.Classes.Facebook;
using Organiser.Common.Controlls;
using Organiser.Common.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace GoViral.Controls
{
    /// <summary>
    /// Interaction logic for SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        public event Action<string> OnOpenInBrowserRequested = delegate { };//url
        public event Action<string> OnOpenInBrowserForDownloadRequested = delegate { };//url
        public event Action<string, List<string>> OnStoreForDominationRequested = delegate { };//url,all urls to store

        public SearchVM ViewModel { get; set; }

        public SearchUserControl()
        {
            InitializeComponent();

            if(ViewModel == null)
            {
                ViewModel = new SearchVM();
                this.DataContext = ViewModel;
            }
        }

        #region ui

        private void miCollpse_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var listView = getCurrentSelectedListViewFromMI(menuItem);
            if (listView == null) return;

            var expander = listView.Parent as Organiser.Common.Controlls.ExpanderDoubleClickExpand;
            if (expander == null)return;

            expander.SetExpandedValue(false);
        }
        
        private ListView getCurrentSelectedListViewFromMI(MenuItem mi)
        {
            var contextMenu = mi.Parent as ContextMenu;
            if (contextMenu != null)
            {
                return contextMenu.PlacementTarget as ListView;
            }

            return null;
        }



        ListView lvInnerCurrent = null;

        private void LVOuterData_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListView lv = sender as ListView;
            ScrollViewer sv = getScrollViewerFromLV(lv);

            if (sv.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden || sv.ScrollableHeight == 0)
            {
                e.Handled = true;

                if (lvInnerCurrent != null)
                {
                    if (e.Delta < 0)
                    {
                        if (lvInnerCurrent.SelectedIndex < lvInnerCurrent.Items.Count - 1)
                            lvInnerCurrent.SelectedIndex += 1;
                    }
                    else
                    {
                        if (lvInnerCurrent.SelectedIndex > 0)
                            lvInnerCurrent.SelectedIndex -= 1;
                    }

                    lvInnerCurrent.ScrollIntoView(lvInnerCurrent.SelectedItem);
                    //ScrollViewer svInner = getScrollViewerFromLV(lvInnerCurrent);
                    //svInner.ScrollToVerticalOffset(svInner.VerticalOffset - e.Delta);
                }
                else
                {
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    var parent = ((Control)sender).Parent as UIElement;
                    parent.RaiseEvent(eventArg);
                }
            }
        }

        private void LVOuterData_ScrollChanged(object sender, RoutedEventArgs e)
        {
            if (lvInnerCurrent != null)
            {
                e.Handled = true;
                ListView lv = sender as ListView;
                lv.ScrollIntoView(lv.SelectedItem);
            }
        }

        private void LVOuterData_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = getScrollViewerFromLV(sender as ListView);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            lvInnerCurrent = null;
        }


        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander ex = sender as Expander;
            ListView lvParent = getLVparentByExpanderName(ex.Name);

            Models.SearchResult selectedSR = ex.DataContext as Models.SearchResult;
            lvParent.SelectedIndex = ViewModel.SearchResultsWithKwList.SearchResultsList.IndexOf(selectedSR);
            scrollExpanderIntoView(lvParent, selectedSR);

            ScrollViewer sv = getScrollViewerFromLV(lvParent);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            lvInnerCurrent = ex.Content as ListView;
            if (lvInnerCurrent.SelectedIndex == -1) lvInnerCurrent.SelectedIndex = 0;

           // lvPreview.ItemTemplate.Template = lvInnerCurrent.ItemTemplate.Template;
           // lvPreview.Items.Refresh();

            //string lvXaml = XamlWriter.Save(lvInnerCurrent);
            //StringReader stringReader = new StringReader(lvXaml);
            //XmlReader xmlReader = XmlReader.Create(stringReader);
            //lvPreview = (ListView)XamlReader.Load(xmlReader);
            //cpSelectedItemPreview.Content = (ListView)XamlReader.Load(xmlReader);
            //cpSelectedItemPreview. = lvInnerCurrent.Template;
            //ccPreviewSelectedItem.DataContext = lvInnerCurrent.DataContext;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander ex = sender as Expander;
            ListView lvParent = getLVparentByExpanderName(ex.Name);
            ScrollViewer sv = getScrollViewerFromLV(lvParent);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void Expander_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Expander ex = sender as Expander;
            ListView lvParent = getLVparentByExpanderName(ex.Name);
            ScrollViewer sv = getScrollViewerFromLV(lvParent);
            ListView lvInner = (sender as Expander).Content as ListView;
            if (lvInnerCurrent == null && !(sender as Expander).IsExpanded) sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            else lvInnerCurrent = lvInner;

            if (lvInner != null && lvPreview.ItemsSource != lvInner.ItemsSource)
            {
                lvPreview.ItemsSource = lvInner.ItemsSource;
                lvPreview.ItemTemplate = lvInner.ItemTemplate;
                lvPreview.ContextMenu = new ContextMenu();
                if (lvInner.ContextMenu.Items.Count > 0)
                {
                    foreach (var mi in lvInner.ContextMenu.Items)
                    {
                        object miToAdd = null;

                        if (mi is MenuItem)
                        {
                            MenuItem miInner = mi as MenuItem;
                            if (miInner.Name != "miCollpse")
                            {
                                MenuItem miPreview = new MenuItem();
                                miPreview.Name = miInner.Name;
                                miPreview.Header = miInner.Header;
                                miPreview.Click += CMMIinner_Click;
                                miToAdd = miPreview;
                            }
                        }
                        else if(mi is Separator)
                        {
                            Separator sInner = mi as Separator;
                            Separator sPrevirew = new Separator();
                            sPrevirew.Margin = sInner.Margin;
                            miToAdd = sPrevirew;
                        }

                        if(miToAdd != null) lvPreview.ContextMenu.Items.Add(miToAdd);
                    }
                }
            }
        }

        private ListView getLVparentByExpanderName(string name)
        {
            switch (name)
            {
                case "exPages":
                    return lvDataPages;

                case "exGroups":
                    return lvDataGroups;

                case "exEvents":
                    return lvDataEvents;

                case "exPlaces":
                    return lvDataPlaces;

                case "exPhotos":
                    return lvDataPhotos;

                case "exVideos":
                    return lvDataVideos;

                case "exPeople":
                    return lvDataPeople;

                default:
                    break;
            }

            return null;
        }

        private ScrollViewer getScrollViewerFromLV(object sender)
        {
            ListView lv = sender as ListView;
            if (lv == null) return null;
            Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
            if (border == null) return null;
            ScrollViewer sv = border.Child as ScrollViewer;
            if (sv == null) return null;
            return sv;
        }

        private void scrollExpanderIntoView(ListView sender, Models.SearchResult selectedSR)
        {
            ScrollViewer sv = getScrollViewerFromLV(sender);

            sv.ScrollToTop();
            sender.ScrollIntoView(selectedSR);
        }





        private void lvPreview_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (lvPreview.SelectedIndex == -1) lvPreview.SelectedIndex = 0;

            if (e.Delta < 0)
            {
                if (lvPreview.SelectedIndex < lvPreview.Items.Count - 1)
                    lvPreview.SelectedIndex += 1;
            }
            else
            {
                if (lvPreview.SelectedIndex > 0)
                    lvPreview.SelectedIndex -= 1;
            }

            lvPreview.ScrollIntoView(lvPreview.SelectedItem);
        }

        #endregion


        #region send to vm / dominator

        #region multi links window
        RssFeedsLinksMultiWindow multiWindowForLinksAdd;
        private void addLinkToMultiWindow(string link)
        {
            if (multiWindowForLinksAdd == null)
            {
                multiWindowForLinksAdd = new RssFeedsLinksMultiWindow();
                multiWindowForLinksAdd.Title = "One Link On A Line";
                multiWindowForLinksAdd.Closed += MultiWindowForLinksAdd_Closed;
                multiWindowForLinksAdd.Show();
            }

            multiWindowForLinksAdd.tbInputedText.Text += link + Environment.NewLine;
        }

        private void MultiWindowForLinksAdd_Closed(object sender, EventArgs e)
        {
            // OnStoreForDominationRequested(link, null); if one
            // OnStoreForDominationRequested(null, likslist); if multi
            if (multiWindowForLinksAdd.OKClicked &&
                !string.IsNullOrEmpty(multiWindowForLinksAdd.tbInputedText.Text) && !string.IsNullOrWhiteSpace(multiWindowForLinksAdd.tbInputedText.Text))
            {
                List<string> links = multiWindowForLinksAdd.tbInputedText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (links.Count == 1) OnStoreForDominationRequested(links[0], null);
                else if (links.Count > 1) OnStoreForDominationRequested(null, links);
            }

            multiWindowForLinksAdd = null;
        }
        #endregion

        private void exMIdelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            var contextMenu = mi.Parent as ContextMenu;
            if (contextMenu == null) return;
            var ex = contextMenu.PlacementTarget as Expander;
            if (ex == null) return;
            switch (mi.Name)
            {
                case "exMIdelete":
                    ViewModel.SearchResultsWithKwList.SearchResultsList[ViewModel.SearchResultsWithKwList.SISearchResultList].ClearAllDataFrom((ex.Content as ListView).ItemsSource);
                    break;

                default:
                    break;
            }
        }

        private void TextBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                string link = getLinkFromDataContext((sender as TextBlock).DataContext, forDominator: false);
                if ((sender as TextBlock).DataContext is MediaResultData)
                {
                    string id = (sender as TextBlock).Text;
                    link = "https://www.facebook.com/" + id;
                }
                if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link))
                {
                    OnOpenInBrowserRequested(link);
                }
            }
        }

        private void CMMIinner_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            ListView lv = getCurrentSelectedListViewFromMI(mi);
            if (lv == null) return;

            switch (mi.Name)
            {
                case "miOrderLikes":
                case "miOrderTalkingAbout":
                case "miOrderMembers":
                case "miOrderPrivacy":
                case "miOrderInterested":
                case "miOrderGoing":
                case "miOrderInvited":
                case "miOrderOrderMaybe":
                case "miOrderComments":
                case "miOrderViews":
                    ViewModel.OrderResultsOfListBy(mi.Name, lv.ItemsSource, true);
                    break;

                case "miRemove":
                    ViewModel.SearchResultsWithKwList.SearchResultsList[ViewModel.SearchResultsWithKwList.SISearchResultList].RemoveThisResultFromData(lv.SelectedItem);
                    break;

                case "miRemoveSelective":
                    TopNsearchResultsToDominateWindow filderForDeletionWindow = getSelectiveFilterWindw(lv, mi);
                    filderForDeletionWindow.rbBottom.IsChecked = true;
                    if (filderForDeletionWindow.ShowDialog() == true)
                    {
                        List<object> filterdItemsToDelete = getAllFilterdDataFromDataContext(lv.ItemsSource, lv.Items,
                            filderForDeletionWindow.MaxNums, filderForDeletionWindow.rbTopResults.IsChecked,
                            filderForDeletionWindow.miOrderLikes.IsChecked, filderForDeletionWindow.miOrderTalkingAbout.IsChecked,
                            filderForDeletionWindow.miOrderMembers.IsChecked, filderForDeletionWindow.miOrderPrivacyOpen.IsChecked, filderForDeletionWindow.miOrderPrivacyClosed.IsChecked,
                            filderForDeletionWindow.miOrderInterested.IsChecked, filderForDeletionWindow.miOrderGoing.IsChecked, filderForDeletionWindow.miOrderInvited.IsChecked, filderForDeletionWindow.miOrderOrderMaybe.IsChecked,
                            filderForDeletionWindow.miOrderComments.IsChecked, filderForDeletionWindow.miOrderViews.IsChecked);

                        if (filterdItemsToDelete.Count > 0)
                        {
                            foreach (object item in filterdItemsToDelete)
                            {
                                ViewModel.SearchResultsWithKwList.SearchResultsList[ViewModel.SearchResultsWithKwList.SISearchResultList].RemoveThisResultFromDataConditionally(item,
                                     (bool)filderForDeletionWindow.miOrderPrivacyOpen.IsChecked, (bool)filderForDeletionWindow.miOrderPrivacyClosed.IsChecked);
                            }
                        }
                    }
                    break;

                case "miDominate":
                    string link = getLinkFromDataContext(lv.SelectedItem, forDominator: true);
                    if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link))
                    {
                        addLinkToMultiWindow(link);
                    }
                    break;

                case "miDominateAll":
                    List<string> multiLinksToSend = new List<string>();
                    foreach (var item in lv.Items)
                    {
                        string linkToSent = getLinkFromDataContext(item, forDominator: true);
                        if (!string.IsNullOrEmpty(linkToSent) && !string.IsNullOrWhiteSpace(linkToSent)) multiLinksToSend.Add(linkToSent);
                    }

                    if (multiLinksToSend.Count > 0)
                    {
                        foreach (string l in multiLinksToSend)
                        {
                            addLinkToMultiWindow(l);
                        }
                    }
                    break;

                case "miDominateBy":
                    TopNsearchResultsToDominateWindow filterForDominationWindow = getSelectiveFilterWindw(lv, mi);
                    if (filterForDominationWindow.ShowDialog() == true)
                    {
                        List<object> filterdItemsForDominator = getAllFilterdDataFromDataContext(lv.ItemsSource, lv.Items,
                            filterForDominationWindow.MaxNums, filterForDominationWindow.rbTopResults.IsChecked,
                            filterForDominationWindow.miOrderLikes.IsChecked, filterForDominationWindow.miOrderTalkingAbout.IsChecked,
                            filterForDominationWindow.miOrderMembers.IsChecked, filterForDominationWindow.miOrderPrivacyOpen.IsChecked, filterForDominationWindow.miOrderPrivacyClosed.IsChecked,
                            filterForDominationWindow.miOrderInterested.IsChecked, filterForDominationWindow.miOrderGoing.IsChecked, filterForDominationWindow.miOrderInvited.IsChecked, filterForDominationWindow.miOrderOrderMaybe.IsChecked,
                            filterForDominationWindow.miOrderComments.IsChecked, filterForDominationWindow.miOrderViews.IsChecked);

                        if (filterdItemsForDominator.Count > 0)
                        {
                            foreach (object item in filterdItemsForDominator)
                            {
                                string linkToAdd = getLinkFromDataContext(item, forDominator: true);
                                if (!string.IsNullOrEmpty(linkToAdd) && !string.IsNullOrWhiteSpace(linkToAdd)) addLinkToMultiWindow(linkToAdd);
                            }
                        }
                    }
                    break;

                case "miDownload":
                    if (lv.SelectedItem is MediaResultData)
                    {
                        MediaResultData data = (lv.SelectedItem as MediaResultData);
                        if (data.is_video)
                        {
                            string linkForDownload = data.source;
                            linkForDownload = linkForDownload.Replace("&amp;", "&");
                            linkForDownload = linkForDownload.Replace("amp;", "");
                            OnOpenInBrowserForDownloadRequested(linkForDownload);
                        }
                        else
                        {
                            ViewModel.DownloadImageFromUrl(getLinkFromDataContext(lv.SelectedItem, forDominator: false));
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private TopNsearchResultsToDominateWindow getSelectiveFilterWindw(ListView lv,MenuItem mi)
        {
            TopNsearchResultsToDominateWindow tsrWindow = new TopNsearchResultsToDominateWindow();
            tsrWindow.tbMax.Text = Convert.ToString(lv.Items.Count);
            foreach (var item in (mi.Parent as ContextMenu).Items)
            {
                if (item is MenuItem)
                {
                    MenuItem miTcmbi = item as MenuItem;
                    switch (miTcmbi.Name)
                    {
                        case "miOrderLikes":
                            tsrWindow.miOrderLikes.Visibility = Visibility.Visible;
                            break;
                        case "miOrderComments":
                            tsrWindow.miOrderComments.Visibility = Visibility.Visible;
                            break;
                        case "miOrderViews":
                            tsrWindow.miOrderViews.Visibility = Visibility.Visible;
                            break;
                        case "miOrderTalkingAbout":
                            tsrWindow.miOrderTalkingAbout.Visibility = Visibility.Visible;
                            break;
                        case "miOrderMembers":
                            tsrWindow.miOrderMembers.Visibility = Visibility.Visible;
                            break;
                        case "miOrderPrivacy":
                            tsrWindow.miOrderPrivacyOpen.Visibility = Visibility.Visible;
                            tsrWindow.miOrderPrivacyClosed.Visibility = Visibility.Visible;
                            break;
                        case "miOrderInterested":
                            tsrWindow.miOrderInterested.Visibility = Visibility.Visible;
                            break;
                        case "miOrderGoing":
                            tsrWindow.miOrderGoing.Visibility = Visibility.Visible;
                            break;
                        case "miOrderInvited":
                            tsrWindow.miOrderInvited.Visibility = Visibility.Visible;
                            break;
                        case "miOrderOrderMaybe":
                            tsrWindow.miOrderOrderMaybe.Visibility = Visibility.Visible;
                            break;

                        default:
                            break;
                    }
                }
            }

            return tsrWindow;
        }

        private List<object> getAllFilterdDataFromDataContext(IEnumerable itemsSource, ItemCollection items,
            int maxListSize, bool? rbTopResults,
            bool? miOrderLikes, bool? miOrderTalkingAbout,
            bool? miOrderMembers, bool? miOrderPrivacyOpen, bool? miOrderPrivacyClosed,
            bool? miOrderInterested, bool? miOrderGoing, bool? miOrderInvited, bool? miOrderOrderMaybe, 
            bool? miOrderComments, bool? miOrderViews)
        {
            List<object> multiItemsWithFilter = new List<object>();
            int devideMaxBy = 0;

            if (miOrderLikes == true) devideMaxBy++;
            if (miOrderTalkingAbout == true) devideMaxBy++;

            if (miOrderMembers == true) devideMaxBy++;
            if (miOrderPrivacyOpen == true) devideMaxBy++;
            if (miOrderPrivacyClosed == true) devideMaxBy++;

            if (miOrderInterested == true) devideMaxBy++;
            if (miOrderGoing == true) devideMaxBy++;
            if (miOrderInvited == true) devideMaxBy++;
            if (miOrderViews == true) devideMaxBy++;

            if (miOrderComments == true) devideMaxBy++;
            if (miOrderViews == true) devideMaxBy++;

            if (devideMaxBy == 0) devideMaxBy = 1;
            int maxChunk = maxListSize / devideMaxBy;

            if (miOrderLikes == true || miOrderTalkingAbout == true ||
                miOrderMembers == true || miOrderPrivacyOpen == true || miOrderPrivacyClosed == true ||
                miOrderInterested == true || miOrderGoing == true || miOrderInvited == true || miOrderOrderMaybe == true||
                miOrderComments == true || miOrderViews == true)
            {
                addAllFilterdItemsRecursive(itemsSource, items,
                    maxListSize, maxChunk, devideMaxBy, ref multiItemsWithFilter,
                    (bool)rbTopResults,
                    (bool)miOrderLikes, (bool)miOrderTalkingAbout,
                    (bool)miOrderMembers, (bool)miOrderPrivacyOpen, (bool)miOrderPrivacyClosed,
                    (bool)miOrderInterested, (bool)miOrderGoing, (bool)miOrderInvited, (bool)miOrderOrderMaybe,
                    (bool)miOrderComments, (bool)miOrderViews);

                return multiItemsWithFilter;
            }
            else
            {
                foreach (var item in items)
                {
                    multiItemsWithFilter.Add(item);
                    if (multiItemsWithFilter.Count == maxListSize) break;
                }

                return multiItemsWithFilter;
            }
        }

        private void addAllFilterdItemsRecursive(IEnumerable itemsSource, ItemCollection items,
            int maxListSize, int maxChunk, int devideMaxBy, ref List<object> multiItemsWithFilter,
            bool topResults,
            bool miOrderLikes, bool miOrderTalkingAbout,
            bool miOrderMembers, bool miOrderPrivacyOpen, bool miOrderPrivacyClosed,
            bool miOrderInterested, bool miOrderGoing, bool miOrderInvited, bool miOrderOrderMaybe,
            bool miOrderComments, bool miOrderViews)
        {
            #region pages or places  [miOrderLikes | miOrderTalkingAbout] 
            if (miOrderLikes)
            {
                ViewModel.OrderResultsOfListBy("miOrderLikes", itemsSource, topResults);
                miOrderLikes = false;
            }
            else if (miOrderTalkingAbout)
            {
                ViewModel.OrderResultsOfListBy("miOrderTalkingAbout", itemsSource, topResults);
                miOrderTalkingAbout = false;
            }
            #endregion
            #region media [miOrderComments | miOrderViews ]
            else if (miOrderComments)
            {
                ViewModel.OrderResultsOfListBy("miOrderComments", itemsSource, topResults);
                miOrderComments = false;
            }
            else if (miOrderViews)
            {
                ViewModel.OrderResultsOfListBy("miOrderViews", itemsSource, topResults);
                miOrderViews = false;
            }
            #endregion
            #region groups [miOrderMembers | miOrderPrivacyOpen | miOrderPrivacyClosed]
            else if (miOrderMembers)
            {
                ViewModel.OrderResultsOfListBy("miOrderMembers", itemsSource, topResults);
                miOrderMembers = false;
            }
            else if (miOrderPrivacyOpen)
            {
                ViewModel.OrderResultsOfListBy("miOrderPrivacyOpen", itemsSource, topResults);
                miOrderPrivacyOpen = false;
            }
            else if (miOrderPrivacyClosed)
            {
                ViewModel.OrderResultsOfListBy("miOrderPrivacyClosed", itemsSource, topResults);
                miOrderPrivacyClosed = false;
            }
            #endregion
            #region events [miOrderInterested | miOrderGoing | miOrderInvited | miOrderOrderMaybe]
            else if (miOrderInterested)
            {
                ViewModel.OrderResultsOfListBy("miOrderInterested", itemsSource, topResults);
                miOrderInterested = false;
            }
            else if (miOrderGoing)
            {
                ViewModel.OrderResultsOfListBy("miOrderGoing", itemsSource, topResults);
                miOrderGoing = false;
            }
            else if (miOrderInvited)
            {
                ViewModel.OrderResultsOfListBy("miOrderInvited", itemsSource, topResults);
                miOrderInvited = false;
            }
            else if (miOrderOrderMaybe)
            {
                ViewModel.OrderResultsOfListBy("miOrderOrderMaybe", itemsSource, topResults);
                miOrderOrderMaybe = false;
            }
            #endregion

            List<object> multiLinksChunk = new List<object>();
            foreach (var item in items)
            {
                if (!multiLinksChunk.Contains(item)) multiLinksChunk.Add(item);
                if (multiLinksChunk.Count == maxChunk) break;
            }
            multiItemsWithFilter.AddRange(multiLinksChunk);

            if (devideMaxBy > 1)
            {
                devideMaxBy--;
                addAllFilterdItemsRecursive(itemsSource, items,
                    maxListSize, maxChunk, devideMaxBy, ref multiItemsWithFilter,
                    topResults,
                    miOrderLikes, miOrderTalkingAbout,
                    miOrderMembers, miOrderPrivacyOpen, miOrderPrivacyClosed,
                    miOrderInterested, miOrderGoing, miOrderInvited, miOrderOrderMaybe,
                    miOrderComments, miOrderViews);
            }
        }


        private string getLinkFromDataContext(object dataContext, bool forDominator)
        {
            string link = "";

            if (dataContext is PagesResultData)
            {
                PagesResultData data = (dataContext as PagesResultData);
                if (forDominator)
                {
                    link = Social.FACEBOOK_PAGES_DEFAULT_URL + data.name + "-" + data.id;
                }
                else
                {
                    link = data.link;
                }
            }
            else if (dataContext is GroupsResultData)
            {
                GroupsResultData data = (dataContext as GroupsResultData);
                if (forDominator)
                {
                    link = Social.FACEBOOK_GROUPS_DEFAULT_URL + data.name + "-" + data.id;
                }
                else
                {
                    link = Social.FACEBOOK_GROUPS_DEFAULT_URL + data.id;
                }
            }
            else if (dataContext is EventsResultData)
            {
                EventsResultData data = (dataContext as EventsResultData);
                if (forDominator)
                {
                    link = Social.FACEBOOK_EVENTS_DEFAULT_URL + data.name + "-" + data.id;
                }
                else
                {
                    link = Social.FACEBOOK_EVENTS_DEFAULT_URL + data.id;
                }
            }
            else if (dataContext is PlacesResultData)
            {
                PlacesResultData data = (dataContext as PlacesResultData);
                if (forDominator)
                {
                    link = Social.FACEBOOK_PLACES_DEFAULT_URL + data.name + "-" + data.id;
                }
                else
                {
                    link = data.link;
                }
            }
            else if (dataContext is MediaResultData)
            {
                MediaResultData data = (dataContext as MediaResultData);
                if (forDominator)
                {
                    if (data.is_video)
                    {
                        link = Social.FACEBOOK_VIDEOS_DEFAULT_URL + data.id + "-" + data.id;
                    }
                    else
                    {
                        link = Social.FACEBOOK_PHOTOS_DEFAULT_URL + data.id + "-" + data.id;
                    }
                }
                else
                {
                    link = data.link;
                }
            }
            else if (dataContext is PersonsResultData)
            {
                PersonsResultData data = (dataContext as PersonsResultData);
                if (forDominator)
                {
                    link = Social.FACEBOOK_USERS_DEFAULT_URL + data.name + "-" + data.id;
                }
                else
                {
                    link = data.link;
                }
                if (link.Contains("app_scoped_user_id/")) link = link.Replace("app_scoped_user_id/", "");
            }

            return link;
        }

        #endregion
    }
}


















//if (dcItemSource is System.Collections.ObjectModel.ObservableCollection<PagesResultData>)
//{
//    foreach (var data in dcItemSource as System.Collections.ObjectModel.ObservableCollection<PagesResultData>)
//    {
//        string link = getLinkFromDataContext(data);
//        if(!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link)) allAvailableLinks.Add(link);
//    }
//}
//else if (dcItemSource is System.Collections.ObjectModel.ObservableCollection<GroupsResultData>)
//{
//    foreach (var data in dcItemSource as System.Collections.ObjectModel.ObservableCollection<GroupsResultData>)
//    {
//        string link = getLinkFromDataContext(data);
//        if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link)) allAvailableLinks.Add(link);
//    }
//}
//else if (dcItemSource is System.Collections.ObjectModel.ObservableCollection<EventsResultData>)
//{
//    foreach (var data in dcItemSource as System.Collections.ObjectModel.ObservableCollection<EventsResultData>)
//    {
//        string link = getLinkFromDataContext(data);
//        if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link)) allAvailableLinks.Add(link);
//    }
//}
//else if (dcItemSource is System.Collections.ObjectModel.ObservableCollection<PlacesResultData>)
//{
//    foreach (var data in dcItemSource as System.Collections.ObjectModel.ObservableCollection<PlacesResultData>)
//    {
//        string link = getLinkFromDataContext(data);
//        if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link)) allAvailableLinks.Add(link);
//    }
//}
//else if (dcItemSource is System.Collections.ObjectModel.ObservableCollection<PersonsResultData>)
//{
//    // link = (dataContext as PersonsResultData).link;
//    //if (link.Contains("app_scoped_user_id/")) link = link.Replace("app_scoped_user_id/", "");
//    foreach (var data in dcItemSource as System.Collections.ObjectModel.ObservableCollection<PersonsResultData>)
//    {
//        string link = getLinkFromDataContext(data);
//        if (!string.IsNullOrEmpty(link) && !string.IsNullOrWhiteSpace(link)) allAvailableLinks.Add(link);
//    }
//}