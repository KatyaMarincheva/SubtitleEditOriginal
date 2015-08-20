// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Titra.cs" company="">
//   
// </copyright>
// <summary>
//   The titra.
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
    /// The titra.
    /// </summary>
    public class Titra : SubtitleFormat
    {
        // * 1 : 01:01:31:19 01:01:33:04 22c
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex regexTimeCodes = new Regex(@"^\* \d+ :\t\d\d:\d\d:\d\d:\d\d\t\d\d:\d\d:\d\d:\d\d\t\d+c", RegexOptions.Compiled);

        /// <summary>
        /// The _max ms div 10.
        /// </summary>
        private int _maxMsDiv10;

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
                return "Titra";
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
            sb.AppendLine(@"TVS - TITRA FILM

Titre VO :   L'heure d'été
Titre VST :
Création :   23/10/2009 - 16:31
Révision :   26/10/2009 - 17:48
Langue VO :  Français
Langue VST : Espagnol
Bobine :     e01

BEWARE : No more than 40 characters ON A LINE
ATTENTION : Pas plus de 40 caractères PAR LIGNE

");
            int index = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                index++;
                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                sb.AppendLine(string.Format("* {0} :\t{1}\t{2}\t{3}{4}{5}", index, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), GetMaxCharsForDuration(p.Duration.TotalSeconds) + "c", Environment.NewLine, text));
                sb.AppendLine();
                if (!text.Contains(Environment.NewLine))
                {
                    sb.AppendLine();
                }
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
            this._maxMsDiv10 = 0;
            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (regexTimeCodes.IsMatch(line))
                {
                    try
                    {
                        string[] arr = line.Split('\t');
                        string start = arr[1];
                        string end = arr[2];

                        string[] startParts = start.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] endParts = end.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (startParts.Length == 4 && endParts.Length == 4)
                        {
                            p = new Paragraph(this.DecodeTimeCode(startParts), this.DecodeTimeCode(endParts), string.Empty);
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                    catch
                    {
                        this._errorCount += 10;
                    }
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    // skip these lines
                }
                else if (p != null)
                {
                    if (string.IsNullOrEmpty(p.Text))
                    {
                        p.Text = line;
                    }
                    else
                    {
                        p.Text = p.Text + Environment.NewLine + line;
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
        /// The get max chars for duration.
        /// </summary>
        /// <param name="durationSeconds">
        /// The duration seconds.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetMaxCharsForDuration(double durationSeconds)
        {
            return (int)Math.Round(15.7 * durationSeconds);
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
        private TimeCode DecodeTimeCode(string[] parts)
        {
            // 00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            int frames = int.Parse(parts[3]);

            if (frames > this._maxMsDiv10)
            {
                this._maxMsDiv10 = frames;
            }

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(frames));
        }
    }
}