using System;

namespace MHLab.Utilities.Communication
{
    public interface ISubscriber : IDisposable
    {
        uint EventsCount { get; }

        void Consume();
    }

    public interface ISubscriber<TMessage> : ISubscriber where TMessage : IMessage
    {
        void Subscribe(IListener<TMessage>   listener);
        void Unsubscribe(IListener<TMessage> listener);
    }
}