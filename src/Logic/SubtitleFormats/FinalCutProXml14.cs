﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalCutProXml14.cs" company="">
//   
// </copyright>
// <summary>
//   The final cut pro xml 14.
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
    /// The final cut pro xml 14.
    /// </summary>
    public class FinalCutProXml14 : SubtitleFormat
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
                return "Final Cut Pro Xml 1.4";
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

            string xmlStructure = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>" + Environment.NewLine + "<!DOCTYPE fcpxml>" + Environment.NewLine + Environment.NewLine + "<fcpxml version=\"1.4\">" + Environment.NewLine + "   <resources>" + Environment.NewLine + "       <format id=\"r1\" name=\"FFVideoFormatDV720x480p2398_16x9\" frameDuration=\"1001/24000s\" width=\"720\" height=\"480\" paspH=\"40\" paspV=\"33\"/>" + Environment.NewLine + "       <format id=\"r2\" frameDuration=\"1001/24000s\" width=\"854\" height=\"480\"/>" + Environment.NewLine + "       <asset id=\"r3\" name=\"We Bought a Zoo\" uid=\"CD3633F9E6E39321B1E389FA4F3FE814\" src=\"file:///Volumes/Caleb%20Works/To%20Edit/We%20Bought%20a%20Zoo.mov\" start=\"0s\" duration=\"4535531/600s\" hasVideo=\"1\" format=\"r2\" hasAudio=\"1\" audioSources=\"1\" audioChannels=\"2\" audioRate=\"48000\">" + Environment.NewLine + "           <metadata>" + Environment.NewLine + "               <md key=\"com.apple.proapps.spotlight.kMDItemCodecs\">" + Environment.NewLine + "                   <array>" + Environment.NewLine + "                       <string>Apple ProRes 422 Proxy</string>" + Environment.NewLine + "                       <string>Linear PCM</string>" + Environment.NewLine + "                   </array>" + Environment.NewLine + "               </md>" + Environment.NewLine + "               <md key=\"com.apple.proapps.mio.ingestDate\" value=\"2014-07-12 15:00:41 +1200\"/>" + Environment.NewLine + "           </metadata>" + Environment.NewLine + "       </asset>" + Environment.NewLine + "       <effect id=\"r4\" name=\"Basic Subtitles\" uid=\"~/Generators.localized/My FCPeffects - BG/BG Basic Subtitles/Basic Subtitles.motn\"/>" + Environment.NewLine + "   </resources>" + Environment.NewLine + "   <library location=\"file:///Volumes/Caleb%20Works/Libraries/Subtitles.fcpbundle/\">" + Environment.NewLine + "       <event name=\"We Bought A Zoo\" uid=\"15A02C43-1B7A-4CF8-8007-5C266E77A91E\">" + Environment.NewLine + "           <project name=\"We Bought A Zoo\" uid=\"E04A4539-1369-47C8-AC44-F459A96AC90F\">" + Environment.NewLine + "               <sequence duration=\"181421240/24000s\" format=\"r1\" tcStart=\"0s\" tcFormat=\"NDF\" audioLayout=\"stereo\" audioRate=\"48k\">" + Environment.NewLine + "                    <spine>" + Environment.NewLine + "                    <clip>" + Environment.NewLine + // From here down I am unsure how it should be
                                  "                    </clip>" + Environment.NewLine + "                    </spine>" + Environment.NewLine + "                </sequence>" + Environment.NewLine + "            </project>" + Environment.NewLine + "        </event>" + Environment.NewLine + "    </library>" + Environment.NewLine + "</fcpxml>";

            string xmlClipStructure = "   <video name=\"Basic Subtitles\" lane=\"1\" offset=\"1758757/24000s\" ref=\"r4\" duration=\"240240/24000s\" start=\"86486400/24000s\">" + Environment.NewLine + "       <param name=\"Text\" key=\"9999/999166889/999166904/2/369\" value=\"\"/>" + Environment.NewLine + "   </video>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            XmlNode videoNode = xml.DocumentElement.SelectSingleNode("//project/sequence/spine/clip");
            int number = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode video = xml.CreateElement("video");
                video.InnerXml = xmlClipStructure;

                XmlNode generatorNode = video.SelectSingleNode("video");
                generatorNode.Attributes["offset"].Value = Convert.ToInt64(p.StartTime.TotalSeconds * 2400000) + "/2400000s";
                generatorNode.Attributes["duration"].Value = Convert.ToInt64(p.Duration.TotalSeconds * 2400000) + "/2400000s";
                generatorNode.Attributes["start"].Value = Convert.ToInt64(p.StartTime.TotalSeconds * 2400000) + "/2400000s";

                XmlNode param = video.SelectSingleNode("video/param");
                param.Attributes["value"].InnerText = HtmlUtil.RemoveHtmlTags(p.Text);

                videoNode.AppendChild(generatorNode);
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
            string x = sb.ToString();
            if (!x.Contains("<fcpxml version=\"1.4\">") && !x.Contains("<fcpxml version=\"1.5\">"))
            {
                return;
            }

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            try
            {
                xml.LoadXml(x.Trim());

                foreach (XmlNode node in xml.SelectNodes("//project/sequence/spine/clip/video/param[@name='Text']"))
                {
                    try
                    {
                        string text = node.Attributes["value"].InnerText;
                        Paragraph p = new Paragraph();
                        p.Text = text.Trim();
                        p.StartTime = DecodeTime(node.ParentNode.Attributes["offset"]);
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + DecodeTime(node.ParentNode.Attributes["duration"]).TotalMilliseconds;
                        subtitle.Paragraphs.Add(p);
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }

                if (subtitle.Paragraphs.Count == 0)
                {
                    foreach (XmlNode node in xml.SelectNodes("//project/sequence/spine/clip/video/title/text"))
                    {
                        try
                        {
                            string text = node.ParentNode.InnerText;
                            Paragraph p = new Paragraph();
                            p.Text = text.Trim();
                            p.StartTime = DecodeTime(node.ParentNode.Attributes["offset"]);
                            p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + DecodeTime(node.ParentNode.Attributes["duration"]).TotalMilliseconds;
                            bool add = true;
                            if (subtitle.Paragraphs.Count > 0)
                            {
                                Paragraph prev = subtitle.Paragraphs[subtitle.Paragraphs.Count - 1];
                                if (prev.Text == p.Text && prev.StartTime.TotalMilliseconds == p.StartTime.TotalMilliseconds)
                                {
                                    add = false;
                                }
                            }

                            if (add)
                            {
                                subtitle.Paragraphs.Add(p);
                            }
                        }
                        catch
                        {
                            this._errorCount++;
                        }
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