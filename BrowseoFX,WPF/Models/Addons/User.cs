using Newtonsoft.Json;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BrowseoFX_WPF.Models.Addons
{
    public class User
    {
        public event Action<User, object> OnCommand_Raised;

        [JsonIgnore]
        public ICommand OnCommandFromView { get; set; }

        public string Username { get; set; }
        public string ID { get; set; }

        public User()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        private void OnCommandFromView_Raised(object obj)
        {
            OnCommand_Raised?.Invoke(this, obj);
        }
    }
}
