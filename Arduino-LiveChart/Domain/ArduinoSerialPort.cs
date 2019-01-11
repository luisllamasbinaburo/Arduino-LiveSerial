using Arduino_LiveChart.Utils;
using RJCP.IO.Ports;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace Arduino_LiveChart
{
    public class ArduinoSerialPort
    {
        private const char NEW_LINE_CHAR = '\n';

        private SerialPortStream _arduinoPort = new SerialPortStream();

        public string LastLine { get; private set; } = "";
        private string _lastLineBuffer { get; set; } = "";

        public bool IsReady { get; private set; }

        public bool IsOpen;

        public event EventHandler DataArrived;
        public event EventHandler LineArrived;

        public void Open(string port, int baud)
        {
            IsOpen = true;
            _arduinoPort.PortName = port;
            _arduinoPort.BaudRate = baud;
            _arduinoPort.DtrEnable = true;
            _arduinoPort.ReadTimeout = 1;
            _arduinoPort.WriteTimeout = 1;
            _arduinoPort.Open();
            _arduinoPort.DiscardInBuffer();
            _arduinoPort.DiscardOutBuffer();
            ClearData();
            _arduinoPort.DataReceived += DataRecieved;
        }


        public void Close()
        {
            if (!IsOpen) return;
            try
            {
                _arduinoPort.Flush();
                _arduinoPort.DataReceived -= DataRecieved;
                _arduinoPort.Close();
                IsOpen = false;
            }
            catch (Exception)
            {
                //do nothing
            }
          
        }

        private void DataRecieved(object sender, RJCP.IO.Ports.SerialDataReceivedEventArgs e)
        {
            var incomingData = _arduinoPort.ReadExisting();
            _lastLineBuffer += incomingData;

            while (_lastLineBuffer.Contains(NEW_LINE_CHAR))
            {
                LastLine = _lastLineBuffer.GetBefore(NEW_LINE_CHAR);
                _lastLineBuffer = _lastLineBuffer.GetAfter(NEW_LINE_CHAR);
                LineArrived?.Invoke(this, new EventArgs());
            }

            DataArrived?.Invoke(this, new EventArgs());
        }

        public void ClearData()
        {
            LastLine = "";
            _lastLineBuffer = "";
        }

        public void SendData(string data)
        {
            _arduinoPort.WriteLine(data);
        }
    }
}