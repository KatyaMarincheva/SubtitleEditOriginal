// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle51.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 51.
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
    /// The unknown subtitle 51.
    /// </summary>
    public class UnknownSubtitle51 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d+:\d+:\d+:\d+  ,  \d+:\d+:\d+:\d+  , .*$", RegexOptions.Compiled);

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
                return "Unknown 51";
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
            const string paragraphWriteFormat = "{0}  ,  {1}  , {2}\r\n";
            const string timeFormat = "{0:00}:{1:00}:{2:00}:{3:00}";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format(timeFormat, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, MillisecondsToFramesMaxFrameRate(p.StartTime.Milliseconds));
                string endTime = string.Format(timeFormat, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, MillisecondsToFramesMaxFrameRate(p.EndTime.Milliseconds));
                sb.AppendFormat(paragraphWriteFormat, startTime, endTime, HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, " | ")));
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
            int number = 0;
            Paragraph p = null;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || string.IsNullOrWhiteSpace(line.Trim('-')))
                {
                    continue;
                }

                if (RegexTimeCodes.Match(line).Success)
                {
                    string[] threePart = line.Split(new[] { ',' }, StringSplitOptions.None);
                    p = new Paragraph();
                    if (threePart.Length > 2 && line.Length > 32 && GetTimeCode(p.StartTime, threePart[0].Trim()) && GetTimeCode(p.EndTime, threePart[1].Trim()))
                    {
                        number++;
                        p.Number = number;
                        p.Text = line.Remove(0, 31).Trim().Replace(" | ", Environment.NewLine).Replace("|", Environment.NewLine);
                        subtitle.Paragraphs.Add(p);
                    }
                }
                else if (line.StartsWith("//"))
                {
                    // comment
                }
                else if (p != null && p.Text.Length < 200)
                {
                    p.Text = (p.Text + Environment.NewLine + line.Trim()).Trim();
                }
                else
                {
                    this._errorCount++;
                }
            }
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
                string[] timeParts = timeString.Split(':');
                timeCode.Hours = int.Parse(timeParts[0]);
                timeCode.Minutes = int.Parse(timeParts[1]);
                timeCode.Seconds = int.Parse(timeParts[2]);
                timeCode.Milliseconds = FramesToMillisecondsMax999(int.Parse(timeParts[3]));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}