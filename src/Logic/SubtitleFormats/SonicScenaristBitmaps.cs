// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SonicScenaristBitmaps.cs" company="">
//   
// </copyright>
// <summary>
//   The sonic scenarist bitmaps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The sonic scenarist bitmaps.
    /// </summary>
    public class SonicScenaristBitmaps : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\d\d\d\d\s+\d\d:\d\d:\d\d:\d\d\s+\d\d:\d\d:\d\d:\d\d\s+.+\.(tif|tiff|png|bmp|TIF|TIFF|PNG|BMP)", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".sst";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Sonic Scenarist Bitmaps";
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
            if (Path.GetExtension(fileName).ToLowerInvariant() != ".sst")
            {
                return false;
            }

            Subtitle subtitle = new Subtitle();
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
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
            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                // 0001       00:49:26:22  00:49:27:13  t01_v001c001_22_0001.bmp
                sb.AppendLine(string.Format("{0:0000}       {1}  {2}  {3}", index + 1, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), HtmlUtil.RemoveHtmlTags(p.Text).Replace(Environment.NewLine, "\t")));
                index++;
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
            // 00:03:15:22 00:03:23:10 This is line one.
            // This is line two.
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            int index = 0;
            foreach (string line in lines)
            {
                if (regexTimeCodes.IsMatch(line))
                {
                    string temp = line.Substring(0, regexTimeCodes.Match(line).Length);

                    string[] parts = temp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 3)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 3; i < parts.Length; i++)
                        {
                            sb.Append(parts[i] + " ");
                        }

                        string text = sb.ToString().Trim();
                        p = new Paragraph(DecodeTimeCode(parts[1].Split(':')), DecodeTimeCode(parts[2].Split(':')), text);
                        subtitle.Paragraphs.Add(p);
                    }
                }
                else if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Display_Area") || line.StartsWith('#') || line.StartsWith("Color") || index < 10)
                {
                    // skip these lines
                }
                else if (p != null)
                {
                    this._errorCount++;
                }

                index++;
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
            return time.ToHHMMSSFF();
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

            int milliseconds = FramesToMillisecondsMax999(double.Parse(frames));
            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), milliseconds);
        }
    }
}