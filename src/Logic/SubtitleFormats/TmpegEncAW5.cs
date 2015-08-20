// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TmpegEncAW5.cs" company="">
//   
// </copyright>
// <summary>
//   The tmpeg enc a w 5.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The tmpeg enc a w 5.
    /// </summary>
    public class TmpegEncAW5 : TmpegEncXml
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TMPGEnc AW5";
            }
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
            string xmlStructure = Layout.Replace("'", "\"");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            XmlNode div = xml.DocumentElement.SelectSingleNode("Subtitle");
            div.InnerXml = string.Empty;
            int no = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("SubtitleItem");

                string text = HtmlUtil.RemoveHtmlTags(p.Text);
                paragraph.InnerText = text;
                paragraph.InnerXml = "<Text><![CDATA[" + paragraph.InnerXml.Replace(Environment.NewLine, "\\n") + "\\n]]></Text>";

                XmlAttribute layoutIndex = xml.CreateAttribute("layoutindex");
                if (p.Text.TrimStart().StartsWith("<i>") && p.Text.TrimEnd().EndsWith("</i>"))
                {
                    layoutIndex.InnerText = "4";
                }
                else
                {
                    layoutIndex.InnerText = "0";
                }

                paragraph.Attributes.Append(layoutIndex);

                XmlAttribute enable = xml.CreateAttribute("enable");
                enable.InnerText = "1";
                paragraph.Attributes.Append(enable);

                XmlAttribute start = xml.CreateAttribute("starttime");
                start.InnerText = p.StartTime.ToString();
                paragraph.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("endtime");
                end.InnerText = p.EndTime.ToString();
                paragraph.Attributes.Append(end);

                div.AppendChild(paragraph);
                no++;
            }

            string s = ToUtf8XmlString(xml);
            int startPos = s.IndexOf("<Subtitle>", StringComparison.Ordinal) + 10;
            s = s.Substring(startPos, s.IndexOf("</Subtitle>", StringComparison.Ordinal) - startPos).Trim();
            return Layout.Replace("@", s);
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
            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            string xmlAsString = sb.ToString().Trim();
            if ((xmlAsString.Contains("<TMPGEncVMESubtitleTextFormat>") || xmlAsString.Contains("<SubtitleItem ")) && xmlAsString.Contains("<Subtitle"))
            {
                Subtitle subtitle = new Subtitle();
                this.LoadSubtitle(subtitle, lines, fileName);
                return subtitle.Paragraphs.Count > this._errorCount;
            }

            return false;
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
            this.LoadTMpeg(subtitle, lines, true);
        }
    }
}