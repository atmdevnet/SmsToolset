using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools
{
    internal static class AppHelper
    {
        internal static string ToBdcString(this long value)
        {
            var str = value.ToString();
            var chars = str.PadRight(str.Length + str.Length % 2, 'F').ToCharArray();
            var octets = new StringBuilder();
            for (int c = 0; c < chars.Length - 1; octets.Append(chars[c + 1]).Append(chars[c]), c += 2) { }
            return octets.ToString();
        }

        internal static byte[] FromBdc(this string value)
        {
            var bytes = new byte[value.Length >> 1];
            for (int c = 0; c < value.Length - 1; bytes[c >> 1] = byte.Parse(new string(new char[] { value[c + 1], value[c] }), NumberStyles.HexNumber), c += 2) { }
            return bytes;
        }

        internal static long FromRBcdToDec(this byte[] value)
        {
            long result = 0L;
            var reversed = value.SelectMany(v => new byte[] { (byte)(v >> 4), (byte)(v & 0x0f) }).Reverse();
            int exp = 0;
            foreach (var v in reversed.Skip(reversed.First() == 0x0f ? 1 : 0))
            {
                result += v * (long)Math.Pow(10, exp++);
            }

            return result;
        }

        internal static int FromRBcdToDec(this byte value)
        {
            return ((value >> 4) * 10) + (value & 0x0f);
        }

        internal static int DigitsCount(this long value)
        {
            return value.ToString().Length;
        }

        internal static int OctetsCount(this string value)
        {
            return (value.Length + value.Length % 2) >> 1;
        }

        internal static TResult GetEnumAttributeValue<TAttribute, TResult>(this Enum value, Func<TAttribute, TResult> valueSelector)
        {
            MemberInfo member = value.GetType().GetMember(Enum.GetName(value.GetType(), value)).FirstOrDefault();
            if (member != null)
            {
                TAttribute attribute = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().FirstOrDefault();
                return attribute == null ? default(TResult) : valueSelector(attribute);
            }
            else
            {
                return default(TResult);
            }
        }


    }
}
