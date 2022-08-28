/*
The MIT License (MIT)

Copyright (c) 2022 Emanuele Manzione

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
using System.Runtime.CompilerServices;
using MHLab.Utilities.Asserts;

namespace MHLab.Utilities.Memory
{
    public readonly unsafe struct MemoryBlock
    {
        private readonly void* _pointer;
        private readonly uint  _size;

        internal MemoryBlock(void* pointer, uint size)
        {
            _pointer = pointer;
            _size    = size;
        }

        public static MemoryBlock InvalidBlock => new MemoryBlock(null, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInvalid() => _pointer == null && _size == 0;

        public uint Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _size;
        }

        public byte this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Assert.Debug.Check(index < _size);
                return *((byte*)_pointer + index);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Assert.Debug.Check(index < _size);
                *((byte*)_pointer + index) = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(in MemoryBlock block) => block._pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => AsSpan(0, _size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(uint offset, uint size) => new Span<byte>((byte*)_pointer + offset, (int)size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan<T>() where T : unmanaged =>
            AsSpan<T>(0, _size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan<T>(uint offset, uint size) where T : unmanaged =>
            new Span<T>((T*)_pointer + offset, (int)size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* AsPointer() => _pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* AsPointer<T>() where T : unmanaged => (T*)_pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(MemoryBlock block, uint offset, uint size)
        {
            Assert.Debug.Check(size <= _size);
            Unsafe.CopyBlockUnaligned(_pointer, ((byte*)block._pointer + offset), size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(MemoryBlock block)
        {
            CopyFrom(block, 0, block._size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(byte[] block, uint offset, uint size)
        {
            Assert.Debug.Check(size <= _size);
            for (var i = 0; i < size - offset; i++)
            {
                *((byte*)_pointer + i) = block[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(byte[] block)
        {
            CopyFrom(block, 0, (uint)block.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(IntPtr block, uint offset, uint size)
        {
            Assert.Debug.Check(size <= _size);
            Unsafe.CopyBlockUnaligned(_pointer, ((byte*)block + offset), size);
        }
    }

    public readonly unsafe struct MemoryBlock<T> where T : unmanaged
    {
        private readonly T*   _pointer;
        private readonly uint _size;

        internal MemoryBlock(T* pointer, uint size)
        {
            _pointer = pointer;
            _size    = size;
        }

        public static MemoryBlock<T> InvalidBlock => new MemoryBlock<T>(null, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInvalid() => _pointer == null && _size == 0;

        public uint Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _size;
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Assert.Debug.Check(index < _size);
                return ref *(_pointer + index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetElementAt(T element, uint index)
        {
            Assert.Debug.Check(index < _size);
            *(_pointer + index) = element;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T*(in MemoryBlock<T> block) => block._pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(in MemoryBlock<T> block) => block._pointer;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator MemoryBlock(in MemoryBlock<T> block) => new MemoryBlock(block._pointer, block._size);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator MemoryBlock<T>(in MemoryBlock block) => new MemoryBlock<T>(block.AsPointer<T>(), block.Size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator MemoryBlock<T>(T* pointer) => new MemoryBlock<T>(pointer, 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => AsSpan(0, _size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(uint offset, uint size) => new Span<T>(_pointer + offset, (int)size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* AsPointer() => _pointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* AsPointer(uint index) => _pointer + index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(MemoryBlock<T> block, uint offset, uint size)
        {
            Assert.Debug.Check(size <= _size);
            Unsafe.CopyBlockUnaligned(_pointer, (block._pointer + offset), size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(MemoryBlock<T> block)
        {
            CopyFrom(block, 0, block._size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(T[] block, uint offset, uint size)
        {
            Assert.Debug.Check(size <= _size);
            for (var i = 0; i < size - offset; i++)
            {
                *(_pointer + i) = block[i];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(T[] block)
        {
            CopyFrom(block, 0, (uint)block.Length);
        }
    }
}