using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public enum ATCommand
    {
        /// <summary>
        /// ATI
        /// </summary>
        [ATCommand(Command = "ATI", AllowsAnonymous = true)]
        DefaultInfo,

        /// <summary>
        /// AT+CPIN?
        /// </summary>
        [ATCommand(Command = "AT+CPIN?", AllowsAnonymous = true)]
        PinAuthenticateInfo,
        /// <summary>
        /// AT+CPIN=
        /// </summary>
        [ATCommand(Command = "AT+CPIN=", HasParameters = true, AllowsAnonymous = true)]
        PinAuthenticate,

        /// <summary>
        /// AT+CSCA?
        /// </summary>
        [ATCommand(Command = "AT+CSCA?")]
        ServiceCenterAddressInfo,
        /// <summary>
        /// AT+CSCA=
        /// </summary>
        [ATCommand(Command = "AT+CSCA=", HasParameters = true)]
        ServiceCenterAddress,

        /// <summary>
        /// AT+CMGF?
        /// </summary>
        [ATCommand(Command = "AT+CMGF?")]
        MessageFormatInfo,
        /// <summary>
        /// AT+CMGF=?
        /// </summary>
        [ATCommand(Command = "AT+CMGF=?")]
        MessageFormatOptions,
        /// <summary>
        /// AT+CMGF=
        /// </summary>
        [ATCommand(Command = "AT+CMGF=", HasParameters = true)]
        MessageFormat,

        /// <summary>
        /// AT+CMGS=?
        /// </summary>
        [ATCommand(Command = "AT+CMGS=?")]
        MessageSendOptions,
        /// <summary>
        /// AT+CMGS=
        /// </summary>
        [ATCommand(Command = "AT+CMGS=", HasParameters = true, HasSteps = true)]
        MessageSend
    }
}
