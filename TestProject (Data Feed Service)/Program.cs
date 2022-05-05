using System;
using System.IO;
using System.IO.Compression;

namespace TestProject__Data_Feed_Service_
{
    class Program
    {
        static string zipPath = @"..\..\..\data.zip";
        static string path = @"..\..\..\data";

        static void Main(string[] args)
        {
            Console.WriteLine("Test Project: Data Feed Service");
            if (Directory.Exists(path))
            {
                Console.WriteLine("The 'data' directory was found.", path);
            }
            else
            {
                ZipFile.ExtractToDirectory(zipPath, path);
                Console.WriteLine("The data.zip extracted");
            }

        }
    }
}
