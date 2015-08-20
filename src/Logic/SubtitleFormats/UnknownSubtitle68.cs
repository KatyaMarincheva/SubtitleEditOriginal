﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle68.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 68.
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
    /// The unknown subtitle 68.
    /// </summary>
    public class UnknownSubtitle68 : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\d\d:\d\d:\d\dF\d\d", RegexOptions.Compiled); // 10:00:02F00

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
                return "Unknown 68";
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
            // Mais l'image utilisée pour
            // nous vendre la nourriture
            // 10:00:44F11
            // est restée celle d'une Amérique
            // bucolique et agraire.
            // 10:00:46F18   10:00:50F14
            StringBuilder sb = new StringBuilder();
            if (subtitle.Header != null && subtitle.Header.Contains(";» DO NOT MODIFY THE FOLLOWING 3 LINES"))
            {
                sb.AppendLine(subtitle.Header.Trim());
                sb.AppendLine();
            }
            else
            {
                sb.AppendLine(@";» Video file: C:\Documents and Settings\video.mpg
;» Last edited: 24 sept. 09 11:26
;» Timing model: NTSC (30 fps)
;» Drop frame timing: ON
;» Number of captions: 1348
;» Caption time codes: 10:00:02F00 - 11:32:07F00
;» Video start time: 09:59:15F12 (Forced)
;» Insertion disk created: NO
;» Reading speed: 300
;» Minimum display time: 30
;» Maximum display time (sec.): 5
;» Minimum erase time: 0
;» Tab stop value: 4
;» Sticky mode: OFF
;» Right justification ragged left: OFF
;» Parallelogram filter: OFF
;» Coding standard: EIA-608" + Environment.NewLine + ";» \"Bottom\" row: 15" + @"
;» Lines per caption: 15
;» Characters per line: 32
;» Default horizontal position: Center
;» Default vertical position: Bottom
;» Default mode: PopOn
;» Captioning channel: 1

;» DO NOT MODIFY THE FOLLOWING 3 LINES
;»»10<210>10000001000000040?000?000100030000100?:?8;;:400685701001000100210<1509274
;»»2000000000200500005>??????091000000014279616<6000000000000000000000000000000000000000000000000000000005000105=4641;?
;»»0020??000000900<0<0<008000000000000<0<0<0080000000??00<0??000=200>1000;5=83<:;
;»
;» ************************************************************************");
                sb.AppendLine();
            }

            const string paragraphWriteFormat = "{0}{1}  {2}   {3}{1}";
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = p.Text.Replace("♪", "|");
                if (text.StartsWith("<i>"))
                {
                    text = ",b" + Environment.NewLine + text;
                }

                if (text.StartsWith("{\\an8}"))
                {
                    text = ",12" + Environment.NewLine + text;
                }

                text = HtmlUtil.RemoveHtmlTags(text, true);
                sb.AppendLine(string.Format(paragraphWriteFormat, text, Environment.NewLine, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime)));
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
            StringBuilder text = new StringBuilder();
            StringBuilder header = new StringBuilder();
            Paragraph p = null;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (subtitle.Paragraphs.Count == 0 && line.StartsWith(';') || line.Length == 0)
                {
                    header.AppendLine(line);
                }
                else if (RegexTimeCode.IsMatch(line))
                {
                    string[] timeParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (timeParts.Length == 1)
                    {
                        try
                        {
                            TimeCode start = DecodeTimeCode(timeParts[0]);
                            if (p != null && p.EndTime.TotalMilliseconds == 0)
                            {
                                p.EndTime.TotalMilliseconds = start.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                            }

                            TimeCode end = new TimeCode(0, 0, 0, 0);
                            p = MakeTextParagraph(text, p, start, end);
                            subtitle.Paragraphs.Add(p);
                            text = new StringBuilder();
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                    else if (timeParts.Length == 2)
                    {
                        try
                        {
                            TimeCode start = DecodeTimeCode(timeParts[0]);
                            if (p != null && p.EndTime.TotalMilliseconds == 0)
                            {
                                p.EndTime.TotalMilliseconds = start.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                            }

                            TimeCode end = DecodeTimeCode(timeParts[1]);
                            p = MakeTextParagraph(text, p, start, end);
                            subtitle.Paragraphs.Add(p);
                            text = new StringBuilder();
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    text.AppendLine(line.Trim().Replace("|", "♪"));
                    if (text.Length > 5000)
                    {
                        return;
                    }
                }
                else
                {
                    text = new StringBuilder();
                }
            }

            subtitle.Header = header.ToString();
            subtitle.Renumber();
        }

        /// <summary>
        /// The make text paragraph.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        private static Paragraph MakeTextParagraph(StringBuilder text, Paragraph p, TimeCode start, TimeCode end)
        {
            p = new Paragraph(start, end, text.ToString().Trim());
            if (p.Text.StartsWith(",b" + Environment.NewLine))
            {
                p.Text = "<i>" + p.Text.Remove(0, 2).Trim() + "</i>";
            }
            else if (p.Text.StartsWith(",1" + Environment.NewLine))
            {
                p.Text = "{\\an8}" + p.Text.Remove(0, 2).Trim();
            }
            else if (p.Text.StartsWith(",12" + Environment.NewLine))
            {
                p.Text = "{\\an8}" + p.Text.Remove(0, 3).Trim();
            }

            return p;
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
            return string.Format("{0:00}:{1:00}:{2:00}F{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timePart">
        /// The time part.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timePart)
        {
            string s = timePart.Substring(0, 11);
            string[] parts = s.Split(new[] { ':', 'F' }, StringSplitOptions.RemoveEmptyEntries);
            return new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), FramesToMillisecondsMax999(int.Parse(parts[3])));
        }
    }
}