using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsTools.Commands
{
    public class StepwiseCommandParameter : CommandParameter
    {
        public StepwiseCommandParameter(string value, string successfulResponsePattern, bool ignoreCase = true, bool isNextParameter = false)
            : base(value, successfulResponsePattern, ignoreCase)
        {
            IsNextParameter = isNextParameter;
        }
    }
}
