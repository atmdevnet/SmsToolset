using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public class ATCommandAttribute : Attribute
    {
        public string Command { get; set; }
        public bool HasSteps { get; set; }
        public bool HasParameters { get; set; }
        public bool AllowsAnonymous { get; set; }

        public ATCommandAttribute()
        {
        }
    }
}
