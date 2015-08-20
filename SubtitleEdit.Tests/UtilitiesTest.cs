// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilitiesTest.cs" company="">
//   
// </copyright>
// <summary>
//   The utilities test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// <copyright file="UtilitiesTest.cs" company="Nikse">Nikse</copyright>

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Controls;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The utilities test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(Utilities))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UtilitiesTest
    {
        /// <summary>
        /// The try read video info via direct show.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaDirectShow(String)")]
        internal VideoInfo TryReadVideoInfoViaDirectShow(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)typeof(Utilities).GetMethod("TryReadVideoInfoViaDirectShow", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            Assert.IsNotNull(result0);
            return result;
        }

        /// <summary>
        /// The get video info.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        [PexMethod]
        public VideoInfo GetVideoInfo(string fileName)
        {
            VideoInfo result = Utilities.GetVideoInfo(fileName);
            return result;

            // TODO: add assertions to method UtilitiesTest.GetVideoInfo(String)
        }

        /// <summary>
        /// The try read video info via matroska header.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaMatroskaHeader(String)")]
        internal VideoInfo TryReadVideoInfoViaMatroskaHeader(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)typeof(Utilities).GetMethod("TryReadVideoInfoViaMatroskaHeader", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            return result;

            // TODO: add assertions to method UtilitiesTest.TryReadVideoInfoViaMatroskaHeader(String)
        }

        /// <summary>
        /// The try read video info via avi header.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaAviHeader(String)")]
        internal VideoInfo TryReadVideoInfoViaAviHeader(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)typeof(Utilities).GetMethod("TryReadVideoInfoViaAviHeader", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            return result;

            // TODO: add assertions to method UtilitiesTest.TryReadVideoInfoViaAviHeader(String)
        }

        /// <summary>
        /// The try read video info via mp 4.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaMp4(String)")]
        internal VideoInfo TryReadVideoInfoViaMp4(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)typeof(Utilities).GetMethod("TryReadVideoInfoViaMp4", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null, CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)).Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            return result;

            // TODO: add assertions to method UtilitiesTest.TryReadVideoInfoViaMp4(String)
        }

        /// <summary>
        /// The get movie file extensions.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [PexMethod]
        public List<string> GetMovieFileExtensions()
        {
            List<string> result = Utilities.GetMovieFileExtensions();
            return result;

            // TODO: add assertions to method UtilitiesTest.GetMovieFileExtensions()
        }

        /// <summary>
        /// The get video file filter.
        /// </summary>
        /// <param name="includeAudioFiles">
        /// The include audio files.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        public string GetVideoFileFilter(bool includeAudioFiles)
        {
            string result = Utilities.GetVideoFileFilter(includeAudioFiles);
            return result;

            // TODO: add assertions to method UtilitiesTest.GetVideoFileFilter(Boolean)
        }

        /// <summary>
        /// The is integer.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [PexMethod]
        public bool IsInteger(string s)
        {
            bool result = Utilities.IsInteger(s);
            return result;

            // TODO: add assertions to method UtilitiesTest.IsInteger(String)
        }

        /// <summary>
        /// The get subtitle format by friendly name.
        /// </summary>
        /// <param name="friendlyName">
        /// The friendly name.
        /// </param>
        /// <returns>
        /// The <see cref="SubtitleFormat"/>.
        /// </returns>
        [PexMethod]
        public SubtitleFormat GetSubtitleFormatByFriendlyName(string friendlyName)
        {
            SubtitleFormat result = Utilities.GetSubtitleFormatByFriendlyName(friendlyName);
            return result;

            // TODO: add assertions to method UtilitiesTest.GetSubtitleFormatByFriendlyName(String)
        }

        /// <summary>
        /// The format bytes to display file size.
        /// </summary>
        /// <param name="fileSize">
        /// The file size.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [PexMethod]
        public string FormatBytesToDisplayFileSize(long fileSize)
        {
            string result = Utilities.FormatBytesToDisplayFileSize(fileSize);
            return result;

            // TODO: add assertions to method UtilitiesTest.FormatBytesToDisplayFileSize(Int64)
        }

        /// <summary>
        /// The show subtitle.
        /// </summary>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        /// <param name="videoPlayerContainer">
        /// The video player container.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [PexMethod]
        public int ShowSubtitle(List<Paragraph> paragraphs, VideoPlayerContainer videoPlayerContainer)
        {
            int result = Utilities.ShowSubtitle(paragraphs, videoPlayerContainer);

            return result;

            // TODO: add assertions to method UtilitiesTest.ShowSubtitle(List`1<Paragraph>, VideoPlayerContainer)
        }
    }
}