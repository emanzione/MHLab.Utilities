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
using System.Collections.Generic;
using MHLab.Utilities.Asserts;

namespace MHLab.Utilities.Collections.Stackonly
{
    public ref struct FixedDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public int Count    => _count;
        public int Capacity => _keyBuffer.Length;

        public bool IsEmpty => Count <= 0;
        public bool IsFull  => Count >= Capacity;

        private int          _count;
        private Span<TKey>   _keyBuffer;
        private Span<TValue> _valueBuffer;
        
        public FixedDictionary(Span<TKey> keys, Span<TValue> values)
        {
            Assert.Debug.Check(keys.Length > 0);
            Assert.Debug.Check(keys.Length == values.Length);
            _count       = 0;
            _keyBuffer   = keys;
            _valueBuffer = values;
        }
        
        public ref TValue this[TKey index] => ref GetValueRef(index);

        public Span<TKey> GetKeysBuffer()
        {
            return _keyBuffer;
        }
        
        public Span<TValue> GetValuesBuffer()
        {
            return _valueBuffer;
        }

        public bool TryAdd(TKey key, TValue item)
        {
            if (IsFull) return false;

            var index = _count;
            _count++;

            _keyBuffer[index]   = key;
            _valueBuffer[index] = item;

            return true;
        }

        public ref TValue GetValueRef(TKey key)
        {
            if (IsEmpty) throw new KeyNotFoundException();

            for (var i = 0; i < Count; i++)
            {
                var compareKey = _keyBuffer[i];
                if (compareKey.Equals(key))
                {
                    return ref _valueBuffer[i];
                }
            }

            throw new KeyNotFoundException();
        }
        
        public bool TryGetValue(TKey key, out TValue item)
        {
            item = default;
            
            if (IsEmpty)
            {
                return false;
            }

            for (var i = 0; i < Count; i++)
            {
                var compareKey = _keyBuffer[i];
                if (compareKey.Equals(key))
                {
                    item = _valueBuffer[i];
                    return true;
                }
            }
            
            return false;
        }
        
        public bool TryRemove(TKey key)
        {
            if (IsEmpty) return false;

            var index = -1;
            for (var i = 0; i < Count; i++)
            {
                var compareKey = _keyBuffer[i];
                if (compareKey.Equals(key))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            var lastIndex = _count - 1;
            _count--;

            if (lastIndex == index) return true;

            _keyBuffer[index]   = _keyBuffer[lastIndex];
            _valueBuffer[index] = _valueBuffer[lastIndex];

            return true;
        }

        public void Clear()
        {
            _count = 0;
        }
    }
}