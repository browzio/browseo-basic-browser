using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectsList.Models
{
    public class ProjectData
    {
        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        private string projDir;
        public string ProjDir
        {
            get { return projDir; }
            set { projDir = value; }
        }

        private List<string> sites;
        public List<string> Sites
        {
            get { return sites; }
            set { sites = value; }
        }

        private PersonData personData;
        public PersonData PersonData
        {
            get { return personData; }
            set { personData = value; }
        }
    }
}
