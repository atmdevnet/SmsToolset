using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// PDU header for send packet
    /// </summary>
    public class PduSendHeaderSegment : PduHeaderSegment
    {
        public PduSendHeaderSegment(IPduProfileSettings settings)
            : base(settings)
        {
        }

        public void SetStatusReportRequired(bool value)
        {
            setHeader(getMask(5, value), value);
        }

        public bool IsStatusReportRequired()
        {
            return (_header & getMask(5, true)) > 0;
        }

        public void SetValidityPeriodFormat(VPF value)
        {
            _header |= (int)value << 3;
        }

        public VPF GetValidityPeriodFormat()
        {
            int mask = 3 << 3;
            return (VPF)((_header & mask) >> 3);
        }

        public void SetRejectDuplicates(bool value)
        {
            setHeader(getMask(2, value), value);
        }

        public bool HasRejectDuplicates()
        {
            return (_header & getMask(2, true)) > 0;
        }
    }
}
