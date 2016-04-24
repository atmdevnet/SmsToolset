using SmsTools.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public interface IATCommand
    {
        string Command { get; }
        ICommandParameter Parameter { get; }
        bool HasParameters { get; }
        bool HasSteps { get; }
        Task<string> ExecuteAsync(IPortPlug port);
        string Response { get; }
        bool Succeeded();
    }
}
