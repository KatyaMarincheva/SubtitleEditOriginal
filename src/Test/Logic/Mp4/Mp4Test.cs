// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mp4Test.cs" company="">
//   
// </copyright>
// <summary>
//   The mp 4 test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic.Mp4
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4;

    /// <summary>
    /// The mp 4 test.
    /// </summary>
    [TestClass]
    [DeploymentItem("Files")]
    public class Mp4Test
    {
        /// <summary>
        /// The mp 4 test 1.
        /// </summary>
        [TestMethod]
        public void Mp4Test1()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample_MP4_SRT.mp4");
            var parser = new MP4Parser(fileName);

            var tracks = parser.GetSubtitleTracks();

            Assert.IsTrue(tracks.Count == 1);
            Assert.IsTrue(tracks[0].Mdia.Minf.Stbl.EndTimeCodes.Count == 2);
        }
    }
}