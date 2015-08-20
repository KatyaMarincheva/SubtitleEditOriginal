// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatroskaTest.cs" company="">
//   
// </copyright>
// <summary>
//   The matroska test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic.VideoFormats
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska;

    /// <summary>
    /// The matroska test.
    /// </summary>
    [TestClass]
    [DeploymentItem("Files")]
    public class MatroskaTest
    {
        /// <summary>
        /// The matroska test valid.
        /// </summary>
        [TestMethod]
        public void MatroskaTestValid()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MKV_SRT.mkv");
            using (var parser = new MatroskaFile(fileName))
            {
                Assert.IsTrue(parser.IsValid);
            }
        }

        /// <summary>
        /// The matroska test invalid.
        /// </summary>
        [TestMethod]
        public void MatroskaTestInvalid()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_TS_with_graphics.ts");
            using (var parser = new MatroskaFile(fileName))
            {
                Assert.IsFalse(parser.IsValid);
            }
        }

        /// <summary>
        /// The matroska test is srt.
        /// </summary>
        [TestMethod]
        public void MatroskaTestIsSrt()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MKV_SRT.mkv");
            using (var parser = new MatroskaFile(fileName))
            {
                var tracks = parser.GetTracks(true);
                Assert.IsTrue(tracks[0].CodecId == "S_TEXT/UTF8");
            }
        }

        /// <summary>
        /// The matroska test srt content.
        /// </summary>
        [TestMethod]
        public void MatroskaTestSrtContent()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MKV_SRT.mkv");
            using (var parser = new MatroskaFile(fileName))
            {
                var tracks = parser.GetTracks(true);
                var subtitles = parser.GetSubtitle(Convert.ToInt32(tracks[0].TrackNumber), null);
                Assert.IsTrue(subtitles.Count == 2);
                Assert.IsTrue(subtitles[0].Text == "Line 1");
                Assert.IsTrue(subtitles[1].Text == "Line 2");
            }
        }

        /// <summary>
        /// The matroska test vob sub pgs.
        /// </summary>
        [TestMethod]
        public void MatroskaTestVobSubPgs()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MKV_VobSub_PGS.mkv");
            using (var parser = new MatroskaFile(fileName))
            {
                var tracks = parser.GetTracks(true);
                Assert.IsTrue(tracks[0].CodecId == "S_VOBSUB");
                Assert.IsTrue(tracks[1].CodecId == "S_HDMV/PGS");
            }
        }

        /// <summary>
        /// The matroska test vob sub pgs content.
        /// </summary>
        [TestMethod]
        public void MatroskaTestVobSubPgsContent()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MKV_VobSub_PGS.mkv");
            using (var parser = new MatroskaFile(fileName))
            {
                var tracks = parser.GetTracks(true);
                var subtitles = parser.GetSubtitle(Convert.ToInt32(tracks[0].TrackNumber), null);
                Assert.IsTrue(subtitles.Count == 2);

                // TODO: Check bitmaps

                // subtitles = parser.GetSubtitle(Convert.ToInt32(tracks[1].TrackNumber), null);
                // Assert.IsTrue(subtitles.Count == 2);
                // check bitmaps
            }
        }

        /// <summary>
        /// The matroska test delayed 500 ms.
        /// </summary>
        [TestMethod]
        public void MatroskaTestDelayed500Ms()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MKV_delayed.mkv");
            using (var parser = new MatroskaFile(fileName))
            {
                var delay = parser.GetTrackStartTime(parser.GetTracks()[0].TrackNumber);
                Assert.IsTrue(delay == 500);
            }
        }
    }
}