// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle6.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 6.
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
    /// The unknown subtitle 6.
    /// </summary>
    public class UnknownSubtitle6 : SubtitleFormat
    {
        /// <summary>
        /// The regex before text.
        /// </summary>
        private static readonly Regex regexBeforeText = new Regex(@"^\d\s+\d\s+\d\s+\d\s+\d\s+\d$", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d+\s+\d+$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".titl";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 6";
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

            sb.Append(' ');
            sb.Append(subtitle.Paragraphs.Count);
            sb.AppendLine("           4             1234 ");
            sb.AppendLine(@"NORMAL
00:00:00.00

SRPSKI

00:00:00.00
26.11.2008  18:54:15");

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string firstLine = string.Empty;
                string secondLine = string.Empty;
                string[] lines = p.Text.SplitToLines();
                if (lines.Length > 2)
                {
                    lines = Utilities.AutoBreakLine(p.Text).SplitToLines();
                }

                if (lines.Length > 0)
                {
                    firstLine = lines[0];
                }

                if (lines.Length > 1)
                {
                    secondLine = lines[1];
                }

                sb.AppendLine(string.Format(" {0}          {1} " + Environment.NewLine + "1    0    0    0    0    0" + Environment.NewLine + "{2}" + Environment.NewLine + "{3}", p.StartTime.TotalMilliseconds / 10, p.EndTime.TotalMilliseconds / 10, firstLine, secondLine));
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
            Paragraph paragraph = new Paragraph();
            ExpectingLine expecting = ExpectingLine.TimeCodes;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (regexTimeCodes.IsMatch(s))
                {
                    if (!string.IsNullOrEmpty(paragraph.Text))
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }

                    paragraph = new Paragraph();
                    string[] parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            paragraph.StartTime.TotalMilliseconds = long.Parse(parts[0]) * 10;
                            paragraph.EndTime.TotalMilliseconds = long.Parse(parts[1]) * 10;
                            expecting = ExpectingLine.BeforeText;
                        }
                        catch
                        {
                            expecting = ExpectingLine.TimeCodes;
                        }
                    }
                }
                else if (regexBeforeText.IsMatch(s))
                {
                    expecting = ExpectingLine.Text;
                }
                else
                {
                    if (expecting == ExpectingLine.Text)
                    {
                        if (s.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(paragraph.Text))
                            {
                                paragraph.Text += Environment.NewLine + s;
                            }
                            else
                            {
                                paragraph.Text = s;
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(paragraph.Text))
            {
                subtitle.Paragraphs.Add(paragraph);
            }

            subtitle.Renumber();
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
            /// The before text.
            /// </summary>
            BeforeText, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}