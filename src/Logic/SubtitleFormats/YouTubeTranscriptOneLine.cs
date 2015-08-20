// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YouTubeTranscriptOneLine.cs" company="">
//   
// </copyright>
// <summary>
//   The you tube transcript one line.
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
    /// The you tube transcript one line.
    /// </summary>
    public class YouTubeTranscriptOneLine : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d{1,3}:\d\d.+$", RegexOptions.Compiled);

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
                return "YouTube Transcript one line";
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

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

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
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("{0}{1}", EncodeTimeCode(p.StartTime), HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, " "))));
                index++;
            }

            return sb.ToString();
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
            foreach (string line in lines)
            {
                if (regexTimeCodes.IsMatch(line))
                {
                    int splitter = line.IndexOf(':') + 3;
                    string text = line.Remove(0, splitter);
                    Paragraph p = new Paragraph(DecodeTimeCode(line.Substring(0, splitter)), new TimeCode(0, 0, 0, 0), text);
                    subtitle.Paragraphs.Add(p);
                    text = text.Trim().Trim('–', '.', ';', ':').Trim();
                    if (text.Length > 0 && char.IsDigit(text[0]))
                    {
                        this._errorCount++;
                    }
                }
                else
                {
                    this._errorCount += 2;
                }
            }

            foreach (Paragraph p2 in subtitle.Paragraphs)
            {
                p2.Text = Utilities.AutoBreakLine(p2.Text);
            }

            subtitle.RecalculateDisplayTimes(Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds, null);
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
            return string.Format("{0}:{1:00}", time.Hours * 60 + time.Minutes, time.Seconds);
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string s)
        {
            string[] parts = s.Split(':');

            string minutes = parts[0];
            string seconds = parts[1];

            return new TimeCode(0, int.Parse(minutes), int.Parse(seconds), 0);
        }
    }
}