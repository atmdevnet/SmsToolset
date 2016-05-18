using SmsTools.Commands;
using SmsTools.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools
{
    /// <summary>
    /// Queries of basic information.
    /// </summary>
    public class DeviceStatus
    {
        private IATCommand _generalInfo = null;
        private IATCommand _rssi = null;
        private IATCommand _rscp = null;
        private IATCommand _sysInfo = null;
        private ICommandParameter _empty = null;


        public DeviceStatus()
        {
            init();
        }

        /// <summary>
        /// Doesn't require PIN.
        /// </summary>
        public async Task<string> GeneralInfo(IPortPlug port)
        {
            await _generalInfo.ExecuteAsync(port);
            return _generalInfo.Succeeded() ? _generalInfo.Response : port?.LastError?.Message ?? "operation failed";
        }

        /// <summary>
        /// Requires PIN.
        /// </summary>
        public async Task<string> ReceivedSignalStrength(IPortPlug port)
        {
            await _rssi.ExecuteAsync(port);
            return _rssi.Succeeded() ? _rssi.Response : port?.LastError?.Message ?? "operation failed";
        }

        /// <summary>
        /// Requires PIN.
        /// </summary>
        public async Task<string> ReceivedSignalCodePower(IPortPlug port)
        {
            await _rscp.ExecuteAsync(port);
            return _rscp.Succeeded() ? _rscp.Response : port?.LastError?.Message ?? "operation failed";
        }

        /// <summary>
        /// Doesn't require PIN.
        /// </summary>
        public async Task<string> SystemInfo(IPortPlug port)
        {
            await _sysInfo.ExecuteAsync(port);
            return _sysInfo.Succeeded() ? _sysInfo.Response : port?.LastError?.Message ?? "operation failed";
        }


        private void init()
        {
            _empty = CommandParameter.CreateEmpty(Constants.BasicSuccessfulResponse);

            _generalInfo = new SimpleATCommand(ATCommand.DefaultInfo.Command(), _empty);
            _rssi = new SimpleATCommand(ATCommand.ReceivedSignalStrengthInfo.Command(), _empty);
            _rscp = new SimpleATCommand(ATCommand.ReceivedSignalCodePowerInfo.Command(), _empty);
            _sysInfo = new SimpleATCommand(ATCommand.SystemInfo.Command(), _empty);
        }
}
}
