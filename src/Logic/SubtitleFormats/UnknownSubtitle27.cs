// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle27.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 27.
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
    /// The unknown subtitle 27.
    /// </summary>
    public class UnknownSubtitle27 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:  ", RegexOptions.Compiled);

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
                return "Unknown 27";
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
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                index++;

                // 00:18:02:  (斉藤)失礼な大人って！  (悠子)何言ってんのあんた？
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                text = text.Replace(Environment.NewLine, "  ");
                sb.AppendLine(string.Format("{0}  {1}", EncodeTimeCode(p.StartTime), text));
                sb.AppendLine();
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
            // 00:18:02:  (斉藤)失礼な大人って！  (悠子)何言ってんのあんた？
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (regexTimeCodes.IsMatch(s))
                {
                    string temp = s.Substring(0, 8);

                    string[] startParts = temp.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (startParts.Length == 3)
                    {
                        try
                        {
                            string text = s.Remove(0, 10).Trim();
                            text = text.Replace("  ", Environment.NewLine);
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
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // skip empty lines
                }
                else if (p != null)
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
            // 00:03:15:22 (last is frame)
            int sec = time.Seconds;
            if (time.Milliseconds >= 500)
            {
                sec++;
            }

            return string.Format("{0:00}:{1:00}:{2:00}:", time.Hours, time.Minutes, sec);
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

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), 0);
            return tc;
        }
    }
}