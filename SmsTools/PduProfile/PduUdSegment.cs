using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public PduSegment Type { get { return PduSegment.UserData; } }


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

        public PduUdSegment()
        {
        }

        public void SetUserData(string value)
        {
            _source = value ?? string.Empty;
            encode(_source, _dcs.GetCoder());
        }

        public int Length()
        {
            return _length + 1;
        }

        public override string ToString()
        {
            return $"{_length.ToString("X2")}{_data}";
        }

        private void encode(string source, ICoder coder)
        {
            _data = coder.Encode(source, out _length);
        }
    }
}
