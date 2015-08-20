// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmilTimesheetData.cs" company="">
//   
// </copyright>
// <summary>
//   http://wam.inrialpes.fr/timesheets/annotations/video.html
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    ///     http://wam.inrialpes.fr/timesheets/annotations/video.html
    /// </summary>
    public class SmilTimesheetData : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".html";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SMIL Timesheet";
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
            StringBuilder sb = new StringBuilder();
            foreach (string l in lines)
            {
                sb.AppendLine(l);
            }

            if (!sb.ToString().Contains(" data-begin"))
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
            string header = @"<!DOCTYPE html>
                                <html>
                                <head>" + Environment.NewLine + "\t<meta charset=\"utf-8\">" + Environment.NewLine + "\t<title>SMIL Timesheet</title>" + @"
                                </head>
                                <body>" + Environment.NewLine + "\t<div id=\"media\" data-timecontainer=\"excl\" data-timeaction=\"display\">" + Environment.NewLine + "\t\t<video data-syncmaster=\"true\" data-timeaction=\"none\" controls=\"true\" preload=\"auto\">" + Environment.NewLine + "\t\t\t<source type=\"video/webm\" src=\"_TITLE_.webm\"/>" + Environment.NewLine + "\t\t\t<source type=\"video/ogg\"  src=\"_TITLE_.ogv\" />" + Environment.NewLine + "\t\t\t<source type=\"video/mp4\"  src=\"_TITLE_.mp4\" />" + Environment.NewLine + "\t\t\tYour browser does not support the HTML5 &lt;video&gt; tag" + Environment.NewLine + "\t\t</video>" + Environment.NewLine;

            const string paragraphWriteFormatOpen = "\t\t<p data-begin=\"{0}\">\r\n{1}\r\n</p>";
            const string paragraphWriteFormat = "\t\t<p data-begin=\"{0}\" data-end=\"{1}\">\r\n{2}\r\n</p>";
            int count = 1;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header.Replace("_TITLE_", title));
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                Paragraph next = subtitle.GetParagraphOrDefault(count);
                string text = p.Text;
                if (next != null && Math.Abs(next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds) < 100)
                {
                    sb.AppendLine(string.Format(paragraphWriteFormatOpen, EncodeTime(p.StartTime), text));
                }
                else
                {
                    sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTime(p.StartTime), EncodeTime(p.EndTime), text));
                }

                count++;
            }

            sb.AppendLine("\t</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
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
            StringBuilder sb = new StringBuilder();
            foreach (string l in lines)
            {
                sb.AppendLine(l);
            }

            string allInput = sb.ToString();
            string allInputLower = allInput.ToLower();
            const string syncTag = "<p ";
            int syncStartPos = allInputLower.IndexOf(syncTag, StringComparison.Ordinal);
            int index = syncStartPos + syncTag.Length;
            while (syncStartPos >= 0)
            {
                int syncEndPos = allInputLower.IndexOf("</p>", index, StringComparison.Ordinal);
                if (syncEndPos > 0)
                {
                    string s = allInput.Substring(syncStartPos + 2, syncEndPos - syncStartPos - 2);
                    int indexOfBegin = s.IndexOf(" data-begin=", StringComparison.Ordinal);
                    int indexOfAttributesEnd = s.IndexOf('>');
                    if (indexOfBegin >= 0 && indexOfAttributesEnd > indexOfBegin)
                    {
                        string text = s.Substring(indexOfAttributesEnd + 1).Trim();
                        text = text.Replace("<br>", Environment.NewLine);
                        text = text.Replace("<br/>", Environment.NewLine);
                        text = text.Replace("<br />", Environment.NewLine);
                        text = text.Replace("\t", " ");
                        while (text.Contains("  "))
                        {
                            text = text.Replace("  ", " ");
                        }

                        while (text.Contains(Environment.NewLine + " "))
                        {
                            text = text.Replace(Environment.NewLine + " ", Environment.NewLine);
                        }

                        while (text.Contains(Environment.NewLine + Environment.NewLine))
                        {
                            text = text.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
                        }

                        string begin = s.Substring(indexOfBegin + " data-begin=".Length);
                        StringBuilder tcBegin = new StringBuilder();
                        for (int i = 0; i <= 10; i++)
                        {
                            if (begin.Length > i && @"0123456789:.".Contains(begin[i]))
                            {
                                tcBegin.Append(begin[i]);
                            }
                        }

                        StringBuilder tcEnd = new StringBuilder();
                        int indexOfEnd = s.IndexOf(" data-end=", StringComparison.Ordinal);
                        if (indexOfEnd >= 0)
                        {
                            string end = s.Substring(indexOfEnd + " data-end=".Length);
                            for (int i = 0; i <= 10; i++)
                            {
                                if (end.Length > i && @"0123456789:.".Contains(end[i]))
                                {
                                    tcEnd.Append(end[i]);
                                }
                            }
                        }

                        string[] arr = tcBegin.ToString().Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 3 || arr.Length == 4)
                        {
                            Paragraph p = new Paragraph();
                            p.Text = text;
                            try
                            {
                                p.StartTime = DecodeTimeCode(arr);
                                if (tcEnd.Length > 0)
                                {
                                    arr = tcEnd.ToString().Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    p.EndTime = DecodeTimeCode(arr);
                                }

                                subtitle.Paragraphs.Add(p);
                            }
                            catch
                            {
                                this._errorCount++;
                            }
                        }
                    }
                }

                if (syncEndPos <= 0)
                {
                    syncStartPos = -1;
                }
                else
                {
                    syncStartPos = allInputLower.IndexOf(syncTag, syncEndPos, StringComparison.Ordinal);
                    index = syncStartPos + syncTag.Length;
                }
            }

            index = 1;
            foreach (Paragraph paragraph in subtitle.Paragraphs)
            {
                Paragraph next = subtitle.GetParagraphOrDefault(index);
                if (next != null)
                {
                    paragraph.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                }
                else if (paragraph.EndTime.TotalMilliseconds < 50)
                {
                    paragraph.EndTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(paragraph.Text);
                }

                if (paragraph.Duration.TotalMilliseconds > Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds)
                {
                    paragraph.EndTime.TotalMilliseconds = paragraph.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(paragraph.Text);
                }

                index++;
            }

            foreach (Paragraph p2 in subtitle.Paragraphs)
            {
                p2.Text = WebUtility.HtmlDecode(p2.Text);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode time.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTime(TimeCode time)
        {
            // 3:15:22
            if (time.Hours > 0)
            {
                return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 10);
            }

            if (time.Minutes > 9)
            {
                return string.Format("{0:00}:{1:00}.{2:00}", time.Minutes, time.Seconds, time.Milliseconds / 10);
            }

            return string.Format("{0}:{1:00}.{2:00}", time.Minutes, time.Seconds, time.Milliseconds / 10);
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string[] s)
        {
            if (s.Length == 3)
            {
                return new TimeCode(0, int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]) * 10);
            }

            return new TimeCode(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]) * 10);
        }
    }
}