using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppNetMQ
{
    class Subcriber
    {
        static int count, currentTextLine = 0;
        int number;
        string[] _topics;
        int messageCount = 0;
        int verticalTextShift = 18;


        /// <summary>
        /// The parameters specify the topics for the subscription
        /// </summary>
        /// <param name="topics"></param>
        public Subcriber(params string[] topics)
        {
            _topics = topics;
            verticalTextShift += currentTextLine;
            currentTextLine += (_topics.Length + 3);
            number = ++count;

        }

        /// <summary>
        /// Start retrieving data
        /// </summary>
        public void Run()
        {
            if (_topics.Length == 0) return;
            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                foreach (var topic in _topics) subSocket.Subscribe(topic);
 
                lock (Program.ConsoleWriterLock)
                {
                    Console.SetCursorPosition(0, 5 + number);
                    Console.WriteLine($"Subscriber № {number} connecting..."); 
                }

                while (true)
                {
                    string messageTopic = subSocket.ReceiveFrameString();
                    string messageReceived = subSocket.ReceiveFrameString();

                    lock (Program.ConsoleWriterLock)
                    {
                        var timeStamp = messageReceived.Substring(messageReceived.Length -14, 12);
                        var timeNow = DateTime.Now.ToString("hh:mm:ss.fff");
                        var diffMilliseconds = (DateTime.Now - DateTime.Parse(timeStamp)).Milliseconds;
                        Console.SetCursorPosition(0,  verticalTextShift);
                        Console.WriteLine($"Subscriber № {number}");
                        Console.WriteLine($"Received message count: {messageCount++}");
                        Console.SetCursorPosition(0, verticalTextShift + 2 + Array.IndexOf(_topics, messageTopic));
                        Console.WriteLine($"{messageTopic.PadRight(4)}: {messageReceived}  Latency: {diffMilliseconds}ms     ");
                    }
                }
            }
        }
    }
}
