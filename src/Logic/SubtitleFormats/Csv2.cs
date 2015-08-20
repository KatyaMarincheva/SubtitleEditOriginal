// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Csv2.cs" company="">
//   
// </copyright>
// <summary>
//   The csv 2.
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
    /// The csv 2.
    /// </summary>
    public class Csv2 : SubtitleFormat
    {
        /// <summary>
        /// The separator.
        /// </summary>
        private const string Separator = ",";

        // 1,01:00:10:03,01:00:15:25,I thought I should let my sister-in-law know.
        /// <summary>
        /// The csv line.
        /// </summary>
        private static readonly Regex CsvLine = new Regex(@"^\d+" + Separator + @"\d\d:\d\d:\d\d:\d\d" + Separator + @"\d\d:\d\d:\d\d:\d\d" + Separator, RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".csv";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Csv2";
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
            int fine = 0;
            int failed = 0;
            bool continuation = false;
            foreach (string line in lines)
            {
                Match m = CsvLine.Match(line);
                if (m.Success)
                {
                    fine++;
                    string s = line.Remove(0, m.Length);
                    continuation = s.StartsWith('"');
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (continuation)
                    {
                        continuation = false;
                    }
                    else
                    {
                        failed++;
                    }
                }
            }

            return fine > failed;
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
            const string format = "{1}{0}{2}{0}{3}{0}\"{4}\"";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(format, Separator, "Number", "Start time (hh:mm:ss:ff)", "End time (hh:mm:ss:ff)", "Text"));
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(format, Separator, p.Number, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), p.Text.Replace(Environment.NewLine, "\n")));
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
            this._errorCount = 0;
            bool continuation = false;
            Paragraph p = null;
            foreach (string line in lines)
            {
                Match m = CsvLine.Match(line);
                if (m.Success)
                {
                    string[] parts = line.Substring(0, m.Length).Split(Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        try
                        {
                            TimeCode start = DecodeTimeCode(parts[1]);
                            TimeCode end = DecodeTimeCode(parts[2]);
                            string text = line.Remove(0, m.Length);
                            continuation = text.StartsWith('"') && !text.EndsWith('"');
                            text = text.Trim('"');
                            p = new Paragraph(start, end, text);
                            subtitle.Paragraphs.Add(p);
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (continuation)
                    {
                        if (p != null && p.Text.Length < 300)
                        {
                            p.Text = (p.Text + Environment.NewLine + line.TrimEnd('"')).Trim();
                        }

                        continuation = !line.TrimEnd().EndsWith('"');
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
        /// <param name="part">
        /// The part.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string part)
        {
            string[] parts = part.Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries);

            // 00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
        }
    }
}