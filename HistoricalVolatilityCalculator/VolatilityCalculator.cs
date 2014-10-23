using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HistoricalVolatilityCalculator
{
    public static class VolatilityCalculator
    {
        public static double CalculateTimeFactor(int period)
        {
            double timeFactorMultiple = 0;
            //Daily samples so number of days between observations is 1
            if (period == 0)
                timeFactorMultiple = 1;
            //Weekly samples so number of days between observations is 7
            if (period == 1)
                timeFactorMultiple = 7;
            //Monthly samples so number of days between observations is 30
            if (period == 2)
                timeFactorMultiple = 30;
            //The annual standard deviation is proportional to the square root of the quotient of 365 divided
            //by the number of days between observations.
            double timeFactor = Math.Sqrt(365 / timeFactorMultiple);
            return timeFactor;
        }

        public static double calculateSampleStDev(double lnSum, double lnSquaredSum, int sampleSize)
        {
            //calculates standard deviation of the sample
            double meanSquared = (lnSum * lnSum);
            double xValue = (sampleSize) * lnSquaredSum;
            double denom = (sampleSize) * ((sampleSize) - 1);
            double sampleStDev = Math.Sqrt((xValue - meanSquared) / denom);
            return sampleStDev;
        }

        public static double calculateAnnualStDev(double sampleStDev, double timeFactor)
        {
            //Extrapolates annual standard deviation from the sample standard deviation
            double annualStDev = sampleStDev * timeFactor;
            return annualStDev;
        }

        //Parses the Yahoo Finance CSV file
        public static List<Price> parseCSV(string fileName, int sampleSize)
        {
            List<Price> prices = new List<Price>();
            int count = 0;
            double lastPrice = 0;
            double diff = 0;
            double square = 0;
            double ln = 0;
            StreamReader reader = new StreamReader(fileName);
            while (!reader.EndOfStream && count <= sampleSize)
            {
                string line = reader.ReadLine();
                string[] cols = line.Split(',');
                //skips first line of reader which contains text
                if (count > 0)
                {
                    Price nextPrice = new Price();
                    nextPrice.Date = cols[0];
                    nextPrice.Close = Convert.ToDouble(cols[4]);
                    if (count > 1)
                    {
                        diff = (1 + ((nextPrice.Close - lastPrice) / lastPrice));
                        ln = Math.Log(diff);
                        square = (ln * ln);
                    }
                    nextPrice.Diff = diff;
                    nextPrice.Ln = ln;
                    nextPrice.Square = square;
                    lastPrice = nextPrice.Close;
                    prices.Add(nextPrice);
                }
                count++;
            }
            return prices;
        }

        //This is a conversion of the NORMSDIST() function from Excel.  The x value represents the z value and the function
        //converts it into a percentage.  3 standard deviations is 99.86% so NORMSDIST(3) returns .9986...
        public static double N(double x)
        {
            const double b1 = 0.319381530;
            const double b2 = -0.356563782;
            const double b3 = 1.781477937;
            const double b4 = -1.821255978;
            const double b5 = 1.330274429;
            const double p = 0.2316419;
            const double c = 0.39894228;

            if (x >= 0.0)
            {
                double t = 1.0 / (1.0 + p * x);
                return (1.0 - c * Math.Exp(-x * x / 2.0) * t * (t * (t * (t * (t * b5 + b4) + b3) + b2) + b1));
            }
            else
            {
                double t = 1.0 / (1.0 - p * x);
                return (c * Math.Exp(-x * x / 2.0) * t * (t * (t * (t * (t * b5 + b4) + b3) + b2) + b1));
            }
        }
    }
}