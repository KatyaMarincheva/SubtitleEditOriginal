// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheetahCaption.cs" company="">
//   
// </copyright>
// <summary>
//   The cheetah caption.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The cheetah caption.
    /// </summary>
    public class CheetahCaption : SubtitleFormat
    {
        /// <summary>
        /// The name of format.
        /// </summary>
        public const string NameOfFormat = "Cheetah Caption";

        /// <summary>
        /// The latin codes.
        /// </summary>
        private static readonly List<int> LatinCodes = new List<int> { 0x81, // ♪
                                                                       0x82, // á
                                                                       0x83, // é
                                                                       0x84, // í
                                                                       0x85, // ó
                                                                       0x86, // ú
                                                                       0x87, // â
                                                                       0x88, // ê
                                                                       0x89, // î
                                                                       0x8A, // ô
                                                                       0x8B, // û
                                                                       0x8C, // à
                                                                       0x8D, // è
                                                                       0x8E, // Ñ
                                                                       0x8F, // ñ
                                                                       0x90, // ç
                                                                       0x91, // ¢
                                                                       0x92, // £
                                                                       0x93, // ¿
                                                                       0x94, // ½
                                                                       0x95 // ®
                                                                     };

        /// <summary>
        /// The latin letters.
        /// </summary>
        private static readonly List<string> LatinLetters = new List<string> { "♪", "á", "é", "í", "ó", "ú", "â", "ê", "î", "ô", "û", "à", "è", "Ñ", "ñ", "ç", "¢", "£", "¿", "½", "®" };

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".cap";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return NameOfFormat;
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
        /// The save.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public static void Save(string fileName, Subtitle subtitle)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = { 0xEA, 0x22, 1, 0 }; // header
                fs.Write(buffer, 0, buffer.Length);

                int numberOfLines = subtitle.Paragraphs.Count;
                fs.WriteByte((byte)(numberOfLines % 256)); // paragraphs - low byte
                fs.WriteByte((byte)(numberOfLines / 256)); // paragraphs - high byte

                buffer = new byte[] { 9, 0xA8, 0xAF, 0x4F }; // ?
                fs.Write(buffer, 0, buffer.Length);

                for (int i = 0; i < 118; i++)
                {
                    fs.WriteByte(0);
                }

                // paragraphs
                for (int index = 0; index < subtitle.Paragraphs.Count; index++)
                {
                    Paragraph p = subtitle.Paragraphs[index];
                    Paragraph next = subtitle.GetParagraphOrDefault(index + 1);
                    string text = p.Text;

                    byte[] bufferShort = { 0, 0, 3, // justification, 1=left, 2=right, 3=center
                                           0xE, // horizontal position, 1=top, F=bottom
                                           0x10 // horizontal position, 3=left, 0x10=center, 0x19=right
                                         };

                    // styles + ?
                    buffer = new byte[] { 0x12, 1, 0, 0, 0, 0, 3, // justification, 1=left, 2=right, 3=center
                                          0xF, // horizontal position, 1=top, F=bottom
                                          0x10 // horizontal position, 3=left, 0x10=center, 0x19=right
                                        };

                    // Normal        : 12 01 00 00 00 00 03 0F 10
                    // Right-top     : 12 01 00 00 00 00 03 01 1C
                    // Top           : 12 01 00 00 00 00 03 01 10
                    // Left-top      : 12 01 00 00 00 00 03 01 05
                    // Left          : 12 01 00 00 00 00 03 0F 0A
                    // Right         : 12 01 00 00 00 00 03 0F 1E
                    // Left          : 12 03 00 00 00 00 03 0F 07
                    if (text.StartsWith("{\\an7}") || text.StartsWith("{\\an8}") || text.StartsWith("{\\an9}"))
                    {
                        buffer[7] = 1; // align top (vertial)
                        bufferShort[3] = 1; // align top (vertial)
                    }
                    else if (text.StartsWith("{\\an4}") || text.StartsWith("{\\an5}") || text.StartsWith("{\\an6}"))
                    {
                        buffer[7] = 8; // center (vertical)
                        bufferShort[3] = 8; // align top (vertial)
                    }

                    if (text.StartsWith("{\\an7}") || text.StartsWith("{\\an4}") || text.StartsWith("{\\an1}"))
                    {
                        buffer[8] = 2; // align left (horizontal)
                        bufferShort[4] = 2; // align left (horizontal)
                    }
                    else if (text.StartsWith("{\\an9}") || text.StartsWith("{\\an6}") || text.StartsWith("{\\an3}"))
                    {
                        buffer[8] = 0x1e; // align right (vertical)
                        bufferShort[4] = 0x1e; // align right (vertical)
                    }

                    int startTag = text.IndexOf('}');
                    if (text.StartsWith("{\\") && startTag > 0 && startTag < 10)
                    {
                        text = text.Remove(0, startTag + 1);
                    }

                    List<byte> textBytes = new List<byte>();
                    bool italic = p.Text.StartsWith("<i>") && p.Text.EndsWith("</i>");
                    text = HtmlUtil.RemoveHtmlTags(text);
                    int j = 0;
                    if (italic)
                    {
                        textBytes.Add(0xd0);
                    }

                    while (j < text.Length)
                    {
                        if (text.Substring(j).StartsWith(Environment.NewLine))
                        {
                            j += Environment.NewLine.Length;
                            textBytes.Add(0);
                            textBytes.Add(0);
                            textBytes.Add(0);
                            textBytes.Add(0);
                            if (italic)
                            {
                                textBytes.Add(0xd0);
                            }
                        }
                        else
                        {
                            int idx = LatinLetters.IndexOf(text.Substring(j, 1));
                            if (idx >= 0)
                            {
                                textBytes.Add((byte)LatinCodes[idx]);
                            }
                            else
                            {
                                textBytes.Add(Encoding.GetEncoding(1252).GetBytes(new[] { text[j] })[0]);
                            }

                            j++;
                        }
                    }

                    int length = textBytes.Count + 20;
                    long end = fs.Position + length;
                    if (Configuration.Settings.SubtitleSettings.CheetahCaptionAlwayWriteEndTime || (next != null && next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds >= 1500))
                    {
                        fs.WriteByte((byte)length);

                        if (p.Text.Trim().Contains(Environment.NewLine))
                        {
                            fs.WriteByte(0x62); // two lines?
                        }
                        else
                        {
                            fs.WriteByte(0x61); // one line?
                        }

                        WriteTime(fs, p.StartTime);
                        WriteTime(fs, p.EndTime);
                        fs.Write(buffer, 0, buffer.Length); // styles
                    }
                    else
                    {
                        length = textBytes.Count + 20 - (buffer.Length - bufferShort.Length);
                        end = fs.Position + length;
                        fs.WriteByte((byte)length);

                        if (p.Text.Trim().Contains(Environment.NewLine))
                        {
                            fs.WriteByte(0x42); // two lines?
                        }
                        else
                        {
                            fs.WriteByte(0x41); // one line?
                        }

                        WriteTime(fs, p.StartTime);
                        fs.WriteByte(2);
                        fs.WriteByte(1);
                        fs.WriteByte(0);
                        fs.WriteByte(0);
                        fs.Write(bufferShort, 0, bufferShort.Length); // styles
                    }

                    foreach (byte b in textBytes)
                    {
                        // text
                        fs.WriteByte(b);
                    }

                    while (end > fs.Position)
                    {
                        fs.WriteByte(0);
                    }
                }
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
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.Length >= 200 && fi.Length < 1024000)
                {
                    // not too small or too big
                    if (fileName.EndsWith(".cap", StringComparison.OrdinalIgnoreCase))
                    {
                        byte[] buffer = FileUtil.ReadAllBytesShared(fileName);
                        for (int i = 0; i < buffer.Length - 20; i++)
                        {
                            if (buffer[i + 0] == 0xEA && buffer[i + 1] == 0x22 && buffer[i + 2] <= 3)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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
            return "Not supported!";
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
            subtitle.Paragraphs.Clear();
            subtitle.Header = null;
            byte[] buffer = FileUtil.ReadAllBytesShared(fileName);

            int i = 128;
            Paragraph last = null;
            while (i < buffer.Length - 20)
            {
                Paragraph p = new Paragraph();
                int length = buffer[i];
                int textLength = length - 20;
                int start = 19;
                for (int j = 0; j < 4; j++)
                {
                    if (buffer[i + start - 1] > 0x10)
                    {
                        start--;
                        textLength++;
                    }
                }

                if (textLength > 0 && buffer.Length >= i + textLength)
                {
                    p.StartTime = DecodeTimestamp(buffer, i + 2);

                    if (last != null && last.EndTime.TotalMilliseconds > p.StartTime.TotalMilliseconds)
                    {
                        last.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                    }

                    p.EndTime = DecodeTimestamp(buffer, i + 6);

                    StringBuilder sb = new StringBuilder();
                    int j = 0;
                    bool italics = false;
                    while (j < textLength)
                    {
                        int index = i + start + j;
                        if (buffer[index] == 0)
                        {
                            if (italics)
                            {
                                sb.Append("</i>");
                            }

                            italics = false;
                            if (!sb.ToString().EndsWith(Environment.NewLine))
                            {
                                sb.AppendLine();
                            }
                        }
                        else if (LatinCodes.Contains(buffer[index]))
                        {
                            sb.Append(LatinLetters[LatinCodes.IndexOf(buffer[index])]);
                        }
                        else if (buffer[index] >= 0xC0 || buffer[index] <= 0x14)
                        {
                            // codes/styles?
                            if (buffer[index] == 0xd0)
                            {
                                // italics
                                italics = true;
                                sb.Append("<i>");
                            }
                        }
                        else
                        {
                            sb.Append(Encoding.GetEncoding(1252).GetString(buffer, index, 1));
                        }

                        j++;
                    }

                    if (italics)
                    {
                        sb.Append("</i>");
                    }

                    p.Text = sb.ToString().Trim();
                    p.Text = p.Text.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine);

                    subtitle.Paragraphs.Add(p);
                    last = p;
                }

                if (length == 0)
                {
                    length++;
                }

                i += length;
            }

            if (last != null && last.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
            {
                last.EndTime.TotalMilliseconds = last.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(last.Text);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The write time.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        private static void WriteTime(FileStream fs, TimeCode timeCode)
        {
            fs.WriteByte((byte)timeCode.Hours);
            fs.WriteByte((byte)timeCode.Minutes);
            fs.WriteByte((byte)timeCode.Seconds);
            fs.WriteByte((byte)MillisecondsToFramesMaxFrameRate(timeCode.Milliseconds));
        }

        /// <summary>
        /// The decode timestamp.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimestamp(byte[] buffer, int index)
        {
            return new TimeCode(buffer[index], buffer[index + 1], buffer[index + 2], FramesToMillisecondsMax999(buffer[index + 3]));
        }
    }
}