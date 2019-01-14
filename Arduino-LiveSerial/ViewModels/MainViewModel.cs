using Arduino_LiveSerial.Domain;
using Arduino_LiveSerial.Utils;
using Arduino_LiveSerial.View.Dialogs.Predefined;
using MaterialDesignThemes.Wpf;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Arduino_LiveSerial.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        #region Constructor
        public MainViewModel() : base()
        {
            _isPortSelected = this
              .WhenAnyValue(x => x.SelectedSerialPort)
              .Select(SelectedSerialPort => !string.IsNullOrEmpty(SelectedSerialPort))
              .ToProperty(this, x => x.IsPortSelected);

            _dataWindow = this.WhenAnyValue(x => x.DataWindowUI)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.DataWindow);

            this.WhenAnyValue(x => x.ValueSeparator)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .Where(sep => !string.IsNullOrWhiteSpace(sep))
                .Select(x => x.First())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((x) => SerialDataReciever.ValueSeparator = x);

            this.WhenAnyValue(x => x.MillisSeparator)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .Where(sep => !string.IsNullOrWhiteSpace(sep))
                .Select(x => x.First())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe((x) => SerialDataReciever.MillisSeparator = x);

            SuscribeMessageBus();
            WireCommands();
            InitData();
        }

        private void SuscribeMessageBus()
        {
            MessageBus.Current.Listen<Domain.SerialDataSerie>("NewDataSerie")
                .Subscribe(x => DataSerieRecieved(x));

            MessageBus.Current.Listen<Domain.SerialData>("NewData")
                .Subscribe(x => DataRecieved(x));

            MessageBus.Current.Listen<Domain.SerialMessage>("NewMessage")
                .Subscribe(x => MessageRecieved(x));
        }

        private void InitData()
        {
            ValueSeparator = Properties.Settings.Default["ValueSeparator"].ToString();
            MillisSeparator = Properties.Settings.Default["MillisSeparator"].ToString();
            DataWindowUI = (int)Properties.Settings.Default["DataWindow"];
            SelectedSerialPort = Properties.Settings.Default["Port"].ToString();
            SelectedBaudRate = (int)Properties.Settings.Default["BaudRate"];

            LoadPorts();
            BaudRates = new int[] { 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 };

            PlotModel.Axes.Add(new DateTimeAxis());
        }
        #endregion


        #region Commands
        private void WireCommands()
        {
            ShowAboutCommand = ReactiveCommand.Create(ShowAbout);
            OpenGithubCommand = ReactiveCommand.Create(OpenGithub);
            ClosingCommand = ReactiveCommand.Create(OnClosing);
            LoadPortsCommand = ReactiveCommand.Create(LoadPorts);
            ChangeConnectionCommand = ReactiveCommand.Create<bool, Unit>(ChangeConnection, this.WhenAnyValue(x => x.IsPortSelected));

            SendCommand = ReactiveCommand.Create(Send, this.WhenAnyValue(
                                                    x => x.IsConnected, x => x.SendText,
                                                    (isConnected, sendText) =>
                                                    isConnected && !string.IsNullOrEmpty(sendText)));
        }

 

        public ReactiveCommand<Unit, Unit> ShowAboutCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ClosingCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoadPortsCommand { get; set; }
        public ReactiveCommand<bool, Unit> ChangeConnectionCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SendCommand { get; set; }
        #endregion


        #region Properties

        public ObservableCollection<string> SerialPorts { get; set; } = new ObservableCollection<string>();


        private string _valueSeparator;
        public string ValueSeparator
        {
            get => _valueSeparator;
            set => this.RaiseAndSetIfChanged(ref _valueSeparator, value);
        }


        private string _MillisSeparator;
        public string MillisSeparator
        {
            get => _MillisSeparator;
            set => this.RaiseAndSetIfChanged(ref _MillisSeparator, value);
        }


        private readonly ObservableAsPropertyHelper<int> _dataWindow;
        public int DataWindow => _dataWindow.Value;

        private int _dataWindowUI;
        public int DataWindowUI
        {
            get => _dataWindowUI;
            set => this.RaiseAndSetIfChanged(ref _dataWindowUI, value);
        }

        public ObservableCollection<SerialMessage> SendedMessages { get; set; } = new ObservableCollection<SerialMessage>();

        public ObservableCollection<SerialMessage> RecievedMessages { get; set; } = new ObservableCollection<SerialMessage>();

        public int[] BaudRates { get; set; }


        private string _selectedSerialPort;
        public string SelectedSerialPort
        {
            get => _selectedSerialPort;
            set => this.RaiseAndSetIfChanged(ref _selectedSerialPort, value);
        }


        private int _selectedBaudRate = 115200;
        public int SelectedBaudRate
        {
            get => _selectedBaudRate;
            set => this.RaiseAndSetIfChanged(ref _selectedBaudRate, value);
        }


        private readonly ObservableAsPropertyHelper<bool> _isPortSelected;
        public bool IsPortSelected => _isPortSelected.Value;

        private bool _canConnect;
        public bool CanConnect
        {
            get => _canConnect;
            set => this.RaiseAndSetIfChanged(ref _canConnect, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => this.RaiseAndSetIfChanged(ref _isConnected, value);
        }


        private string _sendText;
        public string SendText
        {
            get => _sendText;
            set => this.RaiseAndSetIfChanged(ref _sendText, value);
        }

        public SerialDataReciever SerialDataReciever { get; set; } = new SerialDataReciever();
        public ObservableCollection<SerialDataSerie> DataSeries { get; set; } = new ObservableCollection<SerialDataSerie>();

        private PlotModel _plotModel = new PlotModel();
        public PlotModel PlotModel
        {
            get => _plotModel;
            set => this.RaiseAndSetIfChanged(ref _plotModel, value);
        }
        #endregion


        #region Methods
        private void ShowAbout()
        {
            var dialog = new Views.Dialogs.AboutDialog();
            DialogHost.Show(dialog, "RootDialog", (s, e)=> { }, (s, e) => { });
        }

        private void OpenGithub()
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/luisllamasbinaburo"));
            }
            catch (Exception)
            {
                // do nothing
            }
            
        }

        private void OnClosing()
        {
            ClosePort();

            Properties.Settings.Default["ValueSeparator"] = ValueSeparator;
            Properties.Settings.Default["MillisSeparator"] = MillisSeparator;
            Properties.Settings.Default["DataWindow"] = DataWindowUI;
            Properties.Settings.Default["Port"] =  SelectedSerialPort;
            Properties.Settings.Default["BaudRate"] = SelectedBaudRate;
            Properties.Settings.Default.Save();
        }

        private void LoadPorts()
        {
            SerialPorts.Clear();
            SerialPort.GetPortNames().ForEach(x => SerialPorts.Add(x));
            if (!SerialPorts.Contains(SelectedSerialPort)) SelectedSerialPort = SerialPorts.FirstOrDefault();
        }

        private Unit ChangeConnection(bool changeToConnected)
        {
            if (changeToConnected) OpenPort();
            else ClosePort();
            return new Unit();
        }

        private void OpenPort()
        {
            try
            {
                SerialDataReciever.OpenPort(SelectedSerialPort, SelectedBaudRate);
                IsConnected = true;
            }
            catch (Exception)
            {
                PredefinedDialogs.MessageDialog(this, "Error opening port", (s, e) => { }, (s, e) => { });
                IsConnected = false;
            }
        }

        private void ClosePort()
        {
            try
            {
                SerialDataReciever.ClosePort();
                IsConnected = false;
            }
            catch (Exception)
            {
                PredefinedDialogs.MessageDialog(this, "Error closing port", (s, e) => { }, (s, e) => { });
                IsConnected = false;
            }
        }

        private void Send()
        {
            try
            {
                SerialDataReciever.SendData(SendText);
                SendedMessages.Add(new SerialMessage { Content = SendText });
                SendText = "";
            }
            catch (Exception)
            {
                PredefinedDialogs.MessageDialog(this, "Error sending", (s, e) => { }, (s, e) => { });
                IsConnected = false;
            }
        }

        private void DataSerieRecieved(SerialDataSerie dataSerie)
        {
            Application.Current.Dispatcher.Invoke(() => DataSeries.Add(dataSerie));

            var line = new LineSeries
            {
                Title = dataSerie.Name,
                TrackerFormatString = "{0}\n{1}: {2:HH.mm.ss.ffff}\n{3}: {4:0.###}"
            };
            dataSerie.ChartSerie = line;

            PlotModel.Series.Add(line);
        }

        private void DataRecieved(Domain.SerialData data)
        {
            data.Serie.ChartSerie.Points.Add(DateTimeAxis.CreateDataPoint(data.Time, data.Value));
            if (DataWindow > 0)
            {
                while (data.Serie.ChartSerie.Points.Count > DataWindow)
                {
                    data.Serie.ChartSerie.Points.RemoveAt(0);
                }
            }

            PlotModel.InvalidatePlot(true);
        }

        private void MessageRecieved(SerialMessage x)
        {
            Application.Current.Dispatcher.Invoke(() => RecievedMessages.Add(x));
        }

        private void Refresh()
        {
            PlotModel.Series.Clear();

            var serie = DataSeries.First();
            DataSerieRecieved(serie);
            PlotModel.InvalidatePlot(true);
            foreach (var data in serie.Data)
            {
                DataRecieved(data);
                PlotModel.InvalidatePlot(true);
            }
            PlotModel.InvalidatePlot(true);
            PlotModel.InvalidatePlot(true);
        }
        #endregion
    }
}
