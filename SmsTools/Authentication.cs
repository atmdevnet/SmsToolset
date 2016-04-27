using SmsTools.Commands;
using SmsTools.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools
{
    public partial class Authentication
    {
        private IATCommand _pinQuery = null;

        public Authentication()
        {
            init();
        }

        private void init()
        {
            var pinQueryParam = CommandParameter.CreateEmpty(@"\s*ready\s*.*\s*ok\s*$", true);
            _pinQuery = new SimpleATCommand(ATCommand.PinAuthenticateInfo.Command(), pinQueryParam);
        }

        public async Task<bool> IsAuthenticated(IPortPlug port)
        {
            await _pinQuery.ExecuteAsync(port);
            return _pinQuery.Succeeded();
        }

        public async Task<bool> Authenticate(IPortPlug port, int pin)
        {
            if (pin < 0)
                return false;

            var pinParam = new CommandParameter(pin.ToString(), Constants.BasicSuccessfulResponse);
            var pinCmd = new ParamATCommand(ATCommand.PinAuthenticate.Command(), pinParam);

            await pinCmd.ExecuteAsync(port);
            return pinCmd.Succeeded();
        }

        public async Task<bool> AuthenticateIfNotReady(IPortPlug port, int pin)
        {
            bool authenticated = await IsAuthenticated(port);
            if (!authenticated)
            {
                authenticated = await Authenticate(port, pin);
            }
            return authenticated;
        }
    }
}
