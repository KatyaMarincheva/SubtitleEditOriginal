// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle48.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 48.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    // 00:01:27.703 00:01:29.514 Okay.
    // 00:01:29.259 00:01:31.514 Okaaayyyy.
    // 00:01:32.534 00:01:34.888 Let's go over this once again.
    // 00:01:35.446 00:01:38.346 Pick up the bread, walk the dog, go to the dry cleaners,
    // 00:01:38.609 00:01:41.471 pick up the bread, walk the dog, go thoughtless,
    // 00:01:42.247 00:01:43.915 pick up the cake
    /// <summary>
    /// The unknown subtitle 48.
    /// </summary>
    public class UnknownSubtitle48 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d.\d\d\d \d\d:\d\d:\d\d.\d\d\d .*$", RegexOptions.Compiled);

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
                return "Unknown 48";
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
            if (lines.Count > 0 && lines[0] != null && lines[0].StartsWith("{\\rtf1"))
            {
                return false;
            }

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
            const string paragraphWriteFormat = "{0} {1} {2}";
            const string timeFormat = "{0:00}:{1:00}:{2:00}.{3:000}";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format(timeFormat, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, p.StartTime.Milliseconds);
                string endTime = string.Format(timeFormat, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, p.EndTime.Milliseconds);
                sb.AppendLine(string.Format(paragraphWriteFormat, startTime, endTime, HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, " "))));
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
                if (RegexTimeCodes.Match(line).Success)
                {
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.None);
                    Paragraph p = new Paragraph();
                    if (parts.Length > 2 && GetTimeCode(p.StartTime, parts[0].Trim()) && GetTimeCode(p.EndTime, parts[1].Trim()))
                    {
                        p.Text = line.Remove(0, 25).Trim();
                        subtitle.Paragraphs.Add(p);
                    }
                }
                else
                {
                    this._errorCount += 10;
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="timeString">
        /// The time string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool GetTimeCode(TimeCode timeCode, string timeString)
        {
            try
            {
                string[] timeParts = timeString.Split(':', '.');
                timeCode.Hours = int.Parse(timeParts[0]);
                timeCode.Minutes = int.Parse(timeParts[1]);
                timeCode.Seconds = int.Parse(timeParts[2]);
                timeCode.Milliseconds = int.Parse(timeParts[3]);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}