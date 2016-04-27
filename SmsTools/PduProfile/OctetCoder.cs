using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// 8-bit encoding.
    /// </summary>
    public class OctetCoder : ICoder
    {
        public int MaxLength { get { return 140; } }


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
