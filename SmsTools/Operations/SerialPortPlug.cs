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
        private StringBuilder _buffer = new StringBuilder();
        private ManualResetEventSlim _wait = new ManualResetEventSlim();

        public bool IsOpen { get; private set; }
        public Exception LastError { get; private set; }
        public dynamic GetConfig() { return _config.GetConfig(); }
        public int OperationTimeout { get; private set; } = 5000;

        public SerialPortPlug(SerialPortConfig config)
        {
            if (!SerialPortConfig.IsValid(config))
                throw new ArgumentException("Invalid config.");

            _config = config;

            configurePort();
        }

        public async Task<string> SendAndReceiveAsync(string data)
        {
            _wait.Reset();
            _buffer.Clear();

            if (!_port.IsOpen || data == null)
            {
                return string.Empty;
            }

            _port.DiscardOutBuffer();
            _port.DiscardInBuffer();
            _port.Write(data);

            var response = await Task.WhenAny<string>(Task.Run<string>(() => {
                return _wait.Wait(OperationTimeout) ? _buffer.ToString() : string.Empty;
            }));

            return await response;
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
                IsOpen = _port.IsOpen;
            }
            catch(Exception ex) { LastError = ex; }
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _buffer.Append((sender as SerialPort).ReadExisting());

            if (e.EventType == SerialData.Eof
                || (e.EventType == SerialData.Chars
                    && (Regex.IsMatch(_buffer.ToString(), @"\s*(ok|>)\s*$", RegexOptions.IgnoreCase)
                        || Regex.IsMatch(_buffer.ToString(), @"error", RegexOptions.IgnoreCase))))
            {
                _wait.Set();
            }
        }

        private void _port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            LastError = new Exception($"Error received: {e.EventType}.");

            _wait.Set();
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

                    if (_wait != null)
                    {
                        _wait.Set();
                        _wait.Dispose();
                        _wait = null;
                    }
                }

                _disposed = true;
            }
        }

        public bool Disposed { get { return _disposed; } }

        #endregion
    }
}
