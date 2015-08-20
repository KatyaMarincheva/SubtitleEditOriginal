// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OcrAlphabet.cs" company="">
//   
// </copyright>
// <summary>
//   The ocr alphabet.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System.Collections.Generic;

    /// <summary>
    /// The ocr alphabet.
    /// </summary>
    public class OcrAlphabet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OcrAlphabet"/> class.
        /// </summary>
        public OcrAlphabet()
        {
            this.OcrCharacters = new List<OcrCharacter>();
        }

        /// <summary>
        /// Gets the ocr characters.
        /// </summary>
        public List<OcrCharacter> OcrCharacters { get; private set; }

        /// <summary>
        /// The calculate maximum size.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CalculateMaximumSize()
        {
            int max = 0;
            foreach (OcrCharacter c in this.OcrCharacters)
            {
                foreach (OcrImage img in c.OcrImages)
                {
                    int size = img.Bmp.Width * img.Bmp.Height;
                    if (size > max)
                    {
                        max = size;
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// The get ocr character.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="addIfNotExists">
        /// The add if not exists.
        /// </param>
        /// <returns>
        /// The <see cref="OcrCharacter"/>.
        /// </returns>
        public OcrCharacter GetOcrCharacter(string text, bool addIfNotExists)
        {
            foreach (OcrCharacter ocrCharacter in this.OcrCharacters)
            {
                if (ocrCharacter.Text == text)
                {
                    return ocrCharacter;
                }
            }

            if (addIfNotExists)
            {
                OcrCharacter ch = new OcrCharacter(text);
                this.OcrCharacters.Add(ch);
                return ch;
            }

            return null;
        }
    }
}