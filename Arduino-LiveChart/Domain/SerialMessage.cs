using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arduino_LiveChart.Domain
{
    public class SerialMessage
    {
        public DateTime Time { get; set; } = DateTime.Now;

        public string Content { get; set; }
    }
}
