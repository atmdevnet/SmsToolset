using SmsTools.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public class StepwiseATCommand : SimpleATCommand
    {
        private IEnumerable<ICommandParameter> _steps = null;
        private string _currentCommand = string.Empty;
        private string _currentParam = string.Empty;
        private string _currentOperator = string.Empty;

        public StepwiseATCommand(string command, IEnumerable<ICommandParameter> steps)
            : base(command)
        {
            if (steps == null || steps.Count() < 2 || steps.Any(p => p.IsEmpty || string.IsNullOrWhiteSpace(p.Value) || string.IsNullOrWhiteSpace(p.SuccessfulResponsePattern)))
                throw new ArgumentException("Parameters not specified or empty.");

            Command = Command.TrimEnd('=');
            Parameter = steps.Last();
            _steps = steps;
            HasParameters = true;
            HasSteps = true;
        }

        public override async Task<string> ExecuteAsync(IPortPlug port)
        {
            if (port == null || !port.IsOpen)
            {
                Response = string.Empty;
                return string.Empty;
            }

            await Task.Yield();

            foreach (var step in _steps)
            {
                _currentCommand = step.IsNextParameter ? string.Empty : Command;
                _currentOperator = step.IsNextParameter ? string.Empty : "=";
                _currentParam = step.Value;
                Parameter = step;

                Task.WaitAny(Task.Run(async () => { await base.ExecuteAsync(port); }));
                if (!base.Succeeded())
                    break;
            }

            return Response;
        }

        protected override string prepareCommand()
        {
            return $"{_currentCommand}{_currentOperator}{_currentParam}";
        }
    }
}
