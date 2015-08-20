﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="F4Text.cs" company="">
//   
// </copyright>
// <summary>
//   #00:00:06-8#
//   http://www.audiotranskription.de
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    ///     #00:00:06-8#
    ///     http://www.audiotranskription.de
    /// </summary>
    public class F4Text : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d-\d$", RegexOptions.Compiled);

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
                return "F4 Text";
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
        /// The to f 4 text.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToF4Text(Subtitle subtitle)
        {
            StringBuilder sb = new StringBuilder();

            // double lastEndTimeMilliseconds = -1;
            const string writeFormat = "{0}{1}";
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                // if (p.StartTime.TotalMilliseconds == lastEndTimeMilliseconds)
                sb.AppendFormat(writeFormat, HtmlUtil.RemoveHtmlTags(p.Text, true), EncodeTimeCode(p.EndTime));

                // else
                // sb.Append(string.Format("{0}{1}{2}", EncodeTimeCode(p.StartTime), HtmlUtil.RemoveHtmlTags(p.Text), EncodeTimeCode(p.EndTime)));
                // lastEndTimeMilliseconds = p.EndTime.TotalMilliseconds;
            }

            return sb.ToString().Trim();
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
            if (fileName != null && !fileName.EndsWith(this.Extension, StringComparison.OrdinalIgnoreCase))
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
            return ToF4Text(subtitle);
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
                sb.AppendLine(line);
            }

            string text = sb.ToString();
            if (text.Contains("{\\rtf") || text.Contains("<transcript>"))
            {
                return;
            }

            this.LoadF4TextSubtitle(subtitle, text);
        }

        /// <summary>
        /// The load f 4 text subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        protected void LoadF4TextSubtitle(Subtitle subtitle, string text)
        {
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            string[] arr = text.Trim().Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder currentText = new StringBuilder();
            foreach (string line in arr)
            {
                if (regexTimeCodes.IsMatch(line))
                {
                    if (p == null)
                    {
                        p = new Paragraph();
                        if (currentText.Length > 0)
                        {
                            p.Text = currentText.ToString().Trim().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                            p.Text = p.Text.Trim('\n', '\r');
                            subtitle.Paragraphs.Add(p);
                            p = new Paragraph();
                        }
                    }

                    if (p.StartTime.TotalMilliseconds == 0 || currentText.Length == 0)
                    {
                        p.StartTime = this.DecodeTimeCode(line.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    else
                    {
                        p.EndTime = this.DecodeTimeCode(line.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries));
                        p.Text = currentText.ToString().Trim().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                        p.Text = p.Text.Trim('\n', '\r').Trim();
                        subtitle.Paragraphs.Add(p);
                        p = null;
                        currentText.Clear();
                    }
                }
                else
                {
                    if (p == null && subtitle.Paragraphs.Count > 0)
                    {
                        p = new Paragraph();
                        p.StartTime.TotalMilliseconds = subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].EndTime.TotalMilliseconds;
                    }

                    currentText.AppendLine(line.Trim());
                }
            }

            if (currentText.Length > 0 && subtitle.Paragraphs.Count > 0 && currentText.Length < 1000)
            {
                if (p == null)
                {
                    p = new Paragraph();
                }

                p.Text = currentText.ToString().Trim().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                p.Text = p.Text.Trim('\n', '\r').Trim();
                p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + 3000;
                subtitle.Paragraphs.Add(p);
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
            return string.Format(" #{0:00}:{1:00}:{2:00}-{3:0}# ", time.Hours, time.Minutes, time.Seconds, Math.Round(time.Milliseconds / 100.0, 0));
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
        private TimeCode DecodeTimeCode(string[] parts)
        {
            TimeCode tc = new TimeCode(0, 0, 0, 0);
            try
            {
                int hour = int.Parse(parts[0]);
                int minutes = int.Parse(parts[1]);
                int seconds = int.Parse(parts[2]);
                int millisecond = int.Parse(parts[3]);

                int milliseconds = (int)(millisecond * 100.0);
                if (milliseconds > 999)
                {
                    milliseconds = 999;
                }

                tc = new TimeCode(hour, minutes, seconds, milliseconds);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                this._errorCount++;
            }

            return tc;
        }
    }
}