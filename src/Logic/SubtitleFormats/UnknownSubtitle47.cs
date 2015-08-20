// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle47.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 47.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 47.
    /// </summary>
    public class UnknownSubtitle47 : SubtitleFormat
    {
        // 7:00:01:27AM
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\:\d\d\:\d\d\:\d\d\t", RegexOptions.Compiled);

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
                return "Unknown 47";
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
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("{0}\t{1}", EncodeTimeCode(p.StartTime), p.Text.Replace(Environment.NewLine, " ")));
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
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (regexTimeCodes.Match(s).Success)
                {
                    try
                    {
                        string[] arr = s.Substring(0, 10).Split(':');
                        if (arr.Length == 4)
                        {
                            int hours = int.Parse(arr[0]);
                            int minutes = int.Parse(arr[1]);
                            int seconds = int.Parse(arr[2]);
                            int frames = int.Parse(arr[3]);
                            Paragraph p = new Paragraph();
                            p.StartTime = new TimeCode(hours, minutes, seconds, FramesToMillisecondsMax999(frames));
                            p.Text = s.Remove(0, 10).Trim();
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }
                else if (s.Length > 0)
                {
                    this._errorCount++;
                }
            }

            int index = 1;
            foreach (Paragraph paragraph in subtitle.Paragraphs)
            {
                Paragraph next = subtitle.GetParagraphOrDefault(index);
                if (next != null)
                {
                    paragraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                }

                if (paragraph.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                {
                    paragraph.EndTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(paragraph.Text);
                }

                index++;
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }

        /// <summary>
        /// The encode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTimeCode(TimeCode timeCode)
        {
            return string.Format("{0}:{1:00}:{2:00}:{3:00}", timeCode.Hours, timeCode.Minutes, timeCode.Seconds, MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds));
        }
    }
}