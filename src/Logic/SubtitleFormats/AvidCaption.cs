// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvidCaption.cs" company="">
//   
// </copyright>
// <summary>
//   The avid caption.
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
    /// The avid caption.
    /// </summary>
    public class AvidCaption : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d \d\d:\d\d:\d\d:\d\d$", RegexOptions.Compiled);

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
                return "Avid Caption";
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
            int index = 0;
            sb.AppendLine("@ This file written with the Avid Caption plugin, version 1");
            sb.AppendLine();
            sb.AppendLine("<begin subtitles>");
            const string writeFormat = "{0} {1}{2}{3}{2}";
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(writeFormat, p.StartTime.ToHHMMSSFF(), EncodeEndTimeCode(p.EndTime), Environment.NewLine, HtmlUtil.RemoveHtmlTags(p.Text, true)));

                // 00:50:34:22 00:50:39:13
                // Ich muss dafür sorgen,
                // dass die Epsteins weiterleben
                index++;
            }

            sb.AppendLine("<end subtitles>");
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
            // 00:03:15:22  00:03:23:10 This is line one.
            // This is line two.
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            bool beginFound = false;
            bool endFound = false;
            foreach (string line in lines)
            {
                string tline = line.Trim();
                if (tline.Equals("<begin subtitles>", StringComparison.OrdinalIgnoreCase))
                {
                    beginFound = true;
                }
                else if (tline.Equals("<end subtitles>", StringComparison.OrdinalIgnoreCase))
                {
                    endFound = true;
                    break;
                }

                if (line.IndexOf(':') == 2 && regexTimeCodes.IsMatch(line))
                {
                    string temp = line.Substring(0, regexTimeCodes.Match(line).Length);
                    string start = temp.Substring(0, 11);
                    string end = temp.Substring(12, 11);

                    string[] startParts = start.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] endParts = end.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (startParts.Length == 4 && endParts.Length == 4)
                    {
                        p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), string.Empty);
                        subtitle.Paragraphs.Add(p);
                    }
                }
                else if (tline.Length == 0 || tline[0] == '@')
                {
                    // skip these lines
                }
                else if (tline.Length > 0 && p != null)
                {
                    if (string.IsNullOrEmpty(p.Text))
                    {
                        p.Text = line;
                    }
                    else
                    {
                        p.Text = p.Text.TrimEnd() + Environment.NewLine + line;
                    }
                }
            }

            if (!beginFound)
            {
                this._errorCount++;
            }

            if (!endFound)
            {
                this._errorCount++;
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode end time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeEndTimeCode(TimeCode time)
        {
            // 00:50:39:13 (last is frame)

            // Bugfix for Avid - On 23.976 FPS and 24 FPS projects, when the End time of a subtitle ends in 02, 07, 12, 17, 22, 27 frames, the subtitle won't import.
            if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 23.976) < 0.01 || Math.Abs(Configuration.Settings.General.CurrentFrameRate - 24) < 0.01)
            {
                int frames = MillisecondsToFramesMaxFrameRate(time.Milliseconds);
                if (frames == 2 || frames == 7 || frames == 12 || frames == 17 || frames == 22 || frames == 27)
                {
                    frames--;
                }

                return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, frames);
            }

            return time.ToHHMMSSFF();
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