// --------------------------------------------------------------------------------------------------------------------
// <copyright file="F4Xml.cs" company="">
//   
// </copyright>
// <summary>
//   The f 4 xml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The f 4 xml.
    /// </summary>
    public class F4Xml : F4Text
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
                return "F4 Xml";
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
            if (fileName != null && !fileName.EndsWith(this.Extension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
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
            string template = @"<?xml version='1.0' encoding='utf-8' standalone='no'?>
                                <transcript>
                                    <head mediafile=''/>
                                    <content content=''/>
                                    <style>
                                    </style>
                                </transcript>".Replace("'", "\"");
            xml.LoadXml(template);
            xml.DocumentElement.SelectSingleNode("content").Attributes["content"].Value = ToF4Text(subtitle);

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
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            string xml = sb.ToString();
            if (!xml.Contains("<transcript") || !xml.Contains("<content"))
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml(xml);
            XmlNode content = doc.DocumentElement.SelectSingleNode("content");
            if (content == null)
            {
                return;
            }

            XmlAttribute contentAttribute = content.Attributes["content"];
            if (contentAttribute == null)
            {
                return;
            }

            string text = contentAttribute.Value;
            this.LoadF4TextSubtitle(subtitle, text);
        }
    }
}