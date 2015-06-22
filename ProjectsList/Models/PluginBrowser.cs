using BrowserAndFeatures;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsList.Models
{
    public class PluginBrowser
    {
        public string Title { get; private set; }
        public FeatureCallage View { get; private set; }
        public PersonData PData { get; set; }

        public PluginBrowser(PersonData data)
        {
            PData = data;
            Title = data.ProjectName;
            View = new FeatureCallage();
            View.SetPersonData(data);
        }
    }
}
