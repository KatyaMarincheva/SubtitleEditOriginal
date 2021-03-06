﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle36.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 36.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    // 00:00:31:17           ,           00:00:35:12           , Signori e Signorine come state?|Va tutto bene?

    // 00:00:35:24           ,           00:00:40:17           ,Oggi riceveremo un |grande artista che viene dall’Africa.

    // 00:00:40:20           ,           00:00:44:12           ,Indovinate come si chiama?|Indovinate come si chiama?

    // 00:00:44:15           ,           00:00:48:24           ,Si chiama Bella Balde.

    // 00:00:49:04           ,           00:00:50:16           ,Grazie Signore e Signori.

    // 00:00:50:18           ,           00:00:52:24           ,per ricevere questo grande artista| che viene dall’Africa.

    // 00:00:53:00           ,           00:00:55:08           ,Grazie di nuovo,|grazie ancora.

    // ---------------------------------------------------

    // 00:02:13:14           ,           00:02:18:21            ,Stiamo preparando un festival|nel centro scolastico.

    // 00:02:20:16            ,           00:02:24:00           ,Gente di questo quartiere |Un gran festival!
    /// <summary>
    /// The unknown subtitle 36.
    /// </summary>
    public class UnknownSubtitle36 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d+:\d+:\d+:\d+           ,           \d+:\d+:\d+:\d+           ,.*$", RegexOptions.Compiled);

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
                return "Unknown 36";
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
            if (lines.Count > 0 && lines[0] != null && lines[0].StartsWith("{\\rtf1"))
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
            const string paragraphWriteFormat = "{0}           ,           {1}           ,{2}\r\n";
            const string timeFormat = "{0:00}:{1:00}:{2:00}:{3:00}";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string startTime = string.Format(timeFormat, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, MillisecondsToFramesMaxFrameRate(p.StartTime.Milliseconds));
                string endTime = string.Format(timeFormat, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, MillisecondsToFramesMaxFrameRate(p.EndTime.Milliseconds));
                sb.AppendFormat(paragraphWriteFormat, startTime, endTime, HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, " | ")));
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
            int number = 0;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || string.IsNullOrWhiteSpace(line.Trim('-')))
                {
                    continue;
                }

                if (RegexTimeCodes.Match(line).Success)
                {
                    string[] threePart = line.Split(new[] { ',' }, StringSplitOptions.None);
                    Paragraph p = new Paragraph();
                    if (threePart.Length > 2 && line.Length > 58 && GetTimeCode(p.StartTime, threePart[0].Trim()) && GetTimeCode(p.EndTime, threePart[1].Trim()))
                    {
                        number++;
                        p.Number = number;
                        p.Text = line.Remove(0, 57).Trim().Replace(" | ", Environment.NewLine).Replace("|", Environment.NewLine);
                        subtitle.Paragraphs.Add(p);
                    }
                }
                else
                {
                    this._errorCount++;
                }
            }
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="timeString">
        /// The time string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool GetTimeCode(TimeCode timeCode, string timeString)
        {
            try
            {
                string[] timeParts = timeString.Split(':');
                timeCode.Hours = int.Parse(timeParts[0]);
                timeCode.Minutes = int.Parse(timeParts[1]);
                timeCode.Seconds = int.Parse(timeParts[2]);
                timeCode.Milliseconds = FramesToMillisecondsMax999(int.Parse(timeParts[3]));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}