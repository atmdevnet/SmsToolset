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
            var chars = str.PadRight(str.Length + str.Length % 2, 'F').Reverse().ToArray();
            return string.Concat<string>(chars.Select((c, i) => i % 2 == 0 && i < chars.Length - 1 ? string.Concat(c, chars[i + 1]) : "").Reverse());
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
