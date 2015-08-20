// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTest.cs" company="">
//   
// </copyright>
// <summary>
//   The unit test 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Core
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unit test 1.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// The line starts with html tag empty.
        /// </summary>
        [TestMethod]
        public void LineStartsWithHtmlTagEmpty()
        {
            string test = string.Empty;
            Assert.IsFalse(test.LineStartsWithHtmlTag(true));
        }

        /// <summary>
        /// The line starts with html tag null.
        /// </summary>
        [TestMethod]
        public void LineStartsWithHtmlTagNull()
        {
            string test = null;
            Assert.IsFalse(test.LineStartsWithHtmlTag(true));
        }

        /// <summary>
        /// The line starts with html tag italic.
        /// </summary>
        [TestMethod]
        public void LineStartsWithHtmlTagItalic()
        {
            const string test = "<i>Hej</i>";
            Assert.IsTrue(test.LineStartsWithHtmlTag(true));
        }

        /// <summary>
        /// The ends with empty.
        /// </summary>
        [TestMethod]
        public void EndsWithEmpty()
        {
            string test = string.Empty;
            Assert.IsFalse(test.EndsWith('?'));
        }

        /// <summary>
        /// The ends with html tag empty.
        /// </summary>
        [TestMethod]
        public void EndsWithHtmlTagEmpty()
        {
            string test = string.Empty;
            Assert.IsFalse(test.LineEndsWithHtmlTag(true));
        }

        /// <summary>
        /// The ends with html tag italic.
        /// </summary>
        [TestMethod]
        public void EndsWithHtmlTagItalic()
        {
            const string test = "<i>Hej</i>";
            Assert.IsTrue(test.LineEndsWithHtmlTag(true));
        }

        /// <summary>
        /// The line break starts with html tag empty.
        /// </summary>
        [TestMethod]
        public void LineBreakStartsWithHtmlTagEmpty()
        {
            string test = string.Empty;
            Assert.IsFalse(test.LineBreakStartsWithHtmlTag(true));
        }

        /// <summary>
        /// The line break starts with html tag italic.
        /// </summary>
        [TestMethod]
        public void LineBreakStartsWithHtmlTagItalic()
        {
            string test = "<i>Hej</i>" + Environment.NewLine + "<i>Hej</i>";
            Assert.IsTrue(test.LineBreakStartsWithHtmlTag(true));
        }

        /// <summary>
        /// The line break starts with html tag font.
        /// </summary>
        [TestMethod]
        public void LineBreakStartsWithHtmlTagFont()
        {
            string test = "Hej!" + Environment.NewLine + "<font color=FFFFFF>Hej!</font>";
            Assert.IsTrue(test.LineBreakStartsWithHtmlTag(true, true));
        }

        // QUESTION: fix three lines?
        // [TestMethod]
        // public void LineBreakStartsWithHtmlTagFontThreeLines()
        // {
        // string test = "Hej!" + Environment.NewLine + "Hej!" + Environment.NewLine + "<font color=FFFFFF>Hej!</font>";
        // Assert.IsTrue(test.LineBreakStartsWithHtmlTag(true, true));
        // }

        /// <summary>
        /// The line break starts with html tag font false.
        /// </summary>
        [TestMethod]
        public void LineBreakStartsWithHtmlTagFontFalse()
        {
            const string test = "<font color=FFFFFF>Hej!</font>";
            Assert.IsFalse(test.LineBreakStartsWithHtmlTag(true, true));
        }
    }
}