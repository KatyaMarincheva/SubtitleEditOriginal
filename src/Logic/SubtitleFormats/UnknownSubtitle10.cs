﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle10.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 10.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 10.
    /// </summary>
    public class UnknownSubtitle10 : SubtitleFormat
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
                return "Unknown 10";
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
            sb.Append("{\"language_code\":\"en\",\"subtitles\":[");
            int i = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append('{');
                sb.AppendFormat("\"content\":\"{0}\",\"start_time\":{1},\"end_time\":{2}", p.Text.Replace(Environment.NewLine, " <br> "), p.StartTime.TotalMilliseconds, p.EndTime.TotalMilliseconds);
                sb.Append('}');
                i++;
            }

            sb.Append("]}");
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
            StringBuilder temp = new StringBuilder();
            foreach (string l in lines)
            {
                temp.Append(l);
            }

            string all = temp.ToString();
            if (!all.Contains("{\"content\":\""))
            {
                return;
            }

            string[] arr = all.Replace("\n", string.Empty).Replace("{\"content\":\"", "\n").Split('\n');

            this._errorCount = 0;
            subtitle.Paragraphs.Clear();

            // {"content":"La ce se gandeste  Oh Ha Ni a noastra <br> de la inceputul dimineti?","start_time":314071,"end_time":317833},
            for (int i = 0; i < arr.Length; i++)
            {
                string line = arr[i].Trim();

                int indexStartTime = line.IndexOf("\"start_time\":", StringComparison.Ordinal);
                int indexEndTime = line.IndexOf("\"end_time\":", StringComparison.Ordinal);
                if (indexStartTime > 0 && indexEndTime > 0)
                {
                    int indexEndText = indexStartTime;
                    if (indexStartTime > indexEndTime)
                    {
                        indexEndText = indexEndTime;
                    }

                    string text = line.Substring(0, indexEndText - 1).Trim().TrimEnd('\"');
                    text = text.Replace("<br>", Environment.NewLine).Replace("<BR>", Environment.NewLine);
                    text = text.Replace("<br/>", Environment.NewLine).Replace("<BR/>", Environment.NewLine);
                    text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                    text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                    text = text.Replace(" " + Environment.NewLine, Environment.NewLine);
                    try
                    {
                        string start = line.Substring(indexStartTime);
                        string end = line.Substring(indexEndTime);
                        Paragraph paragraph = new Paragraph { Text = text, StartTime = { TotalMilliseconds = GetMilliseconds(start) }, EndTime = { TotalMilliseconds = GetMilliseconds(end) } };
                        subtitle.Paragraphs.Add(paragraph);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                        this._errorCount++;
                    }
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get milliseconds.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double GetMilliseconds(string start)
        {
            while (start.Length > 1 && !start.StartsWith(':'))
            {
                start = start.Remove(0, 1);
            }

            start = start.Trim().Trim(':').Trim('"').Trim();

            int i = 0;
            while (i < start.Length && char.IsDigit(start[i]))
            {
                i++;
            }

            return int.Parse(start.Substring(0, i));
        }
    }
}