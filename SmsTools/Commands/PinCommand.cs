using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.ModemCommands
{
    public class PinCommand : ICommand
    {
        public string GetCommand(ICommandParameters parameters)
        {
            if (parameters == null || parameters.Command != ATCommand.PinAuthenticate)
                throw new ArgumentException("No parameters or invalid command id.");

            var cmd = ATCommand.PinAuthenticate;

            return $"{cmd.Command()}{string.Format(cmd.ParametersFormat(), parameters.Pin)}";
        }
    }
}
