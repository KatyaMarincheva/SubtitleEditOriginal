// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SonyDVDArchitectLineAndDuration.cs" company="">
//   
// </copyright>
// <summary>
//   The sony dvd architect line and duration.
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
    /// The sony dvd architect line and duration.
    /// </summary>
    public class SonyDVDArchitectLineAndDuration : SubtitleFormat
    {
        /// <summary>
        /// The regex.
        /// </summary>
        private static readonly Regex regex = new Regex(@"^\d+\t\d\d:\d\d:\d\d:\d\d\t\d\d:\d\d:\d\d:\d\d\t\d\d:\d\d:\d\d:\d\d$", RegexOptions.Compiled);

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
                return "Sony DVDArchitect line/duration";
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Title: " + title);
            sb.AppendLine("Translator: No Author");
            sb.AppendLine("Date: " + DateTime.Now.ToString("dd-MM-yyyy").Replace("-", ".")); // 25.08.2011
            double milliseconds = 0;
            if (subtitle.Paragraphs.Count > 0)
            {
                milliseconds = subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].EndTime.TotalMilliseconds;
            }

            TimeCode tc = new TimeCode(milliseconds);
            sb.AppendLine(string.Format("Duration: {0:00}:{1:00}:{2:00}:{3:00}", tc.Hours, tc.Minutes, tc.Seconds, MillisecondsToFramesMaxFrameRate(tc.Milliseconds))); // 01:20:49:12
            sb.AppendLine("Program start: 00:00:00:00");
            sb.AppendLine("Title count: " + subtitle.Paragraphs.Count);
            sb.AppendLine();
            sb.AppendLine("#\tIn\tOut\tDuration");
            sb.AppendLine();
            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                count++;
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                sb.AppendLine(string.Format("{13}\t{0:00}:{1:00}:{2:00}:{3:00}\t{4:00}:{5:00}:{6:00}:{7:00}\t{8:00}:{9:00}:{10:00}:{11:00}\r\n{12}" + Environment.NewLine, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, MillisecondsToFramesMaxFrameRate(p.StartTime.Milliseconds), p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, MillisecondsToFramesMaxFrameRate(p.EndTime.Milliseconds), p.Duration.Hours, p.Duration.Minutes, p.Duration.Seconds, MillisecondsToFramesMaxFrameRate(p.Duration.Milliseconds), text, count));
            }

            return sb.ToString().Trim() + Environment.NewLine + Environment.NewLine + Environment.NewLine;
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
        { // 22    00:04:19:12 00:04:21:09 00:00:01:21
            this._errorCount = 0;
            Paragraph lastParagraph = null;
            int count = 0;
            foreach (string line in lines)
            {
                bool isTimeCode = false;
                if (line.Length > 0)
                {
                    bool success = false;
                    if (line.Length > 31 && line.IndexOf(':') > 1)
                    {
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            isTimeCode = true;
                            if (lastParagraph != null)
                            {
                                subtitle.Paragraphs.Add(lastParagraph);
                            }

                            string[] arr = line.Split('\t');
                            TimeCode start = DecodeTimeCode(arr[1]);
                            TimeCode end = DecodeTimeCode(arr[2]);
                            lastParagraph = new Paragraph(start, end, string.Empty);
                            success = true;
                        }
                    }

                    if (!isTimeCode && !string.IsNullOrWhiteSpace(line) && lastParagraph != null && Utilities.GetNumberOfLines(lastParagraph.Text) < 5)
                    {
                        lastParagraph.Text = (lastParagraph.Text + Environment.NewLine + line).Trim();
                        success = true;
                    }

                    if (!success && count > 9)
                    {
                        this._errorCount++;
                    }
                }

                count++;
            }

            if (lastParagraph != null)
            {
                subtitle.Paragraphs.Add(lastParagraph);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string s)
        {
            string[] parts = s.Split(':');

            // 00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
            return tc;
        }
    }
}