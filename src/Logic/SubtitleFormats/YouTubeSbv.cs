// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YouTubeSbv.cs" company="">
//   
// </copyright>
// <summary>
//   YouTube "SubViewer" format... I think YouTube tried to add "SubViewer 2.0" support but instread they created their
//   own format... nice ;)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     YouTube "SubViewer" format... I think YouTube tried to add "SubViewer 2.0" support but instread they created their
    ///     own format... nice ;)
    /// </summary>
    public class YouTubeSbv : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^-?\d+:-?\d+:-?\d+[:,.]-?\d+,\d+:-?\d+:-?\d+[:,.]-?\d+$", RegexOptions.Compiled);

        /// <summary>
        /// The _expecting.
        /// </summary>
        private ExpectingLine _expecting = ExpectingLine.TimeCodes;

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
                return ".sbv";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "YouTube sbv";
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
            const string paragraphWriteFormat = "{0},{1}\r\n{2}\r\n\r\n";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendFormat(paragraphWriteFormat, FormatTime(p.StartTime), FormatTime(p.EndTime), p.Text);
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
            // 0:00:07.500,0:00:13.500
            // In den Bergen über Musanze in Ruanda feiert die Trustbank (Kreditnehmer-Gruppe)  "Trususanze" ihren Erfolg.

            // 0:00:14.000,0:00:17.000
            // Indem sie ihre Zukunft einander anvertraut haben, haben sie sich
            this._paragraph = new Paragraph();
            this._expecting = ExpectingLine.TimeCodes;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].TrimEnd();
                string next = string.Empty;
                if (i + 1 < lines.Count)
                {
                    next = lines[i + 1];
                }

                // A new line is missing between two paragraphs (buggy srt file)
                if (this._expecting == ExpectingLine.Text && i + 1 < lines.Count && this._paragraph != null && !string.IsNullOrEmpty(this._paragraph.Text) && RegexTimeCodes.IsMatch(lines[i]))
                {
                    this.ReadLine(subtitle, string.Empty, string.Empty);
                }

                this.ReadLine(subtitle, line, next);
            }

            if (!string.IsNullOrWhiteSpace(this._paragraph.Text))
            {
                subtitle.Paragraphs.Add(this._paragraph);
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                p.Text = p.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The format time.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatTime(TimeCode timeCode)
        {
            return string.Format("{0}:{1:00}:{2:00}.{3:000}", timeCode.Hours, timeCode.Minutes, timeCode.Seconds, timeCode.Milliseconds);
        }

        /// <summary>
        /// The is text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsText(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || Utilities.IsInteger(text) || RegexTimeCodes.IsMatch(text))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The remove bad chars.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveBadChars(string line)
        {
            return line.Replace('\0', ' ');
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
            line = line.Replace('.', ':');
            line = line.Replace('،', ',');
            line = line.Replace('¡', ':');

            if (RegexTimeCodes.IsMatch(line))
            {
                line = line.Replace(',', ':');
                string[] parts = line.Replace(" ", string.Empty).Split(':', ',');
                try
                {
                    int startHours = int.Parse(parts[0]);
                    int startMinutes = int.Parse(parts[1]);
                    int startSeconds = int.Parse(parts[2]);
                    int startMilliseconds = int.Parse(parts[3]);
                    int endHours = int.Parse(parts[4]);
                    int endMinutes = int.Parse(parts[5]);
                    int endSeconds = int.Parse(parts[6]);
                    int endMilliseconds = int.Parse(parts[7]);
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
        /// <param name="next">
        /// The next.
        /// </param>
        private void ReadLine(Subtitle subtitle, string line, string next)
        {
            switch (this._expecting)
            {
                case ExpectingLine.TimeCodes:
                    if (TryReadTimeCodesLine(line, this._paragraph))
                    {
                        this._paragraph.Text = string.Empty;
                        this._expecting = ExpectingLine.Text;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        this._errorCount++;
                    }

                    break;
                case ExpectingLine.Text:
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (this._paragraph.Text.Length > 0)
                        {
                            this._paragraph.Text += Environment.NewLine;
                        }

                        this._paragraph.Text += RemoveBadChars(line).TrimEnd();
                    }
                    else if (IsText(next))
                    {
                        if (this._paragraph.Text.Length > 0)
                        {
                            this._paragraph.Text += Environment.NewLine;
                        }

                        this._paragraph.Text += RemoveBadChars(line).TrimEnd();
                    }
                    else
                    {
                        subtitle.Paragraphs.Add(this._paragraph);
                        this._paragraph = new Paragraph();
                        this._expecting = ExpectingLine.TimeCodes;
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