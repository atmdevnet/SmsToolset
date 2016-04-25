using SmsTools.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public class ParamATCommand : SimpleATCommand
    {
        public ParamATCommand(string command, ICommandParameter parameter)
            : base(command)
        {
            if (parameter == null || parameter.IsEmpty || string.IsNullOrWhiteSpace(parameter.Value) || string.IsNullOrWhiteSpace(parameter.SuccessfulResponsePattern))
                throw new ArgumentException("Parameter not specified or empty.");

            Command = Command.TrimEnd('=');
            Parameter = parameter;
            HasParameters = true;
        }

        protected override string prepareCommand()
        {
            return $"{this.Command}={Parameter.Value.TrimStart('=')}\r\n";
        }
    }
}
