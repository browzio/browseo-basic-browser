using Browseo.Browser.DataAccess;
using Browseo.Browser.Framework.IO;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.Browser.Framework.Services
{
    public class VersionMigrationService
    {
        /// <summary>
        /// takes old system and migrates to firefox style bookmarks
        /// </summary>
        public void MigrateBookmarksTo52()
        {
            string gloableBookmarks = Path.Combine(BaseDirectories.Instance.BaseDir, "Bookmarks", "GloableBookMarks_G_");
            if (Directory.Exists(gloableBookmarks))
            {
            }

            //string projectFolder = Path.Combine(BaseDirectories.Instance.BaseDir, "Bookmarks", GloableProfData.PData.ProjectName);
            //if (!Directory.Exists(projectFolder)) return;

        }
        
    }
}
