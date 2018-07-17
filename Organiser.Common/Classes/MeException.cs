using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes
{
    public class BadImageException : Exception
    {
        public BadImageException(string message) : base(message)
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
