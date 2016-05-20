using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// Default profile implementation.
    /// </summary>
    public class PduDefaultProfile : IPduProfile
    {
        private IEnumerable<IPduSegment> _segments = Enumerable.Empty<IPduSegment>();
        private Dictionary<PduSegment, IPduSegment> _segmentType = new Dictionary<PduSegment, IPduSegment>();

        public string Name { get; internal set; } = "default";
        public IPduProfileSettings Settings { get; private set; }

        public PduDefaultProfile(IPduProfileSettings settings)
        {
            if (settings == null || !sequenceIsValid(settings))
                throw new ArgumentNullException("Profile settings not specified or not valid.");

            Settings = settings;

            Reset();
        }

        public bool CanDeliver()
        {
            return (_segments.Single(s => s.Type == PduSegment.PduHeader) as PduHeaderSegment).GetMessageType() == MTI.Delivery;
        }

        public bool CanSubmit()
        {
            return (_segments.Single(s => s.Type == PduSegment.PduHeader) as PduHeaderSegment).GetMessageType() == MTI.Submit;
        }

        public bool HasExtendedCharacterSet()
        {
            return (_segments.Single(s => s.Type == PduSegment.DataCodingScheme) as PduDcsSegment).GetCodingScheme() > DCS.Default;
        }

        public bool HasInternationalNumbering()
        {
            return (_segments.Single(s => s.Type == PduSegment.DestinationAddress) as PduDaSegment).HasInternationalNumbering;
        }

        public bool IsServiceCenterAddressDefined()
        {
            return (_segments.Single(s => s.Type == PduSegment.ServiceCenterAddress) as PduScaSegment).HasAddress();
        }

        public DCS GetDataCodingScheme()
        {
            return (_segments.Single(s => s.Type == PduSegment.DataCodingScheme) as PduDcsSegment).GetCodingScheme();
        }

        /// <summary>
        /// Creates packet for specified destination and message.
        /// </summary>
        public string GetPacket(long destination, string message, out int length)
        {
            setDestination(destination);
            setMessage(message);

            var packet = string.Concat<string>(_segments.Select(s => s.ToString()));
            length = _segments.Skip(1).Sum(s => s.Length());

            return packet;
        }

        /// <summary>
        /// Creates packet based on parameters specified in settings object.
        /// </summary>
        public string GetPacket(out int length)
        {
            var packet = string.Concat<string>(_segments.Select(s => s.ToString()));
            length = _segments.Skip(1).Sum(s => s.Length());

            return packet;
        }

        /// <summary>
        /// Decodes message from specified PDU.
        /// </summary>
        public MessageDetails GetMessage(string packet, int length)
        {
            var result = new MessageDetails();

            if (string.IsNullOrWhiteSpace(packet) || length <= 0 || packet.Trim().Length < 28 || packet.Trim().Length % 2 > 0 || !Regex.IsMatch(packet, @"^[a-fA-F0-9]+$"))
                return result;

            var source = packet.Trim();

            var scaLength = byte.Parse(packet.Substring(0, 2), NumberStyles.HexNumber);

            if (source.Length - ((scaLength + 1) << 1) != (length << 1))
                return result;

            int byteIndex = 0;

            foreach (var segment in _segments)
            {
                int bytesToRead = segment.HasVariableLength ? segment.BytesToRead(byte.Parse(source.Substring(byteIndex, 2), NumberStyles.HexNumber)) : segment.BytesToRead(0);
                int shift = segment.HasVariableLength ? 2 : 0;

                if (!segment.Read(source.Substring(byteIndex + shift, bytesToRead << 1)))
                    break;

                byteIndex += ((bytesToRead << 1) + shift);
            }

            return getDecodedMessage(result);
        }

        /// <summary>
        /// Gets message from current profile content.
        /// </summary>
        public MessageDetails GetMessage()
        {
            return getDecodedMessage(new MessageDetails());
        }

        public IEnumerable<IPduSegment> PacketSegments()
        {
            return _segments;
        }

        public Dictionary<PduSegment, IPduSegment> SegmentType()
        {
            return _segmentType;
        }

        public void Reset()
        {
            createSegments();
            createSequence();
        }


        private bool sequenceIsValid(IPduProfileSettings settings)
        {
            return (settings.CanDeliver ^ settings.CanSubmit) && new HashSet<PduSegment>(settings.Sequence).Count == settings.Sequence.Count();
        }

        private void createSegments()
        {
            _segmentType.Clear();

            var header = Settings.CanSubmit ? new PduSendHeaderSegment(Settings) as IPduSegment : new PduReceiveHeaderSegment(Settings) as IPduSegment;

            var dcs = new PduDcsSegment(Settings);

            _segmentType[PduSegment.ServiceCenterAddress] = new PduScaSegment(Settings);
            _segmentType[PduSegment.PduHeader] = header;
            _segmentType[PduSegment.MessageReference] = new PduMrSegment(Settings);
            _segmentType[PduSegment.DestinationAddress] = new PduDaSegment(Settings);
            _segmentType[PduSegment.ProtocolIdentifier] = new PduPidSegment(Settings);
            _segmentType[PduSegment.DataCodingScheme] = dcs;
            _segmentType[PduSegment.ValidityPeriod] = new PduVpSegment(Settings);
            _segmentType[PduSegment.ServiceCenterTimestamp] = new PduSctsSegment(Settings);
            _segmentType[PduSegment.UserData] = new PduUdSegment(Settings, dcs);
        }

        private void createSequence()
        {
            _segments = Settings.Sequence.Select(s => _segmentType[s]);
        }

        private void setDestination(long destination)
        {
            (_segments.Single(s => s.Type == PduSegment.DestinationAddress) as PduDaSegment).SetAddress(destination);
        }

        private long getDestination()
        {
            return (_segments.Single(s => s.Type == PduSegment.DestinationAddress) as PduDaSegment).GetAddress();
        }

        private void setMessage(string message)
        {
            (_segments.Single(s => s.Type == PduSegment.UserData) as PduUdSegment).SetUserData(message);
        }

        private string getMessage()
        {
            return (_segments.Single(s => s.Type == PduSegment.UserData) as PduUdSegment).GetMessage();
        }

        private DateTime getTimestamp()
        {
            return _segments.Any(s => s.Type == PduSegment.ServiceCenterTimestamp) 
                ? (_segments.Single(s => s.Type == PduSegment.ServiceCenterTimestamp) as PduSctsSegment).LocalDateTime()
                : new DateTime();
        }

        private MessageDetails getDecodedMessage(MessageDetails msg)
        {
            msg.IsValid = _segments.All(s => s.IsValid());
            msg.Message = getMessage();
            msg.Sender = getDestination();
            msg.Date = getTimestamp();

            return msg;
        }
    }
}
