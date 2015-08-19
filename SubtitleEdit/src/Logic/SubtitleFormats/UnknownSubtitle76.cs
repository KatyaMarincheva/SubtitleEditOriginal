namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Xml;

    public class UnknownSubtitle76 : SubtitleFormat
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
                return "Unknown 76";
            }
        }

        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        public static TimeCode GetTimeCode(string s)
        {
            return new TimeCode(Convert.ToDouble(s)); // ms
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            string xmlAsString = sb.ToString().Trim();
            if (xmlAsString.Contains("<timedtext"))
            {
                XmlDocument xml = new XmlDocument { XmlResolver = null };
                try
                {
                    xml.LoadXml(xmlAsString);
                    if (xml.DocumentElement != null && xml.DocumentElement.Name == "timedtext")
                    {
                        return xml.DocumentElement.SelectNodes("text").Count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return false;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><timedtext></timedtext>");

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("text");
                paragraph.InnerText = p.Text;

                XmlAttribute dur = xml.CreateAttribute("d");
                dur.InnerText = ConvertToTimeString(p.Duration);
                paragraph.Attributes.Append(dur);

                XmlAttribute start = xml.CreateAttribute("t");
                start.InnerText = ConvertToTimeString(p.StartTime);
                paragraph.Attributes.Append(start);

                xml.DocumentElement.AppendChild(paragraph);
            }

            return ToUtf8XmlString(xml).Replace(" xmlns=\"\"", string.Empty);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            XmlDocument xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml(sb.ToString().Trim());

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("text"))
            {
                try
                {
                    string start = node.Attributes["t"].InnerText;
                    string dur = node.Attributes["d"].InnerText;
                    TimeCode startTimeCode = GetTimeCode(start);
                    TimeCode endTimeCode = new TimeCode(startTimeCode.TotalMilliseconds + GetTimeCode(dur).TotalMilliseconds);
                    Paragraph p = new Paragraph(startTimeCode, endTimeCode, node.InnerText.Replace("   ", " ").Replace("  ", " "));
                    subtitle.Paragraphs.Add(p);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        internal static string ConvertToTimeString(TimeCode time)
        {
            return Convert.ToInt64(Math.Round(time.TotalMilliseconds)).ToString(CultureInfo.InvariantCulture);
        }
    }
}