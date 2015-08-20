// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle76.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 76.
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
    /// The unknown subtitle 76.
    /// </summary>
    public class UnknownSubtitle76 : SubtitleFormat
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
                return "Unknown 76";
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
            return new TimeCode(Convert.ToDouble(s)); // ms
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
            if (xmlAsString.Contains("<timedtext"))
            {
                XmlDocument xml = new XmlDocument { XmlResolver = null };
                try
                {
                    xml.LoadXml(xmlAsString);
                    if (xml.DocumentElement != null && xml.DocumentElement.Name == "timedtext")
                    {
                        return xml.DocumentElement.SelectNodes("text").Count > 0;
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
            xml.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><timedtext></timedtext>");

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("text");
                paragraph.InnerText = p.Text;

                XmlAttribute dur = xml.CreateAttribute("d");
                dur.InnerText = ConvertToTimeString(p.Duration);
                paragraph.Attributes.Append(dur);

                XmlAttribute start = xml.CreateAttribute("t");
                start.InnerText = ConvertToTimeString(p.StartTime);
                paragraph.Attributes.Append(start);

                xml.DocumentElement.AppendChild(paragraph);
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
            XmlDocument xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml(sb.ToString().Trim());

            foreach (XmlNode node in xml.DocumentElement.SelectNodes("text"))
            {
                try
                {
                    string start = node.Attributes["t"].InnerText;
                    string dur = node.Attributes["d"].InnerText;
                    TimeCode startTimeCode = GetTimeCode(start);
                    TimeCode endTimeCode = new TimeCode(startTimeCode.TotalMilliseconds + GetTimeCode(dur).TotalMilliseconds);
                    Paragraph p = new Paragraph(startTimeCode, endTimeCode, node.InnerText.Replace("   ", " ").Replace("  ", " "));
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
            return Convert.ToInt64(Math.Round(time.TotalMilliseconds)).ToString(CultureInfo.InvariantCulture);
        }
    }
}