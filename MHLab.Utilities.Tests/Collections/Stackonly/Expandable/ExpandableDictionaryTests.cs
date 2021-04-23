using System.Collections.Generic;
using MHLab.Utilities.Collections.Stackonly.Expandable;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Collections.Stackonly.Expandable
{
    public class ExpandableDictionaryTests
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

            var sut = new ExpandableDictionary<int, TestStruct>(
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

            var sut = new ExpandableDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );

            sut.TryAdd(1, expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.TryAdd(2, expected));
            Assert.AreEqual(1, sut.Count);
        }

        [Test]
        public unsafe void Add_Item_After_Expand()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new ExpandableDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );

            sut.TryAdd(1, expected);
            Assert.AreEqual(1, sut.Count);

            Assert.False(sut.TryAdd(2, expected));
            Assert.AreEqual(1, sut.Count);

            sut.Expand(stackalloc int[1], stackalloc TestStruct[1]);
            
            Assert.True(sut.TryAdd(2, expected));
            Assert.AreEqual(2, sut.Count);
        }

        [Test]
        public unsafe void Get_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new ExpandableDictionary<int, TestStruct>(
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

                var sut = new ExpandableDictionary<int, TestStruct>(
                    stackalloc int[1],
                    stackalloc TestStruct[1]
                );
                sut.TryAdd(1, expected);
                
                ref var testStruct = ref sut.GetValueRef(2);
            });
        }

        [Test]
        public unsafe void Get_Item_After_Expanding_Success()
        {
            var expected1 = new TestStruct() {X = 3, Y = 1};
            var expected2 = new TestStruct() {X = 5, Y = 2};

            var sut = new ExpandableDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            
            sut.TryAdd(1, expected1);
            
            sut.Expand(stackalloc int[1], stackalloc TestStruct[1]);
            
            sut.TryAdd(2, expected2);
            
            var testStruct1 = sut.GetValueRef(1);
            var testStruct2 = sut.GetValueRef(2);

            Assert.AreEqual(expected1.X, testStruct1.X);
            Assert.AreEqual(expected1.Y, testStruct1.Y);
            Assert.AreEqual(expected2.X, testStruct2.X);
            Assert.AreEqual(expected2.Y, testStruct2.Y);
        }
        
        [Test]
        public unsafe void TryGet_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new ExpandableDictionary<int, TestStruct>(
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

            var sut = new ExpandableDictionary<int, TestStruct>(
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

            var sut = new ExpandableDictionary<int, TestStruct>(
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

            var sut = new ExpandableDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected);

            Assert.False(sut.TryRemove(2));
        }
        
        [Test]
        public unsafe void TryRemove_Item_After_Expanding()
        {
            var expected1 = new TestStruct() {X = 3, Y = 1};
            var expected2 = new TestStruct() {X = 6, Y = 3};

            var sut = new ExpandableDictionary<int, TestStruct>(
                stackalloc int[1],
                stackalloc TestStruct[1]
            );
            sut.TryAdd(1, expected1);
            
            sut.Expand(stackalloc int[1], stackalloc TestStruct[1]);
            
            sut.TryAdd(2, expected2);

            Assert.True(sut.TryRemove(1));
            Assert.False(sut.TryRemove(1));

            var actual = sut.GetValueRef(2);
            
            Assert.AreEqual(expected2.X, actual.X);
            Assert.AreEqual(expected2.Y, actual.Y);
        }

        [Test]
        public unsafe void Modify_Retrieved_Item_Success()
        {
            var expected = new TestStruct() {X = 3, Y = 1};

            var sut = new ExpandableDictionary<int, TestStruct>(
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

            var sut = new ExpandableDictionary<int, TestStruct>(
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