using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GoViral.Models.FilterResults
{
    public enum OptionType
    {
        [Description("Likes")]
        Likes,
        [Description("Comments")]
        Comments,
        [Description("Views")]
        Views,
        [Description("Talking About")]
        TalkingAbout,
        [Description("Members")]
        Members,
        [Description("OPEN")]
        Privacy_OPEN,
        [Description("CLOSED")]
        Privacy_CLOSED,
        [Description("Interested")]
        Interested,
        [Description("Going")]
        Going,
        [Description("Invited")]
        Invited,
        [Description("Maybe")]
        Maybe,
        [Description("Users")]
        Users,
        [Description("Tags")]
        Tags,
        [Description("Media")]
        Media,
        [Description("Following")]
        Following,
        [Description("Followers")]
        Followers,
    }

    public enum ListType
    {
        Users,
        Media,
        Tags
    }
    public class FilterOption : ViewModelBase
    {
        public FilterOption()
        {
            ViSibleStartfomOptions = Visibility.Collapsed;
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged("Title"); }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (value)
                {
                    if (OptionState == OptionType.Privacy_CLOSED || OptionState == OptionType.Privacy_OPEN)
                    {
                        ViSibleStartfomOptions = Visibility.Collapsed;
                    }
                    else
                    {
                        ViSibleStartfomOptions = Visibility.Visible;
                    }
                }
                else
                {
                    ViSibleStartfomOptions = Visibility.Collapsed;
                }
                RaisePropertyChanged("IsChecked");
            }
        }

        private int startingFrom;
        public int StartingFrom
        {
            get { return startingFrom; }
            set { startingFrom = value; RaisePropertyChanged("StartingFrom"); }
        }

        private Visibility viSibleStartfomOptions;
        public Visibility ViSibleStartfomOptions
        {
            get { return viSibleStartfomOptions; }
            set { viSibleStartfomOptions = value; RaisePropertyChanged("ViSibleStartfomOptions"); }
        }


        public OptionType OptionState { get; set; }
        public ListType ListState { get; set; }
    }
}
