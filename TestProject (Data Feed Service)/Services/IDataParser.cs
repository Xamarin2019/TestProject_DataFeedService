using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppNetMQ
{
    public interface IDataParser
    {
        Task<(string, string[])> ReceiveCsvData(string address);
        KeyValuePair<string, SortedDictionary<DateTime, List<MarketData>>> ParseCsvData((string topic, string[] data) tuple);
    }
}
