// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Csv.cs" company="">
//   
// </copyright>
// <summary>
//   The csv.
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
    /// The csv.
    /// </summary>
    public class Csv : SubtitleFormat
    {
        /// <summary>
        /// The separator.
        /// </summary>
        private const string Separator = ";";

        /// <summary>
        /// The csv line.
        /// </summary>
        private static readonly Regex CsvLine = new Regex(@"^""?\d+""?" + Separator + @"""?\d+""?" + Separator + @"""?\d+""?" + Separator + @"""?[^""]*""?$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".csv";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Csv";
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
            int fine = 0;
            int failed = 0;
            foreach (string line in lines)
            {
                if (CsvLine.IsMatch(line))
                {
                    fine++;
                }
                else
                {
                    failed++;
                }
            }

            return fine > failed;
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
            const string format = "{1}{0}{2}{0}{3}{0}\"{4}\"";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format(format, Separator, "Number", "Start time in milliseconds", "End time in milliseconds", "Text"));
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(format, Separator, p.Number, p.StartTime.TotalMilliseconds, p.EndTime.TotalMilliseconds, p.Text.Replace(Environment.NewLine, "\n")));
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
            bool continuation = false;
            Paragraph p = null;
            foreach (string line in lines)
            {
                if (CsvLine.IsMatch(line))
                {
                    string[] parts = line.Split(Separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        try
                        {
                            int start = Convert.ToInt32(Utilities.FixQuotes(parts[1]));
                            int end = Convert.ToInt32(Utilities.FixQuotes(parts[2]));
                            string text = Utilities.FixQuotes(parts[3]);
                            p = new Paragraph(text, start, end);
                            subtitle.Paragraphs.Add(p);
                            continuation = parts[3].StartsWith('"') && !parts[3].EndsWith('"');
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else
                {
                    if (continuation)
                    {
                        if (p.Text.Length < 300)
                        {
                            p.Text = (p.Text + Environment.NewLine + line.TrimEnd('"')).Trim();
                        }

                        continuation = !line.TrimEnd().EndsWith('"');
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
            }

            subtitle.Renumber();
        }
    }
}