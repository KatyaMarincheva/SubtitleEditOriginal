// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubWriterTest.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub writer test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// <copyright file="VobSubWriterTest.cs" company="Nikse">Nikse</copyright>

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The vob sub writer test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(VobSubWriter))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VobSubWriterTest
    {
        /// <summary>
        /// The to hex color.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("ToHexColor(Color)")]
        internal string ToHexColor(Color c)
        {
            object[] args = new object[1];
            args[0] = (object)c;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(Color);
            string result0 = ((MethodBase)typeof(VobSubWriter).GetMethod("ToHexColor", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as string;
            string result = result0;
            return result;

            // TODO: add assertions to method VobSubWriterTest.ToHexColor(Color)
        }

        /// <summary>
        /// The write colors.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        [PexMethod]
        [PexMethodUnderTest("WriteColors(Stream)")]
        internal void WriteColors(Stream stream)
        {
            object[] args = new object[1];
            args[0] = (object)stream;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(Stream);
            object result = ((MethodBase)typeof(VobSubWriter).GetMethod("WriteColors", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args);

            // TODO: add assertions to method VobSubWriterTest.WriteColors(Stream)
        }

        /// <summary>
        /// The write pixel data address.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="imageTopFieldDataAddress">
        /// The image top field data address.
        /// </param>
        /// <param name="imageBottomFieldDataAddress">
        /// The image bottom field data address.
        /// </param>
        [PexMethod]
        [PexMethodUnderTest("WritePixelDataAddress(Stream, Int32, Int32)")]
        internal void WritePixelDataAddress(Stream stream, int imageTopFieldDataAddress, int imageBottomFieldDataAddress)
        {
            object[] args = new object[3];
            args[0] = (object)stream;
            args[1] = (object)imageTopFieldDataAddress;
            args[2] = (object)imageBottomFieldDataAddress;
            Type[] parameterTypes = new Type[3];
            parameterTypes[0] = typeof(Stream);
            parameterTypes[1] = typeof(int);
            parameterTypes[2] = typeof(int);
            object result = ((MethodBase)typeof(VobSubWriter).GetMethod("WritePixelDataAddress", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args);

            // TODO: add assertions to method VobSubWriterTest.WritePixelDataAddress(Stream, Int32, Int32)
        }

        /// <summary>
        /// The write endian word.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        [PexMethod]
        public void WriteEndianWord(int i, Stream stream)
        {
            VobSubWriter.WriteEndianWord(i, stream);

            // TODO: add assertions to method VobSubWriterTest.WriteEndianWord(Int32, Stream)
        }
    }
}