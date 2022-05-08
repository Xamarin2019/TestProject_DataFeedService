using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ConsoleAppNetMQ
{
    class Program
    {
        static string zipPath = @"..\..\..\data.zip";

        static void Main(string[] args)
        {
            Console.WriteLine("Test Project: Data Feed Service");

            myTestCase();

            Console.ReadKey();

        }

         static void TestCase1()
        {
            var publisher = GetPublisher();
            var subcriberA = new Subcriber("AAPL", "AMD", "MSFT", "TSLA", "TWTR");
            var subcriberB = new Subcriber("AAPL", "AMD", "MSFT");
            var subcriberC = new Subcriber("AAPL");

            Task.Run(publisher.RunAsync);
            Task.Run(subcriberA.Run);
            Task.Delay(TimeSpan.FromMinutes(1)).Wait();
            Task.Run(subcriberB.Run);
            Task.Delay(TimeSpan.FromMinutes(2)).Wait();
            Task.Run(subcriberC.Run);
        }

        static void TestCase2()
        {
            var subcriberA = new Subcriber("AAPL", "AMD", "MSFT", "TSLA", "TWTR");
            var subcriberB = new Subcriber("AAPL", "AMD", "MSFT");
            var subcriberC = new Subcriber("AAPL");

            Task.Run(subcriberA.Run);
            Task.Delay(TimeSpan.FromSeconds(30)).Wait();
            var publisher = GetPublisher();
            Task.Run(publisher.RunAsync);
            Task.Delay(TimeSpan.FromSeconds(60)).Wait();
            Task.Run(subcriberB.Run);
            Task.Delay(TimeSpan.FromSeconds(120)).Wait();
            Task.Run(subcriberC.Run);
        }

        static void myTestCase()
        {
            var publisher = GetPublisher();
            var subcriberA = new Subcriber("AAPL", "AMD", "MSFT", "TSLA", "TWTR");
            var subcriberB = new Subcriber("AAPL", "AMD", "MSFT");
            var subcriberC = new Subcriber("AAPL");

            //Task.Run(publisher.Run);
            Task.Run(publisher.RunAsync);
            Task.Run(subcriberA.Run);
            Task.Delay(5000).Wait();
            Task.Run(subcriberB.Run);
            Task.Delay(10000).Wait();
            Task.Run(subcriberC.Run);
        }

        // Here, I've used the Dependency Injection
        static Publisher GetPublisher()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IDataParser, DataParser>()
                .AddSingleton<IFileSystem>(new FileSystem(zipPath))
                .AddSingleton<Publisher>().BuildServiceProvider();

           return serviceProvider.GetService<Publisher>();
        }

        // To correctly output the console from different threads
        public static readonly object ConsoleWriterLock = new object();
        public static void WriteLine(string s) { lock (ConsoleWriterLock) { Console.WriteLine(s); } }
    }
}
