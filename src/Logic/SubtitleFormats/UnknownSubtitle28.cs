// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle28.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 28.
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
    /// The unknown subtitle 28.
    /// </summary>
    public class UnknownSubtitle28 : SubtitleFormat
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
                return "Unknown 28";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<titles/>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            int id = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                id++;
                XmlNode paragraph = xml.CreateElement("title");
                XmlAttribute idAttr = xml.CreateAttribute("id");
                idAttr.InnerText = id.ToString();
                paragraph.Attributes.Append(idAttr);

                XmlNode time = xml.CreateElement("time");
                XmlAttribute start = xml.CreateAttribute("start");
                start.InnerText = ToTimeCode(p.StartTime.TotalMilliseconds);
                time.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("end");
                end.InnerText = ToTimeCode(p.EndTime.TotalMilliseconds);
                time.Attributes.Append(end);

                paragraph.AppendChild(time);

                string[] arr = p.Text.SplitToLines();
                for (int i = 0; i < arr.Length; i++)
                {
                    XmlNode text = xml.CreateElement("text" + (i + 1));
                    text.InnerText = arr[i];
                    paragraph.AppendChild(text);
                }

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
            if (!xmlString.Contains("<titles") || !xmlString.Contains("<text1>"))
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

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("title"))
            {
                try
                {
                    XmlNode timeNode = node.SelectSingleNode("time");
                    string start = timeNode.Attributes["start"].InnerText;
                    string end = timeNode.Attributes["end"].InnerText;
                    string text = string.Empty;
                    for (int i = 1; i < 10; i++)
                    {
                        XmlNode textNode = node.SelectSingleNode("text" + i);
                        if (textNode != null)
                        {
                            text = (text + Environment.NewLine + textNode.InnerText).Trim();
                        }
                    }

                    subtitle.Paragraphs.Add(new Paragraph(text, ParseTimeCode(start), ParseTimeCode(end)));
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
        /// <param name="totalMilliseconds">
        /// The total milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ToTimeCode(double totalMilliseconds)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(totalMilliseconds);
            return string.Format("{0:00}:{1:00}:{2:00},{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        /// <summary>
        /// The parse time code.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double ParseTimeCode(string start)
        {
            string[] arr = start.Split(new[] { ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            TimeSpan ts = new TimeSpan(0, int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), int.Parse(arr[3]));
            return ts.TotalMilliseconds;
        }
    }
}