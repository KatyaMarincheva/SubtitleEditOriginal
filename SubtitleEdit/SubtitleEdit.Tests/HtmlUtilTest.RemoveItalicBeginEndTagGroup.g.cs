using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="HtmlUtilTest.RemoveItalicBeginEndTagGroup.g.cs" company="Nikse">Nikse</copyright>
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
public void RemoveItalicBeginEndTagGroup232()
{
    string s = this.RemoveItalicBeginEndTagGroup("");
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void RemoveItalicBeginEndTagGroup772()
{
    string s = this.RemoveItalicBeginEndTagGroup("<i>      ");
    Assert.AreEqual<string>("<i>      ", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void RemoveItalicBeginEndTagGroup481()
{
    string s = this.RemoveItalicBeginEndTagGroup("<i> ");
    Assert.AreEqual<string>("<i> ", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void RemoveItalicBeginEndTagGroup592()
{
    string s = this.RemoveItalicBeginEndTagGroup("<i>");
    Assert.AreEqual<string>("<i>", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void RemoveItalicBeginEndTagGroup792()
{
    string s = this.RemoveItalicBeginEndTagGroup("<i>\0");
    Assert.AreEqual<string>("<i>\0", s);
}
    }
}
