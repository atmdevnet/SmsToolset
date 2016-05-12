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
    /// SCTS
    /// </summary>
    public class PduSctsSegment : IPduSegment
    {
        private DateTimeOffset _timestamp = new DateTimeOffset();
        private string _scts = string.Empty;

        public PduSegment Type { get { return PduSegment.ServiceCenterTimestamp; } }
        public bool HasVariableLength { get { return false; } }

        public PduSctsSegment(IPduProfileSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");
        }

        public int Length()
        {
            return 7;
        }

        public int BytesToRead(byte segmentLength = 0)
        {
            return Length();
        }

        public bool Read(string segmentValue)
        {
            _timestamp = new DateTimeOffset();

            try
            {
                if (string.IsNullOrWhiteSpace(segmentValue) || segmentValue.Length % 2 > 0 || segmentValue.OctetsCount() != Length() || !Regex.IsMatch(segmentValue, @"^[a-fA-F0-9]+$"))
                    return false;

                _scts = segmentValue;

                var bytes = segmentValue.FromBdc();
                var offset = getTimeOffset(bytes[6]);
                var timestamp = new DateTimeOffset(CultureInfo.CurrentCulture.DateTimeFormat.Calendar.ToFourDigitYear(bytes[0].FromRBcdToDec()), bytes[1].FromRBcdToDec(), bytes[2].FromRBcdToDec(), bytes[3].FromRBcdToDec(), bytes[4].FromRBcdToDec(), bytes[5].FromRBcdToDec(), TimeSpan.FromHours(offset));

                _timestamp = timestamp;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValid()
        {
            return _timestamp != new DateTimeOffset();
        }

        public DateTime LocalDateTime()
        {
            return _timestamp.LocalDateTime;
        }

        public override string ToString()
        {
            return _scts;
        }


        private int getTimeOffset(byte value)
        {
            bool negative = (value & 0x80) > 0;
            byte quarters = (byte)(value & ~0x80);

            return (quarters.FromRBcdToDec() >> 2) * (negative ? -1 : 1);
        }
    }
}
