using Browser.Common.Models;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Browser.Common.ViewModels
{
    public class RemindersVM
    {
        public event Action OnOpen = delegate { };
        public event Action<string> OnNavigate = delegate { };

        public ICommand OnCommand { get; set; }

        public ObservableCollection<Reminder> Reminders { get; set; }

        public RemindersVM()
        {
            OnCommand = new RelayCommand(OnCommand_Raised);

            Reminders = new ObservableCollection<Reminder>();
        }

        private void OnCommand_Raised(object param)
        {
            switch ((string)param)
            {
                case "OPEN":
                    OnOpen();
                    break;

                default:
                    break;
            }
        }

        public void FillReminders(List<string> jsonReminders)
        {
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<List<string>>(FillReminders), jsonReminders);
                return;
            }

            List<Reminder> tempListForOderByDT = new List<Reminder>();

            foreach (var json in jsonReminders)
            {
                if (json != "\r\n")
                {
                    Reminder r = Newtonsoft.Json.JsonConvert.DeserializeObject<Reminder>(json);
                    r.description = r.description.Replace(Environment.NewLine, "");
                    r.description = "Description: " + r.description;
                    tempListForOderByDT.Add(r);
                }
            }

            tempListForOderByDT = tempListForOderByDT.OrderBy(d => d.duedate).ToList();

            foreach (var r in tempListForOderByDT)
            {
                Reminders.Add(r);
            }
        }

        internal void Start(string absoluteUri)
        {
            OnNavigate(absoluteUri);
        }
    }
}
