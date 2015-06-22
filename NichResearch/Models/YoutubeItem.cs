using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichResearch.Models
{
    public class YoutubeItem
    {
        private string imageLink;
        public string ImageLink
        {
            get { return imageLink; }
            set { imageLink = value; }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string byLink;
        public string ByLink
        {
            get { return byLink; }
            set { byLink = value; }
        }

        private string byName;
        public string ByName
        {
            get { return byName; }
            set { byName = value; }
        }

        private string timeAgo;
        public string TimeAgo
        {
            get { return timeAgo; }
            set { timeAgo = value; }
        }

        private string views;
        public string Views
        {
            get { return views; }
            set { views = value; }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
