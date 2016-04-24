using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// Default 7-bit encoding.
    /// </summary>
    public class DefaultCoder : ICoder
    {
        private int _maxLength = 160;


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
