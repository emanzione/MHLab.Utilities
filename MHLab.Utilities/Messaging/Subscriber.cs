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

namespace MHLab.Utilities.Messaging
{
    internal class Subscriber<TMessage, TConstraint> : ISubscriber<TMessage, TConstraint> where TMessage : TConstraint
    {
        public uint MessagesCount { get; private set; }

        private readonly List<IMessageHandler<TMessage, TConstraint>> _handlers;
        private readonly Queue<TMessage>                              _messages;

        private readonly List<IMessageHandler<TMessage, TConstraint>> _handlersSnapshot;

        public Subscriber()
        {
            _handlers         = new List<IMessageHandler<TMessage, TConstraint>>();
            _handlersSnapshot = new List<IMessageHandler<TMessage, TConstraint>>();
            _messages         = new Queue<TMessage>();
        }

        private IReadOnlyList<IMessageHandler<TMessage, TConstraint>> GetHandlersSnapshot()
        {
            _handlersSnapshot.Clear();
            _handlersSnapshot.AddRange(_handlers);
            return _handlersSnapshot;
        }

        public void Dispose()
        {
            _handlers.Clear();
            _handlersSnapshot.Clear();
            _messages.Clear();
        }

        public void Deliver(IMessageHistory<TConstraint> history)
        {
            while (_messages.Count > 0)
            {
                var message = _messages.Dequeue();
                HandleDelivery(message);
                history.Push(new MessageEnvelope<TConstraint>(message));
            }
        }

        public void Deliver()
        {
            while (_messages.Count > 0)
            {
                var message = _messages.Dequeue();
                HandleDelivery(message);
            }
        }

        private void HandleDelivery(TMessage message)
        {
            var handlers = GetHandlersSnapshot();

            for (var i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];
                handler.OnMessageDelivered(message);
            }
        }

        public void AddMessage(TMessage message)
        {
            _messages.Enqueue(message);
        }

        public int Subscribe(IMessageHandler<TMessage, TConstraint> handler)
        {
            var index = _handlers.Count;
            _handlers.Add(handler);
            return index;
        }

        public void Unsubscribe(HandlerSubscription subscription)
        {
            _handlers.RemoveAt(subscription.Index);
        }
    }
}