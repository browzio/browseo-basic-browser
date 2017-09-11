using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMacroMultyLayout.Models
{
    public class ProjectDataLinesSetting : ViewModelBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }

        private int dataSourceLinesFrom;
        public int DataSourceLinesFrom
        {
            get { return dataSourceLinesFrom; }
            set { dataSourceLinesFrom = value; RaisePropertyChanged("DataSourceLinesFrom"); }
        }

        private int dataSourceLinesTo;
        public int DataSourceLinesTo
        {
            get { return dataSourceLinesTo; }
            set { dataSourceLinesTo = value; RaisePropertyChanged("DataSourceLinesTo"); }
        }

    }
}
