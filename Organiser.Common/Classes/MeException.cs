using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes
{
    public class MyException : Exception
    {
        public MyException(string message) : base(message)
        {
        }

        public override string StackTrace
        {
            get
            {
                return string.Join(Environment.NewLine,"UN KNOWN");
            }
        }
    }
}
