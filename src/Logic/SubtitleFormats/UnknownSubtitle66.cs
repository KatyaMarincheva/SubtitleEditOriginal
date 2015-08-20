// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle66.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 66.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 66.
    /// </summary>
    public class UnknownSubtitle66 : SubtitleFormat
    {
        // 24       10:08:57:17   10:08:59:15       01:23
        // The question is,
        // 25       10:08:59:19   10:09:04:01       04:07
        // is this upside-down vision
        // permanent or only temporary?
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d+\s+\d\d:\d\d:\d\d\:\d\d\s+\d\d:\d\d:\d\d\:\d\d\s+\d\d:\d\d$", RegexOptions.Compiled);

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
                return "Unknown 66";
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
            const string format = "{0}       {1}   {2}       {3:00}:{4:00}";
            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(format, count.ToString().PadLeft(5), EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), p.Duration.Seconds, MillisecondsToFramesMaxFrameRate(p.Duration.Milliseconds)));
                sb.AppendLine(p.Text);
                sb.AppendLine();
                count++;
            }

            return sb.ToString().TrimEnd();
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
            bool expectStartTime = true;
            Paragraph p = new Paragraph();
            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                string s = line.Trim().Replace("*", string.Empty);
                Match match = regexTimeCodes.Match(s);
                if (match.Success)
                {
                    string[] parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(p.Text))
                            {
                                subtitle.Paragraphs.Add(p);
                                p = new Paragraph();
                            }

                            p.StartTime = DecodeTimeCode(parts[1]);
                            p.EndTime = DecodeTimeCode(parts[2]);
                            expectStartTime = false;
                        }
                        catch (Exception exception)
                        {
                            this._errorCount++;
                            Debug.WriteLine(exception.Message);
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    if (p.StartTime.TotalMilliseconds == 0 && p.EndTime.TotalMilliseconds == 0)
                    {
                        this._errorCount++;
                    }
                    else
                    {
                        subtitle.Paragraphs.Add(p);
                    }

                    p = new Paragraph();
                }
                else if (!expectStartTime)
                {
                    p.Text = (p.Text + Environment.NewLine + line).Trim();
                    if (p.Text.Length > 500)
                    {
                        this._errorCount += 10;
                        return;
                    }

                    while (p.Text.Contains(Environment.NewLine + " "))
                    {
                        p.Text = p.Text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    }
                }
            }

            if (!string.IsNullOrEmpty(p.Text))
            {
                subtitle.Paragraphs.Add(p);
            }

            subtitle.RemoveEmptyLines();
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
            string[] parts = part.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
        }
    }
}