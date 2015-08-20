// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MidwayInscriberCGX.cs" company="">
//   
// </copyright>
// <summary>
//   The midway inscriber cgx.
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
    /// The midway inscriber cgx.
    /// </summary>
    public class MidwayInscriberCGX : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"<\d\d:\d\d:\d\d:\d\d> <\d\d:\d\d:\d\d:\d\d>$", RegexOptions.Compiled);

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
                return "Midway Inscriber CG-X";
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
            const string writeFormat = "{3} <{0}> <{1}>{2}";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(writeFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), Environment.NewLine, HtmlUtil.RemoveHtmlTags(p.Text, true)));

                // Var vi bedre end japanerne
                // eller bare mere heldige? <12:03:29:03> <12:03:35:06>
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

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.Append(line);
            }

            if (!sb.ToString().Contains("> <"))
            {
                return;
            }

            // Var vi bedre end japanerne
            // eller bare mere heldige? <12:03:29:03> <12:03:35:06>
            subtitle.Paragraphs.Clear();
            sb.Clear();
            char[] splitChar = { ':' };
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (regexTimeCodes.IsMatch(line))
                    {
                        int idx = regexTimeCodes.Match(line).Index;
                        string temp = line.Substring(0, idx).Trim();
                        sb.AppendLine(temp);

                        string start = line.Substring(idx + 1, 11);
                        string end = line.Substring(idx + 15, 11);

                        string[] startParts = start.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
                        string[] endParts = end.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
                        if (startParts.Length == 4 && endParts.Length == 4)
                        {
                            Paragraph p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), sb.ToString().Trim());
                            subtitle.Paragraphs.Add(p);
                        }

                        sb.Clear();
                    }
                    else
                    {
                        sb.AppendLine(line.Trim());
                    }
                }

                if (sb.Length > 1000)
                {
                    return;
                }
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
            // 00:50:39:13 (last is frame)
            return time.ToHHMMSSFF();
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
            // 00:00:07:12
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            int milliseconds = (int)((1000 / Configuration.Settings.General.CurrentFrameRate) * frames);
            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            return new TimeCode(hour, minutes, seconds, milliseconds);
        }
    }
}