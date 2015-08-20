// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle59.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 59.
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
    /// The unknown subtitle 59.
    /// </summary>
    public class UnknownSubtitle59 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        public static readonly Regex RegexTimeCodes = new Regex(@"^\d\d\:\d\d\:\d\d\t.+\t\d\d\:\d\d\:\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes 2.
        /// </summary>
        public static readonly Regex RegexTimeCodes2 = new Regex(@"^\d\d\:\d\d\:\d\d.+\d\d\:\d\d\:\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// The regex start only.
        /// </summary>
        private static readonly Regex regexStartOnly = new Regex(@"^\d\d\:\d\d\:\d\d\t.+$", RegexOptions.Compiled);

        /// <summary>
        /// The regex end only.
        /// </summary>
        private static readonly Regex regexEndOnly = new Regex(@"\d\d\:\d\d\:\d\d$", RegexOptions.Compiled);

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
                return "Unknown 59";
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
            // 00:06:12  Would you like to see any particular style? 00:06:13
            // 00:35:46  I made coffee. Would you like some?             00:35:47
            // Yes.
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string[] lines = HtmlUtil.RemoveHtmlTags(p.Text).SplitToLines();
                if (lines.Length > 0)
                {
                    sb.AppendLine(EncodeTimeCode(p.StartTime) + "\t" + lines[0]);
                    for (int i = 1; i < lines.Length; i++)
                    {
                        sb.AppendLine("\t" + lines[i]);
                    }
                }
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
            Paragraph p = null;
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (RegexTimeCodes.Match(s).Success || RegexTimeCodes2.IsMatch(s))
                {
                    if (RegexTimeCodes2.IsMatch(s))
                    {
                        this._errorCount++;
                    }

                    try
                    {
                        p = new Paragraph();
                        string[] start = s.Substring(0, 8).Split(':');
                        string[] end = s.Remove(0, s.Length - 8).Split(':');
                        if (start.Length == 3)
                        {
                            int hours = int.Parse(start[0]);
                            int minutes = int.Parse(start[1]);
                            int seconds = int.Parse(start[2]);
                            p.StartTime = new TimeCode(hours, minutes, seconds, 0);

                            hours = int.Parse(end[0]);
                            minutes = int.Parse(end[1]);
                            seconds = int.Parse(end[2]);
                            p.EndTime = new TimeCode(hours, minutes, seconds, 0);

                            string text = s.Remove(0, 8).Trim();
                            text = text.Substring(0, text.Length - 8).Trim();
                            p.Text = text;
                            if (text.Length > 1 && Utilities.IsInteger(text.Substring(0, 2)))
                            {
                                this._errorCount++;
                            }

                            subtitle.Paragraphs.Add(p);
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }
                else if (regexStartOnly.Match(s).Success)
                {
                    try
                    {
                        p = new Paragraph();
                        string[] start = s.Substring(0, 8).Split(':');
                        if (start.Length == 3)
                        {
                            int hours = int.Parse(start[0]);
                            int minutes = int.Parse(start[1]);
                            int seconds = int.Parse(start[2]);
                            p.StartTime = new TimeCode(hours, minutes, seconds, 0);

                            string text = s.Remove(0, 8).Trim();
                            p.Text = text;
                            if (text.Length > 1 && Utilities.IsInteger(text.Substring(0, 2)))
                            {
                                this._errorCount++;
                            }

                            subtitle.Paragraphs.Add(p);
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }
                else if (regexEndOnly.Match(s).Success)
                {
                    try
                    {
                        string[] end = s.Remove(0, s.Length - 8).Split(':');
                        if (end.Length == 3 && p != null)
                        {
                            int hours = int.Parse(end[0]);
                            int minutes = int.Parse(end[1]);
                            int seconds = int.Parse(end[2]);
                            p.EndTime = new TimeCode(hours, minutes, seconds, 0);

                            string text = s.Substring(0, s.Length - 8).Trim();
                            p.Text = p.Text + Environment.NewLine + text;
                            if (text.Length > 1 && Utilities.IsInteger(text.Substring(0, 2)))
                            {
                                this._errorCount++;
                            }

                            p = null;
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }
                else if (line.StartsWith("\t") && p != null)
                {
                    if (p.Text.Length > 1000)
                    {
                        this._errorCount += 100;
                        return;
                    }

                    p.Text = (p.Text + Environment.NewLine + s).Trim();
                }
                else if (s.Length > 0 && !Utilities.IsInteger(s))
                {
                    if (p != null && !p.Text.Contains(Environment.NewLine))
                    {
                        p.Text = p.Text + Environment.NewLine + s.Trim();
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
            }

            foreach (Paragraph p2 in subtitle.Paragraphs)
            {
                if (p2.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                {
                    p2.EndTime.TotalMilliseconds = p2.StartTime.TotalMilliseconds + Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
                }

                p2.Text = Utilities.AutoBreakLine(p2.Text);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTimeCode(TimeCode timeCode)
        {
            int seconds = (int)(timeCode.Seconds + timeCode.Milliseconds / 1000 + 0.5);
            return string.Format("{0:00}:{1:00}:{2:00}", timeCode.Hours, timeCode.Minutes, seconds);
        }
    }
}