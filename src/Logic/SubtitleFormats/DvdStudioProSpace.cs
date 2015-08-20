// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DvdStudioProSpace.cs" company="">
//   
// </copyright>
// <summary>
//   The dvd studio pro space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The dvd studio pro space.
    /// </summary>
    public class DvdStudioProSpace : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d+:\d+:\d+:\d+ , \d+:\d+:\d+:\d+ , .*$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".STL";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "DVD Studio Pro with space";
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
        /// The get frame from milliseconds.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public static byte GetFrameFromMilliseconds(int milliseconds, double frameRate)
        {
            return (byte)Math.Round(milliseconds / (TimeCode.BaseUnit / frameRate));
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
            const string paragraphWriteFormat = "{0} , {1} , {2}\r\n";
            const string timeFormat = "{0:00}:{1:00}:{2:00}:{3:00}";
            const string header = @"$VertAlign          =   Bottom
                                    $Bold               =   FALSE
                                    $Underlined         =   FALSE
                                    $Italic             =   0
                                    $XOffset                =   0
                                    $YOffset                =   -5
                                    $TextContrast           =   15
                                    $Outline1Contrast           =   15
                                    $Outline2Contrast           =   13
                                    $BackgroundContrast     =   0
                                    $ForceDisplay           =   FALSE
                                    $FadeIn             =   0
                                    $FadeOut                =   0
                                    $HorzAlign          =   Center
                                    ";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                double factor = TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate;
                string startTime = string.Format(timeFormat, p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, (int)Math.Round(p.StartTime.Milliseconds / factor));
                string endTime = string.Format(timeFormat, p.EndTime.Hours, p.EndTime.Minutes, p.EndTime.Seconds, (int)Math.Round(p.EndTime.Milliseconds / factor));
                sb.AppendFormat(paragraphWriteFormat, startTime, endTime, DvdStudioPro.EncodeStyles(p.Text));
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
            int number = 0;
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && line[0] != '$' && !line.StartsWith("//"))
                {
                    if (RegexTimeCodes.Match(line).Success)
                    {
                        string[] toPart = line.Substring(0, 25).Split(new[] { " ," }, StringSplitOptions.None);
                        Paragraph p = new Paragraph();
                        if (toPart.Length == 2 && DvdStudioPro.GetTimeCode(p.StartTime, toPart[0]) && DvdStudioPro.GetTimeCode(p.EndTime, toPart[1]))
                        {
                            number++;
                            p.Number = number;
                            string text = line.Substring(27).Trim();
                            p.Text = text.Replace(" | ", Environment.NewLine).Replace("|", Environment.NewLine);
                            p.Text = DvdStudioPro.DecodeStyles(p.Text);
                            subtitle.Paragraphs.Add(p);
                        }
                    }
                    else
                    {
                        this._errorCount++;
                    }
                }
            }
        }
    }
}