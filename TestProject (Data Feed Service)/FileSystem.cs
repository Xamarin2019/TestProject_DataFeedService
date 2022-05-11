using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConsoleAppNetMQ
{
    public class FileSystem : IFileSystem
    {
        string _zipPath;

        public FileSystem(string zipPath)
        {
            _zipPath = zipPath;
        }

        // The file 'TWTR.csv' exceeds GitHub's file size restriction of 100MB.
        // Therefore, I send the archive, and the 'data' folder was added to the gitignor 
        public string[] GetFileEntries()
        {
            string folderName = new Regex(@"(\w*)(?=\.zip)").Match(_zipPath).ToString();
#if DEBUG
            string path = @"..\..\..\";
#else
            string path = Directory.GetCurrentDirectory() + "\\";
#endif

            Console.SetCursorPosition(0, 2);
            if (Directory.Exists(path + folderName))
            {
                Console.WriteLine("The \"{0}\" directory was found.", folderName);
            }
            else
            {
                if (!File.Exists(_zipPath))
                {
                    Console.WriteLine(Environment.NewLine + "The \"{0}\" file was not found." + Environment.NewLine, _zipPath);
                    Thread.Sleep(5000);
                    throw new Exception("The data.zip directory must be in the same location!!!");
                }
                ZipFile.ExtractToDirectory(_zipPath, path);
                Console.WriteLine("The \"{0}.zip\" extracted", folderName);
            }

            string[] fileEntries = Directory.GetFiles(path + folderName);
            return fileEntries;
        }

        
    }

    public static class FileSystemExtensions
    {
        /// <summary>
        /// Shortens the file to the specified length (by default to 10000 lines).
        /// </summary>
        /// <param name="fileEntries"></param>
        /// <param name="lineCount"></param>
        /// <returns></returns>
        public static string[] TrimFilesLength(this string[] fileEntries, int lineCount = 10000)
        {
            foreach (var item in fileEntries) TrimFileLength(item, lineCount);
            return fileEntries;
        }

        public static void TrimFileLength(string path, int lineCount)
        {
            List<string> lines = new List<string>();

            using (StreamReader sr = File.OpenText(path))
            {
                while(lineCount-- !=0) lines.Add(sr.ReadLine());
            }

            File.WriteAllLines(path, lines);
        }
    }
}
