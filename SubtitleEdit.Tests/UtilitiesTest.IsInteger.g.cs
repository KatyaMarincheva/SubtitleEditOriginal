using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="UtilitiesTest.IsInteger.g.cs" company="Nikse">Nikse</copyright>
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
public void IsInteger582()
{
    bool b;
    b = this.IsInteger((string)null);
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger766()
{
    bool b;
    b = this.IsInteger("");
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger28()
{
    bool b;
    b = this.IsInteger("\0");
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger667()
{
    bool b;
    b = this.IsInteger("0");
    Assert.AreEqual<bool>(true, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger660()
{
    bool b;
    b = this.IsInteger("0\0");
    Assert.AreEqual<bool>(true, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger989()
{
    bool b;
    b = this.IsInteger("");
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger855()
{
    bool b;
    b = this.IsInteger(":");
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger815()
{
    bool b;
    b = this.IsInteger("-");
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger978()
{
    bool b;
    b = this.IsInteger("-\0");
    Assert.AreEqual<bool>(false, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger450()
{
    bool b;
    b = this.IsInteger("-00");
    Assert.AreEqual<bool>(true, b);
}
[TestMethod]
[PexGeneratedBy(typeof(UtilitiesTest))]
public void IsInteger970()
{
    bool b;
    b = this.IsInteger("-00\0");
    Assert.AreEqual<bool>(true, b);
}
    }
}
