// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParagraphTest.cs" company="">
//   
// </copyright>
// <summary>
//   The paragraph test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The paragraph test.
    /// </summary>
    [TestClass]
    public class ParagraphTest
    {
        /// <summary>
        /// The test method number of lines two lines.
        /// </summary>
        [TestMethod]
        public void TestMethodNumberOfLinesTwoLines()
        {
            var p = new Paragraph { Text = "Hallo!" + Environment.NewLine + "How are you?" };
            Assert.AreEqual(2, p.NumberOfLines);
        }

        /// <summary>
        /// The test method number of lines three lines.
        /// </summary>
        [TestMethod]
        public void TestMethodNumberOfLinesThreeLines()
        {
            var p = new Paragraph { Text = "Hallo!" + Environment.NewLine + Environment.NewLine + "How are you?" };
            Assert.AreEqual(3, p.NumberOfLines);
        }

        /// <summary>
        /// The test method number of lines one line.
        /// </summary>
        [TestMethod]
        public void TestMethodNumberOfLinesOneLine()
        {
            var p = new Paragraph { Text = "Hallo!" };
            Assert.AreEqual(1, p.NumberOfLines);
        }

        /// <summary>
        /// The test method number of lines zero.
        /// </summary>
        [TestMethod]
        public void TestMethodNumberOfLinesZero()
        {
            var p = new Paragraph { Text = string.Empty };
            Assert.AreEqual(0, p.NumberOfLines);
        }
    }
}