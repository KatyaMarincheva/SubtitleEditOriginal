// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle72.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 72.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 72.
    /// </summary>
    public class UnknownSubtitle72 : SubtitleFormat
    {
        // 00:00:02.000
        // Junior Semifinal, part 1
        // Aidiba Talamunuer, Berezan
        // Bogdan Voloshin, Yaroslavl
        // Alexandr Doronin, Almaty

        // 00:04:41.480
        // G. Zhubanova
        // «Kui»
        // Aidiba Talamunuer, Berezan

        // 00:05:55.000
        // N. Mendigaliev
        // «Steppe»
        // Bogdan Voloshin, Yaroslavl
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d.\d{1,3}$", RegexOptions.Compiled);

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
                return "Unknown 72";
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
            const string paragraphWriteFormat = "{0}\r\n{1}\r\n";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(paragraphWriteFormat, p.StartTime.ToString().Replace(",", "."), p.Text));
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
            Paragraph paragraph = null;
            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].TrimEnd();

                if (line.Contains(':') && RegexTimeCodes.IsMatch(line))
                {
                    if (paragraph != null && string.IsNullOrEmpty(paragraph.Text))
                    {
                        this._errorCount++;
                    }

                    paragraph = new Paragraph();
                    if (this.TryReadTimeCodesLine(line, paragraph))
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
                else if (paragraph != null && paragraph.Text.Length < 1000)
                {
                    paragraph.Text = (paragraph.Text + Environment.NewLine + line).Trim();
                }
                else
                {
                    this._errorCount++;
                }
            }

            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                index++;
                p.Text = p.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                Paragraph nextParagraph = subtitle.GetParagraphOrDefault(index);
                p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text) + 100;
                if (nextParagraph != null && p.EndTime.TotalMilliseconds >= nextParagraph.StartTime.TotalMilliseconds)
                {
                    p.EndTime.TotalMilliseconds = nextParagraph.StartTime.TotalMilliseconds - 1;
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The try read time codes line.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool TryReadTimeCodesLine(string line, Paragraph paragraph)
        {
            string[] parts = line.Split(':', '.');
            try
            {
                int startHours = int.Parse(parts[0]);
                int startMinutes = int.Parse(parts[1]);
                int startSeconds = int.Parse(parts[2]);
                int startMilliseconds = int.Parse(parts[3]);

                if (parts[3].Length == 2)
                {
                    this._errorCount++;
                }

                paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, startMilliseconds);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}