// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle19.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 19.
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
    /// The unknown subtitle 19.
    /// </summary>
    public class UnknownSubtitle19 : SubtitleFormat
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
                return "Unknown 19";
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
            // <Subtitle version="1.0" timeline="ee_disc1" name="Subtitle 1:" language="English" type="image">
            // <Clip start="121.888" end="125.092" fileName="ee_disc1_subtitle_1/Subtitle_1.png" text="Hello.My name is Laura Knight-Jadcyzk" x="155" y="364" width="328" height="77"/>
            // <Clip start="125.125" end="129.262" fileName="ee_disc1_subtitle_1/Subtitle_2.png" text="and welcome to the Éiriú Eolasbreathing and meditation program" x="145" y="364" width="348" height="77"/>
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<Subtitle version=\"1.0\" timeline=\"ee_disc1\" name=\"Subtitle 1:\" language=\"English\" type=\"text\"></Subtitle>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            // int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("Clip");

                XmlAttribute attr = xml.CreateAttribute("start");
                attr.InnerText = ToTimeCode(p.StartTime);
                paragraph.Attributes.Append(attr);

                attr = xml.CreateAttribute("end");
                attr.InnerText = ToTimeCode(p.EndTime);
                paragraph.Attributes.Append(attr);

                // attr = xml.CreateAttribute("fileName");
                // attr.InnerText = "ee_disc1_subtitle_1/Subtitle_" + count + ".png";
                // paragraph.Attributes.Append(attr);
                attr = xml.CreateAttribute("text");
                attr.InnerText = HtmlUtil.RemoveHtmlTags(p.Text);
                paragraph.Attributes.Append(attr);

                xml.DocumentElement.AppendChild(paragraph);

                // count++;
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
            if (!allText.Contains("</Subtitle>") || !allText.Contains("<Clip "))
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

            if (xml.DocumentElement == null)
            {
                this._errorCount = 1;
                return;
            }

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Clip"))
            {
                try
                {
                    if (node.Attributes != null)
                    {
                        string text = node.Attributes.GetNamedItem("text").InnerText.Trim();
                        string start = node.Attributes.GetNamedItem("start").InnerText;
                        string end = node.Attributes.GetNamedItem("end").InnerText;
                        subtitle.Paragraphs.Add(new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), text));
                    }
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
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ToTimeCode(TimeCode time)
        {
            return string.Format("{0:0.0}", time.TotalSeconds);
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
            return TimeCode.FromSeconds(double.Parse(s));
        }
    }
}