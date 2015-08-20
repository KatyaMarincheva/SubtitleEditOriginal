// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle15.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 15.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The unknown subtitle 15.
    /// </summary>
    public class UnknownSubtitle15 : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".xml";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 15";
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
            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > 0;
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
            // <page>
            // <video>
            // <cuepoint>
            // <name>That's 123. That's the number one hundred twenty three.</name>
            // <startTime>00:00:04:67</startTime>
            // <endTime>00:00:07:50</endTime>
            // </cuepoint>
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<page><video/></page>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            int id = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("cuepoint");

                XmlNode text = xml.CreateElement("name");
                text.InnerText = HtmlUtil.RemoveHtmlTags(p.Text).Replace(Environment.NewLine, " ").Replace("  ", " ");
                paragraph.AppendChild(text);

                XmlNode start = xml.CreateElement("startTime");
                start.InnerText = ToTimeCode(p.StartTime);
                paragraph.AppendChild(start);

                XmlNode duration = xml.CreateElement("endTime");
                duration.InnerText = ToTimeCode(p.EndTime);
                paragraph.AppendChild(duration);

                xml.DocumentElement.SelectSingleNode("video").AppendChild(paragraph);
                id++;
            }

            return ToUtf8XmlString(xml);
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
            lines.ForEach(line => sb.AppendLine(line));

            string allText = sb.ToString();
            if (!allText.Contains("<page") || !allText.Contains("<cuepoint"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("video/cuepoint"))
            {
                try
                {
                    string text = node.SelectSingleNode("name").InnerText;
                    string start = node.SelectSingleNode("startTime").InnerText;
                    string end = node.SelectSingleNode("endTime").InnerText;
                    subtitle.Paragraphs.Add(new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), text));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The to time code.
        /// </summary>
        /// <param name="tc">
        /// The tc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ToTimeCode(TimeCode tc)
        {
            int last = (int)(tc.Milliseconds / 10.0D + 0.5D);
            return tc.ToString().Substring(0, 8) + ":" + string.Format("{0:0#}", last);
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
        private static TimeCode DecodeTimeCode(string s)
        {
            string[] parts = s.Split(new[] { ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
            TimeCode tc = new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]) * 100);
            return tc;
        }
    }
}