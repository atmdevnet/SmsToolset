using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public interface ICommandParameter
    {
        bool IsEmpty { get; }
        string Value { get; }
        string SuccessfulResponsePattern { get; }
        bool IgnoreCase { get; }
        bool IsResponseSuccessful(string response);
        bool IsNextParameter { get; }
    }
}
