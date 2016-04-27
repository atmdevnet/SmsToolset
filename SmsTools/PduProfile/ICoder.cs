using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public interface ICoder
    {
        string Encode(string value, out int length);
        string Decode(string value);
        int MaxLength { get; }
    }
}
