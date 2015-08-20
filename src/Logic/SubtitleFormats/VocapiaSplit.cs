// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VocapiaSplit.cs" company="">
//   
// </copyright>
// <summary>
//   The vocapia split.
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
    /// The vocapia split.
    /// </summary>
    public class VocapiaSplit : SubtitleFormat
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
                return "Vocapia Split";
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
        /// Gets a value indicating whether has style support.
        /// </summary>
        public override bool HasStyleSupport
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The get styles from header.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> GetStylesFromHeader(Subtitle subtitle)
        {
            List<string> list = new List<string>();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (!string.IsNullOrEmpty(p.Actor))
                {
                    if (list.IndexOf(p.Actor) < 0)
                    {
                        list.Add(p.Actor);
                    }
                }
            }

            return list;
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<AudioDoc name=\"title\">" + Environment.NewLine + "<SpeakerList/>" + Environment.NewLine + "<SegmentList/>" + Environment.NewLine + "</AudioDoc>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            xml.DocumentElement.Attributes["name"].InnerText = title;

            if (subtitle.Header != null && subtitle.Header.Contains("<SpeakerList"))
            {
                XmlDocument header = new XmlDocument();
                try
                {
                    header.LoadXml(subtitle.Header);
                    XmlNode speakerListNode = header.DocumentElement.SelectSingleNode("SpeakerList");
                    if (speakerListNode != null)
                    {
                        xml.DocumentElement.SelectSingleNode("SpeakerList").InnerXml = speakerListNode.InnerXml;
                    }
                }
                catch
                {
                }
            }

            XmlNode reel = xml.DocumentElement.SelectSingleNode("SegmentList");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("SpeechSegment");

                XmlAttribute start = xml.CreateAttribute("stime");
                start.InnerText = ToTimeCode(p.StartTime.TotalMilliseconds);
                paragraph.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("etime");
                end.InnerText = ToTimeCode(p.EndTime.TotalMilliseconds);
                paragraph.Attributes.Append(end);

                if (p.Actor != null)
                {
                    XmlAttribute spkid = xml.CreateAttribute("spkid");
                    spkid.InnerText = p.Actor;
                    paragraph.Attributes.Append(spkid);
                }

                paragraph.InnerText = HtmlUtil.RemoveHtmlTags(p.Text.Replace(Environment.NewLine, "<s/>"));

                reel.AppendChild(paragraph);
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
            if (!xmlString.Contains("<SpeechSegment"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("SegmentList/SpeechSegment"))
            {
                try
                {
                    string start = node.Attributes["stime"].InnerText;
                    string end = node.Attributes["etime"].InnerText;
                    string text = node.InnerText;
                    text = text.Replace("<s/>", Environment.NewLine);
                    text = text.Replace("  ", " ");
                    Paragraph p = new Paragraph(text, ParseTimeCode(start), ParseTimeCode(end));
                    XmlAttribute spkIdAttr = node.Attributes["spkid"];
                    if (spkIdAttr != null)
                    {
                        p.Extra = spkIdAttr.InnerText;
                        p.Actor = p.Extra;
                    }

                    subtitle.Paragraphs.Add(p);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
            if (subtitle.Paragraphs.Count > 0)
            {
                subtitle.Header = xmlString;
            }
        }

        /// <summary>
        /// The to time code.
        /// </summary>
        /// <param name="totalMilliseconds">
        /// The total milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ToTimeCode(double totalMilliseconds)
        {
            return string.Format("{0:0##}", totalMilliseconds / TimeCode.BaseUnit);
        }

        /// <summary>
        /// The parse time code.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double ParseTimeCode(string s)
        {
            return Convert.ToDouble(s) * TimeCode.BaseUnit;
        }
    }
}