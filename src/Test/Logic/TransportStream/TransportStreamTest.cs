// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportStreamTest.cs" company="">
//   
// </copyright>
// <summary>
//   The transport stream test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic.TransportStream
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The transport stream test.
    /// </summary>
    [TestClass]
    [DeploymentItem("Files")]
    public class TransportStreamTest
    {
        /// <summary>
        /// The transport stream test 1.
        /// </summary>
        [TestMethod]
        public void TransportStreamTest1()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_TS_with_graphics.ts");
            var parser = new Nikse.SubtitleEdit.Logic.TransportStream.TransportStreamParser();
            parser.Parse(fileName, null);
            var subtitles = parser.GetDvbSubtitles(41);

            Assert.IsTrue(subtitles.Count == 10);
            using (var bmp = subtitles[0].Pes.GetImageFull())
            {
                Assert.IsTrue(bmp.Width == 719);
                Assert.IsTrue(bmp.Height == 575);
            }
        }
    }
}