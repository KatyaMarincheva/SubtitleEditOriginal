// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwiftTextLineNumber.cs" company="">
//   
// </copyright>
// <summary>
//   The swift text line number.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The swift text line number.
    /// </summary>
    public class SwiftTextLineNumber : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^SUBTITLE: \d+\s+TIMEIN:\s*[0123456789-]+:[0123456789-]+:[0123456789-]+:[0123456789-]+\s*TIMEOUT:\s*[0123456789-]+:[0123456789-]+:[0123456789-]+:[0123456789-]+$", RegexOptions.Compiled);

        /// <summary>
        /// The _expecting.
        /// </summary>
        private ExpectingLine _expecting = ExpectingLine.TimeCodes;

        /// <summary>
        /// The _paragraph.
        /// </summary>
        private Paragraph _paragraph;

        /// <summary>
        /// The _text.
        /// </summary>
        private StringBuilder _text = new StringBuilder();

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
                return "Swift text line#";
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
            if (lines == null || lines.Count > 2 && !string.IsNullOrEmpty(lines[0]) && lines[0].Contains("{QTtext}"))
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
            // SUBTITLE: 1   TIMEIN: 00:00:00:00 TIMEOUT: 00:00:04:00
            // Voor de oorlog

            // SUBTITLE: 2   TIMEIN: 00:00:05:12 TIMEOUT: 00:00:10:02
            // Ik ben Marie Pinhas. Ik ben geboren
            // in Thessaloniki in Griekenland,

            // SUBTITLE: 3   TIMEIN: 00:00:10:06 TIMEOUT: 00:00:15:17
            // op 6 maart '31,
            // in een heel oude Griekse familie.
            const string paragraphWriteFormat = "SUBTITLE: {1}\tTIMEIN: {0}\tTIMEOUT: {2}\r\n{3}\r\n";

            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, MillisecondsToFramesMaxFrameRate(p.StartTime.Milliseconds));
                string timeOut = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, MillisecondsToFramesMaxFrameRate(p.EndTime.Milliseconds));
                sb.AppendLine(string.Format(paragraphWriteFormat, startTime, count, timeOut, p.Text));
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
            this._paragraph = new Paragraph();
            this._expecting = ExpectingLine.TimeCodes;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                this.ReadLine(subtitle, line);
                if (this._text.Length > 1000)
                {
                    return;
                }
            }

            if (this._text != null && this._text.ToString().TrimStart().Length > 0)
            {
                this._paragraph.Text = this._text.ToString().Trim();
                subtitle.Paragraphs.Add(this._paragraph);
            }

            subtitle.Renumber();
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
            line = line.Trim();
            if (RegexTimeCodes.IsMatch(line))
            {
                // SUBTITLE: 59  TIMEIN: 00:04:28:06 TIMEOUT: 00:04:32:12
                string s = line.Replace("SUBTITLE:", string.Empty).Replace("TIMEIN", string.Empty).Replace("TIMEOUT", string.Empty).Replace(" ", string.Empty).Replace("\t", string.Empty);
                string[] parts = s.Split(':');
                try
                {
                    int startHours = int.Parse(parts[1]);
                    int startMinutes = int.Parse(parts[2]);
                    int startSeconds = int.Parse(parts[3]);
                    int startMilliseconds = FramesToMillisecondsMax999(int.Parse(parts[4]));

                    int endHours = 0;
                    if (parts[5] != "--")
                    {
                        endHours = int.Parse(parts[5]);
                    }

                    int endMinutes = 0;
                    if (parts[6] != "--")
                    {
                        endMinutes = int.Parse(parts[6]);
                    }

                    int endSeconds = 0;
                    if (parts[7] != "--")
                    {
                        endSeconds = int.Parse(parts[7]);
                    }

                    int endMilliseconds = 0;
                    if (parts[8] != "--")
                    {
                        endMilliseconds = FramesToMillisecondsMax999(int.Parse(parts[8]));
                    }

                    paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, startMilliseconds);
                    paragraph.EndTime = new TimeCode(endHours, endMinutes, endSeconds, endMilliseconds);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// The read line.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        private void ReadLine(Subtitle subtitle, string line)
        {
            switch (this._expecting)
            {
                case ExpectingLine.TimeCodes:
                    if (TryReadTimeCodesLine(line, this._paragraph))
                    {
                        this._text = new StringBuilder();
                        this._expecting = ExpectingLine.Text;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        this._errorCount++;
                        this._expecting = ExpectingLine.Text; // lets go to next paragraph
                    }

                    break;
                case ExpectingLine.Text:
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        this._text.AppendLine(line.TrimEnd());
                    }
                    else if (this._paragraph != null && this._paragraph.EndTime.TotalMilliseconds > 0)
                    {
                        this._paragraph.Text = this._text.ToString().Trim();
                        subtitle.Paragraphs.Add(this._paragraph);
                        this._paragraph = new Paragraph();
                        this._expecting = ExpectingLine.TimeCodes;
                    }
                    else
                    {
                        this._errorCount++;
                    }

                    break;
            }
        }

        /// <summary>
        /// The expecting line.
        /// </summary>
        private enum ExpectingLine
        {
            /// <summary>
            /// The time codes.
            /// </summary>
            TimeCodes, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}