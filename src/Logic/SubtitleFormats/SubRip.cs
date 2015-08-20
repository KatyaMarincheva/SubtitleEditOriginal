// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubRip.cs" company="">
//   
// </copyright>
// <summary>
//   The sub rip.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The sub rip.
    /// </summary>
    public class SubRip : SubtitleFormat
    {
        /// <summary>
        /// The name of format.
        /// </summary>
        public const string NameOfFormat = "SubRip";

        /// <summary>
        /// The _regex time codes.
        /// </summary>
        private static readonly Regex _regexTimeCodes = new Regex(@"^-?\d+:-?\d+:-?\d+[:,]-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+[:,]-?\d+$", RegexOptions.Compiled);

        /// <summary>
        /// The _regex time codes 2.
        /// </summary>
        private static readonly Regex _regexTimeCodes2 = new Regex(@"^\d+:\d+:\d+,\d+\s*-->\s*\d+:\d+:\d+,\d+$", RegexOptions.Compiled);

        /// <summary>
        /// The _errors.
        /// </summary>
        private StringBuilder _errors;

        /// <summary>
        /// The _expecting.
        /// </summary>
        private ExpectingLine _expecting = ExpectingLine.Number;

        /// <summary>
        /// The _last paragraph.
        /// </summary>
        private Paragraph _lastParagraph;

        /// <summary>
        /// The _line number.
        /// </summary>
        private int _lineNumber;

        /// <summary>
        /// The _paragraph.
        /// </summary>
        private Paragraph _paragraph;

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public string Errors { get; private set; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".srt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return NameOfFormat;
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
        /// Gets the alternate extensions.
        /// </summary>
        public override List<string> AlternateExtensions
        {
            get
            {
                return new List<string> { ".wsrt" };
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
            if (lines.Count > 0 && lines[0].StartsWith("WEBVTT", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            this.Errors = null;
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
            const string paragraphWriteFormat = "{0}\r\n{1} --> {2}\r\n{3}\r\n\r\n";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string s = p.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine).Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                sb.AppendFormat(paragraphWriteFormat, p.Number, p.StartTime, p.EndTime, s);
            }

            return sb.ToString().Trim() + Environment.NewLine + Environment.NewLine;
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
            bool doRenum = false;
            this._errors = new StringBuilder();
            this._lineNumber = 0;

            this._paragraph = new Paragraph();
            this._expecting = ExpectingLine.Number;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                this._lineNumber++;
                string line = lines[i].TrimEnd();
                line = line.Trim('\u007F'); // 127=delete acscii

                string next = string.Empty;
                if (i + 1 < lines.Count)
                {
                    next = lines[i + 1];
                }

                string nextNext = string.Empty;
                if (i + 2 < lines.Count)
                {
                    nextNext = lines[i + 2];
                }

                // A new line is missing between two paragraphs (buggy srt file)
                if (this._expecting == ExpectingLine.Text && i + 1 < lines.Count && this._paragraph != null && !string.IsNullOrEmpty(this._paragraph.Text) && Utilities.IsInteger(line) && _regexTimeCodes.IsMatch(lines[i + 1]))
                {
                    this.ReadLine(subtitle, string.Empty, string.Empty, string.Empty);
                }

                if (this._expecting == ExpectingLine.Number && _regexTimeCodes.IsMatch(line))
                {
                    this._expecting = ExpectingLine.TimeCodes;
                    doRenum = true;
                }

                this.ReadLine(subtitle, line, next, nextNext);
            }

            if (this._paragraph.EndTime.TotalMilliseconds > this._paragraph.StartTime.TotalMilliseconds)
            {
                subtitle.Paragraphs.Add(this._paragraph);
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                p.Text = p.Text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }

            if (doRenum)
            {
                subtitle.Renumber();
            }

            this.Errors = this._errors.ToString();
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
            if (string.IsNullOrWhiteSpace(text) || Utilities.IsInteger(text) || _regexTimeCodes.IsMatch(text))
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
            line = line.Replace('،', ',');
            line = line.Replace('', ',');
            line = line.Replace('¡', ',');

            const string defaultSeparator = " --> ";

            // Fix some badly formatted separator sequences - anything can happen if you manually edit ;)
            line = line.Replace(" -> ", defaultSeparator); // I've seen this
            line = line.Replace(" - > ", defaultSeparator);
            line = line.Replace(" ->> ", defaultSeparator);
            line = line.Replace(" -- > ", defaultSeparator);
            line = line.Replace(" - -> ", defaultSeparator);
            line = line.Replace(" -->> ", defaultSeparator);
            line = line.Replace(" ---> ", defaultSeparator);

            // Removed stuff after timecodes - like subtitle position
            // - example of position info: 00:02:26,407 --> 00:02:31,356  X1:100 X2:100 Y1:100 Y2:100
            if (line.Length > 30 && line[29] == ' ')
            {
                line = line.Substring(0, 29);
            }

            // removes all extra spaces
            line = line.Replace(" ", string.Empty).Replace("-->", defaultSeparator).Trim();

            // Fix a few more cases of wrong time codes, seen this: 00.00.02,000 --> 00.00.04,000
            line = line.Replace('.', ':');
            if (line.Length >= 29 && (line[8] == ':' || line[8] == ';'))
            {
                line = line.Substring(0, 8) + ',' + line.Substring(8 + 1);
            }

            if (line.Length >= 29 && line.Length <= 30 && (line[25] == ':' || line[25] == ';'))
            {
                line = line.Substring(0, 25) + ',' + line.Substring(25 + 1);
            }

            if (_regexTimeCodes.IsMatch(line) || _regexTimeCodes2.IsMatch(line))
            {
                string[] parts = line.Replace("-->", ":").Replace(" ", string.Empty).Split(':', ',');
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
                    if (parts[0].StartsWith('-') && paragraph.StartTime.TotalMilliseconds > 0)
                    {
                        paragraph.StartTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds * -1;
                    }

                    paragraph.EndTime = new TimeCode(endHours, endMinutes, endSeconds, endMilliseconds);
                    if (parts[4].StartsWith('-') && paragraph.EndTime.TotalMilliseconds > 0)
                    {
                        paragraph.EndTime.TotalMilliseconds = paragraph.EndTime.TotalMilliseconds * -1;
                    }

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
        /// <param name="nextNext">
        /// The next next.
        /// </param>
        private void ReadLine(Subtitle subtitle, string line, string next, string nextNext)
        {
            switch (this._expecting)
            {
                case ExpectingLine.Number:
                    int number;
                    if (int.TryParse(line, out number))
                    {
                        this._paragraph.Number = number;
                        this._expecting = ExpectingLine.TimeCodes;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (this._lastParagraph != null && nextNext != null && (this._lastParagraph.Number + 1).ToString(CultureInfo.InvariantCulture) == nextNext)
                        {
                            this._lastParagraph.Text = (this._lastParagraph.Text + Environment.NewLine + line.Trim()).Trim();
                        }
                        else
                        {
                            if (this._errors.Length < 2000)
                            {
                                this._errors.AppendLine(string.Format(Configuration.Settings.Language.Main.LineNumberXExpectedNumberFromSourceLineY, this._lineNumber, line));
                            }

                            this._errorCount++;
                        }
                    }

                    break;
                case ExpectingLine.TimeCodes:
                    if (TryReadTimeCodesLine(line, this._paragraph))
                    {
                        this._paragraph.Text = string.Empty;
                        this._expecting = ExpectingLine.Text;
                    }
                    else if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (this._errors.Length < 2000)
                        {
                            this._errors.AppendLine(string.Format(Configuration.Settings.Language.Main.LineNumberXErrorReadingTimeCodeFromSourceLineY, this._lineNumber, line));
                        }

                        this._errorCount++;
                        this._expecting = ExpectingLine.Number; // lets go to next paragraph
                    }

                    break;
                case ExpectingLine.Text:
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (this._paragraph.Text.Length > 0)
                        {
                            this._paragraph.Text += Environment.NewLine;
                        }

                        this._paragraph.Text += RemoveBadChars(line).TrimEnd().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    }
                    else if (IsText(next))
                    {
                        if (this._paragraph.Text.Length > 0)
                        {
                            this._paragraph.Text += Environment.NewLine;
                        }

                        this._paragraph.Text += RemoveBadChars(line).TrimEnd().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    }
                    else if (string.IsNullOrEmpty(line) && string.IsNullOrEmpty(this._paragraph.Text))
                    {
                        this._paragraph.Text = string.Empty;
                        if (!string.IsNullOrEmpty(next) && (Utilities.IsInteger(next) || _regexTimeCodes.IsMatch(next)))
                        {
                            subtitle.Paragraphs.Add(this._paragraph);
                            this._lastParagraph = this._paragraph;
                            this._paragraph = new Paragraph();
                            this._expecting = ExpectingLine.Number;
                        }
                    }
                    else
                    {
                        subtitle.Paragraphs.Add(this._paragraph);
                        this._lastParagraph = this._paragraph;
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