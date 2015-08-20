// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpacTtxt.cs" company="">
//   
// </copyright>
// <summary>
//   The gpac ttxt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The gpac ttxt.
    /// </summary>
    public class GpacTtxt : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".ttxt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "GPAC TTXT";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + "<!-- GPAC 3GPP Text Stream -->" + Environment.NewLine + "<TextStream version=\"1.1\">" + Environment.NewLine + "  <TextStreamHeader translation_y=\"0\" translation_x=\"0\" layer=\"0\" height=\"60\" width=\"400\">" + Environment.NewLine + "    <TextSampleDescription scroll=\"None\" continuousKaraoke=\"no\" fillTextRegion=\"no\" verticalText=\"no\" backColor=\"0 0 0 0\" verticalJustification=\"bottom\" horizontalJustification=\"center\">" + Environment.NewLine + "      <FontTable>" + Environment.NewLine + "        <FontTableEntry fontID=\"1\" fontName=\"Serif\"/>" + Environment.NewLine + "      </FontTable>" + Environment.NewLine + "      <TextBox right=\"400\" bottom=\"60\" left=\"0\" top=\"0\"/>" + Environment.NewLine + "      <Style fontID=\"1\" color=\"ff ff ff ff\" fontSize=\"18\" styles=\"Normal\"/>" + Environment.NewLine + "    </TextSampleDescription>" + Environment.NewLine + "  </TextStreamHeader>" + Environment.NewLine + "</TextStream>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode textSample = xml.CreateElement("TextSample");

                XmlAttribute preserveSpace = xml.CreateAttribute("xml:space");
                preserveSpace.Value = "preserve";
                textSample.Attributes.Append(preserveSpace);

                XmlAttribute sampleTime = xml.CreateAttribute("sampleTime");
                sampleTime.Value = p.StartTime.ToString().Replace(",", ".");
                textSample.Attributes.Append(sampleTime);

                textSample.InnerText = p.Text;

                xml.DocumentElement.AppendChild(textSample);

                textSample = xml.CreateElement("TextSample");
                preserveSpace = xml.CreateAttribute("xml:space");
                preserveSpace.Value = "preserve";
                textSample.Attributes.Append(preserveSpace);
                sampleTime = xml.CreateAttribute("sampleTime");
                sampleTime.Value = p.EndTime.ToString().Replace(",", ".");
                textSample.Attributes.Append(sampleTime);
                xml.DocumentElement.AppendChild(textSample);
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
            Paragraph last = null;
            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));

            if (!sb.ToString().Contains("<TextStream"))
            {
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                xml.LoadXml(sb.ToString().Trim());

                foreach (XmlNode node in xml.DocumentElement.SelectNodes("TextSample"))
                {
                    if (last != null && last.EndTime.TotalMilliseconds < 1)
                    {
                        last.EndTime = GetTimeCode(node.Attributes["sampleTime"].Value);
                    }

                    Paragraph p = new Paragraph();
                    p.Text = node.InnerText;
                    p.StartTime = GetTimeCode(node.Attributes["sampleTime"].Value);
                    if (string.IsNullOrEmpty(p.Text))
                    {
                        XmlAttribute text = node.Attributes["text"];
                        if (text != null)
                        {
                            p.Text = text.Value;
                        }
                    }

                    if (!string.IsNullOrEmpty(p.Text))
                    {
                        subtitle.Paragraphs.Add(p);
                        last = p;
                    }
                }

                subtitle.Renumber();
            }
            catch
            {
                this._errorCount = 1;
            }
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="timeString">
        /// The time string.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode GetTimeCode(string timeString)
        {
            string[] timeParts = timeString.Split(':', '.');
            return new TimeCode(int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]), int.Parse(timeParts[3]));
        }
    }
}