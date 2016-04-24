using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    public class PduDefaultProfile : IPduProfile
    {
        private IEnumerable<IPduSegment> _segments = Enumerable.Empty<IPduSegment>();
        private Dictionary<PduSegment, IPduSegment> _segmentType = new Dictionary<PduSegment, IPduSegment>();

        public string Name { get; internal set; } = "default";
        public IPduProfileSettings Settings { get; private set; }

        public PduDefaultProfile(IPduProfileSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");

            Settings = settings;

            createSegments(settings);
            createSequence(settings);
        }

        public bool CanDeliver()
        {
            return _segmentType.ContainsKey(PduSegment.PduHeader) && (_segmentType[PduSegment.PduHeader] as PduHeaderSegment).GetMessageType() == MTI.Delivery;
        }

        public bool CanSubmit()
        {
            return _segmentType.ContainsKey(PduSegment.PduHeader) && (_segmentType[PduSegment.PduHeader] as PduHeaderSegment).GetMessageType() == MTI.Submit;
        }

        public bool HasExtendedCharacterSet()
        {
            return _segmentType.ContainsKey(PduSegment.DataCodingScheme) && (_segmentType[PduSegment.DataCodingScheme] as PduDcsSegment).GetCodingScheme() > DCS.Default;
        }

        public bool HasInternationalNumbering()
        {
            return _segmentType.ContainsKey(PduSegment.DestinationAddress) && (_segmentType[PduSegment.DestinationAddress] as PduDaSegment).HasInternationalNumbering;
        }

        public bool IsServiceCenterAddressDefined()
        {
            return _segmentType.ContainsKey(PduSegment.ServiceCenterAddress) && (_segmentType[PduSegment.ServiceCenterAddress] as PduScaSegment).HasAddress();
        }

        public DCS GetDataCodingScheme()
        {
            return _segmentType.ContainsKey(PduSegment.DataCodingScheme) ? 
                (_segmentType[PduSegment.DataCodingScheme] as PduDcsSegment).GetCodingScheme()
                : DCS.Other;
        }

        public string GetPacket(long destination, string message, out int length)
        {
            setDestination(destination);
            setMessage(message);

            var packet = string.Concat<string>(_segments.Select(s => s.ToString()));
            length = _segments.Skip(1).Sum(s => s.Length());

            return packet;
        }

        public IEnumerable<IPduSegment> PacketSegments()
        {
            return _segments;
        }

        public Dictionary<PduSegment, IPduSegment> SegmentType()
        {
            return _segmentType;
        }


        private void createSegments(IPduProfileSettings settings)
        {
            var dcs = new PduDcsSegment(settings);

            _segmentType[PduSegment.ServiceCenterAddress] = new PduScaSegment(settings);
            _segmentType[PduSegment.PduHeader] = new PduHeaderSegment(settings);
            _segmentType[PduSegment.MessageReference] = new PduMrSegment(settings);
            _segmentType[PduSegment.DestinationAddress] = new PduDaSegment(settings);
            _segmentType[PduSegment.ProtocolIdentifier] = new PduPidSegment(settings);
            _segmentType[PduSegment.DataCodingScheme] = dcs;
            _segmentType[PduSegment.ValidityPeriod] = new PduVpSegment(settings);
            _segmentType[PduSegment.UserData] = new PduUdSegment(settings, dcs);
        }

        private void createSequence(IPduProfileSettings settings)
        {
            _segments = settings.Sequence.Select(s => _segmentType[s]);
        }

        private void setDestination(long destination)
        {
            (_segmentType[PduSegment.DestinationAddress] as PduDaSegment).SetAddress(destination);
        }

        private void setMessage(string message)
        {
            (_segmentType[PduSegment.UserData] as PduUdSegment).SetUserData(message);
        }
    }
}
