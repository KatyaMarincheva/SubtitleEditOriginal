using System.Reflection;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="HtmlUtilTest.IfTwoBeginTagsConvertSecondToEndTag.g.cs" company="Nikse">Nikse</copyright>
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
public void IfTwoBeginTagsConvertSecondToEndTag691()
{
    string s = this.IfTwoBeginTagsConvertSecondToEndTag
            ("", 0, 0, BeginTag, 0, EndTag);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void IfTwoBeginTagsConvertSecondToEndTag513()
{
    string s = this.IfTwoBeginTagsConvertSecondToEndTag("", 0, 0, BeginTag, 0, EndTag);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(HtmlUtilTest))]
public void IfTwoBeginTagsConvertSecondToEndTag19()
{
    string s = this.IfTwoBeginTagsConvertSecondToEndTag
            ((string)null, 0, 0, BeginTag, 0, EndTag);
    Assert.AreEqual<string>((string)null, s);
}
    }
}
