// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle16.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 16.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The unknown subtitle 16.
    /// </summary>
    public class UnknownSubtitle16 : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".cip";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 16";
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
            UnknownSubtitle52 u52 = new UnknownSubtitle52();
            using (RichTextBox rtBox = new RichTextBox { Text = u52.ToText(subtitle, title) })
            {
                return rtBox.Rtf;
            }
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

            if (lines.Count == 0 || !lines[0].TrimStart().StartsWith("{\\rtf1"))
            {
                return;
            }

            // load as text via RichTextBox
            StringBuilder text = new StringBuilder();
            foreach (string s in lines)
            {
                text.AppendLine(s);
            }

            using (RichTextBox rtBox = new RichTextBox())
            {
                rtBox.Rtf = text.ToString();
                List<string> lines2 = new List<string>();
                foreach (string line in rtBox.Lines)
                {
                    lines2.Add(line);
                }

                UnknownSubtitle52 u52 = new UnknownSubtitle52();
                u52.LoadSubtitle(subtitle, lines2, fileName);
                this._errorCount = u52.ErrorCount;
            }
        }
    }
}