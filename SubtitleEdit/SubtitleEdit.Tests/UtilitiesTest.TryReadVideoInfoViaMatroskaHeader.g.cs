using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="UtilitiesTest.TryReadVideoInfoViaMatroskaHeader.g.cs" company="Nikse">Nikse</copyright>
// <auto-generated>
// This file contains automatically generated tests.
// Do not modify this file manually.
// 
// If the contents of this file becomes outdated, you can delete it.
// For example, if it no longer compiles.
// </auto-generated>
using System;

namespace Nikse.SubtitleEdit.Logic
{
    public partial class UtilitiesTest
    {
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void TryReadVideoInfoViaMatroskaHeader676()
{
    VideoInfo videoInfo;
    videoInfo = this.TryReadVideoInfoViaMatroskaHeader((string)null);
    Assert.IsNotNull((object)videoInfo);
    Assert.AreEqual<int>(0, videoInfo.Width);
    Assert.AreEqual<int>(0, videoInfo.Height);
    Assert.AreEqual<double>(0, videoInfo.TotalMilliseconds);
    Assert.AreEqual<double>(0, videoInfo.TotalSeconds);
    Assert.AreEqual<double>(0, videoInfo.FramesPerSecond);
    Assert.AreEqual<double>(0, videoInfo.TotalFrames);
    Assert.AreEqual<string>((string)null, videoInfo.VideoCodec);
    Assert.AreEqual<string>((string)null, videoInfo.FileType);
    Assert.AreEqual<bool>(false, videoInfo.Success);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void TryReadVideoInfoViaMatroskaHeader242()
{
    VideoInfo videoInfo;
    videoInfo = this.TryReadVideoInfoViaMatroskaHeader("");
    Assert.IsNotNull((object)videoInfo);
    Assert.AreEqual<int>(0, videoInfo.Width);
    Assert.AreEqual<int>(0, videoInfo.Height);
    Assert.AreEqual<double>(0, videoInfo.TotalMilliseconds);
    Assert.AreEqual<double>(0, videoInfo.TotalSeconds);
    Assert.AreEqual<double>(0, videoInfo.FramesPerSecond);
    Assert.AreEqual<double>(0, videoInfo.TotalFrames);
    Assert.AreEqual<string>((string)null, videoInfo.VideoCodec);
    Assert.AreEqual<string>((string)null, videoInfo.FileType);
    Assert.AreEqual<bool>(false, videoInfo.Success);
}
    }
}
