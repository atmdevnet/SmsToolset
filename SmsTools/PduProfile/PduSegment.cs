using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// Name of PDU packet segment.
    /// </summary>
    public enum PduSegment
    {
        [Description("SCA")]
        ServiceCenterAddress,
        [Description("PDU Type")]
        PduHeader,
        [Description("MR")]
        MessageReference,
        [Description("DA")]
        DestinationAddress,
        [Description("PID")]
        ProtocolIdentifier,
        [Description("DCS")]
        DataCodingScheme,
        [Description("VP")]
        ValidityPeriod,
        [Description("SCTS")]
        ServiceCenterTimestamp,
        [Description("UD")]
        UserData
    }
}
