using System;

namespace MHLab.Utilities.Communication
{
    public interface IPublisher : IDisposable
    {
        void Publish<TMessage>(TMessage message) where TMessage : IMessage;

        ISubscriber<TMessage> GetSubscriber<TMessage>() where TMessage : IMessage;

        void Consume();

        PublisherStatistics GetStatistics();
    }
}