using System.Collections.Generic;
using Pjfb.Extensions;
using NUnit.Framework;

namespace Pjfb.Editor.Tests
{
    public class ListExtensionTest
    {
        [Test]
        public void IsNullOrEmpty()
        {
            Assert.AreEqual(true, ((List<int>) null).IsNullOrEmpty());
            Assert.AreEqual(true, new List<int>().IsNullOrEmpty());
            Assert.AreEqual(false, new List<int>{1}.IsNullOrEmpty());
        }
        
        [Test]
        public void ToCsv()
        {
            Assert.AreEqual("1,2,3", new List<int> {1, 2, 3}.ToCsv());
            Assert.AreEqual("1::2::3", new List<int> {1, 2, 3}.ToCsv(separator:"::"));
            Assert.AreEqual(string.Empty, new List<int> ().ToCsv());
            Assert.AreEqual(string.Empty, ((List<int>) null).ToCsv());
        }
        
        [Test]
        public void PopLast()
        {
            var list = new List<int> {1, 2, 3};
            Assert.AreEqual(3, list.PopLast());
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
        }

        [Test]
        public void ShiftOder()
        {
            Assert.AreEqual(new List<int>{}, new List<int>{}.ShiftOder(startIndex: 1));
            Assert.AreEqual(new List<int>{}, new List<int>{}.ShiftOder(startIndex: -1));
            
            Assert.AreEqual(null, ((List<int>)null).ShiftOder(startIndex: 1));
            Assert.AreEqual(null, ((List<int>)null).ShiftOder(startIndex: -1));
            
            Assert.AreEqual(new List<int>{ 1 }, new List<int>{ 1 }.ShiftOder(startIndex: 0));
            Assert.AreEqual(new List<int>{ 1 }, new List<int>{ 1 }.ShiftOder(startIndex: 1));
            Assert.AreEqual(new List<int>{ 1 }, new List<int>{ 1 }.ShiftOder(startIndex: -1));
            
            Assert.AreEqual(new List<int>{ 1,2,3,4,5 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: 0));
            Assert.AreEqual(new List<int>{ 2,3,4,5,1 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: 1));
            Assert.AreEqual(new List<int>{ 2,3,4,5,1 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: 6));
            Assert.AreEqual(new List<int>{ 3,4,5,1,2 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: 7));
            
            Assert.AreEqual(new List<int>{ 5,1,2,3,4 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: -1));
            Assert.AreEqual(new List<int>{ 4,5,1,2,3 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: -2));
            Assert.AreEqual(new List<int>{ 4,5,1,2,3 }, new List<int>{ 1,2,3,4,5 }.ShiftOder(startIndex: -7));
        }
    }
}
