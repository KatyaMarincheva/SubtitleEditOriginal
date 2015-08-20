// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalCutProXml14Text.cs" company="">
//   
// </copyright>
// <summary>
//   The final cut pro xml 14 text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The final cut pro xml 14 text.
    /// </summary>
    public class FinalCutProXml14Text : SubtitleFormat
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
                return "Final Cut Pro Xml 1.4 Text";
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

            string xmlStructure = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>" + Environment.NewLine + "<!DOCTYPE fcpxml>" + Environment.NewLine + Environment.NewLine + "<fcpxml version=\"1.4\">" + Environment.NewLine + "   <resources>" + Environment.NewLine + "       <format height=\"1080\" width=\"1440\" frameDuration=\"100/2400s\" id=\"r1\"/>" + Environment.NewLine + "       <effect id=\"r2\" uid=\".../Titles.localized/Bumper:Opener.localized/Basic Title.localized/Basic Title.moti\" name=\"Basic Title\"/>" + Environment.NewLine +

                                  // "       <effect id=\"r4\" name=\"Basic Subtitles\" uid=\"~/Generators.localized/My FCPeffects - BG/BG Basic Subtitles/Basic Subtitles.motn\"/>" + Environment.NewLine +
                                  "   </resources>" + Environment.NewLine + "   <library location=\"\">" + Environment.NewLine + "       <event name=\"Title\" uid=\"15A02C43-1B7A-4CF8-8007-5C266E77A91E\">" + Environment.NewLine + "           <project name=\"SUBTITLES\" uid=\"E04A4539-1369-47C8-AC44-F459A96AC90F\">" + Environment.NewLine + "               <sequence duration=\"929s\" format=\"r1\" tcStart=\"0s\" tcFormat=\"NDF\" audioLayout=\"stereo\" audioRate=\"48k\">" + Environment.NewLine + "                   <spine>" + Environment.NewLine + "                       <gap name=\"Gap\" offset=\"0s\" duration=\"929s\" start=\"3600s\">" + Environment.NewLine + "                       </gap>" + Environment.NewLine + "                    </spine>" + Environment.NewLine + "                </sequence>" + Environment.NewLine + "            </project>" + Environment.NewLine + "        </event>" + Environment.NewLine + "    </library>" + Environment.NewLine + "</fcpxml>";

            string xmlClipStructure = "<title name=\"Basic Title: [TITLEID]\" lane=\"1\" offset=\"8665300/2400s\" ref=\"r2\" duration=\"13400/2400s\" start=\"3600s\">" + Environment.NewLine + "    <param name=\"Position\" key=\"9999/999166631/999166633/1/100/101\" value=\"-1.67499 -470.934\"/>" + Environment.NewLine + "    <text>" + Environment.NewLine + "        <text-style ref=\"ts[NUMBER]\">THE NOISEMAKER</text-style>" + Environment.NewLine + "    </text>" + Environment.NewLine + "    <text-style-def id=\"ts[NUMBER]\">" + Environment.NewLine + "        <text-style font=\"Lucida Grande\" fontSize=\"36\" fontFace=\"Regular\" fontColor=\"0.793266 0.793391 0.793221 1\" baseline=\"29\" shadowColor=\"0 0 0 1\" shadowOffset=\"5 315\" alignment=\"center\"/>" + Environment.NewLine + "    </text-style-def>" + Environment.NewLine + "</title>";

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlStructure);
            XmlNode videoNode = xml.DocumentElement.SelectSingleNode("//project/sequence/spine/gap");
            int number = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                XmlNode video = xml.CreateElement("video");
                StringBuilder trimmedTitle = new StringBuilder();
                foreach (char ch in HtmlUtil.RemoveHtmlTags(p.Text, true))
                {
                    if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Contains(ch.ToString(CultureInfo.InvariantCulture)))
                    {
                        trimmedTitle.Append(ch.ToString(CultureInfo.InvariantCulture));
                    }
                }

                string temp = xmlClipStructure.Replace("[NUMBER]", number.ToString(CultureInfo.InvariantCulture)).Replace("[TITLEID]", trimmedTitle.ToString());
                video.InnerXml = temp;

                XmlNode generatorNode = video.SelectSingleNode("title");
                if (IsNearleWholeNumber(p.StartTime.TotalSeconds))
                {
                    generatorNode.Attributes["offset"].Value = Convert.ToInt64(p.StartTime.TotalSeconds) + "s";
                }
                else
                {
                    generatorNode.Attributes["offset"].Value = Convert.ToInt64(p.StartTime.TotalSeconds * 2400000) + "/2400000s";
                }

                if (IsNearleWholeNumber(p.Duration.TotalSeconds))
                {
                    generatorNode.Attributes["duration"].Value = Convert.ToInt64(p.Duration.TotalSeconds) + "s";
                }
                else
                {
                    generatorNode.Attributes["duration"].Value = Convert.ToInt64(p.Duration.TotalSeconds * 2400000) + "/2400000s";
                }

                if (IsNearleWholeNumber(p.StartTime.TotalSeconds))
                {
                    generatorNode.Attributes["start"].Value = Convert.ToInt64(p.StartTime.TotalSeconds) + "s";
                }
                else
                {
                    generatorNode.Attributes["start"].Value = Convert.ToInt64(p.StartTime.TotalSeconds * 2400000) + "/2400000s";
                }

                XmlNode param = video.SelectSingleNode("title/text/text-style");
                param.InnerText = HtmlUtil.RemoveHtmlTags(p.Text);

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
            try
            {
                xml.LoadXml(x.Trim());

                if (subtitle.Paragraphs.Count == 0)
                {
                    foreach (XmlNode node in xml.SelectNodes("//project/sequence/spine/gap/title/text"))
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
        /// The is nearle whole number.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsNearleWholeNumber(double number)
        {
            double rest = number - Convert.ToInt64(number);
            return rest < 0.001;
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