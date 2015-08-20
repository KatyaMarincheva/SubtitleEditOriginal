// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TitleExchangePro.cs" company="">
//   
// </copyright>
// <summary>
//   The title exchange pro.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The title exchange pro.
    /// </summary>
    public class TitleExchangePro : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d,\t[ ]?\d\d:\d\d:\d\d:\d\d,\t[ ]?", RegexOptions.Compiled);

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
                return "TitleExchange Pro";
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

            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            if (sb.ToString().Contains(Environment.NewLine + "SP_NUMBER\tSTART\tEND\tFILE_NAME"))
            {
                return false; // SON
            }

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
                // 00:01:48:22,       00:01:52:17,       - I need those samples, fast!//- Yes, professor.
                string text = p.Text;
                text = text.Replace("<i>", "@Italic@");
                text = text.Replace("</i>", "@Italic@");
                text = text.Replace(Environment.NewLine, "//");
                sb.AppendLine(string.Format("{0},\t{1},\t{2}", EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), HtmlUtil.RemoveHtmlTags(text)));
            }

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
            // 00:01:48:22,       00:01:52:17,       - I need those samples, fast!//- Yes, professor.
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            foreach (string line in lines)
            {
                string s = line.Replace("       ", "\t");
                if (regexTimeCodes.IsMatch(s))
                {
                    string[] temp = s.Split('\t');
                    if (temp.Length > 1)
                    {
                        string start = temp[0].TrimEnd(',');
                        string end = temp[1].TrimEnd(',');

                        string[] startParts = start.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] endParts = end.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (startParts.Length == 4 && endParts.Length == 4)
                        {
                            try
                            {
                                string text = s.Remove(0, regexTimeCodes.Match(s).Length - 1).Trim();
                                if (!text.Contains(Environment.NewLine))
                                {
                                    text = text.Replace("//", Environment.NewLine);
                                }

                                if (text.Contains("@Italic@"))
                                {
                                    bool italicOn = false;
                                    while (text.Contains("@Italic@"))
                                    {
                                        int index = text.IndexOf("@Italic@", StringComparison.Ordinal);
                                        string italicTag = "<i>";
                                        if (italicOn)
                                        {
                                            italicTag = "</i>";
                                        }

                                        text = text.Remove(index, "@Italic@".Length).Insert(index, italicTag);
                                        italicOn = !italicOn;
                                    }

                                    text = HtmlUtil.FixInvalidItalicTags(text);
                                }

                                p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), text);
                                subtitle.Paragraphs.Add(p);
                            }
                            catch (Exception exception)
                            {
                                this._errorCount++;
                                Debug.WriteLine(exception.Message);
                            }
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // skip empty lines
                }
                else if (p != null)
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
            // 00:03:15:22 (last is frame)
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
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
            // 00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
            return tc;
        }
    }
}