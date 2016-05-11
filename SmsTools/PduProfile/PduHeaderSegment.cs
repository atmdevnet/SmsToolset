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
    /// PDU header
    /// </summary>
    public abstract class PduHeaderSegment : IPduSegment
    {
        protected int _header = 0;

        public PduSegment Type { get { return PduSegment.PduHeader; } }
        public bool HasVariableLength { get { return false; } }


        public PduHeaderSegment(IPduProfileSettings settings)
        {
            if (settings == null || settings.PduHeader == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.PduHeader.Value < 0 || settings.PduHeader.Value > 255)
                throw new ArgumentException("Value out of range.");

            _header = settings.PduHeader.Value;
        }

        public virtual int Length()
        {
            return 1;
        }

        public override string ToString()
        {
            return $"{_header.ToString("X2")}";
        }

        public virtual void SetSameReplyPath(bool value)
        {
            setHeader(getMask(7, value), value);
        }

        public virtual bool IsSameReplyPath()
        {
            return (_header & getMask(7, true)) > 0;
        }

        public virtual void SetUserDataContainsHeader(bool value)
        {
            setHeader(getMask(6, value), value);
        }

        public virtual bool UserDataContainsHeader()
        {
            return (_header & getMask(6, true)) > 0;
        }

        public virtual void SetMessageType(MTI value)
        {
            int mask = (int)value;
            _header |= mask;
        }

        public virtual MTI GetMessageType()
        {
            int mask = 3;
            return (MTI)(_header & mask);
        }

        protected void setHeader(int mask, bool set)
        {
            _header = set ? _header | mask : _header & mask;
        }

        protected int getMask(int bitPosition, bool setter)
        {
            return setter ? 1 << bitPosition : ~(1 << bitPosition);
        }

        public virtual int BytesToRead(byte segmentLength = 0)
        {
            return Length();
        }

        public virtual bool Read(string segmentValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(segmentValue) || segmentValue.Length % 2 > 0 || segmentValue.OctetsCount() != Length() || !Regex.IsMatch(segmentValue, @"^[a-fA-F0-9]+$"))
                    return false;

                _header = int.Parse(segmentValue, NumberStyles.HexNumber);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }


    /// <summary>
    /// Validity Period Format
    /// </summary>
    public enum VPF
    {
        NotPresent,
        Enhanced,
        Relative,
        Absolute
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
