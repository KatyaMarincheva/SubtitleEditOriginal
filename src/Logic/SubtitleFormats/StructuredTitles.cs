// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructuredTitles.cs" company="">
//   
// </copyright>
// <summary>
//   The structured titles.
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
    /// The structured titles.
    /// </summary>
    public class StructuredTitles : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d\d\d\d : \d\d:\d\d:\d\d:\d\d,\d\d:\d\d:\d\d:\d\d,\d\d", RegexOptions.Compiled);

        /// <summary>
        /// The regex some codes.
        /// </summary>
        private static readonly Regex RegexSomeCodes = new Regex(@"^\d\d \d\d \d\d", RegexOptions.Compiled);

        /// <summary>
        /// The regex text.
        /// </summary>
        private static readonly Regex RegexText = new Regex(@"^[A-Z]\d[A-Z]\d\d ", RegexOptions.Compiled);

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
                return "Structured titles";
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
            sb.AppendLine(@"Structured titles
0000 : --:--:--:--,--:--:--:--,10
80 80 80
");

            // 0001 : 01:07:25:08,01:07:29:00,10
            // 80 80 80
            // C1Y00 Niemand zal je helpen ontsnappen.
            // C1Y00 - Een agent heeft me geholpen.
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("{0:0000} : {1},{2},10", index + 1, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime)));
                sb.AppendLine("80 80 80");
                foreach (string line in p.Text.SplitToLines())
                {
                    sb.AppendLine("C1Y00 " + line.Trim());
                }

                sb.AppendLine();
                index++;
            }

            sb.AppendLine(string.Format("{0:0000}", index + 1) + @" : --:--:--:--,--:--:--:--,-1
80 80 80");
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
            // 0001 : 01:07:25:08,01:07:29:00,10
            this._errorCount = 0;
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (line.IndexOf(':') == 5 && RegexTimeCodes.IsMatch(line))
                {
                    if (p != null)
                    {
                        subtitle.Paragraphs.Add(p);
                    }

                    string start = line.Substring(7, 11);
                    string end = line.Substring(19, 11);

                    string[] startParts = start.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] endParts = end.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (startParts.Length == 4 && endParts.Length == 4)
                    {
                        p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), string.Empty);
                    }
                }
                else if (p != null && RegexText.IsMatch(line))
                {
                    if (string.IsNullOrEmpty(p.Text))
                    {
                        p.Text = line.Substring(5).Trim();
                    }
                    else
                    {
                        p.Text += Environment.NewLine + line.Substring(5).Trim();
                    }
                }
                else if (line.Length < 10 && RegexSomeCodes.IsMatch(line))
                {
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // skip these lines
                }
                else if (p != null)
                {
                    if (p.Text != null && Utilities.GetNumberOfLines(p.Text) > 3)
                    {
                        this._errorCount++;
                    }
                    else
                    {
                        if (!line.TrimEnd().EndsWith(": --:--:--:--,--:--:--:--,-1", StringComparison.Ordinal))
                        {
                            if (string.IsNullOrEmpty(p.Text))
                            {
                                p.Text = line.Trim();
                            }
                            else
                            {
                                p.Text += Environment.NewLine + line.Trim();
                            }
                        }
                    }
                }
            }

            if (p != null && !string.IsNullOrEmpty(p.Text))
            {
                subtitle.Paragraphs.Add(p);
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
            // 00:00:07:12
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            return new TimeCode(hour, minutes, seconds, FramesToMillisecondsMax999(frames));
        }
    }
}