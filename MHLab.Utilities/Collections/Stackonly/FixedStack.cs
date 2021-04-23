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
    public ref struct FixedStack<TItem>
    {
        public int Count    => _count;
        public int Capacity => _buffer.Length;

        public bool IsEmpty => Count <= 0;
        public bool IsFull  => Count >= Capacity;

        private int         _count;
        private Span<TItem> _buffer;

        public FixedStack(Span<TItem> buffer)
        {
            Assert.Debug.Check(buffer.Length > 0);
            _count  = 0;
            _buffer = buffer;
        }

        public static implicit operator FixedStack<TItem>(Span<TItem> buffer) => new FixedStack<TItem>(buffer);

        public ref TItem this[int index] => ref _buffer[index];

        public Span<TItem> GetBuffer()
        {
            return _buffer;
        }

        public bool Push(TItem item)
        {
            if (IsFull) return false;

            var index = _count;
            _count++;

            _buffer[index] = item;

            return true;
        }

        public ref TItem Pop()
        {
            if (IsEmpty) ThrowInvalidOperationException();

            var index = _count - 1;
            _count--;

            return ref _buffer[index];
        }

        public ref TItem Peek()
        {
            if (Count <= 0) ThrowInvalidOperationException();

            var index = _count - 1;

            return ref _buffer[index];
        }

        public void Clear()
        {
            _count = 0;
        }

        private static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException();
        }
    }
}