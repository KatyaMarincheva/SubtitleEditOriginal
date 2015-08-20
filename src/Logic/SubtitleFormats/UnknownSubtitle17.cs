// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle17.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 17.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 17.
    /// </summary>
    public class UnknownSubtitle17 : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\d\d:\d\d:\d\d:\d\d", RegexOptions.Compiled);

        /// <summary>
        /// The regex number.
        /// </summary>
        private static readonly Regex RegexNumber = new Regex(@"^\[\d+\]$", RegexOptions.Compiled);

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
                return "Unknown 17";
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
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            string s = sb.ToString();
            if (!s.Contains("[HEADER]") || !s.Contains("[BODY]"))
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
            // [1]
            // 01:00:21:20
            // 01:00:23:17
            // [I]Pysy kanavalla,
            // [I]koska myöhemmin tänään
            const string paragraphWriteFormat = "[{4}]{3}{0}{3}{1}{3}{2}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"[HEADER]
SUBTITLING_COMPANY=Softitler Net, Inc.
TIME_FORMAT=NTSC
CLIENT=UNIVERSAL
LANGUAGE=Finnish
DATE=5/28/2007
JOB_ID=89972
JOB_TYPE=Feature
TITLE=Notting Hill
SUBNAME=BD TRTL
YEAR=1999
DIGITAL_CINEMA=YES
[/HEADER]
[BODY]");
            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                count++;
                string text = p.Text.Replace("<i>", "[I]").Replace("</i>", "[/I]");
                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), text, Environment.NewLine, count));
            }

            sb.AppendLine("[/BODY]");
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
            Paragraph paragraph = null;
            ExpectingLine expecting = ExpectingLine.Number;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string l in lines)
            {
                string line = l.Replace("[L]", string.Empty).Replace("[N]", string.Empty).TrimEnd();
                if (RegexNumber.IsMatch(line))
                {
                    if (paragraph != null)
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }

                    paragraph = new Paragraph();
                    expecting = ExpectingLine.TimeStart;
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeStart && RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            paragraph.StartTime = tc;
                            expecting = ExpectingLine.TimeEnd;
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.Number;
                        }
                    }
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeEnd && RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            paragraph.EndTime = tc;
                            expecting = ExpectingLine.Text;
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.Number;
                        }
                    }
                }
                else
                {
                    if (paragraph != null && expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string s = line;
                            if (line.StartsWith("[i]", StringComparison.OrdinalIgnoreCase))
                            {
                                s = "<i>" + s.Remove(0, 3);
                                if (s.EndsWith("[/i]", StringComparison.OrdinalIgnoreCase))
                                {
                                    s = s.Remove(s.Length - 4, 4);
                                }

                                s += "</i>";
                            }

                            s = s.Replace("[I]", "<i>");
                            s = s.Replace("[/I]", "</i>");
                            s = s.Replace("[P]", string.Empty);
                            s = s.Replace("[/P]", string.Empty);
                            s = s.Replace("\0", string.Empty);
                            paragraph.Text = (paragraph.Text + Environment.NewLine + s).Trim();
                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                subtitle.Renumber();
                                return;
                            }
                        }
                    }
                }
            }

            if (paragraph != null && !string.IsNullOrEmpty(paragraph.Text))
            {
                paragraph.Text = paragraph.Text.Replace("[/BODY]", string.Empty).Trim();
                subtitle.Paragraphs.Add(paragraph);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTimeCode(TimeCode time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string[] parts)
        {
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            int milliseconds = (int)((TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate) * int.Parse(frames));
            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), milliseconds);
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
            /// The time start.
            /// </summary>
            TimeStart, 

            /// <summary>
            /// The time end.
            /// </summary>
            TimeEnd, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}