using Nikse.SubtitleEdit.Controls;
using Nikse.SubtitleEdit.Logic.SubtitleFormats;
using System.Collections.Generic;
using System.Reflection;
// <copyright file="UtilitiesTest.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic;

namespace Nikse.SubtitleEdit.Logic
{
    using Nikse.SubtitleEdit.Logic.Interfaces;

    [TestClass]
    [PexClass(typeof(Utilities))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UtilitiesTest
    {
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaDirectShow(String)")]
        internal VideoInfo TryReadVideoInfoViaDirectShow(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)(typeof(Utilities).GetMethod("TryReadVideoInfoViaDirectShow",
                                                                          BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                          CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                    .Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            Assert.IsNotNull(result0);
            return result;
        }
        [PexMethod]
        public VideoInfo GetVideoInfo(string fileName)
        {
            VideoInfo result = Utilities.GetVideoInfo(fileName);
            return result;
            // TODO: add assertions to method UtilitiesTest.GetVideoInfo(String)
        }
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaMatroskaHeader(String)")]
        internal VideoInfo TryReadVideoInfoViaMatroskaHeader(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)(typeof(Utilities).GetMethod("TryReadVideoInfoViaMatroskaHeader",
                                                                          BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                          CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                    .Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            return result;
            // TODO: add assertions to method UtilitiesTest.TryReadVideoInfoViaMatroskaHeader(String)
        }
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaAviHeader(String)")]
        internal VideoInfo TryReadVideoInfoViaAviHeader(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)(typeof(Utilities).GetMethod("TryReadVideoInfoViaAviHeader",
                                                                          BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                          CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                    .Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            return result;
            // TODO: add assertions to method UtilitiesTest.TryReadVideoInfoViaAviHeader(String)
        }
        [PexMethod]
        [PexMethodUnderTest("TryReadVideoInfoViaMp4(String)")]
        internal VideoInfo TryReadVideoInfoViaMp4(string fileName)
        {
            object[] args = new object[1];
            args[0] = (object)fileName;
            Type[] parameterTypes = new Type[1];
            parameterTypes[0] = typeof(string);
            VideoInfo result0 = ((MethodBase)(typeof(Utilities).GetMethod("TryReadVideoInfoViaMp4",
                                                                          BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic, (Binder)null,
                                                                          CallingConventions.Standard, parameterTypes, (ParameterModifier[])null)))
                                    .Invoke((object)null, args) as VideoInfo;
            VideoInfo result = result0;
            return result;
            // TODO: add assertions to method UtilitiesTest.TryReadVideoInfoViaMp4(String)
        }
        [PexMethod]
        public List<string> GetMovieFileExtensions()
        {
            List<string> result = Utilities.GetMovieFileExtensions();
            return result;
            // TODO: add assertions to method UtilitiesTest.GetMovieFileExtensions()
        }
        [PexMethod]
        public string GetVideoFileFilter(bool includeAudioFiles)
        {
            string result = Utilities.GetVideoFileFilter(includeAudioFiles);
            return result;
            // TODO: add assertions to method UtilitiesTest.GetVideoFileFilter(Boolean)
        }
        [PexMethod]
        public bool IsInteger(string s)
        {
            bool result = Utilities.IsInteger(s);
            return result;
            // TODO: add assertions to method UtilitiesTest.IsInteger(String)
        }
        [PexMethod]
        public SubtitleFormat GetSubtitleFormatByFriendlyName(string friendlyName)
        {
            SubtitleFormat result = Utilities.GetSubtitleFormatByFriendlyName(friendlyName);
            return result;
            // TODO: add assertions to method UtilitiesTest.GetSubtitleFormatByFriendlyName(String)
        }
        [PexMethod]
        public string FormatBytesToDisplayFileSize(long fileSize)
        {
            string result = Utilities.FormatBytesToDisplayFileSize(fileSize);
            return result;
            // TODO: add assertions to method UtilitiesTest.FormatBytesToDisplayFileSize(Int64)
        }
        [PexMethod]
        public int ShowSubtitle(List<Paragraph> paragraphs, VideoPlayerContainer videoPlayerContainer)
        {
            int result = Utilities.ShowSubtitle(paragraphs, videoPlayerContainer);

            return result;
            // TODO: add assertions to method UtilitiesTest.ShowSubtitle(List`1<Paragraph>, VideoPlayerContainer)
        }
    }
}
