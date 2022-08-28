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
using System.Runtime.InteropServices;

namespace MHLab.Utilities.Memory
{
    public static class SpanExtensions
    {
        [StructLayout(LayoutKind.Explicit)]
        private ref struct SpanReader
        {
            [FieldOffset(0)] public Span<byte> Span;

            [FieldOffset(0)] public readonly IntPtr Pointer;

            public SpanReader(Span<byte> span)
            {
                Pointer = IntPtr.Zero;
                Span    = span;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr AsIntPtr(this Span<byte> span)
        {
            var reader = new SpanReader(span);
            return reader.Pointer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* AsPointer(this Span<byte> span)
        {
            return (void*)span.AsIntPtr();
        }
    }
}