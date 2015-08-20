// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Footage.cs" company="">
//   
// </copyright>
// <summary>
//   The footage.
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
    /// The footage.
    /// </summary>
    public class Footage : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\s*\d+,\d\d$", RegexOptions.Compiled);

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
                return "Footage";
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
            TimeLineFootageAscii asc = new TimeLineFootageAscii();
            if (fileName != null && asc.IsMine(null, fileName))
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
            // 1.
            // 66,13
            // 70,00
            // #Tā nu es sapazinos
            // #И так я познакомился

            // 2.
            // 71,14
            // 78,10
            // #ar dakteri Henriju Gūsu.
            // #с доктором Генри Гусом.
            const string paragraphWriteFormat = "{4}.{3}{0}{3}{1}{3}{2}{3}";
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                count++;
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                if (p.Text.StartsWith("<i>") && p.Text.EndsWith("</i>"))
                {
                    text = "#" + text;
                }

                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), text, Environment.NewLine, count));
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
            Paragraph paragraph = null;
            ExpectingLine expecting = ExpectingLine.Number;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (line.EndsWith('.') && Utilities.IsInteger(line.TrimEnd('.')))
                {
                    if (paragraph != null && !string.IsNullOrEmpty(paragraph.Text))
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }

                    paragraph = new Paragraph();
                    expecting = ExpectingLine.TimeStart;
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeStart && RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            paragraph.StartTime = tc;
                            expecting = ExpectingLine.TimeEnd;
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.Number;
                        }
                    }
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeEnd && RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            paragraph.EndTime = tc;
                            expecting = ExpectingLine.Text;
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.Number;
                        }
                    }
                }
                else
                {
                    if (paragraph != null && expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string s = line.Trim();
                            if (s.StartsWith('#'))
                            {
                                s = "<i>" + s.Remove(0, 1) + "</i>";
                            }

                            paragraph.Text = (paragraph.Text + Environment.NewLine + s).Trim();
                            paragraph.Text = paragraph.Text.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine);
                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                return;
                            }
                        }
                    }
                }
            }

            if (paragraph != null && !string.IsNullOrEmpty(paragraph.Text))
            {
                subtitle.Paragraphs.Add(paragraph);
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
            int frames = MillisecondsToFrames(time.TotalMilliseconds);
            int footage = frames / 16;
            int rest = (int)((frames % 16) / 16.0 * 24.0);
            return string.Format("{0:00},{1:00}", footage, rest).PadLeft(8);
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
            int frames16 = int.Parse(parts[0]);
            int frames = int.Parse(parts[1]);
            return new TimeCode(0, 0, 0, FramesToMilliseconds(16 * frames16 + frames));
        }

        /// <summary>
        /// The expecting line.
        /// </summary>
        private enum ExpectingLine
        {
            /// <summary>
            /// The number.
            /// </summary>
            Number, 

            /// <summary>
            /// The time start.
            /// </summary>
            TimeStart, 

            /// <summary>
            /// The time end.
            /// </summary>
            TimeEnd, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}