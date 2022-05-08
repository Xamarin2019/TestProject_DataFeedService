using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
            string path = @"..\..\..\";

            Console.SetCursorPosition(0, 1);
            if (Directory.Exists(path + folderName))
            {
                Console.WriteLine("The \"{0}\" directory was found.", folderName);
            }
            else
            {
                ZipFile.ExtractToDirectory(_zipPath, @"..\..\..\");
                Console.WriteLine("The \"{0}.zip\" extracted", folderName);
            }
            Console.WriteLine();

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
