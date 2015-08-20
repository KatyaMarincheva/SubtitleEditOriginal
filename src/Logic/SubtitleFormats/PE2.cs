// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PE2.cs" company="">
//   
// </copyright>
// <summary>
//   The p e 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The p e 2.
    /// </summary>
    public class PE2 : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\d\d:\d\d:\d\d:\d\d ", RegexOptions.Compiled);

        /// <summary>
        /// The regex time code end.
        /// </summary>
        private static readonly Regex RegexTimeCodeEnd = new Regex(@"^\d\d:\d\d:\d\d:\d\d$", RegexOptions.Compiled);

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
                return "PE2";
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
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            string s = sb.ToString();
            if (s.Contains("[HEADER]") && s.Contains("[BODY]"))
            {
                return false; // UnknownSubtitle17
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
            // #PE2 Format file
            // 10:00:05:16 You will get a loan of//Rs 1.5 million in 15 minutes.
            // 10:00:08:19
            // 10:00:09:01 What have you brought//as the guarantee?
            // 10:00:12:01
            // 10:00:12:11 What?//I didn't get you.
            // 10:00:14:11
            // 10:00:14:15 We will sanction your loan.
            // 10:00:16:00
            const string paragraphWriteFormat = "{0} {2}{3}{1}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#PE2 Format file");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = p.Text.Replace(Environment.NewLine, "//");
                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), text, Environment.NewLine));
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
            ExpectingLine expecting = ExpectingLine.TimeStart;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Substring(0, 11).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            if (expecting == ExpectingLine.TimeStart)
                            {
                                paragraph = new Paragraph();
                                paragraph.StartTime = tc;
                                expecting = ExpectingLine.Text;
                                if (line.Length > 12)
                                {
                                    paragraph.Text = line.Substring(12).Trim().Replace("//", Environment.NewLine);
                                }
                            }
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.TimeStart;
                        }
                    }
                }
                else if (RegexTimeCodeEnd.IsMatch(line))
                {
                    string[] parts = line.Substring(0, 11).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        TimeCode tc = DecodeTimeCode(parts);
                        paragraph.EndTime = tc;
                        subtitle.Paragraphs.Add(paragraph);
                        if (paragraph.StartTime.TotalMilliseconds < 0.001)
                        {
                            this._errorCount++;
                        }

                        paragraph = new Paragraph();
                        expecting = ExpectingLine.TimeStart;
                    }
                }
                else
                {
                    if (expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string text = line.Replace("//", Environment.NewLine);
                            paragraph.Text += Environment.NewLine + text;
                            expecting = ExpectingLine.TimeEndOrText;

                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                return;
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(line) && line != "#PE2 Format file")
                    {
                        this._errorCount++;
                    }
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
            /// The time start.
            /// </summary>
            TimeStart, 

            /// <summary>
            /// The text.
            /// </summary>
            Text, 

            /// <summary>
            /// The time end or text.
            /// </summary>
            TimeEndOrText
        }
    }
}