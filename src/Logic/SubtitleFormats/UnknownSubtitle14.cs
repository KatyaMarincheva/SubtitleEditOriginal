// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle14.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 14.
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
    /// The unknown subtitle 14.
    /// </summary>
    public class UnknownSubtitle14 : SubtitleFormat
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
                return "Unknown 14";
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
            // <Phrase TimeStart="4020" TimeEnd="6020">
            // <Text>XYZ PRESENTS</Text>
            // </Phrase>
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<Subtitle xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></Subtitle>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            int id = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("Phrase");

                XmlAttribute start = xml.CreateAttribute("TimeStart");
                start.InnerText = p.StartTime.TotalMilliseconds.ToString();
                paragraph.Attributes.Append(start);

                XmlAttribute duration = xml.CreateAttribute("TimeEnd");
                duration.InnerText = p.EndTime.TotalMilliseconds.ToString();
                paragraph.Attributes.Append(duration);

                XmlNode text = xml.CreateElement("Text");
                text.InnerText = HtmlUtil.RemoveHtmlTags(p.Text).Replace(Environment.NewLine, "\\n");
                paragraph.AppendChild(text);

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
            if (!allText.Contains("<Subtitle") || !allText.Contains("TimeStart="))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Phrase"))
            {
                try
                {
                    string start = node.Attributes["TimeStart"].InnerText;
                    string end = node.Attributes["TimeEnd"].InnerText;
                    string text = node.SelectSingleNode("Text").InnerText;
                    text = text.Replace("\\n", Environment.NewLine);
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
            return new TimeCode(long.Parse(timeCode));
        }
    }
}