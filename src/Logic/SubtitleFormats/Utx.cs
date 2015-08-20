// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utx.cs" company="">
//   
// </copyright>
// <summary>
//   The utx.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The utx.
    /// </summary>
    public class Utx : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".utx";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "UTX";
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
            // I'd forgotten.
            // #0:02:58.21,0:03:00.16

            // Were you somewhere far away?
            // - Yes, four years in Switzerland.
            // #0:03:02.15,0:03:06.14
            const string paragraphWriteFormat = "{0}{1}#{2},{3}{1}";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = p.Text;
                sb.AppendLine(string.Format(paragraphWriteFormat, p.Text, Environment.NewLine, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime)));
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
            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                if (line.StartsWith('#'))
                {
                    string[] timeParts = line.Split(new[] { '#', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (timeParts.Length == 2)
                    {
                        try
                        {
                            TimeCode start = DecodeTimeCode(timeParts[0]);
                            TimeCode end = DecodeTimeCode(timeParts[1]);
                            subtitle.Paragraphs.Add(new Paragraph(start, end, text.ToString().Trim()));
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else if (line.Length > 0)
                {
                    text.AppendLine(line.Trim());
                    if (text.Length > 5000)
                    {
                        return;
                    }
                }
                else
                {
                    text = new StringBuilder();
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
            // 0:03:02.15
            return string.Format("{0}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timePart">
        /// The time part.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timePart)
        {
            // 0:03:02.15
            string[] parts = timePart.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);

            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int milliseconds = (int)((TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate) * int.Parse(parts[3]));
            return new TimeCode(hours, minutes, seconds, milliseconds);
        }
    }
}