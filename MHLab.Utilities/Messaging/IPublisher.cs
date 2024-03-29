﻿/*
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

using System;
using System.Threading.Tasks;

namespace MHLab.Utilities.Messaging
{
    public interface IPublisher<TConstraint> : IDisposable
    {
        void Publish<TMessage>(TMessage message) where TMessage : struct, TConstraint;
        void PublishImmediate<TMessage>(TMessage message) where TMessage : struct, TConstraint;
        void PublishImmediate<TMessage>(TMessage message, IMessageHistory<TConstraint> history) where TMessage : struct, TConstraint;
        ValueTask PublishAsync<TMessage>(TMessage message) where TMessage : struct, TConstraint;

        HandlerSubscription Subscribe<TMessage>(IMessageHandler<TMessage, TConstraint> handler)
            where TMessage : struct, TConstraint;

        void Unsubscribe(HandlerSubscription subscription);

        PublisherStatistics GetStatistics();

        void Deliver(IMessageHistory<TConstraint> history);
        void Deliver();
    }
}