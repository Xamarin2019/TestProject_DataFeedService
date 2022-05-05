using System;
using System.Collections.Generic;
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

            string[] fileEntries = Directory.GetFiles(path + folderName);
            foreach (string fileName in fileEntries)
            {
                Console.WriteLine("Begin reading file {0}", getFileName.Match(fileName));

                var strings = ReadCsvFile(fileName).Result;
                var dictionary = ConvertCsvData(strings);
            }

        }


        static async Task<string[]> ReadCsvFile(string fileName)
        {
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
                .GroupBy(x => x.SortKey)
                .Select(group => group.First().Line);
                //.Select(x => x.Line);

                return sorted.ToArray();
            }
        }

        static SortedDictionary<DateTime, MarketData> ConvertCsvData(string[] data)
        {
            var dict = new SortedDictionary<DateTime, MarketData>();
            foreach (var item in data)
            {
                var values = item.Split(',');
                dict.Add(DateTime.Parse(values[0]), new MarketData(values));
            }

            return dict;
        }
    }
}

