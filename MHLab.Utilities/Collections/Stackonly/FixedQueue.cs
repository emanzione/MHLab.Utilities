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
using MHLab.Utilities.Asserts;

namespace MHLab.Utilities.Collections.Stackonly
{
    public ref struct FixedQueue<TItem>
    {
        public int Count    => _count;
        public int Capacity => _buffer.Length;

        public bool IsEmpty => Count <= 0;
        public bool IsFull  => Count >= Capacity;

        private int         _count;
        private int         _head;
        private int         _tail;
        private Span<TItem> _buffer;

        public FixedQueue(Span<TItem> buffer)
        {
            Assert.Debug.Check(buffer.Length > 0);
            _count  = 0;
            _head   = 0;
            _tail   = 0;
            _buffer = buffer;
        }

        public static implicit operator FixedQueue<TItem>(Span<TItem> buffer) => new FixedQueue<TItem>(buffer);

        public ref TItem this[int index] => ref _buffer[(_head + index) % Capacity];

        public Span<TItem> GetBuffer()
        {
            return _buffer;
        }

        public bool Enqueue(TItem item)
        {
            if (IsFull) return false;

            var index = _tail;
            _tail = (_tail + 1) % Capacity;
            _count++;

            _buffer[index] = item;

            return true;
        }

        public ref TItem Dequeue()
        {
            if (IsEmpty) ThrowInvalidOperationException();

            var index = _head;
            _head = (_head + 1) % Capacity;
            _count--;

            return ref _buffer[index];
        }

        public ref TItem Peek()
        {
            if (IsEmpty) ThrowInvalidOperationException();

            return ref _buffer[_head];
        }

        public void Clear()
        {
            _count = 0;
            _tail  = 0;
            _head  = 0;
        }

        private static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException();
        }
    }
}