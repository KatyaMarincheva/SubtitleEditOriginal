// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Json.cs" company="">
//   
// </copyright>
// <summary>
//   The json.
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
    /// The json.
    /// </summary>
    public class Json : SubtitleFormat
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
                return "JSON";
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
        /// The encode json text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string EncodeJsonText(string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c == '"')
                {
                    sb.Append("\\\"");
                }
                else if (c == '\\')
                {
                    sb.Append("\\\\");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Replace(Environment.NewLine, "<br />");
        }

        /// <summary>
        /// The decode json text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string DecodeJsonText(string text)
        {
            StringBuilder sb = new StringBuilder();
            text = text.Replace("<br />", Environment.NewLine);
            text = text.Replace("<br>", Environment.NewLine);
            text = text.Replace("<br/>", Environment.NewLine);
            text = text.Replace("\\n", Environment.NewLine);
            bool keepNext = false;
            foreach (char c in text)
            {
                if (c == '\\' && !keepNext)
                {
                    keepNext = true;
                }
                else
                {
                    sb.Append(c);
                    keepNext = false;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The convert json special characters.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ConvertJsonSpecialCharacters(string s)
        {
            if (s.Contains("\\u00"))
            {
                for (int i = 33; i < 200; i++)
                {
                    string tag = "\\u" + i.ToString("x4");
                    if (s.Contains(tag))
                    {
                        s = s.Replace(tag, Convert.ToChar(i).ToString());
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// The read tag.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadTag(string s, string tag)
        {
            int startIndex = s.IndexOfAny(new[] { "\"" + tag + "\"", "'" + tag + "'" }, StringComparison.Ordinal);
            if (startIndex < 0)
            {
                return null;
            }

            string res = s.Substring(startIndex + 3 + tag.Length).Trim().TrimStart(':').TrimStart();
            if (res.StartsWith('"'))
            { // text
                res = ConvertJsonSpecialCharacters(res);
                res = res.Replace("\\\"", "@__1");
                int endIndex = res.IndexOf("\"}", StringComparison.Ordinal);
                int endAlternate = res.IndexOf("\",", StringComparison.Ordinal);
                if (endIndex < 0)
                {
                    endIndex = endAlternate;
                }
                else if (endAlternate > 0 && endAlternate < endIndex)
                {
                    endIndex = endAlternate;
                }

                if (endIndex < 0)
                {
                    return null;
                }

                if (res.Length > 1)
                {
                    return res.Substring(1, endIndex - 1).Replace("@__1", "\\\"");
                }

                return string.Empty;
            }
            else
            { // number
                int endIndex = res.IndexOfAny(new[] { ',', '}' });
                if (endIndex < 0)
                {
                    return null;
                }

                return res.Substring(0, endIndex);
            }
        }

        /// <summary>
        /// The read array.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> ReadArray(string s, string tag)
        {
            List<string> list = new List<string>();

            int startIndex = s.IndexOfAny(new[] { "\"" + tag + "\"", "'" + tag + "'" }, StringComparison.Ordinal);
            if (startIndex < 0)
            {
                return list;
            }

            startIndex += tag.Length + 4;

            string res = s.Substring(startIndex);

            int tagLevel = 1;
            int nextTag = 0;
            int oldStart = 0;
            while (tagLevel >= 1 && nextTag >= 0 && nextTag + 1 < res.Length)
            {
                if (res[oldStart] == '"')
                {
                    nextTag = res.IndexOf('"', oldStart + 1);

                    while (nextTag > 0 && nextTag + 1 < res.Length && res[nextTag - 1] == '\\')
                    {
                        nextTag = res.IndexOf('"', nextTag + 1);
                    }

                    if (nextTag > 0)
                    {
                        string newValue = res.Substring(oldStart, nextTag - oldStart);
                        list.Add(newValue.Remove(0, 1));
                        oldStart = nextTag + 2;
                    }
                }
                else if (res[oldStart] != '[' && res[oldStart] != ']')
                {
                    nextTag = res.IndexOf(',', oldStart + 1);
                    if (nextTag > 0)
                    {
                        string newValue = res.Substring(oldStart, nextTag - oldStart);
                        if (newValue.EndsWith(']'))
                        {
                            newValue = newValue.TrimEnd(']');
                            tagLevel = -10; // return
                        }

                        list.Add(newValue.Trim());
                        oldStart = nextTag + 1;
                    }
                }
                else
                {
                    int nextBegin = res.IndexOf('[', nextTag);
                    int nextEnd = res.IndexOf(']', nextTag);
                    if (nextBegin < nextEnd && nextBegin != -1)
                    {
                        nextTag = nextBegin + 1;
                        tagLevel++;
                    }
                    else
                    {
                        nextTag = nextEnd + 1;
                        tagLevel--;
                        if (tagLevel == 1)
                        {
                            string newValue = res.Substring(oldStart, nextTag - oldStart);
                            list.Add(newValue);
                            if (res[nextTag] == ']')
                            {
                                tagLevel--;
                            }

                            oldStart = nextTag + 1;
                        }
                    }
                }
            }

            return list;
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

                sb.Append("{\"start\":");
                sb.Append(p.StartTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                sb.Append(",\"end\":");
                sb.Append(p.EndTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
                sb.Append(",\"text\":\"");
                sb.Append(EncodeJsonText(p.Text));
                sb.Append("\"}");
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

            if (!sb.ToString().TrimStart().StartsWith("[{\"start"))
            {
                return;
            }

            foreach (string line in sb.ToString().Replace("},{", Environment.NewLine).SplitToLines())
            {
                string s = line.Trim() + "}";
                string start = ReadTag(s, "start");
                string end = ReadTag(s, "end");
                string text = ReadTag(s, "text");
                if (start != null && end != null && text != null)
                {
                    double startSeconds;
                    double endSeconds;
                    if (double.TryParse(start, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out startSeconds) && double.TryParse(end, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out endSeconds))
                    {
                        subtitle.Paragraphs.Add(new Paragraph(DecodeJsonText(text), startSeconds * TimeCode.BaseUnit, endSeconds * TimeCode.BaseUnit));
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

        /// <summary>
        /// The read array.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<string> ReadArray(string text)
        {
            List<string> list = new List<string>();
            text = text.Trim();
            if (text.StartsWith('[') && text.EndsWith(']'))
            {
                text = text.Trim('[', ']');
                text = text.Trim();

                text = text.Replace("<br />", Environment.NewLine);
                text = text.Replace("<br>", Environment.NewLine);
                text = text.Replace("<br/>", Environment.NewLine);
                text = text.Replace("\\n", Environment.NewLine);

                bool keepNext = false;
                StringBuilder sb = new StringBuilder();
                foreach (char c in text)
                {
                    if (c == '\\' && !keepNext)
                    {
                        keepNext = true;
                    }
                    else if (!keepNext && c == ',')
                    {
                        list.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                        keepNext = false;
                    }
                }

                if (sb.Length > 0)
                {
                    list.Add(sb.ToString());
                }
            }

            return list;
        }
    }
}