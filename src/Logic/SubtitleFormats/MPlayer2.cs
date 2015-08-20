// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MPlayer2.cs" company="">
//   
// </copyright>
// <summary>
//   The m player 2.
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
    /// The m player 2.
    /// </summary>
    public class MPlayer2 : SubtitleFormat
    {
        /// <summary>
        /// The _regex m player 2 line.
        /// </summary>
        private static readonly Regex _regexMPlayer2Line = new Regex(@"^\[-?\d+]\[-?\d+].*$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".mpl";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "MPlayer2";
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
            int errors = 0;
            List<string> trimmedLines = new List<string>();
            foreach (string line in lines)
            {
                int indexOfStartBracket = line.IndexOf('[');
                if (!string.IsNullOrWhiteSpace(line) && line.Length < 250 && indexOfStartBracket >= 0 && indexOfStartBracket < 10)
                {
                    string s = RemoveIllegalSpacesAndFixEmptyCodes(line);
                    if (_regexMPlayer2Line.IsMatch(s))
                    {
                        trimmedLines.Add(line);
                    }
                    else
                    {
                        errors++;
                    }
                }
                else
                {
                    errors++;
                }
            }

            return trimmedLines.Count > errors;
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
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.Append('[');
                sb.Append((int)(p.StartTime.TotalMilliseconds / 100));
                sb.Append("][");
                sb.Append((int)(p.EndTime.TotalMilliseconds / 100));
                sb.Append(']');

                string[] parts = p.Text.SplitToLines();
                int count = 0;
                bool italicOn = false;
                foreach (string line in parts)
                {
                    if (count > 0)
                    {
                        sb.Append('|');
                    }

                    if (line.StartsWith("<i>") || italicOn)
                    {
                        italicOn = true;
                        sb.Append('/');
                    }

                    if (line.Contains("</i>"))
                    {
                        italicOn = false;
                    }

                    sb.Append(HtmlUtil.RemoveHtmlTags(line));
                    count++;
                }

                sb.AppendLine();
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

            foreach (string line in lines)
            {
                string s = RemoveIllegalSpacesAndFixEmptyCodes(line);
                if (_regexMPlayer2Line.IsMatch(s))
                {
                    try
                    {
                        int textIndex = s.IndexOf(']') + 1;
                        textIndex = s.IndexOf(']', textIndex) + 1;
                        if (textIndex < s.Length)
                        {
                            string text = s.Substring(textIndex);
                            if (text.StartsWith('/') && (Utilities.CountTagInText(text, '|') == 0 || text.Contains("|/")))
                            {
                                text = "<i>" + text.TrimStart('/').Replace("|/", Environment.NewLine) + "</i>";
                            }
                            else if (text.StartsWith('/') && text.Contains('|') && !text.Contains("|/"))
                            {
                                text = "<i>" + text.TrimStart('/').Replace("|", "</i>" + Environment.NewLine);
                            }
                            else if (text.Contains("|/"))
                            {
                                text = text.Replace("|/", Environment.NewLine + "<i>") + "</i>";
                            }

                            text = text.Replace("|", Environment.NewLine);
                            string temp = s.Substring(0, textIndex - 1);
                            string[] frames = temp.Replace("][", ":").Replace("[", string.Empty).Replace("]", string.Empty).Split(':');

                            double startSeconds = double.Parse(frames[0]) / 10;
                            double endSeconds = double.Parse(frames[1]) / 10;

                            if (startSeconds == 0 && subtitle.Paragraphs.Count > 0)
                            {
                                startSeconds = (subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].EndTime.TotalMilliseconds / 1000) + 0.1;
                            }

                            if (endSeconds == 0)
                            {
                                endSeconds = startSeconds;
                            }

                            subtitle.Paragraphs.Add(new Paragraph(text, startSeconds * 1000, endSeconds * 1000));
                        }
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

            subtitle.Renumber();
        }

        /// <summary>
        /// The remove illegal spaces and fix empty codes.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemoveIllegalSpacesAndFixEmptyCodes(string line)
        {
            int index = line.IndexOf(']');
            if (index >= 0 && index < line.Length)
            {
                index = line.IndexOf(']', index + 1);
                if (index >= 0 && index + 1 < line.Length)
                {
                    int indexOfBrackets = line.IndexOf("[]", StringComparison.Ordinal);
                    if (indexOfBrackets >= 0 && indexOfBrackets < index)
                    {
                        line = line.Insert(indexOfBrackets + 1, "0"); // set empty time codes to zero
                        index++;
                    }

                    while (line.Contains(' ') && line.IndexOf(' ') < index)
                    {
                        line = line.Remove(line.IndexOf(' '), 1);
                        index--;
                    }
                }
            }

            return line;
        }
    }
}