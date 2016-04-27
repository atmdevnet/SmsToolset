using System;
using System.Collections.Generic;
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
