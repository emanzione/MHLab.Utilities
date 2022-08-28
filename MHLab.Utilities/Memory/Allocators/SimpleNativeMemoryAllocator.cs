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

namespace MHLab.Utilities.Memory.Allocators;

public unsafe class SimpleNativeMemoryAllocator : IMemoryAllocator
{
    private unsafe class BlockPool : IDisposable
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Block
        {
            [FieldOffset(0)] public void* NextBlock;
        }

        public readonly uint BlockSize;
        public readonly uint BlocksCount;

        private void* _rootMemoryPtr;

        private void* _nextFreeBlock;

        private ulong _currentLeakedBlocks;

        public static uint MinimumBlockSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)sizeof(Block);
        }

        public bool  HasLeakedBlocks        => _currentLeakedBlocks > 0;
        public ulong LeakedBlocksCount      => _currentLeakedBlocks;
        public bool  IsEmpty                => _currentLeakedBlocks >= BlocksCount;
        public uint  AvailableMemoryInBytes => BlocksCount * BlockSize;

        public BlockPool(uint blockSize, uint blocksCount)
        {
            Assert.Check(blockSize > 0);
            Assert.Check(blocksCount > 0);
            Assert.Check(blockSize >= MinimumBlockSize);

            BlockSize   = blockSize;
            BlocksCount = blocksCount;

            _rootMemoryPtr = MemoryHelper.AllocMultiple(blockSize, blocksCount);
            _nextFreeBlock = null;

            InitializeFreeBlocks();
        }

        private void InitializeFreeBlocks()
        {
            for (var i = 0; i < BlocksCount; i++)
            {
                var currentPosition = (byte*)_rootMemoryPtr + (int)(i * BlockSize);
                var currentBlock    = (Block*)currentPosition;

                currentBlock->NextBlock = _nextFreeBlock;

                _nextFreeBlock = currentBlock;
            }
        }

        public void Dispose()
        {
            MemoryHelper.Free(_rootMemoryPtr);
        }

        public void* Allocate()
        {
            var nextFreeBlock = _nextFreeBlock;

            if (nextFreeBlock == null)
            {
                return null;
            }

            var block = (Block*)nextFreeBlock;

            _nextFreeBlock   = block->NextBlock;
            block->NextBlock = default;

            _currentLeakedBlocks++;

            return nextFreeBlock;
        }

        public void Free(void* blockPtr)
        {
            var block = (Block*)blockPtr;
            block->NextBlock = _nextFreeBlock;

            _nextFreeBlock = blockPtr;
            _currentLeakedBlocks--;
        }

        public bool IsOwnerOf(void* blockPtr)
        {
            return ((byte*)blockPtr >= (byte*)_rootMemoryPtr) &&
                   ((byte*)blockPtr < (byte*)_rootMemoryPtr + AvailableMemoryInBytes);
        }
    }

    private readonly List<BlockPool> _pools;

    public bool HasLeakedMemory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            for (var i = 0; i < _pools.Count; i++)
            {
                if (_pools[i].HasLeakedBlocks)
                    return true;
            }

            return false;
        }
    }

    public ulong LeakedMemoryAmount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var count = 0UL;
            for (var i = 0; i < _pools.Count; i++)
            {
                count += _pools[i].LeakedBlocksCount * _pools[i].BlockSize;
            }

            return count;
        }
    }

    /// <param name="initialBucketSize">The initial size of every bucket/pool of blocks.</param>
    /// <param name="maxBlockSize">The maximum allowed size for a block.</param>
    public SimpleNativeMemoryAllocator(uint initialBucketSize, uint maxBlockSize,
        Func<uint, uint> blockSizeIncrementFunction = null)
    {
        _pools = new List<BlockPool>();

        var currentBlockSize = BlockPool.MinimumBlockSize;

        if (blockSizeIncrementFunction == null)
            blockSizeIncrementFunction = IncrementBlockSizeByPowerOfTwo;

        while (true)
        {
            if (currentBlockSize >= maxBlockSize)
            {
                _pools.Add(new BlockPool(maxBlockSize, initialBucketSize));
                break;
            }

            _pools.Add(new BlockPool(currentBlockSize, initialBucketSize));

            currentBlockSize = blockSizeIncrementFunction(currentBlockSize);
        }
    }
    
    private uint IncrementBlockSizeByPowerOfTwo(uint currentBlockSize)
    {
        return currentBlockSize * 2;
    }

    public MemoryBlock Allocate(uint requestedSize)
    {
        for (var i = 0; i < _pools.Count; i++)
        {
            var pool = _pools[i];

            if (requestedSize > pool.BlockSize) continue;

            var allocationResult = pool.Allocate();

            if (allocationResult == null)
                Assert.Fail(
                    $"The allocation of {requestedSize} bytes failed. Probably the allocator exhausted the blocks for the requested size.");

            return new MemoryBlock(allocationResult, requestedSize);
        }

        Assert.Fail(
            $"The allocation of {requestedSize} bytes failed. The allocator doesn't support the requested size.");
        return MemoryBlock.InvalidBlock;
    }

    public MemoryBlock<T> Allocate<T>(uint count) where T : unmanaged
    {
        var typeSize = (uint)Unsafe.SizeOf<T>();

        var block = Allocate(typeSize * count);
        return (MemoryBlock<T>)block;
    }

    public void Free(ref MemoryBlock block)
    {
        for (var i = 0; i < _pools.Count; i++)
        {
            var pool = _pools[i];

            if (pool.IsOwnerOf(block))
            {
                pool.Free(block);
                block = MemoryBlock.InvalidBlock;
                return;
            }
        }

        Assert.Fail($"The memory you requested to free doesn't belong to this allocator.");
    }

    public void Free<T>(ref MemoryBlock<T> block) where T : unmanaged
    {
        var memoryBlock = (MemoryBlock)block;
        Free(ref memoryBlock);
        block = MemoryBlock<T>.InvalidBlock;
    }

    public void Dispose()
    {
        var hasLeakedBlocks   = HasLeakedMemory;
        var leakedBlocksCount = LeakedMemoryAmount;

        foreach (var pool in _pools)
        {
            pool.Dispose();
        }

        if (hasLeakedBlocks)
            Assert.Fail($"The allocator detected that {leakedBlocksCount} bytes of memory leaked.");
    }
}