using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools.ModemCommands
{
    public class ATCommands
    {
        private Dictionary<ATCommand, Type> _commandType = new Dictionary<ATCommand, Type>();


        public ATCommands()
        {
            createCommandTypes();
        }

        public string Create(ATCommand command, ICommandParameters parameters)
        {
            return command.HasParameters() ? createCommand(command).GetCommand(parameters) : command.Command();
        }

        public string GetResponse(string modemOutput, out bool succeeded)
        {
            if (string.IsNullOrEmpty(modemOutput))
            {
                succeeded = false;
                return string.Empty;
            }

            var match = Regex.Match(modemOutput, @"(?:(.*?)\s*(ok|error|\s*)\s*)$", RegexOptions.IgnoreCase);

            succeeded = match.Success && match.Groups.Count == 3 && match.Groups[2].Value.Equals("ok", StringComparison.OrdinalIgnoreCase);
            return match.Success && match.Groups.Count == 3 ? match.Groups[1].Value : string.Empty;
        }


        private void createCommandTypes()
        {
            //_commandType[ATCommand.PinAuthenticate] = typeof(PinCommand);
            //_commandType[ATCommand.MessageFormat] = typeof(MessageFormatCommand);
            //_commandType[ATCommand.MessageSend] = typeof(MessageSendCommand);
        }

        private ICommand createCommand(ATCommand cmd)
        {
            var type = _commandType[cmd];
            var ctor = type.GetConstructor(Type.EmptyTypes);
            return ctor.Invoke(null) as ICommand;
        }
    }
}
