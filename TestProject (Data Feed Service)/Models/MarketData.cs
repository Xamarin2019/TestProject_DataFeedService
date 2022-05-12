using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleAppNetMQ
{
    public class MarketData
    {
        // Ignored some properties so as not to clutter up the console output
        public string Last { get; set; }
        [JsonIgnore]
        public string LastSize { get; set; }
        [JsonIgnore]
        public string TotalVolume { get; set; }
        [JsonIgnore]
        public string Bid { get; set; }
        [JsonIgnore]
        public string Ask { get; set; }
        [JsonIgnore]
        public string TickId { get; set; }
        public string BasisForLast { get; set; }
        [JsonIgnore]
        public string TradeMarketCenter { get; set; }
        [JsonIgnore]
        public string TradeConditions { get; set; }
        public string TimeStamp { get; set; }

        public MarketData(string[] data)
        {

            //Last		        = data[1];
            //LastSize	        = data[2];
            //TotalVolume       = data[3];
            //Bid			    = data[4];
            //Ask			    = data[5];
            //TickId			= data[6];
            //BasisForLast      = data[7];
            //TradeMarketCenter = data[8];
            //TradeConditions   = data[9];

            var dataEnumerator = data.GetEnumerator();
            dataEnumerator.MoveNext();
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).SkipLast(1))
            {
                propertyInfo.SetValue(this, dataEnumerator.Current);
                dataEnumerator.MoveNext();
            }
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this); //.Remove(50);
        }
    }
}
