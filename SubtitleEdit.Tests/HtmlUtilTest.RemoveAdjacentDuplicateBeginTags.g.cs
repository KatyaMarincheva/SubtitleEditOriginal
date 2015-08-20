// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.RemoveAdjacentDuplicateBeginTags.g.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="HtmlUtilTest.RemoveAdjacentDuplicateBeginTags.g.cs" company="Nikse">Nikse</copyright>

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
        /// The remove adjacent duplicate begin tags throws target invocation exception 990.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        [PexRaisedException(typeof(TargetInvocationException))]
        public void RemoveAdjacentDuplicateBeginTagsThrowsTargetInvocationException990()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags(string.Empty, BeginTag);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 958.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags958()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("\0", BeginTag);
            Assert.AreEqual<string>("\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 719.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags719()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("$", BeginTag);
            Assert.AreEqual<string>("$", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 86.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags86()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<", BeginTag);
            Assert.AreEqual<string>("<", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 182.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags182()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<\0\0", BeginTag);
            Assert.AreEqual<string>("<\0\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 512.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags512()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("$\0", BeginTag);
            Assert.AreEqual<string>("$\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 207.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags207()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("$0", BeginTag);
            Assert.AreEqual<string>("$0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 704.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags704()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("${\0", BeginTag);
            Assert.AreEqual<string>("${\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 20701.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags20701()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<i>", BeginTag);
            Assert.AreEqual<string>("<i>", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 302.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags302()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<i> ", BeginTag);
            Assert.AreEqual<string>("<i> ", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 705.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags705()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("\0\0", BeginTag);
            Assert.AreEqual<string>("\0\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 635.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags635()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<i><i>", BeginTag);
            Assert.AreEqual<string>("<i>", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 322.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags322()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<i><i>\0", BeginTag);
            Assert.AreEqual<string>("<i>\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 793.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags793()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("<i><i>\0\0\0", BeginTag);
            Assert.AreEqual<string>("<i>\0\0\0", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 858.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags858()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("${Ā", BeginTag);
            Assert.AreEqual<string>("${Ā", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 553.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags553()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("${Ā}", BeginTag);
            Assert.AreEqual<string>("${Ā}", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 347.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags347()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("${${", BeginTag);
            Assert.AreEqual<string>("${${", s);
        }

        /// <summary>
        /// The remove adjacent duplicate begin tags 147.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveAdjacentDuplicateBeginTags147()
        {
            string s;
            s = this.RemoveAdjacentDuplicateBeginTags("\0\0\0\0\0\0\0", BeginTag);
            Assert.AreEqual<string>("\0\0\0\0\0\0\0", s);
        }
    }
}