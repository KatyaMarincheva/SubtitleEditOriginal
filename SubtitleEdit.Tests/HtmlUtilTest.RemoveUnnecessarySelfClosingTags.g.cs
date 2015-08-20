// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.RemoveUnnecessarySelfClosingTags.g.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="HtmlUtilTest.RemoveUnnecessarySelfClosingTags.g.cs" company="Nikse">Nikse</copyright>

// <auto-generated>

// This file contains automatically generated tests.

// Do not modify this file manually.

// If the contents of this file becomes outdated, you can delete it.

// For example, if it no longer compiles.

// </auto-generated>
namespace Nikse.SubtitleEdit.Core
{
    using System.Reflection;

    using Microsoft.Pex.Framework.Generated;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The html util test.
    /// </summary>
    public partial class HtmlUtilTest
    {
        /// <summary>
        /// The remove unnecessary self closing tags throws target invocation exception 571.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        [PexRaisedException(typeof(TargetInvocationException))]
        public void RemoveUnnecessarySelfClosingTagsThrowsTargetInvocationException571()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags(string.Empty, BeginTag, EndTag);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 538.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags538()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("䀀", BeginTag, EndTag);
            Assert.AreEqual<string>("䀀", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 308.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags308()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags(string.Empty, BeginTag, EndTag);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 255.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags255()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("\0\0\0\0", BeginTag, EndTag);
            Assert.AreEqual<string>("\0\0\0\0", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 600.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags600()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("<\0\0\0", BeginTag, EndTag);
            Assert.AreEqual<string>("<\0\0\0", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 472.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags472()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("<i\0\0", BeginTag, EndTag);
            Assert.AreEqual<string>("<i\0\0", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 193.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags193()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("<i/\0", BeginTag, EndTag);
            Assert.AreEqual<string>("<i/\0", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 659.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags659()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("<i/>\0", BeginTag, EndTag);
            Assert.AreEqual<string>("\0", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 660.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags660()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("<i>\0<i/>\0", BeginTag, EndTag);
            Assert.AreEqual<string>("<i>\0</i>\0", s);
        }

        /// <summary>
        /// The remove unnecessary self closing tags 140.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveUnnecessarySelfClosingTags140()
        {
            string s;
            s = this.RemoveUnnecessarySelfClosingTags("<<\0\0", BeginTag, EndTag);
            Assert.AreEqual<string>("<<\0\0", s);
        }
    }
}