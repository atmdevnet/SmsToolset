using SmsTools.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools
{
    public static partial class Extensions
    {
        public static string Command(this Enum value)
        {
            return value.GetEnumAttributeValue<ATCommandAttribute, string>(a => a.Command);
        }

        public static bool HasParameters(this Enum value)
        {
            return value.GetEnumAttributeValue<ATCommandAttribute, bool>(a => a.HasParameters);
        }

        public static bool HasSteps(this Enum value)
        {
            return value.GetEnumAttributeValue<ATCommandAttribute, bool>(a => a.HasSteps);
        }

        public static bool AllowsAnonymous(this Enum value)
        {
            return value.GetEnumAttributeValue<ATCommandAttribute, bool>(a => a.AllowsAnonymous);
        }

        public static string ToValueString(this Enum value)
        {
            return Convert.ToInt32(value).ToString();
        }

        public static string Description(this Enum value)
        {
            return value.GetEnumAttributeValue<DescriptionAttribute, string>(a => a.Description);
        }

        public static Constants.MessageStorage ToMessageStorage(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Constants.MessageStorage.Unspecified;

            return Enum.GetValues(typeof(Constants.MessageStorage)).Cast<Constants.MessageStorage>().FirstOrDefault(e => e.Description().Equals(value));
        }
    }
}
