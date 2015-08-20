// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle2.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // Subtitle number: 1
    // Start time (or frames): 00:00:48,862:0000001222
    // End time (or frames): 00:00:50,786:0000001270
    // Subtitle text: In preajma lacului Razel,
    /// <summary>
    /// The unknown subtitle 2.
    /// </summary>
    public class UnknownSubtitle2 : SubtitleFormat
    {
        /// <summary>
        /// The _expecting.
        /// </summary>
        private ExpectingLine _expecting = ExpectingLine.Number;

        /// <summary>
        /// The _paragraph.
        /// </summary>
        private Paragraph _paragraph;

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
                return "Unknown 2";
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
            // Subtitle number: 1
            // Start time (or frames): 00:00:48,862:0000001222
            // End time (or frames): 00:00:50,786:0000001270
            // Subtitle text: In preajma lacului Razel,
            const string paragraphWriteFormat = "Subtitle number: {0}\r\nStart time (or frames): {1}\r\nEnd time (or frames): {2}\r\nSubtitle text: {3}\r\n";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format("{0:00}:{1:00}:{2:00},{3:00}:0000000000", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, p.StartTime.Milliseconds / 10);
                string timeOut = string.Format("{0:00}:{1:00}:{2:00},{3:00}:0000000000", p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, p.EndTime.Milliseconds / 10);
                sb.AppendLine(string.Format(paragraphWriteFormat, p.Number, startTime, timeOut, p.Text.Replace(Environment.NewLine, "|")));
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
            this._expecting = ExpectingLine.Number;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                this.ReadLine(subtitle, line);
            }

            if (!string.IsNullOrWhiteSpace(this._paragraph.Text))
            {
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
        /// <param name="start">
        /// The start.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool TryReadTimeCodesLine(string line, Paragraph paragraph, bool start)
        {
            line = line.Trim();

            // 00:00:48,862:0000001222
            line = line.Replace(",", ":");
            string[] parts = line.Split(':');
            try
            {
                int startHours = int.Parse(parts[0]);
                int startMinutes = int.Parse(parts[1]);
                int startSeconds = int.Parse(parts[2]);
                int startMilliseconds = int.Parse(parts[3]);

                if (start)
                {
                    paragraph.StartTime = new TimeCode(startHours, startMinutes, startSeconds, startMilliseconds);
                }
                else
                {
                    paragraph.EndTime = new TimeCode(startHours, startMinutes, startSeconds, startMilliseconds);
                }

                return true;
            }
            catch
            {
                return false;
            }
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
            // Subtitle number: 1
            // Start time (or frames): 00:00:48,862:0000001222
            // End time (or frames): 00:00:50,786:0000001270
            // Subtitle text: In preajma lacului Razel,
            switch (this._expecting)
            {
                case ExpectingLine.Number:
                    if (line.StartsWith("Subtitle number: "))
                    {
                        this._expecting = ExpectingLine.StartTime;
                    }

                    break;
                case ExpectingLine.StartTime:
                    if (line.StartsWith("Start time (or frames): "))
                    {
                        TryReadTimeCodesLine(line.Substring(23), this._paragraph, true);
                        this._expecting = ExpectingLine.EndTime;
                    }

                    break;
                case ExpectingLine.EndTime:
                    if (line.StartsWith("End time (or frames): "))
                    {
                        TryReadTimeCodesLine(line.Substring(21), this._paragraph, false);
                        this._expecting = ExpectingLine.Text;
                    }

                    break;
                case ExpectingLine.Text:
                    if (line.StartsWith("Subtitle text: "))
                    {
                        string text = line.Substring(14).Trim();
                        text = text.Replace("|", Environment.NewLine);
                        this._paragraph.Text = text;
                        subtitle.Paragraphs.Add(this._paragraph);
                        this._paragraph = new Paragraph();
                        this._expecting = ExpectingLine.Number;
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
            /// The number.
            /// </summary>
            Number, 

            /// <summary>
            /// The start time.
            /// </summary>
            StartTime, 

            /// <summary>
            /// The end time.
            /// </summary>
            EndTime, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}