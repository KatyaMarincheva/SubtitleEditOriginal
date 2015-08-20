// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvidDvd.cs" company="">
//   
// </copyright>
// <summary>
//   The avid dvd.
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
    /// The avid dvd.
    /// </summary>
    public class AvidDvd : SubtitleFormat
    {
        // 25    10:03:20:23 10:03:23:05 some text
        // I see, on my way.|New line also.
        // 26    10:03:31:18 10:03:34:00 even more text
        // Panessa, why didn't they give them
        // an escape route ?
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\d+\t\d\d:\d\d:\d\d:\d\d\t\d\d:\d\d:\d\d:\d\d\t.+$", RegexOptions.Compiled);

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
                return "Avid DVD";
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
            if (fileName != null && fileName.EndsWith(".dost", StringComparison.OrdinalIgnoreCase))
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
            StringBuilder sb = new StringBuilder();
            int count = 1;
            bool italic = false;
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = subtitle.Paragraphs[i];
                string text = p.Text;
                if (text.StartsWith('{') && text.Length > 6 && text[6] == '}')
                {
                    text = text.Remove(0, 6);
                }

                if (text.StartsWith("<i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal))
                {
                    if (!italic)
                    {
                        italic = true;
                        sb.AppendLine("$Italic = TRUE");
                    }
                }
                else if (italic)
                {
                    italic = false;
                    sb.AppendLine("$Italic = FALSE");
                }

                text = HtmlUtil.RemoveHtmlTags(text, true);
                sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", count, MakeTimeCode(p.StartTime), MakeTimeCode(p.EndTime), text.Replace(Environment.NewLine, "|")));
                sb.AppendLine();
                count++;
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
            StringBuilder sb = new StringBuilder();
            bool italic = false;
            foreach (string line in lines)
            {
                string s = line.TrimEnd();
                if (RegexTimeCode.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            p.Text = sb.ToString().Replace("|", Environment.NewLine).Trim();
                            subtitle.Paragraphs.Add(p);
                        }

                        sb.Clear();
                        string[] arr = s.Split('\t');
                        if (arr.Length >= 3)
                        {
                            string text = s.Remove(0, arr[0].Length + arr[1].Length + arr[2].Length + 2).Trim();

                            if (string.IsNullOrWhiteSpace(text.Replace("0", string.Empty).Replace("1", string.Empty).Replace("2", string.Empty).Replace("3", string.Empty).Replace("4", string.Empty).Replace("5", string.Empty).Replace("6", string.Empty).Replace("7", string.Empty).Replace("8", string.Empty).Replace("9", string.Empty).Replace(".", string.Empty).Replace(":", string.Empty).Replace(",", string.Empty)))
                            {
                                this._errorCount++;
                            }

                            if (italic)
                            {
                                text = "<i>" + text + "</i>";
                            }

                            sb.AppendLine(text);

                            p = new Paragraph(DecodeTimeCode(arr[1]), DecodeTimeCode(arr[2]), string.Empty);
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (s.StartsWith('$'))
                {
                    if (s.Replace(" ", string.Empty).Equals("$italic=true", StringComparison.OrdinalIgnoreCase))
                    {
                        italic = true;
                    }
                    else if (s.Replace(" ", string.Empty).Equals("$italic=false", StringComparison.OrdinalIgnoreCase))
                    {
                        italic = false;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(s))
                {
                    sb.AppendLine(s);
                }
            }

            if (p != null)
            {
                p.Text = sb.ToString().Replace("|", Environment.NewLine).Trim();
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The make time code.
        /// </summary>
        /// <param name="tc">
        /// The tc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MakeTimeCode(TimeCode tc)
        {
            return tc.ToHHMMSSFF();
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
            string[] arr = timeCode.Split(new[] { ':', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new TimeCode(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), FramesToMillisecondsMax999(int.Parse(arr[3])));
        }
    }
}