using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppNetMQ
{
    public class Publisher
    {
        IDataParser _dataParser;
        IFileSystem _fileSystem;
        Dictionary<string, SortedDictionary<DateTime, List<MarketData>>> topics;
        int messageCount = 0, throughputCount = 0, throughput = 0;
        int verticalTextShift = 5, subscibersCount = 3;

        public Publisher(IDataParser dataParser, IFileSystem fileSystem)
        {
            _dataParser = dataParser;
            _fileSystem = fileSystem;

            // TrimFilesLength() added to speed up test data processing during debugging
#if DEBUG
            string[] fileEntries = fileSystem.GetFileEntries().TrimFilesLength();
#else
            string[] fileEntries = fileSystem.GetFileEntries();
#endif
            Console.Write("Reading files, please wait...");

            // Runs faster in parallel
            topics = fileEntries.AsParallel().Select(d => dataParser.ParseCsvData(dataParser.ReceiveCsvData(d).Result)).ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// This method starts sending topics one by one in a single thread
        /// </summary>
        public void Run()
        {
            using (var pubSocket = new PublisherSocket())
            {
                lock (Program.ConsoleWriterLock)
                {
                    Console.SetCursorPosition(0, verticalTextShift);
                    Console.WriteLine("Publisher socket binding...");
                    Console.SetCursorPosition(0, verticalTextShift + subscibersCount + 2);
                    Console.WriteLine($"Publisher");
                }

                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind("tcp://*:12345");
                new Timer((o) => { throughput = throughputCount; throughputCount = 0; }, null, 0, 1000);

                foreach (var topic in topics)
                {
                    foreach (var date in topic.Value)
                    {
                        foreach (var data in date.Value)
                        {
                            lock (Program.ConsoleWriterLock)
                            {
                                Console.SetCursorPosition(0, verticalTextShift + subscibersCount + 2 + 1);
                                data.TimeStamp = DateTime.Now.ToString("hh:mm:ss.fff");
                                Console.WriteLine($"Sent message count: {messageCount++}  Throughput: {throughput}msg/sec");
                                Console.WriteLine($"{topic.Key.PadRight(4)} : {data}");
                            }
                            pubSocket.SendMoreFrame(topic.Key).SendFrame(data.ToString());
                            throughputCount++;

                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parallel sending of threads in several threads
        /// </summary>
        public void RunAsync()
        {
            using (var pubSocket = new PublisherSocket())
            {
                lock (Program.ConsoleWriterLock)
                {
                    Console.SetCursorPosition(0, verticalTextShift);
                    Console.WriteLine("Publisher socket binding...");
                    Console.SetCursorPosition(0, verticalTextShift + subscibersCount + 2);
                    Console.WriteLine($"Publisher");
                }
                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind("tcp://*:12345");
 
                var keys = topics.Keys.ToArray();
                List<Task> taskArray = new List<Task>();
                new Timer((o) => { throughput = throughputCount; throughputCount = 0; }, null, 0, 1000);

                foreach (var topic in topics)
                {
                    taskArray.Add(Task.Factory.StartNew(() => PrintData(topic.Key, topic.Value)));
                }

                Task.WaitAll(taskArray.ToArray());

           
 

                void PrintData(string topic, SortedDictionary<DateTime, List<MarketData>> date)
                {
                    foreach (var data in date)
                    {
                        foreach (var marketData in data.Value)
                        {
                            lock (Program.ConsoleWriterLock)
                            {
                                Console.SetCursorPosition(0, verticalTextShift + subscibersCount + 2 + 1);
                                Console.WriteLine($"Sent message count: {messageCount++}  Throughput: {throughput}msg/sec");
                                Console.SetCursorPosition(0, verticalTextShift + subscibersCount + 2 + 2 + Array.IndexOf(keys, topic));
                                marketData.TimeStamp = DateTime.Now.ToString("hh:mm:ss.fff");
                                Console.WriteLine($"{topic.PadRight(4)}: {marketData}");
                                pubSocket.SendMoreFrame(topic).SendFrame(marketData.ToString());
                                throughputCount++;
                            }
 
                            Task.Delay(100).Wait();
                            //Thread.Sleep(100);
                        }
                    }
                }
            }
        }
      
    }
}
