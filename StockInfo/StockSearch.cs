using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockInfo
{
    public class StockSearch : IEnumerable<StockSearchResult>, IEquatable<StockSearch>
    {
        private readonly IEnumerable<StockSearchResult> SearchResults;
        
        public StockSearch(List<StockSearchResult> results)
        {
            SearchResults = results;
        }

        // Note: It's possible it might not connect to the internet. Deal with this error
        public static async Task<StockSearch> SearchAsync(string searchText)
        {
            // Gets Data as Json file
            string link = "https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords="
                + searchText + "&apikey=VHGEGTKB3Q3A076C";
            HttpClient client = new HttpClient();

            // Parses json file from the online link to the JSON file
            return new StockSearch(await ParseJson(new StreamReader(await client.GetStreamAsync(link))));
        }

        private static async Task<List<StockSearchResult>> ParseJson(StreamReader json)
        {
            List<StockSearchResult> results = new List<StockSearchResult>();

            Task<string> j = json.ReadToEndAsync();
            Match match = await Task.Run(async () => Regex.Match(await j, "\"\\d\\. (.+)\": \"(.*)\",?"));

            string symbol ="", name ="", type="", region="", currency="";
            TimeSpan open=TimeSpan.Zero, close=TimeSpan.Zero; 
            double timeZoneOffset = 0;

            while (match.Success)
            {
                string key = match.Groups[1].Value;
                string val = match.Groups[2].Value;
                switch (key)
                {
                    case "symbol": symbol = val; break;
                    case "name": name = val; break;
                    case "type": type = val; break;
                    case "region": region = val; break;
                    case "marketOpen": open = TimeSpan.Parse(val); break;
                    case "marketClose": close = TimeSpan.Parse(val); break;
                    case "timezone": timeZoneOffset = GetTimeZoneOffset(val); break;
                    case "currency": currency = val; break;
                    case "matchScore":
                        double score = double.Parse(val);
                        var result = new StockSearchResult(symbol, name, type, region, open, close,
                            timeZoneOffset, currency, score);
                        results.Add(result);
                        break;
                    default: break;
                }
                match = match.NextMatch();
            }

            return results;
        }

        // Convert "UTC+XX" or "UTC-XX" to +/-XX
        private static double GetTimeZoneOffset(string val)
        {
            val = string.Concat(val.Where((ch) => char.IsDigit(ch) || "-+.".Contains(ch)));

            return double.Parse(val);
        }

        public IEnumerator<StockSearchResult> GetEnumerator()
        {
            return SearchResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)SearchResults).GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            StockSearch ss = obj as StockSearch;
            if (this.Count() != ss.Count()) return false;

            for (int i = 0; i < this.Count(); i++)
            {
                if (this[i] != ss[i]) return false;
            }
            return true;
        }

        public override string ToString()
        {
            string s = "";
            foreach (StockSearchResult result in SearchResults)
            {
                s += result + "\n";
            }

            return s.TrimEnd('\n');
        }

        public bool Equals(StockSearch other)
        {
            return other != null &&
                   EqualityComparer<IEnumerable<StockSearchResult>>.Default.Equals(SearchResults, other.SearchResults);
        }

        public override int GetHashCode()
        {
            return -1558587991 + EqualityComparer<IEnumerable<StockSearchResult>>.Default.GetHashCode(SearchResults);
        }

        public static bool operator ==(StockSearch left, StockSearch right)
        {
            return EqualityComparer<StockSearch>.Default.Equals(left, right);
        }

        public static bool operator !=(StockSearch left, StockSearch right)
        {
            return !(left == right);
        }

        public StockSearchResult this[int i]
        {
            get
            {
                return SearchResults.ElementAt(i);
            }
        }
    }

    public class StockSearchResult : IEquatable<StockSearchResult>
    {
        public readonly string Symbol;
        public readonly string Name;
        public readonly string Type;
        public readonly string Region;
        public readonly TimeSpan MarketOpen;
        public readonly TimeSpan MarketClose;
        public readonly double TimeZoneOffset;
        public readonly string Currency;
        public readonly double MatchScore;

        public StockSearchResult(string symbol, string name, string type, string region, 
            TimeSpan open, TimeSpan close, double timeZoneOffset, string currency, double score)
        {
            Symbol = symbol; Name = name; Type = type; Region = region; 
            MarketOpen = open; MarketClose = close; TimeZoneOffset = timeZoneOffset; 
            Currency = currency; MatchScore = score;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StockSearchResult);
        }

        public bool Equals(StockSearchResult other)
        {
            return other != null &&
                   Symbol == other.Symbol &&
                   Name == other.Name &&
                   Type == other.Type &&
                   Region == other.Region &&
                   TimeZoneOffset == other.TimeZoneOffset &&
                   Currency == other.Currency;
        }

        public override int GetHashCode()
        {
            int hashCode = 1195451156;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Symbol);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Region);
            hashCode = hashCode * -1521134295 + TimeZoneOffset.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Currency);
            return hashCode;
        }

        public override string ToString()
        {
            string tzo = (TimeZoneOffset >= 0) ? $"+{TimeZoneOffset}" : $"{TimeZoneOffset}";

            return $"{MatchScore} >> ({Symbol}) {Name} [{Type}]: ({MarketOpen} -> {MarketClose}) {Currency}, {Region} UTC{tzo}";
        }
    }
}
