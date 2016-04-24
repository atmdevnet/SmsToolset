using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsTools.Operations;
using System.Text.RegularExpressions;

namespace SmsTools.Commands
{
    public class SimpleATCommand : IATCommand
    {
        public string Command { get; protected set; }
        public ICommandParameter Parameter { get; protected set; }
        public bool HasParameters { get; protected set; } = false;
        public bool HasSteps { get; protected set; } = false;
        public string Response { get; protected set; }

        public SimpleATCommand(string command, ICommandParameter emptyParameter)
        {
            init(command);

            if (emptyParameter == null || !emptyParameter.IsEmpty || string.IsNullOrWhiteSpace(emptyParameter.SuccessfulResponsePattern))
                throw new ArgumentException("Parameter not empty or not specified.");

            Parameter = emptyParameter;
        }

        protected SimpleATCommand(string command)
        {
            init(command);
        }

        public virtual async Task<string> ExecuteAsync(IPortPlug port)
        {
            if (port == null || !port.IsOpen)
            {
                Response = string.Empty;
                return string.Empty;
            }

            await port.SendAsync(prepareCommand());
            Response = await port.ReceiveAsync();

            return Response;
        }

        public virtual bool Succeeded()
        {
            return Parameter.IsResponseSuccessful(Response);
        }

        protected virtual string prepareCommand()
        {
            return $"{this.Command}\r\n";
        }


        private void init(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command not specified.");

            Command = command.Trim();
        }
    }
}
