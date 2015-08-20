// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebVTTFileWithLineNumber.cs" company="">
//   
// </copyright>
// <summary>
//   http://www.whatwg.org/specs/web-apps/current-work/webvtt.html
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
    ///     http://www.whatwg.org/specs/web-apps/current-work/webvtt.html
    /// </summary>
    public class WebVTTFileWithLineNumber : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^-?\d+:-?\d+:-?\d+\.-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+\.-?\d+", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes middle.
        /// </summary>
        private static readonly Regex RegexTimeCodesMiddle = new Regex(@"^-?\d+:-?\d+\.-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+\.-?\d+", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes short.
        /// </summary>
        private static readonly Regex RegexTimeCodesShort = new Regex(@"^-?\d+:-?\d+\.-?\d+\s*-->\s*-?\d+:-?\d+\.-?\d+", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".vtt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "WebVTT File with#";
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
        /// The get voices.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> GetVoices(Subtitle subtitle)
        {
            List<string> list = new List<string>();
            if (subtitle != null && subtitle.Paragraphs != null)
            {
                foreach (Paragraph p in subtitle.Paragraphs)
                {
                    string s = p.Text;
                    int startIndex = s.IndexOf("<v ", StringComparison.Ordinal);
                    while (startIndex >= 0)
                    {
                        int endIndex = s.IndexOf('>', startIndex);
                        if (endIndex > startIndex)
                        {
                            string voice = s.Substring(startIndex + 2, endIndex - startIndex - 2).Trim();
                            if (!list.Contains(voice))
                            {
                                list.Add(voice);
                            }
                        }

                        if (startIndex == s.Length - 1)
                        {
                            startIndex = -1;
                        }
                        else
                        {
                            startIndex = s.IndexOf("<v ", startIndex + 1, StringComparison.Ordinal);
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// The remove tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveTag(string tag, string text)
        {
            int indexOfTag = text.IndexOf("<" + tag + " ", StringComparison.Ordinal);
            if (indexOfTag >= 0)
            {
                int indexOfEnd = text.IndexOf('>', indexOfTag);
                if (indexOfEnd > 0)
                {
                    text = text.Remove(indexOfTag, indexOfEnd - indexOfTag + 1);
                    text = text.Replace("</" + tag + ">", string.Empty);
                }
            }

            return text;
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
            const string timeCodeFormatNoHours = "{0:00}:{1:00}.{2:000}"; // h:mm:ss.cc
            const string timeCodeFormatHours = "{0}:{1:00}:{2:00}.{3:000}"; // h:mm:ss.cc
            const string paragraphWriteFormat = "{0} --> {1}{4}{2}{3}{4}";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("WEBVTT FILE");
            sb.AppendLine();
            int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string start = string.Format(timeCodeFormatNoHours, p.StartTime.Minutes, p.StartTime.Seconds, p.StartTime.Milliseconds);
                string end = string.Format(timeCodeFormatNoHours, p.EndTime.Minutes, p.EndTime.Seconds, p.EndTime.Milliseconds);

                if (p.StartTime.Hours > 0 || p.EndTime.Hours > 0)
                {
                    start = string.Format(timeCodeFormatHours, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, p.StartTime.Milliseconds);
                    end = string.Format(timeCodeFormatHours, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, p.EndTime.Milliseconds);
                }

                string style = string.Empty;
                if (!string.IsNullOrEmpty(p.Extra) && subtitle.Header == "WEBVTT FILE")
                {
                    style = p.Extra;
                }

                sb.Append(count);
                sb.AppendLine();
                sb.AppendLine(string.Format(paragraphWriteFormat, start, end, FormatText(p), style, Environment.NewLine));
                count++;
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
            bool textDone = true;
            foreach (string line in lines)
            {
                string s = line;
                bool isTimeCode = line.Contains("-->");
                if (isTimeCode && RegexTimeCodesMiddle.IsMatch(s))
                {
                    s = "00:" + s; // start is without hours, end is with hours
                }

                if (isTimeCode && RegexTimeCodesShort.IsMatch(s))
                {
                    s = "00:" + s.Replace("--> ", "--> 00:");
                }

                if (isTimeCode && RegexTimeCodes.IsMatch(s))
                {
                    textDone = false;
                    if (p != null)
                    {
                        subtitle.Paragraphs.Add(p);
                        p = null;
                    }

                    try
                    {
                        string[] parts = s.Replace("-->", "@").Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        p = new Paragraph();
                        p.StartTime = GetTimeCodeFromString(parts[0]);
                        p.EndTime = GetTimeCodeFromString(parts[1]);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (subtitle.Paragraphs.Count == 0 && line.Trim() == "WEBVTT FILE")
                {
                    subtitle.Header = "WEBVTT FILE";
                }
                else if (p != null && !string.IsNullOrWhiteSpace(line))
                {
                    string text = line.Trim();
                    if (!textDone)
                    {
                        p.Text = (p.Text + Environment.NewLine + text).Trim();
                    }
                }
                else if (line.Length == 0)
                {
                    textDone = true;
                }
            }

            if (subtitle.Header == null && subtitle.Header != "WEBVTT FILE")
            {
                subtitle.Paragraphs.Clear();
                this._errorCount++;
            }

            if (p != null)
            {
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The remove native formatting.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="newFormat">
        /// The new format.
        /// </param>
        public override void RemoveNativeFormatting(Subtitle subtitle, SubtitleFormat newFormat)
        {
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (p.Text.Contains('<'))
                {
                    string text = p.Text;
                    text = RemoveTag("v", text);
                    text = RemoveTag("rt", text);
                    text = RemoveTag("ruby", text);
                    text = RemoveTag("c", text);
                    text = RemoveTag("span", text);
                    p.Text = text;
                }
            }
        }

        /// <summary>
        /// The format text.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FormatText(Paragraph p)
        {
            string text = p.Text;
            while (text.Contains(Environment.NewLine + Environment.NewLine))
            {
                text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }

            return text;
        }

        /// <summary>
        /// The get time code from string.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode GetTimeCodeFromString(string time)
        {
            // hh:mm:ss.mmm
            string[] timeCode = time.Trim().Split(':', '.', ' ');
            return new TimeCode(int.Parse(timeCode[0]), int.Parse(timeCode[1]), int.Parse(timeCode[2]), int.Parse(timeCode[3]));
        }
    }
}