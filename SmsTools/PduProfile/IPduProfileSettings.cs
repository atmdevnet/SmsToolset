using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public interface IPduProfileSettings
    {
        PduAddressSegmentContract ServiceCenterAddress { get; }
        PduAddressSegmentContract DestinationAddress { get; }
        PduValueSegmentContract<int> PduHeader { get; }
        PduValueSegmentContract<int> MessageReference { get; }
        PduValueSegmentContract<int> ProtocolIdentifier { get; }
        PduValueSegmentContract<int> DataCodingScheme { get; }
        PduValueSegmentContract<int> ValidityPeriod { get; }
        PduValueSegmentContract<string> ServiceCenterTimestamp { get; }
        PduValueSegmentContract<string> UserData { get; }

        bool CanDeliver { get; }
        bool CanSubmit { get; }

        IEnumerable<PduSegment> Sequence { get; }
    }
}
