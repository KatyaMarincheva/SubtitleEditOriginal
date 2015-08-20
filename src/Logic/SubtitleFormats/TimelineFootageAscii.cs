﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimelineFootageAscii.cs" company="">
//   
// </copyright>
// <summary>
//   Timeline Ascii export - THE MOVIE TITRE EDITOR - http://www.pld.ttu.ee/~priidu/timeline/ by priidu@pld.ttu.ee
//   Sample:
//   1.
//   41,10
//   46,10
//   ±NE/SEVÎ
//   ³ÂÍÅ/ÑÅÁß
//   2.
//   49,05
//   51,09
//   ±Viòð ir klât.
//   ³Îí ïðèøåë.
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
    ///     Timeline Ascii export - THE MOVIE TITRE EDITOR - http://www.pld.ttu.ee/~priidu/timeline/ by priidu@pld.ttu.ee
    ///     Sample:
    ///     1.
    ///     41,10
    ///     46,10
    ///     ±NE/SEVÎ
    ///     ³ÂÍÅ/ÑÅÁß
    ///     2.
    ///     49,05
    ///     51,09
    ///     ±Viòð ir klât.
    ///     ³Îí ïðèøåë.
    /// </summary>
    public class TimeLineFootageAscii : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        private static readonly Regex RegexTimeCode = new Regex(@"^\s*\d+,\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".asc";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Timeline footage ascii";
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
            if (fileName == null || !fileName.EndsWith(this.Extension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

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
            return string.Empty;
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
            Paragraph paragraph = null;
            ExpectingLine expecting = ExpectingLine.Number;
            this._errorCount = 0;
            byte firstLineCode = 0;
            byte secondLineCode = 0;

            subtitle.Paragraphs.Clear();
            IEnumerable<byte[]> byteLines = this.SplitBytesToLines(File.ReadAllBytes(fileName));
            foreach (byte[] bytes in byteLines)
            {
                string line = Encoding.ASCII.GetString(bytes);
                if (line.EndsWith('.') && Utilities.IsInteger(line.TrimEnd('.')))
                {
                    if (paragraph != null && !string.IsNullOrEmpty(paragraph.Text))
                    {
                        subtitle.Paragraphs.Add(paragraph);
                    }

                    paragraph = new Paragraph();
                    expecting = ExpectingLine.TimeStart;
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeStart && RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            paragraph.StartTime = tc;
                            expecting = ExpectingLine.TimeEnd;
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.Number;
                        }
                    }
                }
                else if (paragraph != null && expecting == ExpectingLine.TimeEnd && RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(parts);
                            paragraph.EndTime = tc;
                            expecting = ExpectingLine.Text;
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.Number;
                        }
                    }
                }
                else
                {
                    if (paragraph != null && expecting == ExpectingLine.Text)
                    {
                        if (bytes.Length > 1)
                        {
                            // get text from encoding
                            Encoding enc = this.GetEncodingFromLanguage(bytes[0]);
                            string s = enc.GetString(bytes, 1, bytes.Length - 1).Trim();

                            // italic text
                            if (s.StartsWith('#'))
                            {
                                s = "<i>" + s.Remove(0, 1) + "</i>";
                            }

                            paragraph.Text = (paragraph.Text + Environment.NewLine + s).Trim();
                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                return;
                            }

                            if (paragraph.Text.Contains(Environment.NewLine))
                            {
                                if (secondLineCode == 0)
                                {
                                    secondLineCode = bytes[0];
                                }

                                if (secondLineCode != bytes[0])
                                {
                                    this._errorCount++;
                                }
                            }
                            else
                            {
                                if (firstLineCode == 0)
                                {
                                    firstLineCode = bytes[0];
                                }

                                if (firstLineCode != bytes[0])
                                {
                                    this._errorCount++;
                                }
                            }
                        }
                    }
                }
            }

            if (paragraph != null && !string.IsNullOrEmpty(paragraph.Text))
            {
                subtitle.Paragraphs.Add(paragraph);
            }

            subtitle.Renumber();
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
            int frames16 = int.Parse(parts[0]);
            int frames = int.Parse(parts[1]);
            return new TimeCode(0, 0, 0, FramesToMilliseconds(16 * frames16 + frames));
        }

        /// <summary>
        /// The split bytes to lines.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<byte[]> SplitBytesToLines(byte[] bytes)
        {
            List<byte[]> list = new List<byte[]>();
            int start = 0;
            int index = 0;
            while (index < bytes.Length)
            {
                if (bytes[index] == 13)
                {
                    int length = index - start;
                    byte[] lineBytes = new byte[length];
                    Array.Copy(bytes, start, lineBytes, 0, length);
                    list.Add(lineBytes);
                    index += 2;
                    start = index;
                }
                else
                {
                    index++;
                }
            }

            return list;
        }

        /// <summary>
        /// The get encoding from language.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        private Encoding GetEncodingFromLanguage(byte language)
        {
            if (language == 179)
            {
                // Russian
                return Encoding.GetEncoding(1251);
            }

            if (language == 177)
            {
                // Baltic
                return Encoding.GetEncoding(1257);
            }

            return Encoding.GetEncoding(1252);
        }

        /// <summary>
        /// The expecting line.
        /// </summary>
        private enum ExpectingLine
        {
            /// <summary>
            /// The number.
            /// </summary>
            Number, 

            /// <summary>
            /// The time start.
            /// </summary>
            TimeStart, 

            /// <summary>
            /// The time end.
            /// </summary>
            TimeEnd, 

            /// <summary>
            /// The text.
            /// </summary>
            Text
        }
    }
}