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
        List<string> mYoutubeVariationsChecker = new List<string>();

        public YoutuberVM()
        {
            Organiser.Common.Classes.UsageTracker.AddTraceCookie(Organiser.Common.Classes.UsageTracker.Usage_Type_ToYoutubeUrler);
            OkClicked = new RelayCommand(OnOkClicked);

            mYoutubeVariationsChecker.Add("https://youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsChecker.Add("https://youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsChecker.Add("https://www.youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsChecker.Add("https://www.youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsChecker.Add("https://youtu.be/");
            mYoutubeVariationsChecker.Add("https://youtube.com/embed/");
            mYoutubeVariationsChecker.Add("https://www.youtube.com/embed/");
            mYoutubeVariationsChecker.Add("https://youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsChecker.Add("https://www.youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsChecker.Add("https://youtube.com/watch?v=");
            mYoutubeVariationsChecker.Add("https://www.youtube.com/watch?v=");
            mYoutubeVariationsChecker.Add("http://youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsChecker.Add("http://youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsChecker.Add("http://www.youtube.com/watch?feature=youtu.be&v=");
            mYoutubeVariationsChecker.Add("http://www.youtube.com/watch?feature=youtube_gdata&v=");
            mYoutubeVariationsChecker.Add("http://www.youtube.com/embed/");
            mYoutubeVariationsChecker.Add("http://youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsChecker.Add("http://www.youtube.com/watch?feature=player_embedded&v=");
            mYoutubeVariationsChecker.Add("http://youtube.com/watch?v=");
            mYoutubeVariationsChecker.Add("http://www.youtube.com/watch?v=");

            mYoutubeVariationsList.Add("https://m.youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&app=mobile");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&app=desktop");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&hc_location=ufi");
            mYoutubeVariationsList.Add("https://youtube.com/watch?v=VIDEOCODE&app=desktop");
            mYoutubeVariationsList.Add("https://youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtube.com/watch?feature=youtube_gdata&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtube.com/watch?feature=youtu.be&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtube.com/watch?feature=player_embedded&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtube.com/v/VIDEOCODE?version=3");
            mYoutubeVariationsList.Add("https://youtube.com/v/VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtube.com/embed/VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtube.com/e/VIDEOCODE?app=desktop");
            mYoutubeVariationsList.Add("https://youtube.com/e/VIDEOCODE");
            mYoutubeVariationsList.Add("https://youtu.be/VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&feature=youtube_gdata");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&feature=youtu.be");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&feature=share");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&feature=kp");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE&app=desktop");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=youtube_gdata&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=youtube.be&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=youtu.be&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/watch?feature=player_embedded&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/v/VIDEOCODE?version=3");
            mYoutubeVariationsList.Add("https://www.youtube.com/v/VIDEOCODE&feature=youtube_gdata");
            mYoutubeVariationsList.Add("https://www.youtube.com/v/VIDEOCODE&feature=youtu.be");
            mYoutubeVariationsList.Add("https://www.youtube.com/v/VIDEOCODE&feature=share");
            mYoutubeVariationsList.Add("https://www.youtube.com/v/VIDEOCODE&feature=kp");
            mYoutubeVariationsList.Add("https://www.youtube.com/v/VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/embed/VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/e/VIDEOCODE?app=desktop");
            mYoutubeVariationsList.Add("https://www.youtube.com/e/VIDEOCODE");
            mYoutubeVariationsList.Add("https://www.youtube.com/attribution_link?a=VIDEOCODE&u=watch?v=VIDEOCODE&feature=share");
            mYoutubeVariationsList.Add("https://m.youtube.com/watch?v=VIDEOCODE&feature=youtube_gdata");
            mYoutubeVariationsList.Add("https://m.youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://m.youtube.com/watch?feature=player_embedded&v=VIDEOCODE");
            mYoutubeVariationsList.Add("https://m.youtube.com/v/VIDEOCODE");
            mYoutubeVariationsList.Add("https://m.youtube.com/e/VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/watch?v=VIDEOCODE&app=desktop");
            mYoutubeVariationsList.Add("http://youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/watch?feature=youtube_gdata&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/watch?feature=youtu.be&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/watch?feature=player_embedded&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/v/VIDEOCODE?version=3");
            mYoutubeVariationsList.Add("http://youtube.com/v/VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/embed/VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtube.com/e/VIDEOCODE?app=desktop");
            mYoutubeVariationsList.Add("http://youtube.com/e/VIDEOCODE");
            mYoutubeVariationsList.Add("http://youtu.be/VIDEOCODE");
            mYoutubeVariationsList.Add("http://y2u.be/VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?v=VIDEOCODE&feature=youtube_gdata");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?v=VIDEOCODE&feature=youtu.be");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?v=VIDEOCODE&feature=kp");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?v=VIDEOCODE&app=desktop");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=youtube_gdata&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=youtube.be&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=youtu.be&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/watch?feature=player_embedded&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/v/VIDEOCODE?version=3");
            mYoutubeVariationsList.Add("http://www.youtube.com/v/VIDEOCODE&feature=youtube_gdata");
            mYoutubeVariationsList.Add("http://www.youtube.com/v/VIDEOCODE&feature=youtu.be");
            mYoutubeVariationsList.Add("http://www.youtube.com/v/VIDEOCODE&feature=share");
            mYoutubeVariationsList.Add("http://www.youtube.com/v/VIDEOCODE&feature=kp");
            mYoutubeVariationsList.Add("http://www.youtube.com/v/VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/embed/VIDEOCODE");
            mYoutubeVariationsList.Add("http://www.youtube.com/e/VIDEOCODE?app=desktop");
            mYoutubeVariationsList.Add("http://www.youtube.com/e/VIDEOCODE");
            mYoutubeVariationsList.Add("http://m.youtube.com/watch?v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://m.youtube.com/watch?feature=player_embedded&v=VIDEOCODE");
            mYoutubeVariationsList.Add("http://m.youtube.com/v/VIDEOCODE");
            mYoutubeVariationsList.Add("http://m.youtube.com/e/VIDEOCODE");
            mYoutubeVariationsList.Add("http://i.ytimg.com/vi/VIDEOCODE/3.jpg");
            mYoutubeVariationsList.Add("http://i.ytimg.com/vi/VIDEOCODE/2.jpg");
            mYoutubeVariationsList.Add("http://i.ytimg.com/vi/VIDEOCODE/1.jpg");
            mYoutubeVariationsList.Add("http://i.ytimg.com/vi/VIDEOCODE/maxresdefault.jpg");
            mYoutubeVariationsList.Add(" http://i.ytimg.com/vi/VIDEOCODE/mqdefault.jpg");
            mYoutubeVariationsList.Add("http://i.ytimg.com/vi/VIDEOCODE/default.jpg");
            mYoutubeVariationsList.Add("http://i.ytimg.com/vi/VIDEOCODE/0.jpg");
            mYoutubeVariationsList.Add("http://img.youtube.com/vi/VIDEOCODE/3.jpg");
            mYoutubeVariationsList.Add("http://img.youtube.com/vi/VIDEOCODE/2.jpg");
            mYoutubeVariationsList.Add("http://img.youtube.com/vi/VIDEOCODE/1.jpg");
            mYoutubeVariationsList.Add("http://img.youtube.com/vi/VIDEOCODE/maxresdefault.jpg");
            mYoutubeVariationsList.Add("http://img.youtube.com/vi/VIDEOCODE/default.jpg");
            mYoutubeVariationsList.Add("http://img.youtube.com/vi/VIDEOCODE/0.jpg");


            ResultsVisible = Visibility.Collapsed;
        }

        public void OnOkClicked(object param)
        {
            if (!string.IsNullOrWhiteSpace(InputCode) && !string.IsNullOrEmpty(InputCode))
            {
                try
                {
                    ResultsVisible = Visibility.Collapsed;
                    OutputText = "";

                    string code = "";

                    foreach (string link in mYoutubeVariationsChecker)
                    {
                        if (InputCode.Contains(link))
                        {
                            code = InputCode.Replace(link, "");
                        }
                    }

                    if (code == "") return;

                    Organiser.Common.Classes.UsageTracker.AddTraceCookie(Organiser.Common.Classes.UsageTracker.Usage_Type_CreatedYoutubeUrls+" : " + InputCode);

                    foreach (string link in mYoutubeVariationsList)
                    {
                        OutputText += link.Replace("VIDEOCODE", code) + Environment.NewLine;
                    }

                    ResultsVisible = Visibility.Visible;
                }
                catch { }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
