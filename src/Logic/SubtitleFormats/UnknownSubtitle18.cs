// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle18.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 18.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 18.
    /// </summary>
    public class UnknownSubtitle18 : SubtitleFormat
    {
        // 0001 01:00:15:08 01:00:18:05
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d\d\d\d \d\d:\d\d:\d\d:\d\d \d\d:\d\d:\d\d:\d\d$", RegexOptions.Compiled);

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
                return "Unknown 18";
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
            // 0001 01:00:15:08 01:00:18:05
            // PUHDASTA LÄHDEVETTÄ
            // SUORAAN OVELLE TUOTUNA.
            // 0002 01:00:18:07 01:00:20:18
            // MAKU, JONKA MUISTAT LAPSUUDESTA.
            const string paragraphWriteFormat = "{3:0000} {0} {1} \r\n\r\n{2}\r\n";

            StringBuilder sb = new StringBuilder();
            int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, MillisecondsToFramesMaxFrameRate(p.StartTime.Milliseconds));
                string timeOut = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, MillisecondsToFramesMaxFrameRate(p.EndTime.Milliseconds));
                sb.AppendLine(string.Format(paragraphWriteFormat, startTime, timeOut, p.Text, count));
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
            }

            if (!string.IsNullOrWhiteSpace(this._text.ToString()))
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
                // 0001 01:00:15:08 01:00:18:05
                try
                {
                    string start = line.Substring(5, 11);
                    string[] parts = start.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    int startHours = int.Parse(parts[0]);
                    int startMinutes = int.Parse(parts[1]);
                    int startSeconds = int.Parse(parts[2]);
                    int startMilliseconds = FramesToMillisecondsMax999(int.Parse(parts[3]));

                    string end = line.Substring(17, 11);
                    parts = end.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    int endHours = int.Parse(parts[0]);
                    int endMinutes = int.Parse(parts[1]);
                    int endSeconds = int.Parse(parts[2]);
                    int endMilliseconds = FramesToMillisecondsMax999(int.Parse(parts[3]));

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
                        this._expecting = ExpectingLine.BlankBeforeText;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        this._errorCount++;
                    }

                    break;
                case ExpectingLine.BlankBeforeText:
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        this._text = new StringBuilder();
                        this._expecting = ExpectingLine.Text;
                    }
                    else
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
                    else
                    {
                        this._paragraph.Text = this._text.ToString().Trim();
                        subtitle.Paragraphs.Add(this._paragraph);
                        this._paragraph = new Paragraph();
                        this._expecting = ExpectingLine.TimeCodes;
                        this._text = new StringBuilder();
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
            /// The blank before text.
            /// </summary>
            BlankBeforeText, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}