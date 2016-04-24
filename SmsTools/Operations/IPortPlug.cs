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
        Task SendAsync(string data);
        Task<string> ReceiveAsync();
        Exception LastError { get; }
    }
}
