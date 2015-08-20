// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle45.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 45.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 45.
    /// </summary>
    public class UnknownSubtitle45 : SubtitleFormat
    {
        // *         00001.00-00003.00 02.01 00.0 1 0001 00 16-090-090
        // *         00138.10-00143.05 00.00 00.0 1 0003 00 16-090-090
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\*\s+\d\d\d\d\d\.\d\d-\d\d\d\d\d\.\d\d \d\d.\d\d \d\d.\d\ \d \d\d\d\d \d\d \d\d-\d\d\d-\d\d\d$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".rtf";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 45";
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
            if (fileName != null && !fileName.EndsWith(this.Extension, StringComparison.OrdinalIgnoreCase))
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"0 2 1.0 1.0 3.0 048 0400 0040 0500 100 100 0 100 0 6600 6600 01
CRULIC R1
ST 0 EB 3.10
@");

            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                // 1 00:50:34:22 00:50:39:13
                // Ich muss dafür sorgen,
                // dass die Epsteins weiterleben
                index++;
                sb.AppendLine(string.Format("*         {0}-{1} 00.00 00.0 1 {2} 00 16-090-090{3}{4}{3}@", EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), index.ToString().PadLeft(4, '0'), Environment.NewLine, HtmlUtil.RemoveHtmlTags(p.Text)));
            }

            RichTextBox rtBox = new RichTextBox();
            rtBox.Text = sb.ToString();
            string rtf = rtBox.Rtf;
            rtBox.Dispose();
            return rtf;
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
            // *         00001.00-00003.00 02.01 00.0 1 0001 00 16-090-090
            // CRULIC R1
            // pour Bobi
            // @
            this._errorCount = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            string rtf = sb.ToString().Trim();
            if (!rtf.StartsWith("{\\rtf"))
            {
                return;
            }

            string[] arr = null;
            RichTextBox rtBox = new RichTextBox();
            try
            {
                rtBox.Rtf = rtf;
                arr = rtBox.Text.Replace("\r\n", "\n").Split('\n');
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return;
            }
            finally
            {
                rtBox.Dispose();
            }

            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            foreach (string line in arr)
            {
                if (regexTimeCodes.IsMatch(line.Trim()))
                {
                    string[] temp = line.Substring(1).Trim().Substring(0, 17).Split('-');
                    if (temp.Length == 2)
                    {
                        string start = temp[0];
                        string end = temp[1];

                        string[] startParts = start.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] endParts = end.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (startParts.Length == 2 && endParts.Length == 2)
                        {
                            p = new Paragraph(DecodeTimeCode(startParts), DecodeTimeCode(endParts), string.Empty);
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line) || line.Trim() == "@")
                {
                    // skip these lines
                }
                else if (!string.IsNullOrWhiteSpace(line) && p != null)
                {
                    if (p.Text.Length > 2000)
                    {
                        return; // wrong format
                    }

                    if (string.IsNullOrEmpty(p.Text))
                    {
                        p.Text = line;
                    }
                    else
                    {
                        p.Text = p.Text + Environment.NewLine + line;
                    }
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
            return string.Format("{0:00000}.{1:00}", time.TotalSeconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
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
            // 00119.12
            string seconds = parts[0];
            string frames = parts[1];
            TimeCode tc = new TimeCode(0, 0, int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
            return tc;
        }
    }
}