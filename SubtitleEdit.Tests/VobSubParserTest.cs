// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubParserTest.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub parser test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// <copyright file="VobSubParserTest.cs" company="Nikse">Nikse</copyright>

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The vob sub parser test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(VobSubParser))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubParserTest
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="isPal">
        /// The is pal.
        /// </param>
        /// <returns>
        /// The <see cref="VobSubParser"/>.
        /// </returns>
        [PexMethod]
        public VobSubParser Constructor(bool isPal)
        {
            VobSubParser target = new VobSubParser(isPal);
            return target;

            // TODO: add assertions to method VobSubParserTest.Constructor(Boolean)
        }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="ms">
        /// The ms.
        /// </param>
        [PexMethod]
        public void Open([PexAssumeUnderTest] VobSubParser target, Stream ms)
        {
            target.Open(ms);

            // TODO: add assertions to method VobSubParserTest.Open(VobSubParser, Stream)
        }

        /// <summary>
        /// The merge vob sub packs.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [PexMethod]
        public List<VobSubMergedPack> MergeVobSubPacks([PexAssumeUnderTest] VobSubParser target)
        {
            List<VobSubMergedPack> result = target.MergeVobSubPacks();
            return result;

            // TODO: add assertions to method VobSubParserTest.MergeVobSubPacks(VobSubParser)
        }

        /// <summary>
        /// The is mpeg 2 pack header.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("IsMpeg2PackHeader(Byte[])")]
        internal bool IsMpeg2PackHeader(byte[] buffer)
        {
            object[] args = new object[1];
            args[0] = (object)buffer;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(byte).MakeArrayType();
            bool result0 = (bool)((MethodBase)(typeof(VobSubParser).GetMethod("IsMpeg2PackHeader", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null))).Invoke((object)null, args);
            bool result = result0;
            return result;

            // TODO: add assertions to method VobSubParserTest.IsMpeg2PackHeader(Byte[])
        }

        /// <summary>
        /// The is private stream 1.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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
            bool result0 = (bool)((MethodBase)(typeof(VobSubParser).GetMethod("IsPrivateStream1", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null))).Invoke((object)null, args);
            bool result = result0;
            return result;

            // TODO: add assertions to method VobSubParserTest.IsPrivateStream1(Byte[], Int32)
        }

        /// <summary>
        /// The is private stream 2.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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
            bool result0 = (bool)((MethodBase)(typeof(VobSubParser).GetMethod("IsPrivateStream2", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null))).Invoke((object)null, args);
            bool result = result0;
            return result;

            // TODO: add assertions to method VobSubParserTest.IsPrivateStream2(Byte[], Int32)
        }

        /// <summary>
        /// The is subtitle pack.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("IsSubtitlePack(Byte[])")]
        internal bool IsSubtitlePack(byte[] buffer)
        {
            object[] args = new object[1];
            args[0] = (object)buffer;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(byte).MakeArrayType();
            bool result0 = (bool)((MethodBase)(typeof(VobSubParser).GetMethod("IsSubtitlePack", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null))).Invoke((object)null, args);
            bool result = result0;
            return result;

            // TODO: add assertions to method VobSubParserTest.IsSubtitlePack(Byte[])
        }
    }
}