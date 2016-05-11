using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// UD
    /// </summary>
    public class PduUdSegment : IPduSegment
    {
        private PduDcsSegment _dcs = null;
        private string _source = string.Empty;
        private int _length = 0;
        private string _data = string.Empty;
        private int _bytesToRead = 0;

        public PduSegment Type { get { return PduSegment.UserData; } }
        public bool HasVariableLength { get { return true; } }


        public PduUdSegment(IPduProfileSettings settings, PduDcsSegment dcs)
        {
            if (settings == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (dcs == null || dcs.GetCodingScheme() == DCS.Other)
                throw new NotSupportedException("Data coding scheme not supported.");

            _dcs = dcs;
            _source = settings?.UserData?.Value ?? string.Empty;

            encode(_source, dcs.GetCoder());
        }

        public void SetUserData(string value)
        {
            _source = value ?? string.Empty;
            encode(_source, _dcs.GetCoder());
        }

        public bool HasUserData()
        {
            return string.IsNullOrEmpty(_data);
        }

        public string GetMessage()
        {
            return _source;
        }

        public int Length()
        {
            return _length + 1;
        }

        public override string ToString()
        {
            return $"{_length.ToString("X2")}{_data}";
        }

        public int BytesToRead(byte segmentLength)
        {
            _bytesToRead = _dcs.GetCodingScheme() == DCS.Default ? (((segmentLength * 7) >> 3) + ((segmentLength * 7) % 8 > 0 ? 1 : 0)) : segmentLength;
            return _bytesToRead;
        }

        public bool Read(string segmentValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(segmentValue) || segmentValue.Length % 2 > 0 || segmentValue.OctetsCount() != _bytesToRead || !Regex.IsMatch(segmentValue, @"^[a-fA-F0-9]+$"))
                    return false;

                _data = segmentValue;
                _length = _dcs.GetCodingScheme() == DCS.Default ? ((_bytesToRead << 3) / 7) : _bytesToRead;

                decode(_data, _dcs.GetCoder());

                return true;
            }
            catch
            {
                return false;
            }
        }


        private void encode(string source, ICoder coder)
        {
            _data = coder.Encode(source, out _length);
        }

        private void decode(string source, ICoder coder)
        {
            _source = coder.Decode(source);
        }
    }
}
