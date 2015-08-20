// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Core
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The html util test.
    /// </summary>
    [TestClass]
    public class HtmlUtilTest
    {
        /// <summary>
        /// The test remove open close tag cyrillic i.
        /// </summary>
        [TestMethod]
        public void TestRemoveOpenCloseTagCyrillicI()
        {
            const string source = "<\u0456>SubtitleEdit</\u0456>";
            Assert.AreEqual("SubtitleEdit", HtmlUtil.RemoveOpenCloseTags(source, HtmlUtil.TagCyrillicI));
        }

        /// <summary>
        /// The test remove open close tag font.
        /// </summary>
        [TestMethod]
        public void TestRemoveOpenCloseTagFont()
        {
            const string source = "<font color=\"#000\">SubtitleEdit</font>";
            Assert.AreEqual("SubtitleEdit", HtmlUtil.RemoveOpenCloseTags(source, HtmlUtil.TagFont));
        }

        /// <summary>
        /// The remove open close tags 880.
        /// </summary>
        [TestMethod]
        public void RemoveOpenCloseTags880()
        {
            string s;
            s = HtmlUtil.RemoveOpenCloseTags(string.Empty, (string[])null);
            Assert.AreEqual<string>(string.Empty, s);
        }
    }
}