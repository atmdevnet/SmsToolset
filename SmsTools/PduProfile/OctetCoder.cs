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
    /// 8-bit encoding.
    /// </summary>
    public class OctetCoder : ICoder
    {
        public int MaxLength { get { return 140; } }


        public string Decode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2 || value.Trim().Length % 2 > 0 || !Regex.IsMatch(value, @"^[a-fA-F0-9]+$"))
                return string.Empty;

            var source = value.Trim();

            var bytes = new byte[source.Length >> 1];
            for (int c = 0; c < source.Length - 1; bytes[c >> 1] = byte.Parse(source.Substring(c, 2), NumberStyles.HexNumber), c += 2) { }

            return new string(UTF8Encoding.UTF8.GetChars(bytes));
        }

        public string Encode(string value, out int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                length = 0;
                return string.Empty;
            }

            var chars = value.ToCharArray();
            var bytes = UTF8Encoding.UTF8.GetBytes(chars);

            var ccount = UTF8Encoding.UTF8.GetCharCount(bytes, 0, Math.Min(bytes.Length, MaxLength));
            var bcount = UTF8Encoding.UTF8.GetByteCount(chars, 0, ccount - 1);

            var result = new StringBuilder();
            for (int b = 0; b < bcount; result.Append(bytes[b++].ToString("X2"))) { }

            length = bcount;
            return result.ToString();
        }
    }
}
