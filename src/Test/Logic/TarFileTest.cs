// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TarFileTest.cs" company="">
//   
// </copyright>
// <summary>
//   The tar file test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The tar file test.
    /// </summary>
    [TestClass]
    [DeploymentItem("Files")]
    public class TarFileTest
    {
        /// <summary>
        /// The tar file read test.
        /// </summary>
        [TestMethod]
        public void TarFileReadTest()
        {
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), "sample.tar");
            using (var tr = new TarReader(fileName))
            {
                Assert.IsTrue(tr.Files.Count == 13);
                Assert.IsTrue(tr.Files[0].FileSizeInBytes == 629);
                Assert.IsTrue(tr.Files[0].FileName == "BmpReader.cs");
                Assert.IsTrue(tr.Files[12].FileSizeInBytes == 4078);
                Assert.IsTrue(tr.Files[12].FileName == "XSub.cs");
            }
        }
    }
}