// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UleadSubtitleFormat.cs" company="">
//   
// </copyright>
// <summary>
//   The ulead subtitle format.
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
    /// The ulead subtitle format.
    /// </summary>
    public class UleadSubtitleFormat : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^#\d+ \d\d;\d\d;\d\d;\d\d \d\d;\d\d;\d\d;\d\d", RegexOptions.Compiled);

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
                return "Ulead subtitle format";
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
            const string Header = @"#Ulead subtitle format

#Subtitle stream attribute begin
#FR:25.00
#Subtitle stream attribute end

#Subtitle text begin";

            const string Footer = @"#Subtitle text end
#Subtitle text attribute begin
#/R:1,{0} /FP:8  /FS:24
#Subtitle text attribute end";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Header);
            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                // #3 00;04;26;04 00;04;27;05
                // How much in there? -
                // Three...
                sb.AppendLine(string.Format("#{0} {1} {2}", index, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime)));
                sb.AppendLine(HtmlUtil.RemoveHtmlTags(p.Text));
                index++;
            }

            sb.AppendLine(string.Format(Footer, subtitle.Paragraphs.Count));
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
            // #3 00;04;26;04 00;04;27;05
            // How much in there? -
            // Three...
            Paragraph p = null;
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            foreach (string l2 in lines)
            {
                string line = l2.TrimEnd('ഀ');
                if (line.StartsWith('#') && regexTimeCodes.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        string start = parts[1];
                        string end = parts[2];
                        try
                        {
                            p = new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), string.Empty);
                            subtitle.Paragraphs.Add(p);
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                {
                    // skip these lines
                }
                else if (p != null)
                {
                    if (string.IsNullOrEmpty(p.Text))
                    {
                        p.Text = line.TrimEnd();
                    }
                    else
                    {
                        p.Text = p.Text + Environment.NewLine + line.TrimEnd();
                    }
                }
                else
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
            // 00;04;27;05
            int frames = time.Milliseconds / (1000 / 25);
            return string.Format("{0:00};{1:00};{2:00};{3:00}", time.Hours, time.Minutes, time.Seconds, frames);
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string time)
        {
            // 00;04;26;04
            string hour = time.Substring(0, 2);
            string minutes = time.Substring(3, 2);
            string seconds = time.Substring(6, 2);
            string frames = time.Substring(9, 2);

            int milliseconds = (int)((1000 / 25.0) * int.Parse(frames));
            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), milliseconds);
            return tc;
        }
    }
}