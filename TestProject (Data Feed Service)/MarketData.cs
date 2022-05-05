using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TestProject__Data_Feed_Service_
{
	// Timestamp,Last,LastSize,TotalVolume,Bid,Ask,TickId,BasisForLast,TradeMarketCenter,TradeConditions
	public class MarketData
    {
		//public DateTime TimeStamp { get; set; }
		public string Last { get; set; }
		public string LastSize { get; set; }
		public string TotalVolume { get; set; }
		public string Bid { get; set; }
		public string Ask { get; set; }
		public string TickId { get; set; }
		public string BasisForLast { get; set; }
		public string TradeMarketCenter { get; set; }
		public string TradeConditions { get; set; }

		public MarketData(string[] data)
        {
			////TimeStamp = DateTime.Parse(data[0]);
			//Last		      = data[1];
			//LastSize	      = data[2];
			//TotalVolume       = data[3];
			//Bid			      = data[4];
			//Ask			      = data[5];
			//TickId			  = data[6];
			//BasisForLast      = data[7];
			//TradeMarketCenter = data[8];
			//TradeConditions   = data[9];

			var dataEnumerator = data.GetEnumerator();
			dataEnumerator.MoveNext();
			foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				propertyInfo.SetValue(this, dataEnumerator.Current);
				dataEnumerator.MoveNext();
			}

		}

	}
}
