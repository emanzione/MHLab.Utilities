using System.Collections.Generic;
using MHLab.Utilities.Asserts;
using MHLab.Utilities.Types;

namespace MHLab.Utilities.Communication
{
    public class Publisher : IPublisher
    {
        private readonly Dictionary<TypeId, ISubscriber> _subscribersMap;
        private readonly List<ISubscriber>               _subscribers;

        public Publisher()
        {
            _subscribers    = new List<ISubscriber>();
            _subscribersMap = new Dictionary<TypeId, ISubscriber>();
        }

        public void Dispose()
        {
            _subscribers.Clear();
            _subscribersMap.Clear();
        }

        public void Publish<TMessage>(TMessage message) where TMessage : IMessage
        {
            Assert.Debug.NotNull(message);

            foreach (var subscriber in _subscribers)
            {
                if (subscriber is Subscriber<TMessage> specializedSubscriber)
                {
                    Assert.Debug.NotNull(specializedSubscriber);
                    specializedSubscriber.AddMessage(message);
                    break;
                }
            }
        }

        public ISubscriber<TMessage> GetSubscriber<TMessage>() where TMessage : IMessage
        {
            if (_subscribersMap.TryGetValue(TypeMapper<TMessage>.Id, out var subscriber))
            {
                return (ISubscriber<TMessage>)subscriber;
            }

            var newSubscriber = new Subscriber<TMessage>();
            _subscribersMap.Add(TypeMapper<TMessage>.Id, newSubscriber);
            _subscribers.Add(newSubscriber);
            return newSubscriber;
        }

        public void Consume()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Consume();
            }
        }

        public PublisherStatistics GetStatistics()
        {
            var statistics = new PublisherStatistics();

            statistics.RegisteredSubscribers = (uint)_subscribers.Count;

            foreach (var subscriber in _subscribers)
            {
                statistics.EventsInFlight += subscriber.EventsCount;
            }

            return statistics;
        }
    }
}