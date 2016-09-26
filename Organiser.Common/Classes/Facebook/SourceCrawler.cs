using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes.Facebook
{
    public class FBSourceCrawler
    {
        public static List<string> GetIdsFromVideoScrape(string source)
        {
            List<string> thisIdList = new List<string>();
            try
            {
                List<string> sourceAfterSplit = source.Split(new string[] { "data-bt=\"{id:" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                sourceAfterSplit.RemoveAt(0);
                foreach (string line in sourceAfterSplit)
                {
                    string id = line.Remove(line.IndexOf(","));
                    id = id.Trim();
                    thisIdList.Add(id);
                }
            }
            catch { }
            return thisIdList;
        }
    }
}
