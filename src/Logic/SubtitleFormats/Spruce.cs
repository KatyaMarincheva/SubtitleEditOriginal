// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Spruce.cs" company="">
//   
// </copyright>
// <summary>
//   The spruce.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The spruce.
    /// </summary>
    public class Spruce : SubtitleFormat
    {
        /// <summary>
        /// The italic.
        /// </summary>
        private const string Italic = "^I";

        /// <summary>
        /// The bold.
        /// </summary>
        private const string Bold = "^B";

        /// <summary>
        /// The underline.
        /// </summary>
        private const string Underline = "^U";

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".stl";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Spruce Subtitle File";
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
            const string Header = @"//Font select and font size
                                    $FontName       = Arial
                                    $FontSize       = 30

                                    //Character attributes (global)
                                    $Bold           = FALSE
                                    $UnderLined     = FALSE
                                    $Italic         = FALSE

                                    //Position Control
                                    $HorzAlign      = Center
                                    $VertAlign      = Bottom
                                    $XOffset        = 0
                                    $YOffset        = 0

                                    //Contrast Control
                                    $TextContrast           = 15
                                    $Outline1Contrast       = 8
                                    $Outline2Contrast       = 15
                                    $BackgroundContrast     = 0

                                    //Effects Control
                                    $ForceDisplay   = FALSE
                                    $FadeIn         = 0
                                    $FadeOut        = 0

                                    //Other Controls
                                    $TapeOffset          = FALSE
                                    //$SetFilePathToken  = <<:>>

                                    //Colors
                                    $ColorIndex1    = 0
                                    $ColorIndex2    = 1
                                    $ColorIndex3    = 2
                                    $ColorIndex4    = 3

                                    //Subtitles";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Header);
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("{0},{1},{2}", EncodeTimeCode(p.StartTime), EncodeTimeCode(p.EndTime), EncodeText(p.Text)));
            }

            return sb.ToString();
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
            // 00:01:54:19,00:01:56:17,We should be thankful|they accepted our offer.
            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d,\d\d:\d\d:\d\d:\d\d,.+", RegexOptions.Compiled);
            if (fileName != null && fileName.EndsWith(".stl", StringComparison.OrdinalIgnoreCase))
            {
                // allow empty text if extension is ".stl"...
                regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d,\d\d:\d\d:\d\d:\d\d,", RegexOptions.Compiled);
            }

            foreach (string line in lines)
            {
                if (line.IndexOf(':') == 2 && regexTimeCodes.IsMatch(line))
                {
                    string start = line.Substring(0, 11);
                    string end = line.Substring(12, 11);

                    try
                    {
                        Paragraph p = new Paragraph(DecodeTimeCode(start), DecodeTimeCode(end), DecodeText(line.Substring(24)));
                        subtitle.Paragraphs.Add(p);
                    }
                    catch
                    {
                        this._errorCount++;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//") && !line.StartsWith('$'))
                {
                    this._errorCount++;
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The encode text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EncodeText(string text)
        {
            text = HtmlUtil.FixUpperTags(text);
            bool allItalic = text.StartsWith("<i>") && text.EndsWith("</i>") && Utilities.CountTagInText(text, "<i>") == 1;
            text = text.Replace("<b>", Bold);
            text = text.Replace("</b>", Bold);
            text = text.Replace("<i>", Italic);
            text = text.Replace("</i>", Italic);
            text = text.Replace("<u>", Underline);
            text = text.Replace("</u>", Underline);
            if (allItalic)
            {
                return text.Replace(Environment.NewLine, "|^I");
            }

            return text.Replace(Environment.NewLine, "|");
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
            // 00:01:54:19
            int frames = time.Milliseconds / (1000 / 25);

            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", time.Hours, time.Minutes, time.Seconds, frames);
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string time)
        {
            // 00:01:54:19
            string hour = time.Substring(0, 2);
            string minutes = time.Substring(3, 2);
            string seconds = time.Substring(6, 2);
            string frames = time.Substring(9, 2);

            int milliseconds = (int)((1000 / 25.0) * int.Parse(frames));
            if (milliseconds > 999)
            {
                milliseconds = 999;
            }

            TimeCode tc = new TimeCode(int.Parse(hour), int.Parse(minutes), int.Parse(seconds), milliseconds);
            return tc;
        }

        /// <summary>
        /// The decode text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string DecodeText(string text)
        {
            text = text.Replace("|", Environment.NewLine);

            // ^IBrillstein^I
            if (text.Contains(Bold))
            {
                text = DecoderTextExtension(text, Bold, "<b>");
            }

            if (text.Contains(Italic))
            {
                text = DecoderTextExtension(text, Italic, "<i>");
            }

            if (text.Contains(Underline))
            {
                text = DecoderTextExtension(text, Underline, "<u>");
            }

            return text;
        }

        /// <summary>
        /// The decoder text extension.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="SpruceTag">
        /// The spruce tag.
        /// </param>
        /// <param name="htmlOpenTag">
        /// The html open tag.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string DecoderTextExtension(string text, string SpruceTag, string htmlOpenTag)
        {
            string htmlCloseTag = htmlOpenTag.Insert(1, "/");

            int idx = text.IndexOf(SpruceTag, StringComparison.Ordinal);
            int c = Utilities.CountTagInText(text, SpruceTag);
            if (c == 1)
            {
                int l = idx + SpruceTag.Length;
                if (l < text.Length)
                {
                    text = text.Replace(SpruceTag, htmlOpenTag) + htmlCloseTag;
                }
                else if (l == text.Length)
                {
                    // Brillstein^I
                    text = text.Remove(text.Length - Italic.Length);
                }
            }
            else if (c > 1)
            {
                bool isOpen = true;
                while (idx >= 0)
                {
                    string htmlTag = isOpen ? htmlOpenTag : htmlCloseTag;
                    text = text.Remove(idx, SpruceTag.Length).Insert(idx, htmlTag);
                    isOpen = !isOpen;
                    idx = text.IndexOf(SpruceTag, idx + htmlTag.Length);
                }
            }

            return text;
        }
    }
}