using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragDropListview.Models
{
    public class Reminder : INotifyPropertyChanged 
    {
        private string reminderText;

        public string ReminderText
        {
            get { return reminderText; }
            set
            {
                reminderText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ReminderText"));
            }
        }

        private string reminderDate;

        public string ReminderDate
        {
            get { return reminderDate; }
            set
            {
                reminderDate = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ReminderDate"));
            }
        }

        private string reminderName;

        public string ReminderName
        {
            get { return reminderName; }
            set
            {
                reminderName = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ReminderName"));
            }
        }

        private string resolvedText;

        public string ResolvedText
        {
            get { return resolvedText; }
            set
            {
                resolvedText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ResolvedText"));
            }
        }

        private System.Windows.Media.Brush forColor;
        public System.Windows.Media.Brush ForeColorComplete
        {
            get { return forColor; }
            set
            {
                forColor = value;
                if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("ForeColorComplete"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
