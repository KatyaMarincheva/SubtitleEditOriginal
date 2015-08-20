// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle13.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 13.
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

    /// <summary>
    /// The unknown subtitle 13.
    /// </summary>
    public class UnknownSubtitle13 : SubtitleFormat
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
                return "Unknown 13";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<subtitle><config bgAlpha=\"0.5\" bgColor=\"0x000000\" defaultColor=\"0xCCffff\" fontSize=\"16\"/></subtitle>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            int id = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("entry");

                XmlAttribute duration = xml.CreateAttribute("timeOut");
                duration.InnerText = p.EndTime.ToString();
                paragraph.Attributes.Append(duration);

                XmlAttribute start = xml.CreateAttribute("timeIn");
                start.InnerText = p.StartTime.ToString();
                paragraph.Attributes.Append(start);

                XmlAttribute idAttr = xml.CreateAttribute("id");
                idAttr.InnerText = id.ToString(CultureInfo.InvariantCulture);
                paragraph.Attributes.Append(idAttr);

                paragraph.InnerText = "<![CDATA[" + p.Text + "]]";

                xml.DocumentElement.AppendChild(paragraph);
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
            if (!allText.Contains("<subtitle>") || !allText.Contains("timeIn="))
            {
                return;
            }

            XmlDocument xml = new XmlDocument { XmlResolver = null };
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("entry"))
            {
                try
                {
                    string start = node.Attributes["timeIn"].InnerText;
                    string end = node.Attributes["timeOut"].InnerText;
                    string text = node.InnerText;
                    if (text.StartsWith("![CDATA[", StringComparison.Ordinal))
                    {
                        text = text.Remove(0, 8);
                    }

                    if (text.StartsWith("<![CDATA[", StringComparison.Ordinal))
                    {
                        text = text.Remove(0, 9);
                    }

                    if (text.EndsWith("]]", StringComparison.Ordinal))
                    {
                        text = text.Remove(text.Length - 2, 2);
                    }

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
        /// The decode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timeCode)
        {
            string[] arr = timeCode.Split(new[] { ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            int hours = int.Parse(arr[0]);
            int minutes = int.Parse(arr[1]);
            int seconds = int.Parse(arr[2]);
            int ms = int.Parse(arr[3]);
            return new TimeCode(hours, minutes, seconds, ms);
        }
    }
}