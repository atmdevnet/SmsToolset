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
    /// 16-bit encoding.
    /// </summary>
    public class UCS2Coder : ICoder
    {
        public int MaxLength { get { return 70; } }


        public string Decode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 4 || value.Trim().Length % 2 > 0 || !Regex.IsMatch(value, @"^[a-fA-F0-9]+$"))
                return string.Empty;

            var source = value.Trim();
            int outputLength = source.Length >> 2;

            var chars = new StringBuilder();
            for (int c = 0; chars.Length < outputLength; chars.Append(Convert.ToChar(UInt16.Parse(source.Substring(c, 4), NumberStyles.HexNumber))), c += 4) { }

            return chars.ToString();
        }

        public string Encode(string value, out int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                length = 0;
                return string.Empty;
            }

            var source = value.Take(MaxLength);

            length = source.Count() * 2;
            return string.Concat<string>(source.Select(c => Convert.ToUInt16(c).ToString("X4")));
        }
    }
}
