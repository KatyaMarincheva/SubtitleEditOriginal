// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.EncodeText.g.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="HtmlUtilTest.EncodeText.g.cs" company="Nikse">Nikse</copyright>

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
        /// The encode text 27201.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText27201()
        {
            string s;
            s = this.EncodeText(string.Empty);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The encode text 36701.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText36701()
        {
            string s;
            s = this.EncodeText("\0");
            Assert.AreEqual<string>("\0", s);
        }

        /// <summary>
        /// The encode text 74801.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText74801()
        {
            string s;
            s = this.EncodeText("\'");
            Assert.AreEqual<string>("&#39;", s);
        }

        /// <summary>
        /// The encode text 87001.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText87001()
        {
            string s;
            s = this.EncodeText((string)null);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The encode text 770.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText770()
        {
            string s;
            s = this.EncodeText("?");
            Assert.AreEqual<string>("?", s);
        }

        /// <summary>
        /// The encode text 414.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText414()
        {
            string s;
            s = this.EncodeText("");
            Assert.AreEqual<string>("&#128;", s);
        }

        /// <summary>
        /// The encode text 719.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText719()
        {
            string s;
            s = this.EncodeText("¡");
            Assert.AreEqual<string>("&#161;", s);
        }

        /// <summary>
        /// The encode text 919.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText919()
        {
            string s;
            s = this.EncodeText("\0\0");
            Assert.AreEqual<string>("\0\0", s);
        }

        /// <summary>
        /// The encode text 807.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText807()
        {
            string s;
            s = this.EncodeText("");
            Assert.AreEqual<string>("&#128;&#128;", s);
        }

        /// <summary>
        /// The encode text 256.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText256()
        {
            string s;
            s = this.EncodeText("¡¡");
            Assert.AreEqual<string>("&#161;&#161;", s);
        }

        /// <summary>
        /// The encode text 380.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText380()
        {
            string s;
            s = this.EncodeText("\0\0\0");
            Assert.AreEqual<string>("\0\0\0", s);
        }

        /// <summary>
        /// The encode text 696.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText696()
        {
            string s;
            s = this.EncodeText("\0\0\0\0");
            Assert.AreEqual<string>("\0\0\0\0", s);
        }

        /// <summary>
        /// The encode text 775.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void EncodeText775()
        {
            string s;
            s = this.EncodeText("\0\0\0\0\0");
            Assert.AreEqual<string>("\0\0\0\0\0", s);
        }
    }
}