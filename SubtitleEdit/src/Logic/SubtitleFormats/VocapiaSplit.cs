namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    public class VocapiaSplit : SubtitleFormat
    {
        public override string Extension
        {
            get
            {
                return ".xml";
            }
        }

        public override string Name
        {
            get
            {
                return "Vocapia Split";
            }
        }

        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        public override bool HasStyleSupport
        {
            get
            {
                return true;
            }
        }

        public static List<string> GetStylesFromHeader(Subtitle subtitle)
        {
            List<string> list = new List<string>();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (!string.IsNullOrEmpty(p.Actor))
                {
                    if (list.IndexOf(p.Actor) < 0)
                    {
                        list.Add(p.Actor);
                    }
                }
            }

            return list;
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > 0;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<AudioDoc name=\"title\">" + Environment.NewLine + "<SpeakerList/>" + Environment.NewLine + "<SegmentList/>" + Environment.NewLine + "</AudioDoc>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            xml.DocumentElement.Attributes["name"].InnerText = title;

            if (subtitle.Header != null && subtitle.Header.Contains("<SpeakerList"))
            {
                XmlDocument header = new XmlDocument();
                try
                {
                    header.LoadXml(subtitle.Header);
                    XmlNode speakerListNode = header.DocumentElement.SelectSingleNode("SpeakerList");
                    if (speakerListNode != null)
                    {
                        xml.DocumentElement.SelectSingleNode("SpeakerList").InnerXml = speakerListNode.InnerXml;
                    }
                }
                catch
                {
                }
            }

            XmlNode reel = xml.DocumentElement.SelectSingleNode("SegmentList");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("SpeechSegment");

                XmlAttribute start = xml.CreateAttribute("stime");
                start.InnerText = ToTimeCode(p.StartTime.TotalMilliseconds);
                paragraph.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("etime");
                end.InnerText = ToTimeCode(p.EndTime.TotalMilliseconds);
                paragraph.Attributes.Append(end);

                if (p.Actor != null)
                {
                    XmlAttribute spkid = xml.CreateAttribute("spkid");
                    spkid.InnerText = p.Actor;
                    paragraph.Attributes.Append(spkid);
                }

                paragraph.InnerText = HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, "<s/>"));

                reel.AppendChild(paragraph);
            }

            return ToUtf8XmlString(xml);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<SpeechSegment"))
            {
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                xml.LoadXml(xmlString);
            }
            catch
            {
                this._errorCount = 1;
                return;
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("SegmentList/SpeechSegment"))
            {
                try
                {
                    string start = node.Attributes["stime"].InnerText;
                    string end = node.Attributes["etime"].InnerText;
                    string text = node.InnerText;
                    text = text.Replace("<s/>", Environment.NewLine);
                    text = text.Replace("  ", " ");
                    Paragraph p = new Paragraph(text, ParseTimeCode(start), ParseTimeCode(end));
                    XmlAttribute spkIdAttr = node.Attributes["spkid"];
                    if (spkIdAttr != null)
                    {
                        p.Extra = spkIdAttr.InnerText;
                        p.Actor = p.Extra;
                    }

                    subtitle.Paragraphs.Add(p);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
            if (subtitle.Paragraphs.Count > 0)
            {
                subtitle.Header = xmlString;
            }
        }

        private static string ToTimeCode(double totalMilliseconds)
        {
            return string.Format("{0:0##}", totalMilliseconds / TimeCode.BaseUnit);
        }

        private static double ParseTimeCode(string s)
        {
            return Convert.ToDouble(s) * TimeCode.BaseUnit;
        }
    }
}