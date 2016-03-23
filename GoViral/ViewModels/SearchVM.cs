using GoViral.Helpers;
using GoViral.Models;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using Organiser.Common.Classes.Crawler;
using Organiser.Common.Classes.Facebook;
using Organiser.Common.Windows;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using GoViral.Models.FilterResults;

namespace GoViral.ViewModels
{
    public class SearchVM : ViewModelBase
    {
        public ICommand OnClickCommand { get; set; } 


        private Visibility visibleProgress;    
        public Visibility VisibleProgress
        {
            get { return visibleProgress; }
            set { visibleProgress = value; RaisePropertyChanged("VisibleProgress"); }
        }

        private string status;   
        public string Status
        {
            get { return status; }
            set { status = value; RaisePropertyChanged("Status"); }
        }
                                

        #region search options
        private bool isCheckedPage; 
        public bool IsCheckedPage
        {
            get { return isCheckedPage; }
            set { isCheckedPage = value; RaisePropertyChanged("IsCheckedPage"); }
        }

        private bool isCheckedGroup;
        public bool IsCheckedGroup
        {
            get { return isCheckedGroup; }
            set { isCheckedGroup = value; RaisePropertyChanged("IsCheckedGroup"); }
        }

        private bool isCheckedEvent;
        public bool IsCheckedEvent
        {
            get { return isCheckedEvent; }
            set { isCheckedEvent = value; RaisePropertyChanged("IsCheckedEvent"); }
        }

        private bool isCheckedPlace;
        public bool IsCheckedPlace
        {
            get { return isCheckedPlace; }
            set { isCheckedPlace = value; RaisePropertyChanged("IsCheckedPlace"); }
        }

        private bool isCheckedUser;
        public bool IsCheckedUser
        {
            get { return isCheckedUser; }
            set { isCheckedUser = value; RaisePropertyChanged("IsCheckedUser"); }
        }

        //IsCheckedPhotos
        private bool isCheckedPhotos;
        public bool IsCheckedPhotos
        {
            get { return isCheckedPhotos; }
            set { isCheckedPhotos = value; RaisePropertyChanged("IsCheckedPhotos"); }
        }

        //IsCheckedVideos
        private bool isCheckedVideos;
        public bool IsCheckedVideos
        {
            get { return isCheckedVideos; }
            set { isCheckedVideos = value; RaisePropertyChanged("IsCheckedVideos"); }
        }


