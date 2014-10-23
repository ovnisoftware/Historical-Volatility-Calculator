using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalVolatilityCalculator
{
    public class Price
    {
        public string Date { get; set; }
        public double Close { get; set; }
        public double Diff { get; set; }
        public double Ln { get; set; }
        public double Square { get; set; }
    }
}