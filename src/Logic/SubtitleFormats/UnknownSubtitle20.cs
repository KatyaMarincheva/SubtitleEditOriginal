// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle20.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 20.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 20.
    /// </summary>
    public class UnknownSubtitle20 : SubtitleFormat
    {
        /// <summary>
        /// The _regex time code 1.
        /// </summary>
        private static readonly Regex _regexTimeCode1 = new Regex(@"^     \d\d:\d\d:\d\d:\d\d     \d\d\d\d            ", RegexOptions.Compiled);

        /// <summary>
        /// The _regex time code 1 empty.
        /// </summary>
        private static readonly Regex _regexTimeCode1Empty = new Regex(@"^     \d\d:\d\d:\d\d:\d\d     \d\d\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// The _regex time code 2.
        /// </summary>
        private static readonly Regex _regexTimeCode2 = new Regex(@"^     \d\d:\d\d:\d\d:\d\d     \d\d\d\-\d\d          ", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".C";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 20";
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
            int number = 1;
            int number2 = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string line1 = string.Empty;
                string line2 = string.Empty;
                string[] lines = p.Text.Replace(Environment.NewLine, "\r").Split('\r');
                if (lines.Length > 2)
                {
                    lines = Utilities.AutoBreakLine(p.Text).Replace(Environment.NewLine, "\r").Split('\r');
                }

                if (lines.Length == 1)
                {
                    line2 = lines[0];
                }
                else
                {
                    line1 = lines[0];
                    line2 = lines[1];
                }

                // line 1
                sb.Append(string.Empty.PadLeft(5, ' '));
                sb.Append(p.StartTime.ToHHMMSSFF());
                sb.Append(string.Empty.PadLeft(5, ' '));
                sb.Append(number.ToString("D4"));
                sb.Append(string.Empty.PadLeft(12, ' '));
                sb.AppendLine(line1);

                // line 2
                sb.Append(string.Empty.PadLeft(5, ' '));
                sb.Append(p.EndTime.ToHHMMSSFF());
                sb.Append(string.Empty.PadLeft(5, ' '));
                sb.Append((number2 / 7 + 1).ToString("D3"));
                sb.Append('-');
                sb.Append((number2 % 7 + 1).ToString("D2"));
                sb.Append(string.Empty.PadLeft(10, ' '));
                sb.AppendLine(line2);
                sb.AppendLine();

                number++;
                number2++;
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
            Paragraph p = null;
            foreach (string line in lines)
            {
                string s = line.TrimEnd();
                if (_regexTimeCode1.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            subtitle.Paragraphs.Add(p);
                        }

                        p = new Paragraph(DecodeTimeCode(s.Substring(5, 11)), new TimeCode(0, 0, 0, 0), s.Remove(0, 37).Trim());
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (_regexTimeCode1Empty.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            subtitle.Paragraphs.Add(p);
                        }

                        p = new Paragraph(DecodeTimeCode(s.Substring(5, 11)), new TimeCode(0, 0, 0, 0), string.Empty);
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (_regexTimeCode2.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            p.EndTime = DecodeTimeCode(s.Substring(5, 11));
                            if (string.IsNullOrWhiteSpace(p.Text))
                            {
                                p.Text = s.Remove(0, 37).Trim();
                            }
                            else
                            {
                                p.Text = p.Text + Environment.NewLine + s.Remove(0, 37).Trim();
                            }
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(s))
                {
                    this._errorCount++;
                }
            }

            if (p != null)
            {
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timeCode)
        {
            string[] arr = timeCode.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            return new TimeCode(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), FramesToMillisecondsMax999(int.Parse(arr[3])));
        }
    }
}