using System.Reflection;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="HtmlUtilTest.SwapEndAndBeginTags.g.cs" company="Nikse">Nikse</copyright>
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
public void SwapEndAndBeginTags506()
{
    string s;
    s = this.SwapEndAndBeginTags("", 0, 0, BeginTag, EndTag);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
[PexRaisedException(typeof(TargetInvocationException))]
public void SwapEndAndBeginTagsThrowsTargetInvocationException748()
{
    string s;
    s = this.SwapEndAndBeginTags("", 1, 1, BeginTag, EndTag);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
[PexRaisedException(typeof(TargetInvocationException))]
public void SwapEndAndBeginTagsThrowsTargetInvocationException526()
{
    string s;
    s = this.SwapEndAndBeginTags("\0", 1, 1, BeginTag, EndTag);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void SwapEndAndBeginTags263()
{
    string s;
    s = this.SwapEndAndBeginTags("\0", 1, 1, BeginTag, EndTag);
    Assert.AreEqual<string>("\0", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void SwapEndAndBeginTags553()
{
    string s;
    s = this.SwapEndAndBeginTags
            ("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0", 1, 
             1, BeginTag, EndTag);
    Assert.AreEqual<string>
        ("\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0", 
         s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void SwapEndAndBeginTags961()
{
    string s;
    s = this.SwapEndAndBeginTags((string)null, 0, 0, BeginTag, EndTag);
    Assert.AreEqual<string>((string)null, s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void SwapEndAndBeginTags20()
{
    string s;
    s = this.SwapEndAndBeginTags((string)null, 1, 0, BeginTag, EndTag);
    Assert.AreEqual<string>((string)null, s);
}
    }
}
