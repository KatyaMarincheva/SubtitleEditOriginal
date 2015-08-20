// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdobeAfterEffectsFTME.cs" company="">
//   
// </copyright>
// <summary>
//   The adobe after effects ftme.
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
    /// The adobe after effects ftme.
    /// </summary>
    public class AdobeAfterEffectsFTME : SubtitleFormat
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
                return "Adobe After Effects ft-MarkerExporter";
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
            string xmlStructure = @"
                                    <xml>
                                      <general>
                                        <version>1</version>
                                      </general>
                                      <layers>
                                        <layer name='myLayer' index='1'>
                                        </layer>
                                      </layers>
                                    </xml>".Replace("'", "\"");

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.LoadXml(xmlStructure);
            const string innerXml = "<comment value=\"\"/><time value=\"{0}\"/><duration value=\"{1}\"/>";
            XmlNode root = xml.DocumentElement.SelectSingleNode("layers/layer");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("marker");
                paragraph.InnerXml = string.Format(CultureInfo.InvariantCulture, innerXml, p.StartTime.TotalSeconds, p.Duration.TotalSeconds);
                paragraph.SelectSingleNode("comment").Attributes["value"].InnerText = HtmlUtil.RemoveHtmlTags(p.Text, true).Replace(Environment.NewLine, "||");
                root.AppendChild(paragraph);
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
            if (!allText.Contains("<layers") && !allText.Contains("<marker>"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("layers/layer/marker"))
            {
                try
                {
                    double start = Convert.ToDouble(node.SelectSingleNode("time").Attributes["value"].InnerText, CultureInfo.InvariantCulture);
                    double end = start + Convert.ToDouble(node.SelectSingleNode("duration").Attributes["value"].InnerText, CultureInfo.InvariantCulture);
                    string text = node.SelectSingleNode("comment").Attributes["value"].InnerText.Replace("||", Environment.NewLine);
                    subtitle.Paragraphs.Add(new Paragraph(text, start * TimeCode.BaseUnit, end * TimeCode.BaseUnit));
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