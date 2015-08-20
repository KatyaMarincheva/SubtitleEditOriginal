// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellCheckOcrTextResult.cs" company="">
//   
// </copyright>
// <summary>
//   The spell check ocr text result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    /// <summary>
    /// The spell check ocr text result.
    /// </summary>
    public class SpellCheckOcrTextResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether fixed.
        /// </summary>
        public bool Fixed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fixed whole line.
        /// </summary>
        public bool FixedWholeLine { get; set; }

        /// <summary>
        /// Gets or sets the word.
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        public string Line { get; set; }
    }
}