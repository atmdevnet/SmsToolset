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
    /// PID
    /// </summary>
    public class PduPidSegment : IPduSegment
    {
        /// <summary>
        /// default: sms
        /// </summary>
        private int _pid = 0;

        public PduSegment Type { get { return PduSegment.ProtocolIdentifier; } }
        public bool HasVariableLength { get { return false; } }

        public PduPidSegment(IPduProfileSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.ProtocolIdentifier != null)
            {
                if (settings.ProtocolIdentifier.Value < 0 || settings.ProtocolIdentifier.Value > 255)
                    throw new ArgumentException("Protocol id out of range.");

                _pid = settings.ProtocolIdentifier.Value;
            }
        }

        public int Length()
        {
            return 1;
        }

        public override string ToString()
        {
            return $"{_pid.ToString("X2")}";
        }

        public int BytesToRead(byte segmentLength = 0)
        {
            return Length();
        }

        public bool Read(string segmentValue)
        {
            _pid = -1;

            try
            {
                if (string.IsNullOrWhiteSpace(segmentValue) || segmentValue.Length % 2 > 0 || segmentValue.OctetsCount() != Length() || !Regex.IsMatch(segmentValue, @"^[a-fA-F0-9]+$"))
                    return false;

                _pid = int.Parse(segmentValue, NumberStyles.HexNumber);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValid()
        {
            return _pid >= 0x00 && _pid <= 0xff;
        }
    }
}
