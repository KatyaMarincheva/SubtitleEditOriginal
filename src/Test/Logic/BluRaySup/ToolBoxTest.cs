// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToolBoxTest.cs" company="">
//   
// </copyright>
// <summary>
//   The tool box test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic.BluRaySup
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic.BluRaySup;

    /// <summary>
    /// The tool box test.
    /// </summary>
    [TestClass]
    public class ToolBoxTest
    {
        /// <summary>
        /// The test zero pts to time string.
        /// </summary>
        [TestMethod]
        public void TestZeroPtsToTimeString()
        {
            Assert.AreEqual("00:00:00.000", ToolBox.PtsToTimeString(0));
        }
    }
}