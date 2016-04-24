using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public interface IPduSegment
    {
        PduSegment Type { get; }
        int Length();
    }
}
