using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Models
{
    public enum NameValueModelValueTypes
    {
        Directory
    }

    public class NameValueModel : PropertyChangedBase
    {
        private string name="";
        public string Name
        {
            get { return name; }
            set { name = value; NotifyOfPropertyChange(); }
        }

        private string val="";
        public string Value
        {
            get { return val; }
            set { val = value; NotifyOfPropertyChange(); }
        }

        public NameValueModelValueTypes ValueType = NameValueModelValueTypes.Directory;

        public NameValueModel(string name, string value, NameValueModelValueTypes valueType = NameValueModelValueTypes.Directory)
        {
            this.name = name;
            this.val = value;
            ValueType = valueType;
        }
    }
}
