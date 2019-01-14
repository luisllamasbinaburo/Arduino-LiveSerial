using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;

namespace Arduino_LiveSerial.Domain
{
    public class SerialDataReciever
    {

        private ArduinoSerialPort _arduinoSerialPort = new ArduinoSerialPort();
        public List<SerialDataSerie> DataSeries { get; set; } = new List<SerialDataSerie>();

        public Dictionary<string, SerialDataSerie> DictSeries { get; set; } = new Dictionary<string, SerialDataSerie>();

        public char ValueSeparator { get; set; } 
        public char MillisSeparator { get; set; }
        public string LineRecieved { get; set; }
        public bool IsConnected { get; set; }

        public void OpenPort(string port, int baudRate)
        {
            _arduinoSerialPort.Open(port, baudRate);
            _arduinoSerialPort.LineArrived += DataArrived;
            IsConnected = true;
        }

        public void ClosePort()
        {
            _arduinoSerialPort.LineArrived -= DataArrived;
            _arduinoSerialPort.Close();
            IsConnected = false;
        }

        private void DataArrived(object sender, EventArgs e)
        {
            LineRecieved = _arduinoSerialPort.LastLine;
            ProccessNewLine();
        }

        private void ProccessNewLine()
        {
            if (LineRecieved.Contains(ValueSeparator)) ProcessSerialData();
            else ProcessSerialMessage();
        }

        private void ProcessSerialData()
        {
            var newData = SerialData.CreateFromLine(LineRecieved, ValueSeparator, MillisSeparator);
            AddSerialData(newData);
        }

        private void ProcessSerialMessage()
        {
            var message = new SerialMessage() { Content = LineRecieved };
            MessageBus.Current.SendMessage(message, "NewMessage");
        }

        public void SendData(string data)
        {
            _arduinoSerialPort.SendData(data);
        }

        private void AddSerialData(Domain.SerialData data)
        {
            if (data == null) return;

            if (!DictSeries.ContainsKey(data.Key))
                AddSeries(data.Key);

            AddPoint(data);
        }

        private void AddSeries(string key)
        {
            var dataSerie = new SerialDataSerie() { Name = key };
            DictSeries.Add(key, dataSerie);
            DataSeries.Add(dataSerie);

            MessageBus.Current.SendMessage(dataSerie, "NewDataSerie");
        }

        private void AddPoint(Domain.SerialData data)
        {
            var dataSerie = DictSeries[data.Key];
            data.Serie = dataSerie;

            if (data.Millis != null)
            {
                var firstData = dataSerie.Data.FirstOrDefault(x => x.Millis != null);
                if (firstData != null)
                    data.Time = firstData.Time.AddMilliseconds((double)data.Millis - (double)firstData.Millis);
            }

            dataSerie.AddData(data);
            MessageBus.Current.SendMessage(data, "NewData");
        }
    }
}
