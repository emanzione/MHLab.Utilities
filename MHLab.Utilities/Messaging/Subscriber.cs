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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MHLab.Utilities.Messaging
{
    internal sealed class Subscriber<TMessage, TConstraint> : ISubscriber<TMessage, TConstraint>
        where TMessage : TConstraint
    {
        private readonly struct QueuedMessage
        {
            public readonly TMessage                       Message;
            public readonly Optional<TaskCompletionSource<bool>> CompletionSource;

            public QueuedMessage(TMessage message, Optional<TaskCompletionSource<bool>> completionSource)
            {
                Message          = message;
                CompletionSource = completionSource;
            }
            
            public QueuedMessage(TMessage message)
            {
                Message          = message;
                CompletionSource = Optional<TaskCompletionSource<bool>>.None();
            }
        }

        public uint MessagesCount { get; private set; }

        private readonly List<IMessageHandler<TMessage, TConstraint>> _handlers;
        private readonly Queue<QueuedMessage>                         _messages;

        public Subscriber()
        {
            _handlers = new List<IMessageHandler<TMessage, TConstraint>>();
            _messages = new Queue<QueuedMessage>();
        }

        public void Dispose()
        {
            _handlers.Clear();
            _messages.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deliver(IMessageHistory<TConstraint> history)
        {
            while (_messages.Count > 0)
            {
                var message = _messages.Dequeue();
                HandleDelivery(message);
                history.Push(new MessageEnvelope<TConstraint>(message.Message));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deliver()
        {
            while (_messages.Count > 0)
            {
                var message = _messages.Dequeue();
                HandleDelivery(message);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeliverSingle(TMessage message)
        {
            HandleDelivery(new QueuedMessage(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DeliverSingle(TMessage message, IMessageHistory<TConstraint> history)
        {
            HandleDelivery(new QueuedMessage(message));
            history.Push(new MessageEnvelope<TConstraint>(message));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HandleDelivery(QueuedMessage message)
        {
            var count = _handlers.Count;

            for (var i = 0; i < count; i++)
            {
                _handlers[i].OnMessageDelivered(message.Message);

                if (message.CompletionSource)
                {
                    message.CompletionSource.Value.SetResult(true);
                }
            }
        }

        public void AddMessage(TMessage message)
        {
            _messages.Enqueue(new QueuedMessage(message));
        }

        public void AddAsyncMessage(TMessage message, TaskCompletionSource<bool> completionSource)
        {
            _messages.Enqueue(new QueuedMessage(message, completionSource));
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