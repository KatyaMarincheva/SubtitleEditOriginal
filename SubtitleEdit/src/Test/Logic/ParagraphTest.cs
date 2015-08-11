using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic;

namespace Test.Logic
{
    [TestClass]
    public class ParagraphTest
    {
        [TestMethod]
        public void TestMethodNumberOfLinesTwoLines()
        {
            var p = new Paragraph { Text = "Hallo!" + Environment.NewLine + "How are you?" };
            Assert.AreEqual(2, p.NumberOfLines);
        }

        [TestMethod]
        public void TestMethodNumberOfLinesThreeLines()
        {
            // TODO: Implement me            
        }

        [TestMethod]
        public void TestMethodNumberOfLinesOneLine()
        {
            var p = new Paragraph { Text = "Hallo!" };
            Assert.AreEqual(1, p.NumberOfLines);
        }

        [TestMethod]
        public void TestMethodNumberOfLinesZero()
        {
            var p = new Paragraph { Text = string.Empty };
            Assert.AreEqual(0, p.NumberOfLines);
        }


    }
}
