﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DvdStudioPro.cs" company="">
//   
// </copyright>
// <summary>
//   The dvd studio pro.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The dvd studio pro.
    /// </summary>
    public class DvdStudioPro : SubtitleFormat
    {
        /// <summary>
        /// The regex time codes.
        /// </summary>
        private static readonly Regex RegexTimeCodes = new Regex(@"^\d+:\d+:\d+:\d+\t,\t\d+:\d+:\d+:\d+\t,\t.*$", RegexOptions.Compiled);

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
                return "DVD Studio Pro";
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
            const string paragraphWriteFormat = "{0}\t,\t{1}\t,\t{2}\r\n";
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
                sb.AppendFormat(paragraphWriteFormat, startTime, endTime, EncodeStyles(p.Text));
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
                if (!string.IsNullOrWhiteSpace(line) && line[0] != '$')
                {
                    if (RegexTimeCodes.Match(line).Success)
                    {
                        string[] threePart = line.Split(new[] { "\t,\t" }, StringSplitOptions.None);
                        Paragraph p = new Paragraph();
                        if (threePart.Length == 3 && GetTimeCode(p.StartTime, threePart[0]) && GetTimeCode(p.EndTime, threePart[1]))
                        {
                            number++;
                            p.Number = number;
                            p.Text = threePart[2].TrimEnd().Replace(" | ", Environment.NewLine).Replace("|", Environment.NewLine);
                            p.Text = DecodeStyles(p.Text);
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

        /// <summary>
        /// The decode styles.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string DecodeStyles(string text)
        {
            StringBuilder sb = new StringBuilder();
            bool italicOn = false;
            bool boldOn = false;
            bool skipNext = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (skipNext)
                {
                    skipNext = false;
                }
                else
                {
                    if (text.Substring(i).StartsWith("^I"))
                    {
                        if (!italicOn)
                        {
                            sb.Append("<i>");
                        }
                        else
                        {
                            sb.Append("</i>");
                        }

                        italicOn = !italicOn;
                        skipNext = true;
                    }
                    else if (text.Substring(i).StartsWith("^B"))
                    {
                        if (!boldOn)
                        {
                            sb.Append("<b>");
                        }
                        else
                        {
                            sb.Append("</b>");
                        }

                        boldOn = !boldOn;
                        skipNext = true;
                    }
                    else
                    {
                        sb.Append(text[i]);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The encode styles.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string EncodeStyles(string text)
        {
            text = Utilities.RemoveSsaTags(text);
            text = text.Replace("<I>", "<i>").Replace("</I>", "</i>");
            bool allItalic = text.StartsWith("<i>") && text.EndsWith("</i>") && Utilities.CountTagInText(text, "<i>") == 1;

            text = text.Replace("<i>", "^I");
            text = text.Replace("<I>", "^I");
            text = text.Replace("</i>", "^I");
            text = text.Replace("</I>", "^I");

            text = text.Replace("<b>", "^B");
            text = text.Replace("<B>", "^B");
            text = text.Replace("</b>", "^B");
            text = text.Replace("</B>", "^B");

            if (allItalic)
            {
                return text.Replace(Environment.NewLine, "|^I");
            }

            return text.Replace(Environment.NewLine, "|");
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <param name="timeString">
        /// The time string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool GetTimeCode(TimeCode timeCode, string timeString)
        {
            try
            {
                string[] timeParts = timeString.Split(':');
                timeCode.Hours = int.Parse(timeParts[0]);
                timeCode.Minutes = int.Parse(timeParts[1]);
                timeCode.Seconds = int.Parse(timeParts[2]);
                timeCode.Milliseconds = FramesToMillisecondsMax999(int.Parse(timeParts[3]));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}