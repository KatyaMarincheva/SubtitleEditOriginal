// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle69.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 69.
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
    /// The unknown subtitle 69.
    /// </summary>
    public class UnknownSubtitle69 : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\d+\) \d\d:\d\d:\d\d:\d\d \d\d:\d\d:\d\d:\d\d Durée : \d\d:\d\d", RegexOptions.Compiled); // 10:00:02F00

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
                return "Unknown 69";
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
            // 1) 00:00:06:14 00:00:07:07 Durée : 00:18 Lisibilité : 011 Intervalle : 06:14 Nbc : 018
            // text
            // line2

            // 2) 00:00:07:14 00:00:09:02 Durée : 01:13 Lisibilité : 023 Intervalle : 00:07 Nbc : 026
            // text
            StringBuilder sb = new StringBuilder();
            string paragraphWriteFormat = "{0}) {1} {2} Durée : {3} Lisibilité : {4} Intervalle : {5} Nbc : {6}" + Environment.NewLine + "{7}";
            int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = HtmlUtil.RemoveHtmlTags(p.Text, true);
                string start = p.StartTime.ToHHMMSSFF();
                string end = p.EndTime.ToHHMMSSFF();
                string duration = string.Format("{0:00}:{1:00}", p.Duration.Seconds, MillisecondsToFramesMaxFrameRate(p.Duration.Milliseconds));
                const string readability = "011";
                const string interval = "06:14";
                string nbc = text.Length.ToString().PadLeft(3, '0');
                sb.AppendLine(string.Format(paragraphWriteFormat, count, start, end, duration, readability, interval, nbc, text));
                sb.AppendLine();
                count++;
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
            subtitle.Paragraphs.Clear();
            StringBuilder text = new StringBuilder();
            Paragraph p = null;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0)
                {
                    if (p != null)
                    {
                        p.Text = text.ToString().Trim();
                    }
                }
                else if (RegexTimeCode.IsMatch(line))
                {
                    string[] timeParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (timeParts.Length > 4)
                    {
                        try
                        {
                            string start = timeParts[1];
                            string end = timeParts[2];
                            p = new Paragraph();
                            p.StartTime = DecodeTimeCode(start);
                            p.EndTime = DecodeTimeCode(end);
                            subtitle.Paragraphs.Add(p);
                            text = new StringBuilder();
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else
                {
                    text.AppendLine(line);
                    if (text.Length > 5000)
                    {
                        return;
                    }
                }
            }

            if (p != null)
            {
                p.Text = text.ToString().Trim();
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timePart">
        /// The time part.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timePart)
        {
            string s = timePart.Substring(0, 11);
            string[] parts = s.Split(new[] { ':', 'F' }, StringSplitOptions.RemoveEmptyEntries);
            return new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), FramesToMillisecondsMax999(int.Parse(parts[3])));
        }
    }
}