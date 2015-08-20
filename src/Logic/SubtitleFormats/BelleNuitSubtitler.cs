// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BelleNuitSubtitler.cs" company="">
//   
// </copyright>
// <summary>
//   The belle nuit subtitler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The belle nuit subtitler.
    /// </summary>
    public class BelleNuitSubtitler : SubtitleFormat
    {
        /// <summary>
        /// The regex time code.
        /// </summary>
        /// tc 00:00:35:09 00:00:38:05
        private static readonly Regex RegexTimeCode = new Regex(@"^\/tc \d\d:\d\d:\d\d:\d\d \d\d:\d\d:\d\d:\d\d", RegexOptions.Compiled);

        /// <summary>
        /// The regex file num.
        /// </summary>
        private static readonly Regex RegexFileNum = new Regex(@"^\/file\s+\d+$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".stp";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Belle Nuit Subtitler";
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
            const string paragraphWriteFormat = "/tc {0} {1}{2}{3}{2}";

            StringBuilder sb = new StringBuilder();
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format(paragraphWriteFormat, EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), Environment.NewLine, EncodeText(p.Text)));
            }

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine + @"<xmldict>
                        <key>document</key>
                        <dict>
                            <key>creator</key>
                            <string>SICT</string>
                            <key>type</key>
                            <string>STLI</string>
                            <key>version</key>
                            <real>1.4</real>
                            <key>applicationversion</key>
                            <string>Belle Nuit Subtitler 1.7.8</string>
                            <key>creationdate</key>
                            <date>2012-03-13 16:30:32</date>
                            <key>modificationdate</key>
                            <date>2012-03-13 16:30:32</date>
                        </dict>
                        <key>mainleft</key>
                        <integer>40</integer>
                        <key>maintop</key>
                        <integer>48</integer>
                        <key>mainwidth</key>
                        <integer>825</integer>
                        <key>mainheight</key>
                        <integer>886</integer>
                        <key>styledt</key>
                        <false/>
                        <key>exportdt</key>
                        <true/>
                        <key>previewdt</key>
                        <true/>
                        <key>moviedt</key>
                        <true/>
                        <key>exportformat</key>
                        <string>TIFF</string>
                        <key>style</key>
                        <dict>
                            <key>font</key>
                            <string>Geneva</string>
                            <key>size</key>
                            <integer>26</integer>
                            <key>spacing</key>
                            <real>1</real>
                            <key>leading</key>
                            <real>7</real>
                            <key>bold</key>
                            <false/>
                            <key>italic</key>
                            <false/>
                            <key>underline</key>
                            <false/>
                            <key>vertical</key>
                            <integer>486</integer>
                            <key>halin</key>
                            <integer>1</integer>
                            <key>valign</key>
                            <integer>2</integer>
                            <key>standard</key>
                            <string>PAL</string>
                            <key>height</key>
                            <integer>576</integer>
                            <key>width</key>
                            <integer>720</integer>
                            <key>widthreal</key>
                            <integer>768</integer>
                            <key>antialiasing</key>
                            <integer>4</integer>
                            <key>left</key>
                            <integer>40</integer>
                            <key>right</key>
                            <integer>680</integer>
                            <key>wrapmethod</key>
                            <integer>2</integer>
                            <key>interlaced</key>
                            <true/>
                            <key>textcolor</key>
                            <color>#FBFFF2</color>
                            <key>textalpha</key>
                            <real>1</real>
                            <key>textsoft</key>
                            <integer>0</integer>
                            <key>bordercolor</key>
                            <color>#F0F10</color>
                            <key>borderalpha</key>
                            <real>1</real>
                            <key>bordersoft</key>
                            <integer>0</integer>
                            <key>borderwidth</key>
                            <integer>6</integer>
                            <key>rectcolor</key>
                            <color>#0</color>
                            <key>rectalpha</key>
                            <real>0</real>
                            <key>rectsoft</key>
                            <integer>0</integer>
                            <key>rectform</key>
                            <integer>1</integer>
                            <key>shadowcolor</key>
                            <color>#7F7F7F</color>
                            <key>shadowalpha</key>
                            <real>0</real>
                            <key>shadowsoft</key>
                            <integer>0</integer>
                            <key>shadowx</key>
                            <integer>2</integer>
                            <key>shadowy</key>
                            <integer>2</integer>
                            <key>framerate</key>
                            <string>25</string>
                        </dict>
                        <key>folderpath</key>
                        <string></string>
                        <key>prefix</key>
                        <string></string>
                        <key>moviepath</key>
                        <string></string>
                        <key>movieoffset</key>
                        <string>00:00:00:00</string>
                        <key>moviesyncoption</key>
                        <true/>
                        <key>pagesetup</key>
                        <null/>
                        <key>titlelist</key>
                    </xmldict>");
            XmlNode node = doc.CreateElement("string");
            node.InnerText = sb.ToString().Trim() + Environment.NewLine + Environment.NewLine;
            doc.DocumentElement.AppendChild(node);

            return ToUtf8XmlString(doc).Replace("\r\n", "\n");
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

            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = null;
            try
            {
                doc.LoadXml(sb.ToString());
                if (doc.DocumentElement == null || doc.DocumentElement.Name != "xmldict" || doc.DocumentElement.SelectSingleNode("string") == null)
                {
                    return;
                }
            }
            catch (Exception)
            {
                this._errorCount = 1;
                return;
            }

            string text = null;
            string keyName = string.Empty;
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.Name == "key")
                {
                    keyName = node.InnerText;
                }
                else if (node.Name == "string" && keyName == "titlelist")
                {
                    text = node.InnerText;
                    break;
                }
            }

            if (text == null)
            {
                return;
            }

            subtitle.Paragraphs.Clear();
            Paragraph paragraph = null;
            sb = new StringBuilder();
            foreach (string line in text.Split(Utilities.NewLineChars))
            {
                if (RegexTimeCode.IsMatch(line))
                {
                    string[] parts = line.Substring(4, 11).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4)
                    {
                        try
                        {
                            if (paragraph != null && !string.IsNullOrWhiteSpace(sb.ToString()))
                            {
                                paragraph.Text = DecodeText(sb);
                            }

                            TimeCode start = DecodeTimeCode(parts);
                            parts = line.Substring(16, 11).Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            TimeCode end = DecodeTimeCode(parts);
                            paragraph = new Paragraph { StartTime = start, EndTime = end };
                            subtitle.Paragraphs.Add(paragraph);
                            sb.Clear();
                        }
                        catch
                        {
                            this._errorCount++;
                        }
                    }
                }
                else if (RegexFileNum.IsMatch(line))
                {
                }
                else if (paragraph != null)
                {
                    sb.AppendLine(line);
                }
                else
                {
                    this._errorCount++;
                }
            }

            if (paragraph != null && !string.IsNullOrWhiteSpace(sb.ToString()))
            {
                paragraph.Text = DecodeText(sb);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode text.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeText(string s)
        {
            s = HtmlUtil.RemoveOpenCloseTags(s, HtmlUtil.TagBold, HtmlUtil.TagUnderline, HtmlUtil.TagFont);
            if (s.StartsWith("{\\an3}", StringComparison.Ordinal) || s.StartsWith("{\\an6}", StringComparison.Ordinal))
            {
                s = "/STYLE RIGHT" + Environment.NewLine + s.Remove(0, 6).Trim();
            }

            if (s.StartsWith("{\\an1}", StringComparison.Ordinal) || s.StartsWith("{\\an4}", StringComparison.Ordinal))
            {
                s = "/STYLE LEFT" + Environment.NewLine + s.Remove(0, 6).Trim();
            }

            if (s.StartsWith("{\\an7}", StringComparison.Ordinal) || s.StartsWith("{\\an8}", StringComparison.Ordinal) || s.StartsWith("{\\an9}", StringComparison.Ordinal))
            {
                s = "/STYLE VERTICAL(-25)" + Environment.NewLine + s.Remove(0, 6).Trim();
            }

            if (s.StartsWith("{\\an2}", StringComparison.Ordinal) || s.StartsWith("{\\an5}", StringComparison.Ordinal))
            {
                s = s.Remove(0, 6).Trim();
            }

            return s;
        }

        /// <summary>
        /// The decode text.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string DecodeText(StringBuilder sb)
        {
            string s = sb.ToString().Trim();
            s = s.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine).Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            if (s.StartsWith("/STYLE RIGHT" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an3}" + s.Remove(0, 12).Trim();
            }

            if (s.StartsWith("/STYLE LEFT" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an1}" + s.Remove(0, 11).Trim();
            }

            if (s.StartsWith("/STYLE TOP" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 10).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-25)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-24)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-23)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-22)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-21)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-20)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-19)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an8}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-18)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-17)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-16)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-15)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-14)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-13)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-12)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-11)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            if (s.StartsWith("/STYLE VERTICAL(-10)" + Environment.NewLine, StringComparison.Ordinal))
            {
                s = "{\\an5}" + s.Remove(0, 20).Trim();
            }

            s = HtmlUtil.FixInvalidItalicTags(s);
            return s;
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