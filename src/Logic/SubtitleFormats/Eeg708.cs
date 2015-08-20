// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Eeg708.cs" company="">
//   
// </copyright>
// <summary>
//   The eeg 708.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The eeg 708.
    /// </summary>
    public class Eeg708 : SubtitleFormat
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
                return "EEG 708";
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
            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<EEG708Captions/>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode paragraph = xml.CreateElement("Caption");
                XmlAttribute start = xml.CreateAttribute("timecode");
                start.InnerText = EncodeTimeCode(p.StartTime);
                paragraph.Attributes.Append(start);
                XmlNode text = xml.CreateElement("Text");
                text.InnerText = p.Text;
                paragraph.AppendChild(text);
                xml.DocumentElement.AppendChild(paragraph);

                paragraph = xml.CreateElement("Caption");
                start = xml.CreateAttribute("timecode");
                start.InnerText = EncodeTimeCode(p.EndTime);
                paragraph.Attributes.Append(start);
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

            string allText = sb.ToString();
            if (!allText.Contains("<EEG708Captions") || !allText.Contains("<Caption"))
            {
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                xml.LoadXml(allText);
            }
            catch
            {
                this._errorCount = 1;
                return;
            }

            Paragraph lastParagraph = null;
            foreach (XmlNode node in xml.DocumentElement.SelectNodes("Caption"))
            {
                try
                {
                    string start = node.Attributes["timecode"].InnerText;
                    if (lastParagraph != null)
                    {
                        lastParagraph.EndTime = DecodeTimeCode(start.Split(':'));
                    }

                    XmlNode text = node.SelectSingleNode("Text");
                    if (text != null)
                    {
                        string s = text.InnerText;
                        s = s.Replace("<br />", Environment.NewLine).Replace("<br/>", Environment.NewLine);
                        TimeCode startTime = DecodeTimeCode(start.Split(':'));
                        lastParagraph = new Paragraph(s, startTime.TotalMilliseconds, startTime.TotalMilliseconds + 3000);
                        subtitle.Paragraphs.Add(lastParagraph);
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
        /// The encode time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeTimeCode(TimeCode time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, MillisecondsToFramesMaxFrameRate(time.Milliseconds));
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="parts">
        /// The parts.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string[] parts)
        {
            int hour = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            int frames = int.Parse(parts[3]);

            return new TimeCode(hour, minutes, seconds, FramesToMillisecondsMax999(frames));
        }
    }
}