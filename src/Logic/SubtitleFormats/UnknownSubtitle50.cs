// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle50.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 50.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    // 00.00.05.09-00.00.08.29
    // We don't have a fancy stock abbreviation
    // to go alongside our name in the press.
    // 00.00.09.15-00.00.11.09

    // We don't have a profit margin.
    // 00.00.11.12-00.00.13.29
    // We don't have sacred rock stars
    // that we put above others.
    /// <summary>
    /// The unknown subtitle 50.
    /// </summary>
    public class UnknownSubtitle50 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d\d\.\d\d\.\d\d\.\d\d-\d\d\.\d\d\.\d\d\.\d\d$", RegexOptions.Compiled);

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
                return "Unknown 50";
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
            const string paragraphWriteFormat = "{0}-{1}\r\n{2}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = p.Text;
                if (Utilities.GetNumberOfLines(text) > 2)
                {
                    text = Utilities.AutoBreakLine(text);
                }

                text = HtmlUtil.RemoveHtmlTags(text, true);
                if (p.Text.Contains("<i>"))
                {
                    if (Utilities.CountTagInText(p.Text, "<i>") == 1 && Utilities.CountTagInText(p.Text, "</i>") == 1 && p.Text.StartsWith("<i>") && p.Text.StartsWith("<i>"))
                    {
                        text = "||" + text.Replace(Environment.NewLine, "||" + Environment.NewLine + "||") + "||";
                    }
                    else if (Utilities.CountTagInText(p.Text, "<i>") == 2 && Utilities.CountTagInText(p.Text, "</i>") == 2 && p.Text.StartsWith("<i>") && p.Text.StartsWith("<i>") && p.Text.Contains("</i>" + Environment.NewLine + "<i>"))
                    {
                        text = "||" + text.Replace(Environment.NewLine, "||" + Environment.NewLine + "||") + "||";
                    }
                }

                if (!text.Contains(Environment.NewLine))
                {
                    text = Environment.NewLine + text;
                }

                sb.AppendLine(string.Format(paragraphWriteFormat, FormatTime(p.StartTime), FormatTime(p.EndTime), text));
            }

            sb.AppendLine();
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
            ExpectingLine expecting = ExpectingLine.TimeCodes;
            Paragraph p = new Paragraph();
            expecting = ExpectingLine.TimeCodes;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (expecting == ExpectingLine.TimeCodes && RegexTimeCodes.IsMatch(line))
                {
                    if (p.Text.Length > 0 || p.EndTime.TotalMilliseconds > 0.1)
                    {
                        subtitle.Paragraphs.Add(p);
                        p = new Paragraph();
                    }

                    if (TryReadTimeCodesLine(line, p))
                    {
                        expecting = ExpectingLine.Text1;
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
                else if (expecting == ExpectingLine.Text1)
                {
                    if (p.Text.Length > 500)
                    {
                        this._errorCount += 100;
                        return;
                    }

                    if (line.StartsWith("||"))
                    {
                        line = "<i>" + line.Replace("||", string.Empty) + "</i>";
                    }

                    p.Text = line.Trim();
                    expecting = ExpectingLine.Text2;
                }
                else if (expecting == ExpectingLine.Text2)
                {
                    if (p.Text.Length > 500)
                    {
                        this._errorCount += 100;
                        return;
                    }

                    if (line.StartsWith("||"))
                    {
                        line = "<i>" + line.Replace("||", string.Empty) + "</i>";
                    }

                    p.Text = (p.Text + Environment.NewLine + line).Trim();
                    expecting = ExpectingLine.TimeCodes;
                }
            }

            if (!string.IsNullOrWhiteSpace(p.Text))
            {
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The format time.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatTime(TimeCode timeCode)
        {
            return string.Format("{0:00}.{1:00}.{2:00}.{3:00}", timeCode.Hours, timeCode.Minutes, timeCode.Seconds, MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds));
        }

        /// <summary>
        /// The try read time codes line.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TryReadTimeCodesLine(string line, Paragraph paragraph)
        {
            string[] parts = line.Replace("-", ".").Split('.');
            try
            {
                int startHours = int.Parse(parts[0]);
                int startMinutes = int.Parse(parts[1]);
                int startSeconds = int.Parse(parts[2]);
                int startMilliseconds = int.Parse(parts[3]);
                int endHours = int.Parse(parts[4]);
                int endMinutes = int.Parse(parts[5]);
                int endSeconds = int.Parse(parts[6]);
                int endMilliseconds = int.Parse(parts[7]);
                paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, startMilliseconds);
                paragraph.EndTime = new TimeCode(endHours, endMinutes, endSeconds, endMilliseconds);
                return true;
            }
            catch
            {
                return false;
            }
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
            /// The text 1.
            /// </summary>
            Text1, 

            /// <summary>
            /// The text 2.
            /// </summary>
            Text2
        }
    }
}