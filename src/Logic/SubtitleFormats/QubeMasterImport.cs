﻿namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    public class QubeMasterImport : SubtitleFormat
    {
        // ToText code by Tosil Velkoff, tosil@velkoff.net
        // Based on UnknownSubtitle44
        // SubLine1
        // SubLine2
        // 10:01:04:12
        // 10:01:07:09
        private static readonly Regex regexTimeCodes1 = new Regex(@"^\d\d:\d\d:\d\d:\d\d$", RegexOptions.Compiled);

        public override string Extension
        {
            get
            {
                return ".txt";
            }
        }

        public override string Name
        {
            get
            {
                return "QubeMasterPro Import";
            }
        }

        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

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

        public override string ToText(Subtitle subtitle, string title)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (index != 0)
                {
                    sb.AppendLine();
                }

                index++;

                sb.AppendLine(HtmlUtil.RemoveHtmlTags(p.Text));
                sb.AppendLine(EncodeTimeCode(p.StartTime));
                sb.AppendLine(EncodeTimeCode(p.EndTime));
            }

            return sb.ToString();
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            bool expectStartTime = true;
            Paragraph p = new Paragraph();
            subtitle.Paragraphs.Clear();
            this._errorCount = 0;
            foreach (string line in lines)
            {
                string s = line.Trim();
                Match match = regexTimeCodes1.Match(s);
                if (match.Success)
                {
                    string[] parts = s.Split(':');
                    if (parts.Length == 4)
                    {
                        try
                        {
                            if (expectStartTime)
                            {
                                p.StartTime = DecodeTimeCode(parts);
                                expectStartTime = false;
                            }
                            else
                            {
                                if (p.EndTime.TotalMilliseconds < 0.01)
                                {
                                    this._errorCount++;
                                }

                                p.EndTime = DecodeTimeCode(parts);
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
                    if (p.StartTime.TotalMilliseconds == 0 && p.EndTime.TotalMilliseconds == 0)
                    {
                        this._errorCount++;
                    }
                    else
                    {
                        subtitle.Paragraphs.Add(p);
                    }

                    p = new Paragraph();
                }
                else
                {
                    expectStartTime = true;
                    p.Text = (p.Text + Environment.NewLine + line).Trim();
                    if (p.Text.Length > 500)
                    {
                        this._errorCount += 10;
                        return;
                    }
                }
            }

            if (p.EndTime.TotalMilliseconds > 0)
            {
                subtitle.Paragraphs.Add(p);
            }

            bool allNullEndTime = true;
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                if (subtitle.Paragraphs[i].EndTime.TotalMilliseconds != 0)
                {
                    allNullEndTime = false;
                }
            }

            if (allNullEndTime)
            {
                subtitle.Paragraphs.Clear();
            }

            subtitle.RemoveEmptyLines();
            subtitle.Renumber();
        }

        private static string EncodeTimeCode(TimeCode time)
        {
            return time.ToHHMMSSFF();
        }

        private static TimeCode DecodeTimeCode(string[] parts)
        {
            // 00:00:07:12
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            return new TimeCode(hour, minutes, seconds, FramesToMillisecondsMax999(frames));
        }
    }
}