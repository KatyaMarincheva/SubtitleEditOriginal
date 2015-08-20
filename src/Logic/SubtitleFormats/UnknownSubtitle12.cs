// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle12.cs" company="">
//   
// </copyright>
// <summary>
//   4.01     5.12
//   Dit is de dag.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     4.01     5.12
    ///     Dit is de dag.
    /// </summary>
    public class UnknownSubtitle12 : SubtitleFormat
    {
        /// <summary>
        /// The _regex time code.
        /// </summary>
        private static readonly Regex _regexTimeCode = new Regex(@"^\d+.\d\d\t\t\d+.\d\d\t*$", RegexOptions.Compiled);

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
                return "Unknown 12";
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
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.Append(MakeTimeCode(p.StartTime));
                sb.Append("\t\t");
                sb.Append(MakeTimeCode(p.EndTime));
                sb.Append("\t\t\n");
                sb.Append(p.Text.Replace(Environment.NewLine, "\n") + "\n\n");
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
            Paragraph p = null;
            StringBuilder text = new StringBuilder();
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (_regexTimeCode.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            p.Text = text.ToString().Trim();
                            subtitle.Paragraphs.Add(p);
                        }

                        string[] arr = s.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        text = new StringBuilder();
                        p = new Paragraph(DecodeTimeCode(arr[0]), DecodeTimeCode(arr[1]), string.Empty);
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (p != null)
                {
                    text.AppendLine(s);
                }
                else
                {
                    this._errorCount++;
                }
            }

            if (p != null && text.Length > 0)
            {
                p.Text = text.ToString().Trim();
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The make time code.
        /// </summary>
        /// <param name="tc">
        /// The tc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MakeTimeCode(TimeCode tc)
        {
            return string.Format("{0:0.00}", tc.TotalSeconds);
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timeCode)
        {
            return TimeCode.FromSeconds(double.Parse(timeCode.Trim()));
        }
    }
}