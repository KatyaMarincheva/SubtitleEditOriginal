// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle53.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 53.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 53.
    /// </summary>
    public class UnknownSubtitle53 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d\:\d\d\:\d\d\:\d\d [^ ]+", RegexOptions.Compiled);

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
                return "Unknown 53";
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
            // 10:56:54:12 FEATURING BRIAN LORENTE AND THE
            // 10:56:59:18 USUAL SUSPECTS.
            // 10:57:15:18 \M
            // 10:57:20:07 >> HOW WE DOING TONIGHT,
            // 10:57:27:17 MICHIGAN?
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = HtmlUtil.RemoveHtmlTags(p.Text).Replace("♪", "\\M");
                sb.AppendLine(EncodeTimeCode(p.StartTime) + " " + text);
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
                if (regexTimeCodes.Match(s).Success)
                {
                    if (p != null && !string.IsNullOrEmpty(p.Text))
                    {
                        subtitle.Paragraphs.Add(p);
                    }

                    p = new Paragraph();

                    try
                    {
                        string[] arr = s.Substring(0, 11).Split(':');
                        if (arr.Length == 4)
                        {
                            int hours = int.Parse(arr[0]);
                            int minutes = int.Parse(arr[1]);
                            int seconds = int.Parse(arr[2]);
                            int frames = int.Parse(arr[3]);
                            p.StartTime = new TimeCode(hours, minutes, seconds, FramesToMillisecondsMax999(frames));
                            string text = s.Remove(0, 11).Trim();
                            p.Text = text;
                            if (text.Length > 1 && Utilities.IsInteger(text.Substring(0, 2)))
                            {
                                this._errorCount++;
                            }
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

            if (p != null && !string.IsNullOrEmpty(p.Text))
            {
                subtitle.Paragraphs.Add(p);
            }

            int index = 1;
            foreach (Paragraph paragraph in subtitle.Paragraphs)
            {
                paragraph.Text = paragraph.Text.Replace("\\M", "♪");

                Paragraph next = subtitle.GetParagraphOrDefault(index);
                if (next != null)
                {
                    paragraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                }
                else
                {
                    paragraph.EndTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(paragraph.Text);
                }

                index++;
            }

            subtitle.RemoveEmptyLines();
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
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", timeCode.Hours, timeCode.Minutes, timeCode.Seconds, MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds));
        }
    }
}