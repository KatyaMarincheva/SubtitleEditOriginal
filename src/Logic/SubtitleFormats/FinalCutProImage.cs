// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalCutProImage.cs" company="">
//   
// </copyright>
// <summary>
//   The final cut pro image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The final cut pro image.
    /// </summary>
    public class FinalCutProImage : SubtitleFormat
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
                return "Final Cut Pro Image";
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override string ToText(Subtitle subtitle, string title)
        {
            throw new NotImplementedException();
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
            try
            {
                xml.LoadXml(sb.ToString().Trim());

                foreach (XmlNode node in xml.DocumentElement.SelectNodes("sequence/media/video/track/clipitem"))
                {
                    try
                    {
                        XmlNode fileNode = node.SelectSingleNode("file");
                        if (fileNode != null)
                        {
                            XmlNode fileNameNode = fileNode.SelectSingleNode("name");
                            XmlNode filePathNode = fileNode.SelectSingleNode("pathurl");
                            if (fileNameNode != null)
                            {
                                Paragraph p = new Paragraph();
                                p.Text = fileNameNode.InnerText;
                                XmlNode inNode = node.SelectSingleNode("in");
                                XmlNode startNode = node.SelectSingleNode("start");
                                if (inNode != null)
                                {
                                    p.StartTime.TotalMilliseconds = FramesToMilliseconds(Convert.ToInt32(inNode.InnerText));
                                }
                                else if (startNode != null)
                                {
                                    p.StartTime.TotalMilliseconds = FramesToMilliseconds(Convert.ToInt32(startNode.InnerText));
                                }

                                XmlNode outNode = node.SelectSingleNode("out");
                                XmlNode endNode = node.SelectSingleNode("end");
                                if (outNode != null)
                                {
                                    p.EndTime.TotalMilliseconds = FramesToMilliseconds(Convert.ToInt32(outNode.InnerText));
                                }
                                else if (endNode != null)
                                {
                                    p.EndTime.TotalMilliseconds = FramesToMilliseconds(Convert.ToInt32(endNode.InnerText));
                                }

                                subtitle.Paragraphs.Add(p);
                            }
                        }
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
    }
}