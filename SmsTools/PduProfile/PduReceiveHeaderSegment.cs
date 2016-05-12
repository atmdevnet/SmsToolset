using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// PDU header for receive packet
    /// </summary>
    public class PduReceiveHeaderSegment : PduHeaderSegment
    {
        public PduReceiveHeaderSegment(IPduProfileSettings settings)
            : base(settings)
        {
            if (!settings.CanDeliver || GetMessageType() != MTI.Delivery)
                throw new ArgumentException("Profile settings not valid.");
        }

        public bool IsStateReportRequested()
        {
            return (_header & getMask(5, true)) > 0;
        }

        public bool IsMoreMessagesSent()
        {
            return (_header & getMask(2, true)) > 0;
        }

        public override bool IsValid()
        {
            return base.IsValid() && GetMessageType() == MTI.Delivery;
        }
    }
}
