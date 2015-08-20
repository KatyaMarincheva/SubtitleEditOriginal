// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubPackTest.cs" company="Nikse">
//   Nikse
// </copyright>
// <summary>
//   The vob sub pack test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The vob sub pack test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(VobSubPack))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubPackTest
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="idxLine">
        /// The idx line.
        /// </param>
        /// <returns>
        /// The <see cref="VobSubPack"/>.
        /// </returns>
        [PexMethod]
        public VobSubPack Constructor(byte[] buffer, IdxParagraph idxLine)
        {
            VobSubPack target = new VobSubPack(buffer, idxLine);
            return target;

            // TODO: add assertions to method VobSubPackTest.Constructor(Byte[], IdxParagraph)
        }
    }
}