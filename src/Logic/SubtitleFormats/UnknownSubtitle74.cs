// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle74.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 74.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 74.
    /// </summary>
    public class UnknownSubtitle74 : SubtitleFormat
    {
        // 07:02:27
        // >> GOOD MORNING AND WELCOME TO THE FALL 2014 COMMENCEMENT CEREMONY, A TIME TO RECOGNIZE OUR GRADUATING SENIORS.
        // 07:02:43
        // DURING YOUR TIME HERE, MICHIGAN STATE UNIVERSITY HAS POLLEDLY RECAST ITS LAND-GRANT MISSION TO MEET NEW CHALLENGES AND OPPORTUNITIES AND TO INNOVATE OUR FUTURE.
        // 07:02:54
        // OUR PLANS AND ACTIONS STEM FROM OUR CORE INTERWOVEN VALUES OF QUALITY, INCLUSION AND CONNECTIVITY.
        // 07:03:02
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d$", RegexOptions.Compiled);

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
                return "Unknown 74";
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
            const string paragraphWriteFormat = "{0}\r\n{1}\r\n";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendFormat(paragraphWriteFormat, GetTimeCode(p), p.Text);
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
            subtitle.Paragraphs.Clear();
            int i = 0;
            Paragraph paragraph = null;
            while (i < lines.Count)
            {
                string line = lines[i].TrimEnd();
                string next = string.Empty;
                if (i + 1 < lines.Count)
                {
                    next = lines[i + 1];
                }

                if (line.Length == 8 && line[2] == ':' && RegexTimeCodes.IsMatch(line) && !RegexTimeCodes.IsMatch(next))
                {
                    paragraph = new Paragraph();
                    if (TryReadTimeCodesLine(line, paragraph))
                    {
                        paragraph.Text = next;
                        if (!string.IsNullOrWhiteSpace(paragraph.Text))
                        {
                            subtitle.Paragraphs.Add(paragraph);
                            i++;
                        }
                        else
                        {
                            this._errorCount++;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        this._errorCount++;
                    }
                }
                else
                {
                    if (paragraph != null && paragraph.Text.Length < 500)
                    {
                        paragraph.Text = paragraph.Text + Environment.NewLine + line;
                    }
                    else
                    {
                        this._errorCount++;
                        return;
                    }
                }

                i++;
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                p.Text = p.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }

            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                index++;
                Paragraph nextParagraph = subtitle.GetParagraphOrDefault(index);
                if (nextParagraph != null)
                {
                    p.EndTime.TotalMilliseconds = nextParagraph.StartTime.TotalMilliseconds - 1;
                }
                else
                {
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + 2500;
                }

                p.Text = p.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetTimeCode(Paragraph p)
        {
            int seconds = p.StartTime.Seconds;
            if (p.StartTime.Milliseconds >= 500)
            {
                seconds++;
            }

            return string.Format("{0:00}:{1:00}:{2:00}", p.StartTime.Hours, p.StartTime.Minutes, seconds);
        }

        /// <summary>
        /// The try read time codes line.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TryReadTimeCodesLine(string line, Paragraph paragraph)
        {
            string[] parts = line.Split(':');
            try
            {
                int startHours = int.Parse(parts[0]);
                int startMinutes = int.Parse(parts[1]);
                int startSeconds = int.Parse(parts[2]);
                paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}