using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_port
{
    class Program
    {
        static void Main(string[] args)
        {
            var _port = new SerialPort();
            _port.PortName = "COM4";
            _port.BaudRate = 9600;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.ReadTimeout = -1;
            _port.WriteTimeout = -1;

            _port.DtrEnable = true;
            _port.RtsEnable = true;

            _port.Open();

            _port.Write("ATI\r\n");

            bool _cont = true;
            while (_cont)
            {
                //if (_port.BytesToRead > 0)
                //{
                //    var r = _port.ReadLine();
                //}

                var r = _port.ReadExisting();

                if (Console.ReadLine() == "q")
                    _cont = false;
            }

            _port.Close();
        }
    }
}
