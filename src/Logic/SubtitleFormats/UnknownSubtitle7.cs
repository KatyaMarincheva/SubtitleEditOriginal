// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle7.cs" company="">
//   
// </copyright>
// <summary>
//   Reported by dipa nuswantara
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     Reported by dipa nuswantara
    /// </summary>
    public class UnknownSubtitle7 : SubtitleFormat
    {
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
                return "Unknown 7";
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
            // 00:00:54:16 Bisakah kalian diam,Tolong!
            // 00:00:56:07
            // 00:00:57:16 Benar, tepatnya saya tidak memiliki "Anda
            // sudah mendapat 24 jam" adegan... tapi
            // 00:01:02:03
            const string paragraphWriteFormat = "{0} {2}{3}{1}\t";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), p.Text, Environment.NewLine));
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
            Regex regexTimeCode = new Regex(@"^\d\d:\d\d:\d\d:\d\d ", RegexOptions.Compiled);
            Regex regexTimeCodeEnd = new Regex(@"^\d\d:\d\d:\d\d:\d\d\t$", RegexOptions.Compiled);

            Paragraph paragraph = new Paragraph();
            ExpectingLine expecting = ExpectingLine.TimeStart;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            int count = 0;
            foreach (string line in lines)
            {
                count++;
                if (regexTimeCode.IsMatch(line))
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
                                    paragraph.Text = line.Substring(12).Trim();
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
                else if (regexTimeCodeEnd.IsMatch(line) || (count == lines.Count && regexTimeCodeEnd.IsMatch(line + "\t")))
                {
                    string[] parts = line.Substring(0, 11).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        TimeCode tc = DecodeTimeCode(parts);
                        paragraph.EndTime = tc;
                        subtitle.Paragraphs.Add(paragraph);
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
                            string text = line.Replace("|", Environment.NewLine);
                            paragraph.Text += Environment.NewLine + text;
                            expecting = ExpectingLine.TimeEndOrText;

                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                return;
                            }
                        }
                    }
                    else
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

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
            return tc;
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