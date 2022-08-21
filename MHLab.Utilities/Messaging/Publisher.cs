/*
The MIT License (MIT)

Copyright (c) 2021 Emanuele Manzione

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.Collections.Generic;
using MHLab.Utilities.Asserts;
using MHLab.Utilities.Types;

namespace MHLab.Utilities.Messaging
{
    public class Publisher<TConstraint> : IPublisher<TConstraint>
    {
        private readonly Dictionary<TypeId, ISubscriber> _subscribersMap;
        private readonly List<ISubscriber>               _subscribers;

        private readonly List<ISubscriber> _subscribersSnapshot;

        public Publisher()
        {
            _subscribersMap      = new Dictionary<TypeId, ISubscriber>();
            _subscribers         = new List<ISubscriber>();
            _subscribersSnapshot = new List<ISubscriber>();
        }

        private IReadOnlyList<ISubscriber> GetSubscribersSnapshot()
        {
            _subscribersSnapshot.Clear();
            _subscribersSnapshot.AddRange(_subscribers);
            return _subscribersSnapshot;
        }

        public void Dispose()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Dispose();
            }
            _subscribersSnapshot.Clear();
            _subscribers.Clear();
            _subscribersMap.Clear();
        }

        public void Publish<TMessage>(TMessage message) where TMessage : struct, TConstraint
        {
            var subscribers = GetSubscribersSnapshot();
            
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber is Subscriber<TMessage, TConstraint> specializedSubscriber)
                {
                    Assert.Debug.NotNull(specializedSubscriber);
                    specializedSubscriber.AddMessage(message);
                    break;
                }
            }
        }
        
        public void PublishImmediate<TMessage>(TMessage message) where TMessage : struct, TConstraint
        {
            var subscribers = GetSubscribersSnapshot();
            
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber is Subscriber<TMessage, TConstraint> specializedSubscriber)
                {
                    Assert.Debug.NotNull(specializedSubscriber);
                    specializedSubscriber.AddMessage(message);
                    specializedSubscriber.Deliver();
                    break;
                }
            }
        }
        
        public void PublishImmediate<TMessage>(TMessage message, IMessageHistory<TConstraint> history) where TMessage : struct, TConstraint
        {
            var subscribers = GetSubscribersSnapshot();
            
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                if (subscriber is Subscriber<TMessage, TConstraint> specializedSubscriber)
                {
                    Assert.Debug.NotNull(specializedSubscriber);
                    Assert.Debug.NotNull(history);
                    specializedSubscriber.AddMessage(message);
                    specializedSubscriber.Deliver(history);
                    break;
                }
            }
        }

        public HandlerSubscription Subscribe<TMessage>(IMessageHandler<TMessage, TConstraint> handler) where TMessage : struct, TConstraint
        {
            Assert.Debug.NotNull(handler);

            var messageTypeId = TypeMapper<TMessage>.Id;

            if (_subscribersMap.TryGetValue(messageTypeId, out var sub))
            {
                var index = ((ISubscriber<TMessage, TConstraint>)sub).Subscribe(handler);
                return new HandlerSubscription(messageTypeId, index);
            }
            else
            {
                var subscriber = new Subscriber<TMessage, TConstraint>();
                _subscribersMap.Add(TypeMapper<TMessage>.Id, subscriber);
                _subscribers.Add(subscriber);

                var index = subscriber.Subscribe(handler);
                return new HandlerSubscription(messageTypeId, index);
            }
        }

        public void Unsubscribe(HandlerSubscription subscription)
        {
            var messageTypeId = subscription.Id;

            if (_subscribersMap.TryGetValue(messageTypeId, out var subscriber))
            {
                subscriber.Unsubscribe(subscription);
            }
        }

        public PublisherStatistics GetStatistics()
        {
            var statistics = new PublisherStatistics();

            statistics.RegisteredSubscribers = (uint)_subscribers.Count;

            for (var i = 0; i < _subscribers.Count; i++)
            {
                var subscriber = _subscribers[i];
                statistics.MessagesInFlight += subscriber.MessagesCount;
            }

            return statistics;
        }

        public void Deliver(IMessageHistory<TConstraint> history)
        {
            var subscribers = GetSubscribersSnapshot();
            
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                ((ISubscriber<TConstraint>)subscriber).Deliver(history);
            }
        }

        public void Deliver()
        {
            var subscribers = GetSubscribersSnapshot();
            
            for (var i = 0; i < subscribers.Count; i++)
            {
                var subscriber = subscribers[i];
                ((ISubscriber<TConstraint>)subscriber).Deliver();
            }
        }
    }
}