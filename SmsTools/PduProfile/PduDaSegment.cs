using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// DA
    /// </summary>
    public class PduDaSegment : IPduSegment
    {
        private string _type = string.Empty;
        private string _address = string.Empty;
        private long _addressValue = 0;
        private int _length = 0;

        public PduSegment Type { get { return PduSegment.DestinationAddress; } }
        public bool HasInternationalNumbering { get; private set; }

        public PduDaSegment(IPduProfileSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.DestinationAddress != null)
            {
                if (settings.DestinationAddress.AddressType < 0 || settings.DestinationAddress.AddressType > 255)
                    throw new ArgumentException("Address type out of range.");

                if (settings.DestinationAddress.AddressValue < 0)
                    throw new ArgumentException("Address value out of range.");

                _type = settings.DestinationAddress.AddressType.ToString("X2");
                _address = settings.DestinationAddress.AddressValue.ToBdcString();
                _addressValue = settings.DestinationAddress.AddressValue;
                _length = _addressValue.DigitsCount();
                HasInternationalNumbering = settings.DestinationAddress.AddressType == Constants.InternationalAddressType;
            }
        }

        public PduDaSegment()
        {
        }

        public int Length()
        {
            return _address.OctetsCount() + 2;
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
            _length = _addressValue.DigitsCount();
        }

        public long GetAddress()
        {
            return _addressValue;
        }

        public bool HasAddress()
        {
            return _addressValue > 0;
        }
    }
}
