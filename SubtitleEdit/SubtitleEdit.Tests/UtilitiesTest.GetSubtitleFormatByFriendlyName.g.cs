using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic.SubtitleFormats;
// <copyright file="UtilitiesTest.GetSubtitleFormatByFriendlyName.g.cs" company="Nikse">Nikse</copyright>
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
public void GetSubtitleFormatByFriendlyName836()
{
    SubtitleFormat subtitleFormat;
    subtitleFormat = this.GetSubtitleFormatByFriendlyName((string)null);
    Assert.IsNull((object)subtitleFormat);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void GetSubtitleFormatByFriendlyName827()
{
    SubtitleFormat subtitleFormat;
    subtitleFormat = this.GetSubtitleFormatByFriendlyName("SubRip");
    Assert.IsNotNull((object)subtitleFormat);
    Assert.AreEqual<int>(0, subtitleFormat.ErrorCount);
    Assert.AreEqual<bool>(false, subtitleFormat.BatchMode);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void GetSubtitleFormatByFriendlyName55()
{
    SubtitleFormat subtitleFormat;
    subtitleFormat = this.GetSubtitleFormatByFriendlyName("SubRip (.srt)");
    Assert.IsNotNull((object)subtitleFormat);
    Assert.AreEqual<int>(0, subtitleFormat.ErrorCount);
    Assert.AreEqual<bool>(false, subtitleFormat.BatchMode);
}
    }
}
