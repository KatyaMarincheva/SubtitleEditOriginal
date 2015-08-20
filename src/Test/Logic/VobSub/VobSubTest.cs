// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubTest.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic.VobSub
{
    using System;
    using System.Drawing;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    /// The vob sub test.
    /// </summary>
    [TestClass]
    public class VobSubTest
    {
        /// <summary>
        /// The vob sub write and read two bitmaps.
        /// </summary>
        [TestMethod]
        public void VobSubWriteAndReadTwoBitmaps()
        {
            string fileName = Guid.NewGuid() + ".sub";
            using (var writer = new VobSubWriter(fileName, 800, 600, 10, 10, 32, Color.White, Color.Black, true, "English", "en"))
            {
                var p1 = new Paragraph("Line1", 0, 1000);
                var p2 = new Paragraph("Line2", 2000, 3000);
                writer.WriteParagraph(p1, new Bitmap(200, 20), ContentAlignment.BottomCenter);
                writer.WriteParagraph(p2, new Bitmap(200, 20), ContentAlignment.BottomCenter);
            }

            var reader = new VobSubParser(true);
            reader.Open(fileName);
            var list = reader.MergeVobSubPacks();

            Assert.IsTrue(list.Count == 2);
        }
    }
}