// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwiftInterchange2.cs" company="">
//   
// </copyright>
// <summary>
//   The swift interchange 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The swift interchange 2.
    /// </summary>
    public class SwiftInterchange2 : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".sif";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Swift Interchange File V2";
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
            if (lines.Count > 0 && lines[0] != null && lines[0].StartsWith("{\\rtf1"))
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
            string date = string.Format("{0:00}/{1:00}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            const string header = @"# SWIFT INTERCHANGE FILE V2
# DO NOT EDIT LINES BEGINNING WITH '#' SIGN
# Originating Swift: Line21 30 DROP English (USA)
# VIDEO CLIP : [VIDEO_FILE]
# BROADCAST DATE : [DATE]
# REVISION DATE : [DATE]
# CREATION DATE : [DATE]
# COUNTRY OF ORIGIN : ENG
# EPISODE NUMBER : 0
# DEADLINE DATE : [DATE]
# AUTO TX : false
# CURRENT STYLE : None
# STYLE DATE : None
# STYLE Time : None
# SUBTITLE [1] RU3
# TIMEIN 01:00:00:06
# DURATION 03:21 AUTO
# TIMEOUT --:--:--:--
# START ROW BOTTOM
# ALIGN CENTRE JUSTIFY LEFT
# ROW 0";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header.Replace("[DATE]", date).Replace("[VIDEO_FILE]", title + ".mpg"));
            sb.AppendLine();
            sb.AppendLine();
            const string paragraphWriteFormat = @"# SUBTITLE [{3}] RU3
# TIMEIN {0}
# DURATION {1} AUTO
# TIMEOUT --:--:--:--
# START ROW BOTTOM
# ALIGN CENTRE JUSTIFY LEFT
# ROW 0
{2}";
            int count = 2;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, MillisecondsToFramesMaxFrameRate(p.StartTime.Milliseconds));
                string duration = string.Format("{0:00}:{1:00}", p.Duration.Seconds, MillisecondsToFramesMaxFrameRate(p.Duration.Milliseconds));
                sb.AppendLine(string.Format(paragraphWriteFormat, startTime, duration, HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, " ")), count));
                sb.AppendLine();
                sb.AppendLine();
                count++;
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
            Paragraph p = null;
            foreach (string line in lines)
            {
                if (line.StartsWith("# SUBTITLE"))
                {
                    if (p != null)
                    {
                        subtitle.Paragraphs.Add(p);
                    }

                    p = new Paragraph();
                }
                else if (p != null && line.StartsWith("# TIMEIN"))
                {
                    string timeCode = line.Remove(0, 8).Trim();
                    if (timeCode != "--:--:--:--" && !GetTimeCode(p.StartTime, timeCode))
                    {
                        this._errorCount++;
                    }
                }
                else if (p != null && line.StartsWith("# DURATION"))
                {
                    // # DURATION 01:17 AUTO
                    string timecode = line.Remove(0, 10).Replace("AUTO", string.Empty).Trim();
                    if (timecode != "--:--")
                    {
                        string[] arr = timecode.Split(':', ' ');
                        if (arr.Length > 1)
                        {
                            int sec;
                            int frame;
                            if (int.TryParse(arr[0], out sec) && int.TryParse(arr[1], out frame))
                            {
                                p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + FramesToMillisecondsMax999(frame);
                                p.EndTime.TotalSeconds += sec;
                            }
                        }
                    }
                }
                else if (p != null && line.StartsWith("# TIMEOUT"))
                {
                    string timeCode = line.Remove(0, 9).Trim();
                    if (timeCode != "--:--:--:--" && !GetTimeCode(p.EndTime, timeCode))
                    {
                        this._errorCount++;
                    }
                }
                else if (p != null && !line.StartsWith('#'))
                {
                    if (p.Text.Length > 500)
                    {
                        this._errorCount += 10;
                        return;
                    }

                    p.Text = (p.Text + Environment.NewLine + line).Trim();
                }
            }

            if (p != null)
            {
                subtitle.Paragraphs.Add(p);
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="timeString">
        /// The time string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool GetTimeCode(TimeCode timeCode, string timeString)
        {
            try
            {
                string[] timeParts = timeString.Split(':', '.');
                timeCode.Hours = int.Parse(timeParts[0]);
                timeCode.Minutes = int.Parse(timeParts[1]);
                timeCode.Seconds = int.Parse(timeParts[2]);
                timeCode.Milliseconds = FramesToMillisecondsMax999(int.Parse(timeParts[3]));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}