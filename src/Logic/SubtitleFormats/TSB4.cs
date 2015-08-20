// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TSB4.cs" company="">
//   
// </copyright>
// <summary>
//   The ts b 4.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The ts b 4.
    /// </summary>
    public class TSB4 : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".sub";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TSB4";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is time based.
        /// </summary>
        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The is mine.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsMine(List<string> lines, string fileName)
        {
            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
        }

        /// <summary>
        /// The to text.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToText(Subtitle subtitle, string title)
        {
            return "Not supported!";
        }

        /// <summary>
        /// The load subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            byte[] array;
            try
            {
                array = FileUtil.ReadAllBytesShared(fileName);
            }
            catch
            {
                this._errorCount++;
                return;
            }

            if (array.Length < 100)
            {
                return;
            }

            if (array[0] != 84 || array[1] != 83 || array[2] != 66 || array[3] != 52)
            {
                return;
            }

            for (int i = 0; i < array.Length - 20; i++)
            {
                if (array[i] == 84 && array[i + 1] == 73 && array[i + 2] == 84 && array[i + 3] == 76 && array[i + 8] == 84 && array[i + 9] == 73 && array[i + 10] == 77 && array[i + 11] == 69)
                {
                    // TITL + TIME
                    int endOfText = array[i + 4];

                    int start = array[i + 16] + array[i + 17] * 256;
                    if (array[i + 18] != 32)
                    {
                        start += array[i + 18] * 256 * 256;
                    }

                    int end = array[i + 20] + array[i + 21] * 256;
                    if (array[i + 22] != 32)
                    {
                        end += array[i + 22] * 256 * 256;
                    }

                    int textStart = i;
                    while (textStart < i + endOfText && !(array[textStart] == 0x4C && array[textStart + 1] == 0x49 && array[textStart + 2] == 0x4E && array[textStart + 3] == 0x45))
                    {
                        // LINE
                        textStart++;
                    }

                    int length = i + endOfText - textStart - 2;
                    textStart += 8;

                    string text = Encoding.Default.GetString(array, textStart, length);

                    // text = Encoding.Default.GetString(array, i + 53, endOfText - 47);
                    text = text.Trim('\0').Replace("\0", " ").Trim();
                    Paragraph item = new Paragraph(text, FramesToMilliseconds(start), FramesToMilliseconds(end));
                    subtitle.Paragraphs.Add(item);
                    i += endOfText + 5;
                }
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }
    }
}