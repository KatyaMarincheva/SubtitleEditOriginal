// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubWriterTest.WriteColors.g.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub writer test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



// <copyright file="VobSubWriterTest.WriteColors.g.cs" company="Nikse">Nikse</copyright>

// <auto-generated>

// This file contains automatically generated tests.

// Do not modify this file manually.

// If the contents of this file becomes outdated, you can delete it.

// For example, if it no longer compiles.

// </auto-generated>
namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.IO;

    using Microsoft.Pex.Framework.Generated;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The vob sub writer test.
    /// </summary>
    public partial class VobSubWriterTest
    {
        /// <summary>
        /// The write colors 58.
        /// </summary>
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