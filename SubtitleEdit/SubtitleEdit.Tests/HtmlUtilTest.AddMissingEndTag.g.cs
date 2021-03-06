using System.Reflection;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="HtmlUtilTest.AddMissingEndTag.g.cs" company="Nikse">Nikse</copyright>
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
public void AddMissingEndTag162()
{
    string s = this.AddMissingEndTag("", 0, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
[PexRaisedException(typeof(TargetInvocationException))]
public void AddMissingEndTagThrowsTargetInvocationException206()
{
    string s = this.AddMissingEndTag("", 1, 0, BeginTag, EndTag, 0);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag106()
{
    string s = this.AddMissingEndTag("", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag313()
{
    string s = this.AddMissingEndTag("", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag579()
{
    string s = this.AddMissingEndTag("", 1, 0, BeginTag, EndTag, 2);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag831()
{
    string s = this.AddMissingEndTag("", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag155()
{
    string s = this.AddMissingEndTag("", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
[PexRaisedException(typeof(TargetInvocationException))]
public void AddMissingEndTagThrowsTargetInvocationException886()
{
    string s = this.AddMissingEndTag("\0", 1, 0, BeginTag, EndTag, 0);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag11001()
{
    string s = this.AddMissingEndTag("\0", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag778()
{
    string s = this.AddMissingEndTag("\0", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag240()
{
    string s = this.AddMissingEndTag("\0", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag518()
{
    string s = this.AddMissingEndTag("\0", 1, 0, BeginTag, EndTag, 2);
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
[PexRaisedException(typeof(TargetInvocationException))]
public void AddMissingEndTagThrowsTargetInvocationException681()
{
    string s = this.AddMissingEndTag("\0\0", 1, 0, BeginTag, EndTag, 0);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag491()
{
    string s = this.AddMissingEndTag("\0\0", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("\0\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag273()
{
    string s = this.AddMissingEndTag("\0\0", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("\0\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag444()
{
    string s = this.AddMissingEndTag("\0\0", 1, 0, BeginTag, EndTag, 2);
    Assert.AreEqual<string>("\0\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag895()
{
    string s = this.AddMissingEndTag("\0\0", 1, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>("\0\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag183()
{
    string s = this.AddMissingEndTag((string)null, 0, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>((string)null, s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void AddMissingEndTag110()
{
    string s = this.AddMissingEndTag((string)null, 0, 0, BeginTag, EndTag, 0);
    Assert.AreEqual<string>((string)null, s);
}
    }
}
