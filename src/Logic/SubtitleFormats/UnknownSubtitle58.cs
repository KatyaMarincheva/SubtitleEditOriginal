// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle58.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 58.
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
    /// The unknown subtitle 58.
    /// </summary>
    public class UnknownSubtitle58 : SubtitleFormat
    {
        // [01:01:53:09]
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\[\d\d:\d\d:\d\d:\d\d]$", RegexOptions.Compiled);

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
                return "Unknown 58";
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
            const string format = "[{0}]{3}{3}{2}{3}{3}[{1}]{3}";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(format, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), p.Text, Environment.NewLine));
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
                arr = rtBox.Text.Replace("\r", string.Empty).Split('\n');
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

            Paragraph p = new Paragraph();
            subtitle.Paragraphs.Clear();
            foreach (string line in arr)
            {
                string s = line.Trim();
                if (s.StartsWith('[') && s.EndsWith('>') && s.Length > 13 && s[12] == ']')
                {
                    s = s.Substring(0, 13);
                }

                Match match = regexTimeCodes.Match(s);
                if (match.Success)
                {
                    string[] parts = s.Replace("[", string.Empty).Replace("]", string.Empty).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 1)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(p.Text))
                            {
                                p.EndTime = DecodeTimeCode(parts[0]);
                                subtitle.Paragraphs.Add(p);
                                p = new Paragraph();
                            }
                            else
                            {
                                p.StartTime = DecodeTimeCode(parts[0]);
                            }
                        }
                        catch (Exception exception)
                        {
                            this._errorCount++;
                            Debug.WriteLine(exception.Message);
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                }
                else
                {
                    p.Text = (p.Text + Environment.NewLine + line).Trim();
                    if (p.Text.Length > 500)
                    {
                        this._errorCount += 10;
                        return;
                    }

                    while (p.Text.Contains(Environment.NewLine + " "))
                    {
                        p.Text = p.Text.Replace(Environment.NewLine + " ", Environment.NewLine);
                    }
                }
            }

            if (!string.IsNullOrEmpty(p.Text))
            {
                subtitle.Paragraphs.Add(p);
            }

            subtitle.RemoveEmptyLines();
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
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="part">
        /// The part.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string part)
        {
            string[] parts = part.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            // 00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
        }
    }
}