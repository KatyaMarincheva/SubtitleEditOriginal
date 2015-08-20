// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlSamiArray.cs" company="">
//   
// </copyright>
// <summary>
//   The html sami array.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    /// <summary>
    /// The html sami array.
    /// </summary>
    public class HtmlSamiArray : SubtitleFormat
    {
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
                return "Html javascript sami array";
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
            return subtitle.Paragraphs.Count > 0;
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override string ToText(Subtitle subtitle, string title)
        {
            throw new NotImplementedException();
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
                int pos0 = line.IndexOf("[0] = ", StringComparison.Ordinal);
                int pos1 = line.IndexOf("[1] = ", StringComparison.Ordinal);
                int pos2 = line.IndexOf("[2] = ", StringComparison.Ordinal);
                if (pos0 >= 0 && pos1 >= 0 && pos2 >= 0)
                {
                    Paragraph p = new Paragraph();
                    StringBuilder sb = new StringBuilder();

                    for (int i = pos0 + 6; i < line.Length && char.IsDigit(line[i]); i++)
                    {
                        sb.Append(line[i]);
                    }

                    p.StartTime.TotalMilliseconds = int.Parse(sb.ToString());

                    sb = new StringBuilder();
                    for (int i = pos1 + 7; i < line.Length && line[i] != '\''; i++)
                    {
                        sb.Append(line[i]);
                    }

                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                    }

                    for (int i = pos2 + 7; i < line.Length && line[i] != '\''; i++)
                    {
                        sb.Append(line[i]);
                    }

                    p.Text = sb.ToString().Trim();
                    p.Text = WebUtility.HtmlDecode(p.Text);
                    p.Text = ConvertJavaSpecialCharacters(p.Text);
                    subtitle.Paragraphs.Add(p);
                }
            }

            for (int i = 1; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = subtitle.GetParagraphOrDefault(i - 1);
                Paragraph next = subtitle.GetParagraphOrDefault(i);
                if (p != null && next != null)
                {
                    p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds;
                }

                if (!string.IsNullOrEmpty(next.Text))
                {
                    p.EndTime.TotalMilliseconds--;
                }
            }

            for (int i = subtitle.Paragraphs.Count - 1; i >= 0; i--)
            {
                Paragraph p = subtitle.GetParagraphOrDefault(i);
                if (p != null && string.IsNullOrEmpty(p.Text))
                {
                    subtitle.Paragraphs.RemoveAt(i);
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The convert java special characters.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ConvertJavaSpecialCharacters(string s)
        {
            if (s.Contains("&#"))
            {
                for (int i = 33; i < 255; i++)
                {
                    string tag = @"&#" + i + @";";
                    if (s.Contains(tag))
                    {
                        s = s.Replace(tag, Convert.ToChar(i).ToString());
                    }
                }
            }

            return s;
        }
    }
}