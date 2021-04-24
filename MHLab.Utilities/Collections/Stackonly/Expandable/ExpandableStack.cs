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

using System;
using MHLab.Utilities.Asserts;

namespace MHLab.Utilities.Collections.Stackonly.Expandable
{
    public ref struct ExpandableStack<TItem> where TItem : unmanaged
    {
        public int Count    => _count;
        public int Capacity => _buffer.Length;

        public bool IsEmpty => Count <= 0;
        public bool IsFull  => Count >= Capacity;

        private int _count;

        private ExpandableArray<TItem> _buffer;

        public ExpandableStack(Span<TItem> buffer)
        {
            Assert.Debug.Check(buffer.Length > 0);
            _count  = 0;
            _buffer = buffer;
        }

        public static implicit operator ExpandableStack<TItem>(Span<TItem> buffer) => new ExpandableStack<TItem>(buffer);

        public ref TItem this[int index] => ref _buffer[index];

        public bool Push(TItem item)
        {
            if (IsFull) return false;

            _buffer[_count++] = item;

            return true;
        }

        public ref TItem Pop()
        {
            if (IsEmpty) ThrowInvalidOperationException();

            return ref _buffer[--_count];
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

        public void Expand(ExpandableArray<TItem> buffer)
        {
            Assert.Debug.Check(buffer.Length > 0);
            _buffer.ExpandByRef(ref buffer);
        }

        private static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException();
        }
    }
}