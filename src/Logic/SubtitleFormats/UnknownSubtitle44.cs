// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle44.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 44.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 44.
    /// </summary>
    public class UnknownSubtitle44 : SubtitleFormat
    {
        // >>> "COMMON GROUND" IS FUNDED BY  10:01:04:12                         1
        // THE MINNESOTA ARTS AND CULTURAL   10:01:07:09
        /// <summary>
        /// The regex time codes 1.
        /// </summary>
        private static readonly Regex regexTimeCodes1 = new Regex(@" \d\d:\d\d:\d\d:\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes 2.
        /// </summary>
        private static readonly Regex regexTimeCodes2 = new Regex(@" \d\d:\d\d:\d\d:\d\d         +\d+$", RegexOptions.Compiled);

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
                return "Unknown 44";
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

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

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
            int index = 0;
            int index2 = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                index++;
                StringBuilder text = new StringBuilder();
                text.Append(HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, " ")));
                while (text.Length < 34)
                {
                    text.Append(' ');
                }

                sb.AppendFormat("{0}{1}", text, EncodeTimeCode(p.StartTime));
                if (index % 50 == 1)
                {
                    index2++;
                    sb.Append(new string(' ', 25) + index2);
                }

                sb.AppendLine();
                Paragraph next = subtitle.GetParagraphOrDefault(index);
                if (next != null && next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds > 150)
                {
                    text = new StringBuilder();
                    while (text.Length < 34)
                    {
                        text.Append(' ');
                    }

                    sb.AppendLine(string.Format("{0}{1}", text, EncodeTimeCode(p.EndTime)));
                }
            }

            return sb.ToString();
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
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                string s = line.Trim();
                Match match = regexTimeCodes2.Match(s);
                if (match.Success)
                {
                    s = s.Substring(0, match.Index + 13).Trim();
                }

                match = regexTimeCodes1.Match(s);
                if (match.Success && match.Index > 13)
                {
                    string text = s.Substring(0, match.Index).Trim();
                    string timeCode = s.Substring(match.Index).Trim();

                    string[] startParts = timeCode.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (startParts.Length == 4)
                    {
                        try
                        {
                            p = new Paragraph(DecodeTimeCode(startParts), new TimeCode(0, 0, 0, 0), text);
                            subtitle.Paragraphs.Add(p);
                        }
                        catch (Exception exception)
                        {
                            this._errorCount++;
                            Debug.WriteLine(exception.Message);
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line) || regexTimeCodes1.IsMatch("   " + s))
                {
                    // skip empty lines
                }
                else if (!string.IsNullOrWhiteSpace(line) && p != null)
                {
                    this._errorCount++;
                }
            }

            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph current = subtitle.Paragraphs[i];
                Paragraph next = subtitle.GetParagraphOrDefault(i + 1);
                if (next != null)
                {
                    current.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                }
                else
                {
                    current.EndTime.TotalMilliseconds = current.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(current.Text);
                }

                if (current.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                {
                    current.EndTime.TotalMilliseconds = current.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
                }
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }

        /// <summary>
        /// The encode time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTimeCode(TimeCode time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string[] parts)
        {
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