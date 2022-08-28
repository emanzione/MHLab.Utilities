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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MHLab.Utilities.Asserts;

namespace MHLab.Utilities.Memory
{
    public static unsafe class MemoryHelper
    {
        public static void* Alloc(uint size)
        {
            Assert.Check(size > 0);
#if NATIVE_MEMORY_API_AVAILABLE
            return NativeMemory.Alloc(size);
#else
            return (void*)Marshal.AllocHGlobal((int)size);
#endif
        }
        
        public static void* AllocMultiple(uint elementSize, uint elementCount)
        {
            Assert.Check(elementSize > 0);
            Assert.Check(elementCount > 0);
            
#if NATIVE_MEMORY_API_AVAILABLE
            return NativeMemory.Alloc(elementSize, elementCount);
#else
            return (void*)Marshal.AllocHGlobal((int)(elementSize * elementCount));
#endif
        }

        public static void Free(void* ptr)
        {
#if NATIVE_MEMORY_API_AVAILABLE
            NativeMemory.Free(ptr);
#else
            Marshal.FreeHGlobal((IntPtr)ptr);
#endif
        }

        public static void* Realloc(void* ptr, uint newSize)
        {
            Assert.Check(ptr != null);
            Assert.Check(newSize != 0);
            
#if NATIVE_MEMORY_API_AVAILABLE
            return NativeMemory.Realloc(ptr, newSize);
#else
            return (void*)Marshal.ReAllocHGlobal((IntPtr)ptr, (IntPtr)newSize);
#endif
        }
    }
}