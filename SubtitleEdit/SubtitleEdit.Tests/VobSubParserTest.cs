using System.Reflection;
using System.Collections.Generic;
using System.IO;
// <copyright file="VobSubParserTest.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic.VobSub;

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    [TestClass]
    [PexClass(typeof(VobSubParser))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubParserTest
    {
        [PexMethod]
        public VobSubParser Constructor(bool isPal)
        {
            VobSubParser target = new VobSubParser(isPal);
            return target;
            // TODO: add assertions to method VobSubParserTest.Constructor(Boolean)
        }
        [PexMethod]
        public void Open([PexAssumeUnderTest]VobSubParser target, Stream ms)
        {
            target.Open(ms);
            // TODO: add assertions to method VobSubParserTest.Open(VobSubParser, Stream)
        }
        [PexMethod]
        public List<VobSubMergedPack> MergeVobSubPacks([PexAssumeUnderTest]VobSubParser target)
        {
            List<VobSubMergedPack> result = target.MergeVobSubPacks();
            return result;
            // TODO: add assertions to method VobSubParserTest.MergeVobSubPacks(VobSubParser)
        }
        [PexMethod]
        [PexMethodUnderTest("IsMpeg2PackHeader(Byte[])")]
        internal bool IsMpeg2PackHeader(byte[] buffer)
        {
            object[] args = new object[1];
            args[0] = (object)buffer;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(byte).MakeArrayType();
            bool result0 = (bool)(((MethodBase)(typeof(VobSubParser).GetMethod("IsMpeg2PackHeader",
                                                                               BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                               CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                      .Invoke((object)null, args));
            bool result = result0;
            return result;
            // TODO: add assertions to method VobSubParserTest.IsMpeg2PackHeader(Byte[])
        }
        [PexMethod]
        [PexMethodUnderTest("IsPrivateStream1(Byte[], Int32)")]
        internal bool IsPrivateStream1(byte[] buffer, int index)
        {
            object[] args = new object[2];
            args[0] = (object)buffer;
            args[1] = (object)index;
            Type[] parameterTypes = new Type[2];
            parameterTypes[0] = typeof(byte).MakeArrayType();
            parameterTypes[1] = typeof(int);
            bool result0 = (bool)(((MethodBase)(typeof(VobSubParser).GetMethod("IsPrivateStream1",
                                                                               BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                               CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                      .Invoke((object)null, args));
            bool result = result0;
            return result;
            // TODO: add assertions to method VobSubParserTest.IsPrivateStream1(Byte[], Int32)
        }
        [PexMethod]
        [PexMethodUnderTest("IsPrivateStream2(Byte[], Int32)")]
        internal bool IsPrivateStream2(byte[] buffer, int index)
        {
            object[] args = new object[2];
            args[0] = (object)buffer;
            args[1] = (object)index;
            Type[] parameterTypes = new Type[2];
            parameterTypes[0] = typeof(byte).MakeArrayType();
            parameterTypes[1] = typeof(int);
            bool result0 = (bool)(((MethodBase)(typeof(VobSubParser).GetMethod("IsPrivateStream2",
                                                                               BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                               CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                      .Invoke((object)null, args));
            bool result = result0;
            return result;
            // TODO: add assertions to method VobSubParserTest.IsPrivateStream2(Byte[], Int32)
        }
        [PexMethod]
        [PexMethodUnderTest("IsSubtitlePack(Byte[])")]
        internal bool IsSubtitlePack(byte[] buffer)
        {
            object[] args = new object[1];
            args[0] = (object)buffer;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(byte).MakeArrayType();
            bool result0 = (bool)(((MethodBase)(typeof(VobSubParser).GetMethod("IsSubtitlePack",
                                                                               BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                               CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                      .Invoke((object)null, args));
            bool result = result0;
            return result;
            // TODO: add assertions to method VobSubParserTest.IsSubtitlePack(Byte[])
        }
    }
}