        private string searchText;  
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; RaisePropertyChanged("SearchText"); }
        }
        #endregion


        private SearchResultWithKeyWord searchResultsList; 
        public SearchResultWithKeyWord SearchResultsWithKwList
        {
            get { return searchResultsList; }
            set { searchResultsList = value; }
        }


        private ObservableCollection<FilterOption> filterOptionsList;
        public ObservableCollection<FilterOption> FilterOptionsList
        {
            get { return filterOptionsList; }
            set { filterOptionsList = value; }
        }

        private ObservableCollection<SearchResult> searchResultsForFilter;
        public ObservableCollection<SearchResult> SearchResultsForFilter
        {
            get { return searchResultsForFilter; }
            set { searchResultsForFilter = value; }
        }

        private bool threeStateFilterchecked;
        public bool ThreeStateFilterchecked
        {
            get { return threeStateFilterchecked; }
            set
            {
                threeStateFilterchecked = value;
                foreach (var sr in SearchResultsForFilter)
                {
                    sr.IsChecked = (bool)threeStateFilterchecked;
                }

                RaisePropertyChanged("ThreeStateFilterchecked");
            }
        }


        private CrawlerHost mCrawlerHost;

        private TaskScheduler tsUiContext;

        private string errors = "";

        private object mLock = new object();       
        private object mSaveLock = new object();

        public SearchVM()
        {
            OnClickCommand = new RelayCommand(OnAnyCommandFromView);

            SearchResultsWithKwList = new SearchResultWithKeyWord();
            FilterOptionsList = new ObservableCollection<FilterOption>();
            SearchResultsForFilter = new ObservableCollection<SearchResult>();

            VisibleProgress = Visibility.Collapsed;

            tsUiContext = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(load, CancellationToken.None, TaskCreationOptions.None, tsUiContext);
            

           // BindingOperations.EnableCollectionSynchronization(SearchResultsList.PagesResult.data, mListLock);
           // BindingOperations.EnableCollectionSynchronization(SearchResultsList.GroupsResult.data, mListLock);
        }

        #region from view via command or method

        private void OnAnyCommandFromView(object obj)
        {
            string param = obj as string;
            switch (param)
            {
                case "MULTI":
                    setKwByMulty();
                    break;

                case "SEARCH":
                    new Thread(beginCrawlerSearch).Start();
                    break;

                case "CANCEL":
                    cancelCrawl();
                    break;

                case "SAVE":
                    new Thread(save).Start();
                    break;

                default:
                    break;
            }
        }

        internal void OrderResultsOfListBy(string sortby, IEnumerable itemsSource, bool orderByDescending)
        {
            //SearchResult selectedResult = selectedSearchResultinKWList as SearchResult;
            //if (selectedResult == null) return;

            switch (sortby)
            {
                //pages and places
                case "miOrderLikes":
                case "miOrderTalkingAbout":
                case "miOrderComments":
                case "miOrderViews":
                    if (itemsSource is ObservableCollection<PlacesResultData>) //places
                    {
                        ObservableCollection<PlacesResultData> itemsToSortPlaces = itemsSource as ObservableCollection<PlacesResultData>;
                        List<PlacesResultData> sortedPlaces = orderByDescending ? itemsToSortPlaces.OrderByDescending(i => sortby == "miOrderLikes" ? i.likes : i.talking_about_count).ToList() : 
                                                                                  itemsToSortPlaces.OrderBy(i => sortby == "miOrderLikes" ? i.likes : i.talking_about_count).ToList();
                        itemsToSortPlaces.Clear();
                        foreach (var item in sortedPlaces)
                        {
                            itemsToSortPlaces.Add(item);
                        }
                    }
                    else if (itemsSource is ObservableCollection<PagesResultData>)
                    {
                        ObservableCollection<PagesResultData> itemsToSortPages = itemsSource as ObservableCollection<PagesResultData>;
                        List<PagesResultData> sortedPages = orderByDescending ? itemsToSortPages.OrderByDescending(i => sortby == "miOrderLikes" ? i.likes : i.talking_about_count).ToList() :
                                                                                itemsToSortPages.OrderBy(i => sortby == "miOrderLikes" ? i.likes : i.talking_about_count).ToList(); 
                        itemsToSortPages.Clear();
                        foreach (var item in sortedPages)
                        {
                            itemsToSortPages.Add(item);
                        }
                    }
                    else if (itemsSource is ObservableCollection<MediaResultData>)
                    {
                        ObservableCollection<MediaResultData> itemsToSortPages = itemsSource as ObservableCollection<MediaResultData>;
                        List<MediaResultData> sortedMedia = orderByDescending ? itemsToSortPages.OrderByDescending(i=> sortby == "miOrderLikes" ? i.like_count : sortby == "miOrderComments" ? i.comment_count : i.view_count).ToList():
                                                                                itemsToSortPages.OrderBy(i => sortby == "miOrderLikes" ? i.like_count : sortby == "miOrderComments" ? i.comment_count : i.view_count).ToList();
                        itemsToSortPages.Clear();
                        foreach (var item in sortedMedia)
                        {
                            itemsToSortPages.Add(item);
                        }
                    }
                    break;

                //groups
                case "miOrderMembers":
                case "miOrderPrivacy":
                case "miOrderPrivacyOpen":
                case "miOrderPrivacyClosed":
                    if (itemsSource is ObservableCollection<GroupsResultData>)
                    {
                        ObservableCollection<GroupsResultData> itemsToSortGroups = itemsSource as ObservableCollection<GroupsResultData>;
                        List<GroupsResultData> sorted;
                        if (sortby == "miOrderMembers")
                        {
                            sorted = orderByDescending ? itemsToSortGroups.OrderByDescending(i => i.members == null || i.members.summary == null ? 0 : i.members.summary.total_count).ToList():
                                                         itemsToSortGroups.OrderBy(i => i.members == null || i.members.summary == null ? 0 : i.members.summary.total_count).ToList();
                        }
                        else
                        {
                            sorted = orderByDescending ? itemsToSortGroups.OrderByDescending(i => sortby == "miOrderPrivacy" ? i.privacy :
                                                                                                  sortby == "miOrderPrivacyOpen" ?  "OPEN" : "CLOSED").ToList() :
                                                         itemsToSortGroups.OrderBy(i => sortby == "miOrderPrivacy" ? i.privacy :
                                                                                                  sortby == "miOrderPrivacyOpen" ? "OPEN" : "CLOSED").ToList();
                        }
                        itemsToSortGroups.Clear();

                        foreach (var item in sorted)
                        {
                            itemsToSortGroups.Add(item);
                        }
                    }
                    break;

                //events
                case "miOrderInterested":
                case "miOrderGoing":
                case "miOrderInvited":
                case "miOrderOrderMaybe":
                    if (itemsSource is ObservableCollection<EventsResultData>)
                    {
                        ObservableCollection<EventsResultData> itemsToSortEvents = itemsSource as ObservableCollection<EventsResultData>;
                        List<EventsResultData> sorted = orderByDescending ? itemsToSortEvents.OrderByDescending(i => sortby == "miOrderInterested" ? i.interested == null || i.interested.summary == null ? 0 : i.interested.summary.count :
                                                                                                                i.invited == null || i.invited.summary == null ? 0 :
                                                                                                                sortby == "miOrderGoing" ? i.invited.summary.attending_count :
                                                                                                                sortby == "miOrderInvited" ? i.invited.summary.count :
                                                                                                                i.invited.summary.maybe_count).ToList() :
                                                                            itemsToSortEvents.OrderByDescending(i => sortby == "miOrderInterested" ? i.interested == null || i.interested.summary == null ? 0 : i.interested.summary.count :
                                                                                                                i.invited == null || i.invited.summary == null ? 0 :
                                                                                                                sortby == "miOrderGoing" ? i.invited.summary.attending_count :
                                                                                                                sortby == "miOrderInvited" ? i.invited.summary.count :
                                                                                                                i.invited.summary.maybe_count).ToList();
                        itemsToSortEvents.Clear();

                        foreach (var item in sorted)
                        {
                            itemsToSortEvents.Add(item);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        #region filter
        internal void SetAvailableKeywordsList(IEnumerable itemsSource)
        {
            SearchResultsForFilter.Clear();
            ThreeStateFilterchecked = false;

            foreach (var item in SearchResultsWithKwList.SearchResultsList)
            {
                item.IsChecked = false;

                if (SearchResultsWithKwList.SISearchResultList >= 0 && 
                    item.Keyword == SearchResultsWithKwList.SearchResultsList[SearchResultsWithKwList.SISearchResultList].Keyword)
                {
                    item.IsChecked = true;
                    ThreeStateFilterchecked = false;
                }
                if (itemsSource is ObservableCollection<PlacesResultData>)
                {
                    if (item.PlacesResult.data.Count > 0)
                    {
                        SearchResultsForFilter.Add(item);
                    }
                }
                else if (itemsSource is ObservableCollection<PagesResultData>)
                {
                    if (item.PagesResult.data.Count > 0)
                    {
                        SearchResultsForFilter.Add(item);
                    }
                }
                else if (itemsSource is ObservableCollection<MediaResultData>)
                {
                    if (item.MediaResult.data.Count > 0)
                    {
                        SearchResultsForFilter.Add(item);
                    }
                }
                else if (itemsSource is ObservableCollection<GroupsResultData>)
                {
                    if (item.GroupsResult.data.Count > 0)
                    {
                        SearchResultsForFilter.Add(item);
                    }
                }
                else if (itemsSource is ObservableCollection<EventsResultData>)
                {
                    if (item.EventsResult.data.Count > 0)
                    {
                        SearchResultsForFilter.Add(item);
                    }
                }
            }
        }

        internal void AddFilterOption(OptionType type)
        {
            FilterOptionsList.Add(new FilterOption() { OptionState = type, Title = type.GetDescription() });
        }

        internal bool GetFilterOptionChecked(OptionType option)
        {
            return FilterOptionsList.Any(o => o.OptionState == option && o.IsChecked);
        }

        internal int GetOptionMinStart(OptionType option)
        {
            FilterOption o = FilterOptionsList.FirstOrDefault(p => p.OptionState == option);
            return o == null ? 0 : o.StartingFrom;
        }

        internal int GetCheckedFilterCount()
        {
            return FilterOptionsList.Where(f => f.IsChecked).Count();
        }
        #endregion


        internal void DownloadImageFromUrl(string picUrl)
        {
            Task.Factory.StartNew(() =>
            {
                VisibleProgress = Visibility.Visible;

                picUrl = picUrl.Replace("&amp;", "&");
                picUrl = picUrl.Replace("amp;", "");
                MyFilesDatabase.DownloadImage(picUrl);

                VisibleProgress = Visibility.Collapsed;
            });
        }

        #endregion

        private void save()
        {
            lock (mSaveLock)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(delegate { Mouse.OverrideCursor = Cursors.Wait; });

                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);

                    string savetoFilePath = Path.Combine(saveToDir, "FBSerchInfo");

                    File.WriteAllText(savetoFilePath, ObjectCopier.CleanInvalidXmlChars(SearchResultsWithKwList.SearchResultsList.XmlSerializeToString()));
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Failed to save searches. " + ex.Message);
                }

                Application.Current.Dispatcher.Invoke(delegate { Mouse.OverrideCursor = null; });
            }
        }

        private void load()
        {
            lock (mSaveLock)
            {
                try
                {    
                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "GoViral", GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(saveToDir)) return;

                    string savetoFilePath = Path.Combine(saveToDir, "FBSerchInfo");
                    if (!File.Exists(savetoFilePath)) return;

                    string savedXml = File.ReadAllText(savetoFilePath);
                    ObservableCollection<SearchResult> savedContent = savedXml.XmlDeserializeFromString<ObservableCollection<SearchResult>>();
                    foreach (var item in savedContent)
                    {
                        SearchResultsWithKwList.SearchResultsList.Add(item);
                    }
                }
                catch (Exception ex)
                {   

                } 
            }
        }


        private void setKwByMulty()
        {
            RssFeedsLinksMultiWindow window = new RssFeedsLinksMultiWindow();
            window.Title = "One Per Line";
            if (!string.IsNullOrEmpty(SearchText) && !string.IsNullOrWhiteSpace(SearchText))
            {
                string[] kws = SearchText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);  
                foreach (var kw in kws)
                {
                    window.tbInputedText.Text += kw + Environment.NewLine;
                }
            }
            window.ShowDialog();
            if (window.OKClicked)
            {
                string inputedText = window.tbInputedText.Text;
                if (string.IsNullOrEmpty(inputedText) || string.IsNullOrWhiteSpace(inputedText)) return;

                string[] kws = inputedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                inputedText = "";
                foreach (var kw in kws)
                {
                    inputedText += kw + ",";
                }

                inputedText = inputedText.Remove(inputedText.LastIndexOf(","));
                SearchText = inputedText;
            }
        }

        private void beginCrawlerSearch()
        {
            lock (mLock)
            {
                VisibleProgress = Visibility.Visible;
                Status = "Initializing crawler...";
                if (string.IsNullOrEmpty(SearchText) || string.IsNullOrWhiteSpace(SearchText) || !anyOptionsChecked() || !initCrawler())
                {
                    visibleProgress = Visibility.Collapsed;
                    return;
                }

                string[] kws = SearchText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var kw in kws)
                {
                    if (IsCheckedPage) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Pages, url = kw.Trim() });
                    if (IsCheckedGroup) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Groups, url = kw.Trim() });
                    if (IsCheckedEvent) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Events, url = kw.Trim() });
                    if (IsCheckedPlace) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Places, url = kw.Trim() });
                    if (IsCheckedUser) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Users, url = kw.Trim() });
                    if (IsCheckedPhotos) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Photos, url = kw.Trim() });
                    if(IsCheckedVideos) mCrawlerHost.PreInitStates.Add(new CrawlerPreInitState() { state = CrawlerStates.GraphSearch_Videos, url = kw.Trim() });
                    UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_FacebookCralEvent + " Searched fo kw " + kw);
                }


                mCrawlerHost.IninAdin();
            }
        }

        private bool anyOptionsChecked()
        {
            return IsCheckedPage || IsCheckedGroup || IsCheckedEvent || IsCheckedPlace || IsCheckedUser || IsCheckedPhotos || IsCheckedVideos;
        }

        private bool initCrawler()
        {
            if (mCrawlerHost == null)
            {
                try
                {
                    mCrawlerHost = new CrawlerHost();
                    mCrawlerHost.OnReportProgress += MCrawlerHost_OnReportProgress;
                    mCrawlerHost.OnReportGotGraphData += MCrawlerHost_OnReportGotGraphData;
                    mCrawlerHost.OnReportFatalError += MCrawlerHost_OnReportFatalError;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not start crawl process. " + ex.Message);
                    mCrawlerHost = null;
                    return false;
                }
            }

            return true;
        }

        private void cancelCrawl(bool lockit = true)
        {
            if (lockit)
            {
                lock (mLock)
                {
                    cancelCrawlWork();
                }
            }
            else
            {
                cancelCrawlWork();
            }
        }

        private void cancelCrawlWork()
        {
            if (mCrawlerHost != null && mCrawlerHost.PreInitStates != null) mCrawlerHost.PreInitStates.Clear();

            VisibleProgress = Visibility.Collapsed;
            if(Status == "Initializing crawler...")
            {
                ShutDown();
                mCrawlerHost = null;
            }
            if (errors != "")
            {
                FlexibleMessageBox.Show(errors);
                errors = "";
            }
        }

        private void MCrawlerHost_OnReportFatalError(string userMessage, string fullExceptionText)
        {       
            cancelCrawl(false);
            mCrawlerHost = null;
            MessageBox.Show("A fatal error occured in crawler process the cerawler has been shut down. " + userMessage + Environment.NewLine + fullExceptionText);
        }

        private void MCrawlerHost_OnReportGotGraphData(string json, CrawlerPreInitState searchState)
        {
            //Application.Current.Dispatcher.Invoke((Action)delegate { handleGraphDataResult(json, searchState); });
            //handleGraphDataResult(json, searchState);
            //new Thread(() => { handleGraphDataResult(json, searchState); }).Start();
            Task.Factory.StartNew(() => { handleGraphDataResult(json, searchState); }, CancellationToken.None, TaskCreationOptions.None, tsUiContext);
        }

        private void handleGraphDataResult(string json, CrawlerPreInitState searchState)
        {
            lock (mLock)
            {
                if (json == "N/A")
                {
                    string inPage = " in " + searchState.state.GetDescription();
                    errors += "Failed To Search For " + searchState.url + inPage + Environment.NewLine;
                }
                else
                {
                    SearchResult resultWithKw = SearchResultsWithKwList.SearchResultsList.SingleOrDefault(r => r.Keyword == searchState.url);
                    if (resultWithKw == null)
                    {
                        resultWithKw = new SearchResult() { Keyword = searchState.url };
                        SearchResultsWithKwList.SearchResultsList.Add(resultWithKw);
                    }
                    switch (searchState.state)
                    {
                        case CrawlerStates.GraphSearch_Pages:
                            PagesResult pagesResult = JsonConvert.DeserializeObject<PagesResult>(json); 
                            foreach (var result in pagesResult.data)
                            {
                                if (resultWithKw.PagesResult.data.Any(d => d.name == result.name)) continue;
                                resultWithKw.PagesResult.data.Add(result);
                            }
                            break;
                        case CrawlerStates.GraphSearch_Groups:
                            GroupsResult groupsResult = JsonConvert.DeserializeObject<GroupsResult>(json);
                            foreach (var result in groupsResult.data)
                            {
                                if (resultWithKw.PagesResult.data.Any(d => d.name == result.name)) continue;
                                resultWithKw.GroupsResult.data.Add(result);
                            }
                            break;
                        case CrawlerStates.GraphSearch_Events:
                            EventsResult eventsResult = JsonConvert.DeserializeObject<EventsResult>(json);
                            foreach (var result in eventsResult.data)
                            {
                                if (resultWithKw.PagesResult.data.Any(d => d.name == result.name)) continue;
                                resultWithKw.EventsResult.data.Add(result);
                            }
                            break;
                        case CrawlerStates.GraphSearch_Places:
                            PlacesResult placesResult = JsonConvert.DeserializeObject<PlacesResult>(json);
                            foreach (var result in placesResult.data)
                            {
                                if (resultWithKw.PagesResult.data.Any(d => d.name == result.name)) continue;
                                resultWithKw.PlacesResult.data.Add(result);
                            }
                            break;
                        case CrawlerStates.GraphSearch_Users:
                            PersonsResult personsResult = JsonConvert.DeserializeObject<PersonsResult>(json);
                            foreach (var result in personsResult.data)
                            {
                                if (resultWithKw.PagesResult.data.Any(d => d.name == result.name)) continue;
                                resultWithKw.PersonsResult.data.Add(result);
                            }
                            break;

                        case CrawlerStates.GraphSearch_Photos:
                            MediaResult photoResult = json.XmlDeserializeFromString<MediaResult>();
                            foreach(var result in photoResult.data)
                            {
                                if (resultWithKw.MediaResult.data.Any(d => d.id == result.id)) continue;
                                resultWithKw.MediaResult.data.Add(result);
                            }
                            break;

                        case CrawlerStates.GraphSearch_Videos:
                            MediaResult videoResult = json.XmlDeserializeFromString<MediaResult>();
                            foreach (var result in videoResult.data)
                            {
                                if (resultWithKw.MediaResultVideos.data.Any(d => d.id == result.id)) continue;
                                resultWithKw.MediaResultVideos.data.Add(result);
                            }
                            break;

                        default:
                            break;
                    } 

                    RaisePropertyChanged("SearchResultsList");
                }

                removePreInitState(searchState);
            }
        }

        private void removePreInitState(CrawlerPreInitState preinintState)
        {
            mCrawlerHost.PreInitStates.Remove(preinintState);

            if (mCrawlerHost.PreInitStates.Count == 0)
            {
                VisibleProgress = Visibility.Collapsed;
                if (errors != "")
                {
                    FlexibleMessageBox.Show(errors);
                    errors = "";
                }
            }
            else
            {
                mCrawlerHost.navigateToNextUrl();
            }
        }

        private void MCrawlerHost_OnReportProgress(string progressText)
        {
            if (VisibleProgress != Visibility.Visible) VisibleProgress = Visibility.Visible;
            Status = progressText;
        }

        public void ShutDown()
        {
            if(mCrawlerHost != null)
            {
                mCrawlerHost.Shutdown();
            }
        }
    }
}
