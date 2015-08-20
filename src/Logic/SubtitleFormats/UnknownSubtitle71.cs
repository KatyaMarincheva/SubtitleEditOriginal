// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle71.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 71.
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
    /// The unknown subtitle 71.
    /// </summary>
    public class UnknownSubtitle71 : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^ \d \d : \d \d : \d \d : \d \d $", RegexOptions.Compiled);

        /// <summary>
        /// The regex time code 2.
        /// </summary>
        private static readonly Regex RegexTimeCode2 = new Regex(@"^\d \d : \d \d : \d \d : \d \d$", RegexOptions.Compiled);

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
                return "Unknown 71";
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
            // 1 .
            // 0 0 : 0 0 : 0 4 : 1 2
            // 0 0 : 0 0 : 0 6 : 0 5
            // H a l l o !

            // 2.
            // 0 0 : 0 0 : 0 6 : 1 6
            // 0 0 : 0 0 : 0 7 : 2 0
            // G e t   i n s i d e ,   m o m !
            // -   I   w a n t   t o   c o m e .
            const string paragraphWriteFormat = "{4} . {3}{3}{0}{3}{3}{1}{3}{3}{2}{3}{3}";
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                count++;
                string text = AddSpaces(HtmlUtil.RemoveOpenCloseTags(p.Text, HtmlUtil.TagFont));
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
                if (expecting == ExpectingLine.Number && (RegexTimeCode.IsMatch(line) || RegexTimeCode2.IsMatch(line.Trim())))
                {
                    this._errorCount++;
                    if (paragraph != null)
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }

                    paragraph = new Paragraph();
                    expecting = ExpectingLine.TimeStart;
                }

                if (line.TrimEnd().EndsWith('.') && Utilities.IsInteger(RemoveSpaces(line.Trim().TrimEnd('.').Trim())))
                {
                    if (paragraph != null)
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }

                    paragraph = new Paragraph();
                    expecting = ExpectingLine.TimeStart;
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeStart && (RegexTimeCode.IsMatch(line) || RegexTimeCode2.IsMatch(line.Trim())))
                {
                    string[] parts = RemoveSpaces(line.Trim()).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
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
                else if (paragraph != null && expecting == ExpectingLine.TimeEnd && (RegexTimeCode.IsMatch(line) || RegexTimeCode2.IsMatch(line.Trim())))
                {
                    string[] parts = RemoveSpaces(line.Trim()).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
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
                    if (line == " " || line.Trim() == @"...........\...........")
                    {
                    }
                    else if (line == "*END*")
                    {
                        this._errorCount++;
                        if (paragraph != null)
                        {
                            subtitle.Paragraphs.Add(paragraph);
                        }

                        paragraph = new Paragraph();
                        expecting = ExpectingLine.Number;
                    }
                    else if (paragraph != null && expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string s = RemoveSpaces(line);
                            paragraph.Text = (paragraph.Text + Environment.NewLine + s).Trim();
                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                return;
                            }
                        }
                    }
                    else if (line.Length > 1)
                    {
                        this._errorCount++;
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
        /// The add spaces.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string AddSpaces(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return " ";
            }

            StringBuilder sb = new StringBuilder(@" ");
            for (int i = 0; i < text.Length; i++)
            {
                sb.Append(text[i]);
                sb.Append(' ');
            }

            return sb.ToString();
        }

        /// <summary>
        /// The remove spaces.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveSpaces(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = text.Trim();
            for (int i = 0; i < text.Length; i++)
            {
                if (i % 2 == 1 && text[i] != ' ')
                {
                    return text;
                }
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append(text[i]);
                }
            }

            return sb.ToString();
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
            string s = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
            return AddSpaces(s);
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
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
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