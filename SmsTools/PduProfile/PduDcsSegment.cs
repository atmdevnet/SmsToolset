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
    /// DCS
    /// </summary>
    public class PduDcsSegment : IPduSegment
    {
        /// <summary>
        /// default: 7-bit coding scheme
        /// </summary>
        private int _dcs = 0;

        private Dictionary<DCS, Type> _coder = new Dictionary<DCS, System.Type>();

        public PduSegment Type { get { return PduSegment.DataCodingScheme; } }
        public bool HasVariableLength { get { return false; } }

        public PduDcsSegment(IPduProfileSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.DataCodingScheme != null)
            {
                if (settings.DataCodingScheme.Value < 0 || settings.DataCodingScheme.Value > 255)
                    throw new ArgumentException("Data coding scheme out of range.");

                _dcs = settings.DataCodingScheme.Value;
            }

            createCoders();
        }

        public int Length()
        {
            return 1;
        }

        public override string ToString()
        {
            return $"{_dcs.ToString("X2")}";
        }

        public void SetCodingScheme(DCS value)
        {
            if (value != DCS.Other)
            {
                _dcs = (int)value;
            }
        }

        public DCS GetCodingScheme()
        {
            return Enum.IsDefined(typeof(DCS), _dcs) ? (DCS)_dcs : DCS.Other;
        }

        public ICoder GetCoder()
        {
            var coderType = _coder[GetCodingScheme()];
            var ctor = coderType.GetConstructor(System.Type.EmptyTypes);
            return ctor.Invoke(null) as ICoder;
        }

        public int BytesToRead(byte segmentLength = 0)
        {
            return Length();
        }

        public bool Read(string segmentValue)
        {
            _dcs = (int)DCS.Other;

            try
            {
                if (string.IsNullOrWhiteSpace(segmentValue) || segmentValue.Length % 2 > 0 || segmentValue.OctetsCount() != Length() || !Regex.IsMatch(segmentValue, @"^[a-fA-F0-9]+$"))
                    return false;

                var value = int.Parse(segmentValue, NumberStyles.HexNumber);
                _dcs = Enum.IsDefined(typeof(DCS), value) ? value : (int)DCS.Other;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValid()
        {
            return GetCodingScheme() != DCS.Other;
        }


        private void createCoders()
        {
            _coder[DCS.Default] = typeof(DefaultCoder);
            _coder[DCS.Octet] = typeof(OctetCoder);
            _coder[DCS.UCS2] = typeof(UCS2Coder);
            _coder[DCS.Other] = typeof(UndefinedEncoder);
        }


        /// <summary>
        /// dummy encoder
        /// </summary>
        public class UndefinedEncoder : ICoder
        {
            public int MaxLength { get { return 0; } }

            public string Decode(string value)
            {
                throw new NotImplementedException();
            }

            public string Encode(string value, out int length)
            {
                throw new NotImplementedException();
            }
        }
    }


    /// <summary>
    /// Common data coding schemes
    /// </summary>
    public enum DCS
    {
        Other = -1,
        Default = 0,
        Octet = 4,
        UCS2 = 8
    }
}
