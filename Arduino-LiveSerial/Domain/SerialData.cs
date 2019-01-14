using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_LiveSerial.Domain
{
    public class SerialData
    {
        public SerialDataSerie Serie;

        public string Key { get; set; }

        public double Value { get; set; }

        public uint? Millis { get; set; }

        public DateTime Time { get; set; } = System.DateTime.Now;

        public static SerialData CreateFromLine(string line, char valueSeparator, char millisSeparator)
        {
            if(line.Contains(valueSeparator))
            {
                if (!line.Contains(millisSeparator))
                {
                    return CreateSerialData(line, valueSeparator);
                }
                else
                {
                    return CreateSerialData(line, valueSeparator, millisSeparator);
                }
            }

            return null;
        }

        private static SerialData CreateSerialData(string line, char valueSeparator)
        {
            var key = line.Split(valueSeparator)[0];
            var valueToken = line.Split(valueSeparator)[1];
            var validValue = double.TryParse(valueToken, NumberStyles.Any, CultureInfo.InvariantCulture, out double value);

            if (validValue) return new SerialData()
            {
                Key = key,
                Value = value
            };

            return null;
        }

        private static SerialData CreateSerialData(string line, char valueSeparator, char millisSeparator)
        {
            var key = line.Split(valueSeparator)[0];
            var restToken = line.Split(valueSeparator)[1];
            var valueToken = restToken.Split(millisSeparator)[0];
            var millisToken = restToken.Split(millisSeparator)[1];

            var validValue = double.TryParse(valueToken, NumberStyles.Any, CultureInfo.InvariantCulture, out double value);
            var validMillis = uint.TryParse(millisToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint millis);

            if (validValue) return new SerialData()
            {
                Key = key,
                Value = value,
                Millis = millis
            };

            return null;
        }
    }
}
