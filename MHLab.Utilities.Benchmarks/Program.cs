using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using MHLab.Utilities.Benchmarks.Collections.Stackonly;
using MHLab.Utilities.Benchmarks.Messaging;
using MHLab.Utilities.Messaging;

namespace MHLab.Utilities.Benchmarks
{
    class Program
    {
        private static bool _shouldStop = false;
        
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<PublisherBenchmark>();
            return;
            
            //RunPublisherProfilerLoop();
        }
        
        static void RunPublisherProfilerLoop()
        {
            var publisher = new Publisher<IMyMessage>();
            var handler   = new MyMessageHandler();
            
            publisher.Subscribe(handler);

            Task.Run(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Q)
                    {
                        _shouldStop = true;
                        break;
                    }
                }
            });

            Console.WriteLine("Ready.");
            
            Console.ReadKey();
            
            Console.WriteLine("Start iterating.");
            
            var message = new TestMessage(5);

            while (_shouldStop == false)
            {
                publisher.Publish(message);
                publisher.Deliver();
            }
        }
    }
}