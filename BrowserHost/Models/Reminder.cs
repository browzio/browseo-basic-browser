using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserHost.Models
{
    public class Reminder
    {
        public string title { get; set; }
        public string description { get; set; }
        public DateTime duedate { get; set; }
    }
}
