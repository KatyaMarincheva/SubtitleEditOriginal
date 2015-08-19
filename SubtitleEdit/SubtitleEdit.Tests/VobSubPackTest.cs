// <copyright file="VobSubPackTest.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic.VobSub;

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    [TestClass]
    [PexClass(typeof(VobSubPack))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubPackTest
    {
        [PexMethod]
        public VobSubPack Constructor(byte[] buffer, IdxParagraph idxLine)
        {
            VobSubPack target = new VobSubPack(buffer, idxLine);
            return target;
            // TODO: add assertions to method VobSubPackTest.Constructor(Byte[], IdxParagraph)
        }
    }
}
