// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle65.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 65.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 65.
    /// </summary>
    public class UnknownSubtitle65 : SubtitleFormat
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
                return "Unknown 65";
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
            const string paragraphWriteFormat = "{0:00}:{1:00}:{2:00},{3:00}:{4:00}:{5:00}{6}{7}";

            // 00:00:08,00:00:13
            // The 8.7 update will bring the British self-propelled guns, the map, called Severogorsk,

            // 00:00:13,00:00:18
            // the soviet light tank MT-25 and the new German premium TD, the E25.

            // 00:00:18,00:00:22
            // We will tell you about this and lots of other things in our review.
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = p.Text.Replace(Environment.NewLine, " ");

                sb.AppendLine(string.Format(paragraphWriteFormat, p.StartTime.Hours, p.StartTime.Minutes, RoundSeconds(p.StartTime), p.EndTime.Hours, p.EndTime.Minutes, RoundSeconds(p.EndTime), Environment.NewLine, text));
                sb.AppendLine();
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
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d,\d\d:\d\d:\d\d$", RegexOptions.Compiled);

            Paragraph paragraph = new Paragraph();
            ExpectingLine expecting = ExpectingLine.TimeCodes;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (regexTimeCodes.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ':', ',', '.', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 6)
                    {
                        try
                        {
                            int startHours = int.Parse(parts[0]);
                            int startMinutes = int.Parse(parts[1]);
                            int startSeconds = int.Parse(parts[2]);
                            int endHours = int.Parse(parts[3]);
                            int endMinutes = int.Parse(parts[4]);
                            int endSeconds = int.Parse(parts[5]);
                            paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, 0);
                            paragraph.EndTime = new TimeCode(endHours, endMinutes, endSeconds, 0);
                            expecting = ExpectingLine.Text;
                        }
                        catch
                        {
                            expecting = ExpectingLine.TimeCodes;
                        }
                    }
                }
                else
                {
                    if (expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string text = Utilities.AutoBreakLine(line.Trim());
                            paragraph.Text = text;
                            subtitle.Paragraphs.Add(paragraph);
                            paragraph = new Paragraph();
                            expecting = ExpectingLine.TimeCodes;
                        }
                    }
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The round seconds.
        /// </summary>
        /// <param name="tc">
        /// The tc.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int RoundSeconds(TimeCode tc)
        {
            int rounded = (int)Math.Round(tc.Seconds + tc.Milliseconds / TimeCode.BaseUnit);
            return rounded;
        }

        /// <summary>
        /// The expecting line.
        /// </summary>
        private enum ExpectingLine
        {
            /// <summary>
            /// The time codes.
            /// </summary>
            TimeCodes, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}