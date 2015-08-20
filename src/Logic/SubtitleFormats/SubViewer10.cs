// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubViewer10.cs" company="">
//   
// </copyright>
// <summary>
//   The sub viewer 10.
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
    /// The sub viewer 10.
    /// </summary>
    public class SubViewer10 : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex regexTimeCode = new Regex(@"^\[\d\d:\d\d:\d\d\]$", RegexOptions.Compiled);

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
                return "SubViewer 1.0";
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
            // [00:02:14]
            // Yes a new line|Line number 2
            // [00:02:15]
            string paragraphWriteFormat = "[{0:00}:{1:00}:{2:00}]" + Environment.NewLine + "{3}" + Environment.NewLine + "[{4:00}:{5:00}:{6:00}]";
            const string header = @"[TITLE]
{0}
[AUTHOR]
[SOURCE]
[PRG]
[FILEPATH]
[DELAY]
0
[CD TRACK]
0
[BEGIN]
******** START SCRIPT ********
";
            const string footer = @"[end]
******** END SCRIPT ********
";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(header, title);
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, "|"));

                sb.AppendLine(string.Format(paragraphWriteFormat, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, text, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds));
                sb.AppendLine();
            }

            sb.Append(footer);
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
            Paragraph paragraph = new Paragraph();
            ExpectingLine expecting = ExpectingLine.TimeStart;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (line.StartsWith('[') && regexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ':', ']', '[', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        try
                        {
                            int startHours = int.Parse(parts[0]);
                            int startMinutes = int.Parse(parts[1]);
                            int startSeconds = int.Parse(parts[2]);
                            TimeCode tc = new TimeCode(startHours, startMinutes, startSeconds, 0);
                            if (expecting == ExpectingLine.TimeStart)
                            {
                                paragraph = new Paragraph();
                                paragraph.StartTime = tc;
                                expecting = ExpectingLine.Text;
                            }
                            else if (expecting == ExpectingLine.TimeEnd)
                            {
                                paragraph.EndTime = tc;
                                expecting = ExpectingLine.TimeStart;
                                subtitle.Paragraphs.Add(paragraph);
                                paragraph = new Paragraph();
                            }
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.TimeStart;
                        }
                    }
                }
                else
                {
                    if (expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string text = line.Replace("|", Environment.NewLine);
                            paragraph.Text = text;
                            expecting = ExpectingLine.TimeEnd;
                        }
                    }
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The expecting line.
        /// </summary>
        private enum ExpectingLine
        {
            /// <summary>
            /// The time start.
            /// </summary>
            TimeStart, 

            /// <summary>
            /// The text.
            /// </summary>
            Text, 

            /// <summary>
            /// The time end.
            /// </summary>
            TimeEnd
        }
    }
}