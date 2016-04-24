using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public interface IPduProfile
    {
        string Name { get; }
        bool CanSubmit();
        bool CanDeliver();
        string GetPacket(long destination, string message, out int length);
        bool HasInternationalNumbering();
        bool HasExtendedCharacterSet();
        DCS GetDataCodingScheme();
        bool IsServiceCenterAddressDefined();
        IEnumerable<IPduSegment> PacketSegments();
        IPduProfileSettings Settings { get; }
        Dictionary<PduSegment, IPduSegment> SegmentType();
    }
}
