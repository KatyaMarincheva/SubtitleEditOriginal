using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="HtmlUtilTest.FixInvalidItalicTags.g.cs" company="Nikse">Nikse</copyright>
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
public void FixInvalidItalicTags84()
{
    string s = this.FixInvalidItalicTags("﻿<");
    Assert.AreEqual<string>("﻿<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags504()
{
    string s = this.FixInvalidItalicTags(" ");
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags232()
{
    string s = this.FixInvalidItalicTags("");
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags400()
{
    string s = this.FixInvalidItalicTags("\0");
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags549()
{
    string s = this.FixInvalidItalicTags("<");
    Assert.AreEqual<string>("<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags432()
{
    string s = this.FixInvalidItalicTags("<\n<");
    Assert.AreEqual<string>("<\n<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags186()
{
    string s = this.FixInvalidItalicTags("<\n6<<<");
    Assert.AreEqual<string>("<\n6<<<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags146()
{
    string s = this.FixInvalidItalicTags("<\t \t<");
    Assert.AreEqual<string>("<\t \t<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags735()
{
    string s = this.FixInvalidItalicTags("<Ā");
    Assert.AreEqual<string>("<Ā", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags442()
{
    string s = this.FixInvalidItalicTags("<i\t\t");
    Assert.AreEqual<string>("<i", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags283()
{
    string s = this.FixInvalidItalicTags("<i>");
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags911()
{
    string s = this.FixInvalidItalicTags("<i>\t");
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags412()
{
    string s = this.FixInvalidItalicTags("<i><  ");
    Assert.AreEqual<string>("<i><  </i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags647()
{
    string s = this.FixInvalidItalicTags("<i><");
    Assert.AreEqual<string>("<i><</i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags408()
{
    string s = this.FixInvalidItalicTags("<i><\n\nĀ");
    Assert.AreEqual<string>("<i><\n\nĀ</i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags994()
{
    string s = this.FixInvalidItalicTags("<i><\t\t");
    Assert.AreEqual<string>("<i><</i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags575()
{
    string s = this.FixInvalidItalicTags("<i><i>");
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags684()
{
    string s = this.FixInvalidItalicTags("<i><i>b");
    Assert.AreEqual<string>("<i>b</i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags938()
{
    string s = this.FixInvalidItalicTags("<i><");
    Assert.AreEqual<string>("<i><</i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags137()
{
    string s = this.FixInvalidItalicTags("<Ā<");
    Assert.AreEqual<string>("<Ā<", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void FixInvalidItalicTags678()
{
    string s = this.FixInvalidItalicTags("Ȁ53\0\0");
    Assert.AreEqual<string>("Ȁ53\0\0", s);
}
    }
}
