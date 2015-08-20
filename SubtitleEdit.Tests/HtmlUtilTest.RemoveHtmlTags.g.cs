// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.RemoveHtmlTags.g.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="HtmlUtilTest.RemoveHtmlTags.g.cs" company="Nikse">Nikse</copyright>

// <auto-generated>

// This file contains automatically generated tests.

// Do not modify this file manually.

// If the contents of this file becomes outdated, you can delete it.

// For example, if it no longer compiles.

// </auto-generated>
namespace Nikse.SubtitleEdit.Core
{
    using Microsoft.Pex.Framework.Generated;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The html util test.
    /// </summary>
    public partial class HtmlUtilTest
    {
        /// <summary>
        /// The remove html tags 246.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags246()
        {
            string s = this.RemoveHtmlTags((string)null, false);
            Assert.AreEqual<string>((string)null, s);
        }

        /// <summary>
        /// The remove html tags 117.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags117()
        {
            string s = this.RemoveHtmlTags("\0<\0 ", false);
            Assert.AreEqual<string>("\0<\0 ", s);
        }

        /// <summary>
        /// The remove html tags 158.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags158()
        {
            string s = this.RemoveHtmlTags("{\0}", true);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The remove html tags 576.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags576()
        {
            string s = this.RemoveHtmlTags("\0\0{", true);
            Assert.AreEqual<string>("\0\0{", s);
        }

        /// <summary>
        /// The remove html tags 278.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags278()
        {
            string s = this.RemoveHtmlTags("\0\0{}", true);
            Assert.AreEqual<string>("\0\0", s);
        }

        /// <summary>
        /// The remove html tags 450.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags450()
        {
            string s = this.RemoveHtmlTags("< <", false);
            Assert.AreEqual<string>("< <", s);
        }

        /// <summary>
        /// The remove html tags 252.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags252()
        {
            string s = this.RemoveHtmlTags("< i>", false);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The remove html tags 742.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags742()
        {
            string s = this.RemoveHtmlTags("< i\n", false);
            Assert.AreEqual<string>("< i", s);
        }

        /// <summary>
        /// The remove html tags 17.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags17()
        {
            string s = this.RemoveHtmlTags("< i>w", false);
            Assert.AreEqual<string>("w", s);
        }

        /// <summary>
        /// The remove html tags 634.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags634()
        {
            string s = this.RemoveHtmlTags("< i><i>", false);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The remove html tags 763.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags763()
        {
            string s = this.RemoveHtmlTags("<  {}", true);
            Assert.AreEqual<string>("<", s);
        }

        /// <summary>
        /// The remove html tags 821.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags821()
        {
            string s = this.RemoveHtmlTags("\0\0\0", true);
            Assert.AreEqual<string>("\0\0\0", s);
        }

        /// <summary>
        /// The remove html tags 835.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags835()
        {
            string s = this.RemoveHtmlTags("< ƀ", false);
            Assert.AreEqual<string>("< ƀ", s);
        }

        /// <summary>
        /// The remove html tags 235.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags235()
        {
            string s = this.RemoveHtmlTags("<< <", false);
            Assert.AreEqual<string>("<< <", s);
        }

        /// <summary>
        /// The remove html tags 332.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags332()
        {
            string s = this.RemoveHtmlTags("\0< ʀ\0{}", true);
            Assert.AreEqual<string>("\0< ʀ\0", s);
        }

        /// <summary>
        /// The remove html tags 97.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags97()
        {
            string s = this.RemoveHtmlTags("*< Ҁ>{}", PexSafeHelpers.ByteToBoolean((byte)128));
            Assert.AreEqual<string>("*< Ҁ>", s);
        }

        /// <summary>
        /// The remove html tags 658.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        [PexDescription("the test state was: duplicate path")]
        public void RemoveHtmlTags658()
        {
            string s = this.RemoveHtmlTags("< Ā>{}", PexSafeHelpers.ByteToBoolean((byte)8));
            Assert.AreEqual<string>("< Ā>", s);
        }

        /// <summary>
        /// The remove html tags 925.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags925()
        {
            string s = this.RemoveHtmlTags(" >< \f{}", true);
            Assert.AreEqual<string>(" ><", s);
        }

        /// <summary>
        /// The remove html tags 954.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags954()
        {
            string s = this.RemoveHtmlTags("﻿< \f", false);
            Assert.AreEqual<string>("﻿<", s);
        }

        /// <summary>
        /// The remove html tags 633.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveHtmlTags633()
        {
            string s = this.RemoveHtmlTags("{\0}{}", true);
            Assert.AreEqual<string>(string.Empty, s);
        }
    }
}