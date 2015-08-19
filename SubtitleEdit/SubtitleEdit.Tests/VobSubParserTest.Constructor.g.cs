using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="VobSubParserTest.Constructor.g.cs" company="Nikse">Nikse</copyright>
// <auto-generated>
// This file contains automatically generated tests.
// Do not modify this file manually.
// 
// If the contents of this file becomes outdated, you can delete it.
// For example, if it no longer compiles.
// </auto-generated>
using System;

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    public partial class VobSubParserTest
    {
[TestMethod]
[PexGeneratedBy(typeof(VobSubParserTest))]
public void Constructor124()
{
    VobSubParser vobSubParser;
    vobSubParser = this.Constructor(false);
    Assert.IsNotNull((object)vobSubParser);
    Assert.IsNotNull((object)(vobSubParser.IdxPalette));
    Assert.AreEqual<int>(0, vobSubParser.IdxPalette.Capacity);
    Assert.AreEqual<int>(0, vobSubParser.IdxPalette.Count);
    Assert.IsNotNull((object)(vobSubParser.IdxLanguages));
    Assert.AreEqual<int>(0, vobSubParser.IdxLanguages.Capacity);
    Assert.AreEqual<int>(0, vobSubParser.IdxLanguages.Count);
    Assert.AreEqual<bool>(false, vobSubParser.IsPal);
    Assert.IsNotNull(vobSubParser.VobSubPacks);
    Assert.AreEqual<int>(0, vobSubParser.VobSubPacks.Capacity);
    Assert.AreEqual<int>(0, vobSubParser.VobSubPacks.Count);
}
    }
}
