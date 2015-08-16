namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    public class UniversalSubtitleFormat : SubtitleFormat
    {
        public override string Extension
        {
            get
            {
                return ".usf";
            }
        }

        public override string Name
        {
            get
            {
                return "Universal Subtitle Format";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<USFSubtitles version=\"1.0\">" + Environment.NewLine + @"<metadata>
    <title>Universal Subtitle Format</title>
    <author>
      <name>SubtitleEdit</name>
      <email>nikse.dk@gmail.com</email>
      <url>http://www.nikse.dk/</url>
    </author>" + Environment.NewLine + "   <language code=\"eng\">English</language>" + Environment.NewLine + @"  <date>[DATE]</date>
    <comment>This is a USF file</comment>
  </metadata>
  <styles>
    <!-- Here we redefine the default style -->" + Environment.NewLine + "    <style name=\"Default\">" + Environment.NewLine + "      <fontstyle face=\"Arial\" size=\"24\" color=\"#FFFFFF\" back-color=\"#AAAAAA\" />" + Environment.NewLine + "      <position alignment=\"BottomCenter\" vertical-margin=\"20%\" relative-to=\"Window\" />" + @"    </style>
  </styles>

  <subtitles>
  </subtitles>
</USFSubtitles>";
            xmlStructure = xmlStructure.Replace("[DATE]", DateTime.Now.ToString("yyyy-MM-dd"));

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            xml.DocumentElement.SelectSingleNode("metadata/title").InnerText = title;
            XmlNode subtitlesNode = xml.DocumentElement.SelectSingleNode("subtitles");

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("subtitle");

                XmlAttribute start = xml.CreateAttribute("start");
                start.InnerText = p.StartTime.ToString().Replace(",", ".");
                paragraph.Attributes.Prepend(start);

                XmlAttribute stop = xml.CreateAttribute("stop");
                stop.InnerText = p.EndTime.ToString().Replace(",", ".");
                paragraph.Attributes.Append(stop);

                XmlNode text = xml.CreateElement("text");
                text.InnerText = HtmlUtil.RemoveHtmlTags(p.Text);
                paragraph.AppendChild(text);

                XmlAttribute style = xml.CreateAttribute("style");
                style.InnerText = "Default";
                text.Attributes.Append(style);

                subtitlesNode.AppendChild(paragraph);
            }

            return ToUtf8XmlString(xml);
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<USFSubtitles") || !xmlString.Contains("<subtitles>"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("subtitles/subtitle"))
            {
                try
                {
                    string start = node.Attributes["start"].InnerText;
                    string stop = node.Attributes["stop"].InnerText;
                    string text = node.SelectSingleNode("text").InnerText;

                    subtitle.Paragraphs.Add(new Paragraph(DecodeTimeCode(start), DecodeTimeCode(stop), text));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        private static TimeCode DecodeTimeCode(string code)
        {
            string[] parts = code.Split(new[] { ':', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // 00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            return new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), FramesToMillisecondsMax999(int.Parse(frames)));
        }
    }
}