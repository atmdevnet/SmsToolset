using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public class CommandParameter : ICommandParameter
    {
        public CommandParameter(string value, string successfulResponsePattern, bool ignoreCase, bool useCommand)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(successfulResponsePattern))
                throw new ArgumentException("Value or/and response pattern not specified.");

            Value = value;
            SuccessfulResponsePattern = successfulResponsePattern;
            IgnoreCase = ignoreCase;
            UseCommand = useCommand;
        }

        public bool IgnoreCase { get; private set; }
        public bool IsEmpty { get; private set; }
        public string SuccessfulResponsePattern { get; private set; }
        public string Value { get; private set; }
        public bool UseCommand { get; private set; }

        public bool IsResponseSuccessful(string response)
        {
            return !string.IsNullOrWhiteSpace(response) && Regex.IsMatch(response, SuccessfulResponsePattern, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public static ICommandParameter CreateEmpty(string successfulResponsePattern, bool ignoreCase)
        {
            return new CommandParameter("empty", successfulResponsePattern, ignoreCase, false) { Value = string.Empty, IsEmpty = true };
        }

        private CommandParameter()
        {
        }
    }
}
