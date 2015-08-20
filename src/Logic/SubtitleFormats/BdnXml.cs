// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BdnXml.cs" company="">
//   
// </copyright>
// <summary>
//   The bdn xml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The bdn xml.
    /// </summary>
    public class BdnXml : SubtitleFormat
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
                return "BDN Xml";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<Subtitle/>";

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.LoadXml(xmlStructure);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("Paragraph");

                XmlNode number = xml.CreateElement("Number");
                number.InnerText = p.Number.ToString(CultureInfo.InvariantCulture);
                paragraph.AppendChild(number);

                XmlNode start = xml.CreateElement("StartMilliseconds");
                start.InnerText = p.StartTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                paragraph.AppendChild(start);

                XmlNode end = xml.CreateElement("EndMilliseconds");
                end.InnerText = p.EndTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                paragraph.AppendChild(end);

                XmlNode text = xml.CreateElement("Text");
                text.InnerText = HtmlUtil.RemoveHtmlTags(p.Text, true);
                paragraph.AppendChild(text);

                xml.DocumentElement.AppendChild(paragraph);
            }

            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            xml.Save(writer);
            return Encoding.UTF8.GetString(ms.ToArray()).Trim();
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

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<BDN"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Events/Event"))
            {
                try
                {
                    string start = node.Attributes["InTC"].InnerText;
                    string end = node.Attributes["OutTC"].InnerText;
                    StringBuilder textBuilder = new StringBuilder();
                    foreach (XmlNode graphic in node.SelectNodes("Graphic"))
                    {
                        textBuilder.AppendLine(graphic.InnerText);
                    }

                    Paragraph p = new Paragraph(textBuilder.ToString().Trim(), GetMillisecondsFromTimeCode(start), GetMillisecondsFromTimeCode(end));
                    if (node.Attributes["Forced"] != null && node.Attributes["Forced"].Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        p.Forced = true;
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
        }

        /// <summary>
        /// The get milliseconds from time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double GetMillisecondsFromTimeCode(string time)
        {
            string[] parts = time.Split(new[] { ':', ';' }, StringSplitOptions.RemoveEmptyEntries);

            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            return new TimeCode(hour, minutes, seconds, FramesToMillisecondsMax999(frames)).TotalMilliseconds;
        }
    }
}