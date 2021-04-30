using BenchmarkDotNet.Running;
using MHLab.Utilities.Benchmarks.Collections.Stackonly;

namespace MHLab.Utilities.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FixedStackBenchmark>();
        }
    }
}