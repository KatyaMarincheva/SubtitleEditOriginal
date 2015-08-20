// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonType5.cs" company="">
//   
// </copyright>
// <summary>
//   The json type 5.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The json type 5.
    /// </summary>
    public class JsonType5 : SubtitleFormat
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
                return "JSON Type 5";
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
            sb.Append("{\"text_tees\":[");
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = subtitle.Paragraphs[i];
                sb.Append(p.StartTime.TotalMilliseconds);
                sb.Append(',');
                sb.Append(p.EndTime.TotalMilliseconds);
                if (i < subtitle.Paragraphs.Count - 1)
                {
                    sb.Append(',');
                }
            }

            sb.Append("],");

            sb.Append("\"text_target\":[");
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                sb.Append("[\"w1\",\"w3\"],[\"w1\",\"w3\"]");
                if (i < subtitle.Paragraphs.Count - 1)
                {
                    sb.Append(',');
                }
            }

            sb.Append("],");

            sb.Append("\"text_content\":[");
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                sb.Append('[');
                Paragraph p = subtitle.Paragraphs[i];
                string[] lines = p.Text.Replace(Environment.NewLine, "\n").Split('\n');
                for (int j = 0; j < lines.Length; j++)
                {
                    sb.Append('"');
                    sb.Append(Json.EncodeJsonText(lines[j]));
                    sb.Append('"');
                    if (j < lines.Length - 1)
                    {
                        sb.Append(',');
                    }
                }

                sb.Append("],");
                if (i < subtitle.Paragraphs.Count - 1)
                {
                    sb.Append("[\"\",\"\"],");
                }
                else
                {
                    sb.Append("[\"\",\"\"]");
                }
            }

            sb.Append("],");

            sb.Append("\"text_styles\":[");
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                sb.Append("[\"s1\",\"s2\"],[\"s1\",\"s2\"]");
                if (i < subtitle.Paragraphs.Count - 1)
                {
                    sb.Append(',');
                }
            }

            sb.Append("],");

            sb.Append("\"timerange\":[");
            Paragraph timerageP = subtitle.GetParagraphOrDefault(0);
            if (timerageP == null)
            {
                sb.Append('0');
            }
            else
            {
                sb.Append(timerageP.StartTime.TotalMilliseconds);
            }

            sb.Append(',');
            timerageP = subtitle.GetParagraphOrDefault(subtitle.Paragraphs.Count - 1);
            if (timerageP == null)
            {
                sb.Append('0');
            }
            else
            {
                sb.Append(timerageP.EndTime.TotalMilliseconds);
            }

            sb.Append(']');

            sb.Append('}');

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

            string allText = sb.ToString();
            if (!allText.Contains("text_tees"))
            {
                return;
            }

            List<string> times = Json.ReadArray(allText, "text_tees");
            List<string> texts = Json.ReadArray(allText, "text_content");

            for (int i = 0; i < Math.Min(times.Count, texts.Count); i++)
            {
                try
                {
                    string text = texts[i];
                    if (text.StartsWith('['))
                    {
                        List<string> textLines = Json.ReadArray("{\"text\":" + texts[i] + "}", "text");
                        StringBuilder textSb = new StringBuilder();
                        foreach (string line in textLines)
                        {
                            string t = Json.DecodeJsonText(line);
                            if (t.StartsWith("[\"", StringComparison.Ordinal) && t.EndsWith("\"]", StringComparison.Ordinal))
                            {
                                StringBuilder innerSb = new StringBuilder();
                                List<string> innerTextLines = Json.ReadArray("{\"text\":" + t + "}", "text");
                                foreach (string innerLine in innerTextLines)
                                {
                                    innerSb.Append(' ');
                                    innerSb.Append(innerLine);
                                }

                                textSb.AppendLine(innerSb.ToString().Trim());
                            }
                            else
                            {
                                textSb.AppendLine(t);
                            }
                        }

                        text = textSb.ToString().Trim();
                        text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                    }

                    Paragraph p = new Paragraph(text, int.Parse(times[i]), 0);
                    if (i + 1 < times.Count)
                    {
                        p.EndTime.TotalMilliseconds = int.Parse(times[i + 1]);
                    }

                    subtitle.Paragraphs.Add(p);
                }
                catch
                {
                    this._errorCount++;
                }
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }
    }
}