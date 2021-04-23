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
    internal unsafe ref struct ArrayChunk
    {
        public ArrayChunk* Next;
        public void*       Buffer;
        public int         Size;

        public ArrayChunk(void* buffer, int size)
        {
            Buffer = buffer;
            Size   = size;
            Next   = null;
        }

        public static ArrayChunk Create<TItem>(Span<TItem> buffer) where TItem : unmanaged
        {
            fixed (TItem* item = &buffer[0])
            {
                return new ArrayChunk(item, buffer.Length);
            }
        }
    }

    public unsafe ref struct ExpandableArray<TItem> where TItem : unmanaged
    {
        private ArrayChunk _chunk;

        public ExpandableArray(Span<TItem> buffer)
        {
            Assert.Debug.Check(buffer.Length > 0);
            
            _chunk = ArrayChunk.Create(buffer);
        }

        public static ExpandableArray<TItem> Create(Span<TItem> buffer) => new ExpandableArray<TItem>(buffer);
        
        public static implicit operator ExpandableArray<TItem>(Span<TItem> buffer) => Create(buffer);

        public void Expand(ExpandableArray<TItem> array)
        {
            Assert.Debug.Check(array.Length > 0);
            
            if (_chunk.Next == null)
            {
                _chunk.Next = &array._chunk;
                return;
            }

            var nextChunk = _chunk.Next;

            while (true)
            {
                if (nextChunk->Next == null)
                {
                    nextChunk->Next = &array._chunk;
                    return;
                }
            }
        }
        
        internal void ExpandByRef(ref ExpandableArray<TItem> array)
        {
            Assert.Debug.Check(array.Length > 0);
            
            fixed (ArrayChunk* chunkPtr = &array._chunk)
            {
                if (_chunk.Next == null)
                {
                    _chunk.Next = chunkPtr;
                    return;
                }

                var nextChunk = _chunk.Next;

                while (true)
                {
                    if (nextChunk->Next == null)
                    {
                        nextChunk->Next = chunkPtr;
                        return;
                    }
                }
            }
        }

        public int Length => GetLength();
        
        public int GetLength()
        {
            var length = 0;

            var currentChunk = _chunk;

            while (true)
            {
                length += currentChunk.Size;

                if (currentChunk.Next == null)
                    break;

                currentChunk = *_chunk.Next;
            }

            return length;
        }

        public TItem* GetAtIndexUnsafe(int index)
        {
            var currentChunk = _chunk;
            var currentIndex = index;
            
            while (true)
            {
                if (currentChunk.Size > currentIndex)
                {
                    // Found it.
                    return ((TItem*)currentChunk.Buffer) + currentIndex;
                }

                if (currentChunk.Next == null)
                    // Cannot access out of bound indexes.
                    throw new IndexOutOfRangeException();
                
                currentIndex -= currentChunk.Size;
                currentChunk =  *currentChunk.Next;
            }
        }

        public ref TItem GetAtIndex(int index)
        {
            var currentChunk = _chunk;
            var currentIndex = index;
            
            while (true)
            {
                if (currentChunk.Size > currentIndex)
                {
                    // Found it.
                    var span = new Span<TItem>(currentChunk.Buffer, currentChunk.Size);
                    return ref span[currentIndex];
                }

                if (currentChunk.Next == null)
                    // Cannot access out of bound indexes.
                    throw new IndexOutOfRangeException();
                
                currentIndex -= currentChunk.Size;
                currentChunk =  *currentChunk.Next;
            }
        }
        
        public ref TItem this[int index] => ref GetAtIndex(index);
    }
}