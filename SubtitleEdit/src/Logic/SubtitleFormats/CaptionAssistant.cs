﻿namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    public class CaptionAssistant : SubtitleFormat
    {
        public override string Extension
        {
            get
            {
                return ".cac";
            }
        }

        public override string Name
        {
            get
            {
                return "Caption Assistant";
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
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > 0;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<CaptionAssistant><CaptionData></CaptionData></CaptionAssistant>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            xml.XmlResolver = null;
            XmlNode cd = xml.DocumentElement.SelectSingleNode("CaptionData");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("CaptionDetail");

                XmlAttribute start = xml.CreateAttribute("PositionIn");
                start.InnerText = ToTimeCode(p.StartTime);
                paragraph.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("PositionOut");
                end.InnerText = ToTimeCode(p.EndTime);
                paragraph.Attributes.Append(end);

                XmlAttribute text = xml.CreateAttribute("CaptionText");
                text.InnerText = HtmlUtil.RemoveHtmlTags(p.Text, true);
                paragraph.Attributes.Append(text);

                XmlAttribute align = xml.CreateAttribute("Align");
                if (p.Text.StartsWith("{\\an1}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an4}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an7}", StringComparison.Ordinal))
                {
                    align.InnerText = "Left";
                }
                else if (p.Text.StartsWith("{\\an3}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an6}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an9}", StringComparison.Ordinal))
                {
                    align.InnerText = "Right";
                }
                else
                {
                    align.InnerText = "Center";
                }

                paragraph.Attributes.Append(align);

                XmlAttribute captionType = xml.CreateAttribute("CaptionType");
                captionType.InnerText = "608CC1";
                paragraph.Attributes.Append(captionType);

                cd.AppendChild(paragraph);
            }

            return ToUtf8XmlString(xml);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            string allText = sb.ToString();
            if (!allText.Contains("<CaptionAssistant>") || !allText.Contains("<CaptionData>"))
            {
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                xml.LoadXml(allText);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                this._errorCount = 1;
                return;
            }

            if (xml.DocumentElement == null)
            {
                this._errorCount = 1;
                return;
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("CaptionData/CaptionDetail"))
            {
                try
                {
                    if (node.Attributes != null)
                    {
                        string text = node.Attributes.GetNamedItem("CaptionText").InnerText.Trim();

                        if (node.Attributes.GetNamedItem("Align") != null)
                        {
                            string align = node.Attributes.GetNamedItem("Align").InnerText.Trim();
                            if (align.Equals("left", StringComparison.OrdinalIgnoreCase))
                            {
                                text = "{\\an1}" + text;
                            }
                            else if (align.Equals("right", StringComparison.OrdinalIgnoreCase))
                            {
                                text = "{\\an3}" + text;
                            }
                        }

                        string start = node.Attributes.GetNamedItem("PositionIn").InnerText;
                        string end = node.Attributes.GetNamedItem("PositionOut").InnerText;
                        subtitle.Paragraphs.Add(new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), text));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        private static string ToTimeCode(TimeCode time)
        {
            return time.ToHHMMSSFF();
        }

        private static TimeCode DecodeTimeCode(string s)
        {
            string[] parts = s.Split(new[] { ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            int milliseconds = (int)Math.Round((TimeCode.BaseUnit / Configuration.Settings.General.CurrentFrameRate) * frames);
            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            return new TimeCode(hour, minutes, seconds, milliseconds);
        }
    }
}