// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeXml.cs" company="">
//   
// </copyright>
// <summary>
//   The time xml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The time xml.
    /// </summary>
    public class TimeXml : SubtitleFormat
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
                return "Xml";
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
            xml.LoadXml(xmlStructure);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("Paragraph");

                XmlNode number = xml.CreateElement("Number");
                number.InnerText = p.Number.ToString();
                paragraph.AppendChild(number);

                XmlNode start = xml.CreateElement("StartMilliseconds");
                start.InnerText = p.StartTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                paragraph.AppendChild(start);

                XmlNode end = xml.CreateElement("EndMilliseconds");
                end.InnerText = p.EndTime.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                paragraph.AppendChild(end);

                XmlNode text = xml.CreateElement("Text");
                text.InnerText = HtmlUtil.RemoveHtmlTags(p.Text);
                paragraph.AppendChild(text);

                xml.DocumentElement.AppendChild(paragraph);
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

            string xmlString = sb.ToString();
            if (!xmlString.Contains("<Paragraph>") || !xmlString.Contains("<Text>"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Paragraph"))
            {
                try
                {
                    string start = node.SelectSingleNode("StartMilliseconds").InnerText;
                    string end = node.SelectSingleNode("EndMilliseconds").InnerText;
                    string text = node.SelectSingleNode("Text").InnerText;

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