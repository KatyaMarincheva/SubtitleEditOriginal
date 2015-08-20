// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lrc.cs" company="">
//   
// </copyright>
// <summary>
//   LRC is a format that synchronizes song lyrics with an audio/video file, [mm:ss.xx] where mm is minutes, ss is
//   seconds and xx is hundredths of a second.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic.Enums;

    /// <summary>
    ///     LRC is a format that synchronizes song lyrics with an audio/video file, [mm:ss.xx] where mm is minutes, ss is
    ///     seconds and xx is hundredths of a second.
    /// </summary>
    public class Lrc : SubtitleFormat
    {
        /// <summary>
        /// The _time code.
        /// </summary>
        private static readonly Regex _timeCode = new Regex(@"^\[\d+:\d\d\.\d\d\].*$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".lrc";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "LRC Lyrics";
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

            if (subtitle.Paragraphs.Count > 4)
            {
                bool allStartWithNumber = true;
                foreach (Paragraph p in subtitle.Paragraphs)
                {
                    if (p.Text.Length > 1 && !Utilities.IsInteger(p.Text.Substring(0, 2)))
                    {
                        allStartWithNumber = false;
                        break;
                    }
                }

                if (allStartWithNumber)
                {
                    return false;
                }
            }

            if (subtitle.Paragraphs.Count > this._errorCount)
            {
                if (new UnknownSubtitle33().IsMine(lines, fileName) || new UnknownSubtitle36().IsMine(lines, fileName) || new TMPlayer().IsMine(lines, fileName))
                {
                    return false;
                }

                return true;
            }

            return false;
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
            if (!string.IsNullOrEmpty(subtitle.Header) && (subtitle.Header.Contains("[ar:") || subtitle.Header.Contains("[ti:")))
            {
                sb.Append(subtitle.Header);
            }

            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = subtitle.Paragraphs[i];
                Paragraph next = null;
                if (i + 1 < subtitle.Paragraphs.Count)
                {
                    next = subtitle.Paragraphs[i + 1];
                }

                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                text = text.Replace(Environment.NewLine, " "); // text = text.Replace(Environment.NewLine, "|");
                sb.AppendLine(string.Format("[{0:00}:{1:00}.{2:00}]{3}", p.StartTime.Hours * 60 + p.StartTime.Minutes, p.StartTime.Seconds, (int)Math.Round(p.StartTime.Milliseconds / 10.0), text));

                if (next == null || next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds > 100)
                {
                    TimeCode tc = new TimeCode(p.EndTime.TotalMilliseconds);
                    sb.AppendLine(string.Format("[{0:00}:{1:00}.{2:00}]{3}", tc.Hours * 60 + tc.Minutes, tc.Seconds, (int)Math.Round(tc.Milliseconds / 10.0), string.Empty));
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
        { // [01:05.99]I've been walking in the same way as I do
            this._errorCount = 0;
            StringBuilder header = new StringBuilder();
            foreach (string line in lines)
            {
                if (line.StartsWith('[') && _timeCode.Match(line).Success)
                {
                    string s = line;
                    s = line.Substring(1, 8);
                    string[] parts = s.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        try
                        {
                            int minutes = int.Parse(parts[0]);
                            int seconds = int.Parse(parts[1]);
                            int milliseconds = int.Parse(parts[2]) * 10;
                            string text = line.Remove(0, 9).Trim().TrimStart(']').Trim();
                            TimeCode start = new TimeCode(0, minutes, seconds, milliseconds);
                            Paragraph p = new Paragraph(start, new TimeCode(0, 0, 0, 0), text);
                            subtitle.Paragraphs.Add(p);
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
                else if (line.StartsWith("[ar:"))
                {
                    // [ar:Lyrics artist]
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }
                }
                else if (line.StartsWith("[al:"))
                {
                    // [al:Album where the song is from]
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }
                }
                else if (line.StartsWith("[ti:"))
                {
                    // [ti:Lyrics (song) title]
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }
                }
                else if (line.StartsWith("[au:"))
                {
                    // [au:Creator of the Songtext]
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }
                }
                else if (line.StartsWith("[length:"))
                {
                    // [length:How long the song is]
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }
                }
                else if (line.StartsWith("[by:"))
                {
                    // [by:Creator of the LRC file]
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (subtitle.Paragraphs.Count < 1)
                    {
                        header.AppendLine(line);
                    }

                    this._errorCount++;
                }
                else if (subtitle.Paragraphs.Count < 1)
                {
                    header.AppendLine(line);
                }
            }

            subtitle.Header = header.ToString();

            int max = subtitle.Paragraphs.Count;
            for (int i = 0; i < max; i++)
            {
                Paragraph p = subtitle.Paragraphs[i];
                while (_timeCode.Match(p.Text).Success)
                {
                    string s = p.Text.Substring(1, 8);
                    p.Text = p.Text.Remove(0, 10).Trim();
                    string[] parts = s.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        int minutes = int.Parse(parts[0]);
                        int seconds = int.Parse(parts[1]);
                        int milliseconds = int.Parse(parts[2]) * 10;
                        string text = GetTextAfterTimeCodes(p.Text);
                        TimeCode start = new TimeCode(0, minutes, seconds, milliseconds);
                        Paragraph newParagraph = new Paragraph(start, new TimeCode(0, 0, 0, 0), text);
                        subtitle.Paragraphs.Add(newParagraph);
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }
            }

            subtitle.Sort(SubtitleSortCriteria.StartTime);

            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                p.Text = Utilities.AutoBreakLine(p.Text);
                Paragraph next = subtitle.GetParagraphOrDefault(index + 1);
                if (next != null)
                {
                    if (string.IsNullOrEmpty(next.Text))
                    {
                        p.EndTime = new TimeCode(next.StartTime.TotalMilliseconds);
                    }
                    else
                    {
                        p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                    }

                    if (p.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                    {
                        double duration = Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
                        p.EndTime = new TimeCode(p.StartTime.TotalMilliseconds + duration);
                    }
                }
                else
                {
                    double duration = Utilities.GetOptimalDisplayMilliseconds(p.Text, 16) + 1500;
                    p.EndTime = new TimeCode(p.StartTime.TotalMilliseconds + duration);
                }

                index++;
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }

        /// <summary>
        /// The get text after time codes.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetTextAfterTimeCodes(string s)
        {
            while (_timeCode.IsMatch(s))
            {
                s = s.Remove(0, 10).Trim();
            }

            return s;
        }
    }
}