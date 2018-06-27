using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Prospector.Models
{
    public class DWQueryOption
    {
        public event Action<DWQueryOption> OnCloseMe;
        public string Type { get; set; }
        public string Value { get; set; }

        public ICommand OnCommandFromView { get; set; }

        public DWQueryOption()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            
        }

        private void OnCommandFromView_Raised(object obj)
        {
            OnCloseMe?.Invoke(this);
        }
    }
}
