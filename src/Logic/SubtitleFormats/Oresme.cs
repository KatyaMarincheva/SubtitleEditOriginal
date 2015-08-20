// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Oresme.cs" company="">
//   
// </copyright>
// <summary>
//   The oresme.
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
    /// The oresme.
    /// </summary>
    public class Oresme : SubtitleFormat
    {
        // 00:00:00:00{BC}{W2710}
        // 10:00:00:15{Bottom}{Open Caption}{Center}{White}{Font Arial GVP Bold}
        // 10:00:17:06{Bottom}{Open Caption}{Center}{White}{Font Arial GVP Bold}We view
        // 10:00:19:06{Bottom}{Open Caption}{Center}{White}{Font Arial GVP Bold}Lufa Farms as{N}an agrotechnology business
        /// <summary>
        /// The regex time codes 1.
        /// </summary>
        private static readonly Regex regexTimeCodes1 = new Regex(@"^\d\d:\d\d:\d\d:\d\d\{", RegexOptions.Compiled);

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
                return "Oresme";
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
            const string format = "{0}{1}{2}";
            const string tags = "{Bottom}{Open Caption}{Center}{White}{Font Arial GVP Bold}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("00:00:00:00{BC}{W2710}");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = HtmlUtil.RemoveHtmlTags(p.Text, true);
                text = text.Replace(Environment.NewLine, "{N}");
                sb.AppendLine(string.Format(format, EncodeTimeCode(p.StartTime), tags, text));
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
                string s = line.Trim();
                Match match = regexTimeCodes1.Match(s);
                if (match.Success)
                {
                    Paragraph p = new Paragraph();
                    try
                    {
                        p.StartTime = DecodeTimeCode(s.Substring(0, 11));
                        p.Text = GetText(line.Remove(0, 11));
                        subtitle.Paragraphs.Add(p);
                    }
                    catch (Exception exception)
                    {
                        this._errorCount++;
                        Debug.WriteLine(exception.Message);
                    }
                }
                else
                {
                    this._errorCount++;
                }

                for (int i = 0; i < subtitle.Paragraphs.Count - 1; i++)
                {
                    Paragraph p2 = subtitle.Paragraphs[i];
                    Paragraph next = subtitle.Paragraphs[i + 1];
                    p2.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                }

                if (subtitle.Paragraphs.Count > 0)
                {
                    subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].EndTime.TotalMilliseconds = subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].Text);
                }
            }

            subtitle.RemoveEmptyLines();
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
            return time.ToHHMMSSFF();
        }

        /// <summary>
        /// The get text.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetText(string s)
        {
            s = s.Replace("{N}", Environment.NewLine);
            StringBuilder sb = new StringBuilder();
            bool tagOn = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '{')
                {
                    tagOn = true;
                }
                else if (s[i] == '}')
                {
                    tagOn = false;
                }
                else if (!tagOn)
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string part)
        {
            string[] parts = part.Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries);

            // 00:00:07:12
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            return new TimeCode(hour, minutes, seconds, FramesToMillisecondsMax999(frames));
        }
    }
}