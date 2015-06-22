using NichResearch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NichResearch.Helpers
{
    public class WrightListResults
    {
        //public static void WrightResults(PersonData pdata, ObservableCollection<YoutubeItem> youtubeResultsList, ObservableCollection<SocialMentionItem> socialmentionResultsList)
        //{
        //    string path = Path.Combine(GetBaseDir(), "Projects", pdata.ProjectName);
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);

        //    IniFile fileWrighter = new IniFile(Path.Combine(path, "SearchResults.ini"));
        //    foreach (YoutubeItem ytResult in youtubeResultsList)
        //    {
        //        fileWrighter.IniWriteValue("ytResult", "ByLink", ytResult.ByLink);
        //        fileWrighter.IniWriteValue("ytResult", "ByName", ytResult.ByName);
        //        fileWrighter.IniWriteValue("ytResult", "Description", ytResult.Description);
        //        fileWrighter.IniWriteValue("ytResult", "ImageLink", ytResult.ImageLink);
        //        fileWrighter.IniWriteValue("ytResult", "Link", ytResult.Link);
        //        fileWrighter.IniWriteValue("ytResult", "TimeAgo", ytResult.TimeAgo);
        //        fileWrighter.IniWriteValue("ytResult", "Title", ytResult.Title);
        //        fileWrighter.IniWriteValue("ytResult", "Views", ytResult.Views);
        //    }
        //    foreach (SocialMentionItem smResult in socialmentionResultsList)
        //    {
        //         fileWrighter.IniWriteValue("smResult", "Description", smResult.Description);
        //         fileWrighter.IniWriteValue("smResult", "Icon", smResult.Icon);
        //         fileWrighter.IniWriteValue("smResult", "IconSentiment", smResult.IconSentiment);
        //         fileWrighter.IniWriteValue("smResult", "Info", smResult.Info);
        //         fileWrighter.IniWriteValue("smResult", "Link", smResult.Link);
        //         fileWrighter.IniWriteValue("smResult", "Title", smResult.Title);
        //    }
        //    int i = 0;
        //}

        //public static string GetBaseDir()
        //{
        //    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        //}
    }
}
