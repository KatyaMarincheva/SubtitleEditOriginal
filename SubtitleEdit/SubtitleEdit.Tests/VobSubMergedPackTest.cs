// <copyright file="VobSubMergedPackTest.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic.VobSub;

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    [TestClass]
    [PexClass(typeof(VobSubMergedPack))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubMergedPackTest
    {
        [PexMethod]
        public VobSubMergedPack Constructor(
            byte[] subPictureData,
            TimeSpan presentationTimestamp,
            int streamId,
            IdxParagraph idxLine
        )
        {
            VobSubMergedPack target
               = new VobSubMergedPack(subPictureData, presentationTimestamp, streamId, idxLine);
            return target;
            // TODO: add assertions to method VobSubMergedPackTest.Constructor(Byte[], TimeSpan, Int32, IdxParagraph)
        }
    }
}
