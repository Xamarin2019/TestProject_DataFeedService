using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestProject__Data_Feed_Service_
{
    class Program
    {
        static string zipPath = @"..\..\..\data.zip";
        static string folderName = new Regex(@"(\w*)(?=\.zip)").Match(zipPath).ToString();
        static Regex getFileName = new Regex(@"(\w*)(?=\.\w{3})");
        static string path = @"..\..\..\";

        static void Main(string[] args)
        {
            Console.WriteLine("Test Project: Data Feed Service");

            if (Directory.Exists(path + folderName))
            {
                Console.WriteLine("The \"{0}\" directory was found.", folderName);
            }
            else
            {
                ZipFile.ExtractToDirectory(zipPath, @"..\..\..\");
                Console.WriteLine("The \"{0}.zip\" extracted", folderName);
            }
            Console.WriteLine();

            string[] fileEntries = Directory.GetFiles(path + folderName);

            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (string fileName in fileEntries)
            {
                var strings = ReadCsvFile(fileName).Result;
                SortedDictionary<DateTime, List<MarketData>> dictionary = ParseCsvData(strings);
            }

            stopwatch.Stop();
            Console.WriteLine( $"Elapsed retrieval time: {stopwatch.ElapsedMilliseconds:#,0} milliseconds.\n");

            stopwatch.Restart();

            IEnumerable<Task<string[]>> downloads = fileEntries.Select(ReadCsvFile);
            IEnumerable <SortedDictionary<DateTime, List<MarketData>>> dictionaries = Task.WhenAll(downloads).Result.AsParallel().Select(ParseCsvData);
            var dictionariesArray = dictionaries.ToArray();

            stopwatch.Stop();
            Console.WriteLine($"Elapsed retrieval time: {stopwatch.ElapsedMilliseconds:#,0} milliseconds.\n");
        }


        static async Task<string[]> ReadCsvFile(string fileName)
        {
            Console.WriteLine("Begin reading file {0}", getFileName.Match(fileName));
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
                //.GroupBy(x => x.SortKey)
                //.Select(group => group.First().Line);

                Console.WriteLine("End reading file   {0}", getFileName.Match(fileName));
                return sorted.ToArray();
            }
        }

        static SortedDictionary<DateTime, List<MarketData>> ParseCsvData(string[] data)
        {
            var dict = new SortedDictionary<DateTime, List<MarketData>>();
            foreach (var item in data)
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

            return dict;
        }
    }
}

