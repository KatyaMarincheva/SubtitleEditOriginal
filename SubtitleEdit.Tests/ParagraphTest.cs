// <copyright file="ParagraphTest.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikse.SubtitleEdit.Logic;

namespace Nikse.SubtitleEdit.Logic
{
    [TestClass]
    [PexClass(typeof(Paragraph))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ParagraphTest
    {
        [PexMethod]
        public Paragraph Constructor()
        {
            Paragraph target = new Paragraph();
            return target;
            // TODO: add assertions to method ParagraphTest.Constructor()
        }
        [PexMethod]
        public Paragraph Constructor(
            TimeCode startTime,
            TimeCode endTime,
            string text
        )
        {
            Paragraph target = new Paragraph(startTime, endTime, text);
            return target;
            // TODO: add assertions to method ParagraphTest.Constructor(TimeCode, TimeCode, String)
        }
        [PexMethod]
        public Paragraph Constructor(
            int startFrame,
            int endFrame,
            string text
        )
        {
            Paragraph target = new Paragraph(startFrame, endFrame, text);
            return target;
            // TODO: add assertions to method ParagraphTest.Constructor(Int32, Int32, String)
        }
        [PexMethod]
        public Paragraph Constructor(
            string text,
            double startTotalMilliseconds,
            double endTotalMilliseconds
        )
        {
            Paragraph target = new Paragraph(text, startTotalMilliseconds, endTotalMilliseconds);
            return target;
            // TODO: add assertions to method ParagraphTest.Constructor(String, Double, Double)
        }
    }
}
