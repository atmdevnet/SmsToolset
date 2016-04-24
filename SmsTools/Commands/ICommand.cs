using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.ModemCommands
{
    public interface ICommand
    {
        string GetCommand(ICommandParameters parameters);
    }
}
