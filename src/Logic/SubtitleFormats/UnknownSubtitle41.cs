// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle41.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 41.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The unknown subtitle 41.
    /// </summary>
    public class UnknownSubtitle41 : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes 1.
        /// </summary>
        private static readonly Regex regexTimeCodes1 = new Regex(@"^\d+.\d$", RegexOptions.Compiled);

        /// <summary>
        /// The regex time codes 2.
        /// </summary>
        private static readonly Regex regexTimeCodes2 = new Regex(@"^\d+.\d\d$", RegexOptions.Compiled);

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
                return "Unknown 41";
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
            const string paragraphWriteFormat = "{0}\r\n{1}\r\n{2}\r\n";
            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), p.Text, EncodeTimeCode(p.EndTime)));
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
            // 911.2
            // C’est l’enfant qui l’a tuée ?
            // 915.8/

            // 921.8
            // Comment elle s’appelait ?
            // 924.6/
            this._errorCount = 0;
            Paragraph p = null;
            bool textOn = false;
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                try
                {
                    if (textOn)
                    {
                        if (regexTimeCodes1.Match(line.TrimEnd('/')).Success || regexTimeCodes2.Match(line).Success)
                        {
                            p.EndTime = DecodeTimeCode(line.TrimEnd('/').Split('.'));
                            if (sb.Length > 0)
                            {
                                p.Text = sb.ToString().TrimEnd();
                                subtitle.Paragraphs.Add(p);
                                textOn = false;
                            }
                        }
                        else
                        {
                            sb.AppendLine(line);
                            if (sb.Length > 500)
                            {
                                this._errorCount += 10;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (regexTimeCodes1.Match(line).Success || regexTimeCodes2.Match(line).Success)
                        {
                            p = new Paragraph();
                            sb.Clear();
                            p.StartTime = DecodeTimeCode(line.Split('.'));
                            textOn = true;
                        }
                    }
                }
                catch
                {
                    textOn = false;
                    this._errorCount++;
                }
            }

            if (textOn && sb.Length > 0)
            {
                p.Text = sb.ToString().TrimEnd();
                subtitle.Paragraphs.Add(p);
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
            Configuration.Settings.General.CurrentFrameRate = 24.0;
            int frames = MillisecondsToFrames(time.TotalMilliseconds);
            int footage = frames / 16;
            int rest = (int)((frames % 16) / 16.0 * Configuration.Settings.General.CurrentFrameRate);
            return string.Format("{0}.{1:0}", footage, rest);
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
            Configuration.Settings.General.CurrentFrameRate = 24.0;
            int frames16 = int.Parse(parts[0]);
            int frames = int.Parse(parts[1]);
            return new TimeCode(0, 0, 0, FramesToMilliseconds(16 * frames16 + frames));
        }
    }
}