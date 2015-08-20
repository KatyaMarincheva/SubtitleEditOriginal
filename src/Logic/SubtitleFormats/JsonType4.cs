// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonType4.cs" company="">
//   
// </copyright>
// <summary>
//   The json type 4.
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
    /// The json type 4.
    /// </summary>
    public class JsonType4 : SubtitleFormat
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
                return "JSON Type 4";
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

            string guid = Guid.NewGuid().ToString();
            string segmentTypeId = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 24);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string id = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 24);
                if (count > 0)
                {
                    sb.Append(',');
                }

                sb.Append("{\"hitType\":\"tag\",\"subTrack\":null,\"tags\":[],\"track\":\"Closed Captioning\",\"startTime\":");
                sb.Append(p.StartTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                sb.Append(",\"guid\":\"" + guid + "\",\"segmentTypeId\":\"" + segmentTypeId + "\",\"endTime\":");
                sb.Append(p.EndTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                sb.Append(",\"id\":\"" + id + "\",\"metadata\":{\"Text\":\"");
                sb.Append(Json.EncodeJsonText(p.Text) + "\"");

                sb.Append(",\"ID\":\"\",\"Language\":\"en\"}}");
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

            int startIndex = sb.ToString().IndexOf("[{\"hitType", StringComparison.Ordinal);
            if (startIndex < 0)
            {
                return;
            }

            string text = sb.ToString().Substring(startIndex);
            foreach (string line in text.Replace("},{", Environment.NewLine).SplitToLines())
            {
                string s = line.Trim() + "}";
                string start = Json.ReadTag(s, "startTime");
                string end = Json.ReadTag(s, "endTime");
                string content = Json.ReadTag(s, "Text");
                if (start != null && end != null && content != null)
                {
                    double startSeconds;
                    double endSeconds;
                    if (double.TryParse(start, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out startSeconds) && double.TryParse(end, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out endSeconds))
                    {
                        subtitle.Paragraphs.Add(new Paragraph(Json.DecodeJsonText(content), startSeconds * TimeCode.BaseUnit, endSeconds * TimeCode.BaseUnit));
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