using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleAppNetMQ
{
    class DataParser : IDataParser
    {
        Regex getFileName = new Regex(@"(\w*)(?=\.\w{3})");

        async Task<(string, string[])> ReadCsvFile(string fileName)
        {
            string topic = getFileName.Match(fileName).ToString();
            Debug.WriteLine($"Begin reading file {topic}");
            using (var reader = File.OpenText(fileName))
            {
                string[] lines = await File.ReadAllLinesAsync(fileName);

                var data = lines.Skip(1);
                var sorted = data.Select(line => new
                {
                    SortKey = DateTime.Parse(line.Split(',')[0]),
                    Line = line
                })
                .OrderBy(x => x.SortKey)
                .Select(x => x.Line);

                Debug.WriteLine($"End reading file   {topic}");
                return (topic, sorted.ToArray());
            }
        }

        public Task<(string, string[])> ReceiveCsvData(string address)
        {
            return ReadCsvFile(address);
        }

        public KeyValuePair<string, SortedDictionary<DateTime, List<MarketData>>> ParseCsvData((string topic, string[] data) tuple)
        {
            Debug.WriteLine($"Begin ParseCsvData {tuple.data[1]}");
            var dict = new SortedDictionary<DateTime, List<MarketData>>();
            foreach (var item in tuple.data)
            {
                var values = item.Split(',');
                var key = DateTime.Parse(values[0]);

                if (dict.ContainsKey(key))
                {
                    dict[key].Add(new MarketData(values));
                }
                else
                {
                    dict.Add(key, new List<MarketData>() { new MarketData(values) });
                }
            }
            Debug.WriteLine($"End ParseCsvData   {tuple.data[1]}");

            var result = new KeyValuePair<string, SortedDictionary<DateTime, List<MarketData>>>(tuple.topic, dict);
            return new KeyValuePair<string, SortedDictionary<DateTime, List<MarketData>>>(tuple.topic, dict);
        }

    }
}
