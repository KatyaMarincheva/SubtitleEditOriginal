// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle3.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 3.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // Subtitle number: 1
    // Start time (or frames): 00:00:48,862:0000001222
    // End time (or frames): 00:00:50,786:0000001270
    // Subtitle text: In preajma lacului Razel,
    /// <summary>
    /// The unknown subtitle 3.
    /// </summary>
    public class UnknownSubtitle3 : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".txt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 3";
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
            // 150583||3968723||Rythme standard quatre-par-quatre.\~- Sûr... Accord d'entrée, D majeur?||
            // 155822||160350||Rob n'y connait rien en claviers. Il\~commence chaque chanson en D majeur||
            const string paragraphWriteFormat = "{0}||{1}||{2}||";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (!subtitle.WasLoadedWithFrameNumbers)
                {
                    p.CalculateFrameNumbersFromTimeCodes(Configuration.Settings.General.CurrentFrameRate);
                }

                sb.AppendLine(string.Format(paragraphWriteFormat, p.StartFrame, p.EndFrame, p.Text.Replace(Environment.NewLine, "\\~")));
            }

            return sb.ToString().Trim();
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
            foreach (string line in lines)
            {
                this.ReadLine(subtitle, line);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The read line.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        private void ReadLine(Subtitle subtitle, string line)
        {
            // 150583||3968723||Rythme standard quatre-par-quatre.\~- Sûr... Accord d'entrée, D majeur?||
            // 155822||160350||Rob n'y connait rien en claviers. Il\~commence chaque chanson en D majeur||
            string[] parts = line.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3)
            {
                int start;
                int end;
                if (int.TryParse(parts[0], out start) && int.TryParse(parts[1], out end))
                {
                    Paragraph p = new Paragraph(parts[2].Replace("\\~", Environment.NewLine), start, end);
                    subtitle.Paragraphs.Add(p);
                }
                else
                {
                    this._errorCount++;
                }
            }
            else
            {
                this._errorCount++;
            }
        }
    }
}