using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    [DataContract]
    public class PduValueSegmentContract<T>
    {
        [DataMember(Name = "value", IsRequired = true)]
        public T Value { get; set; }
    }
}
