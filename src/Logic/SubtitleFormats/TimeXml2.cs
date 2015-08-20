// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeXml2.cs" company="">
//   
// </copyright>
// <summary>
//   The time xml 2.
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
    /// The time xml 2.
    /// </summary>
    public class TimeXml2 : SubtitleFormat
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
                return "Xml 2";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<Subtitles/>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("Subtitle");

                XmlNode number = xml.CreateElement("Number");
                number.InnerText = p.Number.ToString();
                paragraph.AppendChild(number);

                XmlNode start = xml.CreateElement("Start");
                start.InnerText = p.StartTime.ToString();
                paragraph.AppendChild(start);

                XmlNode end = xml.CreateElement("End");
                end.InnerText = p.EndTime.ToString();
                paragraph.AppendChild(end);

                XmlNode duration = xml.CreateElement("Duration");
                duration.InnerText = p.Duration.ToShortString();
                paragraph.AppendChild(duration);

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
            if (!xmlString.Contains("<Subtitles>") || !xmlString.Contains("<Text>") || !xmlString.Contains("<Duration>"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Subtitle"))
            {
                try
                {
                    TimeCode startTimeCode = DecodeTimeCode(node.SelectSingleNode("Start").InnerText);
                    TimeCode endTimeCode = DecodeTimeCode(node.SelectSingleNode("End").InnerText);
                    string text = node.SelectSingleNode("Text").InnerText;
                    subtitle.Paragraphs.Add(new Paragraph(startTimeCode, endTimeCode, text));
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
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string p)
        {
            string[] parts = p.Split(';', '.', ':', ',');

            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string ms = parts[3];

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), int.Parse(ms));
            return tc;
        }
    }
}