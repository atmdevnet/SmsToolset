using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// 16-bit encoding.
    /// </summary>
    public class UCS2Coder : ICoder
    {
        private int _maxLength = 70;


        public string Decode(string value)
        {
            throw new NotImplementedException();
        }

        public string Encode(string value, out int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                length = 0;
                return string.Empty;
            }

            var source = string.Concat<char>(value.Take(_maxLength));

            length = source.Length * 2;
            return string.Concat<string>(source.Select(c => Convert.ToInt32(c).ToString("X4")));
        }
    }
}
