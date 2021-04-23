using System.Collections.Generic;
using MHLab.Utilities.Collections.Stackonly;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Collections.Stackonly
{
    public class FixedDictionaryTests
    {
        private struct TestStruct
        {
            public int X;
            public int Y;
        }

        [Test]
        public unsafe void Add_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );

            Assert.AreEqual(1, sut.Capacity);
            Assert.AreEqual(0, sut.Count);

            Assert.True(sut.TryAdd(1, expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Add_Item_Fail()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );

            sut.TryAdd(1, expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.TryAdd(2, expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Get_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            ref var testStruct = ref sut.GetValueRef(1);

            Assert.AreEqual(expected.X, testStruct.X);
            Assert.AreEqual(expected.Y, testStruct.Y);
        }
        
        [Test]
        public unsafe void Get_Item_Fail()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var expected = new TestStruct() {X = 3, Y = 1};

                var sut = new FixedDictionary<int, TestStruct>(
                    stackalloc int[1],
                    stackalloc TestStruct[1]
                );
                sut.TryAdd(1, expected);
                
                ref var testStruct = ref sut.GetValueRef(2);
            });
        }
        
        [Test]
        public unsafe void TryGet_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            Assert.True(sut.TryGetValue(1, out var testStruct));

            Assert.AreEqual(expected.X, testStruct.X);
            Assert.AreEqual(expected.Y, testStruct.Y);
        }
        
        [Test]
        public unsafe void TryGet_Item_Fail()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            Assert.False(sut.TryGetValue(2, out var testStruct));
        }
        
        [Test]
        public unsafe void TryRemove_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            Assert.True(sut.TryRemove(1));

            Assert.AreEqual(0, sut.Count);
        }
        
        [Test]
        public unsafe void TryRemove_Item_Fail()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            Assert.False(sut.TryRemove(2));
        }

        [Test]
        public unsafe void Modify_Retrieved_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            ref TestStruct testStruct2 = ref sut.GetValueRef(1);
            testStruct2.X = 15;
            testStruct2.Y = 12;

            Assert.AreEqual(sut[1].X, testStruct2.X);
            Assert.AreEqual(sut[1].Y, testStruct2.Y);
        }
        
        [Test]
        public unsafe void Clear_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new FixedDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            
            sut.TryAdd(1, expected);
            Assert.AreEqual(1, sut.Count);

            sut.Clear();
            Assert.AreEqual(0, sut.Count);
        }
    }
}