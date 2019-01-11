using OxyPlot.Series;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Arduino_LiveChart.Domain
{
    public class SerialDataSerie : ReactiveObject
    {

        public SerialDataSerie()
        {
            peakSearch = new PeakSearch(this);
        }


        private Color _color = Colors.Black;
        public Color Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        public string Name { get; set; }

        public string Designation { get; set; }

        public List<SerialData> Data { get; set; } = new List<SerialData>();


        private bool _visible = true;
        public bool IsVisible
        {
            get => _visible;
            set
            {
                this.RaiseAndSetIfChanged(ref _visible, value);
                this.ChartSerie.IsVisible = value;
            }
        }
        
        private int _count;
        public int Count
        {
            get => _count;
            set => this.RaiseAndSetIfChanged(ref _count, value);
        }

        private double _value;
        public double Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }


        public LineSeries ChartSerie { get; set; }

        private DateTime _minT;
        public DateTime MinT
        {
            get => _minT;
            set => this.RaiseAndSetIfChanged(ref _minT, value);
        }

        private DateTime _maxT;
        public DateTime MaxT
        {
            get => _maxT;
            set => this.RaiseAndSetIfChanged(ref _maxT, value);
        }


        private double _min;
        public double Min
        {
            get => _min;
            set => this.RaiseAndSetIfChanged(ref _min, value);
        }

        private double _max;
        public double Max
        {
            get => _max;
            set => this.RaiseAndSetIfChanged(ref _max, value);
        }

        private double _peakMax;
        public double PeakMax
        {
            get => _peakMax;
            set => this.RaiseAndSetIfChanged(ref _peakMax, value);
        }


        private DateTime _peakT;
        public DateTime PeakT
        {
            get => _peakT;
            set => this.RaiseAndSetIfChanged(ref _peakT, value);
        }

        private double _frequency;
        public double Frequency
        {
            get => _frequency;
            set => this.RaiseAndSetIfChanged(ref _frequency, value);
        }

        private double _range;
        public double Range
        {
            get => _range;
            set => this.RaiseAndSetIfChanged(ref _range, value);
        }

        private double _sum;
        public double Sum
        {
            get => _sum;
            set => this.RaiseAndSetIfChanged(ref _sum, value);
        }

        private double _mean;
        public double Mean
        {
            get => _mean;
            set => this.RaiseAndSetIfChanged(ref _mean, value);
        }

        private double _delta;
        public double Delta
        {
            get => _delta;
            set => this.RaiseAndSetIfChanged(ref _delta, value);
        }


        private TimeSpan _deltaT;
        public TimeSpan DeltaT
        {
            get => _deltaT;
            set => this.RaiseAndSetIfChanged(ref _deltaT, value);
        }

        private TimeSpan _interval;
        public TimeSpan Interval
        {
            get => _interval;
            set => this.RaiseAndSetIfChanged(ref _interval, value);
        }

        private double _slope;
        public double Slope
        {
            get => _slope;
            set => this.RaiseAndSetIfChanged(ref _slope, value);
        }

        private PeakSearch peakSearch;

        public void AddData(SerialData item)
        {
            Count++;
            Value = item.Value;
            Min = Math.Min(item.Value, Min);
            Max = Math.Max(item.Value, Max);
            MinT = item.Time < MinT ? item.Time : MinT;
            MaxT = item.Time > MaxT ? item.Time : MaxT;
            Range = Max - Min;
            Sum += item.Value;
            Mean = Sum / Count;

            SerialData prev = Data.LastOrDefault();
            if (prev != null)
            {

                Delta = item.Value - prev.Value;
                DeltaT = item.Time - prev.Time;
                Slope = Delta / DeltaT.TotalSeconds;
            }
            else
            {
                MinT = item.Time;
                Min = item.Value;
            }
            peakSearch.ProcessItems(prev, item);
            Data.Add(item);
        }

        public class PeakSearch
        {
            public SerialDataSerie serie;
            public SerialData start;

            //public bool IsGoingDown = false;
            public bool IsGoingUp = false;

            public PeakSearch(SerialDataSerie serie)
            {
                this.serie = serie;
            }

            public void ProcessItems(SerialData prev, SerialData current)
            {
                if (prev == null) return;
                if (prev.Value > current.Value)
                {
                    if (IsGoingUp && start != null)
                    {
                        TimeSpan ts = current.Time.Subtract(start.Time);
                        DateTime middleTime = start.Time.AddMinutes(ts.TotalMinutes / 2);

                        if (serie.PeakT.Ticks > 0)
                        {
                            serie.Interval = middleTime - serie.PeakT;
                            serie.Frequency = 1 / serie.Interval.TotalSeconds;
                        }

                        serie.PeakT = middleTime;
                        serie.PeakMax = prev.Value;
                    }
                    IsGoingUp = false;
                }
                if (prev.Value < current.Value)
                {
                    start = current;
                    IsGoingUp = true;
                }

            }
        }
    }
}