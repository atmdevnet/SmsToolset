using SmsTools.Commands;
using SmsTools.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools
{
    public partial class ServiceCenter
    {
        private IATCommand _scaQuery = null;
        private ICommandParameter _emptyParam = null;

        public ServiceCenter()
        {
            _emptyParam = CommandParameter.CreateEmpty(Constants.BasicSuccessfulResponse, true);
            _scaQuery = new SimpleATCommand(ATCommand.ServiceCenterAddressInfo.Command(), _emptyParam);
        }

        public async Task<bool> IsDefined(IPortPlug port)
        {
            var matches = await scaMatches(port);
            return matches != null && matches.Count > 0;
        }

        public async Task<bool> HasInternationalFormat(IPortPlug port)
        {
            var matches = await scaMatches(port);
            int tosca = 0;
            return matches != null && matches.Count > 1 && int.TryParse(matches[1].Value, out tosca) && tosca == Constants.InternationalAddressType;
        }

        public async Task<string> GetAddress(IPortPlug port)
        {
            var matches = await scaMatches(port);
            return matches != null && matches.Count > 0 ? matches[0].Value : string.Empty;
        }

        public async Task<bool> SetAddress(IPortPlug port, long address, bool international)
        {
            if (address < 0)
                return false;

            var sign = international ? "+" : string.Empty;
            var tosca = international ? Constants.InternationalAddressType : Constants.DomesticAddressType;

            var scaParam = new CommandParameter($"\"{sign}{address}\",{tosca}", Constants.BasicSuccessfulResponse, true, false);
            var scaCmd = new ParamATCommand(ATCommand.ServiceCenterAddress.Command(), scaParam);

            await scaCmd.ExecuteAsync(port);
            return scaCmd.Succeeded();
        }


        private async Task<MatchCollection> scaMatches(IPortPlug port)
        {
            await _scaQuery.ExecuteAsync(port);
            return _scaQuery.Succeeded() ? Regex.Matches(_scaQuery.Response, @"\+?\d+") : null;
        }
    }
}
