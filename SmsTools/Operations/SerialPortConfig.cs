using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsTools.Operations
{
    public class SerialPortConfig
    {
        public string Name { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        public static bool IsValid(SerialPortConfig config)
        {
            return
                config != null &&
                !string.IsNullOrWhiteSpace(config.Name) &&
                Regex.IsMatch(config.Name, @"^com\d{1,2}$", RegexOptions.IgnoreCase) &&
                config.BaudRate >= 75 &&
                config.DataBits > 4 && config.DataBits < 10;
        }

        public static SerialPortConfig CreateDefault()
        {
            return new SerialPortConfig() { Name = "COM1", BaudRate = 9600, Parity = Parity.None, DataBits = 8, StopBits = StopBits.One };
        }
    }
}
