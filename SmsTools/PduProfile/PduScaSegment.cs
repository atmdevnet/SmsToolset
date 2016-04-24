using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public PduSegment Type { get { return PduSegment.ServiceCenterAddress; } }
        public bool HasInternationalNumbering { get; private set; }
        public static PduScaSegment Empty = new PduScaSegment();

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

        public PduScaSegment()
        {
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
    }
}
