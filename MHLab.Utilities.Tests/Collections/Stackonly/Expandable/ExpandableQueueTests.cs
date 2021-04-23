using System;
using MHLab.Utilities.Collections.Stackonly.Expandable;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Collections.Stackonly.Expandable
{
    public class ExpandableQueueTests
    {
        private struct TestStruct
        {
            public int X;
            public int Y;
        }

        [Test]
        public unsafe void Enqueue_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];

            Assert.AreEqual(1, sut.Capacity);
            Assert.AreEqual(0, sut.Count);

            Assert.True(sut.Enqueue(expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Enqueue_Item_Fail()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];

            sut.Enqueue(expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.Enqueue(expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Enqueue_Item_After_Expand()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];

            sut.Enqueue(expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.Enqueue(expected));
            Assert.AreEqual(1, sut.Count);

            sut.Expand(stackalloc TestStruct[1]);
            
            Assert.True(sut.Enqueue(expected));
            Assert.AreEqual(2, sut.Count);
        }

        [Test]
        public unsafe void Dequeue_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];
            sut.Enqueue(expected);

            var testStruct = sut.Dequeue();
            Assert.AreEqual(0, sut.Count);

            Assert.AreEqual(3, testStruct.X);
            Assert.AreEqual(1, testStruct.Y);
        }

        [Test]
        public unsafe void Dequeue_Item_After_Expanding_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];
            
            sut.Enqueue(new TestStruct(){ X = 1, Y = 1 });
            
            sut.Expand(stackalloc TestStruct[1]);
            
            sut.Enqueue(expected);
            
            sut.Dequeue();
            var testStruct = sut.Dequeue();
            Assert.AreEqual(0, sut.Count);

            Assert.AreEqual(expected.X, testStruct.X);
            Assert.AreEqual(expected.Y, testStruct.Y);
        }
        
        [Test]
        public unsafe void Peek_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];
            sut.Enqueue(expected);

            var testStruct = sut.Peek();
            Assert.AreEqual(1, sut.Count);

            Assert.AreEqual(3, testStruct.X);
            Assert.AreEqual(1, testStruct.Y);
        }

        [Test]
        public unsafe void Dequeue_Item_Fail()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];
                sut.Dequeue();
            });
        }

        [Test]
        public unsafe void Wrapping_Success()
        {
            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];

            for (var i = 0; i < 20; i++)
            {
                var expected = new TestStruct() {X = i, Y = i};
                sut.Enqueue(expected);

                ref var actual = ref sut.Dequeue();
                
                Assert.AreEqual(expected.X, actual.X);
                Assert.AreEqual(expected.Y, actual.Y);
            }
            
            Assert.AreEqual(0, sut.Count);
        }

        [Test]
        public unsafe void Modify_Dequeued_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];
            sut.Enqueue(expected);

            ref var actual = ref sut.Peek();
            actual.X = 5;
            actual.Y = 18;

            Assert.AreEqual(sut[0].X, actual.X);
            Assert.AreEqual(sut[0].Y, actual.Y);
        }
        
        [Test]
        public unsafe void Clear_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[1];

            sut.Enqueue(expected);
            Assert.AreEqual(1, sut.Count);

            sut.Clear();
            Assert.AreEqual(0, sut.Count);
        }
        
        [Test]
        public unsafe void Multiple_Usages()
        {
            ExpandableQueue<TestStruct> sut = stackalloc TestStruct[10];

            for (var i = 0; i < 10; i++)
            { 
                sut.Enqueue(new TestStruct() {X = i, Y = i});
            }
            
            for (var i = 0; i < 10; i++)
            {
                var actual = sut.Dequeue();
                Assert.AreEqual(i, actual.X);
                Assert.AreEqual(i, actual.Y);
            }
        }
    }
}