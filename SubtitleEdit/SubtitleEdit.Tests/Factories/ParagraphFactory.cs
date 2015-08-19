using Nikse.SubtitleEdit.Logic;
// <copyright file="ParagraphFactory.cs" company="Nikse">Nikse</copyright>

using System;
using Microsoft.Pex.Framework;

namespace Nikse.SubtitleEdit.Logic
{
    /// <summary>A factory for Nikse.SubtitleEdit.Logic.Paragraph instances</summary>
    public static partial class ParagraphFactory
    {
        /// <summary>A factory for Nikse.SubtitleEdit.Logic.Paragraph instances</summary>
        [PexFactoryMethod(typeof(Paragraph))]
        public static Paragraph Create(Paragraph paragraph_paragraph1)
        {
            Paragraph paragraph = new Paragraph(paragraph_paragraph1);
            return paragraph;

            // TODO: Edit factory method of Paragraph
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
