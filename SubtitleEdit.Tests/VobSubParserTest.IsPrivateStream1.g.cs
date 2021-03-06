// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubParserTest.IsPrivateStream1.g.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub parser test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="VobSubParserTest.IsPrivateStream1.g.cs" company="Nikse">Nikse</copyright>

// <auto-generated>

// This file contains automatically generated tests.

// Do not modify this file manually.

// If the contents of this file becomes outdated, you can delete it.

// For example, if it no longer compiles.

// </auto-generated>
namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using Microsoft.Pex.Framework.Generated;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The vob sub parser test.
    /// </summary>
    public partial class VobSubParserTest
    {
        /// <summary>
        /// The is private stream 1789.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(VobSubParserTest))]
        public void IsPrivateStream1789()
        {
            bool b;
            byte[] bs = new byte[0];
            b = this.IsPrivateStream1(bs, 0);
            Assert.AreEqual<bool>(false, b);
        }

        /// <summary>
        /// The is private stream 1102.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(VobSubParserTest))]
        public void IsPrivateStream1102()
        {
            bool b;
            byte[] bs = new byte[3];
            b = this.IsPrivateStream1(bs, 0);
            Assert.AreEqual<bool>(false, b);
        }

        /// <summary>
        /// The is private stream 1508.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(VobSubParserTest))]
        public void IsPrivateStream1508()
        {
            bool b;
            byte[] bs = new byte[3];
            bs[1] = (byte)1;
            b = this.IsPrivateStream1(bs, 0);
            Assert.AreEqual<bool>(false, b);
        }

        /// <summary>
        /// The is private stream 1499.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(VobSubParserTest))]
        public void IsPrivateStream1499()
        {
            bool b;
            byte[] bs = new byte[3];
            bs[0] = (byte)1;
            b = this.IsPrivateStream1(bs, 0);
            Assert.AreEqual<bool>(false, b);
        }

        /// <summary>
        /// The is private stream 1645.
        /// </summary>
        [TestMethod]
        [PexGeneratedBy(typeof(VobSubParserTest))]
        public void IsPrivateStream1645()
        {
            bool b;
            byte[] bs = new byte[7];
            bs[2] = (byte)1;
            b = this.IsPrivateStream1(bs, 0);
            Assert.AreEqual<bool>(false, b);
        }
    }
}