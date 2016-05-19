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
        /// AT+CSQ
        /// </summary>
        [ATCommand(Command = "AT+CSQ")]
        ReceivedSignalStrengthInfo,
        /// <summary>
        /// AT^CSNR
        /// </summary>
        [ATCommand(Command = "AT^CSNR?")]
        ReceivedSignalCodePowerInfo,
        /// <summary>
        /// AT^SYSINFO
        /// </summary>
        [ATCommand(Command = "AT^SYSINFO", AllowsAnonymous = true)]
        SystemInfo,

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
        MessageSend,

        /// <summary>
        /// AT+CPMS?
        /// </summary>
        [ATCommand(Command = "AT+CPMS?")]
        MessageStorageInfo,
        /// <summary>
        /// AT+CPMS=
        /// </summary>
        [ATCommand(Command = "AT+CPMS=", HasParameters = true)]
        MessageStorage,

        /// <summary>
        /// AT+CMGL=
        /// </summary>
        [ATCommand(Command = "AT+CMGL=", HasParameters = true)]
        MessageList,
        /// <summary>
        /// AT+CMGR=
        /// </summary>
        [ATCommand(Command = "AT+CMGR=", HasParameters = true)]
        MessageRead,
        /// <summary>
        /// AT+CMGD=
        /// </summary>
        [ATCommand(Command = "AT+CMGD=", HasParameters = true)]
        MessageDelete
    }
}
