// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RealTime.cs" company="">
//   
// </copyright>
// <summary>
//   The real time.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The real time.
    /// </summary>
    public class RealTime : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".rt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RealTime";
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
            int index = 0;
            sb.AppendLine("<Window" + Environment.NewLine + "  Width    = \"640\"" + Environment.NewLine + "  Height   = \"480\"" + Environment.NewLine + "  WordWrap = \"true\"" + Environment.NewLine + "  Loop     = \"true\"" + Environment.NewLine + "  bgcolor  = \"black\"" + Environment.NewLine + ">" + Environment.NewLine + "<Font" + Environment.NewLine + "  Color = \"white\"" + Environment.NewLine + "  Face  = \"Arial\"" + Environment.NewLine + "  Size  = \"+2\"" + Environment.NewLine + ">" + Environment.NewLine + "<center>" + Environment.NewLine + "<b>" + Environment.NewLine);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                // <Time begin="0:03:24.8" end="0:03:29.4" /><clear/>Man stjæler ikke fra Chavo, nej.
                sb.AppendLine(string.Format("<Time begin=\"{0}\" end=\"{1}\" /><clear/>{2}", EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), p.Text.Replace(Environment.NewLine, " ")));
                index++;
            }

            sb.AppendLine("</b>");
            sb.AppendLine("</center>");
            return sb.ToString();
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
            // <Time begin="0:03:24.8" end="0:03:29.4" /><clear/>Man stjæler ikke fra Chavo, nej.
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            foreach (string line in lines)
            {
                try
                {
                    if (line.Contains("<Time ") && line.Contains(" begin=") && line.Contains("end="))
                    {
                        int indexOfBegin = line.IndexOf(" begin=", StringComparison.Ordinal);
                        int indexOfEnd = line.IndexOf(" end=", StringComparison.Ordinal);
                        string begin = line.Substring(indexOfBegin + 7, 11);
                        string end = line.Substring(indexOfEnd + 5, 11);

                        string[] startParts = begin.Split(new[] { ':', '.', '"' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] endParts = end.Split(new[] { ':', '.', '"' }, StringSplitOptions.RemoveEmptyEntries);
                        if (startParts.Length == 4 && endParts.Length == 4)
                        {
                            string text = line.Substring(line.LastIndexOf("/>", StringComparison.Ordinal) + 2);
                            Paragraph p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), text);
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                }
                catch
                {
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTimeCode(TimeCode time)
        {
            // 0:03:24.8
            return string.Format("{0:0}:{1:00}:{2:00}.{3:0}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 100);
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string[] parts)
        {
            // [00:06:51.48]
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int millisesonds = int.Parse(parts[3]);

            return new TimeCode(hour, minutes, seconds, millisesonds * 10);
        }
    }
}