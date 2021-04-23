using System;
using MHLab.Utilities.Collections.Stackonly;
using MHLab.Utilities.Collections.Stackonly.Expandable;
using NUnit.Framework;

namespace MHLab.Utilities.Tests.Collections.Stackonly
{
    public class ExpandableArrayTests
    {
        private struct TestStruct
        {
            public int X;
            public int Y;
        }
        
        [Test]
        public unsafe void Size_Correctly_Set()
        {
            ExpandableArray<TestStruct> sut = stackalloc TestStruct[1];

            Assert.AreEqual(1, sut.Length);
        }
        
        [Test]
        public unsafe void Size_Correctly_Set_After_Expand()
        {
            ExpandableArray<TestStruct> sut = stackalloc TestStruct[1];
            
            sut.Expand(stackalloc TestStruct[1]);

            Assert.AreEqual(2, sut.Length);
        }
        
        [Test]
        public unsafe void Item_Access_Out_Of_Bound()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                ExpandableArray<TestStruct> sut = stackalloc TestStruct[1];

                var expected = sut[1];
            });
        }
        
        [Test]
        public unsafe void Item_Correctly_Set()
        {
            ExpandableArray<TestStruct> sut = stackalloc TestStruct[1];

            ref var expected = ref sut[0];
            expected.X = 5;
            expected.Y = 1;

            var actual = sut[0];
            
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Y, actual.Y);
        }
        
        [Test]
        public unsafe void Item_Correctly_Set_After_Expand()
        {
            ExpandableArray<TestStruct> sut = stackalloc TestStruct[1];
            sut.Expand(stackalloc TestStruct[1]);
            
            ref var expected = ref sut[1];
            expected.X = 5;
            expected.Y = 1;

            var actual = sut[1];
            
            Assert.AreEqual(expected.X, actual.X);
            Assert.AreEqual(expected.Y, actual.Y);
        }
    }
}