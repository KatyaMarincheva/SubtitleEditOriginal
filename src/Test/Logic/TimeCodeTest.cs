// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeCodeTest.cs" company="">
//   
// </copyright>
// <summary>
//   The time code test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Test.Logic
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The time code test.
    /// </summary>
    [TestClass]
    public class TimeCodeTest
    {
        /// <summary>
        /// The time code add time 1.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime1()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(1000);

            Assert.AreEqual(tc.TotalMilliseconds, 2000);
        }

        /// <summary>
        /// The time code add time 2.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime2()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(-1000);

            Assert.AreEqual(tc.TotalMilliseconds, 0);
        }

        /// <summary>
        /// The time code add time 3.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime3()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(0, 0, 0, 1000);

            Assert.AreEqual(tc.TotalMilliseconds, 2000);
        }

        /// <summary>
        /// The time code add time 4.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime4()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(0, 0, 1, 0);

            Assert.AreEqual(tc.TotalMilliseconds, 2000);
        }

        /// <summary>
        /// The time code add time 5.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime5()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(0, 1, 0, 0);

            Assert.AreEqual(tc.TotalMilliseconds, 60000 + 1000);
        }

        /// <summary>
        /// The time code add time 6.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime6()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(TimeSpan.FromMilliseconds(1000));

            Assert.AreEqual(tc.TotalMilliseconds, 2000);
        }

        /// <summary>
        /// The time code add time 7.
        /// </summary>
        [TestMethod]
        public void TimeCodeAddTime7()
        {
            var tc = new TimeCode(1000);
            tc.AddTime(1000.0);

            Assert.AreEqual(tc.TotalMilliseconds, 2000);
        }

        /// <summary>
        /// The time code milliseconds.
        /// </summary>
        [TestMethod]
        public void TimeCodeMilliseconds()
        {
            var tc = new TimeCode(1, 2, 3, 4) { Milliseconds = 9 };

            Assert.AreEqual(tc.TotalMilliseconds, new TimeSpan(0, 1, 2, 3, 9).TotalMilliseconds);
        }

        /// <summary>
        /// The time code seconds 1.
        /// </summary>
        [TestMethod]
        public void TimeCodeSeconds1()
        {
            var tc = new TimeCode(1, 2, 3, 4) { Seconds = 9 };

            Assert.AreEqual(tc.TotalMilliseconds, new TimeSpan(0, 1, 2, 9, 4).TotalMilliseconds);
        }

        /// <summary>
        /// The time code minutes 1.
        /// </summary>
        [TestMethod]
        public void TimeCodeMinutes1()
        {
            var tc = new TimeCode(1, 2, 3, 4) { Minutes = 9 };

            Assert.AreEqual(tc.TotalMilliseconds, new TimeSpan(0, 1, 9, 3, 4).TotalMilliseconds);
        }

        /// <summary>
        /// The time code hours 1.
        /// </summary>
        [TestMethod]
        public void TimeCodeHours1()
        {
            var tc = new TimeCode(1, 2, 3, 4) { Hours = 9 };

            Assert.AreEqual(tc.TotalMilliseconds, new TimeSpan(0, 9, 2, 3, 4).TotalMilliseconds);
        }

        /// <summary>
        /// The time code parse to milliseconds 1.
        /// </summary>
        [TestMethod]
        public void TimeCodeParseToMilliseconds1()
        {
            var ms = TimeCode.ParseToMilliseconds("01:02:03:999");

            Assert.AreEqual(ms, new TimeSpan(0, 1, 2, 3, 999).TotalMilliseconds);
        }

        /// <summary>
        /// The time code get total milliseconds.
        /// </summary>
        [TestMethod]
        public void TimeCodeGetTotalMilliseconds()
        {
            var tc = new TimeCode(1, 2, 3, 4);

            Assert.AreEqual(tc.TotalMilliseconds, 3723004);
            Assert.IsTrue(Math.Abs(tc.TotalMilliseconds - (tc.TotalSeconds * 1000.0)) < 0.001);
        }
    }
}