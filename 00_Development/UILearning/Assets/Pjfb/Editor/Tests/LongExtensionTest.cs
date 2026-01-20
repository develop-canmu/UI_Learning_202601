using Pjfb.Extensions;
using NUnit.Framework;

namespace Pjfb.Editor.Tests
{
    public class LongExtensionTest
    {
        [Test]
        public void GetStringNumberWithComma()
        {
            Assert.AreEqual("0", ((long)0).GetStringNumberWithComma());
            Assert.AreEqual("234", ((long)234).GetStringNumberWithComma());
            Assert.AreEqual("1,234", ((long)1234).GetStringNumberWithComma());
            Assert.AreEqual("1,234,567", ((long)1234567).GetStringNumberWithComma());

            Assert.AreEqual("1234", ((long)1234).GetStringNumberWithComma(digitCount: 4));
            Assert.AreEqual("1,2345", ((long)12345).GetStringNumberWithComma(digitCount: 4));
            Assert.AreEqual("1,2345,6789", ((long)123456789).GetStringNumberWithComma(digitCount: 4));
        }
    }
}
