// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle43.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 43.
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
    /// The unknown subtitle 43.
    /// </summary>
    public class UnknownSubtitle43 : SubtitleFormat
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
                return "Unknown 43";
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
        /// The get time code.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        public static TimeCode GetTimeCode(string s)
        {
            string[] arr = s.Split(':');
            if (arr.Length == 2)
            {
                return new TimeCode(0, int.Parse(arr[0]), int.Parse(arr[1]), 0);
            }

            if (arr.Length == 3)
            {
                return new TimeCode(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), 0);
            }

            return new TimeCode(0, 0, int.Parse(s), 0);
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
            if (xmlAsString.Contains("<subtitle"))
            {
                XmlDocument xml = new XmlDocument();
                xml.XmlResolver = null;
                try
                {
                    xml.LoadXml(xmlAsString);
                    if (xml.DocumentElement.Name == "parfums")
                    {
                        return xml.DocumentElement.SelectNodes("subtitle").Count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return false;
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
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><parfums></parfums>");

            int no = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("subtitle");
                string text = p.Text;

                bool first = true;
                foreach (string line in text.SplitToLines())
                {
                    if (!first)
                    {
                        XmlNode br = xml.CreateElement("br");
                        paragraph.AppendChild(br);
                    }

                    XmlText textNode = xml.CreateTextNode(string.Empty);
                    textNode.InnerText = line;
                    paragraph.AppendChild(textNode);
                    first = false;
                }

                XmlAttribute end = xml.CreateAttribute("end");
                end.InnerText = ConvertToTimeString(p.EndTime);
                paragraph.Attributes.Append(end);

                XmlAttribute start = xml.CreateAttribute("start");
                start.InnerText = ConvertToTimeString(p.StartTime);
                paragraph.Attributes.Append(start);

                xml.DocumentElement.AppendChild(paragraph);
                no++;
            }

            return ToUtf8XmlString(xml).Replace(" xmlns=\"\"", string.Empty);
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
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.LoadXml(sb.ToString().Trim());

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("subtitle"))
            {
                try
                {
                    StringBuilder pText = new StringBuilder();
                    foreach (XmlNode innerNode in node.ChildNodes)
                    {
                        switch (innerNode.Name)
                        {
                            case "br":
                                pText.AppendLine();
                                break;
                            case "span":
                                ReadSpan(pText, innerNode);
                                break;

                            default:
                                pText.Append(innerNode.InnerText);
                                break;
                        }
                    }

                    string start = node.Attributes["start"].InnerText;
                    string end = node.Attributes["end"].InnerText;
                    Paragraph p = new Paragraph(GetTimeCode(start), GetTimeCode(end), pText.ToString().Replace("   ", " ").Replace("  ", " "));
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

        /// <summary>
        /// The convert to time string.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string ConvertToTimeString(TimeCode time)
        {
            return string.Format("{0}:{1:00}", (int)(time.TotalSeconds / 60), (int)(time.TotalSeconds % 60));
        }

        /// <summary>
        /// The read span.
        /// </summary>
        /// <param name="pText">
        /// The p text.
        /// </param>
        /// <param name="innerNode">
        /// The inner node.
        /// </param>
        private static void ReadSpan(StringBuilder pText, XmlNode innerNode)
        {
            if (innerNode.HasChildNodes)
            {
                foreach (XmlNode innerInnerNode in innerNode.ChildNodes)
                {
                    if (innerInnerNode.Name == "br")
                    {
                        pText.AppendLine();
                    }
                    else if (innerInnerNode.Name == "span")
                    {
                        ReadSpan(pText, innerInnerNode);
                    }
                    else
                    {
                        pText.Append(innerInnerNode.InnerText);
                    }
                }
            }
            else
            {
                pText.Append(innerNode.InnerText);
            }
        }
    }
}