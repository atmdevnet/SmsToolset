using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    [DataContract]
    public class PduAddressSegmentContract
    {
        [DataMember(Name = "type", IsRequired = true)]
        public int AddressType { get; set; }

        [DataMember(Name = "value", IsRequired = false)]
        public long AddressValue { get; set; }
    }
}
