// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StripableTextTest.cs" company="">
//   
// </copyright>
// <summary>
//   The stripable text test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The stripable text test.
    /// </summary>
    [TestClass]
    public class StripableTextTest
    {
        /// <summary>
        /// The stripable text italic.
        /// </summary>
        [TestMethod]
        public void StripableTextItalic()
        {
            var st = new StripableText("<i>Hi!</i>");
            Assert.AreEqual(st.Pre, "<i>");
            Assert.AreEqual(st.Post, "!</i>");
            Assert.AreEqual(st.StrippedText, "Hi");
        }

        /// <summary>
        /// The stripable text ass.
        /// </summary>
        [TestMethod]
        public void StripableTextAss()
        {
            var st = new StripableText("{\\an9}Hi!");
            Assert.AreEqual(st.Pre, "{\\an9}");
            Assert.AreEqual(st.Post, "!");
            Assert.AreEqual(st.StrippedText, "Hi");
        }

        /// <summary>
        /// The stripable text font.
        /// </summary>
        [TestMethod]
        public void StripableTextFont()
        {
            var st = new StripableText("<font color=\"red\">Hi!</font>");
            Assert.AreEqual(st.Pre, "<font color=\"red\">");
            Assert.AreEqual(st.Post, "!</font>");
            Assert.AreEqual(st.StrippedText, "Hi");
        }

        /// <summary>
        /// The stripable text italic 2.
        /// </summary>
        [TestMethod]
        public void StripableTextItalic2()
        {
            var st = new StripableText("<i>O</i>");
            Assert.AreEqual(st.Pre, "<i>");
            Assert.AreEqual(st.Post, "</i>");
            Assert.AreEqual(st.StrippedText, "O");
        }

        /// <summary>
        /// The stripable text italic 3.
        /// </summary>
        [TestMethod]
        public void StripableTextItalic3()
        {
            var st = new StripableText("<i>Hi!");
            Assert.AreEqual(st.Pre, "<i>");
            Assert.AreEqual(st.Post, "!");
            Assert.AreEqual(st.StrippedText, "Hi");
        }

        /// <summary>
        /// The stripable text font dont touch.
        /// </summary>
        [TestMethod]
        public void StripableTextFontDontTouch()
        {
            var st = new StripableText("{MAN} Hi, how are you today!");
            Assert.AreEqual(st.Pre, string.Empty);
            Assert.AreEqual(st.Post, "!");
            Assert.AreEqual(st.StrippedText, "{MAN} Hi, how are you today");
        }

        /// <summary>
        /// The stripable only pre.
        /// </summary>
        [TestMethod]
        public void StripableOnlyPre()
        {
            var st = new StripableText("(");
            Assert.AreEqual(st.Pre, "(");
            Assert.AreEqual(st.Post, string.Empty);
            Assert.AreEqual(st.StrippedText, string.Empty);
        }

        /// <summary>
        /// The stripable only pre 2.
        /// </summary>
        [TestMethod]
        public void StripableOnlyPre2()
        {
            var st = new StripableText("<");
            Assert.AreEqual(st.Pre, string.Empty);
            Assert.AreEqual(st.Post, string.Empty);
            Assert.AreEqual(st.StrippedText, "<");
        }

        /// <summary>
        /// The stripable only pre 3.
        /// </summary>
        [TestMethod]
        public void StripableOnlyPre3()
        {
            var st = new StripableText("<i>");
            Assert.AreEqual(st.Pre, "<i>");
            Assert.AreEqual(st.Post, string.Empty);
            Assert.AreEqual(st.StrippedText, string.Empty);
        }

        /// <summary>
        /// The stripable only text.
        /// </summary>
        [TestMethod]
        public void StripableOnlyText()
        {
            var st = new StripableText("H");
            Assert.AreEqual(st.Pre, string.Empty);
            Assert.AreEqual(st.Post, string.Empty);
            Assert.AreEqual(st.StrippedText, "H");
        }

        /// <summary>
        /// The stripable text italic and font.
        /// </summary>
        [TestMethod]
        public void StripableTextItalicAndFont()
        {
            var st = new StripableText("<i><font color=\"red\">Hi!</font></i>");
            Assert.AreEqual(st.Pre, "<i><font color=\"red\">");
            Assert.AreEqual(st.Post, "!</font></i>");
            Assert.AreEqual(st.StrippedText, "Hi");
        }

        /// <summary>
        /// The stripable text italic and more.
        /// </summary>
        [TestMethod]
        public void StripableTextItalicAndMore()
        {
            var st = new StripableText("<i>...<b>Hi!</b></i>");
            Assert.AreEqual(st.Pre, "<i>...<b>");
            Assert.AreEqual(st.Post, "!</b></i>");
            Assert.AreEqual(st.StrippedText, "Hi");
        }
    }
}