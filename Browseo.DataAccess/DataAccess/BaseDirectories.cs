using Browseo.Browser.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.Browser.DataAccess
{
    public class BaseDirectories : SingletonBase<BaseDirectories>
    {
        public string BaseDir { get; set; }

        public BaseDirectories()
        {
            BaseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        }
    }
}
