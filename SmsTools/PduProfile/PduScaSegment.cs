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
    /// SCA
    /// </summary>
    public class PduScaSegment : IPduSegment
    {
        private string _type = string.Empty;
        private string _address = string.Empty;
        private long _addressValue = 0;
        private int _length = 0;
        private int _bytesToRead = 0;

        public PduSegment Type { get { return PduSegment.ServiceCenterAddress; } }
        public bool HasVariableLength { get { return true; } }
        public bool HasInternationalNumbering { get; private set; }

        public PduScaSegment(IPduProfileSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.ServiceCenterAddress != null)
            {
                if (settings.ServiceCenterAddress.AddressType < 0 || settings.ServiceCenterAddress.AddressType > 255)
                    throw new ArgumentException("Address type out of range.");

                if (settings.ServiceCenterAddress.AddressValue < 0)
                    throw new ArgumentException("Address value out of range.");

                _type = settings.ServiceCenterAddress.AddressType.ToString("X2");
                _addressValue = settings.ServiceCenterAddress.AddressValue;
                _address = settings.ServiceCenterAddress.AddressValue.ToBdcString();
                _length = _address.OctetsCount() + 1;
                HasInternationalNumbering = settings.ServiceCenterAddress.AddressType == Constants.InternationalAddressType;
            }
        }

        public int Length()
        {
            return _length + 1;
        }

        public override string ToString()
        {
            return $"{_length:X2}{_type}{_address}";
        }

        public void SetAddress(long address)
        {
            if (address < 0)
                throw new ArgumentException("Address value out of range.");

            _address = address.ToBdcString();
            _addressValue = address;
            _length = _address.OctetsCount() + 1;
        }

        public long GetAddress()
        {
            return _addressValue;
        }

        public bool HasAddress()
        {
            return _addressValue > 0;
        }

        public int BytesToRead(byte segmentLength)
        {
            return _bytesToRead = segmentLength;
        }

        public bool Read(string segmentValue)
        {
            _addressValue = 0L;

            try
            {
                if (string.IsNullOrWhiteSpace(segmentValue) || segmentValue.Length % 2 > 0 || segmentValue.OctetsCount() != _bytesToRead || !Regex.IsMatch(segmentValue, @"^[a-fA-F0-9]+$"))
                    return false;

                _type = segmentValue.Substring(0, 2);
                _address = segmentValue.Substring(2);
                _length = _bytesToRead;
                HasInternationalNumbering = int.Parse(_type, NumberStyles.HexNumber) == Constants.InternationalAddressType;

                var bytes = _address.FromBdc();
                _addressValue = bytes.FromRBcdToDec();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValid()
        {
            return HasAddress();
        }
    }
}
