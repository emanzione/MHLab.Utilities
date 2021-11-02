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

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MHLab.Utilities.Asserts
{
    public static partial class Assert
    {
        public static class Debug
        {
            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void Fail()
            {
                Assert.Fail();
            }

            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void Fail(string message)
            {
                Assert.Fail(message);
            }

            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void Check(bool condition)
            {
                Assert.Check(condition);
            }

            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void Check(bool condition, string message)
            {
                Assert.Check(condition, message);
            }

            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void NotNull(object obj)
            {
                Assert.NotNull(obj);
            }

            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void NotNull(object obj, string message)
            {
                Assert.NotNull(obj, message);
            }
        
            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void IsTypeOf<TTypeToCheck>(object obj)
            {
                NotNull(obj);
                if (obj is TTypeToCheck) return;
                Fail($"Expected [{typeof(TTypeToCheck).FullName}], found [{obj.GetType().FullName}]");
            }
        
            [Conditional("DEBUG")]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [DebuggerHidden]
            public static void IsTypeOf<TTypeToCheck, TExpectedType>()
            {
                if (typeof(TTypeToCheck) != typeof(TExpectedType))
                    Fail($"Expected [{typeof(TExpectedType).FullName}], found [{typeof(TTypeToCheck).FullName}]");
            }
        }
    }
}