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


        private CrawlerHost mCrawlerHost;

        private TaskScheduler tsUiContext;

        private string errors = "";

        private object mLock = new object();       
        private object mSaveLock = new object();

        public SearchVM()
        {
            OnClickCommand = new RelayCommand(OnAnyCommandFromView);

            SearchResultsWithKwList = new SearchResultWithKeyWord();

            VisibleProgress = Visibility.Collapsed;

            tsUiContext = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Factory.StartNew(load, CancellationToken.None, TaskCreationOptions.None, tsUiContext);
            

           // BindingOperations.EnableCollectionSynchronization(SearchResultsList.PagesResult.data, mListLock);
           // BindingOperations.EnableCollectionSynchronization(SearchResultsList.GroupsResult.data, mListLock);
        }

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
                }

                mCrawlerHost.IninAdin();
            }
        }

        private bool anyOptionsChecked()
        {
            return IsCheckedPage || IsCheckedGroup || IsCheckedEvent || IsCheckedPlace || IsCheckedUser;
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
            mCrawlerHost.PreInitStates.Clear();
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
                    string inPage = " in ";
                    switch (searchState.state)
                    {    
                        case CrawlerStates.GraphSearch_Pages:
                            inPage += "Pages";
                            break;
                        case CrawlerStates.GraphSearch_Groups:
                            inPage += "Groups";
                            break;
                        case CrawlerStates.GraphSearch_Events:
                            inPage += "Events";
                            break;
                        case CrawlerStates.GraphSearch_Places:
                            inPage += "Places";
                            break;
                        case CrawlerStates.GraphSearch_Users:
                            inPage += "Users";
                            break;
                        default:
                            break;
                    }
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
