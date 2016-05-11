using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// Default profile settings for read received packets.
    /// </summary>
    [DataContract]
    public class PduDefaultReceiveProfileSettings : IPduProfileSettings
    {
        [DataMember(Name = "sca", IsRequired = false)]
        public PduAddressSegmentContract ServiceCenterAddress { get; set; }

        [DataMember(Name = "pdu-header", IsRequired = true)]
        public PduValueSegmentContract<int> PduHeader { get; set; }

        [DataMember(Name = "da", IsRequired = false)]
        public PduAddressSegmentContract DestinationAddress { get; set; }

        [DataMember(Name = "pid", IsRequired = false)]
        public PduValueSegmentContract<int> ProtocolIdentifier { get; set; }

        [DataMember(Name = "dcs", IsRequired = false)]
        public PduValueSegmentContract<int> DataCodingScheme { get; set; }

        [DataMember(Name = "scts", IsRequired = false)]
        public PduValueSegmentContract<string> ServiceCenterTimestamp { get; set; }

        [DataMember(Name = "ud", IsRequired = false)]
        public PduValueSegmentContract<string> UserData { get; set; }

        public PduValueSegmentContract<int> MessageReference { get; set; }
        public PduValueSegmentContract<int> ValidityPeriod { get; set; }

        public IEnumerable<PduSegment> Sequence
        {
            get
            {
                return new PduSegment[] {
                    PduSegment.ServiceCenterAddress,
                    PduSegment.PduHeader,
                    PduSegment.DestinationAddress,
                    PduSegment.ProtocolIdentifier,
                    PduSegment.DataCodingScheme,
                    PduSegment.ServiceCenterTimestamp,
                    PduSegment.UserData
                };
            }
        }

        public bool CanDeliver { get; private set; } = true;
        public bool CanSubmit { get; private set; } = false;
    }
}
