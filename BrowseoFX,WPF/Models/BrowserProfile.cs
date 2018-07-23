using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Models
{
    [Serializable]
    public class BrowserProfile
    {
        #region cache indexing
        public string SectionName { get; set; }

        //Name
        public string ProfileName { get; set; }
        //IsRelative
        public string IsRelative { get; set; }
        //Path
        public string Path { get; set; }
        //Default
        public string Default { get; set; }
        //{
        //    get
        //    {
        //        return _profileName;
        //    }
        //    set
        //    {
        //        _profileName = value;
        //        CreateDirectory(ProfilePathRoaming);
        //        CreateDirectory(ProfilePathLocal);
        //        if (File.Exists(ProfilesFile))
        //        {
        //            var filedata = new IniFile(ProfilesFile);
        //            int i = 0;
        //            try
        //            {
        //                while (filedata.IniReadValue("Profile" + i + "", "Name") != "")
        //                {
        //                    i++;
        //                }
        //            }
        //            catch { }

        //            var profnameValSection = "Profile" + i + "";
        //            filedata.IniWriteValue(profnameValSection, "Name", ProfileName);
        //            filedata.IniWriteValue(profnameValSection, "IsRelative", "1");
        //            filedata.IniWriteValue(profnameValSection, "Path", "Profiles/" + ProfileName);
        //        }
        //    }
        //}
        #endregion

        #region usage info... bookmarks
        public List<NameValueModel> Bookmarks { get; set; }

        #endregion

        public BrowserProfile()
        {
            Bookmarks = new List<NameValueModel>();
        }
    }
}
