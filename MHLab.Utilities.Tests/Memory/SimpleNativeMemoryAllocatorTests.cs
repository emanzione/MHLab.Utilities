using System;
using MHLab.Utilities.Memory;
using MHLab.Utilities.Memory.Allocators;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Memory;

public class SimpleNativeMemoryAllocatorTests
{
    private const int BlocksAmount = 16;
    private const int MaxBlockSize = 2048;

    private IMemoryAllocator _memoryAllocator;

    private Random _random;

    [SetUp]
    public void Setup()
    {
        _memoryAllocator = new SimpleNativeMemoryAllocator(BlocksAmount, MaxBlockSize);
        _random          = new Random();
    }

    [TearDown]
    public void Teardown()
    {
        try
        {
            _memoryAllocator.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private uint GetRandomAllocationSize()
    {
        return (uint)_random.Next(1, MaxBlockSize);
    }

    [Test]
    public void Allocate_Block()
    {
        var block = _memoryAllocator.Allocate(GetRandomAllocationSize());

        Assert.True(_memoryAllocator.HasLeakedMemory);
        Console.WriteLine($"Leaked memory: {_memoryAllocator.LeakedMemoryAmount}");

        _memoryAllocator.Free(ref block);
    }

    [Test]
    public void Allocate_More_Blocks()
    {
        for (var i = 0; i < BlocksAmount; i++)
        {
            var block = _memoryAllocator.Allocate(GetRandomAllocationSize());

            Assert.True(_memoryAllocator.HasLeakedMemory);
        }

        Console.WriteLine($"Leaked memory: {_memoryAllocator.LeakedMemoryAmount}");
    }

    [Test]
    public void Allocate_Too_Many_Blocks()
    {
        for (var i = 0; i < BlocksAmount; i++)
        {
            var block = _memoryAllocator.Allocate(3);

            Assert.True(_memoryAllocator.HasLeakedMemory);
        }

        Assert.Catch(() => _memoryAllocator.Allocate(3));
    }

    [Test]
    public void Free_Block()
    {
        var block = _memoryAllocator.Allocate(GetRandomAllocationSize());
        _memoryAllocator.Free(ref block);

        Assert.False(_memoryAllocator.HasLeakedMemory);
        Assert.That(_memoryAllocator.LeakedMemoryAmount, Is.Zero);
    }

    [Test]
    public void Free_More_Blocks()
    {
        var blocks = new MemoryBlock[BlocksAmount];

        for (var i = 0; i < BlocksAmount; i++)
        {
            var block = _memoryAllocator.Allocate(GetRandomAllocationSize());
            blocks[i] = block;

            Assert.True(_memoryAllocator.HasLeakedMemory);
        }

        for (var i = 0; i < BlocksAmount; i++)
        {
            unsafe
            {
                var block = blocks[i];

                _memoryAllocator.Free(ref block);
            }
        }

        Assert.False(_memoryAllocator.HasLeakedMemory);
    }
}