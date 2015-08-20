﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle25.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 25.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 25.
    /// </summary>
    public class UnknownSubtitle25 : SubtitleFormat
    {
        // 79.29 1.63
        /// <summary>
        /// The regex time code 1.
        /// </summary>
        private static readonly Regex RegexTimeCode1 = new Regex(@"^\d+.[0-9]{1,2} \d+.[0-9]{1,2}$", RegexOptions.Compiled);

        /// <summary>
        /// The regex time code 2.
        /// </summary>
        private static readonly Regex RegexTimeCode2 = new Regex(@"^\d+ \d+.[0-9]{1,2}$", RegexOptions.Compiled);

        /// <summary>
        /// The regex time code 3.
        /// </summary>
        private static readonly Regex RegexTimeCode3 = new Regex(@"^\d+.[0-9]{1,2} \d+$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".sub";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 25";
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"TITLE=
FILE=
AUTHOR=
TYPE=VIDEO
FORMAT=TIME
NOTE=
");

            Paragraph last = null;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("{0} {1}\r\n{2}\r\n", MakeTimeCode(p.StartTime, last), string.Format("{0:0.0#}", p.Duration.Seconds + p.Duration.Milliseconds / TimeCode.BaseUnit), p.Text));
                last = p;
            }

            return sb.ToString().Trim().Replace(Environment.NewLine, "\n");
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
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                string s = line.TrimEnd();
                if (RegexTimeCode1.IsMatch(s) || RegexTimeCode2.IsMatch(s) || RegexTimeCode3.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            p.Text = sb.ToString().Trim();
                            subtitle.Paragraphs.Add(p);
                        }

                        sb = new StringBuilder();
                        string[] arr = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 2)
                        {
                            double secondsSinceLast = double.Parse(arr[0]);
                            double secondsDuration = double.Parse(arr[1]);
                            if (subtitle.Paragraphs.Count > 0)
                            {
                                secondsSinceLast += subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].EndTime.TotalSeconds;
                            }

                            p = new Paragraph(string.Empty, (int)Math.Round(secondsSinceLast * TimeCode.BaseUnit), (int)Math.Round((secondsSinceLast + secondsDuration) * TimeCode.BaseUnit));
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (p != null && s.Length > 0)
                {
                    sb.AppendLine(s.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(s))
                {
                    if (subtitle.Paragraphs.Count == 0 && (s.StartsWith("TITLE=") || s.StartsWith("TITLE=") || s.StartsWith("FILE=") || s.StartsWith("AUTHOR=") || s.StartsWith("TYPE=VIDEO") || s.StartsWith("FORMAT=") || s.StartsWith("NOTE=")))
                    {
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
            }

            if (p != null)
            {
                p.Text = sb.ToString().Trim();
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The make time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="last">
        /// The last.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MakeTimeCode(TimeCode timeCode, Paragraph last)
        {
            double start = 0;
            if (last != null)
            {
                start = last.EndTime.TotalSeconds;
            }

            return string.Format("{0:0.0#}", timeCode.TotalSeconds - start);
        }
    }
}