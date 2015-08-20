// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonType3.cs" company="">
//   
// </copyright>
// <summary>
//   The json type 3.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The json type 3.
    /// </summary>
    public class JsonType3 : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".json";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "JSON Type 3";
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
            StringBuilder sb = new StringBuilder(@"[");
            int count = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (count > 0)
                {
                    sb.Append(',');
                }

                sb.Append("{\"duration\":");
                sb.Append(p.Duration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
                sb.Append(",\"content\":\"");
                sb.Append(Json.EncodeJsonText(p.Text) + "\"");
                sb.Append(",\"startOfParagraph\":false");
                sb.Append(",\"startTime\":");
                sb.Append(p.StartTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
                sb.Append('}');
                count++;
            }

            sb.Append(']');
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

            StringBuilder sb = new StringBuilder();
            foreach (string s in lines)
            {
                sb.Append(s);
            }

            int startIndex = sb.ToString().IndexOf("[{\"duration", StringComparison.Ordinal);
            if (startIndex < 0)
            {
                return;
            }

            string text = sb.ToString().Substring(startIndex);
            foreach (string line in text.Replace("},{", Environment.NewLine).SplitToLines())
            {
                string s = line.Trim() + "}";
                string start = Json.ReadTag(s, "startTime");
                string duration = Json.ReadTag(s, "duration");
                string content = Json.ReadTag(s, "content");
                if (start != null && duration != null && content != null)
                {
                    double startSeconds;
                    double durationSeconds;
                    if (double.TryParse(start, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out startSeconds) && double.TryParse(duration, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out durationSeconds))
                    {
                        subtitle.Paragraphs.Add(new Paragraph(Json.DecodeJsonText(content), startSeconds, startSeconds + durationSeconds));
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
                else
                {
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }
    }
}