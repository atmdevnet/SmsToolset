using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools
{
    public static partial class Constants
    {
        public static readonly int InternationalAddressType = 0x91;
        public static readonly int DomesticAddressType = 0x81;

        public static readonly int PduHeaderSubmitNoReport = 0x11;

        public static readonly int MessageReferenceAuto = 0x00;

        public static readonly int ProtocolIdentifierSms = 0x00;

        public static readonly int DataCodingScheme7bit = 0x00;
        public static readonly int DataCodingScheme8bit = 0x04;
        public static readonly int DataCodingScheme16bit = 0x08;

        public static readonly int ValidityPeriodNone = 0x00;
        public static readonly int ValidityPeriodMinimum = 0x01;
        public static readonly int ValidityPeriodMaximum = 0xFF;
        public static readonly int ValidityPeriodDay = 0xA7;
        public static readonly int ValidityPeriodMonth = 0xC4;

        public static readonly char CR = '\u000D';
        public static readonly char SUB = '\u001A';

        public static readonly string BasicSuccessfulResponse = @"\s*OK\s*$";
        public static readonly string ContinueResponse = @"\s*>\s*$";

        public enum MessageFormat
        {
            Pdu, Text
        }

        public enum MessageStorage
        {
            Unspecified,
            [Description("\"MT\"")]
            MobileAssociated,
            [Description("\"ME\"")]
            MobileEquipment,
            [Description("\"SM\"")]
            Sim,
            [Description("\"SR\"")]
            StateReport
        }

        public enum MessageStatus
        {
            Unread,
            Read,
            Unsent,
            Sent,
            Any
        }
    }
}
