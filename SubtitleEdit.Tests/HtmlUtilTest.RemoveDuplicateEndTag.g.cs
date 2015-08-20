// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlUtilTest.RemoveDuplicateEndTag.g.cs" company="">
//   
// </copyright>
// <summary>
//   The html util test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="HtmlUtilTest.RemoveDuplicateEndTag.g.cs" company="Nikse">Nikse</copyright>

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
        /// The remove duplicate end tag 33.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag33()
        {
            string s;
            s = this.RemoveDuplicateEndTag(string.Empty, 0, 0, EndTag);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The remove duplicate end tag 701.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag701()
        {
            string s;
            s = this.RemoveDuplicateEndTag(string.Empty, 0, 0, EndTag);
            Assert.AreEqual<string>(string.Empty, s);
        }

        /// <summary>
        /// The remove duplicate end tag 27.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag27()
        {
            string s;
            s = this.RemoveDuplicateEndTag("</i>- <i> -\0\0\0", 1, 2, EndTag);
            Assert.AreEqual<string>(" <i> -\0\0\0", s);
        }

        /// <summary>
        /// The remove duplicate end tag 370.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag370()
        {
            string s;
            s = this.RemoveDuplicateEndTag("</i>- <i>-\0\0\0\0", 1, 2, EndTag);
            Assert.AreEqual<string>(" <i>-\0\0\0\0", s);
        }

        /// <summary>
        /// The remove duplicate end tag 288.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag288()
        {
            string s;
            s = this.RemoveDuplicateEndTag("</i>-<i> -\0\0", 1, 2, EndTag);
            Assert.AreEqual<string>("<i> -\0\0", s);
        }

        /// <summary>
        /// The remove duplicate end tag 462.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag462()
        {
            string s;
            s = this.RemoveDuplicateEndTag("</i>-<i>-\0", 1, 2, EndTag);
            Assert.AreEqual<string>("<i>-\0", s);
        }

        /// <summary>
        /// The remove duplicate end tag 401.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag401()
        {
            string s;
            s = this.RemoveDuplicateEndTag((string)null, 0, 0, EndTag);
            Assert.AreEqual<string>((string)null, s);
        }

        /// <summary>
        /// The remove duplicate end tag 982.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(HtmlUtilTest))]
        public void RemoveDuplicateEndTag982()
        {
            string s;
            s = this.RemoveDuplicateEndTag((string)null, 1, 0, EndTag);
            Assert.AreEqual<string>((string)null, s);
        }
    }
}