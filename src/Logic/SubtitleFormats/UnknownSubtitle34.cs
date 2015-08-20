// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle34.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 34.
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
    /// The unknown subtitle 34.
    /// </summary>
    public class UnknownSubtitle34 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d\:\d\d\:\d\d\t[^ ]+", RegexOptions.Compiled);

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
                return "Unknown 34";
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
            // 08:55:05  >>> WELCOME BACK.
            // 08:59:49  """OFF THE RECORD"" STARTS RIGHT NOW."
            // 08:59:51  ON THE PANEL THIS WEEK WE HAVE EMILY LAWLER AND ZACH
            // 08:59:54  "GORCHOW, ALONG WITH RON DZWONKOWSKI."
            // HERE IS THE RUNDOWN.
            // A POSSIBLE REDO OF THE EM LAW IF VOTERS REJECT IT.
            // 09:00:03  AND MIKE DUGAN AND LATER GENE CLEM IS DISCUSSING THIS
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string[] lines = HtmlUtil.RemoveHtmlTags(p.Text).SplitToLines();
                if (lines.Length > 0)
                {
                    sb.AppendLine(EncodeTimeCode(p.StartTime) + "\t" + lines[0]);
                    for (int i = 1; i < lines.Length; i++)
                    {
                        sb.AppendLine("\t" + lines[i]);
                    }
                }
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
                if (regexTimeCodes.Match(s).Success && !UnknownSubtitle59.RegexTimeCodes.IsMatch(s))
                {
                    if (p != null && !string.IsNullOrEmpty(p.Text))
                    {
                        subtitle.Paragraphs.Add(p);
                    }

                    p = new Paragraph();

                    try
                    {
                        string[] arr = s.Substring(0, 8).Split(':');
                        if (arr.Length == 3)
                        {
                            int hours = int.Parse(arr[0]);
                            int minutes = int.Parse(arr[1]);
                            int seconds = int.Parse(arr[2]);
                            p.StartTime = new TimeCode(hours, minutes, seconds, 0);
                            string text = s.Remove(0, 8).Trim();
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
                else if (line.StartsWith("\t") && p != null)
                {
                    if (p.Text.Length > 1000)
                    {
                        this._errorCount += 100;
                        return;
                    }

                    p.Text = (p.Text + Environment.NewLine + s).Trim();
                }
                else if (s.Length > 0 && !Utilities.IsInteger(s))
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
            int seconds = (int)(timeCode.Seconds + timeCode.Milliseconds / 1000 + 0.5);
            return string.Format("{0:00}:{1:00}:{2:00}", timeCode.Hours, timeCode.Minutes, seconds);
        }
    }
}