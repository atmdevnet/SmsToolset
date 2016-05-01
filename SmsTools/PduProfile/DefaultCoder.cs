using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// Default 7-bit encoding.
    /// </summary>
    public class DefaultCoder : ICoder
    {
        public int MaxLength { get { return 160; } }


        public string Decode(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2 || value.Trim().Length % 2 > 0 || !Regex.IsMatch(value, @"^[a-fA-F0-9]+$"))
                return string.Empty;

            var source = value.Trim();

            var packed = new byte[source.Length >> 1];
            for (int p = 0; p < source.Length - 1; packed[p >> 1] = byte.Parse(source.Substring(p, 2), NumberStyles.HexNumber), p += 2) { }

            var bits = new BitBag(packed);
            var unpacked = bits.Unpack();

            return ASCIIEncoding.ASCII.GetString(unpacked);
        }

        public string Encode(string value, out int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                length = 0;
                return string.Empty;
            }

            var bytes = ASCIIEncoding.ASCII.GetBytes(value.ToCharArray(), 0, Math.Min(value.Length, MaxLength));

            var bits = new BitBag();
            for (int b = 0; b < bytes.Length; bits.Pack(bytes[b++])) { }
            var outbytes = bits.ToOctets();

            var result = new StringBuilder();
            for (int b = 0; b < outbytes.Length; result.Append(outbytes[b++].ToString("X2"))) { }

            length = bytes.Length;
            return result.ToString();
        }
    }
}
