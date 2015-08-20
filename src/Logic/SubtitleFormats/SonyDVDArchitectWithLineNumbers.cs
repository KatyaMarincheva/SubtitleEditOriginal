// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SonyDVDArchitectWithLineNumbers.cs" company="">
//   
// </copyright>
// <summary>
//   The sony dvd architect with line numbers.
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
    /// The sony dvd architect with line numbers.
    /// </summary>
    public class SonyDVDArchitectWithLineNumbers : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex regexTimeCode = new Regex(@"^\d\d\d\d  \d\d:\d\d:\d\d:\d\d  \d\d:\d\d:\d\d:\d\d", RegexOptions.Compiled);

        /// <summary>
        /// The regex 1 digit milliseconds.
        /// </summary>
        private static readonly Regex regex1DigitMilliseconds = new Regex(@"^\d\d\d\d  \d\d\d:\d\d:\d\d:\d  \d\d\d:\d\d:\d\d:\d", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".sub";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Sony DVDArchitect w. line#";
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
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                text = text.Replace(Environment.NewLine, "\r");
                sb.AppendLine(string.Format("{9:0000}  {0:00}:{1:00}:{2:00}:{3:00}  {4:00}:{5:00}:{6:00}:{7:00}    \t{8}" + Environment.NewLine, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, p.StartTime.Milliseconds / 10, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, p.EndTime.Milliseconds / 10, text, p.Number));
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
        { // 00:04:10:92 - 00:04:13:32    Raise Yourself To Help Mankind
            // 00:04:27:92 - 00:04:30:92    الجهة المتولية للمسئولية الاجتماعية لشركتنا.
            this._errorCount = 0;
            Paragraph lastParagraph = null;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                bool success = false;
                if (line.IndexOf(':') > 0)
                {
                    string s = line;
                    Match match = regexTimeCode.Match(s);
                    Match match1DigitMilliseconds = regex1DigitMilliseconds.Match(s);
                    if (s.Length > 31 && match.Success)
                    {
                        s = s.Substring(5, match.Length - 5).TrimStart();
                        s = s.Replace("  ", ":");
                        s = s.Replace(" ", string.Empty);
                        string[] parts = s.Split(':');
                        if (parts.Length == 8)
                        {
                            int hours = int.Parse(parts[0]);
                            int minutes = int.Parse(parts[1]);
                            int seconds = int.Parse(parts[2]);
                            int milliseconds = int.Parse(parts[3]) * 10;
                            TimeCode start = new TimeCode(hours, minutes, seconds, milliseconds);

                            hours = int.Parse(parts[4]);
                            minutes = int.Parse(parts[5]);
                            seconds = int.Parse(parts[6]);
                            milliseconds = int.Parse(parts[7]) * 10;
                            TimeCode end = new TimeCode(hours, minutes, seconds, milliseconds);

                            string text = line.Replace("\0", string.Empty).Substring(match.Length).TrimStart();
                            text = text.Replace("|", Environment.NewLine);

                            lastParagraph = new Paragraph(start, end, text);
                            subtitle.Paragraphs.Add(lastParagraph);
                            success = true;
                        }
                    }
                    else if (s.Length > 29 && match1DigitMilliseconds.Success)
                    {
                        s = s.Substring(5, match1DigitMilliseconds.Length - 5).TrimStart();
                        s = s.Replace("  ", ":");
                        s = s.Replace(" ", string.Empty);
                        string[] parts = s.Split(':');
                        if (parts.Length == 8)
                        {
                            int hours = int.Parse(parts[0]);
                            int minutes = int.Parse(parts[1]);
                            int seconds = int.Parse(parts[2]);
                            int milliseconds = int.Parse(parts[3]) * 10;
                            TimeCode start = new TimeCode(hours, minutes, seconds, milliseconds);

                            hours = int.Parse(parts[4]);
                            minutes = int.Parse(parts[5]);
                            seconds = int.Parse(parts[6]);
                            milliseconds = int.Parse(parts[7]) * 10;
                            TimeCode end = new TimeCode(hours, minutes, seconds, milliseconds);

                            string text = line.Replace("\0", string.Empty).Substring(match1DigitMilliseconds.Length).TrimStart();
                            text = text.Replace("|", Environment.NewLine);

                            lastParagraph = new Paragraph(start, end, text);
                            subtitle.Paragraphs.Add(lastParagraph);
                            success = true;
                        }
                    }
                    else if (lastParagraph != null && Utilities.GetNumberOfLines(lastParagraph.Text) < 5)
                    {
                        lastParagraph.Text += Environment.NewLine + line.Trim();
                        success = true;
                    }
                }
                else if (lastParagraph != null && Utilities.GetNumberOfLines(lastParagraph.Text) < 5)
                {
                    lastParagraph.Text += Environment.NewLine + line.Trim();
                    success = true;
                }

                if (!success)
                {
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }
    }
}