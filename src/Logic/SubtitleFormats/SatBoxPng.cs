// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SatBoxPng.cs" company="">
//   
// </copyright>
// <summary>
//   http://forum.videohelp.com/threads/365786-Converting-Subtitles-%28XML-PNG%29-to-idx-sub
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    ///     http://forum.videohelp.com/threads/365786-Converting-Subtitles-%28XML-PNG%29-to-idx-sub
    /// </summary>
    public class SatBoxPng : SubtitleFormat
    {
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
                return "SatBox png";
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
            return "Not implemented";
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
            // <I s="0.600" e="3.720" x="268" y="458" w="218" h="58" i="AYZ1.png" />
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string path = Path.GetDirectoryName(fileName);
            foreach (string line in lines)
            {
                string s = line;
                if (line.Contains(" s=\"") && line.Contains(" e=\"") && line.Contains(" i=\"") && line.Contains(".png") && (line.Contains("<I ") || line.Contains("&lt;I ")))
                {
                    string start = GetTagValue("s", line);
                    string end = GetTagValue("e", line);
                    string text = GetTagValue("i", line);
                    try
                    {
                        if (File.Exists(Path.Combine(path, text)))
                        {
                            text = Path.Combine(path, text);
                        }

                        p = new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), text);
                        subtitle.Paragraphs.Add(p);
                    }
                    catch (Exception exception)
                    {
                        this._errorCount++;
                        Debug.WriteLine(exception.Message);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line) && p != null)
                {
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get tag value.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetTagValue(string tag, string line)
        {
            int start = line.IndexOf(tag + "=\"", StringComparison.Ordinal);
            if (start > 0 && line.Length > start + 4)
            {
                int end = line.IndexOf('"', start + 3);
                if (end > 0 && line.Length > end + 3)
                {
                    string value = line.Substring(start + 3, end - start - 3);
                    return value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string s)
        {
            return TimeCode.FromSeconds(double.Parse(s, CultureInfo.InvariantCulture));
        }
    }
}