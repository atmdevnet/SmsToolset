using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// PID
    /// </summary>
    public class PduPidSegment : IPduSegment
    {
        private int _pid = 0;

        public PduSegment Type { get { return PduSegment.ProtocolIdentifier; } }

        public PduPidSegment(IPduProfileSettings settings)
        {
            if (settings == null || settings.ProtocolIdentifier == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.ProtocolIdentifier.Value < 0 || settings.ProtocolIdentifier.Value > 255)
                throw new ArgumentException("Protocol id out of range.");

            _pid = settings.ProtocolIdentifier.Value;
        }

        public PduPidSegment()
        {
        }

        public int Length()
        {
            return 1;
        }

        public override string ToString()
        {
            return $"{_pid.ToString("X2")}";
        }
    }
}
