using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.DataAccess
{
    public class LocalFiles
    {
        public static string GetBaseDir()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Browseo\\Browzio";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            return directory;
        }
    }
}
