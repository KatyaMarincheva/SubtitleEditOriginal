namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Xml;

    public class CaraokeXml : SubtitleFormat
    {
        public override string Extension
        {
            get
            {
                return ".crk";
            }
        }

        public override string Name
        {
            get
            {
                return "Caraoke Xml";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<caraoke name=\"\" filename=\"\"><paragraph attr=\"\" /></caraoke>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            XmlNode paragraph = xml.DocumentElement.SelectSingleNode("paragraph");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode item = xml.CreateElement("item");

                XmlAttribute start = xml.CreateAttribute("tc1");
                start.InnerText = p.StartTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                item.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("tc2");
                end.InnerText = p.EndTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                item.Attributes.Append(end);

                XmlAttribute attr = xml.CreateAttribute("attr");
                attr.InnerText = string.Empty;
                item.Attributes.Append(attr);

                item.InnerText = p.Text;

                paragraph.AppendChild(item);
            }

            return ToUtf8XmlString(xml);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            string xmlAsText = sb.ToString();

            if (!xmlAsText.Contains("<caraoke"))
            {
                return;
            }

            xmlAsText = xmlAsText.Replace("< /", "</");

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                xml.LoadXml(xmlAsText);
            }
            catch (Exception ex)
            {
                this._errorCount = 1;
                Debug.WriteLine(ex.Message);
                return;
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("//item"))
            {
                try
                {
                    string start = node.Attributes["tc1"].InnerText;
                    string end = node.Attributes["tc2"].InnerText;
                    string text = node.InnerText;

                    subtitle.Paragraphs.Add(new Paragraph(text, Convert.ToDouble(start, CultureInfo.InvariantCulture), Convert.ToDouble(end, CultureInfo.InvariantCulture)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }
    }
}