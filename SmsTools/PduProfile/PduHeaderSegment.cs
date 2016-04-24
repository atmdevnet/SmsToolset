using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// PDU header
    /// </summary>
    public class PduHeaderSegment : IPduSegment
    {
        private int _header = 0;

        public PduSegment Type { get { return PduSegment.PduHeader; } }


        public PduHeaderSegment(IPduProfileSettings settings)
        {
            if (settings == null || settings.PduHeader == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.PduHeader.Value < 0 || settings.PduHeader.Value > 255)
                throw new ArgumentException("Value out of range.");

            _header = settings.PduHeader.Value;
        }

        public PduHeaderSegment()
        {
        }

        public int Length()
        {
            return 1;
        }

        public override string ToString()
        {
            return $"{_header.ToString("X2")}";
        }

        public void SetSameReplyPath(bool value)
        {
            setHeader(getMask(7, value), value);
        }

        public bool IsSameReplyPath()
        {
            return (_header & getMask(7, true)) > 0;
        }

        public void SetUserDataContainsHeader(bool value)
        {
            setHeader(getMask(6, value), value);
        }

        public bool UserDataContainsHeader()
        {
            return (_header & getMask(6, true)) > 0;
        }

        public void SetStatusReportRequired(bool value)
        {
            setHeader(getMask(5, value), value);
        }

        public bool IsStatusReportRequired()
        {
            return (_header & getMask(5, true)) > 0;
        }

        public void SetValidityPeriodFormat(VPF value)
        {
            _header |= (int)value << 3;
        }

        public VPF GetValidityPeriodFormat()
        {
            int mask = 3 << 3;
            return (VPF)((_header & mask) >> 3);
        }

        public void SetRejectDuplicates(bool value)
        {
            setHeader(getMask(2, value), value);
        }

        public bool HasRejectDuplicates()
        {
            return (_header & getMask(2, true)) > 0;
        }

        public void SetMessageType(MTI value)
        {
            int mask = (int)value;
            _header |= mask;
        }

        public MTI GetMessageType()
        {
            int mask = 3;
            return (MTI)(_header & mask);
        }

        private void setHeader(int mask, bool set)
        {
            _header = set ? _header | mask : _header & mask;
        }

        private int getMask(int bitPosition, bool setter)
        {
            return setter ? 1 << bitPosition : ~(1 << bitPosition);
        }
    }


    /// <summary>
    /// Validity Period Format
    /// </summary>
    public enum VPF
    {
        Invalid,
        ValidAbsolute,
        ValidRelative,
        ValidAbsolute2
    }


    /// <summary>
    /// Message Type Indicator
    /// </summary>
    public enum MTI
    {
        Delivery,
        Submit,
        StatusCommand,
        Reserved
    }
}
