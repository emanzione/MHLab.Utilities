using System;
using BenchmarkDotNet.Attributes;
using MHLab.Utilities.Messaging;

namespace MHLab.Utilities.Benchmarks.Messaging
{
    public interface IMyMessage
    {
        int Counter { get; }
    }

    public readonly struct TestMessage : IMyMessage
    {
        public int Counter { get; }

        public TestMessage(int testValue)
        {
            Counter = testValue;
        }
    }

    public class MyMessageHandler : IMessageHandler<TestMessage, IMyMessage>
    {
        public int Counter { get; private set; }

        public void OnMessageDelivered(TestMessage message)
        {
            Counter += message.Counter;
        }

        public void Dispose()
        {
        }
    }

    [MemoryDiagnoser]
    public class PublisherBenchmark
    {
        private IPublisher<IMyMessage> _publisher;
        private MyMessageHandler       _handler;

        public PublisherBenchmark()
        {
            _publisher = new Publisher<IMyMessage>();
            _handler   = new MyMessageHandler();
            
            _publisher.Subscribe(_handler);
        }

        [Benchmark]
        public void Publish()
        {
            _publisher.Publish(new TestMessage(5));
            _publisher.Deliver();
        }
    }
}