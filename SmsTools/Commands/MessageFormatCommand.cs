using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.ModemCommands
{
    public class MessageFormatCommand : ICommand
    {
        public string GetCommand(ICommandParameters parameters)
        {
            if (parameters == null || parameters.Command != ATCommand.MessageFormat)
                throw new ArgumentException("No parameters or invalid command id.");

            var cmd = ATCommand.MessageFormat;

            return $"{cmd.Command()}{string.Format(cmd.ParametersFormat(), parameters.MessageFormat.MessageFormat())}";
        }
    }
}
