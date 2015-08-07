using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Youtuber
{
    public class YoutuberVM : INotifyPropertyChanged
    {
        public ICommand OkClicked { get; set; }

        public string InputCode { get; set; }

        private string outputText;
        public string OutputText
        {
            get { return outputText; }
            set
            {
                outputText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OutputText"));
                }
            }
        }

        private Visibility resultsVisible;
        public Visibility ResultsVisible
        {
            get { return resultsVisible; }
            set
            {
                resultsVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ResultsVisible"));
                }
            }
        }

        List<string> mYoutubeVariationsList = new List<string>();

        public YoutuberVM()
        {
            OkClicked = new RelayCommand(OnOkClicked);

            mYoutubeVariationsList.Add("https://youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsList.Add("https://youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsList.Add("https://youtu.be/");
            mYoutubeVariationsList.Add("https://youtube.com/embed/");
            mYoutubeVariationsList.Add("https://www.youtube.com/embed/");
            mYoutubeVariationsList.Add("https://youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsList.Add("https://youtube.com/watch?v=");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=");
            mYoutubeVariationsList.Add("http://youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsList.Add("http://youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsList.Add("http://youtu.be/");
            mYoutubeVariationsList.Add("http://youtube.com/embed/");
            mYoutubeVariationsList.Add("http://www.youtube.com/embed/");
            mYoutubeVariationsList.Add("http://youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsList.Add("http://youtube.com/watch?v=");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?v=");

            ResultsVisible = Visibility.Collapsed;
        }

        public void OnOkClicked(object param)
        {
            if (!string.IsNullOrWhiteSpace(InputCode) && !string.IsNullOrEmpty(InputCode))
            {
                ResultsVisible = Visibility.Collapsed;
                OutputText = "";

                string code = "";

                foreach (string link in mYoutubeVariationsList)
                {
                    if (InputCode.Contains(link))
                    {
                        code = InputCode.Replace(link, "");
                    }
                }

                if (code == "") return;
                
                foreach (string link in mYoutubeVariationsList)
                {
                    OutputText += link + code + Environment.NewLine;
                }

                ResultsVisible = Visibility.Visible;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
