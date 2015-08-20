// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle5.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 5.
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
    /// The unknown subtitle 5.
    /// </summary>
    public class UnknownSubtitle5 : SubtitleFormat
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
                return "Unknown 5";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<transcript/>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("text");

                XmlAttribute start = xml.CreateAttribute("start");
                start.InnerText = string.Format("{0}", p.StartTime.TotalMilliseconds / 1000).Replace(",", ".");
                paragraph.Attributes.Append(start);

                XmlAttribute duration = xml.CreateAttribute("dur");
                duration.InnerText = string.Format("{0}", p.Duration.TotalMilliseconds / 1000).Replace(",", ".");
                paragraph.Attributes.Append(duration);

                paragraph.InnerText = p.Text;

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

            string allText = sb.ToString();
            if (!allText.Contains("<text") || !allText.Contains("start="))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("text"))
            {
                try
                {
                    string start = node.Attributes["start"].InnerText;
                    if (!string.IsNullOrEmpty(start))
                    {
                        start = start.Replace(",", ".");
                    }

                    string end = node.Attributes["dur"].InnerText;
                    if (!string.IsNullOrEmpty(end))
                    {
                        end = end.Replace(",", ".");
                    }

                    string text = node.InnerText;

                    subtitle.Paragraphs.Add(new Paragraph(text, Convert.ToDouble(start, CultureInfo.InvariantCulture) * TimeCode.BaseUnit, TimeCode.BaseUnit * (Convert.ToDouble(start, CultureInfo.InvariantCulture) + Convert.ToDouble(end, CultureInfo.InvariantCulture))));
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