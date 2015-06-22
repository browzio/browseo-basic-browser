using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichResearch.Models
{
    public class SocialMentionItem
    {
        private string  iconSentiment;
        public string  IconSentiment
        {
            get { return iconSentiment; }
            set { iconSentiment = value; }
        }

        private string icon;
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
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

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string info;
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

    }
}
