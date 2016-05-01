using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SmsTools.Operations
{
    /// <summary>
    /// Serial port wrapper
    /// </summary>
    public class SerialPortPlug : IPortPlug
    {
        private bool _disposed = false;
        private SerialPortConfig _config = SerialPortConfig.CreateDefault();
        private SerialPort _port = new SerialPort();
        private long _result = (long)Result.None;
        private StringBuilder _buffer = new StringBuilder();

        private enum Result:long
        {
            None, Chars, Eof, Error
        }

        public bool IsOpen { get { return _port.IsOpen; } }
        public Exception LastError { get; private set; }
        public int WaitPeriod { get; private set; } = 100;
        public int OperationTimeout { get; private set; } = 5000;

        public SerialPortPlug(SerialPortConfig config)
        {
            if (!SerialPortConfig.IsValid(config))
                throw new ArgumentException("Invalid config.");

            _config = config;

            configurePort();
        }

        public async Task SendAsync(string data)
        {
            Interlocked.Exchange(ref _result, (long)Result.None);
            _buffer.Clear();

            if (!_port.IsOpen || data == null)
            {
                return;
            }

            await Task.Run(() =>
            {
                _port.DiscardOutBuffer();
                _port.DiscardInBuffer();
                _port.Write(data);
            });
        }

        public async Task<string> ReceiveAsync()
        {
            return await Task.Run<string>(() => 
            {
                Task.WaitAny(new Task[] { Task.Run(() => 
                {
                    while (Interlocked.Read(ref _result) == (long)Result.None)
                    {
                        Task.WaitAny(Task.Delay(WaitPeriod));
                    }
                }) }, OperationTimeout);

                if (Interlocked.Read(ref _result) == (long)Result.Chars)
                {
                    Task.WaitAny(new Task[] { Task.Run(() => 
                    {
                        while (!Regex.IsMatch(_buffer.ToString(), @"\s*(ok|>)\s*$", RegexOptions.IgnoreCase)
                            && !Regex.IsMatch(_buffer.ToString(), @"error", RegexOptions.IgnoreCase))
                        {
                            Task.WaitAny(Task.Delay(WaitPeriod));
                        }
                    }) }, OperationTimeout);
                }

                return _buffer.ToString();
            });
        }

        public static IEnumerable<string> AvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        private void configurePort()
        {
            _port.PinChanged += _port_PinChanged;
            _port.ErrorReceived += _port_ErrorReceived;
            _port.DataReceived += _port_DataReceived;

            _port.PortName = _config.Name;
            _port.BaudRate = _config.BaudRate;
            _port.Parity = _config.Parity;
            _port.DataBits = _config.DataBits;
            _port.StopBits = _config.StopBits;

            _port.DtrEnable = true;
            _port.RtsEnable = true;

            try
            {
                _port.Open();
            }
            catch(Exception ex) { LastError = ex; }
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Interlocked.Exchange(ref _result, (long)e.EventType);

            _buffer.Append((sender as SerialPort).ReadExisting());
        }

        private void _port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Interlocked.Exchange(ref _result, (long)Result.Error);

            LastError = new Exception($"Error received: {e.EventType}.");
        }

        private void _port_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
        }


        #region IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_port != null)
                    {
                        _port.PinChanged -= _port_PinChanged;
                        _port.ErrorReceived -= _port_ErrorReceived;
                        _port.DataReceived -= _port_DataReceived;

                        _port.Close();
                        _port.Dispose();
                        _port = null;
                    }
                }

                _disposed = true;
            }
        }

        public bool Disposed { get { return _disposed; } }

        #endregion
    }
}
