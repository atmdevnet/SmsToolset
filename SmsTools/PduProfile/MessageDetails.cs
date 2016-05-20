using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public class MessageDetails
    {
        public string Message { get; internal set; }
        public long Sender { get; internal set; }
        public DateTime Date { get; internal set; }
        public bool IsValid { get; internal set; }
    }
}
