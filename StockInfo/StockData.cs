using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockInfo
{
    public enum TimeFunction
    {
        INTRADAY = -1, DAILY, DAILY_ADJUSTED, WEEKLY, WEEKLY_ADJUSTED, MONTHLY, MONTHLY_ADJUSTED
    }

    public enum TimeInterval { min1 = 1, min5 = 5, min15 = 15, min30 = 30, min60 = 60 }

    /// <summary>
    /// Compact by default (returns latest 100 records). Full returns all records for the 20+ years of historical data.
    /// Applies only to same-day or daily data.
    /// </summary>
    public enum OutputSize { compact, full }

    /// <summary>
    /// Retrieves stock data from AlphaVantage. 
    /// </summary>
    public class StockData : IEnumerable<StockRecord>
    {
        private const string urlStart = "https://www.alphavantage.co/";
        private const string _adjusted = "_ADJUSTED";
        private const string finish = "&apikey=VHGEGTKB3Q3A076C&datatype=csv";

        private readonly List<StockRecord> _stockRecords = new List<StockRecord>();

        /// <summary>
        /// The list of stock records encapsulated by this StockData.
        /// </summary>
        public List<StockRecord> StockRecords
        {
            get { return _stockRecords; }
        }
        public readonly TimeFunction TimeFunction;

        /// <summary>
        /// Stock data for same day data.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="outputSize"></param>
        public StockData(string symbol, TimeInterval interval = TimeInterval.min15, OutputSize outputSize = OutputSize.compact)
        {
            TimeFunction = TimeFunction.INTRADAY;
            string link = $"{urlStart}query?function=TIME_SERIES_{TimeFunction}&symbol={symbol}" +
                $"&outputsize={outputSize}&interval={(int)interval}min&{finish}";

            ReadData(TimeFunction,link);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeFunction"></param>
        /// <param name="symbol"></param>
        /// <param name="adjusted"></param>
        /// <param name="outputSize">Null by default.</param>
        public StockData(string symbol, TimeFunction timeFunction, OutputSize? size = null)
        {
            TimeFunction = timeFunction;
            string outputSize = size.HasValue ? $"&outputsize={size.Value}" : "";

            string link = $"{urlStart}query?function=TIME_SERIES_{TimeFunction}&symbol={symbol}" + outputSize + $"&{finish}";

            ReadData(timeFunction,link);
        }

        private void ReadData(TimeFunction timeFunction, string link)
        {
            using (WebClient client = new WebClient()) { client.DownloadFile(link, "temp.csv"); }
            StreamReader file = new StreamReader("temp.csv");

            file.ReadLine();
            while (!file.EndOfStream) _stockRecords.Add(StockRecord.Parse(file.ReadLine(), timeFunction));
            file.Close();
            File.Delete("temp.csv");
        }

        public override string ToString()
        {
            string str = "";
            foreach (var record in _stockRecords)
            {
                str += record.ToString() + "\n";
            }
            return str;
        }

        public IEnumerator<StockRecord> GetEnumerator() => ((IEnumerable<StockRecord>)_stockRecords).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_stockRecords).GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    public class StockRecord : IEquatable<StockRecord>
    {
        public readonly bool Adjusted;
        public readonly DateTime TimeStamp;
        public readonly double Open, High, Low, Close;
        public readonly double? AdjustedClose, SplitRatio, DividendAmount;
        public readonly int Volume;

        /// <summary>
        /// Constructs a stock record for non-adjusted, non-daily stocks.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="open"></param>
        /// <param name="hi"></param>
        /// <param name="lo"></param>
        /// <param name="close"></param>
        /// <param name="volume"></param>
        public StockRecord(DateTime time, double open, double hi, double lo, double close, int volume)
        {
            Adjusted = false; TimeStamp = time; Open = open; High = hi; Low = lo; Close = close;
            AdjustedClose = null; Volume = volume; SplitRatio = null; DividendAmount = null;
        }

        /// <summary>
        /// A constructor for adjusted stocks.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="open"></param>
        /// <param name="hi"></param>
        /// <param name="lo"></param>
        /// <param name="close"></param>
        /// <param name="adjustedClose"></param>
        /// <param name="volume"></param>
        /// <param name="splitRatio"></param>
        /// <param name="dividendAmount"></param>
        public StockRecord(DateTime time, double open, double hi, double lo, double close, double adjustedClose,
            int volume, double dividendAmount, double? splitRatio = null)
        {
            Adjusted = false; TimeStamp = time; Open = open; High = hi; Low = lo; Close = close;
            AdjustedClose = adjustedClose; Volume = volume; DividendAmount = dividendAmount; SplitRatio = splitRatio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static StockRecord Parse(string s, TimeFunction timeFunction)
        {
            int i = (int)timeFunction;
            var S = s.Split(new Char[] { ',' });

            DateTime dateTime = DateTime.Parse(S[0]);
            double open = double.Parse(S[1]), high = double.Parse(S[2]), low = double.Parse(S[3]),
                close = double.Parse(S[4]);
            int volume;

            if (i > 0 && (i % 2 == 1))
            {
                double adjustedClose = double.Parse(S[5]), dividendAmount = double.Parse(S[7]);
                volume = int.Parse(S[6]);
                if (timeFunction == TimeFunction.DAILY_ADJUSTED)
                {
                    double splitRatio = double.Parse(S[8]);
                    return new StockRecord(dateTime, open, high, low, close, adjustedClose, volume, dividendAmount, splitRatio);
                }
                return new StockRecord(dateTime, open, high, low, close, adjustedClose, volume, dividendAmount);
            }
            volume = int.Parse(S[5]);

            return new StockRecord(dateTime, open, high, low, close, volume);
        }

        private bool Approx(double f1, double f2, double precision) => Math.Abs(f2 - f1) <= precision;

        /// <summary>
        /// Ensures the other stock record is identical to this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(StockRecord other)
        {
            double p = 0.0005;
            return TimeStamp == other.TimeStamp && Approx(Open, other.Open, p) && Approx(High, other.High, p)
                && Approx(Low, other.Low, p) && Approx(Close, other.Open, p) && Volume == other.Volume;
        }

        public override string ToString()
        {
            string time = (TimeStamp.TimeOfDay == TimeSpan.Zero) ? TimeStamp.ToShortDateString()
                : TimeStamp.ToString();
            string adjustedClose = AdjustedClose.HasValue ? $" (${AdjustedClose.Value})" : "";
            string dividend = DividendAmount.HasValue ? $" | {DividendAmount.Value}" : "";
            string splitRatio = SplitRatio.HasValue ? $" | {SplitRatio.Value}" : "";

            return $"{time} [${Open}, (${Low} : ${High}), ${Close}{adjustedClose}] ({Volume}){dividend}{splitRatio}";
        }

        public static string StringRepresentation()
        {
            return "{Time} [{Open}, ({Low} : {High}), {Close} ({Adjusted Close?})] (Volume)" +
                " | {Dividend?} | {Split Ratio?}";
        }
    }
}
