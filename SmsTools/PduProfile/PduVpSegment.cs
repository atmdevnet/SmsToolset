using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.PduProfile
{
    /// <summary>
    /// VP
    /// </summary>
    public class PduVpSegment : IPduSegment
    {
        private int _vp = 0;

        public PduSegment Type { get { return PduSegment.ValidityPeriod; } }

        public PduVpSegment(IPduProfileSettings settings)
        {
            if (settings == null || settings.ValidityPeriod == null)
                throw new ArgumentNullException("Profile settings not specified.");

            if (settings.ValidityPeriod.Value < 0 || settings.ValidityPeriod.Value > 255)
                throw new ArgumentException("Validity period out of range.");

            _vp = settings.ValidityPeriod.Value;
        }

        public PduVpSegment()
        {
        }

        public int Length()
        {
            return 1;
        }

        public override string ToString()
        {
            return $"{_vp.ToString("X2")}";
        }

        public void SetValidityPeriod(VP value)
        {
            if (value != VP.Other)
            {
                _vp = (int)value;
            }
        }

        public VP GetValidityPeriod()
        {
            return Enum.IsDefined(typeof(VP), _vp) ? (VP)_vp : VP.Other;
        }
    }


    /// <summary>
    /// Some validity periods
    /// </summary>
    public enum VP
    {
        Other = -1,
        None = 0,
        Minimum = 1,
        Day = 167,
        Month = 196,
        Maximum = 255
    }
}
