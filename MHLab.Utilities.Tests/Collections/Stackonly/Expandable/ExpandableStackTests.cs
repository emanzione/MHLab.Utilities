using System;
using MHLab.Utilities.Collections.Stackonly;
using MHLab.Utilities.Collections.Stackonly.Expandable;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Collections.Stackonly.Expandable
{
    public class ExpandableStackTests
    {
        private struct TestStruct
        {
            public int X;
            public int Y;
        }

        [Test]
        public unsafe void Push_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];

            Assert.AreEqual(1, sut.Capacity);
            Assert.AreEqual(0, sut.Count);

            Assert.True(sut.Push(expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Push_Item_Fail()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];

            sut.Push(expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.Push(expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Push_Item_After_Expand()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];

            sut.Push(expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.Push(expected));
            Assert.AreEqual(1, sut.Count);

            sut.Expand(stackalloc TestStruct[1]);
            
            Assert.True(sut.Push(expected));
            Assert.AreEqual(2, sut.Count);
        }

        [Test]
        public unsafe void Pop_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];
            sut.Push(expected);

            var testStruct = sut.Pop();
            Assert.AreEqual(0, sut.Count);

            Assert.AreEqual(3, testStruct.X);
            Assert.AreEqual(1, testStruct.Y);
        }

        [Test]
        public unsafe void Pop_Item_After_Expanding_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];
            
            sut.Push(expected);
            
            sut.Expand(stackalloc TestStruct[1]);
            
            sut.Push(new TestStruct(){ X = 1, Y = 1 });
            
            sut.Pop();
            var testStruct = sut.Pop();
            Assert.AreEqual(0, sut.Count);

            Assert.AreEqual(expected.X, testStruct.X);
            Assert.AreEqual(expected.Y, testStruct.Y);
        }
        
        [Test]
        public unsafe void Peek_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];
            sut.Push(expected);

            var testStruct = sut.Peek();
            Assert.AreEqual(1, sut.Count);

            Assert.AreEqual(3, testStruct.X);
            Assert.AreEqual(1, testStruct.Y);
        }

        [Test]
        public unsafe void Pop_Item_Fail()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];
                sut.Pop();
            });
        }

        [Test]
        public unsafe void Modify_Popped_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];
            sut.Push(expected);

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

            ExpandableStack<TestStruct> sut = stackalloc TestStruct[1];

            sut.Push(expected);
            Assert.AreEqual(1, sut.Count);

            sut.Clear();
            Assert.AreEqual(0, sut.Count);
        }
    }
}