// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TMPlayer.cs" company="">
//   
// </copyright>
// <summary>
//   The tm player.
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
    /// The tm player.
    /// </summary>
    public class TMPlayer : SubtitleFormat
    {
        /// <summary>
        /// The regex.
        /// </summary>
        private static readonly Regex regex = new Regex(@"^\d+:\d\d:\d\d[: ].*$", RegexOptions.Compiled); // accept a " " instead of the last ":" too

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
                return "TMPlayer";
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

            if (subtitle.Paragraphs.Count > 4)
            {
                bool allStartWithNumber = true;
                foreach (Paragraph p in subtitle.Paragraphs)
                {
                    if (p.Text.Length > 1 && !Utilities.IsInteger(p.Text.Substring(0, 2)))
                    {
                        allStartWithNumber = false;
                        break;
                    }
                }

                if (allStartWithNumber)
                {
                    return false;
                }
            }

            if (subtitle.Paragraphs.Count > this._errorCount)
            {
                if (new UnknownSubtitle33().IsMine(lines, fileName) || new UnknownSubtitle36().IsMine(lines, fileName))
                {
                    return false;
                }

                return true;
            }

            return false;
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
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                text = text.Replace(Environment.NewLine, "|");
                sb.AppendLine(string.Format("{0:00}:{1:00}:{2:00}:{3}", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, text));
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
        { // 0:02:36:You've returned to the village|after 2 years, Shekhar.
            // 00:00:50:America has made my fortune.
            this._errorCount = 0;
            foreach (string line in lines)
            {
                bool success = false;
                if (line.IndexOf(':') > 0 && regex.Match(line).Success)
                {
                    try
                    {
                        string s = line;
                        if (line.Length > 9 && line[8] == ' ')
                        {
                            s = line.Substring(0, 8) + ":" + line.Substring(9);
                        }

                        string[] parts = s.Split(':');
                        if (parts.Length > 3)
                        {
                            int hours = int.Parse(parts[0]);
                            int minutes = int.Parse(parts[1]);
                            int seconds = int.Parse(parts[2]);
                            string text = string.Empty;
                            for (int i = 3; i < parts.Length; i++)
                            {
                                if (text.Length == 0)
                                {
                                    text = parts[i];
                                }
                                else
                                {
                                    text += ":" + parts[i];
                                }
                            }

                            text = text.Replace("|", Environment.NewLine);
                            TimeCode start = new TimeCode(hours, minutes, seconds, 0);
                            double duration = Utilities.GetOptimalDisplayMilliseconds(text);
                            TimeCode end = new TimeCode(start.TotalMilliseconds + duration);

                            Paragraph p = new Paragraph(start, end, text);
                            subtitle.Paragraphs.Add(p);
                            success = true;
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }

                if (!success)
                {
                    this._errorCount++;
                }
            }

            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                Paragraph next = subtitle.GetParagraphOrDefault(index + 1);
                if (next != null && next.StartTime.TotalMilliseconds <= p.EndTime.TotalMilliseconds)
                {
                    p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                }

                index++;
                p.Number = index;
            }
        }
    }
}