using System;

namespace MHLab.Utilities.Communication
{
    public interface IListener<TMessage> : IDisposable where TMessage : IMessage
    {
        void OnMessage(TMessage message);
    }
}