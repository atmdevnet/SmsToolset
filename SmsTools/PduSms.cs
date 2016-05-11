using SmsTools.Commands;
using SmsTools.Operations;
using SmsTools.PduProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools
{
    public partial class PduSms
    {
        private PduProfileManager _manager = new PduProfileManager();
        private IPduProfile _profile = null;
        private IATCommand _mfCmd = null;

        public PduSms()
        {
            initProfile();
            initCommands();
        }

        public async Task<bool> Send(IPortPlug port, long destination, string message)
        {
            bool send = false;

            var formatSet = await setFormat(port);

            if (formatSet)
            {
                int length = 0;
                string packet = _profile.GetPacket(destination, message, out length);

                var lengthStep = new StepwiseCommandParameter($"{length}{Constants.CR}", Constants.ContinueResponse, false);
                var messageStep = new StepwiseCommandParameter($"{packet}{Constants.SUB}", Constants.BasicSuccessfulResponse, true, true);

                var sendCmd = new StepwiseATCommand(ATCommand.MessageSend.Command(), new ICommandParameter[] { lengthStep, messageStep });
                await sendCmd.ExecuteAsync(port);
                send = sendCmd.Succeeded();
            }

            return send;
        }


        private async Task<bool> setFormat(IPortPlug port)
        {
            await _mfCmd.ExecuteAsync(port);
            return _mfCmd.Succeeded();
        }

        private void initProfile()
        {
            using (var settingsFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(IPduProfileSettings), "nosca-submit-16bit.json"))
            {
                var settings = _manager.CreateProfileSettings<PduDefaultSendProfileSettings>(settingsFile);
                var profile = _manager.CreateDefaultProfile(settings, "nosca-submit-16bit");

                _manager.AddProfile(profile);

                _profile = profile;
            }
        }

        private void initCommands()
        {
            var mfParam = new CommandParameter(Constants.MessageFormat.Pdu.ToValueString(), Constants.BasicSuccessfulResponse);
            _mfCmd = new ParamATCommand(ATCommand.MessageFormat.Command(), mfParam);
        }
    }
}
