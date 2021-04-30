using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using MHLab.Utilities.Collections.Stackonly;

namespace MHLab.Utilities.Benchmarks.Collections.Stackonly
{
    [MemoryDiagnoser]
    public class FixedStackBenchmark
    {
        public struct TestStruct
        {
            public int X;
            public int Y;
        }

        [Benchmark]
        public void StandardStack_Pushing_16_Elements()
        {
            var entry = new TestStruct() {X = 1, Y = 1};
            var stack = new Stack<TestStruct>(16);

            for (var i = 0; i < 16; i++)
            {
                stack.Push(entry);
            }
        }

        [Benchmark]
        public void FixedStack_Pushing_16_Elements()
        {
            var entry = new TestStruct() {X = 1, Y = 1};
            
            FixedStack<TestStruct> stack = stackalloc TestStruct[16];

            for (var i = 0; i < 16; i++)
            {
                stack.Push(entry);
            }
        }
    }
}