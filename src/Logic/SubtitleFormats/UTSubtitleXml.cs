// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UTSubtitleXml.cs" company="">
//   
// </copyright>
// <summary>
//   The ut subtitle xml.
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
    /// The ut subtitle xml.
    /// </summary>
    public class UTSubtitleXml : SubtitleFormat
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
                return "UT Subtitle xml";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<uts>" + Environment.NewLine + "</uts>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            XmlNode root = xml.DocumentElement;

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                // <ut secOut="26.4" secIn="21.8">
                // <![CDATA[Pozdrav i dobrodošli natrag<br>u drugi dio naše emisije]]>
                // </ut>
                XmlNode ut = xml.CreateElement("ut");

                XmlAttribute et = xml.CreateAttribute("secOut");
                et.InnerText = string.Format("{0:0.0##}", p.EndTime.TotalSeconds).Replace(",", ".");
                ut.Attributes.Append(et);

                XmlAttribute st = xml.CreateAttribute("secIn");
                st.InnerText = string.Format("{0:0.0##}", p.StartTime.TotalSeconds).Replace(",", ".");
                ut.Attributes.Append(st);

                ut.InnerText = p.Text;
                ut.InnerXml = "<![CDATA[" + ut.InnerXml.Replace(Environment.NewLine, "<br>") + "]]>";

                root.AppendChild(ut);
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
            if (!sb.ToString().Contains("<uts") || !sb.ToString().Contains("secOut="))
            {
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                string xmlText = sb.ToString();
                xml.LoadXml(xmlText);

                foreach (XmlNode node in xml.SelectNodes("//ut"))
                {
                    try
                    {
                        string endTime = node.Attributes["secOut"].InnerText;
                        string startTime = node.Attributes["secIn"].InnerText;
                        string text = node.InnerText;
                        text = text.Replace("<br>", Environment.NewLine).Replace("<br />", Environment.NewLine);
                        Paragraph p = new Paragraph();
                        p.StartTime.TotalSeconds = Convert.ToDouble(startTime, CultureInfo.InvariantCulture);
                        p.EndTime.TotalSeconds = Convert.ToDouble(endTime, CultureInfo.InvariantCulture);
                        p.Text = text;
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this._errorCount = 1;
            }
        }
    }
}