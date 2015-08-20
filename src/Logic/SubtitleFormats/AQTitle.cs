// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AQTitle.cs" company="">
//   
// </copyright>
// <summary>
//   The aq title.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The aq title.
    /// </summary>
    public class AQTitle : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".aqt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "AQTitle";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is time based.
        /// </summary>
        public override bool IsTimeBased
        {
            get
            {
                return false;
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
            // -->> 072058
            // <i>Meine Mutter und meine Schwester,

            // -->> 072169

            // -->> 072172
            // <i>die in Zürich lebt, und ich,

            // -->> 072247
            const string paragraphWriteFormat = "-->> {0}{3}{2}{3}-->> {1}{3}{3}";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string text = Utilities.RemoveSsaTags(p.Text);
                int noOfLines = Utilities.GetNumberOfLines(text);
                if (noOfLines > 2)
                {
                    text = Utilities.AutoBreakLine(text);
                }
                else if (noOfLines == 1)
                {
                    text += Environment.NewLine;
                }

                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), text, Environment.NewLine));
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
            Paragraph paragraph = new Paragraph();
            ExpectingLine expecting = ExpectingLine.TimeStart;
            this._errorCount = 0;

            subtitle.Paragraphs.Clear();
            foreach (string line in lines)
            {
                if (line.StartsWith("-->> ", StringComparison.Ordinal))
                {
                    string timePart = line.Substring(4).Trim();
                    if (timePart.Length > 0)
                    {
                        try
                        {
                            TimeCode tc = DecodeTimeCode(timePart);
                            if (expecting == ExpectingLine.TimeStart)
                            {
                                paragraph = new Paragraph();
                                paragraph.StartFrame = int.Parse(timePart);
                                paragraph.StartTime = tc;
                                expecting = ExpectingLine.Text;
                            }
                            else if (expecting == ExpectingLine.TimeEndOrText)
                            {
                                paragraph.EndFrame = int.Parse(timePart);
                                paragraph.EndTime = tc;
                                subtitle.Paragraphs.Add(paragraph);
                                paragraph = new Paragraph();
                                expecting = ExpectingLine.TimeStart;
                            }
                        }
                        catch
                        {
                            this._errorCount++;
                            expecting = ExpectingLine.TimeStart;
                        }
                    }
                }
                else
                {
                    if (expecting == ExpectingLine.Text || expecting == ExpectingLine.TimeEndOrText)
                    {
                        if (line.Length > 0)
                        {
                            string text = line.Replace("|", Environment.NewLine);
                            if (string.IsNullOrEmpty(paragraph.Text))
                            {
                                paragraph.Text = text.Trim();
                            }
                            else
                            {
                                paragraph.Text += Environment.NewLine + text;
                            }

                            if (paragraph.Text.Length > 2000)
                            {
                                this._errorCount += 100;
                                return;
                            }
                        }

                        expecting = ExpectingLine.TimeEndOrText;
                    }
                    else if (expecting == ExpectingLine.TimeStart && !string.IsNullOrWhiteSpace(line))
                    {
                        int ms = (int)paragraph.EndTime.TotalMilliseconds;
                        int frames = paragraph.EndFrame;
                        paragraph = new Paragraph();
                        paragraph.StartTime.TotalMilliseconds = ms;
                        paragraph.StartFrame = frames;
                        paragraph.Text = line.Trim();
                        expecting = ExpectingLine.TimeEndOrText;
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
            int frames = MillisecondsToFrames(time.TotalMilliseconds) + 1;
            return frames.ToString();
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
            int milliseconds = (int)((TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate) * int.Parse(timePart));
            return new TimeCode(milliseconds);
        }

        /// <summary>
        /// The expecting line.
        /// </summary>
        private enum ExpectingLine
        {
            /// <summary>
            /// The time start.
            /// </summary>
            TimeStart, 

            /// <summary>
            /// The text.
            /// </summary>
            Text, 

            /// <summary>
            /// The time end or text.
            /// </summary>
            TimeEndOrText
        }
    }
}