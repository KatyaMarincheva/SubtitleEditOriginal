using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Pex.Framework.Generated;
// <copyright file="VobSubWriterTest.WriteColors.g.cs" company="Nikse">Nikse</copyright>
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
    public partial class VobSubWriterTest
    {
[TestMethod]
[PexGeneratedBy(typeof(VobSubWriterTest))]
public void WriteColors58()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      MemoryStream memoryStream;
      byte[] bs = new byte[3];
      memoryStream = new MemoryStream(bs, PexSafeHelpers.ByteToBoolean((byte)8));
      disposables.Add((IDisposable)memoryStream);
      this.WriteColors((Stream)memoryStream);
      disposables.Dispose();
    }
}
    }
}
