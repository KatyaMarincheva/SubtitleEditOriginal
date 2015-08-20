// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle73.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 73.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 73.
    /// </summary>
    public class UnknownSubtitle73 : SubtitleFormat
    {
        // 59:00:22:09:14 00:22:12:04 02:15
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d+:\d\d:\d\d:\d\d:\d\d \d\d:\d\d:\d\d:\d\d \d\d:\d\d$", RegexOptions.Compiled);

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
                return "Unknown 73";
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
            // 59:00:22:09:14 00:22:12:04 02:15
            // LÕaria fresca arriva
            // dai monti di Petr—polis
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                index++;
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                text = text.Replace(Environment.NewLine, "\n\n");
                sb.AppendFormat("{0}:{1} {2} {3}\n{4}\n\n", index, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), EncodeDuration(p), text);
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
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            foreach (string line in lines)
            {
                string s = line;
                if (regexTimeCodes.IsMatch(s))
                {
                    s = s.Remove(0, s.IndexOf(':') + 1).Trim();
                    string[] temp = s.Split(' ');
                    if (temp.Length > 1)
                    {
                        string start = temp[0];
                        string end = temp[1];

                        string[] startParts = start.Split(':');
                        string[] endParts = end.Split(':');
                        if (startParts.Length == 4 && endParts.Length == 4)
                        {
                            try
                            {
                                p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), string.Empty);
                                subtitle.Paragraphs.Add(p);
                            }
                            catch (Exception exception)
                            {
                                this._errorCount++;
                                Debug.WriteLine(exception.Message);
                            }
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // skip empty lines
                }
                else if (p == null)
                {
                    this._errorCount++;
                }
                else
                {
                    p.Text = (p.Text + Environment.NewLine + line).Trim();
                    if (p.Text.Length > 1000)
                    {
                        this._errorCount += 10;
                        return;
                    }
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode duration.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeDuration(Paragraph p)
        {
            return string.Format("{0:00}:{1:00}", p.Duration.Seconds, MillisecondsToFramesMaxFrameRate(p.Duration.Milliseconds));
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
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
            return tc;
        }
    }
}