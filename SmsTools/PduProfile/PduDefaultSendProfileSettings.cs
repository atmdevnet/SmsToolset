using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// Default profile settings for sending packets.
    /// </summary>
    [DataContract]
    public class PduDefaultSendProfileSettings : IPduProfileSettings
    {
        [DataMember(Name = "sca", IsRequired = false)]
        public PduAddressSegmentContract ServiceCenterAddress { get; set; }

        [DataMember(Name = "pdu-header", IsRequired = true)]
        public PduValueSegmentContract<int> PduHeader { get; set; }

        [DataMember(Name = "mr", IsRequired = false)]
        public PduValueSegmentContract<int> MessageReference { get; set; }

        [DataMember(Name = "da", IsRequired = false)]
        public PduAddressSegmentContract DestinationAddress { get; set; }

        [DataMember(Name = "pid", IsRequired = true)]
        public PduValueSegmentContract<int> ProtocolIdentifier { get; set; }

        [DataMember(Name = "dcs", IsRequired = true)]
        public PduValueSegmentContract<int> DataCodingScheme { get; set; }

        [DataMember(Name = "vp", IsRequired = true)]
        public PduValueSegmentContract<int> ValidityPeriod { get; set; }

        [DataMember(Name = "ud", IsRequired = false)]
        public PduValueSegmentContract<string> UserData { get; set; }

        public PduValueSegmentContract<string> ServiceCenterTimestamp { get; set; }

        public IEnumerable<PduSegment> Sequence
        {
            get
            {
                return new PduSegment[] {
                    PduSegment.ServiceCenterAddress,
                    PduSegment.PduHeader,
                    PduSegment.MessageReference,
                    PduSegment.DestinationAddress,
                    PduSegment.ProtocolIdentifier,
                    PduSegment.DataCodingScheme,
                    PduSegment.ValidityPeriod,
                    PduSegment.UserData
                };
            }
        }

        public bool CanDeliver { get; private set; } = false;
        public bool CanSubmit { get; private set; } = true;
    }
}
