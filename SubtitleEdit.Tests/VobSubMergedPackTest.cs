// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubMergedPackTest.cs" company="Nikse">
//   Nikse
// </copyright>
// <summary>
//   The vob sub merged pack test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The vob sub merged pack test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(VobSubMergedPack))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubMergedPackTest
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="subPictureData">
        /// The sub picture data.
        /// </param>
        /// <param name="presentationTimestamp">
        /// The presentation timestamp.
        /// </param>
        /// <param name="streamId">
        /// The stream id.
        /// </param>
        /// <param name="idxLine">
        /// The idx line.
        /// </param>
        /// <returns>
        /// The <see cref="VobSubMergedPack"/>.
        /// </returns>
        [PexMethod]
        public VobSubMergedPack Constructor(byte[] subPictureData, TimeSpan presentationTimestamp, int streamId, IdxParagraph idxLine)
        {
            VobSubMergedPack target = new VobSubMergedPack(subPictureData, presentationTimestamp, streamId, idxLine);
            return target;

            // TODO: add assertions to method VobSubMergedPackTest.Constructor(Byte[], TimeSpan, Int32, IdxParagraph)
        }
    }
}