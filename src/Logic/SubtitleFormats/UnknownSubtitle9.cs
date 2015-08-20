// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle9.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 9.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 9.
    /// </summary>
    public class UnknownSubtitle9 : SubtitleFormat
    {
        // 00:04:04.219
        // The city council of long beach
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".html";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 9";
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
            sb.AppendLine("<div id=\"transcript\">");
            sb.AppendLine("  <div name=\"transcriptText\" id=\"transcriptText\">");
            sb.AppendLine("    <div id=\"transcriptPanel\">");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("      <a class=\"caption\" starttime=\"{0}\" duration=\"{1}\">{2}</a>", p.StartTime.TotalMilliseconds, p.Duration.TotalMilliseconds, p.Text.Replace(Environment.NewLine, "<br />")));
            }

            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");
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
            // <a class="caption" starttime="0" duration="16000">[♪techno music♪]</a>
            StringBuilder temp = new StringBuilder();
            foreach (string l in lines)
            {
                temp.Append(l);
            }

            string all = temp.ToString();
            if (!all.Contains("class=\"caption\""))
            {
                return;
            }

            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                int indexOfStart = line.IndexOf("starttime=", StringComparison.Ordinal);
                int indexOfDuration = line.IndexOf("duration=", StringComparison.Ordinal);
                if (line.Contains("class=\"caption\"") && indexOfStart > 0 && indexOfDuration > 0)
                {
                    string startTime = "0";
                    int index = indexOfStart + 10;
                    while (index < line.Length && @"0123456789""'.,".Contains(line[index]))
                    {
                        if (@"0123456789,.".Contains(line[index]))
                        {
                            startTime += line[index];
                        }

                        index++;
                    }

                    string duration = "0";
                    index = indexOfDuration + 9;
                    while (index < line.Length && @"0123456789""'.,".Contains(line[index]))
                    {
                        if (@"0123456789,.".Contains(line[index]))
                        {
                            duration += line[index];
                        }

                        index++;
                    }

                    string text = string.Empty;
                    index = line.IndexOf('>', indexOfDuration);
                    if (index > 0 && index + 1 < line.Length)
                    {
                        text = line.Substring(index + 1).Trim();
                        index = text.IndexOf("</", StringComparison.Ordinal);
                        if (index > 0)
                        {
                            text = text.Substring(0, index);
                        }

                        text = text.Replace("<br />", Environment.NewLine);
                    }

                    int startMilliseconds;
                    int durationMilliseconds;
                    if (text.Length > 0 && int.TryParse(startTime, out startMilliseconds) && int.TryParse(duration, out durationMilliseconds))
                    {
                        subtitle.Paragraphs.Add(new Paragraph(text, startMilliseconds, startMilliseconds + durationMilliseconds));
                    }
                }
            }

            subtitle.Renumber();
        }
    }
}