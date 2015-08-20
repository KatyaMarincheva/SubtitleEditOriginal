// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalCutProXCM.cs" company="">
//   
// </copyright>
// <summary>
//   The final cut pro xcm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The final cut pro xcm.
    /// </summary>
    public class FinalCutProXCM : SubtitleFormat
    {
        /// <summary>
        /// Gets or sets the frame rate.
        /// </summary>
        public double FrameRate { get; set; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".fcpxml";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Final Cut Pro X Chapter Marker";
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
            if (Configuration.Settings.General.CurrentFrameRate > 26)
            {
                this.FrameRate = 30;
            }
            else
            {
                this.FrameRate = 25;
            }

            string xmlStructure = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>" + Environment.NewLine + "<!DOCTYPE fcpxml>" + Environment.NewLine + Environment.NewLine + "<fcpxml version=\"1.1\">" + Environment.NewLine + "  <project name=\"Subtitle Edit subtitle\" uid=\"C1E80D31-57D4-4E6C-84F6-86A75DCB7A54\" eventID=\"B5C98F73-1D7E-4205-AEF3-1485842EB191\" location=\"file://localhost/Volumes/Macintosh%20HD/Final%20Cut%20Projects/Yma%20Sumac/Yma%20LIVE%20in%20Moscow/\" >" + Environment.NewLine + "    <resources>" + Environment.NewLine + "      <format id=\"r1\" name=\"FFVideoFormatDV720x480i5994\" frameDuration=\"2002/60000s\" fieldOrder=\"lower first\" width=\"720\" height=\"480\" paspH=\"10\" paspV=\"11\"/>" + Environment.NewLine + "      <effect id=\"r6\" name=\"Custom\" uid=\".../Titles.localized/Build In:Out.localized/Custom.localized/Custom.moti\"/>" + Environment.NewLine + "    </resources>" + Environment.NewLine + "    <sequence duration=\"10282752480/2400000s\" format=\"r1\" tcStart=\"0s\" tcFormat=\"NDF\" audioLayout=\"stereo\" audioRate=\"48k\">" + Environment.NewLine + "      <spine>" + Environment.NewLine + "        <clip offset=\"0s\" name=\"Untitled\" duration=\"147005859/24000s\" tcFormat=\"NDF\">" + Environment.NewLine + "          <video offset=\"0s\" ref=\"r6\" duration=\"147005859/24000s\">" + Environment.NewLine + "            <audio lane=\"-1\" offset=\"0s\" ref=\"r6\" duration=\"147005000/24000s\" role=\"dialog\"/>" + Environment.NewLine + "          </video>" + Environment.NewLine + "        </clip>" + Environment.NewLine + "      </spine>" + Environment.NewLine + "    </sequence>" + Environment.NewLine + "  </project>" + Environment.NewLine + "</fcpxml>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);

            XmlNode videoNode = xml.DocumentElement.SelectSingleNode("project/sequence/spine/clip");

            int number = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode chapterMarker = xml.CreateElement("chapter-marker");

                XmlAttribute attr = xml.CreateAttribute("duration");
                attr.Value = Convert.ToInt64(p.Duration.TotalSeconds * 2400000) + "/2400000s";
                chapterMarker.Attributes.Append(attr);

                attr = xml.CreateAttribute("start");
                attr.Value = Convert.ToInt64(p.StartTime.TotalSeconds * 2400000) + "/2400000s";
                chapterMarker.Attributes.Append(attr);

                attr = xml.CreateAttribute("value");
                attr.Value = p.Text.Replace(Environment.NewLine, Convert.ToChar(8232).ToString());
                chapterMarker.Attributes.Append(attr);

                attr = xml.CreateAttribute("posterOffset");
                attr.Value = "11/24s";
                chapterMarker.Attributes.Append(attr);

                videoNode.AppendChild(chapterMarker);
                number++;
            }

            string xmlAsText = ToUtf8XmlString(xml);
            xmlAsText = xmlAsText.Replace("fcpxml[]", "fcpxml");
            xmlAsText = xmlAsText.Replace("fcpxml []", "fcpxml");
            return xmlAsText;
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
            this.FrameRate = Configuration.Settings.General.CurrentFrameRate;

            StringBuilder sb = new StringBuilder();
            lines.ForEach(line => sb.AppendLine(line));
            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.PreserveWhitespace = true;
            try
            {
                xml.LoadXml(sb.ToString().Trim());

                foreach (XmlNode node in xml.SelectNodes("fcpxml/project/sequence/spine/clip/chapter-marker"))
                {
                    try
                    {
                        Paragraph p = new Paragraph();
                        p.Text = node.Attributes["value"].InnerText;
                        p.Text = p.Text.Replace(Convert.ToChar(8232).ToString(), Environment.NewLine);
                        p.StartTime = DecodeTime(node.Attributes["start"]);
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + DecodeTime(node.Attributes["duration"]).TotalMilliseconds;
                        subtitle.Paragraphs.Add(p);
                    }
                    catch
                    {
                        this._errorCount++;
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
        /// The decode time.
        /// </summary>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTime(XmlAttribute duration)
        {
            // 220220/60000s
            if (duration != null)
            {
                string[] arr = duration.Value.TrimEnd('s').Split('/');
                if (arr.Length == 2)
                {
                    return TimeCode.FromSeconds(long.Parse(arr[0]) / double.Parse(arr[1]));
                }

                if (arr.Length == 1)
                {
                    return TimeCode.FromSeconds(float.Parse(arr[0]));
                }
            }

            return new TimeCode(0, 0, 0, 0);
        }
    }
}