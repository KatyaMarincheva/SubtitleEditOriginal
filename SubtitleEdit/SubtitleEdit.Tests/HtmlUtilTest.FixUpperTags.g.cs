using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="HtmlUtilTest.FixUpperTags.g.cs" company="Nikse">Nikse</copyright>
// <auto-generated>
// This file contains automatically generated tests.
// Do not modify this file manually.
// 
// If the contents of this file becomes outdated, you can delete it.
// For example, if it no longer compiles.
// </auto-generated>
using System;

namespace Nikse.SubtitleEdit.Core
{
    public partial class HtmlUtilTest
    {
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags400()
{
    string s = this.FixUpperTags("\0");
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags200()
{
    string s = this.FixUpperTags("\0\0");
    Assert.AreEqual<string>("\0\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags457()
{
    string s = this.FixUpperTags("\0<I>>>");
    Assert.AreEqual<string>("\0<i>>>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags569()
{
    string s = this.FixUpperTags("<FONTTTTT");
    Assert.AreEqual<string>("<FONTTTTT", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags299()
{
    string s = this.FixUpperTags("<I><<");
    Assert.AreEqual<string>("<i><<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags718()
{
    string s = this.FixUpperTags("U\0\0UU");
    Assert.AreEqual<string>("U\0\0UU", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixUpperTags13()
{
    string s = this.FixUpperTags((string)null);
    Assert.AreEqual<string>((string)null, s);
}
    }
}
