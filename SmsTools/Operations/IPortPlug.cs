using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Operations
{
    public interface IPortPlug : IDisposable
    {
        bool IsOpen { get; }
        Task<string> SendAndReceiveAsync(string data);
        Exception LastError { get; }
        dynamic GetConfig();
    }
}
