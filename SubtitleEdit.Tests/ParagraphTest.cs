// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParagraphTest.cs" company="Nikse">
//   Nikse
// </copyright>
// <summary>
//   The paragraph test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The paragraph test.
    /// </summary>
    [TestClass]
    [PexClass(typeof(Paragraph))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ParagraphTest
    {
        /// <summary>
        /// The constructor.
        /// </summary>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        [PexMethod]
        public Paragraph Constructor()
        {
            Paragraph target = new Paragraph();
            return target;

            // TODO: add assertions to method ParagraphTest.Constructor()
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="endTime">
        /// The end time.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        [PexMethod]
        public Paragraph Constructor(TimeCode startTime, TimeCode endTime, string text)
        {
            Paragraph target = new Paragraph(startTime, endTime, text);
            return target;

            // TODO: add assertions to method ParagraphTest.Constructor(TimeCode, TimeCode, String)
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="startFrame">
        /// The start frame.
        /// </param>
        /// <param name="endFrame">
        /// The end frame.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        [PexMethod]
        public Paragraph Constructor(int startFrame, int endFrame, string text)
        {
            Paragraph target = new Paragraph(startFrame, endFrame, text);
            return target;

            // TODO: add assertions to method ParagraphTest.Constructor(Int32, Int32, String)
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="startTotalMilliseconds">
        /// The start total milliseconds.
        /// </param>
        /// <param name="endTotalMilliseconds">
        /// The end total milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        [PexMethod]
        public Paragraph Constructor(string text, double startTotalMilliseconds, double endTotalMilliseconds)
        {
            Paragraph target = new Paragraph(text, startTotalMilliseconds, endTotalMilliseconds);
            return target;

            // TODO: add assertions to method ParagraphTest.Constructor(String, Double, Double)
        }
    }
}