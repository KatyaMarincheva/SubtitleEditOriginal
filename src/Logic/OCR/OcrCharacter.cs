// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrCharacter.cs" company="">
//   
// </copyright>
// <summary>
//   The ocr character.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System.Collections.Generic;

    /// <summary>
    /// The ocr character.
    /// </summary>
    public class OcrCharacter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OcrCharacter"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public OcrCharacter(string text)
        {
            this.Text = text;
            this.OcrImages = new List<OcrImage>();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the ocr images.
        /// </summary>
        public List<OcrImage> OcrImages { get; set; }
    }
}