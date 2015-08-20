// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle46.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 46.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 46.
    /// </summary>
    public class UnknownSubtitle46 : SubtitleFormat
    {
        // 7:00:01:27AM
        /// <summary>
        /// The regex time codes am.
        /// </summary>
        private static readonly Regex regexTimeCodesAM = new Regex(@"^\d\:\d\d\:\d\d\:\d\dAM", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes pm.
        /// </summary>
        private static readonly Regex regexTimeCodesPM = new Regex(@"^\d\:\d\d\:\d\d\:\d\dPM", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".pst";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 46";
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
            // OFF THE RECORD STARTS RIGHT NOW.   7:00:01:27AM
            // HERE IS THE RUNDOWN.               7:00:05:03AM
            StringBuilder sb = new StringBuilder();
            const string format = "{0}{1}";
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(format, p.Text.Replace(Environment.NewLine, " ").PadRight(35), EncodeTimeCode(p.StartTime)));
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
                string s = line.Trim();
                string[] arr = line.Split();
                string timeCode = arr[arr.Length - 1];
                if (regexTimeCodesAM.Match(timeCode).Success || regexTimeCodesPM.Match(timeCode).Success)
                {
                    try
                    {
                        arr = timeCode.Substring(0, 10).Split(':');
                        if (arr.Length == 4)
                        {
                            int hours = int.Parse(arr[0]);
                            int minutes = int.Parse(arr[1]);
                            int seconds = int.Parse(arr[2]);
                            int frames = int.Parse(arr[3]);
                            p = new Paragraph();
                            p.StartTime = new TimeCode(hours, minutes, seconds, FramesToMillisecondsMax999(frames));
                            p.Text = s.Substring(0, s.IndexOf(timeCode, StringComparison.Ordinal)).Trim();
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
                    paragraph.EndTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text);
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
            return string.Format("{0}:{1:00}:{2:00}:{3:00}AM", timeCode.Hours, timeCode.Minutes, timeCode.Seconds, MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds));
        }
    }
}