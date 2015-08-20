﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlashXml.cs" company="">
//   
// </copyright>
// <summary>
//   The flash xml.
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

    // <?xml version="1.0" encoding="UTF-8"?>
    // <tt>
    // <div>
    // <p begin="00:00:00.700" end="00:00:05.000"><![CDATA[<sub>This is fully skinnable through XML<br/>using external images for each button</sub>]]></p>
    // <p begin="00:00:05.200" end="00:00:10.000"><![CDATA[<sub>You can put in any order or enable/disable<br/>the control buttons</sub>]]></p>
    // <p begin="00:00:10.200" end="00:00:15.000"><![CDATA[<sub>Test below some of the customizable<br/>properties this player has</sub>]]></p>
    // <p begin="00:00:15.200" end="00:00:19.700"><![CDATA[<sub>Many other properties related to fonts, sizes, colors<br/>and list properties are in style.css file</sub>]]></p>
    // </div>
    // </tt>
    /// <summary>
    /// The flash xml.
    /// </summary>
    public class FlashXml : SubtitleFormat
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
                return "Flash Xml";
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
            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            string xmlAsString = sb.ToString().Trim();
            if ((xmlAsString.Contains("<tt>") || xmlAsString.Contains("<tt ")) && xmlAsString.Contains("<sub>"))
            {
                XmlDocument xml = new XmlDocument { XmlResolver = null };
                try
                {
                    xml.LoadXml(xmlAsString);
                    XmlNodeList paragraphs = xml.DocumentElement.SelectNodes("div/p");
                    return paragraphs != null && paragraphs.Count > 0 && xml.DocumentElement.Name == "tt";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<tt>" + Environment.NewLine + "   <div>" + Environment.NewLine + "   </div>" + Environment.NewLine + "</tt>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            XmlNode div = xml.DocumentElement.SelectSingleNode("div");
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("p");
                string text = HtmlUtil.RemoveHtmlTags(p.Text, true);

                paragraph.InnerText = text;
                paragraph.InnerXml = "<![CDATA[<sub>" + paragraph.InnerXml.Replace(Environment.NewLine, "<br />") + "</sub>]]>";

                XmlAttribute start = xml.CreateAttribute("begin");
                start.InnerText = ConvertToTimeString(p.StartTime);
                paragraph.Attributes.Append(start);

                XmlAttribute end = xml.CreateAttribute("end");
                end.InnerText = ConvertToTimeString(p.EndTime);
                paragraph.Attributes.Append(end);

                div.AppendChild(paragraph);
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
            double startSeconds = 0;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            XmlDocument xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml(sb.ToString().Trim());

            StringBuilder pText = new StringBuilder();
            foreach (XmlNode node in xml.DocumentElement.SelectNodes("div/p"))
            {
                try
                {
                    foreach (XmlNode innerNode in node.ChildNodes)
                    {
                        switch (innerNode.Name)
                        {
                            case "br":
                                pText.AppendLine();
                                break;
                            default:
                                pText.Append(innerNode.InnerText.Trim());
                                break;
                        }
                    }

                    string start = string.Empty;
                    if (node.Attributes["begin"] != null)
                    {
                        start = node.Attributes["begin"].InnerText;
                    }

                    string end = string.Empty;
                    if (node.Attributes["end"] != null)
                    {
                        end = node.Attributes["end"].InnerText;
                    }

                    string dur = string.Empty;
                    if (node.Attributes["dur"] != null)
                    {
                        dur = node.Attributes["dur"].InnerText;
                    }

                    TimeCode startCode = TimeCode.FromSeconds(startSeconds);
                    if (start.Length > 0)
                    {
                        startCode = GetTimeCode(start);
                    }

                    TimeCode endCode;
                    if (end.Length > 0)
                    {
                        endCode = GetTimeCode(end);
                    }
                    else if (dur.Length > 0)
                    {
                        endCode = new TimeCode(GetTimeCode(dur).TotalMilliseconds + startCode.TotalMilliseconds);
                    }
                    else
                    {
                        endCode = new TimeCode(startCode.TotalMilliseconds + 3000);
                    }

                    startSeconds = endCode.TotalSeconds;

                    subtitle.Paragraphs.Add(new Paragraph(startCode, endCode, pText.ToString().Replace("<sub>", string.Empty).Replace("</sub>", string.Empty)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this._errorCount++;
                }

                pText.Clear();
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
        private static string ConvertToTimeString(TimeCode time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
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
        private static TimeCode GetTimeCode(string s)
        {
            if (s.EndsWith('s'))
            {
                s = s.TrimEnd('s');
                return TimeCode.FromSeconds(double.Parse(s));
            }

            string[] parts = s.Split(':', '.', ',');
            return new TimeCode(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
        }
    }
}