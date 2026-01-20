using System;
using Pjfb.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace Pjfb.Editor.Tests
{
    public class IntExtensionTest
    {
        [Test]
        public void GetStringNumberWithComma()
        {
            Assert.AreEqual("0", 0.GetStringNumberWithComma());
            Assert.AreEqual("234", 234.GetStringNumberWithComma());
            Assert.AreEqual("1,234", 1234.GetStringNumberWithComma());
            Assert.AreEqual("1,234,567", 1234567.GetStringNumberWithComma());

            Assert.AreEqual("1234", 1234.GetStringNumberWithComma(digitCount: 4));
            Assert.AreEqual("1,2345", 12345.GetStringNumberWithComma(digitCount: 4));
            Assert.AreEqual("1,2345,6789", 123456789.GetStringNumberWithComma(digitCount: 4));
        }
    }
}
