using System.Collections.Generic;
using MHLab.Utilities.Asserts;

namespace MHLab.Utilities.Communication
{
    internal class Subscriber<TMessage> : ISubscriber<TMessage> where TMessage : IMessage
    {
        public uint EventsCount => (uint)_messages.Count;

        private readonly List<IListener<TMessage>> _listeners;
        private readonly Queue<TMessage>           _messages;

        public Subscriber()
        {
            _listeners = new List<IListener<TMessage>>();
            _messages  = new Queue<TMessage>();
        }

        public void Subscribe(IListener<TMessage> listener)
        {
            Assert.Debug.NotNull(listener);
            _listeners.Add(listener);
        }

        public void Unsubscribe(IListener<TMessage> listener)
        {
            Assert.Debug.NotNull(listener);
            _listeners.Remove(listener);
        }

        public void Dispose()
        {
            _listeners.Clear();
            _messages.Clear();
        }

        public void AddMessage(TMessage message)
        {
            _messages.Enqueue(message);
        }

        public void Consume()
        {
            while (_messages.Count > 0)
            {
                var message = _messages.Dequeue();
                HandleDelivery(message);
            }
        }

        private void HandleDelivery(TMessage message)
        {
            foreach (var listener in _listeners)
            {
                listener.OnMessage(message);
            }
        }
    }
}