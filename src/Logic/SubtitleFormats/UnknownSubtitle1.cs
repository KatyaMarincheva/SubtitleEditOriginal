// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle1.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 1.
    /// </summary>
    public class UnknownSubtitle1 : SubtitleFormat
    {
        // 0:01 – 0:11
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d+:\d\d – \d+:\d\d ", RegexOptions.Compiled);

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
                return "Unknown 1";
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
            // 0:01 – 0:11 "My vengeance needs blood!" -Marquis De Sade
            // [Laughter, thunder]
            // 0:17 – 0:19 - On this 5th day of December -
            // 0:19 – 0:22 in the year of our Lord 1648, -
            const string paragraphWriteFormat = "{0} – {1} {2}";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                int seconds = p.StartTime.Seconds;
                if (p.StartTime.Milliseconds >= 500)
                {
                    seconds++;
                }

                string startTime = string.Format("{0:0}:{1:00}", p.StartTime.Minutes + p.StartTime.Hours * 60, seconds);

                seconds = p.EndTime.Seconds;
                if (p.EndTime.Milliseconds >= 500)
                {
                    seconds++;
                }

                string timeOut = string.Format("{0:0}:{1:00}", p.EndTime.Minutes + p.EndTime.Hours * 60, seconds);

                sb.AppendLine(string.Format(paragraphWriteFormat, startTime, timeOut, p.Text));
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
            Paragraph p = null;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            StringBuilder text = new StringBuilder();

            foreach (string line in lines)
            {
                Match match = RegexTimeCodes.Match(line);
                if (match.Success)
                {
                    if (p != null)
                    {
                        p.Text = (p.Text + Environment.NewLine + text.ToString().Trim()).Trim();
                    }

                    string[] parts = line.Substring(0, match.Length).Trim().Split(new[] { '–', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        p = new Paragraph();
                        p.StartTime = DecodeTimeCode(parts[0]);
                        p.EndTime = DecodeTimeCode(parts[1]);
                        p.Text = line.Substring(match.Length - 1).Trim();
                        subtitle.Paragraphs.Add(p);
                        text = new StringBuilder();
                    }
                    catch
                    {
                        p = null;
                        this._errorCount++;
                    }
                }
                else if (p == null)
                {
                    this._errorCount++;
                }
                else
                {
                    text.AppendLine(line);
                }

                if (this._errorCount > 20)
                {
                    return;
                }
            }

            if (p != null)
            {
                p.Text = (p.Text + Environment.NewLine + text.ToString().Trim()).Trim();
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string code)
        {
            // 68:20  (minutes:seconds)
            string[] parts = code.Trim().Split(':');
            return new TimeCode(0, int.Parse(parts[0]), int.Parse(parts[1]), 0);
        }
    }
}