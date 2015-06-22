using NichResearch.Crawlers;
using NichResearch.Helpers;
using NichResearch.Models;
using NichResearch.ViewModels;
using NichResearch.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NichResearch
{
    public class ResearchVM : INotifyPropertyChanged
    {
        private ICommand search;
        public ICommand Search
        {
            get { return search; }
            set { search = value; }
        }

        private ICommand multi;
        public ICommand Multi
        {
            get { return multi; }
            set { multi = value; }
        }

        private ICommand sendToWindow;
        public ICommand SendToWindow
        {
            get { return sendToWindow; }
            set { sendToWindow = value; }
        }

        private ICommand save;
        public ICommand Save
        {
            get { return save; }
            set { save = value; }
        }

        #region lists
        private ObservableCollection<YoutubeItem> youtubeResultsList;
        public ObservableCollection<YoutubeItem> YoutubeResultsList
        {
            get { return youtubeResultsList; }
            set { youtubeResultsList = value; }
        }

        private int sIYoutubeResultsList;
        public int SIYoutubeResultsList
        {
            get { return sIYoutubeResultsList; }
            set { sIYoutubeResultsList = value; }
        }

        private ObservableCollection<SocialMentionItem> socialmentionResultsList;
        public ObservableCollection<SocialMentionItem> SocialmentionResultsList
        {
            get { return socialmentionResultsList; }
            set { socialmentionResultsList = value; }
        }

        private int sISocialmentionResultsList;
        public int SISocialmentionResultsList
        {
            get { return sISocialmentionResultsList; }
            set { sISocialmentionResultsList = value; }
        }
        private ObservableCollection<string> socialmentionTypes;
        public ObservableCollection<string> SocialmentionTypes
        {
            get { return socialmentionTypes; }
            set { socialmentionTypes = value; }
        }

        private int cmbSISocialmentionTypes;
        public int CmbSISocialmentionTypes
        {
            get { return cmbSISocialmentionTypes; }
            set { cmbSISocialmentionTypes = value; }
        }

        private bool cmbSocialmentionTypesVisible;
        public bool CmbSocialmentionTypesVisible
        {
            get { return cmbSocialmentionTypesVisible; }
            set
            {
                cmbSocialmentionTypesVisible = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CmbSocialmentionTypesVisible"));
            }
        }
        

        private string commandType;
        public string CommandType
        {
            get { return commandType; }
            set
            {
                commandType = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CommandType"));
            }
        }
        
        #endregion

        #region selection options
        private string keyWords;
        public string KeyWords
        {
            get { return keyWords; }
            set
            {
                keyWords = value;
                PropertyChanged(this, new PropertyChangedEventArgs("KeyWords"));
            }
        }

        private bool searchYoutube;
        public bool SearchYoutube
        {
            get { return searchYoutube; }
            set { searchYoutube = value; }
        }

        private bool searchSocialMention;
        public bool SearchSocialMention
        {
            get { return searchSocialMention; }
            set
            {
                searchSocialMention = value;
                CmbSocialmentionTypesVisible = searchSocialMention;
            }
        }

        private string inputedLinksString;
        public string InputedLinksString
        {
            get { return inputedLinksString; }
            set { inputedLinksString = value; }
        }

        #endregion

        #region pbar
        private bool pBarVisible;
        public bool PBarVisible
        {
            get { return pBarVisible; }
            set
            {
                pBarVisible = value;
                PropertyChanged(this, new PropertyChangedEventArgs("PBarVisible"));
            }
        }
        
        #endregion

        CopyPasteWindow cpWindow = new CopyPasteWindow();
        CopyPasteWindowVM cpWindowDataContxt;

        YoutubeCrawler mYoutubeCrawler = new YoutubeCrawler();
        SocialMentionCrawler mSocialMentionCrawler = new SocialMentionCrawler();

        int timesfinished;

        public ResearchVM()
        {
            cpWindowDataContxt = new CopyPasteWindowVM();
            cpWindow.DataContext = cpWindowDataContxt;
            cpWindow.Closed += cpWindow_Closed;

            YoutubeResultsList = new ObservableCollection<YoutubeItem>();
            SocialmentionResultsList = new ObservableCollection<SocialMentionItem>();
            initCocialMentionTypes();

            Search = new RelayCommand(BeginSearch);
            Multi = new RelayCommand(OpenMultiWindow);
            SendToWindow = new RelayCommand(SendToCopyWindow);
            Save = new RelayCommand(SaveResults);

            mYoutubeCrawler.OnReturnResults += new Action<List<YoutubeItem>>(OnYoutubeListReturned);
            mYoutubeCrawler.OnFinished += OnFinishedSearch;
            mSocialMentionCrawler.OnReturnResults += new Action<List<SocialMentionItem>>(OnSocialMentionListReturned);
            mSocialMentionCrawler.OnFinished += OnFinishedSearch;

            CommandType = "Copy To Window";
        }

        private void initCocialMentionTypes()
        {
            SocialmentionTypes = new ObservableCollection<string>();
            SocialmentionTypes.Add("all");
            SocialmentionTypes.Add("blogs");
            SocialmentionTypes.Add("microblogs");
            SocialmentionTypes.Add("bookmarks");
            SocialmentionTypes.Add("news");
        }

        private void SaveResults(object obj)
        {
            //PersonData personData = Browser.personData;
            //personData = PersonDataFileReader.GetPersonData(personData);
            //WrightListResults.WrightResults(personData, YoutubeResultsList, SocialmentionResultsList);
        }

        private void SendToCopyWindow(object obj)
        {
            try
            {
                if (cpWindow == null)
                {
                    cpWindowDataContxt = new CopyPasteWindowVM();
                    cpWindow = new CopyPasteWindow();
                    cpWindow.DataContext = cpWindowDataContxt;
                    cpWindow.Closed += cpWindow_Closed;
                }
                if (obj.ToString().Equals("youtube"))
                {
                    cpWindowDataContxt.CopyPasteItemList.Add(new CopyPasteItem()
                    {
                        Title = YoutubeResultsList[SIYoutubeResultsList].Title,
                        Link = YoutubeResultsList[SIYoutubeResultsList].Link
                    });
                }
                else
                {
                    cpWindowDataContxt.CopyPasteItemList.Add(new CopyPasteItem()
                    {
                        Title = SocialmentionResultsList[SISocialmentionResultsList].Title,
                        Link = SocialmentionResultsList[SISocialmentionResultsList].Link
                    });
                }

                cpWindow.Show();
            }
            catch { }
        }

        void cpWindow_Closed(object sender, EventArgs e)
        {
            cpWindow = null;
        }

        private void BeginSearch(object obj)
        {
            if (!SearchYoutube && !SearchSocialMention) return;
            
            PBarVisible = true;
            timesfinished = 0;

            if (SearchYoutube)
            {
                YoutubeResultsList.Clear();
                mYoutubeCrawler.Search(KeyWords);
            }

            if (SearchSocialMention)
            {
                SocialmentionResultsList.Clear();
                mSocialMentionCrawler.Search(KeyWords, SocialmentionTypes[CmbSISocialmentionTypes]);
            }
        }

        private void OpenMultiWindow(object obj)
        {
            MultiKeywordWindow w = new MultiKeywordWindow();
            w.DataContext = this;
            w.ShowDialog();
            if (string.IsNullOrEmpty(InputedLinksString) || string.IsNullOrWhiteSpace(InputedLinksString))
                return;

            KeyWords = "";

            string[] linksArray = InputedLinksString.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string link in linksArray)
            {
                if (link != "")
                {
                    string linkWord = link;
                    linkWord = linkWord.Trim();

                    if (KeyWords == "")
                        KeyWords += link;
                    else
                        KeyWords += ", " + link;
                }
            }
        }

        void OnYoutubeListReturned(List<YoutubeItem> resultsList)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (YoutubeItem item in resultsList)
                {
                    YoutubeResultsList.Add(item);
                }
            });
        }

        void OnSocialMentionListReturned(List<SocialMentionItem> resultsList)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (SocialMentionItem item in resultsList)
                {
                    SocialmentionResultsList.Add(item);
                }
            });
        }

        void OnFinishedSearch()
        {
            if (SearchYoutube && SearchSocialMention)
            {
                timesfinished++;
                if (timesfinished == 2)
                    PBarVisible = false;
            }
            else
                PBarVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}
